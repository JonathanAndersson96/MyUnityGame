using System;
using MyUnityGame.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MyUnityGame.Gameplay
{
    [RequireComponent(typeof(BoxCollider), typeof(Renderer))]
    public class GridEncounterTrigger : MonoBehaviour
    {
        private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");
        private static readonly Color IdleColor = new Color(0.82f, 0.18f, 0.22f);
        private static readonly Color NearbyColor = new Color(1f, 0.38f, 0.38f);
        private const string BattleScenePath = "Assets/Scenes/BattleScene.unity";
        private const int InteractionDistance = 1;

        private GridPlayerController player;
        private Renderer targetRenderer;
        private MaterialPropertyBlock materialProperties;
        private Vector2Int encounterCell;
        private Vector2Int lastPlayerCell;
        private string encounterName;
        private bool hasPlayerCell;
        private bool playerInRange;

        private void Awake()
        {
            targetRenderer = GetComponent<Renderer>();
            materialProperties = new MaterialPropertyBlock();
        }

        public void Initialize(GridPlayerController encounterPlayer, Vector2Int cell, string name)
        {
            player = encounterPlayer ?? throw new ArgumentNullException(nameof(encounterPlayer));
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("An encounter name is required.", nameof(name));
            }

            encounterCell = cell;
            encounterName = name;
            UpdatePlayerRange();
        }

        private void Update()
        {
            if (player == null)
            {
                return;
            }

            if (!hasPlayerCell || player.CurrentCell != lastPlayerCell)
            {
                UpdatePlayerRange();
            }

            var keyboard = Keyboard.current;
            if (playerInRange && keyboard != null && keyboard.eKey.wasPressedThisFrame &&
                (GameManager.Instance == null || !GameManager.Instance.IsMenuOpen))
            {
                EnterBattle();
            }
        }

        private void UpdatePlayerRange()
        {
            lastPlayerCell = player.CurrentCell;
            hasPlayerCell = true;
            playerInRange =
                Mathf.Abs(lastPlayerCell.x - encounterCell.x) <= InteractionDistance &&
                Mathf.Abs(lastPlayerCell.y - encounterCell.y) <= InteractionDistance;

            targetRenderer.GetPropertyBlock(materialProperties);
            materialProperties.SetColor(BaseColorProperty, playerInRange ? NearbyColor : IdleColor);
            targetRenderer.SetPropertyBlock(materialProperties);
        }

        private void EnterBattle()
        {
            var battleSceneIndex = SceneUtility.GetBuildIndexByScenePath(BattleScenePath);
            if (battleSceneIndex < 0)
            {
                Debug.LogWarning($"Battle scene is not available in Build Settings: {BattleScenePath}", this);
                return;
            }

            if (GameManager.Instance == null)
            {
                Debug.LogError("Cannot enter the battle scene because the game manager is unavailable.", this);
                return;
            }

            GameManager.Instance.PrepareEncounter(encounterName);
            SceneManager.LoadScene(battleSceneIndex);
        }
    }
}
