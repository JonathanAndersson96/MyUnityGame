using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MyUnityGame.Gameplay
{
    public class GridEncounterTrigger : MonoBehaviour
    {
        private GridPlayerController player;
        private bool playerInRange;

        private void Update()
        {
            if (player == null)
            {
                player = GameObject.FindWithTag("Player")?.GetComponent<GridPlayerController>();
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

            var inRange = player.CurrentCell == activeWorld.DemonCell ||
                          Mathf.Abs(player.CurrentCell.x - activeWorld.DemonCell.x) <= 1 &&
                          Mathf.Abs(player.CurrentCell.y - activeWorld.DemonCell.y) <= 1;

            playerInRange = inRange;

            var material = GetComponent<Renderer>().material;
            material.SetColor("_BaseColor", inRange ? new Color(1f, 0.38f, 0.38f) : new Color(0.82f, 0.18f, 0.22f));

            var keyboard = Keyboard.current;
            if (inRange && keyboard != null && keyboard.eKey.wasPressedThisFrame)
            {
                const string battleScenePath = "Assets/Scenes/BattleScene.unity";
                if (SceneUtility.GetBuildIndexByScenePath(battleScenePath) >= 0)
                {
                    var gameManager = MyUnityGame.Core.GameManager.Instance;
                    if (gameManager != null)
                    {
                        gameManager.StartBattle("Demon");
                    }

                    SceneManager.LoadScene("BattleScene");
                }
                else
                {
                    Debug.LogWarning($"Battle scene is not available in Build Settings: {battleScenePath}");
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            playerInRange = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            playerInRange = false;
        }
    }
}
