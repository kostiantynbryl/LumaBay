using System.Collections.Generic;
using UnityEngine;

namespace LumaBay
{
    public static class CoastalBackdropArt
    {
        private static Sprite cached;

        public static Sprite Create()
        {
            if (cached != null) return cached;

            const int width = 360;
            const int height = 640;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            Color[] pixels = new Color[width * height];

            Color skyTop = new Color(0.015f, 0.075f, 0.16f);
            Color skyMid = new Color(0.08f, 0.32f, 0.54f);
            Color sunset = new Color(0.96f, 0.47f, 0.24f);
            Color horizon = new Color(0.94f, 0.66f, 0.40f);

            for (int y = 0; y < height; y++)
            {
                float t = y / (height - 1f);
                Color row;
                if (t > 0.48f)
                {
                    float skyT = (t - 0.48f) / 0.52f;
                    row = Color.Lerp(Color.Lerp(horizon, skyMid, skyT), skyTop, skyT * skyT * 0.72f);
                }
                else
                {
                    float seaT = t / 0.48f;
                    row = Color.Lerp(new Color(0.008f, 0.045f, 0.085f), new Color(0.025f, 0.24f, 0.34f), seaT);
                }

                for (int x = 0; x < width; x++)
                {
                    float noise = Mathf.Sin(x * 0.11f + y * 0.031f) * 0.007f + Mathf.Sin(x * 0.025f - y * 0.047f) * 0.006f;
                    pixels[y * width + x] = row + new Color(noise, noise, noise, 0f);
                }
            }

            DrawRadial(pixels, width, height, 278, 390, 122, new Color(1f, 0.48f, 0.17f, 0.32f));
            DrawRadial(pixels, width, height, 290, 495, 82, new Color(1f, 0.78f, 0.30f, 0.22f));

            DrawCloud(pixels, width, height, 72, 430, 72, 25, new Color(0.94f, 0.72f, 0.54f, 0.42f));
            DrawCloud(pixels, width, height, 175, 500, 80, 24, new Color(0.79f, 0.84f, 0.88f, 0.28f));
            DrawCloud(pixels, width, height, 252, 414, 82, 28, new Color(1f, 0.70f, 0.40f, 0.34f));

            DrawSea(pixels, width, height);
            DrawCoast(pixels, width, height);
            DrawLighthouse(pixels, width, height);
            DrawBird(pixels, width, height, 188, 515, 7);
            DrawBird(pixels, width, height, 218, 470, 5);
            DrawBird(pixels, width, height, 142, 542, 4);

            texture.SetPixels(pixels);
            texture.Apply();
            cached = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
            cached.name = "LumaBayCoastalBackdrop";
            return cached;
        }

        private static void DrawSea(Color[] pixels, int width, int height)
        {
            for (int y = 44; y < 306; y++)
            {
                float depth = y / 306f;
                for (int x = 0; x < width; x++)
                {
                    float wave = Mathf.Sin(x * 0.13f + y * 0.08f) + Mathf.Sin(x * 0.047f - y * 0.12f);
                    if (wave > 1.30f)
                    {
                        Blend(pixels, width, height, x, y, new Color(0.28f, 0.72f, 0.80f, 0.10f + depth * 0.10f));
                    }
                }
            }

            for (int i = 0; i < 28; i++)
            {
                int y = 70 + i * 8;
                int start = (i * 37) % width;
                int length = 24 + (i * 13) % 70;
                DrawLine(pixels, width, height, start, y, Mathf.Min(width - 1, start + length), y + (i % 3 - 1),
                    new Color(0.50f, 0.88f, 0.92f, 0.10f + i * 0.002f), 1);
            }
        }

