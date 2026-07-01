using RetroFootball76.Core;
using RetroFootball76.Match;
using RetroFootball76.Platform;
using UnityEngine;
using TMPro;

namespace RetroFootball76.UI
{
    public class MatchHudController : MonoBehaviour
    {
        [SerializeField] MatchController matchController;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text clockText;
        [SerializeField] TMP_Text logText;
        [SerializeField] TMP_Text bannerText;
        [SerializeField] UnityEngine.UI.Image bannerPanel;
        [SerializeField] UnityEngine.UI.Button menuButton;

        float _bannerTimer;

        void Start()
        {
            if (menuButton != null)
                menuButton.onClick.AddListener(() => GameBootstrap.LoadScene(GameConstants.SceneMainMenu));

            if (bannerPanel != null)
                bannerPanel.gameObject.SetActive(false);

            RefreshScore();
            UpdateClock(0, false);
        }

        public void Wire(TMP_Text score, TMP_Text clock, TMP_Text log, UnityEngine.UI.Button menu,
            TMP_Text banner = null, UnityEngine.UI.Image bannerBg = null)
        {
            scoreText = score;
            clockText = clock;
            logText = log;
            menuButton = menu;
            bannerText = banner;
            bannerPanel = bannerBg;

            if (menuButton != null)
                menuButton.onClick.AddListener(() => GameBootstrap.LoadScene(GameConstants.SceneMainMenu));

            if (bannerPanel != null)
                bannerPanel.gameObject.SetActive(false);
        }

        public void RefreshScore()
        {
            var mc = matchController != null ? matchController : FindObjectOfType<MatchController>();
            var state = mc?.CurrentState;
            if (state == null || scoreText == null) return;
            scoreText.text = $"{state.homeTeam.name}  {state.homeScore}  -  {state.awayScore}  {state.awayTeam.name}";
        }

        public void UpdateClock(int minute, bool isHalfTime)
        {
            if (clockText == null) return;
            clockText.text = isHalfTime ? "45' HT" : $"{minute}'";
        }

        public void ShowBanner(string message, float duration = 2.2f)
        {
            if (bannerText == null || bannerPanel == null) return;
            bannerText.text = message;
            bannerPanel.gameObject.SetActive(true);
            _bannerTimer = duration;
        }

        public void AppendLog(string line)
        {
            if (logText == null) return;
            logText.text = string.IsNullOrEmpty(logText.text) ? line : logText.text + "\n" + line;
        }

        void Update()
        {
            if (_bannerTimer > 0f)
            {
                _bannerTimer -= Time.unscaledDeltaTime;
                if (_bannerTimer <= 0f && bannerPanel != null)
                    bannerPanel.gameObject.SetActive(false);
            }

            var input = InputRouter.Instance;
            if (input == null) return;
            if (input.BackPressed)
                GameBootstrap.LoadScene(GameConstants.SceneMainMenu);
        }
    }
}
