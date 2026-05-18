## 2025-02-23 - Prevented Debug Data Information Disclosure via Insecure Rpc Filtering
**Vulnerability:** Information Disclosure where `[Rpc(SendTo.Everyone)]` combined with insecure client-side `if (NetworkManager.LocalClientId != targetId)` filtering exposed debug stats to unauthorized players.
**Learning:** Legacy Netcode logic often falls back to `.Everyone` broadcasting with client-side culling. This pattern breaks zero-trust networking as it relies on the client to securely enforce visibility of debug attributes.
**Prevention:** Avoid `[Rpc(SendTo.Everyone)]` for sensitive data. Utilize targeted RPCs with `[Rpc(SendTo.SpecifiedInParams)]` and `RpcTarget.Single(clientId)` to securely and correctly route restricted information across the network.
