using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using RetroFootball76.Core;
using RetroFootball76.UI;

namespace RetroFootball76.Editor
{
    /// <summary>
    /// Menu: Retro Football → Generate MVP Scenes (run once after cloning).
    /// </summary>
    public static class SceneGenerator
    {
        [MenuItem("Retro Football/Generate MVP Scenes")]
        public static void GenerateAll()
        {
            EnsureFolder("Assets/Scenes");
            CreateBoot();
            CreateMainMenu();
            CreateTeamSelect();
            CreateMatch();
            AddScenesToBuildSettings();
            Debug.Log("Retro Football MVP scenes generated. Open Boot.unity and press Play.");
        }

        static void CreateBoot()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var boot = new GameObject("BootLoader");
            boot.AddComponent<BootLoader>();
            SaveScene("Assets/Scenes/Boot.unity");
        }

        static void CreateMainMenu()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            BuildCanvas(scene, "MainMenuController", go => go.AddComponent<MainMenuController>());
            SaveScene("Assets/Scenes/MainMenu.unity");
        }

        static void CreateTeamSelect()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            BuildCanvas(scene, "TeamSelectController", go => go.AddComponent<TeamSelectController>());
            SaveScene("Assets/Scenes/TeamSelect.unity");
        }

        static void CreateMatch()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var bootstrap = new GameObject("MatchSceneBootstrap");
            bootstrap.AddComponent<RetroFootball76.Visual.MatchSceneBootstrap>();

            var root = new GameObject("MatchRoot");
            root.AddComponent<MatchController>();

            BuildMatchHud(scene);
            SaveScene("Assets/Scenes/Match.unity");
        }

        static void BuildMatchHud(Scene scene)
        {
            var canvasGo = new GameObject("Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();

            if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            var hud = new GameObject("MatchHud");
            hud.transform.SetParent(canvasGo.transform, false);
            var hudRect = hud.AddComponent<RectTransform>();
            hudRect.anchorMin = new Vector2(0, 0);
            hudRect.anchorMax = new Vector2(1, 0.28f);
            hudRect.offsetMin = Vector2.zero;
            hudRect.offsetMax = Vector2.zero;
            hud.AddComponent<UnityEngine.UI.Image>().color = new Color(0.06f, 0.1f, 0.14f, 0.92f);

            var hudCtrl = hud.AddComponent<MatchHudController>();

            var score = CreateText(hud.transform, "", 28, new Vector2(0, 60));
            score.name = "ScoreText";
            score.alignment = TextAlignmentOptions.Center;
            var scoreRect = score.GetComponent<RectTransform>();
            scoreRect.sizeDelta = new Vector2(900, 50);

            var log = CreateText(hud.transform, "", 16, new Vector2(0, -20));
            log.name = "LogText";
            log.alignment = TextAlignmentOptions.TopLeft;
            var logRect = log.GetComponent<RectTransform>();
            logRect.sizeDelta = new Vector2(900, 140);

            var menuBtn = new GameObject("MenuButton");
            menuBtn.transform.SetParent(hud.transform, false);
            var btnRect = menuBtn.AddComponent<RectTransform>();
            btnRect.anchoredPosition = new Vector2(-420, 60);
            btnRect.sizeDelta = new Vector2(120, 36);
            var btn = menuBtn.AddComponent<UnityEngine.UI.Button>();
            menuBtn.AddComponent<UnityEngine.UI.Image>().color = new Color(0.2f, 0.75f, 0.6f);
            var btnLabel = CreateText(menuBtn.transform, "Menu", 18, Vector2.zero);
            btnLabel.alignment = TextAlignmentOptions.Center;

            hudCtrl.Wire(score, log, btn);
        }

        static void BuildCanvas(Scene scene, string controllerName, System.Action<GameObject> configure)
        {
            var canvasGo = new GameObject("Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();

            if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            var ctrl = new GameObject(controllerName);
            ctrl.transform.SetParent(canvasGo.transform, false);
            configure(ctrl);

            var title = CreateText(canvasGo.transform, "Retro Football '76", 48, new Vector2(0, 200));
            title.alignment = TextAlignmentOptions.Center;
        }

        static TextMeshProUGUI CreateText(Transform parent, string text, float size, Vector2 pos)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(800, 120);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = Color.white;
            return tmp;
        }

        static void SaveScene(string path)
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        }

        static void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parts = path.Split('/');
                var current = parts[0];
                for (var i = 1; i < parts.Length; i++)
                {
                    var next = current + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                        AssetDatabase.CreateFolder(current, parts[i]);
                    current = next;
                }
            }
        }

        static void AddScenesToBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/Boot.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/TeamSelect.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Match.unity", true),
            };
        }
    }
}
