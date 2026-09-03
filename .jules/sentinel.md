## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-21 - Information Exposure via SendTo.Everyone
**Vulnerability:** Debug data containing full server state (Attributes, Effects, Abilities, Tags) was broadcasted to all clients using `[Rpc(SendTo.Everyone)]`, combined with client-side filtering. This exposes sensitive server data to malicious clients who can simply ignore the client-side check.
**Learning:** Using `[Rpc(SendTo.Everyone)]` combined with client-side filtering for sensitive data creates an Information Exposure vulnerability in Unity Netcode for GameObjects, as the payload is still transmitted to all clients.
**Prevention:** For targeted delivery of sensitive information, always use `[ClientRpc]` and pass `ClientRpcParams` with `Send = new ClientRpcSendParams { TargetClientIds = new[] { targetId } }`. Do not use `[Rpc(SendTo.SpecifiedInParams)]` as it causes compilation errors in this project's NGO version.
