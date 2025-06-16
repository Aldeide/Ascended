using System;
using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Runtime.Effects;
using UnityEngine;
using UnityEngine.Localization;

namespace Systems.GameplayModifier.Runtime
{
    /// <summary>
    /// Represents a gameplay modifier definition as a ScriptableObject.
    /// This class allows for the configuration of modifiers that can grant effects
    /// and abilities to players or AI within the game system.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "GameplayModifier", menuName = "Ascended/GameplayModifier")]
    public class GameplayModifierDefinition : ScriptableObject
    {
        public LocalizedString NameKey;
        public LocalizedString DescriptionKey;
        
        public EffectDefinition[] PlayerGrantedEffects;
        public EffectDefinition[] AIGrantedEffects;

        public AbilityDefinition[] PlayerGrantedAbilities;
        public AbilityDefinition[] AIGrantedAbilities;
        
    }
}