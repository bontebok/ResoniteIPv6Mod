# ResoniteIPv6Mod

세션 연결시 IPv6를 사용할 수 있도록 하는 [ResoniteModLoader](https://github.com/zkxs/ResoniteModLoader)용 모드 입니다. 이 모드는 타사의 IPv6 LNL 서버를 활용하여 UDP 펀치 스루를 사용할 수 있게 합니다. IPv6 펀치스루 시도 후 IPv4로 되돌아 갑니다.

ResoniteIPv6Mod에는 [ModSettings](https://github.com/badhaloninja/ResoniteModSettings) 모드를 통해 설정 가능한 옵션이 3가지 있습니다. 설정 자체는 즉시 반영되지만, 현재 세션에서는 영향이 없으며, 펀치 스루 절차를 통해 연결된 세션에만 그 영향을 미칩니다.

ResoniteVR에서 IPv6를 사용하려면 호스트와 클라이언트 모두 IPv6 IP 주소 및 이 모드를 설치해야 합니다. IPv6가 없으면 이 모드가 제대로 작동하지 않습니다. 잘 동작하지 않는 경우, 먼저 IPv6 IP 주소의 사용 여부를 확인해야 합니다. [Test-IPv6.com](https://test-ipv6.com/) 사이트를 통해 확인하실 수 있습니다. IPv6 IP 주소가 없는 경우 인터넷 서비스 제공업체(ISP)에 문의하여 도움을 받으세요.

ResoniteIPv6Mod는 Windows 및 Linux 클라이언트 및 Windows 및 Linux 헤드리스 클라이언트에서도 작동합니다.


## 설치

1. [ResoniteModLoader](https://github.com/zkxs/ResoniteModLoader)를 설치합니다.
1. [ResoniteIPv6Mod.dll](https://github.com/bontebok/ResoniteIPv6Mod/releases)을 `rml_mods` 폴더에 넣습니다. 이 폴더는 보통 `C:\Program Files (x86)\Steam\steamapps\common\ResoniteVR\rml_mods`에 있어야 합니다. 이 폴더가 없는 경우 직접 만들거나 ResoniteModLoader를 설치한 상태에서 게임을 한 번 실행하면 폴더가 자동으로 생성됩니다.
1. 게임을 시작합니다.(모드가 작동하는지 확인하려면 네오스 로그를 확인합니다.)


## 구성 옵션

|구성 옵션        |기본값               |설명                                                                                                     |
|----------------|---------------------|----------------------------------------------------------------------------------------------------------|
|`ipv6LnlServer` |`lnl6.razortune.com` |IPv6 UDP 펀치 스루를 위한 IPv6 LNL 서버를 의미합니다.                                                        |
|`ipv6Only`      |`false`              |펀치 스루 수행시 IPv6만 이용합니다. 이경우 LNL 릴레이 연결이 차단됩니다.                                       |
|`disableMod`    |`false`              |IPv6 접속 시도를 하지 않고 네오스 기본 네트워크 연결 절차를 이용합니다.                                        |


# 감사의 말

* 이 모드는 엄격한 유형의 NAT 또는 CGNAT(캐리어 등급 NAT)를 포함한 다양한 IPv4 제한으로 인해 ResoniteVR 이용에 제한이 있는 사용자를 위한 모드 입니다.
* 이 모드를 개발하는 데 도움을 주신 "ResoniteVR" 모딩 커뮤니티에 감사드립니다.
* 테스트 및 코드 검토를 도와준 [Stiefel Jackal](https://github.com/stiefeljackal)에게 감사드립니다.
* 일본어 번역을 제공해 주신 [litalita](https://github.com/litalita0)에게 감사드립니다.


# 이슈

* 단일 NatPunchModule이 IPv4와 IPv6에게 공유되는 특성이 있으므로, 일부 IP 로깅이 정상적이지 않을 수 있습니다. 이 문제는 추후 개선할 것입니다.
* 문제가 발견되면 위의 이슈를 통해 신고하여 해결될 수 있도록 도와주세요. 풀 리퀘스트는 언제나 환영합니다!
