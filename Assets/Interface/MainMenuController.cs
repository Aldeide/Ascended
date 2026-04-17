using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace Assets.Interface
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI Document")]
        [SerializeField] private UIDocument uiDocument;

        [Header("Settings UI")]
        [SerializeField] private UIDocument settingsUIDocument;

        private VisualElement _root;
        private VisualElement _contentArea;
        
        private Button _playBtn;
        private Button _settingsBtn;
        private Button _quitBtn;

        private void OnEnable()
        {
            _root = uiDocument.rootVisualElement;

            // Find buttons
            _playBtn = _root.Q<Button>("play-btn");
            _settingsBtn = _root.Q<Button>("settings-btn");
            _quitBtn = _root.Q<Button>("quit-btn");
            _contentArea = _root.Q<VisualElement>("content-area");

            // Register events
            _playBtn.clicked += OnPlayClicked;
            _settingsBtn.clicked += OnSettingsClicked;
            _quitBtn.clicked += OnQuitClicked;
            
            // Initial State: Ensure settings document is hidden if it exists
            if (settingsUIDocument != null)
            {
                settingsUIDocument.rootVisualElement.style.display = DisplayStyle.None;
            }
        }

        private void OnPlayClicked()
        {
            Debug.Log("Loading SampleScene...");
            SceneManager.LoadScene("SampleScene");
        }

        private void OnSettingsClicked()
        {
            ToggleSettings();
        }

        private void OnQuitClicked()
        {
            Debug.Log("Quitting application...");
            Application.Quit();
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }

        private void ToggleSettings()
        {
            if (settingsUIDocument == null) return;

            var settingsRoot = settingsUIDocument.rootVisualElement;
            if (settingsRoot.style.display == DisplayStyle.None)
            {
                settingsRoot.style.display = DisplayStyle.Flex;
            }
            else
            {
                settingsRoot.style.display = DisplayStyle.None;
            }
        }
    }
}