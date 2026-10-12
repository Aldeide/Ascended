## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-10-12 - Ability Termination Authorization Bypass
**Vulnerability:** The `ProcessServerAbilityTermination` method in the replication manager failed to check `AbilityManager.HasAuthorityToTerminate` against the `NetworkSecurityPolicy` before executing server-side ability termination. This allows malicious clients to bypass security policies by sending termination requests for abilities they do not own or are not authorized to terminate.
**Learning:** Both activation and termination paths in networked ability systems must explicitly validate client authorization before delegating to internal managers, which often lack the network context (e.g. `isClient`) needed for security checks.
**Prevention:** Always mirror authorization validations (e.g., `HasAuthorityToActivate` vs `HasAuthorityToTerminate`) on all server-side RPC pathways that modify state, including mock objects to maintain test parity.
