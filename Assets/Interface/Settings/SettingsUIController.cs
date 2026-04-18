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
            if (_root == null) return;

            // Find Elements with null checks
            _resDropdown = _root.Q<DropdownField>("resolution-dropdown");
            _qualityDropdown = _root.Q<DropdownField>("quality-dropdown");
            _vsyncToggle = _root.Q<Toggle>("vsync-toggle");
            _volumeSlider = _root.Q<Slider>("volume-slider");
            _keybindingsList = _root.Q<VisualElement>("keybindings-list");
            _resetBtn = _root.Q<Button>("reset-rebinds-btn");

            if (_resDropdown == null || _qualityDropdown == null || _vsyncToggle == null || _volumeSlider == null)
            {
                Debug.LogWarning("[Settings] Some UI elements were not found in the UXML.");
                return;
            }

            InitializeGraphics();
            InitializeAudio();
            InitializeKeybindings();

            if (_resetBtn != null)
            {
                _resetBtn.clicked -= OnResetClicked;
                _resetBtn.clicked += OnResetClicked;
            }
        }

        private void InitializeGraphics()
        {
            // Resolutions
            _resolutions = Screen.resolutions.Select(r => r).Reverse().ToArray();
            var resChoices = _resolutions.Select(r => $"{r.width}x{r.height} @ {r.refreshRateRatio.value:F0}Hz").ToList();
            _resDropdown.choices = resChoices;
            
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
            
            // Use Unregister if needed? No, Register is fine if we check for changes
            _resDropdown.RegisterValueChangedCallback(evt => {
                if (_resDropdown.index >= 0 && _resDropdown.index < _resolutions.Length)
                {
                    var res = _resolutions[_resDropdown.index];
                    SettingsManager.Instance.SetResolution(res.width, res.height, Screen.fullScreen);
                }
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
            if (inputActions == null || _keybindingsList == null || rebindRowTemplate == null) return;

            _keybindingsList.Clear();

            // Try to find the Player map, or use the first one available
            var map = inputActions.FindActionMap("Player") ?? inputActions.actionMaps.FirstOrDefault();
            if (map == null) return;

            foreach (var action in map.actions)
            {
                // Skip if no bindings
                if (action.bindings.Count == 0) continue;

                var row = rebindRowTemplate.Instantiate();
                var label = row.Q<Label>("action-name");
                var button = row.Q<Button>("rebind-btn");

                if (label != null) label.text = action.name.ToUpper();
                if (button != null)
                {
                    button.text = action.bindings[0].ToDisplayString();
                    button.clicked += () => StartRebind(action, button);
                }
                
                _keybindingsList.Add(row);
            }
        }

        private void StartRebind(InputAction action, Button button)
        {
            button.text = "PRESS ANY KEY...";
            button.SetEnabled(false);

            // Disable all actions while rebinding to prevent accidental triggers
            inputActions.Disable();

            var rebindOperation = action.PerformInteractiveRebinding()
                .WithControlsExcluding("<Pointer>/position")
                .WithControlsExcluding("<Pointer>/delta")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => {
                    button.text = action.bindings[0].ToDisplayString();
                    button.SetEnabled(true);
                    SettingsManager.Instance.SaveRebinds();
                    inputActions.Enable();
                    operation.Dispose();
                })
                .OnCancel(operation => {
                    button.text = action.bindings[0].ToDisplayString();
                    button.SetEnabled(true);
                    inputActions.Enable();
                    operation.Dispose();
                })
                .Start();
        }

        private void OnResetClicked()
        {
            SettingsManager.Instance.ResetToDefaults();
            // Refresh UI values
            _vsyncToggle.value = QualitySettings.vSyncCount > 0;
            _volumeSlider.value = AudioListener.volume;
            _qualityDropdown.index = QualitySettings.GetQualityLevel();
            InitializeKeybindings();
        }
    }
}
