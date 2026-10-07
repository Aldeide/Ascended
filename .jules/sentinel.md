## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-10-07 - Missing explicit authorization validation in server RPCs
**Vulnerability:** A malicious client could send termination commands (via `ServerTryEndAbilityRpc`) for abilities they are not authorized to terminate. The RPC handler merely delegated the request to the internal `AbilityManager`, which evaluated `IsLocalClient` as false on the server, bypassing the client authorization check.
**Learning:** Internal component logic (e.g., `AbilityManager.EndAbility`) that relies on runtime location checks like `IsLocalClient` will misidentify forwarded client requests when executing on the server, leading to authorization bypasses if not explicitly validated at the network boundary.
**Prevention:** Always perform explicit client authorization checks (such as `HasAuthorityToTerminate(ability, isClient: true)`) in the RPC/Replication layer *before* delegating to local internal managers, using C# named arguments for clarity.
