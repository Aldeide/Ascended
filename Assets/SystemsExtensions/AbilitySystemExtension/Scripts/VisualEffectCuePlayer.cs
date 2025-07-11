using System;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace AbilitySystemExtension.Scripts
{
    public class VisualEffectCuePlayer : MonoBehaviour
    {
        public string cuePrefix;
        public VisualEffect visualEffect;
        private CueManagerComponent _cueManager;

        public void Start()
        {
            visualEffect = GetComponent<VisualEffect>();
            _cueManager = GetComponentInParent<CueManagerComponent>();
            _cueManager.OnCueAdded += OnCueAdded;
        }

        public void OnCueAdded(string cueTag, CueDefinition definition)
        {
            if (!cueTag.StartsWith(cuePrefix)) return;
            visualEffect.visualEffectAsset = definition.visualEffectAsset;
            visualEffect.Play();
        }
    }
}