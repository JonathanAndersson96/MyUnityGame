using UnityEditor;
using UnityEngine;

namespace MyUnityGame.Editor
{
    public class CharacterWalkSpriteImporter : AssetPostprocessor
    {
        private const string IdleSpritePath = "Assets/Resources/Characters/MainCharacter.png";

        private static readonly string[] WalkSpriteFolders =
        {
            "Assets/Resources/Characters/Walk/",
            "Assets/Resources/Characters/WalkUp/",
            "Assets/Resources/Characters/WalkLeft/",
            "Assets/Resources/Characters/WalkRight/"
        };

        [InitializeOnLoadMethod]
        private static void ConfigureExistingCharacterSprites()
        {
            var spriteGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Resources/Characters" });
            foreach (var spriteGuid in spriteGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(spriteGuid);
                if (!IsCharacterSprite(path))
                {
                    continue;
                }

                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null || HasPixelSpriteSettings(importer))
                {
                    continue;
                }

                ApplyPixelSpriteSettings(importer);
                importer.SaveAndReimport();
            }
        }

        private void OnPreprocessTexture()
        {
            if (!IsCharacterSprite(assetPath))
            {
                return;
            }

            var importer = (TextureImporter)assetImporter;
            ApplyPixelSpriteSettings(importer);
        }

        private static bool IsCharacterSprite(string path)
        {
            if (string.Equals(path, IdleSpritePath, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            foreach (var folder in WalkSpriteFolders)
            {
                if (path.StartsWith(folder, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasPixelSpriteSettings(TextureImporter importer)
        {
            return importer.textureType == TextureImporterType.Sprite &&
                   importer.spriteImportMode == SpriteImportMode.Single &&
                   Mathf.Approximately(importer.spritePixelsPerUnit, 64f) &&
                   importer.spritePivot == new Vector2(0.5f, 0f) &&
                   importer.filterMode == FilterMode.Point &&
                   !importer.mipmapEnabled &&
                   importer.textureCompression == TextureImporterCompression.Uncompressed;
        }

        private static void ApplyPixelSpriteSettings(TextureImporter importer)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 64f;
            importer.spritePivot = new Vector2(0.5f, 0f);
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
        }
    }
}
