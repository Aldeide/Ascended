using AbilitySystem.Runtime.Calculations;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Test.Utilities;
using Moq;
using NUnit.Framework;
using UnityEngine;
using Attribute = AbilitySystem.Runtime.Attributes.Attribute;

namespace AbilitySystem.Tests.Runtime.Calculations
{
    public class WeaponDamageExecutionTests
    {
        private Mock<AbilitySystem.Runtime.Core.IAbilitySystem> _mockSource;
        private Mock<AbilitySystem.Runtime.Core.IAbilitySystem> _mockTarget;
        private WeaponDamageExecution _execution;
        private Effect _effect;

        private Mock<AbilitySystem.Runtime.AttributeSets.AttributeSetManager> _mockSourceAttrMgr;
        private Mock<AbilitySystem.Runtime.AttributeSets.AttributeSetManager> _mockTargetAttrMgr;

        [SetUp]
        public void Setup()
        {
            _mockSource = AbilitySystemUtilities.CreateMockAbilitySystem();
            _mockTarget = AbilitySystemUtilities.CreateMockAbilitySystem();
            
            var mockSourceAttrMgr = new Mock<AbilitySystem.Runtime.AttributeSets.AttributeSetManager>(_mockSource.Object);
            var mockTargetAttrMgr = new Mock<AbilitySystem.Runtime.AttributeSets.AttributeSetManager>(_mockTarget.Object);
            _mockSource.SetupGet(x => x.AttributeSetManager).Returns(mockSourceAttrMgr.Object);
            _mockTarget.SetupGet(x => x.AttributeSetManager).Returns(mockTargetAttrMgr.Object);

            // Store the mocks so they can be accessed in tests
            _mockSourceAttrMgr = mockSourceAttrMgr;
            _mockTargetAttrMgr = mockTargetAttrMgr;

            _execution = ScriptableObject.CreateInstance<WeaponDamageExecution>();
            
            var def = ScriptableObject.CreateInstance<EffectDefinition>();
            _effect = new Effect(def);
            _effect.Initialise(_mockSource.Object, _mockTarget.Object);
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_execution);
        }

        [Test]
        public void Execute_DealsBaseDamage_WhenNoCritOrArmor()
        {
            // Setup source
            var baseDmgAttr = new Attribute("WeaponDamage", null, 50f, 0f, 100f);
            var critAttr = new Attribute("CritChance", null, 0f, 0f, 1f); // 0% crit
            _mockSourceAttrMgr.Setup(x => x.GetAttribute("WeaponDamage")).Returns(baseDmgAttr);
            _mockSourceAttrMgr.Setup(x => x.GetAttribute("CritChance")).Returns(critAttr);

            // Setup target
            var healthAttr = new Attribute("Health", null, 100f, 0f, 100f);
            var armorAttr = new Attribute("Armor", null, 0f, 0f, 100f); // 0 armor
            _mockTargetAttrMgr.Setup(x => x.GetAttribute("Health")).Returns(healthAttr);
            _mockTargetAttrMgr.Setup(x => x.GetAttribute("Armor")).Returns(armorAttr);

            _execution.Execute(_effect);

            Assert.AreEqual(50f, healthAttr.BaseValue); // 100 - 50 = 50
        }

        [Test]
        public void Execute_AppliesArmorMitigation()
        {
            var baseDmgAttr = new Attribute("WeaponDamage", null, 50f, 0f, 100f);
            var critAttr = new Attribute("CritChance", null, 0f, 0f, 1f);
            _mockSourceAttrMgr.Setup(x => x.GetAttribute("WeaponDamage")).Returns(baseDmgAttr);
            _mockSourceAttrMgr.Setup(x => x.GetAttribute("CritChance")).Returns(critAttr);

            var healthAttr = new Attribute("Health", null, 100f, 0f, 100f);
            var armorAttr = new Attribute("Armor", null, 100f, 0f, 100f); // 100 armor = 50% reduction
            _mockTargetAttrMgr.Setup(x => x.GetAttribute("Health")).Returns(healthAttr);
            _mockTargetAttrMgr.Setup(x => x.GetAttribute("Armor")).Returns(armorAttr);

            _execution.Execute(_effect);

            // 50 base damage reduced by 50% = 25 damage. 100 - 25 = 75.
            Assert.AreEqual(75f, healthAttr.BaseValue);
        }

        [Test]
        public void Execute_AppliesCritMultiplier()
        {
            var baseDmgAttr = new Attribute("WeaponDamage", null, 50f, 0f, 100f);
            var critAttr = new Attribute("CritChance", null, 1f, 0f, 1f); // 100% crit
            var critMultAttr = new Attribute("CritMultiplier", null, 2f, 0f, 2f);
            _mockSourceAttrMgr.Setup(x => x.GetAttribute("WeaponDamage")).Returns(baseDmgAttr);
            _mockSourceAttrMgr.Setup(x => x.GetAttribute("CritChance")).Returns(critAttr);
            _mockSourceAttrMgr.Setup(x => x.GetAttribute("CritMultiplier")).Returns(critMultAttr);

            var healthAttr = new Attribute("Health", null, 100f, 0f, 100f);
            var armorAttr = new Attribute("Armor", null, 0f, 0f, 100f);
            _mockTargetAttrMgr.Setup(x => x.GetAttribute("Health")).Returns(healthAttr);
            _mockTargetAttrMgr.Setup(x => x.GetAttribute("Armor")).Returns(armorAttr);

            _execution.Execute(_effect);

            // 50 * 2.0 = 100 damage. 100 - 100 = 0.
            Assert.AreEqual(0f, healthAttr.BaseValue);
        }
    }
}
