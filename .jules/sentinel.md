## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-07-01 - Information Exposure via RPC Broadcast
**Vulnerability:** Sending sensitive data using `[Rpc(SendTo.Everyone)]` and relying on client-side filtering (e.g., `if (LocalClientId != targetId) return;`) exposes the data to all clients over the network.
**Learning:** In Unity NGO, data sent to `SendTo.Everyone` is transmitted to all clients regardless of client-side logic.
**Prevention:** Always use `[Rpc(SendTo.SpecifiedInParams)]` with a specific target for sensitive data.
