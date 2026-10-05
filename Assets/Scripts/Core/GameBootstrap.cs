using MyUnityGame.Gameplay;
using MyUnityGame.UI;
using UnityEngine;

namespace MyUnityGame.Core
{
    public static class GameBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (Object.FindAnyObjectByType<GameManager>() == null)
            {
                var managerObject = new GameObject("GameManager");
                managerObject.AddComponent<GameManager>();
            }

            if (Object.FindAnyObjectByType<PartyRosterPanel>() == null)
            {
                var panelObject = new GameObject("PartyRosterPanel");
                panelObject.AddComponent<PartyRosterPanel>();
            }
        }
    }
}
