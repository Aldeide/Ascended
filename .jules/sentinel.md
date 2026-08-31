## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-08-31 - Rpc(SendTo.Everyone) Information Exposure
**Vulnerability:** Sending sensitive data using `[Rpc(SendTo.Everyone)]` and relying on client-side filtering (e.g. `if (LocalClientId != targetId) return;`) exposes the data to all connected clients over the network.
**Learning:** Unity Netcode sends the payload to everyone for `SendTo.Everyone`, creating an Information Exposure vulnerability.
**Prevention:** For targeted delivery, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }`. Do not use `[Rpc(SendTo.SpecifiedInParams)]` as it causes compilation errors in this project's NGO version.
