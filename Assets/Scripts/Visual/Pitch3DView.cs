using UnityEngine;

namespace RetroFootball76.Visual
{
    /// <summary>
    /// Full 3D football pitch with primitive meshes, goals, players, and animated ball.
    /// </summary>
    public class Pitch3DView : MonoBehaviour
    {
        public float fieldWidth = 20f;
        public float fieldLength = 32f;
        public Color homeColor = new Color(0.95f, 0.25f, 0.2f);
        public Color awayColor = new Color(0.2f, 0.55f, 0.95f);
        public Color ballColor = Color.white;
        public Color grassDark = new Color(0.14f, 0.44f, 0.18f);
        public Color grassLight = new Color(0.2f, 0.55f, 0.24f);

        Transform _root;
        Transform _ball;
        Transform[] _homePlayers;
        Transform[] _awayPlayers;
        bool _built;

        public void Build(string homeName, string awayName)
        {
            if (_built) return;
            _built = true;

            _root = new GameObject("Pitch3D").transform;
            _root.SetParent(transform, false);

            BuildStadiumBase();
            BuildField();
            BuildMarkings();
            BuildGoals();
            BuildPlayers(homeName, awayName);
            BuildBall();
            ResetFormation();
        }

        void BuildStadiumBase()
        {
            var baseGo = CreateCube("StadiumBase", new Vector3(0, -0.5f, 0),
                new Vector3(fieldWidth * 1.4f, 1f, fieldLength * 1.35f), new Color(0.08f, 0.12f, 0.16f));
            baseGo.transform.SetParent(_root, false);
        }

        void BuildField()
        {
            var pitch = GameObject.CreatePrimitive(PrimitiveType.Plane);
            pitch.name = "Grass";
            pitch.transform.SetParent(_root, false);
            pitch.transform.localScale = new Vector3(fieldWidth / 10f, 1f, fieldLength / 10f);
            pitch.GetComponent<Renderer>().material = Mat(grassLight);
            Destroy(pitch.GetComponent<Collider>());
        }

        void BuildMarkings()
        {
            CreateLine("CenterLine", new Vector3(0, 0.02f, 0), new Vector3(fieldWidth, 0.04f, 0.12f), Color.white);
            CreateCircle("CenterCircle", new Vector3(0, 0.02f, 0), 4f);

            CreateLine("HomeBox", new Vector3(0, 0.02f, -fieldLength * 0.38f),
                new Vector3(fieldWidth * 0.55f, 0.04f, fieldLength * 0.2f), new Color(1, 1, 1, 0.5f));
            CreateLine("AwayBox", new Vector3(0, 0.02f, fieldLength * 0.38f),
                new Vector3(fieldWidth * 0.55f, 0.04f, fieldLength * 0.2f), new Color(1, 1, 1, 0.5f));
        }

        void BuildGoals()
        {
            BuildGoal(new Vector3(0, 0, -fieldLength * 0.48f));
            BuildGoal(new Vector3(0, 0, fieldLength * 0.48f));
        }

        void BuildGoal(Vector3 pos)
        {
            var goal = new GameObject("Goal").transform;
            goal.SetParent(_root, false);
            goal.localPosition = pos;

            CreateCube("PostL", new Vector3(-1.3f, 0.65f, 0), new Vector3(0.12f, 1.3f, 0.12f), Color.white, goal);
            CreateCube("PostR", new Vector3(1.3f, 0.65f, 0), new Vector3(0.12f, 1.3f, 0.12f), Color.white, goal);
            CreateCube("Crossbar", new Vector3(0, 1.25f, 0), new Vector3(2.7f, 0.12f, 0.12f), Color.white, goal);
            CreateCube("Net", new Vector3(0, 0.65f, -0.4f), new Vector3(2.4f, 1.2f, 0.05f), new Color(1, 1, 1, 0.15f), goal);
        }

        void BuildPlayers(string homeName, string awayName)
        {
            _homePlayers = new Transform[5];
            _awayPlayers = new Transform[5];
            for (var i = 0; i < 5; i++)
            {
                _homePlayers[i] = CreatePlayer($"Home_{i}", homeColor, i);
                _awayPlayers[i] = CreatePlayer($"Away_{i}", awayColor, i);
            }
        }

