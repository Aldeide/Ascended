## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2025-02-12 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** Sending sensitive data (like debug info) using `[Rpc(SendTo.Everyone)]` while relying on client-side filtering (`if (NetworkManager.LocalClientId != targetId) return;`) results in the payload being broadcast to all connected clients over the network.
**Learning:** Client-side checks do not prevent network transmission. All clients still receive the data packets, which can be sniffed or modified by a malicious client.
**Prevention:** For targeted delivery of sensitive information, always use `[ClientRpc]` and pass a `ClientRpcParams` object specifying the `TargetClientIds`. Avoid `[Rpc(SendTo.Everyone)]` for anything other than true broadcasts.
