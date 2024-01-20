using HarmonyLib;
using ResoniteModLoader;
using FrooxEngine;
using Elements.Core;
using SkyFrost.Base;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using LiteNetLib;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: AssemblyTitle(ResoniteIPv6Mod.BuildInfo.Name)]
[assembly: AssemblyProduct(ResoniteIPv6Mod.BuildInfo.GUID)]
[assembly: AssemblyVersion(ResoniteIPv6Mod.BuildInfo.Version)]
[assembly: AssemblyCompany("com.ruciomods")]

namespace ResoniteIPv6Mod
{
    public static class BuildInfo
    {
        public const string Name = "ResoniteIPv6Mod";
        public const string Author = "Rucio";
        public const string Version = "2.0.0";
        public const string Link = "https://github.com/bontebok/ResoniteIPv6Mod";
        public const string GUID = "com.ruciomods.resoniteipv6mod";
    }

    public class ResoniteIPv6Mod : ResoniteMod
    {
        public override string Name => BuildInfo.Name;
        public override string Author => BuildInfo.Author;
        public override string Version => BuildInfo.Version;
        public override string Link => BuildInfo.Link;

        [AutoRegisterConfigKey]
        public static readonly ModConfigurationKey<string> MATCHMAKER_EPv6 = new("ipv6LnlServer", "IPv6 LNL Server: The hostname of the IPv6 LNL Server used for performing IPv6 UDP punch through.", () => "lnl6.razortune.com");

        [AutoRegisterConfigKey]
        public static readonly ModConfigurationKey<bool> IPV6_ONLY = new("ipv6Only", "IPv6 Only: Only use IPv6 for punch through and ignore IPv4 entirely. Note, this will prevent LNL Relay connectivity.", () => false);

        [AutoRegisterConfigKey]
        public static readonly ModConfigurationKey<bool> DISABLEMOD = new("disableMod", "Disable Mod: Do not perform any IPv6 attempts and fallback to standard Resonite networking.", () => false);

