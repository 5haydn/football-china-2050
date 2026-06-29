using RetroFootball76.Core;
using RetroFootball76.Match;
using UnityEngine;
using TMPro;

namespace RetroFootball76.UI
{
    public class MatchHudController : MonoBehaviour
    {
        [SerializeField] MatchController matchController;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text logText;
        [SerializeField] UnityEngine.UI.Button menuButton;

        void Start()
        {
            if (menuButton != null)
                menuButton.onClick.AddListener(() => GameBootstrap.LoadScene(GameConstants.SceneMainMenu));

            RefreshScore();
        }

        public void Wire(TMP_Text score, TMP_Text log, UnityEngine.UI.Button menu)
        {
            scoreText = score;
            logText = log;
            menuButton = menu;
            if (menuButton != null)
                menuButton.onClick.AddListener(() => GameBootstrap.LoadScene(GameConstants.SceneMainMenu));
        }

        public void RefreshScore()
        {
            var mc = matchController != null ? matchController : FindObjectOfType<MatchController>();
            var state = mc?.CurrentState;
            if (state == null || scoreText == null) return;
            scoreText.text = $"{state.homeTeam.name}  {state.homeScore}  -  {state.awayScore}  {state.awayTeam.name}";
        }

        public void AppendLog(string line)
        {
            if (logText == null) return;
            logText.text = string.IsNullOrEmpty(logText.text) ? line : logText.text + "\n" + line;
        }
    }
}
