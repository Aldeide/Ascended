namespace Systems.AbilitySystemGraph.VM
{
    /// <summary>
    /// Represents a single baked operation executed by the Burst VM.
    /// </summary>
    public struct Instruction
    {
        public OpCode OpCode;
        
        // Indices into the local data arrays (e.g. NativeArray<float> registers)
        public int Arg1;
        public int Arg2;
        public int Result;

        // Payload for commands or inline constant values
        public float FloatPayload;
        public int IntPayload;
    }
}
