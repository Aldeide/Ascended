using AbilitySystem.Runtime.Cues;
using AbilitySystem.Scripts;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;
using Moq;
using UnityEngine;

using static AbilitySystem.Test.Utilities.CueUtilities;

namespace AbilitySystem.Test.Runtime.Cues
{
    public class CueManagerTests
    {
        private GameObject _holder;
        private Mock<IAbilitySystem> _mockAbilitySystem;
        private Mock<IDataManager> _mockDataManager;
        
        [SetUp]
        public void Setup()
        {
            _holder = new GameObject("TestHolder");
            _mockAbilitySystem = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            _mockDataManager = new Mock<IDataManager>();
            _mockAbilitySystem.Setup(x => x.DataManager).Returns(_mockDataManager.Object);
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_holder);
        }
        
        [Test]
        public void CueManagerTests_ServerAbilitySystem_AddCuesDontPlay()
        {
            _mockAbilitySystem.Setup(x => x.IsServer()).Returns(true);
            _mockAbilitySystem.Setup(x => x.IsHost()).Returns(false);
            
            var cueManager = new CueManager(_mockAbilitySystem.Object, _mockDataManager.Object);
            
            bool onCueAddCalled = false;
            cueManager.OnCueAdd += (c, d) => onCueAddCalled = true;
            
            var tag = new Tag("Cue.Test.Add");
            var cueDef = CueUtilities.CreateCueDefinitionWithTag(tag);
            _mockDataManager.Setup(x => x.GetCueByTag(tag)).Returns(cueDef);
            
            cueManager.OnCueReceived(tag, CueAction.Add, new CueData());

            Assert.IsFalse(onCueAddCalled);
        }
        
        [Test]
        public void CueManagerTests_ClientAbilitySystem_AddCuesPlay()
        {
            _mockAbilitySystem.Setup(x => x.IsServer()).Returns(false);
            
            var cueManager = new CueManager(_mockAbilitySystem.Object, _mockDataManager.Object);
            
            bool onCueAddCalled = false;
            cueManager.OnCueAdd += (c, d) => onCueAddCalled = true;
            
            var tag = new Tag("Cue.Test.Add");
            var cueDef = CueUtilities.CreateCueDefinitionWithTag(tag);
            _mockDataManager.Setup(x => x.GetCueByTag(tag)).Returns(cueDef);
            
            cueManager.OnCueReceived(tag, CueAction.Add, new CueData());

            Assert.IsTrue(onCueAddCalled);
            Assert.AreEqual(1, cueManager.GetActiveCues().Count);
        }

        [Test]
        public void CueManagerTests_ClientAbilitySystem_RemoveCuesPlay()
        {
            _mockAbilitySystem.Setup(x => x.IsServer()).Returns(false);
            
            var cueManager = new CueManager(_mockAbilitySystem.Object, _mockDataManager.Object);
            
            var tag = new Tag("Cue.Test.Add");
            var cueDef = CueUtilities.CreateCueDefinitionWithTag(tag);
            _mockDataManager.Setup(x => x.GetCueByTag(tag)).Returns(cueDef);
            
            // Add first
            cueManager.OnCueReceived(tag, CueAction.Add, new CueData());
            
            bool onCueRemoveCalled = false;
            cueManager.OnCueRemove += (c, d) => onCueRemoveCalled = true;
            
            cueManager.OnCueReceived(tag, CueAction.Remove, new CueData());

            Assert.IsTrue(onCueRemoveCalled);
            Assert.AreEqual(0, cueManager.GetActiveCues().Count);
        }

        [Test]
        public void CueManagerTests_ClientAbilitySystem_ExecuteCuesPlay()
        {
            _mockAbilitySystem.Setup(x => x.IsServer()).Returns(false);
            
            var cueManager = new CueManager(_mockAbilitySystem.Object, _mockDataManager.Object);
            
            bool onCueExecuteCalled = false;
            cueManager.OnCueExecute += (c, d) => onCueExecuteCalled = true;
            
            var tag = new Tag("Cue.Test.Execute");
            var cueDef = CueUtilities.CreateCueDefinitionWithTag(tag);
            _mockDataManager.Setup(x => x.GetCueByTag(tag)).Returns(cueDef);
            
            cueManager.OnCueReceived(tag, CueAction.Execute, new CueData());

            Assert.IsTrue(onCueExecuteCalled);
        }
        
        [Test]
        public void CueManagerTests_ClientAbilitySystem_AddCueDefinitionNotFound_NoCueAdded()
        {
            _mockAbilitySystem.Setup(x => x.IsServer()).Returns(false);
            var cueManager = new CueManager(_mockAbilitySystem.Object, _mockDataManager.Object);
            var onCueAddCalled = false;
            cueManager.OnCueAdd += (c, d) => onCueAddCalled = true;
            var tag = new Tag("Cue.Test.Add");
            var cueDef = CueUtilities.CreateCueDefinitionWithTag(tag);
            _mockDataManager.Setup(x => x.GetCueByTag(tag)).Returns((CueDefinition)null);
            
            cueManager.OnCueReceived(tag, CueAction.Add, new CueData());

            Assert.IsFalse(onCueAddCalled);
            Assert.AreEqual(0, cueManager.GetActiveCues().Count, "Cues were added when no definition exists.");
        }
    }
}