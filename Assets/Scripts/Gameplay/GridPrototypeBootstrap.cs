using System.Collections.Generic;
using MyUnityGame.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUnityGame.Gameplay
{
    public class GridPrototypeBootstrap : MonoBehaviour, IGridWorld
    {
        private const string WorldSceneName = "SampleScene";
        private const string SettingsResourcePath = "Gameplay/GridWorldSettings";
        private const float GroundHeight = GridTerrainBuilder.TileThickness * 0.5f;

        public static GridPrototypeBootstrap Instance { get; private set; }

        private HashSet<Vector2Int> blockedCells = new HashSet<Vector2Int>();
        private GridPrototypeAssets assets;
        private GridWorldSettings settings;
        private GameObject demonObject;

        public Vector2Int GridSize => settings.GridSize;
        public Vector2Int PlayerStartCell => settings.PlayerStartCell;
        public Vector2Int DemonCell => settings.DemonCell;
        public IReadOnlyCollection<Vector2Int> BlockedCells => blockedCells;
        public bool DemonAlive => demonObject != null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RegisterSceneBootstrap()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != WorldSceneName || FindAnyObjectByType<GridPlayerController>() != null)
            {
                return;
            }

            var worldObject = new GameObject("AujurlandarPrototype");
            worldObject.AddComponent<GridPrototypeBootstrap>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            settings = Resources.Load<GridWorldSettings>(SettingsResourcePath);
            if (settings == null)
            {
                Debug.LogError($"World settings asset is missing from Resources/{SettingsResourcePath}.", this);
                DisableBootstrap();
                return;
            }

            if (!settings.TryValidate(out var validationError))
            {
                Debug.LogError($"World settings are invalid: {validationError}", settings);
                DisableBootstrap();
                return;
            }

            BuildWorld();
        }

        private void DisableBootstrap()
        {
            if (Instance == this)
            {
                Instance = null;
            }

            enabled = false;
        }

        private void BuildWorld()
        {
            assets = new GridPrototypeAssets();
            GridTerrainBuilder.CreateLighting(transform);
            GridTerrainBuilder.CreateGround(transform, settings.GridSize, assets.GrassMaterial);
            blockedCells = GridTerrainBuilder.CreateObstacles(
                transform,
                GetObstacleCells(),
                this,
                assets.RockMaterial);
            var player = GridActorFactory.CreatePlayer(transform, settings.PlayerStartCell, this, settings);
            if (GameManager.Instance == null || !GameManager.Instance.IsEncounterDefeated(settings.EncounterName))
            {
                demonObject = GridActorFactory.CreateDemon(
                    transform,
                    DemonCell,
                    this,
                    player,
                    settings.EncounterName,
                    settings.WildDemon,
                    assets.CreateDemonMaterial());
            }

            GridActorFactory.CreateCamera(player.transform);
        }

        private List<Vector2Int> GetObstacleCells()
        {
            var cells = new List<Vector2Int>(settings.ObstacleOffsets.Count);
            foreach (var offset in settings.ObstacleOffsets)
            {
                cells.Add(settings.PlayerStartCell + offset);
            }

            return cells;
        }

        public Vector3 GridToWorld(Vector2Int cell)
        {
            return transform.TransformPoint(new Vector3(cell.x, GroundHeight, cell.y));
        }

        public bool IsWithinGrid(Vector2Int cell)
        {
            var size = settings != null ? settings.GridSize : Vector2Int.zero;
            return cell.x >= 0 && cell.x < size.x && cell.y >= 0 && cell.y < size.y;
        }

        public bool IsBlocked(Vector2Int cell)
        {
            return blockedCells.Contains(cell) || (DemonAlive && cell == DemonCell);
        }

        private void OnDestroy()
        {
            assets?.Dispose();
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
