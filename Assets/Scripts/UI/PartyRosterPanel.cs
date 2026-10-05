using MyUnityGame.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyUnityGame.UI
{
    public class PartyRosterPanel : MonoBehaviour
    {
        private bool isOpen;

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.tabKey.wasPressedThisFrame)
            {
                isOpen = !isOpen;
            }
        }

        private void OnGUI()
        {
            if (!isOpen)
            {
                return;
            }

            var box = new Rect(Screen.width * 0.5f - 200f, Screen.height * 0.5f - 180f, 400f, 360f);
            GUI.Box(box, "Party");

            GUILayout.BeginArea(box);
            try
            {
                GUILayout.Space(28f);

                var gameManager = GameManager.Instance;
                if (gameManager == null || gameManager.PartyRoster == null || gameManager.PartyRoster.Members == null)
                {
                    GUILayout.Label("No party loaded.");
                    return;
                }

                GUILayout.Label($"Phase: {gameManager.CurrentPhase}");
                if (!string.IsNullOrEmpty(gameManager.CurrentEncounter))
                {
                    GUILayout.Label($"Encounter: {gameManager.CurrentEncounter}");
                }

                foreach (var member in gameManager.PartyRoster.Members)
                {
                    GUILayout.Label($"{member.Name}: {member.CurrentHp}/{member.MaxHp}");
                }

                if (GUILayout.Button("Close"))
                {
                    isOpen = false;
                }
            }
            finally
            {
                GUILayout.EndArea();
            }
        }
    }
}
