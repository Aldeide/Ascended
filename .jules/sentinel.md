## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2025-02-27 - Information Disclosure via SendTo.Everyone RPCs
**Vulnerability:** Debug information intended for a single client was sent to all clients using `[Rpc(SendTo.Everyone)]`, combined with a client-side check `if (NetworkManager.LocalClientId != targetId) return;`. This allows any malicious or modified client to bypass the filter and inspect sensitive system state or debug info, resulting in Information Disclosure.
**Learning:** In Unity Netcode for GameObjects (NGO), relying on client-side filtering to hide sensitive broadcast data is an anti-pattern.
**Prevention:** Always restrict data at the server level using targeted RPCs, such as `[Rpc(SendTo.SpecifiedInParams)]` with `RpcTarget.Single()`.
