using UnityEngine;

namespace Gameplay.Scripts
{
    public class SteamController : MonoBehaviour
    {
        private void Awake()
        {
            try
            {
                // TODO(antho): Uncomment once we have a steam app id.
                // Steamworks.SteamClient.Init( 252490, true );
            }
            catch ( System.Exception e )
            {
                // Something went wrong! Steam is closed?
            }
        }

        private void Update()
        {
            Steamworks.SteamClient.RunCallbacks();
        }
        
        private void OnDisable()
        {
            SteamClient.Shutdown();
        }
    }
}