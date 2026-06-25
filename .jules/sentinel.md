## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-06-25 - RPC Information Disclosure via SendTo.Everyone
**Vulnerability:** Sending sensitive data (like `ServerDebugString`) using `[Rpc(SendTo.Everyone)]` combined with client-side filtering (`NetworkManager.LocalClientId != targetId`) creates an Information Disclosure vulnerability, as all clients still receive the payload over the network.
**Learning:** Client-side filtering in RPCs never prevents network transmission and cannot protect sensitive data in Unity Netcode.
**Prevention:** Always use targeted RPCs (`[Rpc(SendTo.SpecifiedInParams)]` with `RpcTarget.Single`) to restrict data transmission at the server level.
