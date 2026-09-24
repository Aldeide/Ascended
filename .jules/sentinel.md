## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-09-24 - Targeted RPC Delivery
**Vulnerability:** Information Exposure. Using `[Rpc(SendTo.Everyone)]` combined with client-side filtering (`if (localId != targetId) return;`) for sensitive debug data transmits the payload to all connected clients, allowing malicious clients to intercept data not meant for them.
**Learning:** Unity Netcode's `SendTo.Everyone` always broadcasts the payload over the network. Client-side filtering only prevents the local logic from executing, but does not stop the network transmission of potentially sensitive data.
**Prevention:** For targeted delivery of sensitive information, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to ensure the data is only transmitted to the intended recipient over the network.
