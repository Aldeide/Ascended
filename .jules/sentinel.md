## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2025-05-22 - Information Exposure via Client-Side Filtering in Rpc
**Vulnerability:** Sending sensitive data using `[Rpc(SendTo.Everyone)]` and relying on client-side filtering (e.g., checking `LocalClientId` against a target ID) exposes the data to all clients over the network.
**Learning:** In Unity Netcode for GameObjects (NGO), payloads sent with `SendTo.Everyone` are transmitted to all connected clients, regardless of whether they process it locally.
**Prevention:** Use `[Rpc(SendTo.SpecifiedInParams)]` and pass an `RpcTarget` (like `RpcTarget.Single(targetId, RpcTargetUse.Temp)`) for targeted delivery of sensitive information.
