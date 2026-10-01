## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-10-01 - Ability System Network Security Bypass
**Vulnerability:** The server blindly accepted ability activation and termination requests from clients over RPCs without validating if the client had authority for that specific ability.
**Learning:** In Unity Netcode, internal managers (like AbilityManager) may rely on checks like IsLocalClient() that evaluate to false on the server. If the RPC entry points in ReplicationManager don't explicitly validate the client's authority via the NetworkSecurityPolicy, clients can bypass intended restrictions.
**Prevention:** Always explicitly validate client authorization against the NetworkSecurityPolicy using `isClient: true` on the server before delegating to internal managers for processing client requests.
