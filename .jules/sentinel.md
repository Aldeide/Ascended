## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-11-04 - Authorization Bypass via Missing RequireOwnership in Server RPCs
**Vulnerability:** Any connected client could execute Server RPCs on network objects they do not own because the newer Unity Netcode `[Rpc(SendTo.Server)]` attribute does not enforce ownership by default (unlike the older `[ServerRpc]`).
**Learning:** In Unity Netcode for GameObjects, `[Rpc]` attributes delegate authorization to the developer. Missing ownership checks lead to severe authorization bypass vulnerabilities where players can manipulate other players' states.
**Prevention:** Always explicitly enforce ownership using `[Rpc(SendTo.Server, RequireOwnership = true)]` for client-to-server RPCs that manipulate the object's authoritative state.
