## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2025-02-14 - Information Exposure via SendTo.Everyone
**Vulnerability:** Sending sensitive data using `[Rpc(SendTo.Everyone)]` and filtering by `targetId` on the client exposes the payload to all clients on the network.
**Learning:** In Unity Netcode for GameObjects, `SendTo.Everyone` transmits the payload to all clients regardless of client-side filtering logic, leading to Information Exposure.
**Prevention:** Use `[Rpc(SendTo.SpecifiedInParams)]` and pass `RpcTarget.Single(targetId, RpcTargetUse.Temp)` to send sensitive data only to the intended client.
