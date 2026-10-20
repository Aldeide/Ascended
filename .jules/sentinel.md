## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-10-20 - Unity Netcode RpcAttribute Ownership Bypass
**Vulnerability:** The newer `[Rpc(SendTo.Server)]` attribute in Unity Netcode for GameObjects does not enforce client ownership by default. Any connected client could potentially execute these RPCs on objects they do not own, leading to authorization bypass.
**Learning:** Unlike the legacy `[ServerRpc]` which enforced ownership by default, `[Rpc]` defaults to allowing anyone to invoke it.
**Prevention:** Always explicitly set `RequireOwnership = true` on Server RPCs (e.g., `[Rpc(SendTo.Server, RequireOwnership = true)]`) unless it's a specific endpoint intended for global use.
