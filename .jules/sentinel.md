## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-06-21 - Information Disclosure via SendTo.Everyone RPC
**Vulnerability:** In Unity Netcode for GameObjects, using `[Rpc(SendTo.Everyone)]` combined with client-side filtering (e.g., `if (LocalClientId != targetId) return;`) creates an Information Disclosure vulnerability. Sensitive data broadcast this way can be intercepted by manipulated clients listening to network traffic.
**Learning:** Client-side filtering is ineffective for restricting sensitive data over the network.
**Prevention:** Always restrict data at the server level using targeted RPCs, such as `[Rpc(SendTo.SpecifiedInParams)]` with `RpcTarget.Single`, to securely transmit information to specific clients.
