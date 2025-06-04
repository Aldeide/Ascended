using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Tags;
using AbilitySystem.Scripts;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AbilitySystemExtension.Scripts
{
    public class IKCueListener: MonoBehaviour, ICueListener
    {
        [field: SerializeField]
        public GameplayTagQuery TagQuery { get; set; }
        
        private CueManagerComponent _cueManager;
        private AimIK _aimIK;
        
        private void Start()
        {
            _cueManager = GetComponent<CueManagerComponent>();
            _cueManager.OnCueAdded += OnCueAdded;
            _aimIK = GetComponent<AimIK>();
        }

        private void OnCueAdded(string cueTag, CueDefinition definition)
        {
            if (cueTag == "Cue.IK.Arm.Right.Disable")
            {
                
            }
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