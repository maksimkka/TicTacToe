using UnityEngine;

namespace Code.Main
{
    [DisallowMultipleComponent]
    public sealed class LevelInstance : MonoBehaviour
    {
        public static LevelInstance Instance { get; private set; }
        public GameMode CurrentGameMode { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetGameMode(GameMode mode)
        {
            CurrentGameMode = mode;
        }
    }
}