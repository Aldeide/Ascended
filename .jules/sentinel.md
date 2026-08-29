## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-22 - Information Exposure via SendTo.Everyone
**Vulnerability:** Sending sensitive payloads (like full debug string information) via `[Rpc(SendTo.Everyone)]` and filtering on the client side causes all clients to receive the data, exposing potentially sensitive logic state.
**Learning:** In Unity NGO, client-side filtering does not prevent network transmission. The payload is still broadcasted.
**Prevention:** For targeted delivery of data, use `[ClientRpc]` along with `ClientRpcParams` configuring `TargetClientIds`, ensuring only the intended client receives the payload. Avoid `[Rpc(SendTo.SpecifiedInParams)]` as it causes compilation errors in this project.
