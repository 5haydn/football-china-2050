using UnityEngine;
using UnityEngine.SceneManagement;

namespace RetroFootball76.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void EnsureBootstrap()
        {
            if (FindObjectOfType<GameBootstrap>() != null) return;
            var go = new GameObject("GameBootstrap");
            go.AddComponent<GameBootstrap>();
        }

        void Awake()
        {
            if (FindObjectsOfType<GameBootstrap>().Length > 1)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            if (GetComponent<Data.HistoricalDatabase>() == null)
                gameObject.AddComponent<Data.HistoricalDatabase>();
        }

        public static void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
