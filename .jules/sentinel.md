## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-22 - Information Exposure in Debug Data
**Vulnerability:** The `AbilitySystemComponent` was sending sensitive debug information (the entire state of the Ability System, including Attributes, Effects, Abilities, and Tags) to *all* connected clients using `[Rpc(SendTo.Everyone)]`, and relying on client-side filtering (`if (NetworkManager.LocalClientId != targetId) return;`) to hide it.
**Learning:** Sending sensitive payload data with `SendTo.Everyone` and filtering it client-side is an Information Exposure vulnerability. The data is still transmitted over the network and can be intercepted or read by modified malicious clients.
**Prevention:** For targeted delivery of sensitive information, always use `[ClientRpc]` with `ClientRpcParams` specifying `TargetClientIds`, ensuring the data is only transmitted to the intended recipient over the network.
