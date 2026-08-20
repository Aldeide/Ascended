## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-08-20 - Information Exposure with Rpc(SendTo.Everyone)
**Vulnerability:** Using `[Rpc(SendTo.Everyone)]` and subsequently checking `NetworkManager.LocalClientId != targetId` transmits the data payload to all connected clients. For sensitive information like debug data, this exposes internal data over the network to unintended recipients, violating the principle of least privilege.
**Learning:** Unity NGO `SendTo.Everyone` always broadcasts the payload over the network. Client-side filtering only prevents the method's logic from running, it does not prevent the network transmission itself.
**Prevention:** For targeted delivery of data, use standard `[ClientRpc]` with `ClientRpcParams` containing `TargetClientIds`, ensuring only the intended client receives the payload over the network. Avoid `[Rpc(SendTo.SpecifiedInParams)]` as it causes compilation errors in this project.
