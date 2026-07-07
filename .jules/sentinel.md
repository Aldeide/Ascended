## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2025-02-24 - Information Disclosure via Broadcasted Rpc
**Vulnerability:** A client could request sensitive debugging information from the server using `RequestDebugDataServerRpc()`, and the server would respond by broadcasting that sensitive data to all connected clients using `[Rpc(SendTo.Everyone)]`. This is a classic Information Disclosure vulnerability because clients who did not request the data and shouldn't have access to it were receiving full diagnostic server dumps.
**Learning:** In Unity Netcode for GameObjects, never rely on client-side filtering (e.g., checking `LocalClientId`) to hide sensitive data broadcast via `[Rpc(SendTo.Everyone)]`.
**Prevention:** Always restrict data at the server level using targeted RPCs. To send a targeted RPC to a specific client safely, use the `[Rpc(SendTo.SpecifiedInParams)]` attribute with an `RpcParams` parameter, and call it using `RpcTarget.Single(clientId, RpcTargetUse.Temp)`.
