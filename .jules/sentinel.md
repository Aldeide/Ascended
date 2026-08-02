## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-08-02 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** Sending sensitive data (like full server debug state) using `[Rpc(SendTo.Everyone)]` and filtering the data client-side (`if (NetworkManager.LocalClientId != targetId) return;`) results in the payload still being transmitted over the network to all clients, allowing malicious actors to intercept data intended for others.
**Learning:** Unity Netcode for GameObjects (NGO) transmits the RPC payload to all targets defined by the `SendTo` scope regardless of internal method logic.
**Prevention:** For targeted data delivery, always use `[ClientRpc]` and pass `ClientRpcParams` configuring `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` so the server only sends the payload to the intended recipient.
