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
    /// <summary>
    /// Unit tests for visual cue prediction, verifying that the CueManager correctly culls redundant replicated cues from the server to prevent duplicate visual effects.
    /// </summary>
    public class CuePredictionTests : AbilitySystemTestBase
    {
        private CueManager _cueManager;
        private LocalMockDataManager _dataManager;

        [SetUp]
        public override void SetUp()
        {
            // Setup client system
            SourceMock = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            _dataManager = ScriptableObject.CreateInstance<LocalMockDataManager>();
            _cueManager = new CueManager(Source, _dataManager);
            
            base.SetUp();
        }

        /// <summary>
        /// Verifies that a replicated cue from the server is suppressed (culled) if it has already been locally predicted using the same prediction key.
        /// </summary>
        [Test]
        public void CuePredictionTests_ServerCueReceived_CullsIfAlreadyPredictedLocally()
        {
            var tag = new Tag("Cue.Test");
            var key = new PredictionKey { currentKey = 12345 };
            var data = new CueData { PredictionKey = key };
            
            // 1. Mark as predicted locally (as if an ability just triggered locally)
            _cueManager.MarkCueAsPredicted(tag.Name, key);
            
            // 2. Receive the same cue from the server via RPC
            var cueExecuted = false;
            _cueManager.OnCueExecute += (def, d) => cueExecuted = true;
            
            _cueManager.OnCueReceived(tag, CueAction.Execute, data);
            
            // 3. Verification
            Assert.IsFalse(cueExecuted, "The replicated cue should have been culled because a local prediction with the same key was found");
        }

        /// <summary>
        /// Verifies that a replicated cue from the server correctly executes if no matching local prediction exists for its prediction key.
        /// </summary>
        [Test]
        public void CuePredictionTests_ServerCueReceived_ExecutesIfNotPredictedLocally()
        {
            var tag = new Tag("Cue.Test");
            var def = ScriptableObject.CreateInstance<CueDefinition>();
            def.CueTag = tag;
            _dataManager.Cues.Add(def);
            
            var key = new PredictionKey { currentKey = 999 };
            var data = new CueData { PredictionKey = key };
            
            // We do NOT mark this key as predicted
            
            var cueExecuted = false;
            _cueManager.OnCueExecute += (d, dat) => cueExecuted = true;
            
            _cueManager.OnCueReceived(tag, CueAction.Execute, data);
            
            Assert.IsTrue(cueExecuted, "The replicated cue should execute because no matching local prediction was found");
        }

        #region Helper Classes
        private class LocalMockDataManager : ScriptableObject, IDataManager
        {
            public List<CueDefinition> Cues = new();
            public CueDefinition GetCueByTag(Tag tag) => Cues.Find(c => c.CueTag.Name == tag.Name);
            public CueDefinition GetCueByTag(string tag) => Cues.Find(c => c.CueTag.Name == tag);
            public AbilitySystem.Runtime.Abilities.AbilityDefinition GetAbilityByName(string name) => null;
            public AbilitySystem.Runtime.Effects.EffectDefinition GetEffectByName(string name) => null;
        }
        #endregion
    }
}
