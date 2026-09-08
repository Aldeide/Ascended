## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-30 - Information Exposure via Client-Side Filtering
**Vulnerability:** Debug data was broadcast to all clients using `[Rpc(SendTo.Everyone)]`, relying on client-side logic (`if (NetworkManager.LocalClientId != targetId) return;`) to drop it for non-targeted clients.
**Learning:** In Unity Netcode for GameObjects (NGO), using `SendTo.Everyone` means the payload is physically sent across the network to all clients, allowing malicious clients to inspect data meant for others before the local script discards it.
**Prevention:** For targeted RPC delivery, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` instead of relying on client-side ID filtering. Avoid `Rpc(SendTo.SpecifiedInParams)` as it causes compilation errors in this project's NGO version.
