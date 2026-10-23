using MyUnityGame.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUnityGame.Gameplay
{
    public class BattleSceneBootstrap : MonoBehaviour
    {
        private void Start()
        {
            if (PartyManager.Instance == null)
            {
                var partyManagerObject = new GameObject("PartyManager");
                partyManagerObject.AddComponent<PartyManager>();
            }

            if (BattleManager.Instance == null)
            {
                var battleObject = new GameObject("BattleManager");
                battleObject.AddComponent<BattleManager>();
            }

            var battle = BattleManager.Instance;
            if (battle == null)
            {
                Debug.LogError("Unable to initialize the battle manager.", this);
                enabled = false;
                return;
            }

            if (Camera.main == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                var camera = cameraObject.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.09f, 0.10f, 0.15f);
                camera.orthographic = true;
                camera.orthographicSize = 5f;
                camera.transform.position = new Vector3(0f, 0f, -10f);
            }

            var encounterName = GameManager.Instance != null
                ? GameManager.Instance.CurrentEncounter
                : null;
            if (!battle.StartBattle(string.IsNullOrWhiteSpace(encounterName) ? "Demon" : encounterName))
            {
                Debug.LogError("The battle could not be started. Check the active party and encounter settings.", this);
                enabled = false;
            }
        }

        public void ReturnToWorld()
        {
            const string worldScenePath = "Assets/Scenes/SampleScene.unity";
            var worldSceneIndex = SceneUtility.GetBuildIndexByScenePath(worldScenePath);
            if (worldSceneIndex < 0)
            {
                Debug.LogError($"World scene is not available in Build Settings: {worldScenePath}", this);
                return;
            }

            SceneManager.LoadScene(worldSceneIndex);
        }
    }
}
