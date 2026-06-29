using UnityEngine;

namespace RetroFootball76.Platform
{
    /// <summary>
    /// Routes touch (iOS) vs keyboard (macOS) to shared game actions.
    /// </summary>
    public class InputRouter : MonoBehaviour
    {
        public static InputRouter Instance { get; private set; }

        public bool ConfirmPressed { get; private set; }
        public bool BackPressed { get; private set; }
        public int HorizontalAxis { get; private set; }
        public int VerticalAxis { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            ConfirmPressed = false;
            BackPressed = false;
            HorizontalAxis = 0;
            VerticalAxis = 0;

#if UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                ConfirmPressed = true;
#else
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
                ConfirmPressed = true;

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
                BackPressed = true;

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.H))
                HorizontalAxis = -1;
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.L))
                HorizontalAxis = 1;

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.K))
                VerticalAxis = 1;
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.J))
                VerticalAxis = -1;
#endif
        }
    }
}
