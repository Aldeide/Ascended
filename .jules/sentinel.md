## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-09-30 - Missing Client Authority Validation on Server RPCs
**Vulnerability:** Client requests to terminate abilities via `ServerTryEndAbilityRpc` bypassed authorization checks, because the server processed the termination as a trusted server request.
**Learning:** Unity Netcode Server RPC handlers must independently validate client authority for both ability activation and termination paths before delegating to internal managers, which may misattribute the source of the action.
**Prevention:** Always validate client authorization against the defined `NetworkSecurityPolicy` using `isClient: true` inside Server RPC handlers before executing authoritative state changes.
