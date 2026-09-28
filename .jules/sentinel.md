## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-09-28 - Missing RPC Authorization Check
**Vulnerability:** Client could force termination of abilities on the server (like death/stun) via ServerTryEndAbilityRpc because ReplicationManager.ProcessServerAbilityTermination and ProcessServerAbilityBatch did not validate if the client had authority to terminate the ability based on its NetworkSecurityPolicy, even though activation was properly checked.
**Learning:** In networked ability systems, you must explicitly validate both activation AND termination requests against the security policy at the RPC entry point (Replication Manager), not just rely on internal manager checks which might assume the server has full authority.
**Prevention:** Always validate client requests against explicit security policies (like HasAuthorityToTerminate) in server RPC handlers before delegating to internal state managers.
