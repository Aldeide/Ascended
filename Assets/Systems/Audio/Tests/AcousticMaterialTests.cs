using NUnit.Framework;
using Systems.Audio;
using UnityEngine;

namespace Systems.Audio.Tests
{
    /// <summary>
    /// Unit tests verifying ScriptableObject configurations and material mapping lookup functions.
    /// </summary>
    public class AcousticMaterialTests
    {
        private AudioAcousticMaterial _concrete;
        private AudioAcousticMaterial _wood;
        private AudioAcousticMaterial _defaultMaterial;
        private AcousticPhysicMaterialMap _map;

        private PhysicsMaterial _physicConcrete;
        private PhysicsMaterial _physicWood;
        private PhysicsMaterial _physicUnmapped;

        [SetUp]
        public void Setup()
        {
            // Create materials dynamically for testing (EditMode safe)
            _concrete = ScriptableObject.CreateInstance<AudioAcousticMaterial>();
            _concrete.AbsorptionCoefficient = 0.05f;
            _concrete.TransmissionLossDb = 35f;
            _concrete.LowPassCutoffHz = 300f;

            _wood = ScriptableObject.CreateInstance<AudioAcousticMaterial>();
            _wood.AbsorptionCoefficient = 0.15f;
            _wood.TransmissionLossDb = 12f;
            _wood.LowPassCutoffHz = 1500f;

            _defaultMaterial = ScriptableObject.CreateInstance<AudioAcousticMaterial>();
            _defaultMaterial.AbsorptionCoefficient = 0.1f;
            _defaultMaterial.TransmissionLossDb = 15f;
            _defaultMaterial.LowPassCutoffHz = 1000f;

            // Create physics materials dynamically
            _physicConcrete = new PhysicsMaterial("ConcretePhysic");
            _physicWood = new PhysicsMaterial("WoodPhysic");
            _physicUnmapped = new PhysicsMaterial("UnmappedPhysic");

            // Build map
            _map = ScriptableObject.CreateInstance<AcousticPhysicMaterialMap>();
            _map.DefaultMaterial = _defaultMaterial;
            _map.Mappings = new AcousticPhysicMaterialMap.MaterialMapping[]
            {
                new AcousticPhysicMaterialMap.MaterialMapping { PhysicsMaterial = _physicConcrete, AcousticMaterial = _concrete },
                new AcousticPhysicMaterialMap.MaterialMapping { PhysicsMaterial = _physicWood, AcousticMaterial = _wood }
            };
        }

        [TearDown]
        public void TearDown()
        {
            if (AbilitySystem.Scripts.AbilitySystemComponent.ActiveInstances != null) AbilitySystem.Scripts.AbilitySystemComponent.ActiveInstances.Clear();
            // Clean up ScriptableObjects to avoid memory leaks in the test runner
            Object.DestroyImmediate(_concrete);
            Object.DestroyImmediate(_wood);
            Object.DestroyImmediate(_defaultMaterial);
            Object.DestroyImmediate(_map);

            Object.DestroyImmediate(_physicConcrete);
            Object.DestroyImmediate(_physicWood);
            Object.DestroyImmediate(_physicUnmapped);
        }

        [Test]
        public void GetAcousticMaterial_MappedMaterial_ReturnsCorrectProperties()
        {
            var result = _map.GetAcousticMaterial(_physicConcrete);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(0.05f, result.AbsorptionCoefficient);
            Assert.AreEqual(35f, result.TransmissionLossDb);
            Assert.AreEqual(300f, result.LowPassCutoffHz);
        }

        [Test]
        public void GetAcousticMaterial_UnmappedMaterial_ReturnsDefault()
        {
            var result = _map.GetAcousticMaterial(_physicUnmapped);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(_defaultMaterial, result);
        }

        [Test]
        public void GetAcousticMaterial_NullMaterial_ReturnsDefault()
        {
            var result = _map.GetAcousticMaterial(null);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(_defaultMaterial, result);
        }

        [Test]
        public void GetAcousticMaterial_EmptyMappingsList_ReturnsDefault()
        {
            _map.Mappings = null;
            var result = _map.GetAcousticMaterial(_physicConcrete);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(_defaultMaterial, result);
        }
    }
}
