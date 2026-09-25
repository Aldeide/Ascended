## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-05-18 - Missing Authorization Check on Client Ability Termination
**Vulnerability:** Clients could arbitrarily terminate server-authoritative abilities (like Stun or root duration effects) because `ProcessServerAbilityTermination` blindly accepted incoming termination RPCs without verifying the ability's `NetworkSecurityPolicy` (unlike the activation path).
**Learning:** Even when abilities are properly authenticated for activation, their termination path must also be validated on the server. If a client can send an RPC to end an effect that should be server-authoritative, it's a security bypass.
**Prevention:** Always mirror activation security checks on termination RPCs. Ensure methods like `ProcessServerAbilityTermination` validate the client's request against the `NetworkSecurityPolicy` before invoking `EndAbility`.
