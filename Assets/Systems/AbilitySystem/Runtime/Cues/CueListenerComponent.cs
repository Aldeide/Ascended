using System.Collections.Generic;
using AbilitySystem.Scripts;
using GameplayTags.Runtime;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystem.Runtime.Cues
{
    public abstract class CueListenerComponent : MonoBehaviour, ICueListener
    {
        [field: SerializeField]
        public TagQuery TagQuery { get; set; }
        public List<DurationalCue> ActiveCues { get; set; }
        public CueManager CueManager { get; set; }

        private AbilitySystemComponent _asc;
        
        public virtual void Start()
        {
            _asc = GetComponentInParent<AbilitySystemComponent>();
            if (_asc == null) return;

            if (_asc.IsInitialized)
            {
                BindToCueManager();
            }
            else
            {
                _asc.OnAbilitySystemInitialised += BindToCueManager;
            }
        }

        private void BindToCueManager()
        {
            if (_asc.AbilitySystem == null) return;
            
            CueManager = _asc.AbilitySystem.CueManager;
            CueManager.OnCueAdd += OnPlayCue;
            CueManager.OnCueRemove += OnStopCue;
            CueManager.OnCueExecute += OnExecuteCue;
        }

        private void OnDestroy()
        {
            if (_asc != null)
            {
                _asc.OnAbilitySystemInitialised -= BindToCueManager;
            }
            
            if (CueManager != null)
            {
                CueManager.OnCueAdd -= OnPlayCue;
                CueManager.OnCueRemove -= OnStopCue;
                CueManager.OnCueExecute -= OnExecuteCue;
            }
        }

        public abstract void OnExecuteCue(CueDefinition definition, CueData cueData);

        public abstract void OnPlayCue(CueDefinition definition, CueData cueData);

        public abstract void OnStopCue(CueDefinition definition, CueData cueData);
    }
}