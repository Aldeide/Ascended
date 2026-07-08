## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-07-08 - Information Exposure in Debug Data RPC
**Vulnerability:** The NotifyDebugDataClientRpc used [Rpc(SendTo.Everyone)] with a client-side filter. This means sensitive internal debug information (attributes, effects, abilities, tags) was being sent over the network to all connected clients, allowing malicious clients to intercept other players' private debug data.
**Learning:** Using [Rpc(SendTo.Everyone)] combined with a local ID check is an anti-pattern for targeted communication in Netcode for GameObjects and creates an Information Exposure vulnerability.
**Prevention:** Always use [Rpc(SendTo.SpecifiedInParams)] and pass the target client ID within RpcSendParams when data should only be accessible by a specific client.
