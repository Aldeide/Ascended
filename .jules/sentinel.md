## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-08-22 - Information Exposure in RPC
**Vulnerability:** Using `[Rpc(SendTo.Everyone)]` with client-side filtering for sensitive data transmits the payload to all clients over the network. Malicious clients can intercept data meant for other clients.
**Learning:** Sending data to all clients and dropping it on the client side still exposes the data in transit. Targeted RPCs must be handled at the networking level.
**Prevention:** Use targeted `[ClientRpc]` with `ClientRpcParams` setting `TargetClientIds` to ensure the server only sends the payload to the intended client.
