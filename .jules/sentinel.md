## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-06-01 - Information Disclosure in Debug RPC
**Vulnerability:** The `NotifyDebugDataClientRpc` in `AbilitySystemComponent.cs` was marked as `[Rpc(SendTo.Everyone)]`, broadcasting potentially sensitive debug information to all connected clients. It relied on client-side filtering (`if (NetworkManager.LocalClientId != targetId) return;`) to prevent display, which could be easily bypassed by a modified client, leading to an Information Disclosure vulnerability.
**Learning:** Never rely on client-side filtering to protect sensitive data sent over the network. If data is meant for a single client, it must only be sent to that specific client by the server.
**Prevention:** Use targeted RPCs (`[Rpc(SendTo.SpecifiedInParams)]` in NGO 2.x) and `RpcTarget.Single(clientId, RpcTargetUse.Temp)` from the server to send data only to the intended recipient, eliminating the need for client-side validation and preventing broadcast leaks.
