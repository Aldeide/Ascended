## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-21 - Information Disclosure via SendTo.Everyone
**Vulnerability:** Debug information was being broadcast to all clients using `[Rpc(SendTo.Everyone)]` and filtered on the client side using `NetworkManager.LocalClientId`. This allowed malicious clients to inspect network traffic to read sensitive server state.
**Learning:** Client-side filtering is never sufficient for hiding sensitive data over network RPCs, as all clients still receive the payload.
**Prevention:** Always restrict data at the server level using targeted RPCs, such as `[Rpc(SendTo.SpecifiedInParams)]` with `RpcTarget.Single`, to ensure sensitive data is only sent to the intended recipient.
