using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Managers
{
    public class Bootstrapper : MonoBehaviour
    {
        [Header("Scenes")]
        public string GameSceneName = "GameScene"; // Change this to your actual game scene's name in the Inspector

        private void Start()
        {
            SceneManager.LoadScene(GameSceneName);
        }
    }
}