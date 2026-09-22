## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-09-22 - Prevent Information Exposure via SendTo.Everyone
**Vulnerability:** Information Exposure. Using `[Rpc(SendTo.Everyone)]` combined with client-side filtering (`if (NetworkManager.LocalClientId != targetId) return;`) for sensitive debug data means the payload is still transmitted to all clients.
**Learning:** Unity Netcode for GameObjects (NGO) transmits data over the network to all clients designated by the send target, regardless of whether a client drops the payload upon receipt. Relying on client-side checks for data security exposes that data to packet sniffing.
**Prevention:** For targeted delivery, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to ensure the server only sends the sensitive data to the intended client.
