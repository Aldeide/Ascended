using NUnit.Framework;
using Unity.Collections;
using Systems.Core.Utilities;

namespace Systems.Core.Tests
{
    [TestFixture]
    public class StringUtilitiesTests
    {
        [Test]
        public void SanitizeForRichText_StripsHtmlTags()
        {
            // Arrange
            FixedString64Bytes maliciousInput = new FixedString64Bytes("<color=red>Hacked!</color>");

            // Act
            FixedString64Bytes sanitized = StringUtilities.SanitizeForRichText(maliciousInput);

            // Assert
            Assert.That(sanitized.ToString(), Is.EqualTo("color=redHacked!/color"));
        }

        [Test]
        public void SanitizeForRichText_NoTags_ReturnsOriginal()
        {
            // Arrange
            FixedString64Bytes normalInput = new FixedString64Bytes("Normal Player");

            // Act
            FixedString64Bytes sanitized = StringUtilities.SanitizeForRichText(normalInput);

            // Assert
            Assert.That(sanitized.ToString(), Is.EqualTo("Normal Player"));
        }

        [Test]
        public void SanitizeForRichText_String_StripsHtmlTags()
        {
            // Arrange
            string maliciousInput = "<color=red>Hacked!</color>";

            // Act
            string sanitized = StringUtilities.SanitizeForRichText(maliciousInput);

            // Assert
            Assert.That(sanitized, Is.EqualTo("color=redHacked!/color"));
        }

        [Test]
        public void SanitizeForRichText_String_NoTags_ReturnsOriginal()
        {
            // Arrange
            string normalInput = "Normal Player";

            // Act
            string sanitized = StringUtilities.SanitizeForRichText(normalInput);

            // Assert
            Assert.That(sanitized, Is.EqualTo("Normal Player"));
        }
    }
}
