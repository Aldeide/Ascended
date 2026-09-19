## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-06-25 - Prevent Information Exposure via SendTo.Everyone
**Vulnerability:** Debug data containing full stats (Attributes, Effects, Abilities, Tags) was broadcasted to all clients using `[Rpc(SendTo.Everyone)]` instead of specifically to the requesting client.
**Learning:** Using `[Rpc(SendTo.Everyone)]` with a client-side filter (`if (NetworkManager.LocalClientId != targetId) return;`) still transmits the entire payload over the network to all clients, creating an Information Exposure vulnerability in Unity Netcode.
**Prevention:** Always use `[ClientRpc]` with `ClientRpcParams` (`Send = new ClientRpcSendParams { TargetClientIds = ... }`) for sensitive data that should only be seen by specific clients, rather than filtering on the receiving end.
