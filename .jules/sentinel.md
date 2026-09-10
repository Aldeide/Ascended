## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-05-24 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** Debug data was broadcasted to all clients using `[Rpc(SendTo.Everyone)]` and filtered client-side, unnecessarily exposing sensitive data over the network.
**Learning:** In Unity Netcode for GameObjects (NGO), relying on client-side filtering with a broadcast RPC still transmits the payload to all clients, violating the principle of least privilege.
**Prevention:** For targeted delivery, always use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }`. Do not use `[Rpc(SendTo.SpecifiedInParams)]` due to compilation errors in this NGO version.
