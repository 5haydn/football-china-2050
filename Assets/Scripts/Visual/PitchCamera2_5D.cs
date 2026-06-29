using UnityEngine;

namespace RetroFootball76.Visual
{
    /// <summary>
    /// Sets up angled orthographic camera for 2.5D pitch view.
    /// </summary>
    public class PitchCamera2_5D : MonoBehaviour
    {
        public Vector3 cameraOffset = new Vector3(0f, 16f, -14f);
        public Vector3 lookAt = new Vector3(0f, 0f, 2f);
        public float orthoSize = 11f;
        public Color backgroundColor = new Color(0.06f, 0.1f, 0.14f);

        void Awake()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera");
                cam = go.AddComponent<Camera>();
                cam.tag = "MainCamera";
            }

            cam.orthographic = true;
            cam.orthographicSize = orthoSize;
            cam.transform.position = cameraOffset;
            cam.transform.LookAt(lookAt);
            cam.backgroundColor = backgroundColor;
            cam.clearFlags = CameraClearFlags.SolidColor;
        }
    }
}
