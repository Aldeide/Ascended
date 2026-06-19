## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-21 - Information Disclosure via Broadcast RPCs
**Vulnerability:** Sending sensitive data (like debug information or private stats) to all clients using [Rpc(SendTo.Everyone)] and relying on the receiving client to ignore it (e.g., if (NetworkManager.LocalClientId != targetId) return;) allows malicious clients to bypass the check and intercept data meant for others.
**Learning:** Client-side checks do not prevent the network packets from being sent to and read by the client's machine.
**Prevention:** Always restrict data transmission at the server level using targeted RPCs, such as [Rpc(SendTo.SpecifiedInParams)] with RpcTarget.Single, to ensure only the intended recipient receives the network packet.
