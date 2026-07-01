using UnityEngine;

namespace RetroFootball76.Visual
{
    /// <summary>
    /// Broadcast camera: follows the ball and shakes on big moments.
    /// </summary>
    public class PitchCamera3D : MonoBehaviour
    {
        public Vector3 basePosition = new Vector3(0f, 22f, -18f);
        public Vector3 lookAtOffset = new Vector3(0f, 0f, 4f);
        public Color backgroundColor = new Color(0.05f, 0.08f, 0.12f);
        public float fieldOfView = 48f;
        public float followStrength = 0.35f;
        public float goalZoomFov = 42f;

        Camera _cam;
        Transform _followTarget;
        Vector3 _lookAt = Vector3.zero;
        float _shake;
        float _shakeDecay = 6f;
        float _targetFov;

        void Awake()
        {
            _cam = Camera.main;
            if (_cam == null)
            {
                var go = new GameObject("Main Camera");
                _cam = go.AddComponent<Camera>();
                _cam.tag = "MainCamera";
            }

            _cam.orthographic = false;
            _targetFov = fieldOfView;
            _cam.fieldOfView = fieldOfView;
            _cam.transform.position = basePosition;
            _lookAt = lookAtOffset;
            _cam.transform.LookAt(_lookAt);
            _cam.backgroundColor = backgroundColor;
            _cam.clearFlags = CameraClearFlags.SolidColor;
        }

        public void SetFollowTarget(Transform target) => _followTarget = target;

        public void Shake(float intensity = 0.45f) => _shake = Mathf.Max(_shake, intensity);

        public void PulseGoalZoom() => _targetFov = goalZoomFov;

        void LateUpdate()
        {
            if (_cam == null) return;

            var desiredLook = lookAtOffset;
            if (_followTarget != null)
            {
                var ball = _followTarget.localPosition;
                desiredLook = Vector3.Lerp(lookAtOffset, ball, followStrength);
            }

            _lookAt = Vector3.Lerp(_lookAt, desiredLook, Time.deltaTime * 3f);

            var pos = basePosition;
            if (_followTarget != null)
                pos += new Vector3(_followTarget.localPosition.x * 0.25f, 0f, _followTarget.localPosition.z * 0.15f);

            if (_shake > 0.01f)
            {
                pos += Random.insideUnitSphere * _shake;
                _shake = Mathf.Max(0f, _shake - _shakeDecay * Time.deltaTime);
            }

            _cam.transform.position = pos;
            _cam.transform.LookAt(_lookAt);
            _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _targetFov, Time.deltaTime * 4f);
            if (Mathf.Abs(_cam.fieldOfView - _targetFov) < 0.2f)
                _targetFov = fieldOfView;
        }
    }
}
