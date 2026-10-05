using UnityEngine;

public class GridEncounterTrigger : MonoBehaviour
{
    private GridPlayerController player;

    private void Update()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<GridPlayerController>();
        }

        if (player == null)
        {
            return;
        }

        var activeWorld = GridPrototypeBootstrap.Instance;
        if (activeWorld == null)
        {
            return;
        }

        if (player.CurrentCell == activeWorld.DemonCell)
        {
            var material = GetComponent<Renderer>().material;
            material.SetColor("_BaseColor", new Color(1f, 0.38f, 0.38f));

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Encounter triggered: demon in the wild. Battle state ready.");
            }
        }
        else
        {
            var material = GetComponent<Renderer>().material;
            material.SetColor("_BaseColor", new Color(0.82f, 0.18f, 0.22f));
        }
    }
}
