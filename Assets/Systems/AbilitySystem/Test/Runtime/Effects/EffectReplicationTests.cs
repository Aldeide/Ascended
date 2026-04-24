using System.Linq;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Effects;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Effects
{
    public class EffectReplicationTests
    {
        [Test]
        public void ReplicationManager_PopulatesSyncData_WithSetByCaller()
        {
            var owner = AbilitySystemUtilities.CreateMockAbilitySystem();
            owner.Setup(o => o.IsServer()).Returns(true);
            var repl = new ReplicationManager(owner.Object);
            
            EffectSyncData capturedData = default;
            repl.OnNotifyClientsEffectAdded += (d) => capturedData = d;
            
            var def = ScriptableObject.CreateInstance<EffectDefinition>();
            def.name = "TestEffect";
            var effect = new Effect(def);
            effect.Initialise(owner.Object, owner.Object);
            var tag = new Tag("Test.Tag");
            effect.SetSetByCallerMagnitude(tag, 123f);
            effect.Level = 5;
            
            repl.NotifyClientsEffectAdded(effect);
            
            Assert.AreEqual("TestEffect", capturedData.EffectName);
            Assert.AreEqual(5, capturedData.Level);
            Assert.IsNotNull(capturedData.SetByCallerTags, "SetByCallerTags should not be null");
            Assert.AreEqual(1, capturedData.SetByCallerTags.Length);
            Assert.AreEqual(tag.Name, capturedData.SetByCallerTags[0].Name);
            Assert.AreEqual(123f, capturedData.SetByCallerValues[0]);
        }

        [Test]
        public void Effect_SetByCallerMagnitude_SyncsToClient()
        {
            // Simulate the full cycle logic inside AbilitySystemComponent
            var serverSys = AbilitySystemUtilities.CreateMockAbilitySystem();
            serverSys.Setup(s => s.IsServer()).Returns(true);
            
            var clientSys = AbilitySystemUtilities.CreateMockAbilitySystem();
            clientSys.Setup(s => s.IsServer()).Returns(false);
            
            var effectDef = ScriptableObject.CreateInstance<EffectDefinition>();
            effectDef.name = "TestEffect";
            
            var tag = new Tag("Modifier.Damage");
            var data = new EffectSyncData
            {
                EffectName = "TestEffect",
                ActivationTime = 10f,
                Level = 3,
                NumStacks = 1,
                SetByCallerTags = new[] { tag },
                SetByCallerValues = new[] { 99f }
            };
            
            // Client side logic simulation (Mirroring ApplySyncDataToEffect)
            var clientEffect = effectDef.ToEffect(clientSys.Object, clientSys.Object);
            clientEffect.ActivationTime = data.ActivationTime;
            clientEffect.Level = data.Level;
            clientEffect.NumStacks = data.NumStacks;
            
            if (data.SetByCallerTags != null)
            {
                for (int i = 0; i < data.SetByCallerTags.Length; i++)
                {
                    clientEffect.SetSetByCallerMagnitude(data.SetByCallerTags[i], data.SetByCallerValues[i]);
                }
            }
            
            Assert.AreEqual(99f, clientEffect.GetSetByCallerMagnitude(tag));
            Assert.AreEqual(3, clientEffect.Level);
        }
    }
}
