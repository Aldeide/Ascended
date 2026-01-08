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

        public virtual void Start()
        {
            CueManager = GetComponentInParent<AbilitySystemComponent>().AbilitySystem.CueManager;
            CueManager.OnCueAdd += OnPlayCue;
            CueManager.OnCueRemove += OnStopCue;
            CueManager.OnCueExecute += OnExecuteCue;
        }

        public abstract void OnExecuteCue(CueDefinition definition, CueData cueData);

        public abstract void OnPlayCue(CueDefinition definition, CueData cueData);

        public abstract void OnStopCue(CueDefinition definition, CueData cueData);
    }
}