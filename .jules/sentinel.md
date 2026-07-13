## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2026-07-13 - Information Exposure via Rpc(SendTo.Everyone)
**Vulnerability:** Debug or sensitive information sent over `[Rpc(SendTo.Everyone)]` combined with client-side filtering (e.g., checking `LocalClientId` against a target ID) results in all connected clients receiving the payload before filtering it locally. This causes Information Exposure.
**Learning:** Unity NGO's `SendTo.Everyone` broadcasts data universally, disregarding any internal conditional checks the function might perform after the packet is received.
**Prevention:** Use `[Rpc(SendTo.SpecifiedInParams)]` and pass an explicit `RpcTarget` (e.g., `RpcTarget.Single(targetId, RpcTargetUse.Temp)`) directly when invoking the RPC, rather than accepting a client ID as a method argument for client-side evaluation.
