using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace RetroFootball76.Data
{
    /// <summary>
    /// Loads offline historical JSON from StreamingAssets/Historical/.
    /// </summary>
    public class HistoricalDatabase : MonoBehaviour
    {
        public static HistoricalDatabase Instance { get; private set; }

        public IReadOnlyList<TournamentData> Tournaments => _tournaments;
        public IReadOnlyList<TeamData> Teams => _teams;

        readonly List<TournamentData> _tournaments = new();
        readonly List<TeamData> _teams = new();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAll();
        }

        public void LoadAll()
        {
            _tournaments.Clear();
            _teams.Clear();

            var tournamentsJson = ReadStreamingText("Historical/tournaments.json");
            var teamsJson = ReadStreamingText("Historical/teams.json");

            var tFile = JsonConvert.DeserializeObject<TournamentsFile>(tournamentsJson);
            var teamFile = JsonConvert.DeserializeObject<TeamsFile>(teamsJson);

            if (tFile?.tournaments != null) _tournaments.AddRange(tFile.tournaments);
            if (teamFile?.teams != null) _teams.AddRange(teamFile.teams);

            Debug.Log($"[HistoricalDatabase] Loaded {_tournaments.Count} tournaments, {_teams.Count} teams.");
        }

        public TeamData GetTeam(string teamId)
        {
            return _teams.Find(t => t.id == teamId);
        }

        public List<TeamData> GetTeamsByTournament(string tournamentId)
        {
            return _teams.FindAll(t => t.tournamentId == tournamentId);
        }

        static string ReadStreamingText(string relativePath)
        {
            var path = Path.Combine(Application.streamingAssetsPath, relativePath);

#if UNITY_IOS && !UNITY_EDITOR
            // StreamingAssets on iOS uses file:// URL — read via UnityWebRequest in production;
            // for MVP editor + macOS standalone, direct read works on most targets.
#endif
            if (!File.Exists(path))
            {
                Debug.LogError($"Missing StreamingAssets file: {path}");
                return "{}";
            }
            return File.ReadAllText(path);
        }
    }
}
