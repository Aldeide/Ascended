## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-08-23 - Information Exposure in RPC
**Vulnerability:** `[Rpc(SendTo.Everyone)]` combined with client-side filtering (`if (LocalClientId != targetId) return;`) was used to send sensitive debug data. This transmits the payload to all clients, creating an Information Exposure vulnerability.
**Learning:** In Unity Netcode for GameObjects (NGO), relying on client-side filtering after broadcasting to everyone is insecure for targeted delivery of sensitive information.
**Prevention:** For targeted delivery, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }`. Do not use `[Rpc(SendTo.Everyone)]` for sensitive data meant for a specific client.
