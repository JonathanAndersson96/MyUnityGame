using System.Collections.Generic;
using UnityEngine;

namespace MyUnityGame.Gameplay
{
    internal static class GridTerrainBuilder
    {
        public const float TileThickness = 0.2f;

        public static void CreateLighting(Transform parent)
        {
            var lightObject = new GameObject("Directional Light");
            lightObject.transform.SetParent(parent, false);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.color = new Color(1f, 0.97f, 0.9f);
            lightObject.transform.localRotation = Quaternion.Euler(45f, 30f, 0f);
        }

        public static void CreateGround(Transform parent, Vector2Int size, Material material)
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Grass";
            ground.transform.SetParent(parent, false);
            ground.transform.localPosition = new Vector3((size.x - 1) * 0.5f, 0f, (size.y - 1) * 0.5f);
            ground.transform.localScale = new Vector3(size.x, TileThickness, size.y);

            material.SetTextureScale("_BaseMap", new Vector2(size.x, size.y));
            var renderer = ground.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
        }

        public static HashSet<Vector2Int> CreateObstacles(
            Transform parent,
            IReadOnlyList<Vector2Int> cells,
            IGridWorld world,
            Material material)
        {
            var blockedCells = new HashSet<Vector2Int>();
            foreach (var cell in cells)
            {
                if (!world.IsWithinGrid(cell))
                {
                    Debug.LogWarning($"Ignoring obstacle outside the grid at {cell}.", parent);
                    continue;
                }

                if (!blockedCells.Add(cell))
                {
                    Debug.LogWarning($"Ignoring duplicate obstacle at {cell}.", parent);
                    continue;
                }

                var rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rock.name = $"Rock_{cell.x}_{cell.y}";
                rock.transform.SetParent(parent, true);
                rock.transform.position = world.GridToWorld(cell) + Vector3.up * 0.4f;
                rock.transform.localScale = new Vector3(0.8f, 1f, 0.8f);
                rock.GetComponent<Renderer>().sharedMaterial = material;
            }

            return blockedCells;
        }
    }
}
