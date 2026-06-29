using RetroFootball76.Data;

namespace RetroFootball76.Core
{
    public static class GameConstants
    {
        public const string SceneBoot = "Boot";
        public const string SceneMainMenu = "MainMenu";
        public const string SceneTeamSelect = "TeamSelect";
        public const string SceneMatch = "Match";

        public const int HalfMinutes = 3;
        public const int PhasesPerHalf = 12;
        public const int SquadSize = 5;
    }

    public static class GameSession
    {
        public static TeamData HomeTeam;
        public static TeamData AwayTeam;
        public static int MatchSeed;

        public static void Reset()
        {
            HomeTeam = null;
            AwayTeam = null;
            MatchSeed = 0;
        }
    }
}
