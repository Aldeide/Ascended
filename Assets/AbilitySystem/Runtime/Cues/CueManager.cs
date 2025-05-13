using System;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Scripts;

namespace AbilitySystem.Runtime.Cues
{
    /// <summary>
    /// Manages the lifecycle of gameplay cues, handling their addition, removal, and execution.
    /// </summary>
    public class CueManager
    {
        private readonly IAbilitySystem _owner;

        public Action<CueDefinition, CueData> OnCueAdd;
        public Action<CueDefinition, CueData> OnCueRemove;
        public Action<CueDefinition, CueData> OnCueExecute;
        
        public CueManager(IAbilitySystem owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Processes a gameplay cue based on its tag, action, and data, and invokes the appropriate operation for the cue.
        /// </summary>
        /// <param name="cueTag">The tag that identifies the specific cue.</param>
        /// <param name="cueAction">The action to perform on the cue (Add, Remove, or Execute).</param>
        /// <param name="cueData">The data associated with the cue, including positional and normal information.</param>
        public void OnCueReceived(GameplayTag cueTag, CueAction cueAction, CueData cueData)
        {
            // Don't play cues on the server.
            if (_owner.IsServer() && !_owner.IsHost()) return;
            
            var cueDefinition = DataLibrary.Instance.GetCueByTag(cueTag);
            switch(cueAction)
            {
                case CueAction.Add:
                    AddCue(cueDefinition, cueData);
                    break;
                case CueAction.Remove:
                    RemoveCue(cueDefinition, cueData);
                    break;
                case CueAction.Execute:
                    ExecuteCue(cueDefinition, cueData);
                    break;
                default:
                    break;
            }
        }

        public void AddCue(CueDefinition cue, CueData data)
        {
            OnCueAdd?.Invoke(cue, data);
        }

        public void RemoveCue(CueDefinition cue, CueData data)
        {
            OnCueRemove?.Invoke(cue, data);
        }
        
        public void ExecuteCue(CueDefinition cue, CueData data)
        {
            OnCueExecute?.Invoke(cue, data);
        }
    }
}