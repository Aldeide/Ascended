using AbilitySystem.Runtime.Abilities;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;

namespace AbilitySystem.Test.Runtime.Abilities
{
    /// <summary>
    /// Unit tests for the AbilityDefinition class and its subclasses, verifying correct metadata initialization and type mapping.
    /// </summary>
    public class AbilityDefinitionTests
    {
        /// <summary>
        /// Verifies that an ability definition correctly reports its unique name, type name, and associated runtime class type.
        /// </summary>
        [Test]
        public void AbilityDefinitionTests_Creation_HasCorrectMetadata()
        {
            var abilityDefinition = AbilityUtilities.CreatePassiveAbilityDefinition();
            abilityDefinition.UniqueName = "PassiveTest";

            Assert.AreEqual("PassiveTest", abilityDefinition.UniqueName);
            Assert.AreEqual("PassiveAbilityDefinition", abilityDefinition.TypeName);
            Assert.AreEqual(typeof(PassiveAbility), abilityDefinition.AbilityType());
            Assert.AreEqual("AbilitySystem.Runtime.Abilities.PassiveAbilityDefinition",
                abilityDefinition.TypeFullName);
        }
    }
}