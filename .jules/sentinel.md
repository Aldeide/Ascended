## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-06-16 - Information Disclosure via Client-Side Filtering in RPCs
**Vulnerability:** A ClientRpc decorated with `[Rpc(SendTo.Everyone)]` that relies on client-side filtering (e.g., `if (NetworkManager.LocalClientId != targetId) return;`) broadcasts sensitive data to all connected clients. A malicious client could intercept the network traffic to read data intended for other users (Information Disclosure).
**Learning:** Unity Netcode for GameObjects (NGO) broadcasts `SendTo.Everyone` RPCs to all peers regardless of internal client-side `if` checks. Never trust the client to ignore data.
**Prevention:** Always restrict sensitive data broadcast at the server level. To send targeted data, use `[Rpc(SendTo.SpecifiedInParams)]` and invoke it using `new RpcParams { Send = new RpcSendParams { Target = RpcTarget.Single(clientId, RpcTargetUse.Temp) } }`.
