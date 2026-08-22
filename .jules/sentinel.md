## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-06-22 - Networked Information Disclosure via Client-Side Filtering
**Vulnerability:** Debug data was broadcast to all clients using `[Rpc(SendTo.Everyone)]`, relying on client-side logic (`if (NetworkManager.LocalClientId != targetId) return;`) to hide the data from non-target clients. This meant malicious clients could easily intercept and read debug info meant for others or the server by simply ignoring the client-side check.
**Learning:** Unity Netcode for GameObjects (NGO) broadcasts `SendTo.Everyone` RPCs to all connected endpoints. Client-side filtering is fundamentally insecure for sensitive data as the data is still transmitted over the network to all participants.
**Prevention:** Always restrict data at the server level using targeted RPCs. Use `[Rpc(SendTo.SpecifiedInParams)]` and pass `new RpcParams { Send = new RpcSendParams { Target = RpcTarget.Single(clientId, RpcTargetUse.Temp) } }` to ensure sensitive data is only transmitted to the intended recipient.
