## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-29 - Client-Side Filtering in Unity NGO RPCs
**Vulnerability:** Sending sensitive data via `[Rpc(SendTo.Everyone)]` and filtering it out on the client side using `if (NetworkManager.LocalClientId != targetId) return;` creates an Information Disclosure vulnerability. A malicious client could intercept the broadcasted packet and read sensitive debug or state data intended for other users.
**Learning:** In Unity Netcode for GameObjects, client-side conditionals do not prevent network transmission. The server pushes the data to all clients, relying on the clients to "honestly" ignore it.
**Prevention:** Never rely on client-side filtering for sensitive data. Always restrict data at the server level using targeted RPCs such as `[Rpc(SendTo.SpecifiedInParams)]` with `RpcTarget.Single(clientId, RpcTargetUse.Temp)` to ensure the data is only sent to the intended recipient's socket.
