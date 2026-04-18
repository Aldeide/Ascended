using NUnit.Framework;
using UnityEngine;
using Systems.Core;
using UnityEngine.InputSystem;

namespace Systems.Core.Tests
{
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
            Object.DestroyImmediate(_settingsGO);
            PlayerPrefs.DeleteAll();
        }

        [Test]
        public void Settings_SetVolume_PersistsAndLoads()
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

        [Test]
        public void Settings_SetQuality_Persists()
        {
            int targetQuality = 1; // Example index
            _manager.SetQuality(targetQuality);

            Assert.AreEqual(targetQuality, QualitySettings.GetQualityLevel());
            Assert.AreEqual(targetQuality, PlayerPrefs.GetInt("Settings_Quality"));
        }

        [Test]
        public void Settings_Reset_ClearsPreferences()
        {
            _manager.SetVolume(0.1f);
            _manager.ResetToDefaults();

            // Default volume is typically 1.0f or whatever it was at build
            // But we know PlayerPrefs should be empty
            Assert.IsFalse(PlayerPrefs.HasKey("Settings_Volume"));
        }
    }
}
