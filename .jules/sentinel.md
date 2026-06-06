## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-06-06 - Information Disclosure via Broadcast RPCs
**Vulnerability:** Debug data was broadcast to all clients using `[Rpc(SendTo.Everyone)]`, combined with a client-side filter (`if (NetworkManager.LocalClientId != targetId) return;`) to visually hide the data. This creates an Information Disclosure vulnerability because a modified or malicious client could simply ignore the client-side check and read the sensitive debugging info of other players/entities.
**Learning:** Client-side filters on broadcast RPCs are inherently insecure for sensitive data in an authoritative server architecture.
**Prevention:** Never rely on client-side filtering to hide sensitive data broadcast via `[Rpc(SendTo.Everyone)]`. Always restrict data at the server level using targeted RPCs (`[Rpc(SendTo.SpecifiedInParams)]` with `RpcTarget.Single`).
