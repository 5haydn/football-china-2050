using RetroFootball76.Core;
using UnityEngine;

namespace RetroFootball76.Visual
{
    /// <summary>
    /// Full 3D football pitch with detailed procedural stadium, players, and ball.
    /// </summary>
    public class Pitch3DView : MonoBehaviour
    {
        public float fieldWidth = 20f;
        public float fieldLength = 32f;
        public Color grassDark = new Color(0.14f, 0.44f, 0.18f);
        public Color grassLight = new Color(0.22f, 0.58f, 0.26f);

        Transform _root;
        Transform _ball;
        Transform[] _homePlayers;
        Transform[] _awayPlayers;
        Transform _homeGk;
        Transform _awayGk;
        Transform _referee;
        Vector3[] _homeBase;
        Vector3[] _awayBase;
        Color _homeColor;
        Color _awayColor;
        bool _built;

        public Transform BallTransform => _ball;

        public void Build(string homeName, string awayName)
        {
            if (_built) return;
            _built = true;

            _homeColor = GameSession.HomeTeam != null
                ? TeamColors.ForTeam(GameSession.HomeTeam)
                : new Color(0.95f, 0.25f, 0.2f);
            _awayColor = GameSession.AwayTeam != null
                ? TeamColors.ForTeam(GameSession.AwayTeam)
                : new Color(0.2f, 0.55f, 0.95f);

            _root = new GameObject("Pitch3D").transform;
            _root.SetParent(transform, false);

            ProceduralAssets.CreateSky(_root);
            ProceduralAssets.CreateStadium(_root, fieldWidth, fieldLength);
            BuildField();
            BuildMarkings();
            ProceduralAssets.CreateDetailedGoal(_root, new Vector3(0, 0, -fieldLength * 0.48f), -1f);
            ProceduralAssets.CreateDetailedGoal(_root, new Vector3(0, 0, fieldLength * 0.48f), 1f);
            BuildCornerFlags();
            BuildPlayers();
            BuildOfficials();
            BuildBall();
            ResetFormation();
        }

        void BuildField()
        {
            var pitch = GameObject.CreatePrimitive(PrimitiveType.Plane);
            pitch.name = "Grass";
            pitch.transform.SetParent(_root, false);
            pitch.transform.localScale = new Vector3(fieldWidth / 10f, 1f, fieldLength / 10f);
            pitch.GetComponent<Renderer>().material = ProceduralAssets.CreateGrassMaterial(grassLight, grassDark);
            Destroy(pitch.GetComponent<Collider>());
        }

        void BuildMarkings()
        {
            var line = new Color(1f, 1f, 1f, 0.92f);
            var w = 0.08f;

            Line("TouchlineW", new Vector3(-fieldWidth / 2f, 0.03f, 0), new Vector3(w, 0.02f, fieldLength), line);
            Line("TouchlineE", new Vector3(fieldWidth / 2f, 0.03f, 0), new Vector3(w, 0.02f, fieldLength), line);
            Line("GoalLineN", new Vector3(0, 0.03f, -fieldLength / 2f), new Vector3(fieldWidth, 0.02f, w), line);
            Line("GoalLineS", new Vector3(0, 0.03f, fieldLength / 2f), new Vector3(fieldWidth, 0.02f, w), line);
            Line("CenterLine", Vector3.zero, new Vector3(fieldWidth, 0.02f, w), line);

            CircleMark("CenterCircle", Vector3.zero, 3.8f);
            Spot("CenterSpot", Vector3.zero);

            var boxW = fieldWidth * 0.55f;
            var boxD = fieldLength * 0.2f;
            BoxOutline("HomeBox", new Vector3(0, 0.03f, -fieldLength * 0.38f), boxW, boxD, line);
            BoxOutline("AwayBox", new Vector3(0, 0.03f, fieldLength * 0.38f), boxW, boxD, line);
            Spot("HomePenalty", new Vector3(0, 0.03f, -fieldLength * 0.28f));
            Spot("AwayPenalty", new Vector3(0, 0.03f, fieldLength * 0.28f));
        }

