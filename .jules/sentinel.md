## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-08-03 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** Using `[Rpc(SendTo.Everyone)]` combined with client-side filtering for targeted delivery (e.g., sending debug info only intended for the requesting client) causes the payload to be transmitted to all clients. A malicious client could intercept sensitive data meant for someone else.
**Learning:** In Unity Netcode for GameObjects, `SendTo.Everyone` always broadcasts the payload network-wide regardless of client-side `if (targetId != LocalClientId)` checks.
**Prevention:** For targeted delivery, always use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` instead of broadcasting and relying on client-side filtering.
