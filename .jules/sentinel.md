## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-10-22 - NGO Server RPC Authorization Bypass
**Vulnerability:** Unity Netcode for GameObjects (NGO) uses `[Rpc(SendTo.Server)]` for server RPCs, but unlike the older `[ServerRpc]`, it does not require ownership by default. This allows any connected client to execute RPCs on objects they don't own, leading to an authorization bypass.
**Learning:** The newer `[Rpc]` attribute defaults to allowing anyone to call it if it targets the server, which is a significant change from `[ServerRpc]`.
**Prevention:** Always explicitly enforce ownership using `[Rpc(SendTo.Server, RequireOwnership = true)]` to prevent connected clients from executing RPCs on objects they do not own.
