## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-10-15 - [Missing RPC Ownership Validation]
**Vulnerability:** Server RPCs using `[Rpc(SendTo.Server)]` lacked the `RequireOwnership = true` flag, allowing authorization bypass.
**Learning:** In Unity Netcode for GameObjects (NGO), the newer `[Rpc]` attribute defaults to allowing any client to invoke the RPC unless explicitly restricted, unlike the older `[ServerRpc]` attribute.
**Prevention:** Always include `RequireOwnership = true` on `[Rpc(SendTo.Server)]` when the action should be restricted to the object's owner.
