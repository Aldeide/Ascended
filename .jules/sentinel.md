## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-22 - Information Exposure in RPC
**Vulnerability:** Information Exposure when using `[Rpc(SendTo.Everyone)]` combined with client-side filtering (e.g., `if (NetworkManager.LocalClientId != targetId) return;`). A malicious client could intercept the data intended for another client.
**Learning:** The `SendTo.Everyone` target transmits the payload to all connected clients. Client-side ID checks do not prevent the network transmission to other clients.
**Prevention:** For targeted delivery, always use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to ensure the server only sends the sensitive payload to the specific client.
