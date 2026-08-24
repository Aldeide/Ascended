## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-08-24 - Information Exposure via Broadcast RPC
**Vulnerability:** Debug data was broadcast to all clients using `[Rpc(SendTo.Everyone)]` combined with client-side filtering, exposing potentially sensitive data over the network to all connected clients.
**Learning:** In Unity Netcode for GameObjects (NGO), relying on client-side filtering for sensitive data while broadcasting to everyone causes an Information Exposure vulnerability.
**Prevention:** For targeted delivery of sensitive information, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to only send the data to the intended recipient.
