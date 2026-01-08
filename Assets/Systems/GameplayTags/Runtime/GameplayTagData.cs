using System.Collections.Generic;
using UnityEngine;

namespace GameplayTags.Runtime
{
    [CreateAssetMenu(fileName = "GameplayTagData", menuName = "AbilitySystem/Gameplay Tag Data")]
    public class GameplayTagData : ScriptableObject
    {
        public List<string> Tags = new();
    }
}