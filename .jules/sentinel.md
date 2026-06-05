## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-06-05 - Information Disclosure via SendTo.Everyone
**Vulnerability:** `[Rpc(SendTo.Everyone)]` combined with a client-side check like `if (NetworkManager.LocalClientId != targetId) return;` causes sensitive debug or state data to be broadcasted to *all* clients. Malicious clients can simply intercept the RPC payload before the client-side check executes, leading to Information Disclosure.
**Learning:** Client-side filtering in multiplayer games provides false security. Network data is visible to any client receiving the packet.
**Prevention:** Always restrict data transmission at the server level using targeted RPCs. Use `[Rpc(SendTo.SpecifiedInParams)]` with a signature accepting `RpcParams`, and call it from the server using `RpcTarget.Single(clientId, RpcTargetUse.Temp)` to guarantee that only the intended recipient receives the data.
