## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-10-04 - [Ability Termination Authorization Bypass]
**Vulnerability:** A missing client authorization check in `ProcessServerAbilityTermination` allowed any client to terminate any ability by exploiting a lifecycle mismatch where `AbilityManager.EndAbility` running on the server evaluated as a server request (bypassing client restriction checks).
**Learning:** When dealing with Unity Netcode (NGO) RPC handlers (Replication Managers), server-side termination logic must manually validate client constraints using `HasAuthorityToTerminate(ability, isClient: true)` before delegating to internal managers which might misuse `IsLocalClient()`/`IsServer()`.
**Prevention:** In Network Replication Managers handling client requests, always explicitly validate client authorization before passing execution to core logic that might assume local context.
