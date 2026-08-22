## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2025-02-27 - Information Disclosure via RPC client-side filtering
**Vulnerability:** A `ServerRpc` was broadcasting debug data to all clients using `[Rpc(SendTo.Everyone)]`, relying on client-side logic (`if (NetworkManager.LocalClientId != targetId) return;`) to hide the data from unintended clients. This allows modified clients to simply bypass the check and read sensitive data meant for others.
**Learning:** In Unity Netcode for GameObjects, never rely on client-side filtering to hide sensitive data broadcast via `[Rpc(SendTo.Everyone)]`. It creates an Information Disclosure vulnerability.
**Prevention:** Always restrict data at the server level. Use targeted RPCs with `[Rpc(SendTo.SpecifiedInParams)]` and `RpcTarget.Single(clientId, RpcTargetUse.Temp)` to send sensitive data only to the intended client.
