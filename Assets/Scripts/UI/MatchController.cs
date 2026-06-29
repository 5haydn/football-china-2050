using RetroFootball76.Core;
using RetroFootball76.Data;
using RetroFootball76.Match;
using UnityEngine;

namespace RetroFootball76.UI
{
    /// <summary>
    /// Attach to Match scene. Runs simulation and feeds HUD.
    /// </summary>
    public class MatchController : MonoBehaviour
    {
        MatchSimulator _simulator;
        int _eventIndex;

        public MatchState CurrentState => _simulator?.State;

        void Start()
        {
            if (GameSession.HomeTeam == null || GameSession.AwayTeam == null)
            {
                Debug.LogWarning("No teams selected — loading TeamSelect.");
                GameBootstrap.LoadScene(GameConstants.SceneTeamSelect);
                return;
            }

            GameSession.MatchSeed = Random.Range(1, int.MaxValue);
            _simulator = new MatchSimulator(GameSession.HomeTeam, GameSession.AwayTeam, GameSession.MatchSeed);
            _simulator.SimulateFullMatch();
            _eventIndex = 0;
            Debug.Log($"Match finished: {GameSession.HomeTeam.name} {_simulator.State.homeScore} - {_simulator.State.awayScore} {GameSession.AwayTeam.name}");
        }

        public bool TryGetNextEvent(out MatchEvent evt)
        {
            evt = null;
            if (_simulator == null || _eventIndex >= _simulator.State.events.Count) return false;
            evt = _simulator.State.events[_eventIndex++];
            return true;
        }
    }
}
