using Newtonsoft.Json;

var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
var teamsPath = Path.Combine(root, "Assets", "StreamingAssets", "Historical", "teams.json");

if (!File.Exists(teamsPath))
{
    Console.WriteLine($"Missing {teamsPath}");
    return 1;
}

var file = JsonConvert.DeserializeObject<TeamsFile>(File.ReadAllText(teamsPath))!;
var home = file.teams.First(t => t.id == "ned1974");
var away = file.teams.First(t => t.id == "frg1974");

Console.WriteLine("Retro Football '76 — Console Demo");
Console.WriteLine($"Match: {home.name} ({home.year}) vs {away.name} ({away.year})");
Console.WriteLine(new string('-', 50));

var sim = new MatchSimulator(home, away, seed: 76);
sim.SimulateFullMatch();

foreach (var evt in sim.State.events)
    Console.WriteLine($"[{evt.minute,2}'] {evt.description}");

Console.WriteLine(new string('-', 50));
Console.WriteLine($"Final: {home.name} {sim.State.homeScore} - {sim.State.awayScore} {away.name}");
Console.WriteLine("\nOpen in Unity: install Unity Hub 2022.3 LTS → open project → Retro Football → Generate MVP Scenes");
return 0;

// ---- Minimal port of Assets/Scripts/Match + Data ----

class TeamsFile { public List<TeamData> teams = new(); }

class TeamData
{
    public string id = "";
    public string name = "";
    public int year;
    public string formation = "";
    public List<PlayerData> players = new();
}

class PlayerData
{
    public string name = "";
    public string position = "";
    public int shoot, pass, defend, pace;
}

enum MatchEventType { Kickoff, Shot, Goal, Save, Turnover, HalfTime, FullTime }

class MatchEvent
{
    public int minute;
    public MatchEventType type;
    public string? teamId;
    public string? playerName;
    public string description = "";
}

class MatchState
{
    public TeamData homeTeam = null!;
    public TeamData awayTeam = null!;
    public int minute, homeScore, awayScore;
    public List<MatchEvent> events = new();
}

class MatchSimulator
{
    const int PhasesPerHalf = 12;
    const int HalfMinutes = 3;
    const int SquadSize = 5;

    readonly Random _rng;
    public MatchState State { get; }

    public MatchSimulator(TeamData home, TeamData away, int seed)
    {
        _rng = new Random(seed);
        State = new MatchState { homeTeam = home, awayTeam = away };
        Add(0, MatchEventType.Kickoff, home.id, null, $"{home.name} vs {away.name} — kickoff!");
    }

    public void SimulateFullMatch()
    {
        var total = PhasesPerHalf * 2;
        for (var phase = 0; phase < total; phase++)
        {
            State.minute = (int)Math.Min(90, (phase + 1) * 90f / total);
            if (phase == PhasesPerHalf)
                Add(State.minute, MatchEventType.HalfTime, null, null, "Half time.");

            var homeAttacks = _rng.NextDouble() > 0.5;
            Phase(homeAttacks ? State.homeTeam : State.awayTeam,
                  homeAttacks ? State.awayTeam : State.homeTeam);
        }
        Add(90, MatchEventType.FullTime, null, null,
            $"Full time: {State.homeTeam.name} {State.homeScore} - {State.awayScore} {State.awayTeam.name}");
    }

    void Phase(TeamData attack, TeamData defend)
    {
        var atk = Power(attack, 0.45f, 0.35f, 0.2f);
        var def = Power(defend, 0.15f, 0.25f, 0.6f);
        var roll = _rng.NextDouble() * (atk + def);
        var shooter = Pick(attack, true);

        if (roll < atk * 0.12) Score(attack, shooter);
        else if (roll < atk * 0.35)
        {
            Add(State.minute, MatchEventType.Shot, attack.id, shooter?.name, $"{shooter?.name} shoots — saved!");
            Add(State.minute, MatchEventType.Save, defend.id, Pick(defend, false)?.name, "Great save!");
        }
        else if (roll < atk * 0.55)
            Add(State.minute, MatchEventType.Turnover, defend.id, null, $"{defend.name} win the ball.");
        else
            Add(State.minute, MatchEventType.Shot, attack.id, shooter?.name, $"{shooter?.name} — shot wide.");
    }

    void Score(TeamData team, PlayerData? scorer)
    {
        if (team.id == State.homeTeam.id) State.homeScore++;
        else State.awayScore++;
        Add(State.minute, MatchEventType.Goal, team.id, scorer?.name,
            $"GOAL! {scorer?.name} ({team.name}) {State.homeScore}-{State.awayScore}");
    }

    static float Power(TeamData team, float sw, float pw, float dw)
    {
        var squad = team.players.Take(SquadSize).ToList();
        if (squad.Count == 0) return 50f;
        return squad.Average(p => p.shoot * sw + p.pass * pw + p.defend * dw + p.pace * 0.1f);
    }

    static PlayerData? Pick(TeamData team, bool shoot)
    {
        var squad = team.players.Take(SquadSize).OrderByDescending(p => shoot ? p.shoot : p.pass).ToList();
        return squad.FirstOrDefault();
    }

    void Add(int minute, MatchEventType type, string? teamId, string? player, string desc)
    {
        State.events.Add(new MatchEvent { minute = minute, type = type, teamId = teamId, playerName = player, description = desc });
    }
}
