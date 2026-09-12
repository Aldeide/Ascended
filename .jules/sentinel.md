## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-09-12 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** Sensitive data (full debug information) was sent using `[Rpc(SendTo.Everyone)]` with client-side filtering (`if (NetworkManager.LocalClientId != targetId) return;`), exposing the payload to all clients over the network.
**Learning:** In Unity Netcode for GameObjects, `SendTo.Everyone` blindly broadcasts the payload. Client-side return statements do not prevent the network transmission, creating an Information Exposure vulnerability.
**Prevention:** Always use `[ClientRpc]` with `ClientRpcParams` containing `TargetClientIds` for targeted data delivery to prevent sensitive information from being broadcasted to all connected clients. Avoid `[Rpc(SendTo.SpecifiedInParams)]` due to current NGO version compilation errors.
