# ResoniteIPv6Mod
[Japanese Translation](https://github.com/bontebok/ResoniteIPv6Mod/blob/main/READMEjp.md) | [Korean Translation (DeepL)](https://github.com/bontebok/ResoniteIPv6Mod/blob/main/READMEkr.md)

Un mod [ResoniteModLoader](https://github.com/zkxs/ResoniteModLoader) pour [Resonite](https://resonite.com) pour fournir une connection IPv6 transparente pour les sessions. Le mod utilise un serveur LNL tiers supportant IPv6 pour faciliter les "punch trough" UDP. Le mod devrait être transparent pour tous les utilisateurs et essaye avec IPv4 après avoir tenté un "punch trough" utilisant IPv6.

Le mod inclus trois options qui utilisent le mod [ModSettings](https://github.com/badhaloninja/ResoniteModSettings) pour permettre de les changer. Les options sont appliquées instantanément mais n'affectent pas les sessions déjà ouvertes à travers le système de "punch through".

Pour que Resonite puisse utiliser IPv6, les deux parties (hébergeur et client) doivent avoir une addresse IPv6 et avoir le mod installé. Sans IPv6, le mod ne fonctionnera pas correctement. Si vous avez des problèmes, vérifiez que vous avez une addresse IPv6; le site [test-ipv6.com](https://test-ipv6.com) peut vous montrer si vous avez une addresse IPv6. Si vous n'avez pas d'addresse IPv6, contactez votre FAI.

ResoniteIPv6Mod fonctionne sur les clients Linux et Windows et sur les clients Headless.

## Installation

1. Installez [ResoniteModLoader](https://github.com/zkxs/ResoniteModLoader).
1. Placez [ResoniteIPv6Mod](https://github.com/bontebok/ResoniteIPv6Mod/releases) dans le dossier `rml_mods`. Ce dossier devrait être dans `C:\Program Files (x86)\Steam\steamapps\common\ResoniteVR\rml_mods` pour une installation par défault. Vous pouvez créer le dossier si il n'existe pas. Vous pouvez aussi lancer le jeu avec ResoniteModLoader pour créer le dossier automatiquement.
1. Démarrez le jeu. Si vous voulez vérifier que le mod est bien installé, vous pouvez regarder les logs.


## Options

|Option   |Défaut              |Description                                                                                               |
|----------------|---------------------|----------------------------------------------------------------------------------------------------------|
|`ipv6LnlServer` |`lnl6.razortune.com` |L'hôte du serveur LNL IPv6 pour effectuer le "punch through" UDP |
|`ipv6Only`      |`false`              |Forcer l'usage d'IPv6 seulement. Cette option va désactiver la connectivité vers le relais LNL. |
|`disableMod`    |`false`              |Désactive le mod et utilise le défaut de Resonite. |



# Mercis

* Ce mod est dédicacé a tous les utilisateurs limités dans l'usage de Resonite a cause des restrictions d'IPv4, incluant NAT ou CGNAT.
* Merci à la communauté Resonite Modding pour avoir assisté au dévelopement de ce mod.
* Merci à [Stiefel Jackal](https://github.com/stiefeljackal) pour les tests et avoir révisé le code.
* Merci à [litalita](https://github.com/litalita0) pour la traduction Japonaise.
* Merci à [MirPASEC](https://github.com/mirpasec) pour la traduction Coréenne.


# Problèmes

* Les logs d'IP peuvent ne pas être correct vu qu'un seul `NatPunchModule` est partagé pour IPv4 et IPv6; sera probablement résolu dans une prochaine mise à jour.
* Si vous trouvez un problème, reportez-les en utilisant les issues. Les pull request sont bienvenues!
