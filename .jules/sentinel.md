## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-08-05 - Information Exposure in Debug RPC
**Vulnerability:** Debug data was being sent to all clients using `[Rpc(SendTo.Everyone)]` and filtered on the client side, causing sensitive server state to be exposed over the network to unintended clients.
**Learning:** Using `[Rpc(SendTo.Everyone)]` combined with client-side filtering (e.g. `if (localClientId != targetId) return;`) still transmits the payload to all clients, creating an Information Exposure vulnerability in Unity Netcode for GameObjects.
**Prevention:** For targeted delivery, always use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to ensure the network layer only transmits data to the intended recipient.
