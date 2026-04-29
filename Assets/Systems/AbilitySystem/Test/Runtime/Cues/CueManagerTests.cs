using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;
using Moq;

namespace AbilitySystem.Test.Runtime.Cues
{
    public class CueManagerTests
    {
        private Mock<IAbilitySystem> _mockAbilitySystem;
        private Mock<IDataManager> _mockDataManager;
        private CueManager _cueManager;

        [SetUp]
        public void Setup()
        {
            _mockAbilitySystem = AbilitySystemUtilities.CreateMockClientAbilitySystem();
            _mockDataManager = new Mock<IDataManager>();
            _mockAbilitySystem.Setup(x => x.DataManager).Returns(_mockDataManager.Object);
            _cueManager = new CueManager(_mockAbilitySystem.Object, _mockDataManager.Object);

        }
        
        [Test]
        public void CueManagerTests_ServerAbilitySystem_AddCuesDontPlay()
        {
            _mockAbilitySystem.Setup(x => x.IsServer()).Returns(true);
            _mockAbilitySystem.Setup(x => x.IsHost()).Returns(false);
            
            var onCueAddCalled = false;
            _cueManager.OnCueAdd += (c, d) => onCueAddCalled = true;
            var tag = new Tag("Cue.Test.Add");
            var cueDef = CueUtilities.CreateCueDefinitionWithTag(tag);
            _mockDataManager.Setup(x => x.GetCueByTag(tag)).Returns(cueDef);
            
            _cueManager.OnCueReceived(tag, CueAction.Add, new CueData());

            Assert.IsFalse(onCueAddCalled);
        }
        
        [Test]
        public void CueManagerTests_ClientAbilitySystem_AddCuesPlay()
        {
            _mockAbilitySystem.Setup(x => x.IsServer()).Returns(false);
            var onCueAddCalled = false;
            _cueManager.OnCueAdd += (c, d) => onCueAddCalled = true;
            var tag = new Tag("Cue.Test.Add");
            var cueDef = CueUtilities.CreateCueDefinitionWithTag(tag);
            _mockDataManager.Setup(x => x.GetCueByTag(tag)).Returns(cueDef);
            
            _cueManager.OnCueReceived(tag, CueAction.Add, new CueData());

            Assert.IsTrue(onCueAddCalled);
            Assert.AreEqual(1, _cueManager.GetActiveCues().Count);
        }

        [Test]
        public void CueManagerTests_ClientAbilitySystem_RemoveCuesPlay()
        {
            _mockAbilitySystem.Setup(x => x.IsServer()).Returns(false);
            var tag = new Tag("Cue.Test.Add");
            var cueDef = CueUtilities.CreateCueDefinitionWithTag(tag);
            _mockDataManager.Setup(x => x.GetCueByTag(tag)).Returns(cueDef);
            
            _cueManager.OnCueReceived(tag, CueAction.Add, new CueData());
            var onCueRemoveCalled = false;
            _cueManager.OnCueRemove += (c, d) => onCueRemoveCalled = true;
            _cueManager.OnCueReceived(tag, CueAction.Remove, new CueData());

            Assert.IsTrue(onCueRemoveCalled);
            Assert.AreEqual(0, _cueManager.GetActiveCues().Count);
        }

        [Test]
        public void CueManagerTests_ClientAbilitySystem_ExecuteCuesPlay()
        {
            _mockAbilitySystem.Setup(x => x.IsServer()).Returns(false);
            var onCueExecuteCalled = false;
            _cueManager.OnCueExecute += (c, d) => onCueExecuteCalled = true;
            var tag = new Tag("Cue.Test.Execute");
            var cueDef = CueUtilities.CreateCueDefinitionWithTag(tag);
            _mockDataManager.Setup(x => x.GetCueByTag(tag)).Returns(cueDef);
            
            _cueManager.OnCueReceived(tag, CueAction.Execute, new CueData());

            Assert.IsTrue(onCueExecuteCalled);
        }
        
        [Test]
        public void CueManagerTests_ClientAbilitySystem_AddCueDefinitionNotFound_NoCueAdded()
        {
            _mockAbilitySystem.Setup(x => x.IsServer()).Returns(false);
            var onCueAddCalled = false;
            _cueManager.OnCueAdd += (c, d) => onCueAddCalled = true;
            var tag = new Tag("Cue.Test.Add");
            _mockDataManager.Setup(x => x.GetCueByTag(tag)).Returns((CueDefinition)null);
            
            _cueManager.OnCueReceived(tag, CueAction.Add, new CueData());

            Assert.IsFalse(onCueAddCalled);
            Assert.AreEqual(0, _cueManager.GetActiveCues().Count, "Cues were added when no definition exists.");
        }
    }
}