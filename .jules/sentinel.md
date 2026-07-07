## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-06-23 - Information Disclosure via Insecure RPC SendTarget
**Vulnerability:** The `NotifyDebugDataClientRpc` was broadcasting sensitive internal debug data (attributes, effects, etc.) to `Everyone`, relying on a client-side filter (`if (NetworkManager.LocalClientId != targetId) return;`) to hide the data from unintended clients. This is an Information Disclosure vulnerability, as a hacked client could easily bypass the check and read the data.
**Learning:** Never rely on client-side filtering to protect sensitive data sent via RPCs in Unity Netcode.
**Prevention:** Always restrict data broadcast at the server level using `[Rpc(SendTo.SpecifiedInParams)]` with `RpcTarget.Single` when intending to send data to a specific client.
