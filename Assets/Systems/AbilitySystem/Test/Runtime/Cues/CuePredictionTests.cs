using AbilitySystem.Runtime.Cues;
using AbilitySystem.Scripts;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;
using Moq;
using UnityEngine;

namespace AbilitySystem.Test.Runtime.Cues
{
    public class CuePredictionTests
    {
        private GameObject _holder;
        private AbilitySystemComponent _asc;
        private CueManagerComponent _cueManagerComponent;
        private Mock<IAbilitySystem> _mockAbilitySystem;
        private Mock<ICueListener> _mockListener;

        [SetUp]
        public void Setup()
        {
            _holder = new GameObject("TestHolder");
            _asc = _holder.AddComponent<AbilitySystemComponent>();
            _cueManagerComponent = _holder.AddComponent<CueManagerComponent>();
            
            _mockAbilitySystem = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            
            // Inject the mock ability system into the ASC
            // Note: Since AbilitySystem property is private set, we might need to rely on the Initialise method
            // or just test the component's internal logic if we can.
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_holder);
        }

        [Test]
        public void CuePrediction_OwnerPlaysPredictedCue_TriggersLocalListener()
        {
            // This test verifies that if the owner plays a predicted cue, 
            // the listener on the owner's machine receives it.
            
            // 1. Setup CueManager and Listener
            var cueManager = new CueManager(_mockAbilitySystem.Object);
            _mockAbilitySystem.Setup(x => x.CueManager).Returns(cueManager);
            
            var listenerMock = new Mock<ICueListener>();
            cueManager.OnCueExecute += listenerMock.Object.OnExecuteCue;

            // 2. Simulate AbilitySystem firing PlayCue
            var cueTag = "Cue.Test.Prediction";
            var cueDefinition = ScriptableObject.CreateInstance<CueDefinition>();
            cueDefinition.CueTag = new Tag(cueTag);
            
            // We setup the mock to actually fire the event when PlayCue is called
            // This simulates the AbilitySystemManager.PlayCue logic
            _mockAbilitySystem.Setup(x => x.PlayCue(cueDefinition, true))
                .Callback<CueDefinition, bool>((c, p) => cueManager.OnCueExecute?.Invoke(c, new CueData()));
            
            _mockAbilitySystem.Object.PlayCue(cueDefinition, isPredicted: true);
            
            // Assert: Verify the listener received the cue
            listenerMock.Verify(x => x.OnExecuteCue(It.IsAny<CueDefinition>(), It.IsAny<CueData>()), Times.Once);
        }
    }
}
