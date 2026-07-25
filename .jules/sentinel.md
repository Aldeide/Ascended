## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-22 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** Sending sensitive data (like debug info) using `[Rpc(SendTo.Everyone)]` combined with client-side filtering means the payload is still transmitted to all clients. This exposes sensitive information to malicious clients who can inspect network traffic or modify the client code.
**Learning:** Client-side filtering is insecure for authorization and data visibility. All data filtering must occur on the server.
**Prevention:** For targeted delivery in Unity Netcode for GameObjects (NGO), use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` instead of broadcasting to everyone.
