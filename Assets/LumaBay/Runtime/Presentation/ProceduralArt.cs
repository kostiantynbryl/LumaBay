using System;
using System.Collections.Generic;
using UnityEngine;

namespace LumaBay
{
    public static class ProceduralArt
    {
        private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();
        private static readonly Color[] PieceColors =
        {
            new Color(0.18f, 0.78f, 0.92f),
            new Color(1.00f, 0.43f, 0.38f),
            new Color(1.00f, 0.76f, 0.24f),
            new Color(0.29f, 0.84f, 0.60f),
            new Color(0.59f, 0.46f, 0.95f),
            new Color(0.98f, 0.48f, 0.72f)
        };

        public static Color Navy => new Color(0.035f, 0.09f, 0.16f);
        public static Color DeepSea => new Color(0.045f, 0.22f, 0.31f);
        public static Color Teal => new Color(0.08f, 0.58f, 0.62f);
        public static Color Gold => new Color(1.0f, 0.76f, 0.24f);
        public static Color Cream => new Color(0.96f, 0.94f, 0.86f);
        public static Color Coral => new Color(0.95f, 0.36f, 0.32f);

        public static Sprite Piece(PieceKind kind)
        {
            string key = $"piece_{(int)kind}";
            if (Cache.TryGetValue(key, out Sprite sprite)) return sprite;

            const int size = 128;
            var texture = NewTexture(size, size);
            Color baseColor = PieceColors[Mathf.Clamp((int)kind, 0, PieceColors.Length - 1)];
            var pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float nx = (x + 0.5f) / size * 2f - 1f;
                    float ny = (y + 0.5f) / size * 2f - 1f;
                    float shape = ShapeDistance((int)kind, nx, ny);
                    Color color = Color.clear;
                    if (shape <= 1f)
                    {
                        float highlight = Mathf.Clamp01((ny + 1f) * 0.22f + (1f - Mathf.Sqrt(nx * nx + ny * ny)) * 0.25f);
                        color = Color.Lerp(baseColor * 0.70f, Color.Lerp(baseColor, Color.white, 0.24f), highlight);
                        if (shape > 0.88f) color = Color.Lerp(Navy, baseColor, 0.35f);
                        if (nx < -0.25f && ny > 0.28f && shape < 0.72f) color = Color.Lerp(color, Color.white, 0.38f);
                    }
                    pixels[y * size + x] = color;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
            sprite.name = key;
            Cache[key] = sprite;
            return sprite;
        }

        public static Sprite Rounded(string key, Color color, int radius = 14)
        {
            string cacheKey = $"rounded_{key}_{ColorUtility.ToHtmlStringRGBA(color)}_{radius}";
            if (Cache.TryGetValue(cacheKey, out Sprite sprite)) return sprite;

            const int size = 64;
            var texture = NewTexture(size, size);
            var pixels = new Color[size * size];
            float r = Mathf.Clamp(radius, 1, 30);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(r - x, x - (size - 1 - r), 0f);
                    float dy = Mathf.Max(r - y, y - (size - 1 - r), 0f);
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = 1f - Mathf.Clamp01(distance - r + 1f);
                    Color pixel = color;
                    pixel.a *= alpha;
                    pixels[y * size + x] = pixel;
                }
            }
            texture.SetPixels(pixels);
            texture.Apply();
            sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0,
                SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
            sprite.name = cacheKey;
            Cache[cacheKey] = sprite;
            return sprite;
        }

        public static Sprite Background()
        {
            const string key = "background";
            if (Cache.TryGetValue(key, out Sprite sprite)) return sprite;

            const int width = 256;
            const int height = 512;
            var texture = NewTexture(width, height);
            var pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                float t = y / (height - 1f);
                Color row = Color.Lerp(Navy, new Color(0.03f, 0.36f, 0.43f), t);
                for (int x = 0; x < width; x++)
                {
                    float wave = Mathf.Sin(x * 0.08f + y * 0.035f) * 0.018f;
                    pixels[y * width + x] = row + new Color(wave, wave, wave, 0f);
                }
            }
            texture.SetPixels(pixels);
            texture.Apply();
            sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
            sprite.name = key;
            Cache[key] = sprite;
            return sprite;
        }

        private static Texture2D NewTexture(int width, int height)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            return texture;
        }

        private static float ShapeDistance(int shape, float x, float y)
        {
            x /= 0.82f;
            y /= 0.82f;
            switch (shape)
            {
                case 0:
                    return Mathf.Sqrt(x * x + y * y);
                case 1:
                {
                    float angle = Mathf.Atan2(y, x);
                    float radius = Mathf.Sqrt(x * x + y * y);
                    float star = 0.62f + 0.22f * Mathf.Cos(5f * angle);
                    return radius / Mathf.Max(0.2f, star);
                }
                case 2:
                {
                    float body = Mathf.Max(Mathf.Abs(x) * 0.90f, Mathf.Abs(y + 0.08f));
                    float roof = Mathf.Abs(x) + Mathf.Abs(y - 0.32f) * 1.25f;
                    return Mathf.Min(body, roof);
                }
                case 3:
                    return Mathf.Abs(x) + Mathf.Abs(y);
                case 4:
                {
                    float qx = Mathf.Abs(x);
                    float qy = Mathf.Abs(y);
                    return Mathf.Max(qx * 0.86f + qy * 0.50f, qy);
                }
                default:
                {
                    float angle = Mathf.Atan2(y, x);
                    float radius = Mathf.Sqrt(x * x + y * y);
                    float petal = 0.66f + 0.18f * Mathf.Cos(6f * angle);
                    return radius / Mathf.Max(0.2f, petal);
                }
            }
        }
    }
}
