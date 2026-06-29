using RetroFootball76.Core;
using RetroFootball76.Platform;
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

        public void Wire(Button play, Button quit, TMP_Text subtitle)
        {
            playButton = play;
            quitButton = quit;
            subtitleText = subtitle;
            Bind();
        }

        void Start() => Bind();

        void Update()
        {
            var input = InputRouter.Instance;
            if (input == null) return;
            if (input.ConfirmPressed && playButton != null)
                playButton.onClick.Invoke();
            if (input.BackPressed && quitButton != null)
                quitButton.onClick.Invoke();
        }

        void Bind()
        {
            if (subtitleText != null)
                subtitleText.text = "1974 World Cup · 1976 European Championship";

            if (playButton != null)
            {
                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(() => GameBootstrap.LoadScene(GameConstants.SceneTeamSelect));
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(Quit);
            }
        }

        static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
