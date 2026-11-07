## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-05-24 - Unity NGO Default RPC Ownership Bypass
**Vulnerability:** In Unity Netcode for GameObjects (NGO), the newer `[Rpc(SendTo.Server)]` attribute does not require ownership by default, unlike the older `[ServerRpc]`. Without this check, any connected client can invoke these RPCs on objects they do not own, potentially bypassing security policies (like `AbilityNetworkSecurityPolicy`) that rely on the caller being the object owner.
**Learning:** Unity NGO's `[Rpc]` attribute provides no default ownership checks, placing the burden of authorization on the developer. Internal replication managers often assume the caller is the owner because they check `IsLocalClient()` on the client side, but this check is completely bypassed if an arbitrary client calls the Server RPC directly.
**Prevention:** Always explicitly enforce ownership on Server RPCs by using `[Rpc(SendTo.Server, RequireOwnership = true)]` to prevent unauthorized client invocation.
