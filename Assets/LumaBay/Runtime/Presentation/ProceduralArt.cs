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
            new Color(0.35f, 0.20f, 0.83f), // shell
            new Color(0.96f, 0.25f, 0.17f), // starfish
            new Color(0.95f, 0.60f, 0.12f), // lantern
            new Color(0.24f, 0.62f, 0.32f), // compass
            new Color(0.16f, 0.67f, 0.98f), // crystal
            new Color(0.96f, 0.26f, 0.56f)  // flower
        };

        public static Color Navy => NauticalTheme.Navy;
        public static Color DeepSea => NauticalTheme.Ocean;
        public static Color Teal => NauticalTheme.OceanBright;
        public static Color Gold => NauticalTheme.Gold;
        public static Color Cream => NauticalTheme.Pearl;
        public static Color Coral => NauticalTheme.Coral;

        public static Sprite Piece(PieceKind kind)
        {
            string key = $"piece_premium_{(int)kind}";
            if (Cache.TryGetValue(key, out Sprite sprite)) return sprite;

            const int size = 160;
            Texture2D texture = NewTexture(size, size);
            Color[] pixels = new Color[size * size];
            Color baseColor = PieceColors[Mathf.Clamp((int)kind, 0, PieceColors.Length - 1)];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float nx = (x + 0.5f) / size * 2f - 1f;
                    float ny = (y + 0.5f) / size * 2f - 1f;
                    float distance = ShapeDistance((int)kind, nx, ny);
                    Color color = Color.clear;

                    if (distance <= 1.04f)
                    {
                        float edge = Mathf.InverseLerp(1.04f, 0.78f, distance);
                        float radial = Mathf.Clamp01(1f - Mathf.Sqrt(nx * nx + ny * ny));
                        float topLight = Mathf.Clamp01((ny + 1f) * 0.44f);
                        float diagonal = Mathf.Clamp01(1f - Vector2.Distance(new Vector2(nx, ny), new Vector2(-0.32f, 0.38f)) * 1.15f);

                        if (distance > 0.91f)
                        {
                            color = Color.Lerp(NauticalTheme.GoldDark, NauticalTheme.GoldLight, edge);
                        }
                        else
                        {
                            Color shadow = Color.Lerp(baseColor * 0.46f, baseColor * 0.72f, radial);
                            Color lit = Color.Lerp(baseColor, Color.white, 0.34f);
                            color = Color.Lerp(shadow, lit, Mathf.Clamp01(topLight * 0.46f + diagonal * 0.62f));

                            float pattern = DecorativePattern((int)kind, nx, ny);
                            if (pattern > 0.72f)
                            {
                                color = Color.Lerp(color, Color.white, (pattern - 0.72f) * 0.52f);
                            }
                        }

                        float alpha = Mathf.Clamp01((1.05f - distance) * 18f);
                        color.a = alpha;
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

        public static Sprite OrnateFrame(string key, Color fill, bool elevated = false)
        {
            string cacheKey = $"ornate_{key}_{ColorUtility.ToHtmlStringRGBA(fill)}_{elevated}";
            if (Cache.TryGetValue(cacheKey, out Sprite sprite)) return sprite;

            const int size = 128;
            Texture2D texture = NewTexture(size, size);
            Color[] pixels = new Color[size * size];
            float radius = elevated ? 23f : 18f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(radius - x, x - (size - 1 - radius), 0f);
                    float dy = Mathf.Max(radius - y, y - (size - 1 - radius), 0f);
                    float corner = Mathf.Sqrt(dx * dx + dy * dy);
                    float outerAlpha = 1f - Mathf.Clamp01(corner - radius + 1f);
                    if (outerAlpha <= 0f)
                    {
                        pixels[y * size + x] = Color.clear;
                        continue;
                    }

                    float edgeDistance = Mathf.Min(Mathf.Min(x, size - 1 - x), Mathf.Min(y, size - 1 - y));
                    Color color;
                    if (edgeDistance < 4f)
                    {
                        color = NauticalTheme.GoldDark;
                    }
                    else if (edgeDistance < 8f)
                    {
                        float t = (edgeDistance - 4f) / 4f;
                        color = Color.Lerp(NauticalTheme.GoldLight, NauticalTheme.Gold, t);
                    }
                    else if (edgeDistance < 11f)
                    {
                        color = NauticalTheme.GoldDark * 0.78f;
                        color.a = 1f;
                    }
                    else
                    {
                        float vertical = y / (size - 1f);
                        float shimmer = Mathf.Sin((x + y) * 0.10f) * 0.025f;
                        color = Color.Lerp(fill * 0.78f, fill * 1.08f, vertical) + new Color(shimmer, shimmer, shimmer, 0f);
                    }

                    float pearlDistance = Mathf.Min(
                        Vector2.Distance(new Vector2(x, y), new Vector2(11f, 11f)),
                        Mathf.Min(
                            Vector2.Distance(new Vector2(x, y), new Vector2(size - 12f, 11f)),
                            Mathf.Min(
                                Vector2.Distance(new Vector2(x, y), new Vector2(11f, size - 12f)),
                                Vector2.Distance(new Vector2(x, y), new Vector2(size - 12f, size - 12f)))));
                    if (pearlDistance < 6f)
                    {
                        color = Color.Lerp(NauticalTheme.Gold, NauticalTheme.Pearl, 1f - pearlDistance / 6f);
                    }

                    color.a *= outerAlpha;
                    pixels[y * size + x] = color;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0,
                SpriteMeshType.FullRect, new Vector4(22f, 22f, 22f, 22f));
            sprite.name = cacheKey;
            Cache[cacheKey] = sprite;
            return sprite;
        }

        public static Sprite Pearl(string key = "pearl")
        {
            if (Cache.TryGetValue(key, out Sprite sprite)) return sprite;
            const int size = 64;
            Texture2D texture = NewTexture(size, size);
            Color[] pixels = new Color[size * size];
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), center) / (size * 0.5f);
                    if (d > 1f)
                    {
                        pixels[y * size + x] = Color.clear;
                        continue;
                    }
                    float highlight = Mathf.Clamp01(1f - Vector2.Distance(new Vector2(x, y), center + new Vector2(-11f, 12f)) / 25f);
                    Color c = Color.Lerp(new Color(0.47f, 0.37f, 0.22f), NauticalTheme.Pearl, 1f - d * 0.68f);
                    c = Color.Lerp(c, Color.white, highlight * 0.72f);
                    c.a = Mathf.Clamp01((1f - d) * 10f);
                    pixels[y * size + x] = c;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
            Cache[key] = sprite;
            return sprite;
        }

        public static Sprite Rounded(string key, Color color, int radius = 14)
        {
            string cacheKey = $"rounded_{key}_{ColorUtility.ToHtmlStringRGBA(color)}_{radius}";
            if (Cache.TryGetValue(cacheKey, out Sprite sprite)) return sprite;

            const int size = 64;
            Texture2D texture = NewTexture(size, size);
            Color[] pixels = new Color[size * size];
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
            const string key = "background_sunset_v2";
            if (Cache.TryGetValue(key, out Sprite sprite)) return sprite;

            const int width = 320;
            const int height = 640;
            Texture2D texture = NewTexture(width, height);
            Color[] pixels = new Color[width * height];
            Color horizon = new Color(0.28f, 0.39f, 0.52f);
            Color sunset = new Color(0.93f, 0.43f, 0.22f);

            for (int y = 0; y < height; y++)
            {
                float t = y / (height - 1f);
                Color row;
                if (t < 0.42f)
                {
                    row = Color.Lerp(NauticalTheme.Midnight, NauticalTheme.Navy, t / 0.42f);
                }
                else
                {
                    float sky = (t - 0.42f) / 0.58f;
                    row = Color.Lerp(horizon, Color.Lerp(sunset, new Color(0.24f, 0.49f, 0.68f), sky), sky);
                }

                for (int x = 0; x < width; x++)
                {
                    float wave = Mathf.Sin(x * 0.065f + y * 0.022f) * 0.018f;
                    float glow = Mathf.Exp(-Mathf.Pow((x / (float)width - 0.77f) * 4.2f, 2f)) * Mathf.Clamp01((t - 0.45f) * 2.4f) * 0.17f;
                    pixels[y * width + x] = row + new Color(wave + glow, wave + glow * 0.58f, wave, 0f);
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
            return new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
        }

        private static float DecorativePattern(int shape, float x, float y)
        {
            switch (shape)
            {
                case 0:
                {
                    float angle = Mathf.Atan2(y + 0.12f, x);
                    return Mathf.Abs(Mathf.Sin(angle * 5f)) * Mathf.Clamp01(1f - Mathf.Sqrt(x * x + y * y));
                }
                case 1:
                    return Mathf.Clamp01(1f - Mathf.Sqrt(x * x + y * y) * 2.2f);
                case 2:
                    return Mathf.Clamp01(1f - Mathf.Abs(x) * 6f) * Mathf.Clamp01((y + 0.5f) * 1.2f);
                case 3:
                {
                    float ring = Mathf.Abs(Mathf.Sqrt(x * x + y * y) - 0.38f);
                    return Mathf.Clamp01(1f - ring * 12f);
                }
                case 4:
                    return Mathf.Clamp01(1f - Mathf.Abs(x + y * 0.32f) * 5f);
                default:
                {
                    float angle = Mathf.Atan2(y, x);
                    return Mathf.Abs(Mathf.Cos(angle * 6f)) * Mathf.Clamp01(1f - Mathf.Sqrt(x * x + y * y));
                }
            }
        }

        private static float ShapeDistance(int shape, float x, float y)
        {
            x /= 0.80f;
            y /= 0.80f;
            switch (shape)
            {
                case 0: // shell
                {
                    float body = Mathf.Sqrt(x * x + Mathf.Pow((y + 0.10f) * 1.12f, 2f));
                    float cut = Mathf.Clamp01((-y + 0.55f) * 2.0f);
                    return body / Mathf.Lerp(0.78f, 1f, cut);
                }
                case 1: // starfish
                {
                    float angle = Mathf.Atan2(y, x);
                    float radius = Mathf.Sqrt(x * x + y * y);
                    float star = 0.60f + 0.25f * Mathf.Cos(5f * angle);
                    return radius / Mathf.Max(0.18f, star);
                }
                case 2: // lantern
                {
                    float body = Mathf.Max(Mathf.Abs(x) * 0.92f, Mathf.Abs(y + 0.03f));
                    float cap = Mathf.Abs(x) * 1.20f + Mathf.Abs(y - 0.66f) * 2.1f;
                    return Mathf.Min(body, cap);
                }
                case 3: // compass
                    return Mathf.Sqrt(x * x + y * y);
                case 4: // crystal
                {
                    float qx = Mathf.Abs(x);
                    float qy = Mathf.Abs(y);
                    return Mathf.Max(qx * 0.82f + qy * 0.46f, qy);
                }
                default: // flower
                {
                    float angle = Mathf.Atan2(y, x);
                    float radius = Mathf.Sqrt(x * x + y * y);
                    float petal = 0.61f + 0.22f * Mathf.Cos(6f * angle);
                    return radius / Mathf.Max(0.20f, petal);
                }
            }
        }
    }
}
