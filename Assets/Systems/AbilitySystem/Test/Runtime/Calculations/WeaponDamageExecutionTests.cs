using AbilitySystem.Runtime.Calculations;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;
using UnityEngine;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;
namespace AbilitySystem.Test.Runtime.Calculations
{
    /// <summary>
    /// Tests for WeaponDamageExecution, verifying damage calculations including armor mitigation and critical hits.
    /// </summary>
    public class WeaponDamageExecutionTests : AbilitySystemTestBase
    {
        private WeaponDamageExecution _execution;
        private Effect _effect;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            _execution = ScriptableObject.CreateInstance<WeaponDamageExecution>();
            
            var def = ScriptableObject.CreateInstance<EffectDefinition>();
            _effect = new Effect(def);
            _effect.Initialise(Source, Target);
        }

        [TearDown]
        public void Teardown()
        {
            if (_execution != null) Object.DestroyImmediate(_execution);
        }

        /// <summary>
        /// Validates that base damage is correctly applied to target health when no mitigation or modifiers are present.
        /// </summary>
        [Test]
        public void WeaponDamageExecutionTests_Execute_DealsBaseDamageCorrectly()
        {
            // Setup source
            var baseDmgAttr = new AbilitySystem.Runtime.Attributes.Attribute("WeaponDamage", null, 50f, 0f, 100f);
            var critAttr = new AbilitySystem.Runtime.Attributes.Attribute("CritChance", null, 0f, 0f, 1f); // 0% crit
            
            var sourceAttrMgrMock = new Mock<AbilitySystem.Runtime.AttributeSets.AttributeSetManager>(Source);
            sourceAttrMgrMock.Setup(x => x.GetAttribute("WeaponDamage")).Returns(baseDmgAttr);
            sourceAttrMgrMock.Setup(x => x.GetAttribute("CritChance")).Returns(critAttr);
            SourceMock.SetupGet(x => x.AttributeSetManager).Returns(sourceAttrMgrMock.Object);

            // Setup target
            var healthAttr = new AbilitySystem.Runtime.Attributes.Attribute("Health", null, 100f, 0f, 100f);
            var armorAttr = new AbilitySystem.Runtime.Attributes.Attribute("Armor", null, 0f, 0f, 100f); // 0 armor
            var targetAttrMgrMock = new Mock<AbilitySystem.Runtime.AttributeSets.AttributeSetManager>(Target);
            targetAttrMgrMock.Setup(x => x.GetAttribute("Health")).Returns(healthAttr);
            targetAttrMgrMock.Setup(x => x.GetAttribute("Armor")).Returns(armorAttr);
            TargetMock.SetupGet(x => x.AttributeSetManager).Returns(targetAttrMgrMock.Object);

            _execution.Execute(_effect);

            Assert.AreEqual(50f, healthAttr.BaseValue); // 100 - 50 = 50
        }

        /// <summary>
        /// Validates that armor correctly reduces the damage dealt to the target's health.
        /// </summary>
        [Test]
        public void WeaponDamageExecutionTests_Execute_AppliesArmorMitigation()
        {
            var baseDmgAttr = new AbilitySystem.Runtime.Attributes.Attribute("WeaponDamage", null, 50f, 0f, 100f);
            var critAttr = new AbilitySystem.Runtime.Attributes.Attribute("CritChance", null, 0f, 0f, 1f);
            var sourceAttrMgrMock = new Mock<AbilitySystem.Runtime.AttributeSets.AttributeSetManager>(Source);
            sourceAttrMgrMock.Setup(x => x.GetAttribute("WeaponDamage")).Returns(baseDmgAttr);
            sourceAttrMgrMock.Setup(x => x.GetAttribute("CritChance")).Returns(critAttr);
            SourceMock.SetupGet(x => x.AttributeSetManager).Returns(sourceAttrMgrMock.Object);

            var healthAttr = new AbilitySystem.Runtime.Attributes.Attribute("Health", null, 100f, 0f, 100f);
            var armorAttr = new AbilitySystem.Runtime.Attributes.Attribute("Armor", null, 100f, 0f, 100f); // 100 armor = 50% reduction
            var targetAttrMgrMock = new Mock<AbilitySystem.Runtime.AttributeSets.AttributeSetManager>(Target);
            targetAttrMgrMock.Setup(x => x.GetAttribute("Health")).Returns(healthAttr);
            targetAttrMgrMock.Setup(x => x.GetAttribute("Armor")).Returns(armorAttr);
            TargetMock.SetupGet(x => x.AttributeSetManager).Returns(targetAttrMgrMock.Object);

            _execution.Execute(_effect);

            // 50 base damage reduced by 50% = 25 damage. 100 - 25 = 75.
            Assert.AreEqual(75f, healthAttr.BaseValue);
        }

        /// <summary>
        /// Validates that a critical hit correctly multiplies the damage dealt to the target's health.
        /// </summary>
        [Test]
        public void WeaponDamageExecutionTests_Execute_AppliesCritMultiplier()
        {
            var baseDmgAttr = new AbilitySystem.Runtime.Attributes.Attribute("WeaponDamage", null, 50f, 0f, 100f);
            var critAttr = new AbilitySystem.Runtime.Attributes.Attribute("CritChance", null, 1f, 0f, 1f); // 100% crit
            var critMultAttr = new AbilitySystem.Runtime.Attributes.Attribute("CritMultiplier", null, 2f, 0f, 2f);
            var sourceAttrMgrMock = new Mock<AbilitySystem.Runtime.AttributeSets.AttributeSetManager>(Source);
            sourceAttrMgrMock.Setup(x => x.GetAttribute("WeaponDamage")).Returns(baseDmgAttr);
            sourceAttrMgrMock.Setup(x => x.GetAttribute("CritChance")).Returns(critAttr);
            sourceAttrMgrMock.Setup(x => x.GetAttribute("CritMultiplier")).Returns(critMultAttr);
            SourceMock.SetupGet(x => x.AttributeSetManager).Returns(sourceAttrMgrMock.Object);

            var healthAttr = new AbilitySystem.Runtime.Attributes.Attribute("Health", null, 100f, 0f, 100f);
            var armorAttr = new AbilitySystem.Runtime.Attributes.Attribute("Armor", null, 0f, 0f, 100f);
            var targetAttrMgrMock = new Mock<AbilitySystem.Runtime.AttributeSets.AttributeSetManager>(Target);
            targetAttrMgrMock.Setup(x => x.GetAttribute("Health")).Returns(healthAttr);
            targetAttrMgrMock.Setup(x => x.GetAttribute("Armor")).Returns(armorAttr);
            TargetMock.SetupGet(x => x.AttributeSetManager).Returns(targetAttrMgrMock.Object);

            _execution.Execute(_effect);

            // 50 * 2.0 = 100 damage. 100 - 100 = 0.
            Assert.AreEqual(0f, healthAttr.BaseValue);
        }
    }
}
