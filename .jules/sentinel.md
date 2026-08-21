## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-08-21 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** Sensitive data (debug info) was being sent to all clients using `[Rpc(SendTo.Everyone)]`, but then filtered client-side. The payload was still transmitted to everyone, creating an Information Exposure vulnerability.
**Learning:** In Unity Netcode for GameObjects (NGO), using `[Rpc(SendTo.Everyone)]` with client-side filtering does not prevent data transmission to other clients. Also, `[Rpc(SendTo.SpecifiedInParams)]` with `RpcTarget` or `RpcSendParams` causes compilation errors in this project's NGO version.
**Prevention:** For targeted delivery of sensitive data, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }`.
