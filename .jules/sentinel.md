## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-08-27 - Information Exposure via SendTo.Everyone
**Vulnerability:** `NotifyDebugDataClientRpc` used `[Rpc(SendTo.Everyone)]` with a client-side filter (`if (NetworkManager.LocalClientId != targetId) return;`). This caused the debug payload to be broadcast over the network to all connected clients, creating an Information Exposure vulnerability where malicious clients could inspect packets intended for others.
**Learning:** In Unity Netcode for GameObjects, client-side filtering of `SendTo.Everyone` RPCs does not prevent the data from being transmitted to unintended clients.
**Prevention:** For targeted delivery, use `[ClientRpc]` with `ClientRpcParams` (`Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }`) to ensure the payload is only sent to the specific client.
