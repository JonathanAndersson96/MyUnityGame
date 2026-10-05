using UnityEngine;

public class PartyMenu : MonoBehaviour
{
    private bool isOpen;

    public bool IsOpen => isOpen;

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.tabKey.wasPressedThisFrame)
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

            if (PartyManager.Instance == null)
            {
                GUILayout.Label("No party loaded.");
                return;
            }

            foreach (var member in PartyManager.Instance.Members)
            {
                GUILayout.Label(member.HpText);
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
