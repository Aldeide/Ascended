## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-22 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** Debug data was broadcast to all clients using `[Rpc(SendTo.Everyone)]` and filtered client-side by checking `NetworkManager.LocalClientId != targetId`. This transmits potentially sensitive server-side debug data to unauthorized clients.
**Learning:** Client-side filtering of RPCs is a security risk as malicious clients can capture network traffic.
**Prevention:** Use targeted `[ClientRpc]` delivery with `ClientRpcParams { TargetClientIds = new[] { targetId } }` to ensure sensitive data is only transmitted to the intended recipient over the network.
