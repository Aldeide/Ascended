using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Networking;
using Unity.Collections;
using Unity.Jobs;
using Systems.AbilitySystemGraph.VM;
using UnityEngine;

namespace Systems.AbilitySystemGraph.Runtime
{
    /// <summary>
    /// A concrete Ability implementation that executes logic via the Burst-compiled Graph VM.
    /// </summary>
    public class GraphAbility : Ability
    {
        public GraphAbility(GraphAbilityDefinition definition, IAbilitySystem owner, int level = 1) 
            : base(definition, owner, level)
        {
        }

        public new GraphAbilityDefinition Definition => (GraphAbilityDefinition)base.Definition;

        protected override void ActivateAbility(AbilityData data)
        {
            if (Definition.CompiledInstructions == null || Definition.CompiledInstructions.Count == 0)
            {
                Debug.LogWarning($"GraphAbility '{Definition.name}' activated but has no baked graph data.");
                TryEndAbility();
                return;
            }

            // Start the VM execution
            ExecuteGraph();
        }

        private void ExecuteGraph()
        {
            // 1. Prepare unmanaged data for the Job System
            using var instructions = new NativeArray<Instruction>(Definition.CompiledInstructions.ToArray(), Allocator.TempJob);
            using var floatRegisters = new NativeArray<float>(Definition.FloatRegisterCount, Allocator.TempJob);
            using var commandQueue = new NativeQueue<AbilityCommand>(Allocator.TempJob);

            // 2. Initialize the Burst-compiled VM Job
            var job = new AbilityExecutionJob
            {
                Instructions = instructions,
                FloatRegisters = floatRegisters,
                CommandQueue = commandQueue.AsParallelWriter()
            };

            // 3. Schedule and complete execution
            // While this runs on the Job System, we complete it immediately to process the 
            // resulting commands (Cues, Effects) on the main thread in the same frame.
            job.Schedule().Complete();

            // 4. Process commands issued by the Graph logic
            while (commandQueue.TryDequeue(out var cmd))
            {
                ProcessCommand(cmd);
            }

            // 5. Cleanup and Terminate
            // For simple instant graphs, we end the ability immediately after execution.
            // Complex graphs with 'Wait' nodes would need a persistent state-machine runner.
            TryEndAbility();
        }

        private void ProcessCommand(AbilityCommand cmd)
        {
            // Here we map the VM's integer AssetIds back to Unity ScriptableObjects
            // and trigger the appropriate Ability System APIs.
            switch (cmd.Type)
            {
                case CommandType.ApplyEffect:
                    // In a full implementation, the Definition would have a list of EffectDefinitions 
                    // that the AssetId indexes into.
                    Debug.Log($"[GraphVM] ApplyEffect request received for AssetId: {cmd.AssetId}");
                    break;
                    
                case CommandType.PlayCue:
                    Debug.Log($"[GraphVM] PlayCue request received for AssetId: {cmd.AssetId}");
                    break;
            }
        }

        public override void EndAbility()
        {
            // Internal cleanup is handled by Ability.TryEndAbility()
        }
    }
}
