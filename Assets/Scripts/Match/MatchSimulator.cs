using System;
using System.Collections.Generic;
using System.Linq;
using RetroFootball76.Data;

namespace RetroFootball76.Match
{
    /// <summary>
    /// Stat-driven 5-a-side arcade simulator using 1974/76 squad ratings.
    /// </summary>
    public class MatchSimulator
    {
        readonly Random _rng;
        readonly MatchState _state;

        public MatchState State => _state;

        public MatchSimulator(TeamData home, TeamData away, int seed = 0)
        {
            _rng = seed == 0 ? new Random() : new Random(seed);
            _state = new MatchState
            {
                homeTeam = home,
                awayTeam = away,
                minute = 0,
                homeScore = 0,
                awayScore = 0
            };
            AddEvent(0, MatchEventType.Kickoff, home.id, null,
                $"{home.name} vs {away.name} — kickoff!");
        }

        public void SimulateFullMatch()
        {
            var totalPhases = Core.GameConstants.PhasesPerHalf * 2;
            var minutesPerPhase = (Core.GameConstants.HalfMinutes * 2 * 60f) / totalPhases;

            for (var phase = 0; phase < totalPhases; phase++)
            {
                _state.minute = (int)Math.Min(90, (phase + 1) * minutesPerPhase / 60f * 90f / (Core.GameConstants.HalfMinutes * 2));
                if (phase == Core.GameConstants.PhasesPerHalf)
                    AddEvent(_state.minute, MatchEventType.HalfTime, null, null, "Half time.");

                var homeAttacks = _rng.NextDouble() > 0.5;
                SimulatePhase(homeAttacks ? _state.homeTeam : _state.awayTeam,
                              homeAttacks ? _state.awayTeam : _state.homeTeam);
            }

            _state.isFinished = true;
            AddEvent(90, MatchEventType.FullTime, null, null,
                $"Full time: {_state.homeTeam.name} {_state.homeScore} - {_state.awayScore} {_state.awayTeam.name}");
        }

        void SimulatePhase(TeamData attack, TeamData defend)
        {
            var atk = SquadPower(attack, shootWeight: 0.45f, passWeight: 0.35f, defendWeight: 0.2f);
            var def = SquadPower(defend, shootWeight: 0.15f, passWeight: 0.25f, defendWeight: 0.6f);
            var roll = _rng.NextDouble() * (atk + def);
            var shooter = PickPlayer(attack, preferShoot: true);

            if (roll < atk * 0.12)
            {
                Score(attack, shooter);
            }
            else if (roll < atk * 0.35)
            {
                AddEvent(_state.minute, MatchEventType.Shot, attack.id, shooter?.name,
                    $"{shooter?.name} shoots — saved!");
                AddEvent(_state.minute, MatchEventType.Save, defend.id, PickPlayer(defend, preferShoot: false)?.name,
                    "Great save!");
            }
            else if (roll < atk * 0.55)
            {
                AddEvent(_state.minute, MatchEventType.Turnover, defend.id, null, $"{defend.name} win the ball.");
            }
            else
            {
                AddEvent(_state.minute, MatchEventType.Shot, attack.id, shooter?.name,
                    $"{shooter?.name} — shot wide.");
            }
        }

        void Score(TeamData team, PlayerData scorer)
        {
            if (team.id == _state.homeTeam.id) _state.homeScore++;
            else _state.awayScore++;

            AddEvent(_state.minute, MatchEventType.Goal, team.id, scorer?.name,
                $"GOAL! {scorer?.name} ({team.name}) {_state.homeScore}-{_state.awayScore}");
        }

        static float SquadPower(TeamData team, float shootWeight, float passWeight, float defendWeight)
        {
            var squad = team.players.Take(Core.GameConstants.SquadSize).ToList();
            if (squad.Count == 0) return 50f;
            float sum = 0;
            foreach (var p in squad)
                sum += p.shoot * shootWeight + p.pass * passWeight + p.defend * defendWeight + p.pace * 0.1f;
            return sum / squad.Count;
        }

        static PlayerData PickPlayer(TeamData team, bool preferShoot)
        {
            var squad = team.players.Take(Core.GameConstants.SquadSize).ToList();
            if (squad.Count == 0) return null;
            squad.Sort((a, b) => (preferShoot ? b.shoot - a.shoot : b.pass - a.pass));
            return squad[0];
        }

        void AddEvent(int minute, MatchEventType type, string teamId, string player, string desc)
        {
            _state.events.Add(new MatchEvent
            {
                minute = minute,
                type = type,
                teamId = teamId,
                playerName = player,
                description = desc
            });
        }
    }
}
