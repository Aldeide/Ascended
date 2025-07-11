using System.Collections;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Scripts;
using GameplayTags.Runtime;
using UnityEngine;

namespace AbilitySystemExtension.Scripts
{
    public class BurstLightCueListener : MonoBehaviour, ICueListener
    {
        [field: SerializeField]
        public TagQuery TagQuery { get; set; }
        public GameObject Light;
        public float DurationSeconds;
        
        private CueManagerComponent _cueManager;
        
        private void Start()
        {
            _cueManager = GetComponentInParent<CueManagerComponent>();
            _cueManager.OnCueAdded += OnCueAdded;
            if (!Light) return;
            Light.SetActive(false);
        }

        private void OnCueAdded(string cueTag, CueDefinition definition)
        {
            if (!Light) return;
            if (!TagQuery.MatchesTag(definition.cueTag)) return;
            Light.SetActive(true);
            StartCoroutine(DisableLightAfterDelay());
        }

        private IEnumerator DisableLightAfterDelay()
        {
            yield return new WaitForSeconds(DurationSeconds);
            Light.SetActive(false);
        }
    }
}