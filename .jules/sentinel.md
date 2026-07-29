## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-07-29 - Fixed Information Exposure in AbilitySystem Debug RPC
**Vulnerability:** Found `[Rpc(SendTo.Everyone)]` used alongside `if (NetworkManager.LocalClientId != targetId) return;`. This causes sensitive debug data to be broadcast to all connected clients over the network before it's filtered client-side, leading to information exposure.
**Learning:** In Unity NGO, client-side filtering does not prevent network transmission. `[Rpc(SendTo.Everyone)]` always transmits to everyone, exposing data.
**Prevention:** For targeted delivery of sensitive information, always use `[ClientRpc]` with `ClientRpcParams` containing explicit `TargetClientIds` to ensure only the intended recipient receives the packet.
