## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2025-01-20 - [Information Exposure]
**Vulnerability:** Found `[Rpc(SendTo.Everyone)]` used to transmit sensitive debug info (`ServerDebugString`), with client-side filtering (`if (NetworkManager.LocalClientId != targetId) return;`). This exposes the data to malicious actors snooping the network payload on other clients.
**Learning:** In Unity Netcode for GameObjects (NGO), avoid using `[Rpc(SendTo.Everyone)]` combined with client-side filtering for sensitive data, as the payload is still transmitted to all clients.
**Prevention:** For targeted delivery, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }`.
