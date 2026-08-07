## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-22 - Information Exposure via SendTo.Everyone
**Vulnerability:** Using `[Rpc(SendTo.Everyone)]` combined with client-side filtering (e.g., `if (NetworkManager.LocalClientId != targetId) return;`) for targeted delivery of sensitive information (like debug states). The payload is transmitted over the network to all clients, allowing a malicious client to intercept sensitive data intended for another client.
**Learning:** In Unity Netcode for GameObjects (NGO), client-side filtering does not prevent network transmission. `SendTo.Everyone` literally sends the data to everyone, creating an Information Exposure vulnerability.
**Prevention:** For targeted delivery, always use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }`. Avoid `[Rpc(SendTo.Everyone)]` for sensitive data meant for a specific client.
