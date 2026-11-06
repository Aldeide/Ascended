## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-11-06 - Missing Ownership Requirement on RPCs
**Vulnerability:** Any connected client could execute `[Rpc(SendTo.Server)]` methods on objects they didn't own, leading to unauthorized actions like activating abilities or modifying equipment for other players.
**Learning:** Unity Netcode for GameObjects' newer `[Rpc]` attribute does not enforce ownership by default, unlike the older `[ServerRpc]`.
**Prevention:** Always use `[Rpc(SendTo.Server, RequireOwnership = true)]` for server-bound RPCs on player-owned objects unless global access is explicitly intended.
