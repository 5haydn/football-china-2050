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

        float _timer;
        const float EventInterval = 0.4f;

        void Start()
        {
            if (menuButton != null)
                menuButton.onClick.AddListener(() => GameBootstrap.LoadScene(GameConstants.SceneMainMenu));

            RefreshScore();
        }

        void Update()
        {
            if (matchController == null) return;
            _timer += Time.deltaTime;
            if (_timer < EventInterval) return;
            _timer = 0f;

            if (matchController.TryGetNextEvent(out var evt))
            {
                AppendLog($"[{evt.minute}'] {evt.description}");
                RefreshScore();
            }
        }

        void RefreshScore()
        {
            var state = matchController?.CurrentState;
            if (state == null || scoreText == null) return;
            scoreText.text = $"{state.homeTeam.name}  {state.homeScore}  -  {state.awayScore}  {state.awayTeam.name}";
        }

        void AppendLog(string line)
        {
            if (logText == null) return;
            logText.text = string.IsNullOrEmpty(logText.text) ? line : logText.text + "\n" + line;
        }
    }
}
