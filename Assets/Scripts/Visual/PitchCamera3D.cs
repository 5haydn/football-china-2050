using UnityEngine;

namespace RetroFootball76.Visual
{
    /// <summary>
    /// Perspective camera for full 3D match view.
    /// </summary>
    public class PitchCamera3D : MonoBehaviour
    {
        public Vector3 position = new Vector3(0f, 22f, -18f);
        public Vector3 lookAt = new Vector3(0f, 0f, 4f);
        public Color backgroundColor = new Color(0.05f, 0.08f, 0.12f);
        public float fieldOfView = 48f;

        void Awake()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera");
                cam = go.AddComponent<Camera>();
                cam.tag = "MainCamera";
            }

            cam.orthographic = false;
            cam.fieldOfView = fieldOfView;
            cam.transform.position = position;
            cam.transform.LookAt(lookAt);
            cam.backgroundColor = backgroundColor;
            cam.clearFlags = CameraClearFlags.SolidColor;
        }
    }
}
