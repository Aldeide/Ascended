## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-10-14 - [Missing RequireOwnership in Unity Netcode RPCs]
**Vulnerability:** Found `[Rpc(SendTo.Server)]` attributes without explicit ownership validation in `EquipmentComponent` and `AbilitySystemComponent`.
**Learning:** In Unity Netcode for GameObjects (NGO), the newer `[Rpc]` attribute does not enforce ownership by default, unlike the legacy `[ServerRpc]`. This can easily lead to authorization bypasses where clients control other players' entities.
**Prevention:** Always use `[Rpc(SendTo.Server, RequireOwnership = true)]` for client-to-server commands unless explicitly designing an open-access endpoint.
