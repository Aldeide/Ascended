## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-15 - Information Disclosure via Broadcast RPCs
**Vulnerability:** A `ClientRpc` used `SendTo.Everyone` and relied on client-side ID filtering (`if (NetworkManager.LocalClientId != targetId) return;`) to hide debug data. This broadcast sensitive debug information to all clients.
**Learning:** In Unity Netcode, `SendTo.Everyone` effectively sends the data to all clients regardless of client-side logic. Client-side filtering is merely cosmetic and a classic information disclosure risk when dealing with sensitive data (like debug logs or player stats).
**Prevention:** Always use targeted RPCs (`[Rpc(SendTo.SpecifiedInParams)]` with `RpcTarget.Single(clientId)`) to send targeted information directly to the intended client from the server, preventing data leakage over the network.
