using System.Collections.Generic;
using RetroFootball76.Data;
using UnityEngine;

namespace RetroFootball76.Visual
{
    public static class TeamColors
    {
        static readonly Dictionary<string, Color> Kits = new()
        {
            { "NED", new Color(1f, 0.78f, 0.05f) },
            { "FRG", new Color(0.92f, 0.92f, 0.92f) },
            { "POL", new Color(0.92f, 0.12f, 0.18f) },
            { "BRA", new Color(0.98f, 0.88f, 0.08f) },
            { "ITA", new Color(0.05f, 0.42f, 0.22f) },
            { "ENG", new Color(0.92f, 0.12f, 0.18f) },
            { "URS", new Color(0.78f, 0.08f, 0.12f) },
            { "YUG", new Color(0.12f, 0.18f, 0.72f) },
            { "CZE", new Color(0.92f, 0.12f, 0.18f) },
            { "ESP", new Color(0.92f, 0.12f, 0.18f) },
        };

        public static Color ForTeam(TeamData team)
        {
            if (team != null && Kits.TryGetValue(team.nationCode, out var c))
                return c;
            return new Color(0.5f, 0.55f, 0.6f);
        }

        public static readonly Color Goalkeeper = new Color(0.12f, 0.72f, 0.28f);
        public static readonly Color Referee = new Color(0.08f, 0.08f, 0.1f);
    }
}
