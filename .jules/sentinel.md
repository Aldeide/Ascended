## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-30 - Client-Side Filtering vs Server-Side Targeting for Sensitive RPCs
**Vulnerability:** A network debug tool was exposing sensitive internal game data (Information Disclosure). The server used an `[Rpc(SendTo.Everyone)]` attribute to broadcast `ServerDebugString` to all clients, relying on a client-side filter (`if (NetworkManager.LocalClientId != targetId) return;`) to hide the data from unintended recipients. Packet sniffers and modified clients could easily bypass this filter and view debug data of other clients.
**Learning:** In Unity Netcode for GameObjects (NGO), relying on client-side logic to filter data broadcasted via `[Rpc(SendTo.Everyone)]` is fundamentally insecure and creates an Information Disclosure vulnerability.
**Prevention:** Always restrict data transmission at the server level using targeted RPCs. Use the `[Rpc(SendTo.SpecifiedInParams)]` attribute with an `RpcParams` parameter, and target specific clients using `RpcTarget.Single(clientId, RpcTargetUse.Temp)` when sending sensitive data.
