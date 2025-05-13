using System;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystemExtension.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorCuePlayer : MonoBehaviour, ICueListener
    {
        public GameplayTag[] TagFilter { get; set; }
        [ShowInInspector]
        public GameplayTagQuery TagQuery { get; set; }
        
        private Animator _animator;
        private CueManagerComponent _cueManager;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _cueManager = GetComponent<CueManagerComponent>();
            _cueManager.OnCueAdded += OnCueAdded;
        }

        private void OnCueAdded(string cueTag, CueDefinition definition)
        {
            Debug.Log("Cue added with tag:" + cueTag);
            if (!cueTag.StartsWith("Cue.Animation")) return;
            var stateName = (definition as CueAnimationStateDefinition)?.animationLayerName;
            Debug.Log("Player layer: " + stateName);
            _animator.Play(stateName);
        }


    }
}