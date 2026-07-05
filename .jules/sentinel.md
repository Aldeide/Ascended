## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-15 - [NGO RPC Information Exposure]
**Vulnerability:** Debug information was sent over the network to all clients using `[Rpc(SendTo.Everyone)]` and relying on client-side ID filtering, exposing sensitive data to unauthorized clients.
**Learning:** In Unity Netcode for GameObjects (NGO), client-side filtering combined with `[Rpc(SendTo.Everyone)]` does not prevent the payload from being transmitted over the network to all clients, creating an Information Exposure vulnerability.
**Prevention:** Use `[Rpc(SendTo.SpecifiedInParams)]` with an `RpcParams` parameter containing `RpcTarget.Single(targetId)` to ensure targeted delivery and prevent sensitive data from being broadcasted to all clients.
