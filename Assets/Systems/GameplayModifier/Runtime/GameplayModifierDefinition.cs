using System;
using UnityEngine;
using UnityEngine.Localization;

namespace Systems.GameplayModifier.Runtime
{
    [Serializable]
    [CreateAssetMenu(fileName = "GameplayModifier", menuName = "Ascended/GameplayModifier")]
    public class GameplayModifierDefinition : ScriptableObject
    {
        public LocalizedString NameKey;
        public LocalizedString DescriptionKey;
    }
}