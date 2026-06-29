using RetroFootball76.Core;
using UnityEngine;

namespace RetroFootball76.Core
{
    public class BootLoader : MonoBehaviour
    {
        void Start()
        {
            GameBootstrap.LoadScene(GameConstants.SceneMainMenu);
        }
    }
}
