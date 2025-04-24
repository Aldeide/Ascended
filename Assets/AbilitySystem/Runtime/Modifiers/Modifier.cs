using System;
using AbilitySystem.Runtime.Effects;
using Sirenix.OdinInspector;

namespace AbilitySystem.Runtime.Modifiers
{
    [Serializable]
    public abstract class Modifier
    {
        [ValueDropdown("@DropdownValuesUtil.AttributeChoices", IsUniqueList = true)]
        public string attributeName;
        public EffectOperation operation;
        
        public abstract float Calculate(Effect effect);
    }
}