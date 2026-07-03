## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-06-25 - Information Exposure via RPC Broadcast
**Vulnerability:** Sending sensitive data (like debug info) using [Rpc(SendTo.Everyone)] and relying on the client to filter it based on LocalClientId (e.g., if (NetworkManager.LocalClientId != targetId) return;) still transmits the data over the network to all clients, allowing malicious actors to intercept it.
**Learning:** Client-side filtering of network payloads does not prevent the data from being transmitted to unauthorized clients.
**Prevention:** Use [Rpc(SendTo.SpecifiedInParams)] with an RpcParams parameter to send targeted RPCs securely only to the intended client.
