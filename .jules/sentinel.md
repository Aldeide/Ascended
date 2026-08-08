## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-08-08 - [Information Exposure]
**Vulnerability:** Debug RPC sends full debug data to everyone via `[Rpc(SendTo.Everyone)]`, relying on client-side filtering which doesn't stop the packet transmission, thus leaking sensitive info to unauthorized clients.
**Learning:** Using `[Rpc(SendTo.Everyone)]` with a client-side filter still transmits the payload to all clients, making it unsuitable for targeted sensitive data.
**Prevention:** Use `[ClientRpc]` and pass targeted client IDs using `ClientRpcParams` via `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` instead.
