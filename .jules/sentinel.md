## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-05-24 - Information Disclosure via ServerRpc broadcast
**Vulnerability:** Using `[Rpc(SendTo.Everyone)]` and filtering via `if (NetworkManager.LocalClientId != targetId)` leaks sensitive information to all clients because the RPC payload is broadcasted across the network.
**Learning:** Client-side filtering is insecure because any modified client can ignore the `LocalClientId` check and read the data. Always route sensitive data using `SendTo.SpecifiedInParams` directly to the intended client.
**Prevention:** Use `[Rpc(SendTo.SpecifiedInParams)]` and specify the target clientId when calling it using `new RpcParams { Send = new RpcSendParams { Target = RpcTarget.Single(clientId, RpcTargetUse.Temp) } }`.
