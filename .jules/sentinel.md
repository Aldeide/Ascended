## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-09-14 - Fix Information Exposure in Debug RPC
**Vulnerability:** Debug data containing server-authoritative state was broadcast to all clients using `[Rpc(SendTo.Everyone)]` and filtered client-side, exposing sensitive internal state to potential interception.
**Learning:** Using `[Rpc(SendTo.Everyone)]` combined with client-side filtering (e.g., checking `LocalClientId != targetId`) for sensitive data is insecure because the payload is still transmitted to all clients.
**Prevention:** For targeted delivery, use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to ensure data is only transmitted to the intended recipient. Avoid using `[Rpc(SendTo.SpecifiedInParams)]` with `RpcTarget` as they cause compilation errors in this project's NGO version.
