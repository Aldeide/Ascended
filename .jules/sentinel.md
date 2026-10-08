## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-10-08 - Missing Authority Check on Server Ability Termination
**Vulnerability:** `ReplicationManager.ProcessServerAbilityTermination` accepts client requests to terminate abilities but did not enforce `HasAuthorityToTerminate` checks, allowing maliciously crafted RPCs to end ServerOnly abilities.
**Learning:** Functions that act as RPC bridges or managers on the server must explicitly perform authority validation before delegating to internal methods. Do not assume the caller validated authority or that internal methods (like `EndAbility`) will handle it properly, especially if they evaluate `IsLocalClient` logic.
**Prevention:** Always mirror activation security checks (like `HasAuthorityToActivate`) with termination security checks (`HasAuthorityToTerminate`) on all server-side replication entry points.
