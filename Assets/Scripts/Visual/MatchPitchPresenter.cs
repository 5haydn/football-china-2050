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

        float _timer;
        const float Interval = 0.4f;

        void Start()
        {
            if (matchController == null)
                matchController = FindObjectOfType<MatchController>();

            if (pitch3D == null)
                pitch3D = FindObjectOfType<Pitch3DView>();

            if (hud == null)
                hud = FindObjectOfType<MatchHudController>();

            if (pitch3D != null && matchController?.CurrentState != null)
            {
                pitch3D.Build(
                    matchController.CurrentState.homeTeam.name,
                    matchController.CurrentState.awayTeam.name);
            }
        }

        void Update()
        {
            if (matchController == null) return;

            _timer += Time.deltaTime;
            if (_timer < Interval) return;
            _timer = 0f;

            if (!matchController.TryGetNextEvent(out var evt)) return;

            if (hud != null)
            {
                hud.AppendLog($"[{evt.minute}'] {evt.description}");
                hud.RefreshScore();
            }

            if (pitch3D != null)
                pitch3D.AnimateEvent(evt.pitchX, evt.pitchY, MapKind(evt.type));
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
