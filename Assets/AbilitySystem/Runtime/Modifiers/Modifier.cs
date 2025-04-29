using System;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Tags;
using Sirenix.OdinInspector;

namespace AbilitySystem.Runtime.Modifiers
{
    // This abstract class defines the notion of a modifier. Modifiers are applied by Effects and change the value of
    // an attribute. Instant effects change the base value of an attribute and durational or permanent effects change
    // the current value of an attribute.
    // As such, the current value of an attribute is (base + additive_modifiers) * multiplicative_modifiers.
    // Additionally, some modifiers can be of an 'override' type. In that case, the last override modifier to be applied
    // is the one selected.
    [Serializable]
    public abstract class Modifier
    {
        [ValueDropdown("@DropdownValuesUtil.AttributeChoices", IsUniqueList = true)]
        public string attributeName;
        public EffectOperation operation;
        
        public abstract float Calculate(Effect effect);
    }
}