# ResoniteIPv6Mod
一个用于[Resonite](https://resonite.com/)的[ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader)模组，为会话提供透明的IPv6连接。该模组利用第三方IPv6 LNL服务器促进IPv6 UDP穿透。该模组对所有用户都是透明的，在尝试IPv6穿透后会退回到 IPv4。

ResoniteIPv6Mod包括三个设置，可通过[ModSettings](https://github.com/stiefeljackal/ResoniteModSettings)对模组进行更改。这些设置会立即生效，但不会影响任何当前会话连接，只会影响那些通过打通过程建立中的连接。

为了让Resonite使用 IPv6，双方（主机和客户端）都需要拥有 IPv6 IP 地址并安装该模块。如果没有 IPv6，该模组将无法正常运行。如果你遇到问题，请首先验证您是否拥有 IPv6 IP 地址，可以通过以下网站验证。[Test-IPv6.com](https://test-ipv6.com/). 如果你没有 IPv6 IP 地址，请联系你的互联网服务提供商寻求帮助。

在Windows和Linux客户端上可以用，也同样适用于Windows和Linux服务器客户端。


## 安装步骤

1. 安装 [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader/releases).
1. 将 [ResoniteIPv6Mod.dll](https://github.com/bontebok/ResoniteIPv6Mod/releases) 放入你的`rml_mods`文件夹。默认安装时，该文件夹位于`C:\Program Files (x86)\Steam\steamapps\common\ResoniteVR\rml_mods`中。如果缺少该文件夹，你可以创建它，或者如果你安装了ResoniteModLoader后启动游戏，它会为你创建该文件夹。
1. 启动游戏。如果想确认这个模组是否生效，你可以查看Resonite日志。


## 配置选项

|配置选项        |默认                 |说明                                                                                                      |
|----------------|---------------------|----------------------------------------------------------------------------------------------------------|
|`ipv6LnlServer` |`lnl6.razortune.com` |用于执行 IPv6 UDP 穿透的 IPv6 LNL 服务器的主机名。                                                           |
|`ipv6Only`      |`false`              |仅使用IPv6进行穿透，完全忽略 IPv4。注意，这将阻止LNL中继连接。                                                 |
|`disableMod`    |`false`              |不要进行任何IPv6连接，而是退回到标准的 Resonite 网络。                                                        |


# Thank You

* 此模组专用于因各种IPv4限制（包括严格类型 NAT 或 CGNAT（运营商级 NAT））而无法使用Resonite的用户。
* 感谢 Resonite Modding 社区为该模组的开发提供的帮助。
* 感谢[Stiefel Jackal](https://github.com/stiefeljackal)的测试和代码审查。
* 感谢[litalita](https://github.com/litalita0)提供的日语版翻译。
* 感谢[MirPASEC](https://github.com/mirpasec)提供的韩语版翻译。


# Issues

* Some IP logging may not be correct due to the nature of how a single NatPunchModule is shared for both IPv4 and IPv6, I hope to resolve this in future releases.
* If you find any issues, please report them using the issues above so that they are addressed. Pull requests welcome!
