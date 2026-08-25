## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.

## 2026-08-25 - UI Rich Text Injection in Lobby Names
**Vulnerability:** Untrusted Steam lobby data (`HostName` and `LobbyName`) was injected directly into UI Toolkit Labels without sanitization, allowing for XSS-equivalent rich text injection (e.g., embedding tags like `<sprite index=0>`).
**Learning:** Even though server-side components properly sanitized internal player names via RPCs, external peer-to-peer or platform matchmaking data presented in public lobby browsers requires identical sanitization.
**Prevention:** Always pass strings populated from external, untrusted sources through `StringUtilities.SanitizeForRichText` before binding them to any UI label or text component.
