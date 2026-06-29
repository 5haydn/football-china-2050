using System.Collections.Generic;
using RetroFootball76.Core;
using RetroFootball76.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RetroFootball76.UI
{
    public class TeamSelectController : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown homeDropdown;
        [SerializeField] TMP_Dropdown awayDropdown;
        [SerializeField] TMP_Text homeStatsText;
        [SerializeField] TMP_Text awayStatsText;
        [SerializeField] Button startButton;
        [SerializeField] Button backButton;

        List<TeamData> _teams = new();

        void Start()
        {
            var db = HistoricalDatabase.Instance;
            if (db == null)
            {
                Debug.LogError("HistoricalDatabase missing.");
                return;
            }

            _teams = new List<TeamData>(db.Teams);
            var options = new List<TMP_Dropdown.OptionData>();
            foreach (var t in _teams)
                options.Add(new TMP_Dropdown.OptionData($"{t.name} ({t.year})"));

            SetupDropdown(homeDropdown, options, 0);
            SetupDropdown(awayDropdown, options, Mathf.Min(1, _teams.Count - 1));

            if (homeDropdown != null) homeDropdown.onValueChanged.AddListener(_ => RefreshStats());
            if (awayDropdown != null) awayDropdown.onValueChanged.AddListener(_ => RefreshStats());

            if (startButton != null)
                startButton.onClick.AddListener(StartMatch);
            if (backButton != null)
                backButton.onClick.AddListener(() => GameBootstrap.LoadScene(GameConstants.SceneMainMenu));

            RefreshStats();
        }

        static void SetupDropdown(TMP_Dropdown dropdown, List<TMP_Dropdown.OptionData> options, int defaultIndex)
        {
            if (dropdown == null) return;
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
            dropdown.value = defaultIndex;
        }

        void RefreshStats()
        {
            if (_teams.Count == 0) return;
            var home = _teams[homeDropdown != null ? homeDropdown.value : 0];
            var away = _teams[awayDropdown != null ? awayDropdown.value : 0];
            if (homeStatsText != null) homeStatsText.text = FormatTeam(home);
            if (awayStatsText != null) awayStatsText.text = FormatTeam(away);
        }

        static string FormatTeam(TeamData team)
        {
            var lines = new System.Text.StringBuilder();
            lines.AppendLine($"{team.name} · {team.formation}");
            var count = Mathf.Min(GameConstants.SquadSize, team.players.Count);
            for (var i = 0; i < count; i++)
            {
                var p = team.players[i];
                lines.AppendLine($"{p.position} {p.name} OVR {p.overall}");
            }
            return lines.ToString();
        }

        void StartMatch()
        {
            if (_teams.Count < 2) return;
            GameSession.HomeTeam = _teams[homeDropdown.value];
            GameSession.AwayTeam = _teams[awayDropdown.value];
            if (GameSession.HomeTeam.id == GameSession.AwayTeam.id)
            {
                Debug.LogWarning("Pick two different teams.");
                return;
            }
            GameBootstrap.LoadScene(GameConstants.SceneMatch);
        }
    }
}
