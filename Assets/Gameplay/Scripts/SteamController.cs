using System;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.Scripts
{
    public class SteamController : MonoBehaviour
    {
        private const int AppId = 3824900;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            try
            {
                SteamClient.Init( AppId, true );
            }
            catch ( System.Exception e )
            {
                Debug.Log(e.Message);
                return;
            }
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            SetRichPresence();
        }

        private void Update()
        {
            SteamClient.RunCallbacks();
        }
        
        private void OnDisable()
        {
            SteamClient.Shutdown();
        }

        public void SetRichPresence()
        {
            SteamFriends.SetRichPresence("#Status_AtMainMenu", "In Main Menu");
            Debug.Log("Rich Presence: " + SteamFriends.GetRichPresence("#Status_AtMainMenu"));
        }
    }
}