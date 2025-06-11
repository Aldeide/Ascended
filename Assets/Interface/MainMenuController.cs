using UnityEngine;
using UnityEngine.UIElements;

namespace Interface
{
    public class MainMenuController : MonoBehaviour
    {
        public UIDocument SettingsMenu;
        public UIDocument _document;
        private Button _playButton;
        private Button _settingsButton;

        private Label _mainMenu;
        private TemplateContainer _settingsTemplate;
        private void Start()
        {
            _document = GetComponent<UIDocument>();
            SettingsMenu.rootVisualElement.visible = false;
            _playButton = _document.rootVisualElement.Q<Button>("PlayButton");
            _settingsButton = _document.rootVisualElement.Q<Button>("SettingsButton");
            
            _playButton.clicked += OnPlayButtonClicked;
            _settingsButton.clicked += OnSettingsButtonClicked;

            _mainMenu = _document.rootVisualElement.Q<Label>("MainMenu");
            _settingsTemplate = _document.rootVisualElement.Q<TemplateContainer>("SettingsTemplate");
            _settingsTemplate.visible = false;
            _mainMenu.visible = true;
        }

        private static void OnPlayButtonClicked()
        {
            Debug.Log("PlayButtonClicked");
        }
        
        private void OnSettingsButtonClicked()
        {
            _mainMenu.visible = false;
            _settingsTemplate.visible = true;
            Debug.Log("SettingsButtonClicked");
            //_document.rootVisualElement.visible = false;
        }

        private void OnQuiteButtonClicked()
        {
            Debug.Log("QuitButtonClicked");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}