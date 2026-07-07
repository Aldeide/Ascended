## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-05-27 - Information Disclosure via SendTo.Everyone filtering
**Vulnerability:** Debug information was sent using `[Rpc(SendTo.Everyone)]` combined with a client-side check (`if (LocalClientId != targetId) return;`). This meant sensitive debug strings were broadcast to every connected client over the network, leaving it up to the client to drop the message. Malicious clients could easily bypass this client-side filtering and intercept the debug data.
**Learning:** In Unity Netcode for GameObjects, never rely on client-side logic to hide sensitive data broadcast via `SendTo.Everyone`. This creates an Information Disclosure vulnerability.
**Prevention:** Always restrict data at the server level using targeted RPCs, such as `[Rpc(SendTo.SpecifiedInParams)]` in combination with `RpcTarget.Single(clientId, RpcTargetUse.Temp)` to ensure sensitive data is only sent to the intended recipient.
