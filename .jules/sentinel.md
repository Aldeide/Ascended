## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-10-19 - Missing RequireOwnership in Unity Netcode RPCs
**Vulnerability:** Any connected client can invoke `[Rpc(SendTo.Server)]` methods on objects they do not own, leading to severe authorization bypasses (e.g. activating abilities or equipping items on other players).
**Learning:** Unity Netcode's newer `[Rpc]` attribute defaults to allowing anyone to call it, unlike the older `[ServerRpc]` which enforced ownership by default.
**Prevention:** Always use `[Rpc(SendTo.Server, RequireOwnership = true)]` unless the RPC explicitly must be called by non-owners, in which case custom server-side validation is required.
