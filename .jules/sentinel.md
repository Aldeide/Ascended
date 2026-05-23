## 2024-05-21 - TextMeshPro Rich Text Injection
**Vulnerability:** A malicious client could send rich text tags (like `<color=red>`) in their `FixedString64Bytes` player name over a `ServerRpc`. TextMeshPro evaluates these tags indiscriminately, leading to UI spoofing or breaking layout for all clients when the lobby state is synced. This is the Unity equivalent of Cross-Site Scripting (XSS).
**Learning:** `FixedString` types in Unity Collections do not have built-in sanitization and are often blindly passed to UI elements.
**Prevention:** Always sanitize player-provided strings using a centralized utility (like `StringUtilities.SanitizeForRichText`) that strips `<` and `>` characters *before* updating authoritative network state via ServerRpc.
## 2024-05-23 - TextMeshPro XSS via Steam Name Initialization
**Vulnerability:** Initial Steam names fetched via Steam API in AddPlayerToList were not sanitized, allowing TextMeshPro rich text injection (XSS equivalent).
**Learning:** Even though RPCs updating the name were properly sanitizing input, direct assignment from external APIs (like Steamworks) during initial connection bypassed this protection, demonstrating that all external data ingestion points must be guarded.
**Prevention:** Ensure sanitization functions (like StringUtilities.SanitizeForRichText) are applied at the exact point of struct instantiation or assignment, regardless of whether the source is an RPC or an external API integration.
