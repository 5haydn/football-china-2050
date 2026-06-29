using UnityEngine;

namespace RetroFootball76.Visual
{
    /// <summary>
    /// Runtime sprite textures for 2.5D pitch entities (no external assets).
    /// </summary>
    public static class SpriteFactory
    {
        public static Sprite Circle(Color color, int size = 64)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            var center = (size - 1) / 2f;
            var radius = size * 0.42f;

            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
            {
                var dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                var a = dist < radius ? 1f : dist < radius + 1.5f ? 1f - (dist - radius) / 1.5f : 0f;
                tex.SetPixel(x, y, new Color(color.r, color.g, color.b, color.a * a));
            }
            tex.Apply();

            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        public static Sprite StripedField(int w, int h, Color a, Color b, int stripes = 12)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            var stripeW = w / stripes;

            for (var y = 0; y < h; y++)
            for (var x = 0; x < w; x++)
            {
                var stripe = x / stripeW;
                tex.SetPixel(x, y, stripe % 2 == 0 ? a : b);
            }
            tex.Apply();

            return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), w / 10f);
        }
    }
}
