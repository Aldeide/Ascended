using AbilitySystem.Runtime.Cues;
using Systems.Controllers;
using Unity.Netcode;
using UnityEngine;

namespace AbilitySystemExtension.Scripts
{
    public class PrefabCueListener : CueListenerComponent
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            base.Start();
        }

        public override void OnExecuteCue(CueDefinition definition, CueData cueData)
        {
            if (TagQuery.MatchesTag(definition.CueTag) == false) return;
            var prefab = (definition as CuePrefabDefinition)?.Prefab;
            var position = cueData.VectorData[0];
            var muzzle = cueData.VectorData[1];
            var normal = cueData.VectorData[2];
            if (!prefab) return;
            if (definition.CueTag.Name.Contains("Trail"))
            {
                var trail = Instantiate(prefab, muzzle, Quaternion.LookRotation(normal, Vector3.up));
                var moveComponent = trail.GetComponent<MoveController>();
                moveComponent.TargetPosition = position;
                moveComponent.Speed = 60;
            }
            if (definition.CueTag.Name.Contains("Impact"))
            {
                Instantiate(prefab, position, Quaternion.LookRotation(normal, Vector3.up));
            }
            
        }

        public override void OnPlayCue(CueDefinition definition, CueData cueData)
        {
            throw new System.NotImplementedException();
        }

        public override void OnStopCue(CueDefinition definition, CueData cueData)
        {
            throw new System.NotImplementedException();
        }
    }
}
