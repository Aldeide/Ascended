## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-05-22 - SendTo.Everyone Information Exposure
**Vulnerability:** A `[Rpc(SendTo.Everyone)]` combined with client-side filtering (e.g., checking `LocalClientId` against a target ID) for sensitive data (like debug info) means the payload is still transmitted over the network to all clients, creating an Information Exposure vulnerability.
**Learning:** The network payload is sent to all clients regardless of client-side logic.
**Prevention:** Use `[Rpc(SendTo.SpecifiedInParams)]` for targeted delivery instead, passing an `RpcTarget` such as `RpcTarget.Single(targetId, RpcTargetUse.Temp)`.
