## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-05-28 - RPC Information Disclosure via Client-Side Filtering
**Vulnerability:** Sending sensitive or targeted data using `[Rpc(SendTo.Everyone)]` and filtering it client-side (e.g., `if (LocalClientId != targetId) return;`) results in Information Disclosure. Any malicious client could modify their client to ignore the filter and read data meant for other players.
**Learning:** Client-side filtering in RPCs provides no security in a server-authoritative model.
**Prevention:** Always restrict data at the server level using targeted RPCs. In Unity Netcode for GameObjects, use `[Rpc(SendTo.SpecifiedInParams)]` with an `RpcParams` parameter and call it using `RpcTarget.Single(clientId, RpcTargetUse.Temp)` to send data strictly to the intended recipient.
