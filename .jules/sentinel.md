## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-06-17 - Information Disclosure via SendTo.Everyone
**Vulnerability:** `[Rpc(SendTo.Everyone)]` combined with client-side filtering (e.g., checking `LocalClientId`) allows sensitive information to be broadcast to all clients in Unity Netcode for GameObjects, creating an Information Disclosure vulnerability.
**Learning:** Client-side filtering of RPCs is a security anti-pattern because the data is already transmitted over the network and can be intercepted by a modified client or network sniffer.
**Prevention:** Always restrict data at the server level using targeted RPCs. Use `[Rpc(SendTo.SpecifiedInParams)]` with an `RpcParams` parameter containing `RpcTarget.Single(clientId)` to ensure the data is only sent to the intended recipient.
