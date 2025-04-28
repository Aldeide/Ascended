using System;
using AbilitySystem.Runtime.Effects;
using Sirenix.OdinInspector;

namespace AbilitySystem.Runtime.Modifiers
{
    [Serializable]
    public class CostModifier : Modifier
    {
        [ValueDropdown("@DropdownValuesUtil.AttributeChoices", IsUniqueList = true)]
        public string costMetaAttribute;

        public float baseCost;
        public override float Calculate(Effect effect)
        {
            throw new System.NotImplementedException();
        }
    }
}