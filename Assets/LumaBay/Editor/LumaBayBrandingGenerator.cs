using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace LumaBay.Editor
{
    public static class LumaBayBrandingGenerator
    {
        private const string GeneratedFolder = "Assets/LumaBay/Generated";
        private const string IconPath = GeneratedFolder + "/LumaBayAppIcon.png";

        public static void EnsureBranding()
        {
            Directory.CreateDirectory(GeneratedFolder);
            if (!File.Exists(IconPath))
            {
                Texture2D icon = GenerateIcon(512);
                File.WriteAllBytes(IconPath, icon.EncodeToPNG());
                Object.DestroyImmediate(icon);
                AssetDatabase.ImportAsset(IconPath, ImportAssetOptions.ForceUpdate);
            }

            TextureImporter importer = AssetImporter.GetAtPath(IconPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Default;
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.maxTextureSize = 512;
                importer.textureCompression = TextureImporterCompression.CompressedHQ;
                importer.SaveAndReimport();
            }

            Texture2D iconAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath);
            if (iconAsset == null) return;

            int[] sizes = PlayerSettings.GetIconSizes(NamedBuildTarget.Android, IconKind.Application);
            if (sizes != null && sizes.Length > 0)
            {
                Texture2D[] icons = sizes.Select(_ => iconAsset).ToArray();
                PlayerSettings.SetIcons(NamedBuildTarget.Android, icons, IconKind.Application);
            }
        }

        private static Texture2D GenerateIcon(int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            Color[] pixels = new Color[size * size];
            Vector2 center = new Vector2(size * 0.5f, size * 0.5f);

            for (int y = 0; y < size; y++)
            {
                float vertical = y / (size - 1f);
                Color top = new Color(0.015f, 0.055f, 0.13f, 1f);
                Color bottom = new Color(0.02f, 0.38f, 0.50f, 1f);
                Color row = Color.Lerp(bottom, top, vertical);
                for (int x = 0; x < size; x++)
                {
                    float radial = Vector2.Distance(new Vector2(x, y), center) / (size * 0.72f);
                    float vignette = Mathf.Clamp01(radial);
                    pixels[y * size + x] = Color.Lerp(row, row * 0.48f, vignette * vignette * 0.62f);
                }
            }

            DrawCircle(pixels, size, size, 256, 265, 211, new Color(0.015f, 0.115f, 0.22f, 0.96f));
            DrawRing(pixels, size, size, 256, 265, 217, 196, new Color(0.96f, 0.69f, 0.20f, 1f));
            DrawRing(pixels, size, size, 256, 265, 194, 187, new Color(1f, 0.91f, 0.58f, 0.85f));

            DrawRadial(pixels, size, size, 256, 359, 150, new Color(1f, 0.66f, 0.18f, 0.30f));
            DrawTriangle(pixels, size, size, new Vector2Int(84, 345), new Vector2Int(226, 330), new Vector2Int(229, 374),
                new Color(1f, 0.76f, 0.25f, 0.42f));
            DrawTriangle(pixels, size, size, new Vector2Int(428, 345), new Vector2Int(286, 330), new Vector2Int(283, 374),
                new Color(1f, 0.76f, 0.25f, 0.42f));

            for (int y = 123; y < 346; y++)
            {
                float t = (y - 123f) / 223f;
                int halfWidth = Mathf.RoundToInt(Mathf.Lerp(66f, 43f, t));
                for (int x = 256 - halfWidth; x <= 256 + halfWidth; x++)
                {
                    float shade = Mathf.InverseLerp(256 + halfWidth, 256 - halfWidth, x);
                    Color tower = Color.Lerp(new Color(0.51f, 0.47f, 0.39f), new Color(0.98f, 0.94f, 0.79f), shade * 0.64f + 0.18f);
                    Blend(pixels, size, size, x, y, tower);
                }
            }

            for (int y = 152; y < 322; y += 66)
            {
                DrawRect(pixels, size, size, 195, y, 317, y + 19, new Color(0.77f, 0.12f, 0.07f, 1f));
            }

            DrawRect(pixels, size, size, 180, 333, 332, 397, new Color(0.075f, 0.105f, 0.12f, 1f));
            DrawRect(pixels, size, size, 193, 344, 319, 388, new Color(1f, 0.61f, 0.15f, 1f));
            DrawRadial(pixels, size, size, 256, 365, 87, new Color(1f, 0.72f, 0.20f, 0.50f));
            DrawTriangle(pixels, size, size, new Vector2Int(171, 400), new Vector2Int(341, 400), new Vector2Int(256, 454),
                new Color(0.55f, 0.07f, 0.045f, 1f));
            DrawRect(pixels, size, size, 164, 394, 348, 405, new Color(0.96f, 0.69f, 0.20f, 1f));

            DrawWindow(pixels, size, 256, 278);
            DrawWindow(pixels, size, 256, 212);
            DrawWindow(pixels, size, 256, 146);

            DrawEllipse(pixels, size, size, 256, 104, 142, 35, new Color(0.035f, 0.105f, 0.105f, 1f));
            DrawEllipse(pixels, size, size, 256, 82, 185, 28, new Color(0.02f, 0.21f, 0.29f, 0.96f));
            DrawLine(pixels, size, size, 96, 83, 416, 83, new Color(0.22f, 0.72f, 0.76f, 0.48f), 3);

            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        private static void DrawWindow(Color[] pixels, int size, int x, int y)
        {
            DrawRect(pixels, size, size, x - 13, y - 20, x + 13, y + 20, new Color(0.045f, 0.09f, 0.12f, 1f));
            DrawRect(pixels, size, size, x - 8, y - 15, x + 8, y + 15, new Color(1f, 0.62f, 0.16f, 1f));
            DrawRadial(pixels, size, size, x, y, 35, new Color(1f, 0.62f, 0.16f, 0.20f));
        }

        private static void DrawRing(Color[] pixels, int width, int height, int cx, int cy, int outerRadius, int innerRadius, Color color)
        {
            for (int y = Mathf.Max(0, cy - outerRadius); y <= Mathf.Min(height - 1, cy + outerRadius); y++)
            {
                for (int x = Mathf.Max(0, cx - outerRadius); x <= Mathf.Min(width - 1, cx + outerRadius); x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    if (distance <= outerRadius && distance >= innerRadius) Blend(pixels, width, height, x, y, color);
                }
            }
        }

        private static void DrawCircle(Color[] pixels, int width, int height, int cx, int cy, int radius, Color color)
        {
            DrawEllipse(pixels, width, height, cx, cy, radius, radius, color);
        }

        private static void DrawRadial(Color[] pixels, int width, int height, int cx, int cy, int radius, Color color)
        {
            for (int y = Mathf.Max(0, cy - radius); y <= Mathf.Min(height - 1, cy + radius); y++)
            {
                for (int x = Mathf.Max(0, cx - radius); x <= Mathf.Min(width - 1, cx + radius); x++)
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
            float alpha = Mathf.Clamp01(source.a);
            pixels[index] = Color.Lerp(pixels[index], new Color(source.r, source.g, source.b, 1f), alpha);
        }
    }
}
