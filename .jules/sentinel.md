## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-06-10 - Information Disclosure in Debug RPC
**Vulnerability:** Sending sensitive server debug information to all clients using `[Rpc(SendTo.Everyone)]` and relying on client-side filtering (`if (NetworkManager.LocalClientId != targetId) return;`) creates an Information Disclosure vulnerability.
**Learning:** Client-side filters can be bypassed by modified clients. In Unity Netcode, data should be restricted at the server level to prevent sensitive data from being broadcast.
**Prevention:** Always use targeted RPCs (`[Rpc(SendTo.SpecifiedInParams)]` with `RpcTarget.Single`) to send sensitive or user-specific data to a single client.
