## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-31 - Information Disclosure via SendTo.Everyone
**Vulnerability:** In `AbilitySystemComponent.cs`, `RequestDebugDataServerRpc` retrieves full system debug output and sends it over the network via an RPC marked with `[Rpc(SendTo.Everyone)]`. A client-side filter (`if (NetworkManager.LocalClientId != targetId) return;`) was used to hide this from unintended users. However, because the attribute broadcasts the sensitive data to everyone, a malicious client or packet sniffer could easily bypass the filter, leading to a critical Information Disclosure vulnerability.
**Learning:** Never rely on client-side filtering to hide sensitive data broadcast via `[Rpc(SendTo.Everyone)]`.
**Prevention:** Always restrict sensitive data transmission at the server level by using targeted RPCs, such as `[Rpc(SendTo.SpecifiedInParams)]` with `RpcTarget.Single(clientId, RpcTargetUse.Temp)`, to ensure the data is only sent to the intended recipient.
