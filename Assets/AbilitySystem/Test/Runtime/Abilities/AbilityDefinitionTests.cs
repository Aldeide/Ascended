using AbilitySystem.Runtime.Abilities.PassiveAbility;
using NUnit.Framework;
using static AbilitySystem.Test.Utilities.AbilityUtilities;

namespace AbilitySystem.Test.Runtime.Abilities
{
    public class AbilityDefinitionTests
    {
        [Test]
        public void AbilityDefinitionTests_CreationDefault_HasCorrectValues()
        {
            var abilityDefinition = CreatePassiveAbilityDefinition();

            Assert.AreEqual("TestAbility", abilityDefinition.UniqueName);
            Assert.AreEqual("PassiveAbilityDefinition", abilityDefinition.TypeName);
            Assert.AreEqual(typeof(PassiveAbility), abilityDefinition.AbilityType());
            Assert.AreEqual("AbilitySystem.Runtime.Abilities.PassiveAbility.PassiveAbilityDefinition",
                abilityDefinition.TypeFullName);
        }
    }
}