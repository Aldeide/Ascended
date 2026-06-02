## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-06-02 - Information Disclosure via Broadcast RPCs
**Vulnerability:** Debug data was being broadcast to all clients via `[Rpc(SendTo.Everyone)]`, and filtered client-side by checking the `LocalClientId`. This meant any modified or malicious client could simply bypass the check and view sensitive server debug state (Information Disclosure).
**Learning:** Never rely on client-side filtering for sensitive data sent over the network.
**Prevention:** Always use targeted RPCs like `[Rpc(SendTo.SpecifiedInParams)]` with `RpcTarget.Single` to send sensitive data only to authorized clients at the server level.
