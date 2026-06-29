using System;
using System.Collections.Generic;
using RetroFootball76.Data;

namespace RetroFootball76.Match
{
    public enum MatchEventType
    {
        Kickoff,
        Shot,
        Goal,
        Save,
        Turnover,
        HalfTime,
        FullTime
    }

    [Serializable]
    public class MatchEvent
    {
        public int minute;
        public MatchEventType type;
        public string teamId;
        public string playerName;
        public string description;
    }

    [Serializable]
    public class MatchState
    {
        public TeamData homeTeam;
        public TeamData awayTeam;
        public int minute;
        public int homeScore;
        public int awayScore;
        public bool isFinished;
        public List<MatchEvent> events = new();
    }
}