        void BuildCornerFlags()
        {
            var hw = fieldWidth / 2f - 0.3f;
            var hl = fieldLength / 2f - 0.3f;
            ProceduralAssets.CreateCornerFlag(_root, new Vector3(-hw, 0, -hl));
            ProceduralAssets.CreateCornerFlag(_root, new Vector3(hw, 0, -hl));
            ProceduralAssets.CreateCornerFlag(_root, new Vector3(-hw, 0, hl));
            ProceduralAssets.CreateCornerFlag(_root, new Vector3(hw, 0, hl));
        }

        void BuildPlayers()
        {
            _homePlayers = new Transform[5];
            _awayPlayers = new Transform[5];
            _homeBase = new Vector3[5];
            _awayBase = new Vector3[5];

            for (var i = 0; i < 5; i++)
            {
                _homePlayers[i] = ProceduralAssets.CreatePlayerFigure(_root, $"Home_{i}", _homeColor, i + 1);
                _awayPlayers[i] = ProceduralAssets.CreatePlayerFigure(_root, $"Away_{i}", _awayColor, i + 1);
            }
        }

        void BuildOfficials()
        {
            _homeGk = ProceduralAssets.CreatePlayerFigure(_root, "Home_GK", TeamColors.Goalkeeper, 1);
            _awayGk = ProceduralAssets.CreatePlayerFigure(_root, "Away_GK", TeamColors.Goalkeeper, 1);
            _referee = ProceduralAssets.CreatePlayerFigure(_root, "Referee", TeamColors.Referee, 0);
        }

        void BuildBall()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Ball";
            go.transform.SetParent(_root, false);
            go.transform.localScale = Vector3.one * 0.42f;
            go.GetComponent<Renderer>().material = ProceduralAssets.CreateBallMaterial();
            Destroy(go.GetComponent<Collider>());
            _ball = go.transform;
        }

        public void ResetFormation()
        {
            var homeZ = new[] { -0.35f, -0.2f, -0.28f, -0.15f, -0.42f };
            var homeX = new[] { 0f, -0.28f, 0.28f, -0.12f, 0.12f };
            var awayZ = new[] { 0.35f, 0.2f, 0.28f, 0.15f, 0.42f };
            var awayX = new[] { 0f, 0.28f, -0.28f, 0.12f, -0.12f };

            for (var i = 0; i < 5; i++)
            {
                _homeBase[i] = PitchToWorld(homeX[i], homeZ[i]);
                _awayBase[i] = PitchToWorld(awayX[i], awayZ[i]);
                _homePlayers[i].localPosition = _homeBase[i];
                _awayPlayers[i].localPosition = _awayBase[i];
            }

            _homeGk.localPosition = PitchToWorld(0, -0.44f);
            _awayGk.localPosition = PitchToWorld(0, 0.44f);
            _referee.localPosition = PitchToWorld(0.08f, 0.02f);

            _ball.localPosition = PitchToWorld(0, 0) + Vector3.up * 0.35f;
        }

        public void AnimateEvent(float pitchX, float pitchY, MatchEventKind kind,
            string teamId = null, string playerName = null)
        {
            var target = PitchToWorld(pitchX, pitchY) + Vector3.up * 0.35f;
            var duration = kind == MatchEventKind.Goal ? 0.55f : 0.35f;
            var attackingHome = teamId != null && GameSession.HomeTeam?.id == teamId;
            var attackingAway = teamId != null && GameSession.AwayTeam?.id == teamId;
            StartCoroutine(AnimateBall(target, kind, duration, attackingHome, attackingAway));
        }

        System.Collections.IEnumerator AnimateBall(Vector3 target, MatchEventKind kind, float duration,
            bool attackingHome, bool attackingAway)
        {
            var start = _ball.localPosition;
            var elapsed = 0f;
            var peak = kind == MatchEventKind.Goal ? 3.5f : kind == MatchEventKind.Shot ? 2.2f : 1f;
            var baseScale = _ball.localScale;

            var mover = PickMover(attackingHome, attackingAway, target);
            var moverStart = mover != null ? mover.localPosition : Vector3.zero;
            var moverTarget = target;
            moverTarget.y = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var pos = Vector3.Lerp(start, target, t);
                pos.y += Mathf.Sin(t * Mathf.PI) * peak;
                _ball.localPosition = pos;
                _ball.Rotate(Vector3.right, 480f * Time.deltaTime, Space.World);

                if (mover != null)
                    mover.localPosition = Vector3.Lerp(moverStart, moverTarget, t);

                yield return null;
            }

