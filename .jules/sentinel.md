## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2024-06-25 - TextMeshPro Rich Text Injection via Lobby Name
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their lobby name strings over the network (e.g. Steam). TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the list state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** Raw string data retrieved from network sources (like Steam lobbies) must be sanitized before being displayed in UI Text/Label components.
**Prevention:** Always sanitize strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters before passing them to UI labels.