        public static ModConfiguration Config;
        public override void OnEngineInit()
        {
            try
            {
                Config = GetConfiguration();
                Harmony harmony = new Harmony(BuildInfo.GUID);
                harmony.PatchAll();
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        [HarmonyPatch(typeof(NatPunchModule))]
        public class NatPunchModulePatch
        {

            private static readonly Type NatIntroduceRequestPacketType = AccessTools.Inner(typeof(NatPunchModule), "NatIntroduceRequestPacket");
            private static readonly PropertyInfo LocalPort = AccessTools.Property(typeof(NetManager), "LocalPort");
            private static readonly PropertyInfo NIRPInternal = NatIntroduceRequestPacketType.GetProperty("Internal");
            private static readonly PropertyInfo NIRPToken = NatIntroduceRequestPacketType.GetProperty("Token");
            private static readonly MethodInfo SendDelegate = AccessTools.Method(typeof(NatPunchModule), "Send").MakeGenericMethod(NatIntroduceRequestPacketType);

            [HarmonyPrefix]
            [HarmonyPatch("SendNatIntroduceRequest", new Type[] { typeof(IPEndPoint), typeof(string) })]
            public static bool SendNatIntroduceRequest(NatPunchModule __instance, IPEndPoint masterServerEndPoint, string additionalInfo,
                object ____socket)
            {
                var NatIntroduceRequestPacket = AccessTools.CreateInstance(NatIntroduceRequestPacketType);
                int port = (int)LocalPort.GetValue(____socket);
                string networkIp = LiteNetLib.NetUtils.GetLocalIp(LocalAddrType.IPv4);

                if (string.IsNullOrEmpty(networkIp) || masterServerEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    networkIp = LiteNetLib.NetUtils.GetLocalIp(LocalAddrType.IPv6);
                }

                NIRPInternal.SetValue(NatIntroduceRequestPacket, LiteNetLib.NetUtils.MakeEndPoint(networkIp, port));
                NIRPToken.SetValue(NatIntroduceRequestPacket, additionalInfo);

                SendDelegate.Invoke(__instance, new[] { NatIntroduceRequestPacket, masterServerEndPoint });

                return false;
            }
        }

        [HarmonyPatch(typeof(LNL_Listener))]
        public class ResoniteIPv6ModPatch
        {
            public static IPEndPoint _cachedMatchmakerEPv6;
            public static IPEndPoint _cachedMatchmakerEPv4;

            private static IPEndPoint MatchMakerEPv6
            {
                get
                {
                    if (_cachedMatchmakerEPv6 == null)
                        try
                        {
                            _cachedMatchmakerEPv6 = new IPEndPoint(Dns.GetHostEntry(Config.GetValue(MATCHMAKER_EPv6)).AddressList[0], 61618);
                        }
                        catch
                        {
                            _cachedMatchmakerEPv6 = null;
                        }
                    return _cachedMatchmakerEPv6;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(LNL_Listener), "GlobalAnnounceRefresh")]
            public static bool GlobalAnnounceRefresh(LNL_Listener __instance, ref bool ___initialized, ref World ____world, ref NetManager ____server)
            {
                bool ipv6only = Config.GetValue(IPV6_ONLY);
                bool disablemod = Config.GetValue(DISABLEMOD);
                if (disablemod)
                    return true;

                if (!___initialized || ____world?.SessionId == null)
                    return true; // Let the IPv4 deal with this

                if (MatchMakerEPv6 != null)
                {
                    try
                    {
                        string additionalInfo = "S;" + ____world.SessionId + ";" + LNL_Implementer.SecretAnnounceKey.Value;
                        ____server.NatPunchModule.SendNatIntroduceRequest(MatchMakerEPv6, additionalInfo);
                    }
                    catch
                    {
                        //Msg($"Exception in GlobalAnnounce: {ex?.ToString()}");
                    }
                }
                if (ipv6only)
                    return false;
                else
                    return true;
            }

            private static readonly PropertyInfo Peer = AccessTools.Property(typeof(LNL_Connection), "Peer");
            private static readonly PropertyInfo FailReason = AccessTools.Property(typeof(LNL_Connection), "FailReason");
            private static readonly MethodInfo ConnectToRelay = AccessTools.Method(typeof(LNL_Connection), "ConnectToRelay");
            private static bool? ForceRelay;

            [HarmonyPrefix]
            [HarmonyPatch(typeof(LNL_Connection), "PunchthroughConnect")]
            public static bool PunchthroughConnect(LNL_Connection __instance, Action<LocaleString> statusCallback, ref NetManager ____client, ref string ____connectionToken, ref ConnectionEvent ___ConnectionFailed,
                ref string ____nodeId, ref World ___world, ref Task __result)
            {
                bool ipv6only = Config.GetValue(IPV6_ONLY);
                bool disablemod = Config.GetValue(DISABLEMOD);
                if (disablemod)
                    return true;

                string connectionToken = ____connectionToken;
                NetManager client = ____client;
                string nodeId = ____nodeId;
                EngineSkyFrostInterface cloud = ___world?.Engine.Cloud;
                ConnectionEvent ConnectionFailed = ___ConnectionFailed;

                __result = Task.Run(async () =>
                {
                    await new ToBackground();

                    if (!ForceRelay.HasValue)
                    {
                        ForceRelay = false;
                        foreach (string commandLineArg in Environment.GetCommandLineArgs())
                        {
                            if (commandLineArg.ToLower().EndsWith("forcerelay"))
                            {
                                ForceRelay = true;
                                break;
                            }
                        }
                    }

                    List<NetworkNodeInfo> nodes = new List<NetworkNodeInfo>();
                    if (nodeId != null)
                    {
                        NetworkNodeInfo networkNodeInfo = await cloud.NetworkNodes.TryGetNodeWithRefetch(nodeId).ConfigureAwait(false);
                        if (networkNodeInfo == null)
                            Msg($"Cannot find NAT Punchthrough node: {nodeId}");
                        else
                            nodes.Add(networkNodeInfo);
                    }
                    else
                    {
                        foreach (NetworkNodeInfo node in cloud.NetworkNodes.GetNodes(NetworkNodeType.LNL_NAT, 0))
                            nodes.Add(node);
                    }

                    if (!ForceRelay.Value)
                    {
                        for (int i = 0; i < 5; ++i) // IPv6 first
                        {
                            statusCallback("World.Connection.LNL.NATPunchthrough".AsLocaleKey(null, "n", $"IPv6 {i}"));
                            Msg($"IPv6 Punchthrough attempt: {i.ToString()}");
                            client.NatPunchModule.SendNatIntroduceRequest(MatchMakerEPv6, "C;" + connectionToken);
                            await Task.Delay(TimeSpan.FromSeconds(1.0));

                            if (Peer.GetValue(__instance) != null || !client.IsRunning)
                                return; // Connected??
                        }
                        if (ipv6only)
                        {
                            Msg($"IPv6 Punchthrough failed");
                        }
                        else
                        {
                            Msg($"IPv6 Punchthrough failed, falling back to IPv4");

                            for (int i = 0; i < 5; ++i)
                            {
                                statusCallback("World.Connection.LNL.NATPunchthrough".AsLocaleKey(null, "n", $"IPv4 {i}"));
                                Msg($"IPv4 Punchthrough attempt: {i.ToString()}");

                                // new NetworkNode implementation for IPv4
                                foreach (NetworkNodeInfo networkNodeInfo in nodes)
                                    client.NatPunchModule.SendNatIntroduceRequest(networkNodeInfo.Address, networkNodeInfo.Port, "C;" + connectionToken);

                                await Task.Delay(TimeSpan.FromSeconds(1.0));

                                if (Peer.GetValue(__instance) != null || !client.IsRunning)
                                {
                                    nodes = null;
                                    return; // Connected???
                                }
                            }
                        }
                    }
                    if (!ipv6only)
                    {
                        statusCallback("World.Connection.LNL.Relay".AsLocaleKey(null, true, null));
                        Msg("IPv4 Punchthrough failed, Connecting to Relay");
                        //AccessTools.MethodDelegate<Action>(ConnectToRelay, __instance).Invoke();
                        ConnectToRelay.Invoke(__instance, new object[]{});
                    }
                    else
                    {
                        // Exausted all options, fail
                        FailReason.SetValue(__instance, "World.Error.FailedToConnect");
                        ConnectionFailed?.Invoke(__instance);
                    }
                    return;

                });
                return false;
            }
        }
    }
}