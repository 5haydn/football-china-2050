using UnityEngine;

namespace RetroFootball76.Visual
{
    /// <summary>
    /// Isometric-style 2.5D football pitch with layered depth and animated entities.
    /// </summary>
    public class Pitch2_5DView : MonoBehaviour
    {
        [Header("Field size (world units)")]
        public float fieldWidth = 18f;
        public float fieldLength = 28f;

        [Header("Team colors")]
        public Color homeColor = new Color(0.95f, 0.25f, 0.2f);
        public Color awayColor = new Color(0.2f, 0.55f, 0.95f);
        public Color ballColor = Color.white;

        Transform _fieldRoot;
        Transform _ball;
        Transform[] _homePlayers;
        Transform[] _awayPlayers;
        Vector3 _ballBaseScale;
        bool _initialized;

        public void Build(string homeName, string awayName)
        {
            if (_initialized) return;
            _initialized = true;

            _fieldRoot = new GameObject("Field2_5D").transform;
            _fieldRoot.SetParent(transform, false);

            BuildLayers();
            BuildGoals();
            BuildPlayers(homeName, awayName);
            BuildBall();

            ResetFormation();
        }

        void BuildLayers()
        {
            // Grass base (slightly below pitch plane for depth)
            var grass = CreateQuad("Grass", new Vector3(0, -0.08f, 0), fieldWidth * 1.15f, fieldLength * 1.12f,
                new Color(0.12f, 0.38f, 0.16f), 0);
            grass.transform.SetParent(_fieldRoot, false);

            // Striped pitch — main 2.5D play surface
            var pitch = CreateQuad("Pitch", Vector3.zero, fieldWidth, fieldLength,
                SpriteFactory.StripedField(256, 384, new Color(0.18f, 0.52f, 0.22f), new Color(0.15f, 0.46f, 0.19f)));
            pitch.transform.SetParent(_fieldRoot, false);

            // Center line + circle (depth layer on pitch)
            CreateLineQuad("CenterLine", new Vector3(0, 0.02f, 0), fieldWidth, 0.15f, Color.white, 1);
            CreateCircleMark(new Vector3(0, 0.02f, 0), 3.5f);

            // Penalty boxes (raised slightly for 2.5D layering)
            CreateLineQuad("HomeBox", new Vector3(0, 0.03f, -fieldLength * 0.38f), fieldWidth * 0.55f, fieldLength * 0.22f,
                new Color(1f, 1f, 1f, 0.35f), 2, filled: false);
            CreateLineQuad("AwayBox", new Vector3(0, 0.03f, fieldLength * 0.38f), fieldWidth * 0.55f, fieldLength * 0.22f,
                new Color(1f, 1f, 1f, 0.35f), 2, filled: false);
        }

        void BuildGoals()
        {
            CreateGoalPosts(new Vector3(0, 0, -fieldLength * 0.48f));
            CreateGoalPosts(new Vector3(0, 0, fieldLength * 0.48f));
        }

        void CreateGoalPosts(Vector3 basePos)
        {
            var goal = new GameObject("Goal");
            goal.transform.SetParent(_fieldRoot, false);
            goal.transform.localPosition = basePos;

            var postL = CreateSpriteObject("PostL", SpriteFactory.Circle(Color.white, 32), 0.15f);
            postL.transform.SetParent(goal.transform, false);
            postL.transform.localPosition = new Vector3(-1.2f, 0.6f, 0);
            postL.transform.localScale = new Vector3(0.12f, 1.2f, 1f);

            var postR = CreateSpriteObject("PostR", SpriteFactory.Circle(Color.white, 32), 0.15f);
            postR.transform.SetParent(goal.transform, false);
            postR.transform.localPosition = new Vector3(1.2f, 0.6f, 0);
            postR.transform.localScale = new Vector3(0.12f, 1.2f, 1f);

            var cross = CreateSpriteObject("Crossbar", SpriteFactory.Circle(Color.white, 32), 0.15f);
            cross.transform.SetParent(goal.transform, false);
            cross.transform.localPosition = new Vector3(0, 1.15f, 0);
            cross.transform.localScale = new Vector3(2.5f, 0.12f, 1f);
        }

        void BuildPlayers(string homeName, string awayName)
        {
            _homePlayers = new Transform[5];
            _awayPlayers = new Transform[5];

            for (var i = 0; i < 5; i++)
            {
                _homePlayers[i] = CreatePlayerToken($"Home_{i}", homeColor, homeName, i, true);
                _awayPlayers[i] = CreatePlayerToken($"Away_{i}", awayColor, awayName, i, false);
            }
        }

        Transform CreatePlayerToken(string name, Color color, string teamLabel, int index, bool home)
        {
            var go = new GameObject(name);
            go.transform.SetParent(_fieldRoot, false);

            var body = CreateSpriteObject("Body", SpriteFactory.Circle(color, 64), 0.9f);
            body.transform.SetParent(go.transform, false);
            body.transform.localPosition = new Vector3(0, 0.45f, 0);

            // Shadow on pitch (2.5D depth cue)
            var shadow = CreateSpriteObject("Shadow", SpriteFactory.Circle(new Color(0, 0, 0, 0.35f), 64), 1.1f);
            shadow.transform.SetParent(go.transform, false);
            shadow.transform.localPosition = new Vector3(0, 0.04f, 0);
            shadow.transform.localRotation = Quaternion.Euler(90, 0, 0);

            // Number label
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(body.transform, false);
            labelGo.transform.localPosition = new Vector3(0, 0, 0);
            var tm = labelGo.gameObject.AddComponent<TextMesh>();
            tm.text = (index + 1).ToString();
            tm.fontSize = 48;
            tm.characterSize = 0.08f;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = Color.white;
            tm.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return go.transform;
        }

