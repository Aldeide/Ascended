## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2025-02-12 - Information Exposure via SendTo.Everyone
**Vulnerability:** Debug data was sent over the network using `[Rpc(SendTo.Everyone)]` and filtered client-side (`if (NetworkManager.LocalClientId != targetId) return;`). This exposed sensitive data to all connected clients, allowing malicious clients to intercept it.
**Learning:** Client-side filtering in RPCs does not prevent network transmission to those clients. Using `SendTo.Everyone` for targeted sensitive data is an Information Exposure vulnerability in Unity Netcode.
**Prevention:** For targeted delivery of sensitive information, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to ensure the data is only transmitted to the intended recipient. Do not use `[Rpc(SendTo.SpecifiedInParams)]` in this NGO version due to API compatibility issues.
