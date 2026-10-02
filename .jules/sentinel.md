## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-10-02 - Authority Check Bypass in Server Ability Termination
**Vulnerability:** The `ProcessServerAbilityTermination` and `ProcessServerAbilityBatch` methods in `ReplicationManager.cs` did not validate whether the requesting client had the appropriate authority (via `NetworkSecurityPolicy`) to terminate an ability. This allowed clients to arbitrarily cancel abilities (like a stun or server-enforced buff) that they did not own.
**Learning:** Even though internal managers check authority (e.g. `AbilityManager.EndAbility`), those checks may falsely assume a request is from the server if they rely on `_owner.IsLocalClient()` while executing on the server context (because it's the host or server reacting to a ClientRpc/ServerRpc).
**Prevention:** Always explicitly validate client authorization at the RPC boundary or network replication layer *before* delegating to internal logic.
