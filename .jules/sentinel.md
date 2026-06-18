## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-06-25 - Information Disclosure via Broadcast RPCs
**Vulnerability:** A `ServerRpc` was responding to client requests by gathering sensitive debug data and sending it back using a `[Rpc(SendTo.Everyone)]` attribute, but filtering it client-side `if (NetworkManager.LocalClientId != targetId) return;`. This causes the server to broadcast the sensitive data to all connected clients, allowing malicious clients to simply ignore the check and inspect data meant for someone else.
**Learning:** In Unity Netcode for GameObjects (NGO) 2.x, do not rely on client-side filtering (e.g., checking `LocalClientId`) to hide sensitive data broadcast via `[Rpc(SendTo.Everyone)]`, as it creates an Information Disclosure vulnerability.
**Prevention:** Always restrict data at the server level using targeted RPCs. Use the `[Rpc(SendTo.SpecifiedInParams)]` attribute combined with an `RpcParams` argument, and invoke it by passing `new RpcParams { Send = new RpcSendParams { Target = RpcTarget.Single(clientId, RpcTargetUse.Temp) } }`.
