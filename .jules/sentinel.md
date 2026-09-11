## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-18 - Fix Information Exposure in AbilitySystemComponent
**Vulnerability:** Information Exposure via `[Rpc(SendTo.Everyone)]`. Sensitive debug data was sent to all clients and filtered locally, allowing interception.
**Learning:** Using `[Rpc(SendTo.Everyone)]` with client-side filtering transmits the payload to all clients, violating the principle of least privilege in networking. Unity Netcode requires methods marked with `[ClientRpc]` to end with `ClientRpc` suffix. `[Rpc(SendTo.SpecifiedInParams)]` causes compilation errors in this project's NGO version.
**Prevention:** Use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to ensure targeted delivery of sensitive information over the network.
