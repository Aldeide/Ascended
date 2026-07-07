## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-06-11 - Information Disclosure via SendTo.Everyone
**Vulnerability:** Sending sensitive data (like debug information or targeted player states) using `[Rpc(SendTo.Everyone)]` and filtering it client-side (e.g., `if (NetworkManager.LocalClientId != targetId) return;`) broadcasts the information to all connected clients, allowing malicious clients or packet sniffers to intercept data they shouldn't see.
**Learning:** Client-side filtering of `SendTo.Everyone` RPCs is fundamentally insecure for sensitive data in Unity Netcode for GameObjects.
**Prevention:** Always restrict data transmission at the server level using targeted RPCs, such as `[Rpc(SendTo.SpecifiedInParams)]` with `RpcTarget.Single(clientId, RpcTargetUse.Temp)` when sending data meant for a specific client.
