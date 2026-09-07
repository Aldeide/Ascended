## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-09-07 - Information Exposure in Unity Netcode RPCs
**Vulnerability:** Debug data was being broadcast to all clients via `[Rpc(SendTo.Everyone)]` and then filtered locally based on `NetworkManager.LocalClientId`. This exposes sensitive information over the network to all clients, violating the principle of least privilege.
**Learning:** Using `[Rpc(SendTo.Everyone)]` combined with client-side filtering still transmits the payload to all clients, creating an Information Exposure vulnerability in Unity NGO.
**Prevention:** For targeted delivery, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` instead of relying on client-side ID checks. Avoid `[Rpc(SendTo.SpecifiedInParams)]` as it causes compilation errors in this version of NGO.
