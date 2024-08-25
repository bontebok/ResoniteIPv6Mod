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
        public const string Version = "4.0.0";
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

        // LNL NativeSockets is not used, commenting out for now unless this becomes a problem.
        /*
        [HarmonyPatch(typeof(NetManager))]
        public class NetManagerPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("Start", new Type[] { typeof(IPAddress), typeof(IPAddress), typeof(int), typeof(bool) })]
            public static bool Start(ref bool ___UseNativeSockets)
            {
                bool disablemod = Config.GetValue(DISABLEMOD);
                if (disablemod)
                    return true;

                Msg($"Disabling LNL NativeSockets due to IPv6 compatibility issue.");
                ___UseNativeSockets = false;
                return true;
            }
        }
        */

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
            private static readonly MethodInfo ConnectToRelay = AccessTools.Method(typeof(LNL_Connection), "ConnectToRelay", new Type[] { typeof(RelaySettings) });

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
                    RelaySettings settings = await Settings.GetActiveSettingAsync<RelaySettings>();
                    await new ToBackground();

                    List<NetworkNodeInfo> nodes = new List<NetworkNodeInfo>();
                    if (nodeId != null)
                    {
                        NetworkNodeInfo networkNodeInfo = await cloud.NetworkNodes.TryGetNodeWithRefetch(nodeId).ConfigureAwait(false);
                        if (networkNodeInfo == null)
                            Msg($"Cannot find NAT Punchthrough node for connecting to {__instance.Address}: {nodeId}");
                        else
                            nodes.Add(networkNodeInfo);
                    }
                    else
                    {
                        foreach (NetworkNodeInfo node in cloud.NetworkNodes.GetNodes(NetworkNodeType.LNL_NAT, 2, new NetworkNodePreference?(Engine.Config.NodePreference), Engine.Config.UniverseId))
                            nodes.Add(node);
                    }

                    if (!settings.AlwaysUseRelay.Value)
                    {
                        for (int i = 0; i < 5; ++i) // IPv6 first
                        {
                            statusCallback("World.Connection.LNL.NATPunchthrough".AsLocaleKey(null, "n", $"IPv6 {i}"));
                            Msg($"IPv6 Punchthrough attempt for {__instance.Address}: {i.ToString()}");
                            client.NatPunchModule.SendNatIntroduceRequest(MatchMakerEPv6, "C;" + connectionToken);
                            await Task.Delay(TimeSpan.FromSeconds(1.0));

                            if (Peer.GetValue(__instance) != null || !client.IsRunning)
                                return; // Connected??
                        }
                        if (ipv6only)
                        {
                            Msg($"IPv6 Punchthrough failed. IPv4 fallback not enabled");
                            __instance.Failed("World.Error.FailedToConnect");
                        }
                        else
                        {
                            Msg($"IPv6 Punchthrough failed, falling back to IPv4");

                            for (int i = 0; i < 5; ++i) // IPv4 next
                            {
                                statusCallback("World.Connection.LNL.NATPunchthrough".AsLocaleKey(null, "n", $"IPv4 {i}"));
                                Msg($"IPv4 Punchthrough attempt for {__instance.Address}: {i.ToString()}");

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
                        Msg($"IPv4 Punchthrough failed for {__instance.Address}, Connecting to Relay");
                        ConnectToRelay.Invoke(__instance, new object[] { settings });
                    }
                    else
                    {
                        Msg($"IPv4 not enabled. IPv4 relay connection will not be attempted");
                        __instance.Failed("World.Error.FailedToConnect");
                    }
                    return;
                });
                return false;
            }
        }
    }
}