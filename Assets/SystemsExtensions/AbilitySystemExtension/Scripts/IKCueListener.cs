using AbilitySystem.Runtime.Cues;
using FIMSpace.FProceduralAnimation;
using RootMotion.FinalIK;
using UnityEngine;

namespace AbilitySystemExtension.Scripts
{
    [RequireComponent(typeof(AimIK))]
    [RequireComponent(typeof(LegsAnimator))]
    public class IKCueListener: CueListenerComponent
    {
        private AimIK _aimIK;
        private LegsAnimator _legsAnimator;
        
        public override void Start()
        {
            base.Start();
            _aimIK = GetComponent<AimIK>();
            _legsAnimator = GetComponent<LegsAnimator>();
        }

        public override void OnExecuteCue(CueDefinition definition, CueData cueData)
        {
            if (!TagQuery.MatchesTag(definition.CueTag)) return;
            switch (definition.CueTag.Name)
            {
                case "Cue.IK.Feet.Enable":
                    EnableFeetIK();
                    break;
                case "Cue.IK.Feet.Disable":
                    DisableFeetIK();
                    break;
                case "Cue.IK.Arms.Enable":
                    EnableAimIK();
                    break;
                case "Cue.IK.Arms.Disable":
                    DisableAimIK();
                    break;
            }
        }

        public override void OnPlayCue(CueDefinition definition, CueData cueData)
        {
            return;
        }

        public override void OnStopCue(CueDefinition definition, CueData cueData)
        {
            return;
        }
        
        public void DisableAimIK()
        {
            _aimIK.solver.IKPositionWeight = 0;
        }
        
        public void EnableAimIK()
        {
            _aimIK.solver.IKPositionWeight = 1;
        }

        public void DisableFeetIK()
        {
            _legsAnimator.enabled = false;
        }
        
        public void EnableFeetIK()
        {
            _legsAnimator.enabled = true;
        }
    }
}
