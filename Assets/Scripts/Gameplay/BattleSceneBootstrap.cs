using MyUnityGame.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUnityGame.Gameplay
{
    public class BattleSceneBootstrap : MonoBehaviour
    {
        private void Start()
        {
            if (BattleManager.Instance == null)
            {
                var battleObject = new GameObject("BattleManager");
                battleObject.AddComponent<BattleManager>();
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

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartBattle("Demon");
            }

            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.StartBattle();
            }
        }

        public void ReturnToWorld()
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
