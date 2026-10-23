using MyUnityGame.Gameplay;
using MyUnityGame.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUnityGame.Core
{
    public static class GameBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            EnsureCoreServices();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            EnsureCoreServices();
        }

        private static void EnsureCoreServices()
        {
            if (Object.FindAnyObjectByType<GameManager>() == null)
            {
                var managerObject = new GameObject("GameManager");
                managerObject.AddComponent<GameManager>();
            }

            if (PartyManager.Instance == null)
            {
                var partyObject = new GameObject("PartyManager");
                partyObject.AddComponent<PartyManager>();
            }

            if (Object.FindAnyObjectByType<GameMenu>() == null)
            {
                var menuObject = new GameObject("GameMenu");
                menuObject.AddComponent<GameMenu>();
                Object.DontDestroyOnLoad(menuObject);
            }
        }
    }
}
