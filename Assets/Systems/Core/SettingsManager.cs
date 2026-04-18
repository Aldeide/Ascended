using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Systems.Core
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        [Header("Audio")]
        [SerializeField] private UnityEngine.Audio.AudioMixer audioMixer;
        private const string MIXER_EXPOSED_PARAM = "MasterVolume";

        [Header("Input")]
        [SerializeField] private InputActionAsset inputActions;

        private const string QUALITY_KEY = "Settings_Quality";
        private const string VSYNC_KEY = "Settings_VSync";
        private const string VOLUME_KEY = "Settings_Volume";
        private const string REBIND_KEY = "Settings_Rebinds";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadSettings();
        }

        public void SetQuality(int index)
        {
            QualitySettings.SetQualityLevel(index);
            PlayerPrefs.SetInt(QUALITY_KEY, index);
        }

        public void SetVSync(bool enabled)
        {
            QualitySettings.vSyncCount = enabled ? 1 : 0;
            PlayerPrefs.SetInt(VSYNC_KEY, enabled ? 1 : 0);
        }

        public void SetVolume(float volume)
        {
            AudioListener.volume = volume;
            PlayerPrefs.SetFloat(VOLUME_KEY, volume);
            
            if (audioMixer != null)
            {
                // Convert linear 0-1 to decibels -80 to 0
                float db = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
                audioMixer.SetFloat(MIXER_EXPOSED_PARAM, db);
            }
        }

        public void SetResolution(int width, int height, bool fullScreen)
        {
            Screen.SetResolution(width, height, fullScreen);
        }

        public void SaveRebinds()
        {
            if (inputActions == null) return;
            string rebinds = inputActions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString(REBIND_KEY, rebinds);
        }

        private void LoadSettings()
        {
            // Quality
            int quality = PlayerPrefs.GetInt(QUALITY_KEY, QualitySettings.GetQualityLevel());
            QualitySettings.SetQualityLevel(quality, true);

            // VSync
            int vsync = PlayerPrefs.GetInt(VSYNC_KEY, QualitySettings.vSyncCount);
            QualitySettings.vSyncCount = vsync;

            // Volume
            float volume = PlayerPrefs.GetFloat(VOLUME_KEY, AudioListener.volume);
            SetVolume(volume);

            // Rebinds
            if (inputActions != null && PlayerPrefs.HasKey(REBIND_KEY))
            {
                string rebinds = PlayerPrefs.GetString(REBIND_KEY);
                inputActions.LoadBindingOverridesFromJson(rebinds);
            }
        }

        public void ResetToDefaults()
        {
            PlayerPrefs.DeleteAll();
            LoadSettings();
            
            // Clear input overrides
            if (inputActions != null)
            {
                inputActions.RemoveAllBindingOverrides();
            }
        }
    }
}
