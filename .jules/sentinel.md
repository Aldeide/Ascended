## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-10-24 - Information Exposure via [Rpc(SendTo.Everyone)]
**Vulnerability:** Debug data was broadcast to all clients via `[Rpc(SendTo.Everyone)]` and then filtered locally. This exposes potentially sensitive debug data to all clients over the network.
**Learning:** Relying on client-side filtering for network messages still transmits the payload to all clients. `[Rpc(SendTo.SpecifiedInParams)]` causes compilation errors in this project, so `[ClientRpc]` with `ClientRpcParams` must be used for targeted delivery.
**Prevention:** For sensitive data, use `[ClientRpc]` with `ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to ensure it's only transmitted over the network to the intended client.
