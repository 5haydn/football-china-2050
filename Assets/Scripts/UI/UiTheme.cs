using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RetroFootball76.UI
{
    /// <summary>
    /// Shared retro broadcast palette and UI factory helpers.
    /// </summary>
    public static class UiTheme
    {
        public static readonly Color BgDark = new Color(0.04f, 0.07f, 0.11f);
        public static readonly Color Panel = new Color(0.11f, 0.16f, 0.22f, 0.95f);
        public static readonly Color PanelBorder = new Color(1f, 1f, 1f, 0.08f);
        public static readonly Color Accent = new Color(0f, 0.82f, 0.65f);
        public static readonly Color Gold = new Color(0.96f, 0.72f, 0.28f);
        public static readonly Color Cream = new Color(0.94f, 0.91f, 0.84f);
        public static readonly Color Muted = new Color(0.55f, 0.62f, 0.7f);
        public static readonly Color Home = new Color(0.92f, 0.22f, 0.18f);
        public static readonly Color Away = new Color(0.18f, 0.52f, 0.95f);
        public static readonly Color BtnPrimary = new Color(0f, 0.78f, 0.62f);
        public static readonly Color BtnSecondary = new Color(0.16f, 0.22f, 0.3f);

        public static void StylePanel(Image img)
        {
            img.color = Panel;
        }

        public static TMP_Text CreateLabel(Transform parent, string text, float size, Color color,
            TextAlignmentOptions align = TextAlignmentOptions.Left)
        {
            var go = new GameObject("Label");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 40);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.alignment = align;
            return tmp;
        }

        public static Button CreateButton(Transform parent, string label, Vector2 size, Color bg, Color textColor)
        {
            var go = new GameObject(label + "Button");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            var img = go.AddComponent<Image>();
            img.color = bg;
            var btn = go.AddComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = bg * 1.1f;
            colors.pressedColor = bg * 0.85f;
            btn.colors = colors;

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 20;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = textColor;
            tmp.alignment = TextAlignmentOptions.Center;
            return btn;
        }

        public static GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            StylePanel(img);
            return go;
        }
    }
}
