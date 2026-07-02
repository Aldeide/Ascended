## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-22 - Network Information Exposure via Broadcast RPCs
**Vulnerability:** Sending sensitive data (like debug info) using `[Rpc(SendTo.Everyone)]` and filtering by client ID locally exposes the data to all connected clients over the network.
**Learning:** Client-side filtering does not prevent network transmission. Data is still broadcast and can be intercepted by modified clients or packet sniffing.
**Prevention:** Use `[Rpc(SendTo.SpecifiedInParams)]` and `RpcTarget.Single(clientId, RpcTargetUse.Temp)` to deliver targeted RPC payloads exclusively to the intended recipient.
