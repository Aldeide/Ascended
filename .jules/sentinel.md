## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-08-12 - Information Exposure in Debug RPC
**Vulnerability:** Debug data containing sensitive system information was being sent to all clients using `[Rpc(SendTo.Everyone)]` and filtered client-side.
**Learning:** In Unity Netcode for GameObjects (NGO), using `[Rpc(SendTo.Everyone)]` with client-side filtering for sensitive data creates an Information Exposure vulnerability because the payload is transmitted over the network to all clients regardless of filtering.
**Prevention:** For targeted delivery of sensitive information, always use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` instead of relying on client-side rejection.
