using System.Collections.Generic;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Networking;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Cues
{
    public class CuePredictionTests
    {
        [Test]
        public void CueManager_CullsPredictedCue_WhenReceivedFromServer()
        {
            var mockSys = AbilitySystemUtilities.CreateMockAbilitySystem();
            mockSys.Setup(s => s.IsServer()).Returns(false); // Client
            
            var manager = new CueManager(mockSys.Object);
            
            var tag = new Tag("Cue.Test");
            var key = new PredictionKey(12345);
            var data = new CueData { PredictionKey = key };
            
            // 1. Mark as predicted
            manager.MarkCueAsPredicted(tag.Name, key);
            
            // 2. Receive same cue from server
            var cueExecuted = false;
            manager.OnCueExecute += (def, d) => cueExecuted = true;
            
            manager.OnCueReceived(tag, CueAction.Execute, data);
            
            // 3. Verify it was culled (Not executed)
            Assert.IsFalse(cueExecuted, "Predicted cue should have been culled!");
        }

        [Test]
        public void CueManager_ExecutesReplicatedCue_WhenNotPredicted()
        {
            var mockSys = AbilitySystemUtilities.CreateMockAbilitySystem();
            mockSys.Setup(s => s.IsServer()).Returns(false); // Client
            
            // Create a simple mock data manager
            var dataManager = new MockDataManager(); 
            var manager = new CueManager(mockSys.Object, dataManager);
            
            var tag = new Tag("Cue.Test");
            var def = ScriptableObject.CreateInstance<CueDefinition>();
            def.CueTag = tag;
            dataManager.Cues.Add(def);
            
            var key = new PredictionKey(999);
            var data = new CueData { PredictionKey = key };
            
            // Receive cue WITHOUT marking as predicted
            var cueExecuted = false;
            manager.OnCueExecute += (d, dat) => cueExecuted = true;
            
            manager.OnCueReceived(tag, CueAction.Execute, data);
            
            Assert.IsTrue(cueExecuted, "Unpredicted replicated cue should execute!");
        }

        private class MockDataManager : ScriptableObject, IDataManager
        {
            public List<CueDefinition> Cues = new();
            public CueDefinition GetCueByTag(Tag tag) => Cues.Find(c => c.CueTag.Name == tag.Name);
            public CueDefinition GetCueByTag(string tag) => Cues.Find(c => c.CueTag.Name == tag);
            public AbilitySystem.Runtime.Abilities.AbilityDefinition GetAbilityByName(string name) => null;
            public AbilitySystem.Runtime.Effects.EffectDefinition GetEffectByName(string name) => null;
        }
    }
}
