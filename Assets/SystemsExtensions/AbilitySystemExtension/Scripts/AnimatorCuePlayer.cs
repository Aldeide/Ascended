using System;
using AbilitySystem.Runtime.Cues;
using UnityEngine;

namespace AbilitySystemExtension.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorCueListener : CueListenerComponent
    {
        private Animator _animator;
        
        public override void Start()
        {
            base.Start();
            _animator = GetComponent<Animator>();
        }

        public override void OnExecuteCue(CueDefinition definition, CueData cueData)
        {
            if (!TagQuery.MatchesTag(definition.CueTag)) return;
            if (definition is CueAnimationParameterDefinition parameterDefinition)
            {
                TriggerParameter(parameterDefinition.ParameterName);
                return;
            }
            var stateName = (definition as CueAnimationStateDefinition)?.AnimationLayerName;
            _animator.Play(stateName);
        }

        public override void OnPlayCue(CueDefinition definition, CueData cueData)
        {
            if (!TagQuery.MatchesTag(definition.CueTag)) return;
        }

        public override void OnStopCue(CueDefinition definition, CueData cueData)
        {
            if (!TagQuery.MatchesTag(definition.CueTag)) return;
        }
        
        private void TriggerParameter(string parameterName)
        {
            _animator.SetTrigger(parameterName);
        }
    }
}