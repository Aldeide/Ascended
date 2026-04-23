namespace AbilitySystem.Runtime.Abilities
{
    public enum AbilityNetworkPolicy
    {
        // The ability is only run on the local client. Typically for cosmetic reasons.
        ClientOnly,
        // The local client predict ability activation (and subsequent actions). The server will either validate or 
        // force the client to roll back.
        ClientPredicted,
        // The ability is only run on the server. Typically passive abilities (e.g. passive health regen).
        Server
    }

    public enum AbilityNetworkSecurityPolicy
    {
        // Client or server can both trigger execution and termination.
        ClientOrServer,
        // Server only can initiate execution. Client can still request termination.
        ServerOnlyExecution,
        // Client can initiate execution. Server only can request termination.
        ServerOnlyTermination,
        // Only the server can activate and terminate this ability.
        ServerOnly
    }
}