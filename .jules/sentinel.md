## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-05-24 - Fix Information Exposure in Network Debug Data
**Vulnerability:** Debug data was being sent to all clients using `[Rpc(SendTo.Everyone)]` and filtered on the client-side (`if (NetworkManager.LocalClientId != targetId) return;`).
**Learning:** Using `SendTo.Everyone` for sensitive data creates an Information Exposure vulnerability because the payload is still transmitted to all clients. In Unity Netcode for GameObjects (NGO), targeted delivery should be used instead.
**Prevention:** Use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to send data only to specific clients. Avoid `[Rpc(SendTo.SpecifiedInParams)]` as it causes compilation errors in this project's NGO version.
