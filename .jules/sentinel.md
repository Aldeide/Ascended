## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-07-21 - Fix Information Exposure in Network Rpc
**Vulnerability:** The `NotifyDebugDataClientRpc` method used `[Rpc(SendTo.Everyone)]` and performed client-side filtering (`if (NetworkManager.LocalClientId != targetId) return;`). This means the server broadcasted sensitive debug info to all clients over the network, exposing it even if discarded by the client.
**Learning:** In Unity Netcode for GameObjects (NGO), combining `[Rpc(SendTo.Everyone)]` with client-side filtering still transmits the payload to everyone, causing an Information Exposure vulnerability.
**Prevention:** For targeted delivery, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to send data exclusively to the intended recipient.
