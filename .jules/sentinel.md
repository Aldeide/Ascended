## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-05-24 - Information Exposure via SendTo.Everyone filtering
**Vulnerability:** Information Exposure. Server debug data was transmitted to all clients using `[Rpc(SendTo.Everyone)]`, combined with a client-side filter (`if (NetworkManager.LocalClientId != targetId) return;`).
**Learning:** In Unity NGO, client-side filtering does not prevent the underlying data from being transmitted over the network to all clients. Using `SendTo.Everyone` for sensitive data meant that any connected client could inspect network traffic and extract the full debug string intended for a specific client.
**Prevention:** For targeted delivery of sensitive information, always use `[ClientRpc]` and pass a `ClientRpcParams` object specifying the `TargetClientIds` to ensure the data is only sent over the network to authorized recipients.
