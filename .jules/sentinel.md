## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-12-25 - Information Disclosure in Debugging RPC
**Vulnerability:** Debugging information sent from the server using `[Rpc(SendTo.Everyone)]` is visible to all connected clients, even though the client-side implementation immediately discarded it using a client ID check (`if (NetworkManager.LocalClientId != targetId) return;`). This introduces an Information Disclosure vulnerability, allowing malicious clients to read debug info not meant for them.
**Learning:** In Unity Netcode for GameObjects, never rely on client-side filtering to hide sensitive data broadcast via `[Rpc(SendTo.Everyone)]`, as the message reaches every client over the network.
**Prevention:** Always restrict data delivery at the server level using targeted RPCs. Specifically, use `[Rpc(SendTo.SpecifiedInParams)]` and provide targeted RpcParams (`RpcTarget.Single(clientId)`) to ensure sensitive information is sent only to the intended recipient.
