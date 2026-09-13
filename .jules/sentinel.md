## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2023-10-27 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** Information Exposure. Sensitive debug data was being broadcasted to all clients using `[Rpc(SendTo.Everyone)]` and filtered client-side, exposing internal state to unauthorized clients.
**Learning:** In Unity Netcode for GameObjects (NGO), using `[Rpc(SendTo.Everyone)]` combined with client-side filtering for sensitive data still transmits the payload to all clients, creating an Information Exposure vulnerability. Additionally, `[Rpc(SendTo.SpecifiedInParams)]` causes compilation errors in this project.
**Prevention:** For targeted delivery, always use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }`.
