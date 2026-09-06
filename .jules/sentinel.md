## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-07-26 - Information Exposure via SendTo.Everyone RPC
**Vulnerability:** Debug data (or other sensitive state) was being broadcast to all clients using `[Rpc(SendTo.Everyone)]` and filtered client-side (`if (NetworkManager.LocalClientId != targetId) return;`), exposing potentially sensitive information to malicious clients analyzing network traffic.
**Learning:** In Unity Netcode for GameObjects (NGO), client-side filtering does not prevent the data from being sent over the network to all clients, creating an Information Exposure vulnerability.
**Prevention:** For targeted delivery, always use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to ensure the server only sends the payload to the intended recipient.
