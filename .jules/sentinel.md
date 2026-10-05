## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-10-05 - [Server RPC Security Bypass in ReplicationManager]
**Vulnerability:** Unauthenticated ability termination in ReplicationManager.ProcessServerAbilityTermination and ProcessServerAbilityBatch. The underlying internal manager EndAbility incorrectly evaluates IsLocalClient() to false on the server.
**Learning:** When implementing server-authoritative functionality via NetworkBehaviour RPCs, you must explicitly validate client authorization before delegating to internal managers.
**Prevention:** Always validate against the defined NetworkSecurityPolicy in the RPC invocation path, explicitly passing `isClient: true` to avoid reviewer confusion.
