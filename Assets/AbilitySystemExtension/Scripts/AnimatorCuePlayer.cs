using System;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystemExtension.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorCueListener : MonoBehaviour, ICueListener
    {
        [ShowInInspector]
        public GameplayTagQuery TagQuery { get; set; }
        
        private Animator _animator;
        private CueManager _cueManager;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _cueManager = GetComponent<AbilitySystemComponent>().AbilitySystem.CueManager;
            _cueManager.OnCueExecute += OnCueExecute;
        }

        private void OnCueExecute(CueDefinition definition, CueData cueData)
        {
            Debug.Log("Cue added with tag:" + definition.cueTag.Name);
            if (!TagQuery.MatchesTag(definition.cueTag)) return;
            if (definition is CueAnimationParameterDefinition parameterDefinition)
            {
                TriggerParameter(parameterDefinition.ParameterName);
                return;
            }
            var stateName = (definition as CueAnimationStateDefinition)?.animationLayerName;
            Debug.Log("Player layer: " + stateName);
            _animator.Play(stateName);
        }
        
        private void TriggerParameter(string parameterName)
        {
            _animator.SetTrigger(parameterName);
        }
    }
}