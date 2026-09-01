## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-22 - Network Information Exposure via SendTo.Everyone
**Vulnerability:** Using `[Rpc(SendTo.Everyone)]` with client-side ID filtering to transmit sensitive data (like debug info or player secrets) exposes the payload to all connected clients over the network.
**Learning:** Client-side filtering (`if (NetworkManager.LocalClientId != targetId) return;`) does not prevent the network transmission itself. In Unity Netcode for GameObjects (NGO), malicious modified clients can intercept the packet before the C# filter runs.
**Prevention:** For targeted delivery of sensitive information, always use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }`. Avoid `[Rpc(SendTo.Everyone)]` unless the data is truly public. Do not use `[Rpc(SendTo.SpecifiedInParams)]` as it causes compilation issues in this NGO version.
