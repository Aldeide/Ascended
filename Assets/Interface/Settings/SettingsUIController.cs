using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Systems.Core;
using UnityEngine.InputSystem;
using System.Linq;

namespace Interface.Settings
{
    public class SettingsUIController : MonoBehaviour
    {
        [Header("UI Templates")]
        [SerializeField] private VisualTreeAsset rebindRowTemplate;

        [Header("Input Asset")]
        [SerializeField] private InputActionAsset inputActions;

        private VisualElement _root;
        private DropdownField _resDropdown;
        private DropdownField _qualityDropdown;
        private Toggle _vsyncToggle;
        private Slider _volumeSlider;
        private VisualElement _keybindingsList;
        private Button _resetBtn;

        private Resolution[] _resolutions;

        private void OnEnable()
        {
            var uiDoc = GetComponent<UIDocument>();
            if (uiDoc == null) return;
            
            _root = uiDoc.rootVisualElement;
            _root.RegisterCallback<AttachToPanelEvent>(evt => InitializeUI());
        }

        private void InitializeUI()
        {
            // Find Elements
            _resDropdown = _root.Q<DropdownField>("resolution-dropdown");
            _qualityDropdown = _root.Q<DropdownField>("quality-dropdown");
            _vsyncToggle = _root.Q<Toggle>("vsync-toggle");
            _volumeSlider = _root.Q<Slider>("volume-slider");
            _keybindingsList = _root.Q<VisualElement>("keybindings-list");
            _resetBtn = _root.Q<Button>("reset-rebinds-btn");

            if (_resDropdown == null) return; // Haven't loaded yet?

            InitializeGraphics();
            InitializeAudio();
            InitializeKeybindings();

            _resetBtn.clicked -= OnResetClicked; // Prevent double registration
            _resetBtn.clicked += OnResetClicked;
        }

        private void InitializeGraphics()
        {
            // Resolutions
            _resolutions = Screen.resolutions.Select(r => r).Reverse().ToArray();
            _resDropdown.choices = _resolutions.Select(r => $"{r.width}x{r.height} @ {r.refreshRateRatio.value:F0}Hz").ToList();
            
            // Find current resolution index
            int currentResIndex = 0;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height)
                {
                    currentResIndex = i;
                    break;
                }
            }
            _resDropdown.index = currentResIndex;
            _resDropdown.RegisterValueChangedCallback(evt => {
                var res = _resolutions[_resDropdown.index];
                SettingsManager.Instance.SetResolution(res.width, res.height, Screen.fullScreen);
            });

            // Quality
            _qualityDropdown.choices = QualitySettings.names.ToList();
            _qualityDropdown.index = QualitySettings.GetQualityLevel();
            _qualityDropdown.RegisterValueChangedCallback(evt => {
                SettingsManager.Instance.SetQuality(_qualityDropdown.index);
            });

            // VSync
            _vsyncToggle.value = QualitySettings.vSyncCount > 0;
            _vsyncToggle.RegisterValueChangedCallback(evt => {
                SettingsManager.Instance.SetVSync(evt.newValue);
            });
        }

        private void InitializeAudio()
        {
            _volumeSlider.value = AudioListener.volume;
            _volumeSlider.RegisterValueChangedCallback(evt => {
                SettingsManager.Instance.SetVolume(evt.newValue);
            });
        }

        private void InitializeKeybindings()
        {
            if (inputActions == null || _keybindingsList == null) return;

            _keybindingsList.Clear();

            foreach (var action in inputActions.FindActionMap("Player").actions)
            {
                var row = rebindRowTemplate.Instantiate();
                var label = row.Q<Label>("action-name");
                var button = row.Q<Button>("rebind-btn");

                label.text = action.name.ToUpper();
                button.text = action.bindings[0].ToDisplayString();

                button.clicked += () => StartRebind(action, button);
                _keybindingsList.Add(row);
            }
        }

        private void StartRebind(InputAction action, Button button)
        {
            button.text = "PRESS ANY KEY...";
            button.SetEnabled(false);

            var rebindOperation = action.PerformInteractiveRebinding()
                .WithControlsExcluding("<Pointer>/position")
                .WithControlsExcluding("<Pointer>/delta")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => {
                    button.text = action.bindings[0].ToDisplayString();
                    button.SetEnabled(true);
                    SettingsManager.Instance.SaveRebinds();
                    operation.Dispose();
                })
                .Start();
        }

        private void OnResetClicked()
        {
            SettingsManager.Instance.ResetToDefaults();
            // Refresh UI
            InitializeGraphics();
            InitializeAudio();
            InitializeKeybindings();
        }
    }
}
