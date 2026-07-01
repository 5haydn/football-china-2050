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
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            new GameObject("BootLoader").AddComponent<BootLoader>();
            SaveScene("Assets/Scenes/Boot.unity");
        }

        static void CreateMainMenu()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EnsureEventSystem();
            var canvas = CreateRootCanvas();

            var bg = UiTheme.CreatePanel(canvas.transform, "Background", Vector2.zero, Vector2.one);
            bg.GetComponent<Image>().color = UiTheme.BgDark;

            var panel = UiTheme.CreatePanel(canvas.transform, "MenuPanel", new Vector2(div(1, 2), 0.2f), new Vector2(0.5f, 0.8f));
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(520, 0);

            var eyebrow = UiTheme.CreateLabel(panel.transform, "RETRO FOOTBALL", 14, UiTheme.Gold, TextAlignmentOptions.Center);
            SetRect(eyebrow, new Vector2(0, 140), new Vector2(480, 28));
            eyebrow.characterSpacing = 8;

            var title = UiTheme.CreateLabel(panel.transform, "'76", 72, UiTheme.Cream, TextAlignmentOptions.Center);
            SetRect(title, new Vector2(0, 80), new Vector2(480, 90));
            title.fontStyle = FontStyles.Bold;

            var subtitle = UiTheme.CreateLabel(panel.transform, "1974 World Cup · 1976 European Championship", 18, UiTheme.Muted, TextAlignmentOptions.Center);
            SetRect(subtitle, new Vector2(0, 20), new Vector2(480, 36));

            var ctrl = panel.AddComponent<MainMenuController>();
            var playBtn = UiTheme.CreateButton(panel.transform, "▶  KICK OFF", new Vector2(280, 52), UiTheme.BtnPrimary, Color.black);
            SetRect(playBtn.GetComponent<RectTransform>(), new Vector2(0, -50), new Vector2(280, 52));

            var quitBtn = UiTheme.CreateButton(panel.transform, "Quit", new Vector2(160, 44), UiTheme.BtnSecondary, UiTheme.Cream);
            SetRect(quitBtn.GetComponent<RectTransform>(), new Vector2(0, -120), new Vector2(160, 44));

            var hints = UiTheme.CreateLabel(panel.transform, "Space / Enter — Kick Off   ·   Esc — Quit", 14, UiTheme.Muted, TextAlignmentOptions.Center);
            SetRect(hints, new Vector2(0, -180), new Vector2(480, 28));

            ctrl.Wire(playBtn, quitBtn, subtitle);

            SaveScene("Assets/Scenes/MainMenu.unity");
        }

        static void CreateTeamSelect()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EnsureEventSystem();
            var canvas = CreateRootCanvas();

            var bg = UiTheme.CreatePanel(canvas.transform, "Background", Vector2.zero, Vector2.one);
            bg.GetComponent<Image>().color = UiTheme.BgDark;

            var header = UiTheme.CreateLabel(canvas.transform, "SELECT YOUR TEAMS", 22, UiTheme.Gold, TextAlignmentOptions.Center);
            SetRect(header, new Vector2(0, 260), new Vector2(700, 40));
            header.characterSpacing = 4;
            header.fontStyle = FontStyles.Bold;

            var homeCard = CreateTeamCard(canvas.transform, "HomeCard", new Vector2(-200, 40), UiTheme.Home, "HOME");
            var awayCard = CreateTeamCard(canvas.transform, "AwayCard", new Vector2(200, 40), UiTheme.Away, "AWAY");

            var ctrlGo = new GameObject("TeamSelectController");
            ctrlGo.transform.SetParent(canvas.transform, false);
            var ctrl = ctrlGo.AddComponent<TeamSelectController>();

            var startBtn = UiTheme.CreateButton(canvas.transform, "▶  START MATCH", new Vector2(300, 52), UiTheme.BtnPrimary, Color.black);
            SetRect(startBtn.GetComponent<RectTransform>(), new Vector2(0, -200), new Vector2(300, 52));

            var backBtn = UiTheme.CreateButton(canvas.transform, "← Back", new Vector2(140, 44), UiTheme.BtnSecondary, UiTheme.Cream);
            SetRect(backBtn.GetComponent<RectTransform>(), new Vector2(-220, -200), new Vector2(140, 44));

            ctrl.Wire(
                homeCard.GetComponentInChildren<TMP_Dropdown>(),
                awayCard.GetComponentInChildren<TMP_Dropdown>(),
                homeCard.transform.Find("StatsText")?.GetComponent<TMP_Text>(),
                awayCard.transform.Find("StatsText")?.GetComponent<TMP_Text>(),
                startBtn, backBtn);

            var hints = UiTheme.CreateLabel(canvas.transform,
                "← → Home team   ·   ↑ ↓ Away team   ·   Space Start   ·   Esc Back",
                14, UiTheme.Muted, TextAlignmentOptions.Center);
            SetRect(hints, new Vector2(0, -260), new Vector2(800, 28));

            SaveScene("Assets/Scenes/TeamSelect.unity");
        }

        static GameObject CreateTeamCard(Transform parent, string name, Vector2 pos, Color accent, string label)
        {
            var card = UiTheme.CreatePanel(parent, name, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            var rect = card.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(340, 320);

            var tag = UiTheme.CreateLabel(card.transform, label, 12, accent, TextAlignmentOptions.Left);
            SetRect(tag, new Vector2(-140, 130), new Vector2(120, 24));
            tag.fontStyle = FontStyles.Bold;
            tag.characterSpacing = 3;

            var dropdownGo = CreateDropdown(card.transform, "Dropdown");
            var ddRect = dropdownGo.GetComponent<RectTransform>();
            ddRect.anchoredPosition = new Vector2(0, 90);
            ddRect.sizeDelta = new Vector2(300, 36);

            var stats = UiTheme.CreateLabel(card.transform, "", 14, UiTheme.Cream, TextAlignmentOptions.TopLeft);
            stats.gameObject.name = "StatsText";
            SetRect(stats, new Vector2(0, -30), new Vector2(300, 180));

            return card;
        }

        static void CreateMatch()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            new GameObject("MatchSceneBootstrap").AddComponent<RetroFootball76.Visual.MatchSceneBootstrap>();
            new GameObject("MatchRoot").AddComponent<MatchController>();

            EnsureEventSystem();
            var canvas = CreateRootCanvas();
            BuildMatchHud(canvas.transform);
            SaveScene("Assets/Scenes/Match.unity");
        }

        static void BuildMatchHud(Transform canvas)
        {
            var hud = UiTheme.CreatePanel(canvas, "MatchHud", new Vector2(0, 0), new Vector2(1, 0.3f));
            var hudCtrl = hud.AddComponent<MatchHudController>();

            var scoreBar = UiTheme.CreatePanel(hud.transform, "ScoreBar", new Vector2(0.05f, 0.55f), new Vector2(0.95f, 0.95f));
            scoreBar.GetComponent<Image>().color = new Color(0.08f, 0.12f, 0.17f, 0.98f);

            var clock = UiTheme.CreateLabel(scoreBar.transform, "0'", 22, UiTheme.Gold, TextAlignmentOptions.Left);
            SetRect(clock, new Vector2(-360, 0), new Vector2(80, 36));
            clock.fontStyle = FontStyles.Bold;

            var score = UiTheme.CreateLabel(scoreBar.transform, "—  0 - 0  —", 30, UiTheme.Cream, TextAlignmentOptions.Center);
            SetRect(score, Vector2.zero, new Vector2(800, 50));
            score.fontStyle = FontStyles.Bold;

            var banner = UiTheme.CreatePanel(hud.transform, "GoalBanner", new Vector2(0.25f, 0.72f), new Vector2(0.75f, 0.92f));
            banner.GetComponent<Image>().color = new Color(0.96f, 0.72f, 0.28f, 0.95f);
            var bannerText = UiTheme.CreateLabel(banner.transform, "GOAL!", 28, Color.black, TextAlignmentOptions.Center);
            SetRect(bannerText, Vector2.zero, new Vector2(600, 44));
            bannerText.fontStyle = FontStyles.Bold;
            banner.SetActive(false);

            var feedLabel = UiTheme.CreateLabel(hud.transform, "MATCH FEED", 11, UiTheme.Muted, TextAlignmentOptions.Left);
            SetRect(feedLabel, new Vector2(-380, 55), new Vector2(120, 20));
            feedLabel.characterSpacing = 2;

            var log = UiTheme.CreateLabel(hud.transform, "", 15, UiTheme.Cream, TextAlignmentOptions.TopLeft);
            SetRect(log, new Vector2(0, -10), new Vector2(860, 110));
            log.name = "LogText";

            var menuBtn = UiTheme.CreateButton(hud.transform, "Menu", new Vector2(100, 36), UiTheme.BtnSecondary, UiTheme.Cream);
            SetRect(menuBtn.GetComponent<RectTransform>(), new Vector2(400, 70), new Vector2(100, 36));

            var hints = UiTheme.CreateLabel(hud.transform, "Esc — Main Menu", 12, UiTheme.Muted, TextAlignmentOptions.Right);
            SetRect(hints, new Vector2(380, -55), new Vector2(160, 20));

            hudCtrl.Wire(score, clock, log, menuBtn, bannerText, banner.GetComponent<Image>());
        }

        static GameObject CreateDropdown(Transform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            var img = go.AddComponent<Image>();
            img.color = new Color(0.08f, 0.12f, 0.17f);
            var dd = go.AddComponent<TMP_Dropdown>();

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRect = labelGo.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(12, 4);
            labelRect.offsetMax = new Vector2(-28, -4);
            var label = labelGo.AddComponent<TextMeshProUGUI>();
            label.fontSize = 16;
            label.color = UiTheme.Cream;
            dd.captionText = label;

            var arrowGo = new GameObject("Arrow");
            arrowGo.transform.SetParent(go.transform, false);
            var arrowRect = arrowGo.AddComponent<RectTransform>();
            arrowRect.anchorMin = new Vector2(1, 0.5f);
            arrowRect.anchorMax = new Vector2(1, 0.5f);
            arrowRect.anchoredPosition = new Vector2(-14, 0);
            arrowRect.sizeDelta = new Vector2(16, 16);
            var arrow = arrowGo.AddComponent<TextMeshProUGUI>();
            arrow.text = "▼";
            arrow.fontSize = 12;
            arrow.color = UiTheme.Muted;
            arrow.alignment = TextAlignmentOptions.Center;

            return go;
        }

        static GameObject CreateRootCanvas()
        {
            var canvasGo = new GameObject("Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();
            return canvasGo;
        }

        static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null) return;
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        static void SetRect(Component c, Vector2 pos, Vector2 size)
        {
            var rect = c.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
        }

        static float div(float a, float b) => a / b;

        static void SaveScene(string path) => EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
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
