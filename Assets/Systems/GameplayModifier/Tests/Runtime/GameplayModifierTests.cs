using NUnit.Framework;
using Systems.GameplayModifier.Runtime;

namespace Systems.GameplayModifier.Tests.Runtime
{
    public class GameplayModifierTests
    {
        [Test]
        public void GameplayModifier_Enable_SetsIsActiveToTrue()
        {
            // Arrange
            var modifier = new GameplayModifier.Runtime.GameplayModifier(null);

            // Act
            modifier.Enable();

            // Assert
            Assert.That(modifier.IsActive, Is.True);
        }

        [Test]
        public void GameplayModifier_Disable_SetsIsActiveToFalse()
        {
            // Arrange
            var modifier = new GameplayModifier.Runtime.GameplayModifier(null);
            modifier.Enable(); // Start with it active

            // Act
            modifier.Disable();

            // Assert
            Assert.That(modifier.IsActive, Is.False);
        }

        [Test]
        public void GameplayModifier_DefaultState_IsInactive()
        {
            // Arrange & Act
            var modifier = new GameplayModifier.Runtime.GameplayModifier(null);

            // Assert
            Assert.That(modifier.IsActive, Is.False);
        }
    }
}
