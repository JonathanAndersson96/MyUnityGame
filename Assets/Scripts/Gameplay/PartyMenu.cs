using MyUnityGame.Core;
using MyUnityGame.UI;
using UnityEngine;

namespace MyUnityGame.Gameplay
{
    public class PartyMenu : MonoBehaviour
    {
        private PartyRosterPanel panel;

        private void Awake()
        {
            if (FindAnyObjectByType<PartyRosterPanel>() != null)
            {
                panel = FindAnyObjectByType<PartyRosterPanel>();
            }
        }

        private void Update()
        {
            if (panel != null)
            {
                return;
            }
        }

        private void OnGUI()
        {
            if (panel != null)
            {
                return;
            }

            if (GameManager.Instance == null || GameManager.Instance.PartyRoster == null)
            {
                return;
            }

            var box = new Rect(Screen.width * 0.5f - 200f, Screen.height * 0.5f - 180f, 400f, 360f);
            GUI.Box(box, "Party");

            GUILayout.BeginArea(box);
            try
            {
                GUILayout.Space(28f);
                GUILayout.Label($"Phase: {GameManager.Instance.CurrentPhase}");

                foreach (var member in GameManager.Instance.PartyRoster.Members)
                {
                    GUILayout.Label($"{member.Name}: {member.CurrentHp}/{member.MaxHp}");
                }
            }
            finally
            {
                GUILayout.EndArea();
            }
        }
    }
}
