## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-08-17 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** Using `[Rpc(SendTo.Everyone)]` combined with client-side filtering (e.g., `if (NetworkManager.LocalClientId != targetId) return;`) for sensitive data like debug info. This causes the payload to be transmitted to all clients, creating an Information Exposure vulnerability.
**Learning:** In Unity Netcode for GameObjects (NGO), the payload is still transmitted to all clients even if it's discarded locally on most of them.
**Prevention:** For targeted delivery of sensitive information, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }`. Do not use `[Rpc(SendTo.SpecifiedInParams)]` with `RpcTarget` or `RpcSendParams` in this project's version due to compilation issues.
