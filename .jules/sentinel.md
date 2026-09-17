## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-22 - Networked Information Exposure
**Vulnerability:** A `[Rpc(SendTo.Everyone)]` attribute combined with client-side filtering (`if (NetworkManager.LocalClientId != targetId) return;`) was used to send sensitive debug data. This caused the payload to be transmitted to all clients, creating an Information Exposure vulnerability where malicious clients could intercept data meant for others.
**Learning:** Client-side filtering in RPCs does not prevent network transmission to all clients when using `SendTo.Everyone`.
**Prevention:** Always use targeted delivery for sensitive information, such as `[ClientRpc]` with `ClientRpcParams` containing specific `TargetClientIds`.
