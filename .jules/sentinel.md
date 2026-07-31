## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-07-31 - Information Exposure via Client-Side Filtering in RPCs
**Vulnerability:** The `RequestDebugDataServerRpc` sent sensitive debug data to all clients over the network using `[Rpc(SendTo.Everyone)]`, relying on client-side filtering (`if (NetworkManager.LocalClientId != targetId) return;`) to hide it.
**Learning:** In Unity Netcode for GameObjects, `[Rpc(SendTo.Everyone)]` combined with client-side filtering creates an Information Exposure vulnerability because the payload is still transmitted to all clients.
**Prevention:** For targeted delivery of sensitive data, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }`. Do not use `[Rpc(SendTo.SpecifiedInParams)]` in this project due to compilation errors with `RpcSendParams`.
