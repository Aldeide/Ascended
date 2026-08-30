## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-08-30 - Fix Information Exposure vulnerability in ClientRpc
**Vulnerability:** Information Exposure via `[Rpc(SendTo.Everyone)]` with client-side filtering for sensitive debug data.
**Learning:** Using `[Rpc(SendTo.Everyone)]` transmits the payload to all connected clients, regardless of whether they process it locally (e.g., `if (localId != targetId) return;`). This exposes sensitive information over the network.
**Prevention:** Use targeted delivery with `[ClientRpc]` and pass `ClientRpcParams` configured with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to ensure only the intended client receives the data.
