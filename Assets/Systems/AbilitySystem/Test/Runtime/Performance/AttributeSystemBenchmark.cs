using System.Diagnostics;
using System.Collections.Generic;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Modifiers;
using AbilitySystem.Test.Utilities;
using NUnit.Framework;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Moq;
using AbilitySystem.Runtime.Networking;

namespace AbilitySystem.Test.Runtime.Performance
{
    public class AttributeSystemBenchmark
    {
        private const int EntityCount = 100;
        private const int ModifiersPerAttribute = 5;
        private List<AbilitySystemManager> _systems;
        private List<Effect> _activeEffects;

        [SetUp]
        public void SetUp()
        {
            _systems = new List<AbilitySystemManager>();
            _activeEffects = new List<Effect>();

            for (int i = 0; i < EntityCount; i++)
            {
                var dataMock = new Mock<IDataManager>();
                var netMock = new Mock<INetworkRole>();
                netMock.Setup(x => x.IsServer).Returns(true);
                netMock.Setup(x => x.NetworkObjectId).Returns((ulong)i);
                
                var system = new AbilitySystemManager(dataMock.Object);
                system.NetworkRole = netMock.Object;
                
                system.AttributeSetManager.AddAttributeSet(typeof(TestAttributeSet), new TestAttributeSet(system));
                _systems.Add(system);

                // Add some modifiers to make the job do work
                var effectDef = ScriptableObject.CreateInstance<EffectDefinition>();
                effectDef.DurationType = EffectDurationType.Infinite;
                
                var modifiers = new List<Modifier>();
                var attributes = system.AttributeSetManager.GetAttributeSet<TestAttributeSet>().GetAllAttributes();
                
                foreach (var attr in attributes)
                {
                    for (int j = 0; j < ModifiersPerAttribute; j++)
                    {
                        modifiers.Add(new FloatModifier 
                        { 
                            AttributeName = attr.GetFullName(), 
                            ModifierMagnitude = 1.1f, 
                            Operation = EffectOperation.Multiplicative 
                        });
                    }
                }
                effectDef.Modifiers = modifiers.ToArray();
                
                var effect = system.MakeOutgoingEffect(effectDef);
                system.ApplyEffectToSelf(effect);
                _activeEffects.Add(effect);
            }
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var system in _systems)
            {
                system.Dispose();
            }
            _systems.Clear();
        }

        [Test]
        public void Benchmark_AttributeRecalculation()
        {
            // Warm up
            foreach (var system in _systems)
            {
                system.Tick();
            }

            var sw = new Stopwatch();
            int iterations = 100;
            
            sw.Start();
            for (int i = 0; i < iterations; i++)
            {
                foreach (var system in _systems)
                {
                    // Force dirty to ensure job runs
                    system.AttributeSetManager.MarkDirty();
                    system.AttributeSetManager.UpdateAttributesJobified();
                }
            }
            sw.Stop();

            double totalMs = sw.Elapsed.TotalMilliseconds;
            double avgMsPerTick = totalMs / iterations;
            double avgUsPerEntity = (avgMsPerTick * 1000) / EntityCount;

            Debug.Log($"[Benchmark] Recalculation for {EntityCount} entities ({ModifiersPerAttribute} mods/attr):");
            Debug.Log($"[Benchmark] Total Time ({iterations} iterations): {totalMs:F2}ms");
            Debug.Log($"[Benchmark] Average Time per Tick: {avgMsPerTick:F4}ms");
            Debug.Log($"[Benchmark] Average Time per Entity: {avgUsPerEntity:F2}us");
            
            // Basic sanity check to ensure it's not crazy slow
            Assert.Less(avgMsPerTick, 5.0f, "Average tick time should be under 5ms for 100 entities");
        }
    }
}
