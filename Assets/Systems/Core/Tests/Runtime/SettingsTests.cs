using NUnit.Framework;
using UnityEngine;
using Systems.Core;
using UnityEngine.InputSystem;

namespace Systems.Core.Tests
{
    /// <summary>
    /// Tests for the SettingsManager, verifying persistence and application of audio, 
    /// quality, and resolution settings.
    /// </summary>
    public class SettingsTests
    {
        private GameObject _settingsGO;
        private SettingsManager _manager;

        [SetUp]
        public void SetUp()
        {
            _settingsGO = new GameObject("SettingsManager");
            _manager = _settingsGO.AddComponent<SettingsManager>();
            
            // Clean PlayerPrefs before each test
            PlayerPrefs.DeleteAll();
        }

        [TearDown]
        public void TearDown()
        {
            if (_settingsGO != null) Object.DestroyImmediate(_settingsGO);
            PlayerPrefs.DeleteAll();
        }

        /// <summary>
        /// Validates that setting the master volume persists across sessions and correctly 
        /// updates the AudioListener.
        /// </summary>
        [Test]
        public void SettingsTests_Volume_PersistsAndAppliesOnLoad()
        {
            float targetVolume = 0.75f;
            _manager.SetVolume(targetVolume);
            
            // Verify immediate change
            Assert.AreEqual(targetVolume, AudioListener.volume);

            // Simulate reload by creating a new manager
            Object.DestroyImmediate(_settingsGO);
            _settingsGO = new GameObject("SettingsManager_Reloaded");
            _manager = _settingsGO.AddComponent<SettingsManager>();

            // Verify loaded value
            Assert.AreEqual(targetVolume, AudioListener.volume);
        }

        /// <summary>
        /// Validates that setting the quality level correctly updates Unity's QualitySettings 
        /// and persists the choice.
        /// </summary>
        [Test]
        public void SettingsTests_Quality_PersistsChoice()
        {
            int targetQuality = 1; // Example index
            _manager.SetQuality(targetQuality);

            Assert.AreEqual(targetQuality, QualitySettings.GetQualityLevel());
            Assert.AreEqual(targetQuality, PlayerPrefs.GetInt("Settings_Quality"));
        }

        /// <summary>
        /// Validates that resetting settings to default correctly clears saved preferences.
        /// </summary>
        [Test]
        public void SettingsTests_Reset_ClearsAllPreferences()
        {
            _manager.SetVolume(0.1f);
            _manager.ResetToDefaults();

            Assert.IsFalse(PlayerPrefs.HasKey("Settings_Volume"));
        }
    }
}
