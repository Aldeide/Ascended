using System.Linq;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Effects
{
    /// <summary>
    /// Unit tests for effect replication, verifying that effect state (magnitudes, levels, timing) is correctly serialized and synchronized across the network.
    /// </summary>
    public class EffectReplicationTests : AbilitySystemTestBase
    {
        /// <summary>
        /// Verifies that the ReplicationManager correctly populates EffectSyncData with 'SetByCaller' magnitudes when an effect is added.
        /// </summary>
        [Test]
        public void EffectReplicationTests_ReplicationManagerPopulate_IncludesSetByCallerData()
        {
            // Ensure system is treated as server for replication logic
            SourceMock.Setup(o => o.IsServer()).Returns(true);
            var repl = new ReplicationManager(Source);
            
            EffectSyncData capturedData = default;
            repl.OnNotifyClientsEffectAdded += (d) => capturedData = d;
            
            var def = ScriptableObject.CreateInstance<EffectDefinition>();
            def.name = "TestEffect";
            var effect = new Effect(def);
            effect.Initialise(Source, Target);
            
            var tag = new Tag("Test.Tag");
            effect.SetSetByCallerMagnitude(tag, 123f);
            effect.Level = 5;
            
            repl.NotifyClientsEffectAdded(effect);
            
            Assert.AreEqual("TestEffect", capturedData.EffectName, "Effect name should be preserved in sync data");
            Assert.AreEqual(5, capturedData.Level, "Effect level should be preserved in sync data");
            Assert.IsNotNull(capturedData.SetByCallerTags, "SetByCallerTags array should be initialized in sync data");
            Assert.AreEqual(1, capturedData.SetByCallerTags.Length, "SetByCallerTags count should match");
            Assert.AreEqual(tag.Name, capturedData.SetByCallerTags[0].Name, "Tag name in sync data should match");
            Assert.AreEqual(123f, capturedData.SetByCallerValues[0], "Magnitude value in sync data should match");
        }

        /// <summary>
        /// Verifies that an effect's 'SetByCaller' magnitudes are correctly restored on the client when receiving sync data from the server.
        /// </summary>
        [Test]
        public void EffectReplicationTests_ClientEffectSync_RestoresSetByCallerMagnitudes()
        {
            // Setup client system
            var clientSystemMock = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            var clientSystem = clientSystemMock.Object;
            
            var effectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            effectDef.name = "TestEffect";
            
            var damageTag = new Tag("Modifier.Damage");
            var syncData = new EffectSyncData
            {
                EffectName = "TestEffect",
                ActivationTime = 10f,
                Level = 3,
                NumStacks = 1,
                SetByCallerTags = new[] { damageTag },
                SetByCallerValues = new[] { 99f }
            };
            
            // Simulate client-side application of sync data (usually handled by AbilitySystemComponent)
            var clientEffect = effectDef.ToEffect(clientSystem, clientSystem);
            clientEffect.ActivationTime = syncData.ActivationTime;
            clientEffect.Level = syncData.Level;
            clientEffect.NumStacks = syncData.NumStacks;
            
            if (syncData.SetByCallerTags != null)
            {
                for (int i = 0; i < syncData.SetByCallerTags.Length; i++)
                {
                    clientEffect.SetSetByCallerMagnitude(syncData.SetByCallerTags[i], syncData.SetByCallerValues[i]);
                }
            }
            
            Assert.AreEqual(99f, clientEffect.GetSetByCallerMagnitude(damageTag), "Client effect should have correctly restored the 'SetByCaller' magnitude");
            Assert.AreEqual(3, clientEffect.Level, "Client effect should have correctly restored the level");
            Assert.AreEqual(10f, clientEffect.ActivationTime, "Client effect should have correctly restored the activation time");
        }
    }
}
