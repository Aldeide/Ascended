using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Systems.AbilitySystemGraph.VM
{
    /// <summary>
    /// The Burst-compiled job that evaluates a baked ability graph.
    /// This acts as our lightweight Virtual Machine.
    /// </summary>
    [BurstCompile]
    public struct AbilityExecutionJob : IJob
    {
        [ReadOnly] public NativeArray<Instruction> Instructions;
        
        // Registers / Local Memory
        public NativeArray<float> FloatRegisters;
        
        // Output Command Queue back to Main Thread for Unity APIs
        public NativeQueue<AbilityCommand>.ParallelWriter CommandQueue;
        
        public void Execute()
        {
            int instructionPointer = 0;
            int instructionCount = Instructions.Length;

            while (instructionPointer < instructionCount)
            {
                var inst = Instructions[instructionPointer];
                
                switch (inst.OpCode)
                {
                    case OpCode.Nop:
                        instructionPointer++;
                        break;
                        
                    case OpCode.AddFloat:
                        FloatRegisters[inst.Result] = FloatRegisters[inst.Arg1] + FloatRegisters[inst.Arg2];
                        instructionPointer++;
                        break;
                        
                    case OpCode.SubFloat:
                        FloatRegisters[inst.Result] = FloatRegisters[inst.Arg1] - FloatRegisters[inst.Arg2];
                        instructionPointer++;
                        break;
                        
                    case OpCode.MulFloat:
                        FloatRegisters[inst.Result] = FloatRegisters[inst.Arg1] * FloatRegisters[inst.Arg2];
                        instructionPointer++;
                        break;
                        
                    case OpCode.Jmp:
                        instructionPointer = inst.Arg1; // Jump to index
                        break;
                        
                    case OpCode.CmdApplyEffect:
                        CommandQueue.Enqueue(new AbilityCommand 
                        { 
                            Type = CommandType.ApplyEffect,
                            AssetId = inst.IntPayload,
                            Magnitude = inst.Arg1 >= 0 ? FloatRegisters[inst.Arg1] : inst.FloatPayload
                        });
                        instructionPointer++;
                        break;
                        
                    case OpCode.CmdPlayCue:
                        CommandQueue.Enqueue(new AbilityCommand 
                        { 
                            Type = CommandType.PlayCue,
                            AssetId = inst.IntPayload
                        });
                        instructionPointer++;
                        break;
                        
                    case OpCode.WaitTime:
                        // Real async state-machine logic would yield here
                        instructionPointer++;
                        break;
                        
                    case OpCode.End:
                        instructionPointer = instructionCount; // Exit loop
                        break;
                        
                    default:
                        // Unhandled opcode, advance to prevent infinite loop
                        instructionPointer++;
                        break;
                }
            }
        }
    }
}
