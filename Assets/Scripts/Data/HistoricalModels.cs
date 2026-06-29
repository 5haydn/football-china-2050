using System;
using System.Collections.Generic;

namespace RetroFootball76.Data
{
    [Serializable]
    public class TournamentData
    {
        public string id;
        public string name;
        public int year;
        public string hostNation;
        public string winner;
        public string runnerUp;
    }

    [Serializable]
    public class PlayerData
    {
        public string id;
        public string name;
        public string position;
        public int overall;
        public int pace;
        public int shoot;
        public int pass;
        public int defend;
        public int stamina;
    }

    [Serializable]
    public class TeamData
    {
        public string id;
        public string name;
        public string nationCode;
        public int year;
        public string tournamentId;
        public string formation;
        public List<PlayerData> players = new();
    }

    [Serializable]
    public class TeamsFile
    {
        public List<TeamData> teams = new();
    }

    [Serializable]
    public class TournamentsFile
    {
        public List<TournamentData> tournaments = new();
    }
}
