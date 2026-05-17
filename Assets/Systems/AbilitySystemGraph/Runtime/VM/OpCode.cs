namespace Systems.AbilitySystemGraph.VM
{
    /// <summary>
    /// Represents the set of available operations in the Graph VM.
    /// </summary>
    public enum OpCode : byte
    {
        // Flow Control
        Nop = 0,
        Jmp = 1,
        BranchTrue = 2,
        BranchFalse = 3,
        End = 4,

        // Math - Floats
        AddFloat = 10,
        SubFloat = 11,
        MulFloat = 12,
        DivFloat = 13,
        
        // Commands (Main Thread execution requests)
        CmdApplyEffect = 50,
        CmdPlayCue = 51,
        
        // Asynchronous / State
        WaitTime = 100,
        WaitForEvent = 101,
    }
}
