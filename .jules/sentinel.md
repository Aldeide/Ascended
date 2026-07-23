## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-05-24 - Information Exposure in RPC
**Vulnerability:** A `[Rpc(SendTo.Everyone)]` method was used to transmit sensitive debug data, relying on a client-side check (`NetworkManager.LocalClientId != targetId`) to filter the output. The payload is still transmitted over the network to all clients, allowing potential exposure of sensitive information.
**Learning:** Combining `[Rpc(SendTo.Everyone)]` with client-side filtering for sensitive data creates an Information Exposure vulnerability.
**Prevention:** Use targeted delivery such as `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to securely transmit data only to the intended recipient.