            _ball.localPosition = target;

            if (kind == MatchEventKind.Goal)
            {
                ProceduralAssets.SpawnConfetti(_root, target + Vector3.up);
                yield return Celebrate(attackingHome, attackingAway);
                for (var i = 0; i < 3; i++)
                {
                    _ball.localScale = baseScale * 1.4f;
                    yield return new WaitForSecondsRealtime(0.1f);
                    _ball.localScale = baseScale;
                    yield return new WaitForSecondsRealtime(0.1f);
                }
            }

            ReturnPlayersToBase();
        }

        Transform PickMover(bool attackingHome, bool attackingAway, Vector3 target)
        {
            Transform[] squad = null;
            if (attackingHome) squad = _homePlayers;
            else if (attackingAway) squad = _awayPlayers;
            if (squad == null) return null;

            Transform best = null;
            var bestDist = float.MaxValue;
            foreach (var p in squad)
            {
                var d = Vector3.SqrMagnitude(p.localPosition - target);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = p;
                }
            }
            return best;
        }

        System.Collections.IEnumerator Celebrate(bool homeScored, bool awayScored)
        {
            var squad = homeScored ? _homePlayers : awayScored ? _awayPlayers : null;
            if (squad == null) yield break;

            var duration = 0.45f;
            var elapsed = 0f;
            var starts = new Vector3[squad.Length];
            for (var i = 0; i < squad.Length; i++)
                starts[i] = squad[i].localPosition;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = elapsed / duration;
                var hop = Mathf.Sin(t * Mathf.PI) * 0.55f;
                for (var i = 0; i < squad.Length; i++)
                {
                    var pos = starts[i];
                    pos.y += hop;
                    squad[i].localPosition = pos;
                }
                yield return null;
            }
        }

        void ReturnPlayersToBase()
        {
            for (var i = 0; i < 5; i++)
            {
                _homePlayers[i].localPosition = _homeBase[i];
                _awayPlayers[i].localPosition = _awayBase[i];
            }
        }

        Vector3 PitchToWorld(float nx, float ny) => new Vector3(nx * fieldWidth, 0f, ny * fieldLength);

        GameObject Line(string name, Vector3 pos, Vector3 scale, Color c)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(_root, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().material = ProceduralAssets.LitMat(c);
            Destroy(go.GetComponent<Collider>());
            return go;
        }

        void Spot(string name, Vector3 pos)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;
            go.transform.SetParent(_root, false);
            go.transform.localPosition = pos;
            go.transform.localScale = Vector3.one * 0.18f;
            go.GetComponent<Renderer>().material = ProceduralAssets.LitMat(Color.white);
            Destroy(go.GetComponent<Collider>());
        }

        void BoxOutline(string name, Vector3 center, float w, float d, Color c)
        {
            var parent = new GameObject(name).transform;
            parent.SetParent(_root, false);
            parent.localPosition = center;
            Line("Top", new Vector3(0, 0, d / 2f), new Vector3(w, 0.02f, 0.08f), c).transform.SetParent(parent);
            Line("Bottom", new Vector3(0, 0, -d / 2f), new Vector3(w, 0.02f, 0.08f), c).transform.SetParent(parent);
            Line("Left", new Vector3(-w / 2f, 0, 0), new Vector3(0.08f, 0.02f, d), c).transform.SetParent(parent);
            Line("Right", new Vector3(w / 2f, 0, 0), new Vector3(0.08f, 0.02f, d), c).transform.SetParent(parent);
        }

        void CircleMark(string name, Vector3 center, float radius)
        {
            var ring = new GameObject(name);
            ring.transform.SetParent(_root, false);
            ring.transform.localPosition = center;
            var lr = ring.AddComponent<LineRenderer>();
            lr.loop = true;
            lr.positionCount = 48;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.material = ProceduralAssets.LitMat(Color.white);
            lr.startColor = lr.endColor = new Color(1, 1, 1, 0.9f);
            lr.useWorldSpace = false;
            for (var i = 0; i < 48; i++)
            {
                var a = i / 48f * Mathf.PI * 2f;
                lr.SetPosition(i, new Vector3(Mathf.Cos(a) * radius, 0.02f, Mathf.Sin(a) * radius));
            }
        }
    }
}
