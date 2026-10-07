using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUnityGame.Gameplay
{
    public class GridPrototypeBootstrap : MonoBehaviour
    {
        private const float TileThickness = 0.2f;
        private const float SpriteGroundClearance = 0.04f;
        private const float CameraFocusHeight = 0.75f;
        private static readonly Vector3 CameraFollowOffset = new Vector3(0f, 7.5f, -6.5f);

        public static GridPrototypeBootstrap Instance { get; private set; }

        [SerializeField] private Vector2Int gridSize = new Vector2Int(35, 35);
        [SerializeField] private Vector2Int playerStartCell = new Vector2Int(17, 17);
        [SerializeField] private Vector2Int demonCell = new Vector2Int(21, 20);

        private readonly HashSet<Vector2Int> blockedCells = new HashSet<Vector2Int>();
        private Texture2D grassTexture;
        private Texture2D rockTexture;
        private Material grassMaterial;
        private Material rockMaterial;
        private GameObject demonObject;
        private Transform playerTransform;
        private Camera worldCamera;

        public IReadOnlyCollection<Vector2Int> BlockedCells => blockedCells;
        public bool DemonAlive => demonObject != null;

        public Vector2Int GridSize => gridSize;
        public Vector2Int PlayerStartCell => playerStartCell;
        public Vector2Int DemonCell => demonCell;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void SpawnPrototype()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "SampleScene" || FindAnyObjectByType<GridPlayerController>() != null)
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
            CreatePixelMaterials();
            CreateLighting();
            CreateGrid();
            CreateBlockedTerrain();
            CreatePartyManager();
            CreatePartyMenu();
            CreateBattleManager();
            CreatePlayer();
            CreateDemon();
            CreateCamera();
        }

        private void CreatePixelMaterials()
        {
            grassTexture = CreateGrassTexture();
            rockTexture = CreateRockTexture();
            grassMaterial = CreatePixelMaterial(grassTexture);
            rockMaterial = CreatePixelMaterial(rockTexture);
        }

        private static Material CreatePixelMaterial(Texture2D texture)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                Debug.LogError("Unable to find the URP Lit shader for the pixel-art overworld.");
                Destroy(texture);
                return null;
            }

            var material = new Material(shader);
            material.SetTexture("_BaseMap", texture);
            material.SetColor("_BaseColor", Color.white);
            material.SetFloat("_Smoothness", 0f);
            material.SetFloat("_Metallic", 0f);
            return material;
        }

        private static Texture2D CreateGrassTexture()
        {
            var colors = new[]
            {
                new Color32(43, 73, 47, 255),
                new Color32(52, 86, 52, 255),
                new Color32(65, 100, 59, 255),
                new Color32(77, 111, 65, 255)
            };
            var texture = CreatePixelTexture(64, 64, (x, y) =>
            {
                var patch = PixelHash(x / 4, y / 4, 17) & int.MaxValue;
                if (patch % 13 == 0)
                {
                    return colors[3];
                }

                if (patch % 7 == 0)
                {
                    return colors[2];
                }

                return (PixelHash(x, y, 31) & int.MaxValue) % 19 == 0
                    ? colors[0]
                    : colors[1];
            });
            return texture;
        }

        private static Texture2D CreateRockTexture()
        {
            var colors = new[]
            {
                new Color32(67, 62, 61, 255),
                new Color32(91, 83, 78, 255),
                new Color32(112, 101, 91, 255),
                new Color32(132, 119, 104, 255)
            };
            return CreatePixelTexture(16, 16, (x, y) =>
            {
                var shade = (PixelHash(x / 2, y / 2, 53) & int.MaxValue) % 9;
                return colors[shade < 4 ? 0 : shade < 7 ? 1 : shade == 7 ? 2 : 3];
            });
        }

        private static Texture2D CreatePixelTexture(int width, int height, System.Func<int, int, Color32> getPixel)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = "PixelArtTexture",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Repeat,
                anisoLevel = 0
            };
            var pixels = new Color32[width * height];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    pixels[y * width + x] = getPixel(x, y);
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            return texture;
        }

        private static int PixelHash(int x, int y, int seed)
        {
            unchecked
            {
                var value = x * 374761393 + y * 668265263 + seed * 1442695041;
                value = (value ^ (value >> 13)) * 1274126177;
                return value ^ (value >> 16);
            }
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
                    tile.transform.localScale = new Vector3(1f, TileThickness, 1f);

                    var renderer = tile.GetComponent<Renderer>();
                    renderer.sharedMaterial = grassMaterial;
                    var tint = new MaterialPropertyBlock();
                    tint.SetColor("_BaseColor", z % 2 == 0 ? new Color(0.94f, 1f, 0.94f) : Color.white);
                    renderer.SetPropertyBlock(tint);
                }
            }
        }

        private void CreateBlockedTerrain()
        {
            var obstacles = new[]
            {
                new Vector2Int(18, 17),
                new Vector2Int(19, 17),
                new Vector2Int(18, 19),
                new Vector2Int(20, 19),
                new Vector2Int(19, 21),
                new Vector2Int(21, 21),
                new Vector2Int(17, 21),
                new Vector2Int(21, 17)
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
                renderer.sharedMaterial = rockMaterial;
            }
        }

        private void CreatePartyManager()
        {
            var partyManagerObject = new GameObject("PartyManager");
            partyManagerObject.transform.SetParent(transform);
            partyManagerObject.AddComponent<PartyManager>();
        }

        private void CreatePartyMenu()
        {
            if (FindAnyObjectByType<MyUnityGame.UI.PartyRosterPanel>() != null)
            {
                return;
            }

            var partyMenuObject = new GameObject("PartyMenu");
            partyMenuObject.transform.SetParent(transform);
            partyMenuObject.AddComponent<PartyMenu>();
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
            playerTransform = player.transform;

            var rigidbody = player.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            rigidbody.useGravity = false;

            player.GetComponent<MeshRenderer>().enabled = false;
            var playerCollider = player.GetComponent<CapsuleCollider>();
            playerCollider.radius = 0.25f;
            playerCollider.height = 0.7f;
            playerCollider.center = new Vector3(0f, 0.35f, 0f);

            var sprite = Resources.Load<Sprite>("Characters/MainCharacter");
            if (sprite == null)
            {
                Debug.LogError("Main character sprite is missing at Resources/Characters/MainCharacter.png.");
            }
            else
            {
                var spriteObject = new GameObject("MainCharacterSprite");
                spriteObject.transform.SetParent(player.transform, false);
                spriteObject.transform.localPosition = new Vector3(0f, SpriteGroundClearance, 0f);

                var spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprite;
                spriteRenderer.sortingOrder = 1;

                var walkFrames = Resources.LoadAll<Sprite>("Characters/Walk");
                Array.Sort(walkFrames, (left, right) => string.CompareOrdinal(left.name, right.name));
                var animator = spriteObject.AddComponent<SpriteWalkAnimator>();
                animator.Initialize(spriteRenderer, sprite, walkFrames);

                var upwardWalkFrames = Resources.LoadAll<Sprite>("Characters/WalkUp");
                Array.Sort(upwardWalkFrames, (left, right) => string.CompareOrdinal(left.name, right.name));
                animator.SetUpwardWalkFrames(upwardWalkFrames);

                var leftWalkFrames = Resources.LoadAll<Sprite>("Characters/WalkLeft");
                Array.Sort(leftWalkFrames, (left, right) => string.CompareOrdinal(left.name, right.name));
                var rightWalkFrames = Resources.LoadAll<Sprite>("Characters/WalkRight");
                Array.Sort(rightWalkFrames, (left, right) => string.CompareOrdinal(left.name, right.name));
                animator.SetHorizontalWalkFrames(leftWalkFrames, rightWalkFrames);
            }

            var controller = player.AddComponent<GridPlayerController>();
            controller.Initialize(playerStartCell, this);
        }

        private void CreateDemon()
        {
            var demon = GameObject.CreatePrimitive(PrimitiveType.Cube);
            demon.name = "DemonEncounter";
            demon.transform.SetParent(transform);
            demon.transform.position = GridToWorld(demonCell) + Vector3.up * 0.6f;
            demon.transform.localScale = new Vector3(0.7f, 1.2f, 0.7f);
            demonObject = demon;

            var collider = demon.AddComponent<BoxCollider>();
            collider.isTrigger = true;

            var renderer = demon.GetComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            renderer.material.SetColor("_BaseColor", new Color(0.82f, 0.18f, 0.22f));

            demon.AddComponent<GridEncounterTrigger>();
        }

        private void CreateCamera()
        {
            worldCamera = Camera.main;
            if (worldCamera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                cameraObject.transform.SetParent(transform);
                worldCamera = cameraObject.AddComponent<Camera>();
            }

            worldCamera.clearFlags = CameraClearFlags.SolidColor;
            worldCamera.backgroundColor = new Color(0.09f, 0.10f, 0.15f);
            worldCamera.fieldOfView = 38f;
            worldCamera.enabled = true;
            worldCamera.gameObject.SetActive(true);
            worldCamera.transform.SetParent(playerTransform, true);
            var focusPosition = playerTransform.position + Vector3.up * CameraFocusHeight;
            worldCamera.transform.position = focusPosition + CameraFollowOffset;
            worldCamera.transform.LookAt(focusPosition);
            worldCamera.transform.localScale = Vector3.one;
        }

        public Vector3 GridToWorld(Vector2Int cell)
        {
            return new Vector3(cell.x, TileThickness * 0.5f, cell.y);
        }

        public bool IsWithinGrid(Vector2Int cell)
        {
            return cell.x >= 0 && cell.x < gridSize.x && cell.y >= 0 && cell.y < gridSize.y;
        }

        public bool IsBlocked(Vector2Int cell)
        {
            if (blockedCells.Contains(cell))
            {
                return true;
            }

            return DemonAlive && cell == demonCell;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }

            if (grassMaterial != null)
            {
                Destroy(grassMaterial);
            }

            if (rockMaterial != null)
            {
                Destroy(rockMaterial);
            }

            if (grassTexture != null)
            {
                Destroy(grassTexture);
            }

            if (rockTexture != null)
            {
                Destroy(rockTexture);
            }
        }
    }
}
