using System.Collections;
using AbilitySystem.Runtime.Cues;
using UnityEngine;

namespace AbilitySystemExtension.Scripts
{
    public class BurstLightCueListener : CueListenerComponent
    {
        public GameObject Light;
        public float DurationSeconds;
        
        public override void Start()
        {
            base.Start();
            if (!Light) return;
            Light.SetActive(false);
        }
        
        public override void OnExecuteCue(CueDefinition definition, CueData cueData)
        {
            if (!TagQuery.MatchesTag(definition.CueTag)) return;
            if (!Light) return;
            Light.SetActive(true);
            StartCoroutine(DisableLightAfterDelay());
        }

        public override void OnPlayCue(CueDefinition definition, CueData cueData)
        {
            if (!TagQuery.MatchesTag(definition.CueTag)) return;
        }

        public override void OnStopCue(CueDefinition definition, CueData cueData)
        {
            if (!TagQuery.MatchesTag(definition.CueTag)) return;
        }


        private IEnumerator DisableLightAfterDelay()
        {
            yield return new WaitForSeconds(DurationSeconds);
            Light.SetActive(false);
        }
    }
}