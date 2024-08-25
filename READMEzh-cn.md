# ResoniteIPv6Mod
一个用于[Resonite](https://resonite.com/)的[ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) 模组，为会话提供透明的 IPv6 连接。该模组利用第三方 IPv6 LNL 服务器促进 IPv6 UDP 穿透。该模块对所有用户都是透明的，在尝试 IPv6 穿越后会退回到 IPv4。

ResoniteIPv6Mod 包括三个设置，可通过 [ModSettings](https://github.com/stiefeljackal/ResoniteModSettings) 对模组进行更改。这些设置会立即生效，但不会影响任何当前会话连接，只会影响那些通过打通过程建立中的连接。

为了让 Resonite 使用 IPv6，双方（主机和客户端）都需要拥有 IPv6 IP 地址并安装该模块。如果没有 IPv6，该模组将无法正常运行。如果你遇到问题，请首先验证您是否拥有 IPv6 IP 地址，可以通过以下网站验证。[Test-IPv6.com](https://test-ipv6.com/). 如果你没有 IPv6 IP 地址，请联系你的互联网服务提供商寻求帮助。

在Windows和 Linux 客户端上可以用，也同样适用于 Windows 和 Linux 服务器客户端。


## 安装步骤

1. 安装 [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader/releases).
1. 将 [ResoniteIPv6Mod.dll](https://github.com/bontebok/ResoniteIPv6Mod/releases) 放入你的 `rml_mods` 文件夹。默认安装时，该文件夹位于 `C:\Program Files (x86)\Steam\steamapps\common\ResoniteVR\rml_mods` 中。如果缺少该文件夹，你可以创建它，或者如果你安装了 ResoniteModLoader 后启动游戏，它会为你创建该文件夹。
1. Start the game. If you want to verify that the mod is working you can check your Resonite logs.启动游戏。如果想确认这个模组是否生效，你可以查看Resonite日志。


## 配置选项

|配置选项        |默认                 |说明                                                                                                      |
|----------------|---------------------|----------------------------------------------------------------------------------------------------------|
|`ipv6LnlServer` |`lnl6.razortune.com` |用于执行 IPv6 UDP 穿透的 IPv6 LNL 服务器的主机名。                                                           |
|`ipv6Only`      |`false`              |仅使用IPv6进行穿透，完全忽略 IPv4。注意，这将阻止LNL中继连接。                                                 |
|`disableMod`    |`false`              |不要进行任何IPv6连接，而是退回到标准的 Resonite 网络。                                                        |


# Thank You

* This Mod is dedicated to the users who are limited in being able to use Resonite due to various IPv4 restrictions, including Strict-Type NAT, or CGNAT (Carrier Grade NAT).
* Thanks to the Resonite Modding Community for providing assistance in developing this mod.
* Thanks to [Stiefel Jackal](https://github.com/stiefeljackal) for testing and code review.
* Thanks to [litalita](https://github.com/litalita0) for providing the Japanese translation.
* Thanks to [MirPASEC](https://github.com/mirpasec) for providing the Korean translation.


# Issues

* Some IP logging may not be correct due to the nature of how a single NatPunchModule is shared for both IPv4 and IPv6, I hope to resolve this in future releases.
* If you find any issues, please report them using the issues above so that they are addressed. Pull requests welcome!
