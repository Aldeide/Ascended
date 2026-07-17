## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-22 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** A `[Rpc(SendTo.Everyone)]` combined with client-side filtering (checking `targetId`) transmits sensitive data to all connected clients, resulting in Information Exposure.
**Learning:** Unity Netcode for GameObjects (NGO) transmits `SendTo.Everyone` payloads universally; client-side early returns only hide the data from the application logic, not the network layer.
**Prevention:** For targeted delivery, always use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to ensure the payload is only sent to the intended recipient.
