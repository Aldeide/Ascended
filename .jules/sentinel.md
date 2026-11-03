## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## $(date +%Y-%m-%d) - Secure RPC Usage by Enforcing Ownership

**Vulnerability:** Several Unity Netcode `[Rpc(SendTo.Server)]` methods in `AbilitySystemComponent.cs` and `EquipmentComponent.cs` did not require ownership, allowing any connected client to bypass authorization and trigger functionality (like activating abilities or modifying equipment) on objects they don't own.
**Learning:** In Unity Netcode for GameObjects (NGO), the newer `[Rpc]` attribute does not require ownership by default, unlike the older `[ServerRpc]`. Default behavior must be explicitly overridden with `RequireOwnership = true`.
**Prevention:** Always define `[Rpc(SendTo.Server, RequireOwnership = true)]` for server-authoritative functionality to prevent authorization bypasses, unless explicitly intended otherwise (e.g. matchmaking).
