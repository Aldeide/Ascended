using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Systems.AbilitySystem.Tags;
using UnityEngine;

namespace Systems.Effects
{
    [CreateAssetMenu(fileName = "Effect", menuName = "AbilitySystem/Effect2", order = 2)]
    public class EffectAuthoring : ScriptableObject
    {
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValueDropdown("GetAllTags", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] Tags;
        

    }
    

}