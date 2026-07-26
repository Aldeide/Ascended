## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-05-21 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** A `[Rpc(SendTo.Everyone)]` combined with client-side filtering (e.g. `if (NetworkManager.LocalClientId != targetId) return;`) was used to send sensitive debug information. The payload is still transmitted to all clients, creating an Information Exposure vulnerability where a malicious client can intercept data belonging to other clients or the server.
**Learning:** Client-side filtering in RPCs does not prevent network transmission. The data is still sent over the wire to all connected clients.
**Prevention:** For targeted delivery, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }`. Do not use `[Rpc(SendTo.Everyone)]` for sensitive data meant for a specific client.
