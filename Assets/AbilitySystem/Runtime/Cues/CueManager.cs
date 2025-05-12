using System;
using System.Data.Common;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Scripts;

namespace AbilitySystem.Runtime.Cues
{
    public class CueManager
    {
        private readonly IAbilitySystem _owner;
        private CueDefinitionLibrary _cueLibrary;

        public Action<CueDefinition, CueData> OnCueAdd;
        public Action<CueDefinition, CueData> OnCueRemove;
        public Action<CueDefinition, CueData> OnCueExecute;
        
        public CueManager(IAbilitySystem owner)
        {
            _owner = owner;
        }

        public void Initialise(CueDefinitionLibrary cueLibrary)
        {
            _cueLibrary = cueLibrary;
        }

        public void OnCueReceived(GameplayTag cueTag, CueAction cueAction, CueData cueData)
        {
            // Don't execute cues on the server.
            if (_owner.IsServer() && !_owner.IsHost()) return;
            
            var cueDefinition = _cueLibrary.GetCueByTag(cueTag);
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