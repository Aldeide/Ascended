using AbilitySystem.Runtime.Cues;
using AbilitySystem.Runtime.Core;
using AbilitySystem.Test.Utilities;
using GameplayTags.Runtime;
using NUnit.Framework;
using Moq;

namespace AbilitySystem.Test.Runtime.Cues
{
    /// <summary>
    /// Unit tests for the CueManager, verifying that Gameplay Cues are correctly processed, 
    /// played on clients, and ignored on pure servers.
    /// </summary>
    public class CueManagerTests : AbilitySystemTestBase
    {
        private Mock<IDataManager> _mockDataManager;
        private CueManager _cueManager;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _mockDataManager = new Mock<IDataManager>();
            SourceMock.Setup(x => x.DataManager).Returns(_mockDataManager.Object);
            _cueManager = new CueManager(Source, _mockDataManager.Object);
        }
        
        /// <summary>
        /// Validates that cues are not played on the server (unless it's a host, which is tested elsewhere).
        /// </summary>
        [Test]
        public void CueManagerTests_Server_IgnoresCues()
        {
            SourceMock.Setup(x => x.IsServer()).Returns(true);
            SourceMock.Setup(x => x.IsHost()).Returns(false);
            
            var onCueAddCalled = false;
            _cueManager.OnCueAdd += (c, d) => onCueAddCalled = true;
            var tag = new Tag("Cue.Test.Add");
            var cueDef = CueUtilities.CreateCueDefinitionWithTag(tag);
            _mockDataManager.Setup(x => x.GetCueByTag(tag)).Returns(cueDef);
            
            _cueManager.OnCueReceived(tag, CueAction.Add, new CueData());

            Assert.IsFalse(onCueAddCalled, "Server should not fire OnCueAdd event");
        }
        
        /// <summary>
        /// Validates that cues are correctly triggered and tracked on clients.
        /// </summary>
        [Test]
        public void CueManagerTests_Client_PlaysAddedCues()
        {
            SourceMock.Setup(x => x.IsServer()).Returns(false);
            var onCueAddCalled = false;
            _cueManager.OnCueAdd += (c, d) => onCueAddCalled = true;
            var tag = new Tag("Cue.Test.Add");
            var cueDef = CueUtilities.CreateCueDefinitionWithTag(tag);
            _mockDataManager.Setup(x => x.GetCueByTag(tag)).Returns(cueDef);
            
            _cueManager.OnCueReceived(tag, CueAction.Add, new CueData());

            Assert.IsTrue(onCueAddCalled, "Client should fire OnCueAdd event");
            Assert.AreEqual(1, _cueManager.GetActiveCues().Count);
        }

        /// <summary>
        /// Validates that removing a cue correctly triggers the removal event and updates the active list.
        /// </summary>
        [Test]
        public void CueManagerTests_Client_RemovesCuesCorrectly()
        {
            SourceMock.Setup(x => x.IsServer()).Returns(false);
            var tag = new Tag("Cue.Test.Add");
            var cueDef = CueUtilities.CreateCueDefinitionWithTag(tag);
            _mockDataManager.Setup(x => x.GetCueByTag(tag)).Returns(cueDef);
            
            _cueManager.OnCueReceived(tag, CueAction.Add, new CueData());
            var onCueRemoveCalled = false;
            _cueManager.OnCueRemove += (c, d) => onCueRemoveCalled = true;
            _cueManager.OnCueReceived(tag, CueAction.Remove, new CueData());

            Assert.IsTrue(onCueRemoveCalled, "Client should fire OnCueRemove event");
            Assert.AreEqual(0, _cueManager.GetActiveCues().Count);
        }

        /// <summary>
        /// Validates that one-shot cues (Execute) are correctly triggered.
        /// </summary>
        [Test]
        public void CueManagerTests_Client_PlaysExecutedCues()
        {
            SourceMock.Setup(x => x.IsServer()).Returns(false);
            var onCueExecuteCalled = false;
            _cueManager.OnCueExecute += (c, d) => onCueExecuteCalled = true;
            var tag = new Tag("Cue.Test.Execute");
            var cueDef = CueUtilities.CreateCueDefinitionWithTag(tag);
            _mockDataManager.Setup(x => x.GetCueByTag(tag)).Returns(cueDef);
            
            _cueManager.OnCueReceived(tag, CueAction.Execute, new CueData());

            Assert.IsTrue(onCueExecuteCalled, "Client should fire OnCueExecute event");
        }
        
        /// <summary>
        /// Validates that receiving a cue tag with no associated definition does not result in an active cue.
        /// </summary>
        [Test]
        public void CueManagerTests_Client_IgnoresCuesWithMissingDefinitions()
        {
            SourceMock.Setup(x => x.IsServer()).Returns(false);
            var onCueAddCalled = false;
            _cueManager.OnCueAdd += (c, d) => onCueAddCalled = true;
            var tag = new Tag("Cue.Test.Add");
            _mockDataManager.Setup(x => x.GetCueByTag(tag)).Returns((CueDefinition)null);
            
            _cueManager.OnCueReceived(tag, CueAction.Add, new CueData());

            Assert.IsFalse(onCueAddCalled, "Cue should not be added if definition is missing");
            Assert.AreEqual(0, _cueManager.GetActiveCues().Count);
        }
    }
}