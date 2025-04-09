using System;
using Systems.AbilitySystem.Components;
using UnityEngine;
using TMPro;

namespace Systems.Development
{
    public class DebugComponent : MonoBehaviour
    {
        public GameObject player;
        private AbilitySystemComponent _asc;
        private TMP_Text _text;
        public void Start()
        {
            _text = GetComponent<TMP_Text>();
            _asc = player.GetComponent<AbilitySystemComponent>();
        }

        public void Update()
        {
            if (!_asc)
            {
                _asc = player.GetComponent<AbilitySystemComponent>();
                return;
            }
            var effects = _asc.EffectSystem.GetAllEffects();
            var output = "";
            foreach (var effect in effects)
            {
                var delta = (Time.time - effect.ActivationTime);
                output += effect.Effect.EffectName + " (" + (effect.Duration - delta) + ")";
            }

            _text.text = output;
        }
    }
}