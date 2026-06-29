using RetroFootball76.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RetroFootball76.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] Button playButton;
        [SerializeField] Button quitButton;
        [SerializeField] TMP_Text subtitleText;

        void Start()
        {
            if (subtitleText != null)
                subtitleText.text = "1974 World Cup · 1976 European Championship";

            if (playButton != null)
                playButton.onClick.AddListener(() => GameBootstrap.LoadScene(GameConstants.SceneTeamSelect));

            if (quitButton != null)
                quitButton.onClick.AddListener(() =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                });
        }
    }
}
