using System.Collections.Generic;
using UnityEngine;

public class GridPrototypeBootstrap : MonoBehaviour
{
    public static GridPrototypeBootstrap Instance { get; private set; }

    [SerializeField] private Vector2Int gridSize = new Vector2Int(7, 7);
    [SerializeField] private Vector2Int playerStartCell = new Vector2Int(0, 0);
    [SerializeField] private Vector2Int demonCell = new Vector2Int(5, 4);

    private readonly HashSet<Vector2Int> blockedCells = new HashSet<Vector2Int>();

    public IReadOnlyCollection<Vector2Int> BlockedCells => blockedCells;

    public Vector2Int GridSize => gridSize;
    public Vector2Int PlayerStartCell => playerStartCell;
    public Vector2Int DemonCell => demonCell;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void SpawnPrototype()
    {
        if (FindFirstObjectByType<GridPlayerController>() != null)
        {
            return;
        }

        var root = new GameObject("AujurlandarPrototype");
        root.AddComponent<GridPrototypeBootstrap>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        BuildWorld();
    }

    private void BuildWorld()
    {
        CreateLighting();
        CreateGrid();
        CreateBlockedTerrain();
        CreateBattleManager();
        CreatePlayer();
        CreateDemon();
        CreateCamera();
    }

    private void CreateLighting()
    {
        var lightObject = new GameObject("Directional Light");
        lightObject.transform.SetParent(transform);
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        light.color = new Color(1f, 0.97f, 0.9f);
        lightObject.transform.rotation = Quaternion.Euler(45f, 30f, 0f);
    }

    private void CreateGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.name = $"Tile_{x}_{z}";
                tile.transform.SetParent(transform);
                tile.transform.position = new Vector3(x, 0f, z);
                tile.transform.localScale = new Vector3(1f, 0.2f, 1f);

                var renderer = tile.GetComponent<Renderer>();
                renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                renderer.material.SetColor("_BaseColor", z % 2 == 0 ? new Color(0.30f, 0.48f, 0.32f) : new Color(0.35f, 0.52f, 0.37f));
            }
        }
    }

    private void CreateBlockedTerrain()
    {
        var obstacles = new[]
        {
            new Vector2Int(2, 1),
            new Vector2Int(3, 1),
            new Vector2Int(2, 3),
            new Vector2Int(4, 3),
            new Vector2Int(3, 5),
            new Vector2Int(5, 5),
            new Vector2Int(1, 5),
            new Vector2Int(5, 1)
        };

        foreach (var obstacle in obstacles)
        {
            if (!IsWithinGrid(obstacle))
            {
                continue;
            }

            blockedCells.Add(obstacle);

            var rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rock.name = $"Rock_{obstacle.x}_{obstacle.y}";
            rock.transform.SetParent(transform);
            rock.transform.position = new Vector3(obstacle.x, 0.5f, obstacle.y);
            rock.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
            rock.AddComponent<BoxCollider>();

            var renderer = rock.GetComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            renderer.material.SetColor("_BaseColor", new Color(0.42f, 0.38f, 0.36f));
        }
    }

    private void CreateBattleManager()
    {
        var battleManagerObject = new GameObject("BattleManager");
        battleManagerObject.transform.SetParent(transform);
        battleManagerObject.AddComponent<BattleManager>();
    }

    private void CreatePlayer()
    {
        var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.tag = "Player";
        player.name = "Player";
        player.transform.SetParent(transform);
        player.transform.position = GridToWorld(playerStartCell);
        player.transform.localScale = new Vector3(0.55f, 0.7f, 0.55f);
        player.AddComponent<CapsuleCollider>();

        var rigidbody = player.AddComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rigidbody.useGravity = false;

        var renderer = player.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        renderer.material.SetColor("_BaseColor", new Color(0.82f, 0.92f, 1f));

        var controller = player.AddComponent<GridPlayerController>();
        controller.Initialize(playerStartCell, this);
    }

    private void CreateDemon()
    {
        var demon = GameObject.CreatePrimitive(PrimitiveType.Cube);
        demon.name = "DemonEncounter";
        demon.transform.SetParent(transform);
        demon.transform.position = GridToWorld(demonCell);
        demon.transform.localScale = new Vector3(0.7f, 1.2f, 0.7f);

        var collider = demon.AddComponent<BoxCollider>();
        collider.isTrigger = true;

        var renderer = demon.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        renderer.material.SetColor("_BaseColor", new Color(0.82f, 0.18f, 0.22f));

        demon.AddComponent<GridEncounterTrigger>();
    }

    private void CreateCamera()
    {
        var cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.SetParent(transform);
        cameraObject.transform.position = new Vector3(gridSize.x * 0.5f, 7.5f, -6.5f);
        cameraObject.transform.rotation = Quaternion.Euler(33f, 0f, 0f);

        var camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.09f, 0.10f, 0.15f);
        camera.fieldOfView = 38f;
    }

    public Vector3 GridToWorld(Vector2Int cell)
    {
        return new Vector3(cell.x, 0.7f, cell.y);
    }

    public bool IsWithinGrid(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < gridSize.x && cell.y >= 0 && cell.y < gridSize.y;
    }

    public bool IsBlocked(Vector2Int cell)
    {
        return blockedCells.Contains(cell);
    }
}