        void BuildBall()
        {
            var go = new GameObject("Ball");
            go.transform.SetParent(_fieldRoot, false);
            _ball = CreateSpriteObject("BallSprite", SpriteFactory.Circle(ballColor, 48), 0.45f).transform;
            _ball.SetParent(go.transform, false);
            _ball.localPosition = new Vector3(0, 0.35f, 0);
            _ballBaseScale = _ball.localScale;

            var shadow = CreateSpriteObject("BallShadow", SpriteFactory.Circle(new Color(0, 0, 0, 0.4f), 48), 0.5f);
            shadow.transform.SetParent(go.transform, false);
            shadow.transform.localPosition = new Vector3(0, 0.05f, 0);
            shadow.transform.localRotation = Quaternion.Euler(90, 0, 0);

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
                _homePlayers[i].localPosition = PitchToWorld(homeX[i], homeZ[i]);
                _awayPlayers[i].localPosition = PitchToWorld(awayX[i], awayZ[i]);
            }

            MoveBallInstant(0f, 0f);
        }

        public void AnimateEvent(float pitchX, float pitchY, MatchEventKind kind)
        {
            var target = PitchToWorld(pitchX, pitchY);
            var duration = kind == MatchEventKind.Goal ? 0.55f : 0.35f;
            StartCoroutine(AnimateBall(target, kind, duration));
        }

        System.Collections.IEnumerator AnimateBall(Vector3 target, MatchEventKind kind, float duration)
        {
            var start = _ball.localPosition;
            var elapsed = 0f;
            var peakHeight = kind == MatchEventKind.Goal ? 2.2f : kind == MatchEventKind.Shot ? 1.4f : 0.6f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var pos = Vector3.Lerp(start, target, t);
                pos.y = Mathf.Lerp(start.y, target.y, t) + Mathf.Sin(t * Mathf.PI) * peakHeight;
                _ball.localPosition = pos;

                var squash = 1f + Mathf.Sin(t * Mathf.PI) * 0.15f;
                _ball.GetChild(0).localScale = _ballBaseScale * squash;

                yield return null;
            }

            _ball.localPosition = target;
            _ball.GetChild(0).localScale = _ballBaseScale;

            if (kind == MatchEventKind.Goal)
                StartCoroutine(GoalPulse());
        }

        System.Collections.IEnumerator GoalPulse()
        {
            for (var i = 0; i < 3; i++)
            {
                _ball.GetChild(0).localScale = _ballBaseScale * 1.4f;
                yield return new WaitForSeconds(0.12f);
                _ball.GetChild(0).localScale = _ballBaseScale;
                yield return new WaitForSeconds(0.12f);
            }
        }

        void MoveBallInstant(float pitchX, float pitchY)
        {
            var w = PitchToWorld(pitchX, pitchY);
            _ball.localPosition = w;
        }

        Vector3 PitchToWorld(float nx, float ny)
        {
            // nx, ny in -0.5..0.5 → field coords; Y is height offset for 2.5D ball arc base
            return new Vector3(nx * fieldWidth, 0.35f, ny * fieldLength);
        }

        GameObject CreateQuad(string name, Vector3 pos, float w, float h, Color color, int sort = 0)
        {
            return CreateQuad(name, pos, w, h, SpriteFactory.StripedField(64, 64, color, color), sort);
        }

        GameObject CreateQuad(string name, Vector3 pos, float w, float h, Sprite sprite, int sort = 0)
        {
            var go = CreateSpriteObject(name, sprite, 1f);
            go.transform.SetParent(_fieldRoot, false);
            go.transform.localPosition = pos;
            go.transform.localScale = new Vector3(w, h, 1f);
            go.transform.localRotation = Quaternion.Euler(90, 0, 0);
            var sr = go.GetComponent<SpriteRenderer>();
            sr.sortingOrder = sort;
            return go;
        }

        Transform CreateLineQuad(string name, Vector3 localPos, float w, float h, Color color, int sort, bool filled = true)
        {
            var alpha = filled ? color.a : 0.15f;
            var c = new Color(color.r, color.g, color.b, alpha);
            var go = CreateQuad(name, localPos, w, h, c, sort);
            if (!filled)
            {
                var sr = go.GetComponent<SpriteRenderer>();
                sr.drawMode = SpriteDrawMode.Sliced;
            }
            return go.transform;
        }

        void CreateCircleMark(Vector3 pos, float radius)
        {
            var go = CreateSpriteObject("CenterCircle", SpriteFactory.Circle(new Color(1, 1, 1, 0.25f), 128), radius * 2f);
            go.transform.SetParent(_fieldRoot, false);
            go.transform.localPosition = pos;
            go.transform.localRotation = Quaternion.Euler(90, 0, 0);
            go.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }

        GameObject CreateSpriteObject(string name, Sprite sprite, float scale)
        {
            var go = new GameObject(name);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = 5;
            go.transform.localScale = Vector3.one * scale;
            return go;
        }
    }

    public enum MatchEventKind
    {
        Kickoff,
        Shot,
        Goal,
        Save,
        Turnover,
        HalfTime,
        FullTime,
        Other
    }
}
