## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-09-27 - Authorization Bypass in API
**Vulnerability:** The server-side RPC handler `ProcessServerAbilityTermination` lacked authorization checks, allowing any client to terminate abilities even if they didn't have authority, circumventing the intended `NetworkSecurityPolicy`.
**Learning:** Unity Netcode server RPC endpoints that manipulate state on behalf of clients must explicitly validate client authorization policies, rather than trusting the client's request blindly.
**Prevention:** Always ensure that server methods initiated by client RPCs (like `[Rpc(SendTo.Server)]`) explicitly validate the request against the relevant `NetworkSecurityPolicy` before acting.
