## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-11-05 - Enforce Ownership on Server RPCs
**Vulnerability:** Authorization bypass allowing any connected client to execute server RPCs on objects they do not own.
**Learning:** Unlike the older `[ServerRpc]` attribute, the newer Unity Netcode `[Rpc(SendTo.Server)]` attribute does not require ownership by default, leading to potential unauthorized actions if not explicitly restricted.
**Prevention:** Always use `[Rpc(SendTo.Server, RequireOwnership = true)]` when implementing server-authoritative functionality that should only be triggered by the owner of the NetworkObject.
