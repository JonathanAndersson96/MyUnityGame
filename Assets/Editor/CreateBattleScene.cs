using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MyUnityGame.Editor
{
    public static class CreateBattleScene
    {
        public static void CreateBattleSceneAsset()
        {
            var sceneDir = Path.Combine("Assets", "Scenes");
            Directory.CreateDirectory(sceneDir);

            var scenePath = Path.Combine(sceneDir, "BattleScene.unity");
            var relativeScenePath = "Assets/Scenes/BattleScene.unity";
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var cameraObject = new GameObject("Main Camera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.09f, 0.10f, 0.15f);
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.transform.position = new Vector3(0f, 0f, -10f);

            var bootstrapObject = new GameObject("BattleSceneBootstrap");
            bootstrapObject.AddComponent<MyUnityGame.Gameplay.BattleSceneBootstrap>();

            EditorSceneManager.SaveScene(scene, scenePath);

            var buildScenes = EditorBuildSettings.scenes;
            if (System.Array.Find(buildScenes, s => s.path == relativeScenePath) == null)
            {
                var updatedScenes = new EditorBuildSettingsScene[buildScenes.Length + 1];
                for (int i = 0; i < buildScenes.Length; i++)
                {
                    updatedScenes[i] = buildScenes[i];
                }

                updatedScenes[updatedScenes.Length - 1] = new EditorBuildSettingsScene(relativeScenePath, true);
                EditorBuildSettings.scenes = updatedScenes;
            }

            Debug.Log($"Created battle scene: {scenePath}");
        }
    }
}
