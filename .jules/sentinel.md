## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-05-24 - RPC Authorization Bypass
**Vulnerability:** Any client could call Server RPCs on objects they didn't own, such as triggering abilities or changing equipment for other players.
**Learning:** Unity Netcode for GameObjects (NGO)'s newer `[Rpc(SendTo.Server)]` attribute does not require ownership by default, unlike the older `[ServerRpc]`.
**Prevention:** Explicitly enforce ownership using `[Rpc(SendTo.Server, RequireOwnership = true)]` for any Server RPCs that should only be executed by the owner of the object.