        private static void DrawCoast(Color[] pixels, int width, int height)
        {
            DrawEllipse(pixels, width, height, 78, 38, 112, 55, new Color(0.018f, 0.055f, 0.060f, 1f));
            DrawEllipse(pixels, width, height, 286, 46, 132, 62, new Color(0.025f, 0.064f, 0.055f, 1f));
            DrawEllipse(pixels, width, height, 306, 140, 110, 66, new Color(0.035f, 0.085f, 0.068f, 1f));

            for (int i = 0; i < 18; i++)
            {
                int x = 8 + (i * 47) % 344;
                int y = 8 + (i * 19) % 52;
                int r = 8 + (i * 7) % 18;
                DrawEllipse(pixels, width, height, x, y, r, Mathf.Max(5, r / 2),
                    Color.Lerp(new Color(0.06f, 0.07f, 0.075f), new Color(0.17f, 0.13f, 0.09f), (i % 5) / 4f));
            }
        }

        private static void DrawLighthouse(Color[] pixels, int width, int height)
        {
            DrawRect(pixels, width, height, 284, 122, 353, 198, new Color(0.20f, 0.095f, 0.060f, 1f));
            DrawTriangle(pixels, width, height, new Vector2Int(278, 198), new Vector2Int(358, 198), new Vector2Int(318, 226),
                new Color(0.34f, 0.075f, 0.045f, 1f));

            for (int y = 176; y < 442; y++)
            {
                float t = (y - 176f) / 266f;
                int half = Mathf.RoundToInt(Mathf.Lerp(30f, 20f, t));
                int center = 306;
                for (int x = center - half; x <= center + half; x++)
                {
                    float shade = Mathf.InverseLerp(center + half, center - half, x);
                    Color tower = Color.Lerp(new Color(0.48f, 0.46f, 0.40f), new Color(0.91f, 0.87f, 0.72f), shade * 0.58f + 0.18f);
                    Blend(pixels, width, height, x, y, tower);
                }
            }

            DrawRect(pixels, width, height, 279, 420, 333, 458, new Color(0.12f, 0.15f, 0.16f, 1f));
            DrawRect(pixels, width, height, 284, 426, 328, 454, new Color(0.96f, 0.55f, 0.16f, 1f));
            DrawRadial(pixels, width, height, 306, 442, 74, new Color(1f, 0.74f, 0.25f, 0.42f));
            DrawTriangle(pixels, width, height, new Vector2Int(274, 458), new Vector2Int(338, 458), new Vector2Int(306, 490),
                new Color(0.42f, 0.055f, 0.035f, 1f));
            DrawRect(pixels, width, height, 273, 456, 339, 461, NauticalTheme.GoldDark);

            DrawWindow(pixels, width, height, 300, 362);
            DrawWindow(pixels, width, height, 300, 280);
            DrawWindow(pixels, width, height, 302, 204);

            for (int y = 186; y < 420; y += 42)
            {
                DrawLine(pixels, width, height, 283, y, 330, y, new Color(0.55f, 0.10f, 0.065f, 0.65f), 3);
            }
        }

        private static void DrawWindow(Color[] pixels, int width, int height, int x, int y)
        {
            DrawRect(pixels, width, height, x - 5, y - 8, x + 5, y + 8, new Color(0.07f, 0.12f, 0.15f, 1f));
            DrawRect(pixels, width, height, x - 3, y - 6, x + 3, y + 6, new Color(1f, 0.58f, 0.15f, 1f));
            DrawRadial(pixels, width, height, x, y, 18, new Color(1f, 0.60f, 0.18f, 0.18f));
        }

        private static void DrawBird(Color[] pixels, int width, int height, int x, int y, int size)
        {
            DrawLine(pixels, width, height, x - size, y, x, y + size / 2, new Color(0.04f, 0.07f, 0.09f, 0.78f), 1);
            DrawLine(pixels, width, height, x, y + size / 2, x + size, y, new Color(0.04f, 0.07f, 0.09f, 0.78f), 1);
        }

        private static void DrawCloud(Color[] pixels, int width, int height, int x, int y, int rx, int ry, Color color)
        {
            DrawEllipse(pixels, width, height, x, y, rx, ry, color);
            DrawEllipse(pixels, width, height, x - rx / 3, y + ry / 2, rx / 2, ry, color);
            DrawEllipse(pixels, width, height, x + rx / 4, y + ry / 3, rx / 2, ry, color);
        }

