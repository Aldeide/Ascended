using System;
using Steamworks;
using UnityEngine;

namespace MatchmakingSystem.Runtime
{
    /// <summary>
    /// Handles the initialization and cleanup of the Steamworks API via Facepunch.
    /// This should be placed on a singleton or persistent object in the starting scene.
    /// </summary>
    public class SteamManager : MonoBehaviour
    {
        public static SteamManager Instance { get; private set; }

        [Tooltip("The Steam App ID to use for initialization.")]
        public uint AppId = 3824900;

        /// <summary>
        /// True if Steam Client is valid and initialized.
        /// </summary>
        public bool IsValid => SteamClient.IsValid;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            try
            {
                // Initialize the Steam client using the provided App ID
                SteamClient.Init(AppId, true);

                if (SteamClient.IsValid)
                {
                    Debug.Log($"[SteamManager] Initialized successfully. Logged in as: {SteamClient.Name} (ID: {SteamClient.SteamId})");
                }
                else
                {
                    Debug.LogWarning("[SteamManager] Initialization returned invalid client. Steam might not be running.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SteamManager] Error initializing Steamworks: {e.Message}");
            }
        }

        private void Update()
        {
            if (SteamClient.IsValid)
            {
                // Run Steam callbacks
                SteamClient.RunCallbacks();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                if (SteamClient.IsValid)
                {
                    SteamClient.Shutdown();
                    Debug.Log("[SteamManager] Steamworks Shutdown.");
                }
                Instance = null;
            }
        }
    }
}
