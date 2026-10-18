## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-10-18 - Unity NGO Authorization Bypass via Default [Rpc] Attribute
**Vulnerability:** In Unity Netcode for GameObjects (NGO), the newer `[Rpc(SendTo.Server)]` attribute defaults to `RequireOwnership = false`. This allows any connected client to arbitrarily execute ServerRPCs on network objects they do not own, leading to authorization bypass.
**Learning:** Unlike the older `[ServerRpc]` attribute which defaults to requiring ownership, the generic `[Rpc]` attribute must have ownership explicitly enforced when handling sensitive, owner-only server requests.
**Prevention:** Always use `[Rpc(SendTo.Server, RequireOwnership = true)]` for client-to-server RPCs that should only be invoked by the object's owner.
