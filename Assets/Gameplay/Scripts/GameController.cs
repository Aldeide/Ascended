using Gameplay.Runtime.Players;
using UnityEngine;

namespace Gameplay.Scripts
{
    public class GameController : MonoBehaviour
    {
        public Player Player;
        public static GameController Instance
        {
            get
            {
                if(!_instance)
                {
                    _instance = FindFirstObjectByType<GameController>();
                }

                return _instance;
            }
        }
        
        private static GameController _instance;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            Player = new Player("Player");
        }
    }
}