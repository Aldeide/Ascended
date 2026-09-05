## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-09-05 - Fix Information Exposure in Debug Data RPC
**Vulnerability:** Debug data was being sent to all clients using `[Rpc(SendTo.Everyone)]` and filtered on the client side, causing an Information Exposure vulnerability where sensitive debug data is transmitted over the network to unintended clients.
**Learning:** In Unity NGO, using `[Rpc(SendTo.Everyone)]` with client-side ID checks still transmits the payload to everyone. Targeted delivery should be used instead.
**Prevention:** For targeted delivery in NGO, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }`. Avoid `[Rpc(SendTo.Everyone)]` for sensitive data meant for a specific client.
