using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyUnityGame.Gameplay
{
    internal sealed class GridPrototypeAssets : IDisposable
    {
        private readonly List<UnityEngine.Object> ownedAssets = new List<UnityEngine.Object>();

        public Material GrassMaterial { get; }
        public Material RockMaterial { get; }

        public GridPrototypeAssets()
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                throw new InvalidOperationException("Unable to find the URP Lit shader for the prototype world.");
            }

            GrassMaterial = CreateMaterial(shader, CreateGrassTexture(), "PrototypeGrass");
            RockMaterial = CreateMaterial(shader, CreateRockTexture(), "PrototypeRock");
        }

        public Material CreateDemonMaterial()
        {
            var material = new Material(RockMaterial)
            {
                name = "PrototypeDemon"
            };
            material.SetColor("_BaseColor", new Color(0.82f, 0.18f, 0.22f));
            ownedAssets.Add(material);
            return material;
        }

        private Material CreateMaterial(Shader shader, Texture2D texture, string materialName)
        {
            ownedAssets.Add(texture);
            var material = new Material(shader)
            {
                name = materialName
            };
            material.SetTexture("_BaseMap", texture);
            material.SetColor("_BaseColor", Color.white);
            material.SetFloat("_Smoothness", 0f);
            material.SetFloat("_Metallic", 0f);
            ownedAssets.Add(material);
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

            return CreatePixelTexture(64, 64, (x, y) =>
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

        private static Texture2D CreatePixelTexture(int width, int height, Func<int, int, Color32> getPixel)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = "PrototypePixelTexture",
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

        public void Dispose()
        {
            foreach (var asset in ownedAssets)
            {
                if (asset != null)
                {
                    UnityEngine.Object.Destroy(asset);
                }
            }

            ownedAssets.Clear();
        }
    }
}
