using RetroFootball76.Core;
using RetroFootball76.UI;
using UnityEngine;

namespace RetroFootball76.Visual
{
    /// <summary>
    /// Runtime bootstrap for Match scene: 3D camera, pitch, presenter wiring.
    /// </summary>
    public class MatchSceneBootstrap : MonoBehaviour
    {
        void Awake()
        {
            if (FindObjectOfType<PitchCamera3D>() == null)
            {
                var camGo = new GameObject("PitchCamera3D");
                camGo.AddComponent<PitchCamera3D>();
            }

            // Remove legacy 2.5D if present
            var legacy = FindObjectOfType<Pitch2_5DView>();
            if (legacy != null) Destroy(legacy.gameObject);

            Pitch3DView pitch = FindObjectOfType<Pitch3DView>();
            if (pitch == null)
            {
                var pitchGo = new GameObject("Pitch3D");
                pitch = pitchGo.AddComponent<Pitch3DView>();
            }

            MatchController match = FindObjectOfType<MatchController>();
            MatchPitchPresenter presenter = FindObjectOfType<MatchPitchPresenter>();
            if (presenter == null)
            {
                var presGo = new GameObject("MatchPitchPresenter");
                presenter = presGo.AddComponent<MatchPitchPresenter>();
            }

            presenter.matchController = match;
            presenter.pitch3D = pitch;
            presenter.hud = FindObjectOfType<MatchHudController>();
            presenter.camera3D = FindObjectOfType<PitchCamera3D>();
        }
    }
}
