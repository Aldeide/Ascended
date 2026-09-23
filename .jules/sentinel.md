## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-09-23 - Prevent Information Exposure with Targeted ClientRpc
**Vulnerability:** Information Exposure. In Unity Netcode for GameObjects (NGO), using `[Rpc(SendTo.Everyone)]` combined with client-side filtering (e.g. `if (NetworkManager.LocalClientId != targetId) return;`) for sensitive data still transmits the payload to all clients, allowing potential interception of debug info or other sensitive data.
**Learning:** Using `SendTo.Everyone` for targeted messaging is a security anti-pattern because the network packet is broadcasted broadly, and filtering happens at the application layer on the client.
**Prevention:** For targeted delivery of sensitive information, always use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to ensure the server only sends the packet to the intended recipient. Avoid `[Rpc(SendTo.SpecifiedInParams)]` as it can cause compilation errors with base types in this NGO version.
