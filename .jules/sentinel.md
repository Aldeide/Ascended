## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-09-29 - Missing Authority Checks on Server RPCs for Ability Termination
**Vulnerability:** The `ReplicationManager` was processing client-requested ability terminations (`ProcessServerAbilityTermination` and `ProcessServerAbilityBatch`) without validating if the client had the required `NetworkSecurityPolicy` authority to terminate those abilities. A malicious client could send termination RPCs for any active ability on the server, resulting in an unauthorized termination of abilities.
**Learning:** While the client-side `EndAbility` call in `AbilityManager` correctly checks `HasAuthorityToTerminate` before sending the RPC, the server must also independently verify this authority when receiving the RPC to prevent spoofed requests.
**Prevention:** Always validate client authorization against the defined `NetworkSecurityPolicy` on both activation and termination paths in server-authoritative components to prevent security bypasses.
