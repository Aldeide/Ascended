## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-22 - Server Debug Info Exposure via RPC
**Vulnerability:** The `RequestDebugDataServerRpc` method triggered `NotifyDebugDataClientRpc` with `SendTo.Everyone`, relying on client-side filtering (`if (NetworkManager.LocalClientId != targetId) return;`) to hide sensitive debug data from other clients. This transmits the sensitive debug data payload to all clients, creating an Information Exposure vulnerability.
**Learning:** Using `[Rpc(SendTo.Everyone)]` with client-side filtering for targeted sensitive data exposes that data over the network to unintended recipients.
**Prevention:** For targeted delivery, always use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to ensure the payload is only transmitted to the intended recipient.
