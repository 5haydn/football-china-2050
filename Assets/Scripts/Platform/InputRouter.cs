using UnityEngine;

namespace RetroFootball76.Platform
{
    /// <summary>
    /// Routes touch (iOS) vs keyboard (macOS) to shared game actions.
    /// </summary>
    public class InputRouter : MonoBehaviour
    {
        public bool ConfirmPressed { get; private set; }
        public bool BackPressed { get; private set; }

        void Update()
        {
            ConfirmPressed = false;
            BackPressed = false;

#if UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                ConfirmPressed = true;
#else
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                ConfirmPressed = true;
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
                BackPressed = true;
#endif
        }
    }
}
