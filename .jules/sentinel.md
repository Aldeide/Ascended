## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-11-10 - Unity Netcode Server RPC Authorization Bypass
**Vulnerability:** Default `[Rpc(SendTo.Server)]` allows any connected client to execute RPCs on any network object.
**Learning:** Unlike the older `[ServerRpc]` which enforced ownership by default, the newer `[Rpc]` attribute requires explicit configuration to prevent unauthorized client execution.
**Prevention:** Always enforce ownership on server-bound RPCs using `[Rpc(SendTo.Server, RequireOwnership = true)]` unless the specific method is explicitly designed for unauthenticated global access.
