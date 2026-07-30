## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-24 - Information Exposure in Network Debug Data
**Vulnerability:** Debug data was requested via `RequestDebugDataServerRpc`, but returned using `[Rpc(SendTo.Everyone)]` and filtered client-side. This transmits potentially sensitive server/player state to all clients over the network before dropping it.
**Learning:** Do not use `[Rpc(SendTo.Everyone)]` with client-side filtering (e.g., `if (NetworkManager.LocalClientId != targetId) return;`) for targeted data delivery, as the payload still crosses the wire to unintended recipients.
**Prevention:** Use `[ClientRpc]` and pass `ClientRpcParams` configured with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }` to ensure sensitive payloads are only transmitted to authorized clients.
