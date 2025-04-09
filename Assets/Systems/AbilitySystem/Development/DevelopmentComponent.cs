using System;
using Systems.AbilitySystem.Authoring;
using Systems.AbilitySystem.Components;
using Systems.AbilitySystem.Effects;
using UnityEngine;

namespace Systems.Development
{
    public class DevelopmentComponent : MonoBehaviour
    {
        public GameObject player;
        public float playerHealth = 0;
        public EffectAsset effect;
        
        private AbilitySystemComponent _asc;
        
        public void Initialise(AbilitySystemComponent asc)
        {
            _asc = asc;
            //playerHealth = asc.AttributesSystem.GetAttributeCurrentValue(
            //    "CharacteristicsAttributeSet", "Health");
        }
        
        public void AddHealth()
        {
            _asc.AttributesSystem.SetAttributeCurrentValue(
                "CharacteristicsAttributeSet", "Health", playerHealth + 10);
            playerHealth = _asc.AttributesSystem.GetAttributeCurrentValue(
                "CharacteristicsAttributeSet", "Health");
        }

        public void AddEffect()
        {
            _asc.AddEffect(new EffectSpec(new Effect(effect)));
        }
    }
}