using System;
using Systems.AbilitySystem.Abilities;
using Systems.AbilitySystem.Attributes;
using Systems.AbilitySystem.Components;
using Systems.AbilitySystem.Effects;
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
            var attributes = _asc.AttributesSystem.GetAllAttributes();
            var abilities = _asc.GetAllAbilities();
            
            var output = "";
            foreach (var effect in effects)
            {
                output += DisplayEffect(effect);
            }

            output += "----\n";
            foreach (var attribute in attributes)
            {
                output += DisplayAttribute(attribute);
            }
            output += "----\n";
            foreach (var ability in abilities)
            {
                output += DisplayAbility(ability);
            }
            _text.text = output;
        }

        private string DisplayEffect(EffectSpec effectSpec)
        {
            var output = "";
            output += effectSpec.Effect.EffectName;
            if (effectSpec.IsActive)
            {
                output += " (Active)";
            }
            else
            {
                output += " (Inactive)";
            }
            output += "\n";
            if (effectSpec.DurationType == EffectDurationType.Infinite)
            {
                output += "Infinite duration";
            }

            if (effectSpec.DurationType == EffectDurationType.FixedDuration)
            {
                output += "Duration: " + (effectSpec.Duration - Time.time + effectSpec.ActivationTime);
            }

            return output += "\n";
        }

        private string DisplayAttribute(AttributeBase attribute)
        {
            string output = "";
            output += attribute.AttributeName() + "\n";
            output += "Base: " + attribute.BaseValue + " Current: " + attribute.CurrentValue + "\n";
            return output;
        }

        private string DisplayAbility(AbilitySpec ability)
        {
            string output = "";
            output += ability.Ability.Name;
            if (ability.IsActive)
            {
                output += " (Active)";
            }
            return output += "\n";
        }
    }
}