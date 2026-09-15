## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-28 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** The `NotifyDebugDataClientRpc` method used `[Rpc(SendTo.Everyone)]` to send debug data to all clients, relying on a client-side `LocalClientId` check to discard it. This causes sensitive debug payloads to be broadcast over the network to all clients, creating an Information Exposure vulnerability.
**Learning:** Using `[Rpc(SendTo.Everyone)]` with client-side filtering for sensitive or targeted data still transmits the payload globally in Unity Netcode for GameObjects.
**Prevention:** For targeted delivery of sensitive information, always use `[ClientRpc]` and pass `ClientRpcParams` configured with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to ensure the payload is only sent over the network to the intended recipient.
