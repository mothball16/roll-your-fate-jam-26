using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Managers
{
    public class TitleManager : MonoBehaviour
    {
        [Header("Scenes")]
        public string GameSceneName = "GameScene"; 

        public void StartGame()
        {
            SceneManager.LoadScene(GameSceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}