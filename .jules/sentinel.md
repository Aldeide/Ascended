## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-06-30 - Information Exposure via Broad RPCs
**Vulnerability:** A server-side RPC `NotifyDebugDataClientRpc` used `[Rpc(SendTo.Everyone)]` to broadcast a user's sensitive debug information to all connected clients. The method relied on client-side logic (`if (NetworkManager.LocalClientId != targetId) return;`) to discard the message if they weren't the intended recipient. A malicious client could intercept this broadcast, bypassing the client-side check, and read another player's debug data.
**Learning:** Sending sensitive information to `SendTo.Everyone` and filtering it client-side is an Information Exposure vulnerability in Unity Netcode for GameObjects.
**Prevention:** For targeted data delivery, always use `[Rpc(SendTo.SpecifiedInParams)]` on the method and invoke it by wrapping the specific target's `ClientId` inside a `RpcTarget.Single` parameter.
