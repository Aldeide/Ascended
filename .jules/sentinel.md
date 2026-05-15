## 2025-02-13 - [DoS Protection for ServerRpc]
**Vulnerability:** A `[ServerRpc]` (RequestDebugDataServerRpc) calculation was performing client-side rate limiting prior to calling the RPC, allowing a malicious client to bypass the local check, spam the RPC directly, and force the server to repeatedly execute an expensive string-building debug calculation (CalculateFullDebugInfo).
**Learning:** Client-side rate-limiting or cooldown checks for server RPCs are ineffective against manipulated clients.
**Prevention:** Always implement rate limiting, cooldowns, and validation logic directly within the `[ServerRpc]` method on the server side to ensure it cannot be bypassed by malicious actors.
