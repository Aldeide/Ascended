## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-10-09 - Authorization Bypass in Ability Termination
**Vulnerability:** Clients could bypass the network security policy (e.g., ServerOnlyTermination) and forcefully end abilities via the ServerTryEndAbilityRpc because the server delegate (`ProcessServerAbilityTermination`) didn't validate authorization before calling `EndAbility`, which relied on `_owner.IsLocalClient()` (evaluates to false on the server).
**Learning:** Unity Netcode ServerRPCs delegating to internal managers can incorrectly bypass internal authority checks if those checks depend on local environment evaluations (e.g., `IsLocalClient()`).
**Prevention:** Always explicitly validate client authorization at the RPC entry point or immediate server-side delegate before invoking internal gameplay state methods. Use named arguments (e.g., `isClient: true`) to clarify authorization flags for code review.
