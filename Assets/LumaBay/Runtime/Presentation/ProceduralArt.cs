using System.Collections.Generic;
using UnityEngine;

namespace LumaBay
{
    public static class ProceduralArt
    {
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();
        private static readonly Color[] PieceColors =
        {
            new Color(0.54f, 0.30f, 0.94f),
            new Color(0.98f, 0.30f, 0.18f),
            new Color(1f, 0.68f, 0.16f),
            new Color(0.22f, 0.73f, 0.38f),
            new Color(0.13f, 0.68f, 1f),
            new Color(0.98f, 0.30f, 0.62f)
        };

        public static Color Navy => NauticalTheme.Navy;
        public static Color DeepSea => NauticalTheme.Ocean;
        public static Color Teal => NauticalTheme.OceanBright;
        public static Color Gold => NauticalTheme.Gold;
        public static Color Cream => NauticalTheme.Pearl;
        public static Color Coral => NauticalTheme.Coral;

        public static Sprite Piece(PieceKind kind)
        {
            Sprite authored = LumaBayArtPack.Piece(kind);
            if (authored != null) return authored;

            string key = $"fallback_piece_{(int)kind}";
            if (Cache.TryGetValue(key, out Sprite cached)) return cached;

            const int size = 160;
            Texture2D texture = NewTexture(size, size);
            Color[] pixels = new Color[size * size];
            Color baseColor = PieceColors[Mathf.Clamp((int)kind, 0, PieceColors.Length - 1)];
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 delta = new Vector2(x, y) - center;
                    float d = delta.magnitude / 66f;
                    if (d > 1.05f)
                    {
                        pixels[y * size + x] = Color.clear;
                        continue;
                    }

                    float highlight = Mathf.Clamp01(1f - Vector2.Distance(new Vector2(x, y), center + new Vector2(-24f, 28f)) / 75f);
                    Color color = Color.Lerp(baseColor * 0.48f, baseColor, 1f - d * 0.62f);
                    color = Color.Lerp(color, Color.white, highlight * 0.42f);
                    if (d > 0.88f) color = Color.Lerp(new Color(0.55f, 0.32f, 0.10f), new Color(1f, 0.82f, 0.36f), (1.05f - d) / 0.17f);
                    color.a = Mathf.Clamp01((1.05f - d) * 18f);
                    pixels[y * size + x] = color;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
            sprite.name = key;
            Cache[key] = sprite;
            return sprite;
        }

        public static Sprite OrnateFrame(string key, Color fill, bool elevated = false)
        {
            Sprite authored = LumaBayArtPack.TintedPanel(fill, elevated);
            if (authored != null) return authored;
            return CreateFallbackFrame(key, fill, elevated);
        }

        public static Sprite Pearl(string key = "pearl")
        {
            string cacheKey = $"pearl_{key}";
            if (Cache.TryGetValue(cacheKey, out Sprite cached)) return cached;

            const int size = 64;
            Texture2D texture = NewTexture(size, size);
            Color[] pixels = new Color[size * size];
            Vector2 center = new Vector2(31.5f, 31.5f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), center) / 31.5f;
                    if (d > 1f)
                    {
                        pixels[y * size + x] = Color.clear;
                        continue;
                    }
                    float glow = Mathf.Clamp01(1f - Vector2.Distance(new Vector2(x, y), center + new Vector2(-10f, 11f)) / 26f);
                    Color color = Color.Lerp(new Color(0.50f, 0.42f, 0.29f), Color.white, Mathf.Clamp01(1f - d * 0.72f));
                    color = Color.Lerp(color, new Color(0.80f, 0.95f, 1f), glow * 0.46f);
                    color.a = Mathf.Clamp01((1f - d) * 12f);
                    pixels[y * size + x] = color;
                }
            }
            texture.SetPixels(pixels);
            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
            sprite.name = cacheKey;
            Cache[cacheKey] = sprite;
            return sprite;
        }

        public static Sprite Rounded(string key, Color color, int radius = 14)
        {
            string cacheKey = $"rounded_{key}_{ColorUtility.ToHtmlStringRGBA(color)}_{radius}";
            if (Cache.TryGetValue(cacheKey, out Sprite cached)) return cached;

            const int size = 64;
            Texture2D texture = NewTexture(size, size);
            Color[] pixels = new Color[size * size];
            float r = Mathf.Clamp(radius, 1, 30);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(Mathf.Max(r - x, x - (size - 1 - r)), 0f);
                    float dy = Mathf.Max(Mathf.Max(r - y, y - (size - 1 - r)), 0f);
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);
                    Color pixel = color;
                    pixel.a *= Mathf.Clamp01(r + 1f - distance);
                    pixels[y * size + x] = pixel;
                }
            }
            texture.SetPixels(pixels);
            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0,
                SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
            sprite.name = cacheKey;
            Cache[cacheKey] = sprite;
            return sprite;
        }

        public static Sprite Background()
        {
            Sprite authored = LumaBayArtPack.Background;
            if (authored != null) return authored;

            const string key = "fallback_background";
            if (Cache.TryGetValue(key, out Sprite cached)) return cached;
            const int width = 180;
            const int height = 320;
            Texture2D texture = NewTexture(width, height);
            Color[] pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                float t = y / (height - 1f);
                Color row = Color.Lerp(new Color(0.015f, 0.07f, 0.13f), new Color(0.33f, 0.53f, 0.70f), t);
                for (int x = 0; x < width; x++) pixels[y * width + x] = row;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
            sprite.name = key;
            Cache[key] = sprite;
            return sprite;
        }

        private static Sprite CreateFallbackFrame(string key, Color fill, bool elevated)
        {
            string cacheKey = $"frame_{key}_{ColorUtility.ToHtmlStringRGBA(fill)}_{elevated}";
            if (Cache.TryGetValue(cacheKey, out Sprite cached)) return cached;

            const int size = 128;
            const float radius = 22f;
            Texture2D texture = NewTexture(size, size);
            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(Mathf.Max(radius - x, x - (size - 1 - radius)), 0f);
                    float dy = Mathf.Max(Mathf.Max(radius - y, y - (size - 1 - radius)), 0f);
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01(radius + 1f - d);
                    float edge = Mathf.Min(Mathf.Min(x, size - 1 - x), Mathf.Min(y, size - 1 - y));
                    Color color = edge < 3.5f
                        ? new Color(0.92f, 0.67f, 0.28f, 1f)
                        : Color.Lerp(fill * 0.76f, fill * 1.08f, y / (size - 1f));
                    color.a *= alpha;
                    pixels[y * size + x] = color;
                }
            }
            texture.SetPixels(pixels);
            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0,
                SpriteMeshType.FullRect, new Vector4(26, 26, 26, 26));
            sprite.name = cacheKey;
            Cache[cacheKey] = sprite;
            return sprite;
        }

        private static Texture2D NewTexture(int width, int height)
        {
            return new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
        }
    }
}