        private static void DrawRadial(Color[] pixels, int width, int height, int cx, int cy, int radius, Color color)
        {
            int minX = Mathf.Max(0, cx - radius);
            int maxX = Mathf.Min(width - 1, cx + radius);
            int minY = Mathf.Max(0, cy - radius);
            int maxY = Mathf.Min(height - 1, cy + radius);
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy)) / radius;
                    if (d > 1f) continue;
                    Color c = color;
                    c.a *= Mathf.Pow(1f - d, 2f);
                    Blend(pixels, width, height, x, y, c);
                }
            }
        }

        private static void DrawEllipse(Color[] pixels, int width, int height, int cx, int cy, int rx, int ry, Color color)
        {
            for (int y = Mathf.Max(0, cy - ry); y <= Mathf.Min(height - 1, cy + ry); y++)
            {
                for (int x = Mathf.Max(0, cx - rx); x <= Mathf.Min(width - 1, cx + rx); x++)
                {
                    float nx = (x - cx) / (float)Mathf.Max(1, rx);
                    float ny = (y - cy) / (float)Mathf.Max(1, ry);
                    if (nx * nx + ny * ny <= 1f) Blend(pixels, width, height, x, y, color);
                }
            }
        }

        private static void DrawRect(Color[] pixels, int width, int height, int minX, int minY, int maxX, int maxY, Color color)
        {
            for (int y = Mathf.Max(0, minY); y <= Mathf.Min(height - 1, maxY); y++)
            {
                for (int x = Mathf.Max(0, minX); x <= Mathf.Min(width - 1, maxX); x++) Blend(pixels, width, height, x, y, color);
            }
        }

        private static void DrawTriangle(Color[] pixels, int width, int height, Vector2Int a, Vector2Int b, Vector2Int c, Color color)
        {
            int minX = Mathf.Max(0, Mathf.Min(a.x, Mathf.Min(b.x, c.x)));
            int maxX = Mathf.Min(width - 1, Mathf.Max(a.x, Mathf.Max(b.x, c.x)));
            int minY = Mathf.Max(0, Mathf.Min(a.y, Mathf.Min(b.y, c.y)));
            int maxY = Mathf.Min(height - 1, Mathf.Max(a.y, Mathf.Max(b.y, c.y)));
            float area = Edge(a, b, c);
            if (Mathf.Abs(area) < 0.001f) return;

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Vector2Int p = new Vector2Int(x, y);
                    float w0 = Edge(b, c, p) / area;
                    float w1 = Edge(c, a, p) / area;
                    float w2 = Edge(a, b, p) / area;
                    if (w0 >= 0f && w1 >= 0f && w2 >= 0f) Blend(pixels, width, height, x, y, color);
                }
            }
        }

        private static float Edge(Vector2Int a, Vector2Int b, Vector2Int c)
        {
            return (c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x);
        }

        private static void DrawLine(Color[] pixels, int width, int height, int x0, int y0, int x1, int y1, Color color, int thickness)
        {
            int dx = Mathf.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Mathf.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;
            int error = dx + dy;
            while (true)
            {
                for (int ox = -thickness; ox <= thickness; ox++)
                {
                    for (int oy = -thickness; oy <= thickness; oy++) Blend(pixels, width, height, x0 + ox, y0 + oy, color);
                }
                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * error;
                if (e2 >= dy) { error += dy; x0 += sx; }
                if (e2 <= dx) { error += dx; y0 += sy; }
            }
        }

        private static void Blend(Color[] pixels, int width, int height, int x, int y, Color source)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return;
            int index = y * width + x;
            Color destination = pixels[index];
            float alpha = Mathf.Clamp01(source.a);
            pixels[index] = Color.Lerp(destination, new Color(source.r, source.g, source.b, 1f), alpha);
        }
    }
}
