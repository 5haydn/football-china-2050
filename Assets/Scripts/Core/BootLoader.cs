using RetroFootball76.Core;
using RetroFootball76.Platform;
using UnityEngine;

namespace RetroFootball76.Core
{
    public class BootLoader : MonoBehaviour
    {
        void Awake()
        {
            if (FindObjectOfType<InputRouter>() == null)
                new GameObject("InputRouter").AddComponent<InputRouter>();
        }

        void Start()
        {
            GameBootstrap.LoadScene(GameConstants.SceneMainMenu);
        }
    }
}
