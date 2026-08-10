## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-08-10 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** The `NotifyDebugDataClientRpc` was broadcasting sensitive debug information to all connected clients using `[Rpc(SendTo.Everyone)]`, combined with a local client ID check `if (NetworkManager.LocalClientId != targetId) return;`. This creates an Information Exposure vulnerability, as the data is transmitted over the network to all clients regardless of the client-side filtering.
**Learning:** In Unity Netcode for GameObjects (NGO), client-side filtering after broadcasting does not prevent the network transmission of data to unintended recipients.
**Prevention:** For targeted delivery of sensitive information, always use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` instead of broadcasting to everyone and filtering locally.
