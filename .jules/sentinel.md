## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-09-26 - Missing Authorization Check in Server RPC
**Vulnerability:** A missing authorization check in `ReplicationManager.ProcessServerAbilityTermination` allowed any client to invoke the server termination path and stop abilities (even ServerOnly abilities) on the server without proper permissions.
**Learning:** Unity Netcode ServerRPCs must always validate the caller's authorization against the system's security policy. The `AbilityManager.HasAuthorityToActivate` was correctly used in `ProcessServerAbilityActivation`, but `ProcessServerAbilityTermination` lacked a corresponding check with `HasAuthorityToTerminate`, assuming the client was well-behaved.
**Prevention:** Always implement symmetric authorization checks for both the activation and termination paths of any server-authoritative functionality.