        Transform CreatePlayer(string name, Color color, int index)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = name;
            go.transform.SetParent(_root, false);
            go.transform.localScale = new Vector3(0.55f, 0.7f, 0.55f);
            go.GetComponent<Renderer>().material = Mat(color);
            Destroy(go.GetComponent<Collider>());

            var label = new GameObject("Label");
            label.transform.SetParent(go.transform, false);
            label.transform.localPosition = new Vector3(0, 0.55f, 0);
            var tm = label.AddComponent<TextMesh>();
            tm.text = (index + 1).ToString();
            tm.fontSize = 64;
            tm.characterSize = 0.06f;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = Color.white;
            tm.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return go.transform;
        }

        void BuildBall()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Ball";
            go.transform.SetParent(_root, false);
            go.transform.localScale = Vector3.one * 0.45f;
            go.GetComponent<Renderer>().material = Mat(ballColor);
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
                _homePlayers[i].localPosition = PitchToWorld(homeX[i], homeZ[i]) + Vector3.up * 0.7f;
                _awayPlayers[i].localPosition = PitchToWorld(awayX[i], awayZ[i]) + Vector3.up * 0.7f;
            }

            _ball.localPosition = PitchToWorld(0, 0) + Vector3.up * 0.35f;
        }

        public void AnimateEvent(float pitchX, float pitchY, MatchEventKind kind)
        {
            var target = PitchToWorld(pitchX, pitchY) + Vector3.up * 0.35f;
            var duration = kind == MatchEventKind.Goal ? 0.55f : 0.35f;
            StartCoroutine(AnimateBall(target, kind, duration));
        }

        System.Collections.IEnumerator AnimateBall(Vector3 target, MatchEventKind kind, float duration)
        {
            var start = _ball.localPosition;
            var elapsed = 0f;
            var peak = kind == MatchEventKind.Goal ? 3.5f : kind == MatchEventKind.Shot ? 2.2f : 1f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var pos = Vector3.Lerp(start, target, t);
                pos.y += Mathf.Sin(t * Mathf.PI) * peak;
                _ball.localPosition = pos;
                _ball.Rotate(Vector3.right, 360f * Time.deltaTime / duration, Space.World);
                yield return null;
            }

            _ball.localPosition = target;
            if (kind == MatchEventKind.Goal)
                StartCoroutine(GoalPulse());
        }

        System.Collections.IEnumerator GoalPulse()
        {
            var baseScale = Vector3.one * 0.45f;
            for (var i = 0; i < 3; i++)
            {
                _ball.localScale = baseScale * 1.5f;
                yield return new WaitForSeconds(0.1f);
                _ball.localScale = baseScale;
                yield return new WaitForSeconds(0.1f);
            }
        }

        Vector3 PitchToWorld(float nx, float ny)
        {
            return new Vector3(nx * fieldWidth, 0f, ny * fieldLength);
        }

        void CreateLine(string name, Vector3 pos, Vector3 scale, Color color)
        {
            var go = CreateCube(name, pos, scale, color);
            go.transform.SetParent(_root, false);
        }

        void CreateCircle(string name, Vector3 center, float radius)
        {
            var ring = new GameObject(name);
            ring.transform.SetParent(_root, false);
            ring.transform.localPosition = center;
            var lr = ring.AddComponent<LineRenderer>();
            lr.loop = true;
            lr.positionCount = 32;
            lr.startWidth = 0.08f;
            lr.endWidth = 0.08f;
            lr.material = Mat(Color.white);
            lr.startColor = new Color(1, 1, 1, 0.45f);
            lr.endColor = new Color(1, 1, 1, 0.45f);
            lr.useWorldSpace = false;

            for (var i = 0; i < 32; i++)
            {
                var a = i / 32f * Mathf.PI * 2f;
                lr.SetPosition(i, new Vector3(Mathf.Cos(a) * radius, 0, Mathf.Sin(a) * radius));
            }
        }

        GameObject CreateCube(string name, Vector3 pos, Vector3 scale, Color color, Transform parent = null)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().material = Mat(color);
            Destroy(go.GetComponent<Collider>());
            if (parent != null) go.transform.SetParent(parent, false);
            return go;
        }

        static Material Mat(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit")
                ?? Shader.Find("Standard")
                ?? Shader.Find("Diffuse");
            var m = new Material(shader);
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
            else m.color = color;
            return m;
        }
    }
}
