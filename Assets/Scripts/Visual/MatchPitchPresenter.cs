using RetroFootball76.Match;
using RetroFootball76.UI;
using UnityEngine;

namespace RetroFootball76.Visual
{
    /// <summary>
    /// Single playback driver for match events → 3D pitch + HUD log.
    /// </summary>
    public class MatchPitchPresenter : MonoBehaviour
    {
        public MatchController matchController;
        public Pitch3DView pitch3D;
        public MatchHudController hud;
        public PitchCamera3D camera3D;

        float _timer;
        float _slowMoTimer;
        const float Interval = 0.4f;

        void Start()
        {
            if (matchController == null)
                matchController = FindObjectOfType<MatchController>();

            if (pitch3D == null)
                pitch3D = FindObjectOfType<Pitch3DView>();

            if (hud == null)
                hud = FindObjectOfType<MatchHudController>();

            if (camera3D == null)
                camera3D = FindObjectOfType<PitchCamera3D>();

            if (pitch3D != null && matchController?.CurrentState != null)
            {
                var state = matchController.CurrentState;
                pitch3D.Build(state.homeTeam.name, state.awayTeam.name);
                if (camera3D != null)
                    camera3D.SetFollowTarget(pitch3D.BallTransform);
            }
        }

        void Update()
        {
            if (_slowMoTimer > 0f)
            {
                _slowMoTimer -= Time.unscaledDeltaTime;
                if (_slowMoTimer <= 0f)
                    Time.timeScale = 1f;
            }

            if (matchController == null) return;

            _timer += Time.deltaTime;
            if (_timer < Interval) return;
            _timer = 0f;

            if (!matchController.TryGetNextEvent(out var evt)) return;

            if (hud != null)
            {
                hud.AppendLog($"[{evt.minute}'] {evt.description}");
                hud.RefreshScore();
                hud.UpdateClock(evt.minute, evt.type == MatchEventType.HalfTime);
            }

            var kind = MapKind(evt.type);

            if (pitch3D != null)
                pitch3D.AnimateEvent(evt.pitchX, evt.pitchY, kind, evt.teamId, evt.playerName);

            if (kind == MatchEventKind.Goal)
            {
                camera3D?.Shake(0.55f);
                camera3D?.PulseGoalZoom();
                hud?.ShowBanner($"GOAL! {evt.playerName}", 2.5f);
                Time.timeScale = 0.35f;
                _slowMoTimer = 1.8f;
            }
            else if (kind == MatchEventKind.Save)
            {
                camera3D?.Shake(0.2f);
            }
        }

        static MatchEventKind MapKind(MatchEventType type)
        {
            switch (type)
            {
                case MatchEventType.Kickoff: return MatchEventKind.Kickoff;
                case MatchEventType.Shot: return MatchEventKind.Shot;
                case MatchEventType.Goal: return MatchEventKind.Goal;
                case MatchEventType.Save: return MatchEventKind.Save;
                case MatchEventType.Turnover: return MatchEventKind.Turnover;
                case MatchEventType.HalfTime: return MatchEventKind.HalfTime;
                case MatchEventType.FullTime: return MatchEventKind.FullTime;
                default: return MatchEventKind.Other;
            }
        }
    }
}
