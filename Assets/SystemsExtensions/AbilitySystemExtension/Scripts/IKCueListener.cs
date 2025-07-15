using AbilitySystem.Runtime.Cues;
using RootMotion.FinalIK;

namespace AbilitySystemExtension.Scripts
{
    public class IKCueListener: CueListenerComponent
    {
        private AimIK _aimIK;
        
        public override void Start()
        {
            base.Start();
            _aimIK = GetComponent<AimIK>();
        }

        public override void OnExecuteCue(CueDefinition definition, CueData cueData)
        {
            return;
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
    }
}