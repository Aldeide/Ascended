## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2025-01-08 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** Sending sensitive data (like full debug info) using `[Rpc(SendTo.Everyone)]` and filtering the delivery on the client side exposes the data to all connected clients over the network.
**Learning:** Even if a client filters the data locally, the payload is still transmitted across the network to everyone, presenting an Information Exposure risk.
**Prevention:** To send data to a specific client securely, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }`. Do not use `[Rpc(SendTo.Everyone)]` combined with client-side filtering for sensitive data.
