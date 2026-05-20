## 2024-05-24 - TextMeshPro Rich Text Injection

**Vulnerability:** A malicious client could send rich text tags (like `<size=1000>`) in their `PlayerName` string to the server via `UpdatePlayerNameServerRpc`. The server was blindly applying this to the lobby state, broadcasting it to all clients, which could cause UI layout breakage or UI text spoofing (an XSS equivalent for Unity).
**Learning:** `FixedString64Bytes` does not validate string content. Unsanitized text displayed via `TextMeshPro` allows rich text injection. RPCs allowing text input must be sanitized if displayed directly.
**Prevention:** Always sanitize or HTML-encode strings that are sourced from an untrusted client before displaying them via `TextMeshPro`.
