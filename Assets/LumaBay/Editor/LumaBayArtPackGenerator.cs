using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LumaBay.Editor
{
    [InitializeOnLoad]
    public static class LumaBayArtPackGenerator
    {
        private const string Root = "Assets/LumaBay/Resources/ArtPack";
        private const string VersionKey = "LumaBay.ArtPack.0.1.2.v2";

        static LumaBayArtPackGenerator()
        {
            EditorApplication.delayCall += GenerateIfNeeded;
        }

        [MenuItem("Luma Bay/Generate Premium Art Pack", priority = 3)]
        public static void GenerateAll()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;

            Directory.CreateDirectory($"{Root}/backgrounds");
            Directory.CreateDirectory($"{Root}/ui");
            Directory.CreateDirectory($"{Root}/pieces");
            Directory.CreateDirectory($"{Root}/boosters");
            Directory.CreateDirectory($"{Root}/lighthouse");

            GenerateBackground();
            GenerateUi();
            GeneratePieces();
            GenerateBoosters();
            GenerateLighthouseStates();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorPrefs.SetBool(VersionKey, true);
            Debug.Log("Luma Bay premium PNG art pack generated successfully.");
        }

        private static void GenerateIfNeeded()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            string sentinel = $"{Root}/lighthouse/lighthouse_31.png";
            if (!EditorPrefs.GetBool(VersionKey, false) || !File.Exists(sentinel)) GenerateAll();
        }

        private static void GenerateBackground()
        {
            var r = new Raster(720, 1280, new Color(0.02f, 0.05f, 0.12f));
            r.VerticalGradient(new Color(0.015f, 0.06f, 0.15f), new Color(0.24f, 0.49f, 0.72f), 350, 1279);
            r.VerticalGradient(new Color(0.04f, 0.18f, 0.31f), new Color(0.02f, 0.07f, 0.13f), 0, 390);
            r.Glow(565, 790, 220, new Color(1f, 0.55f, 0.22f, 0.52f));
            r.Ellipse(565, 790, 62, 62, new Color(1f, 0.84f, 0.42f));
            DrawCloud(r, 130, 925, 1.15f, new Color(0.88f, 0.92f, 1f, 0.24f));
            DrawCloud(r, 470, 1020, 0.82f, new Color(1f, 0.75f, 0.63f, 0.20f));
            DrawCloud(r, 325, 845, 0.68f, new Color(0.92f, 0.84f, 0.88f, 0.13f));

            for (int i = 0; i < 17; i++)
            {
                float y = 24f + i * 20f;
                Color wave = Color.Lerp(new Color(0.08f, 0.34f, 0.50f, 0.58f), new Color(0.24f, 0.68f, 0.74f, 0.42f), i / 16f);
                r.Wave(0, y, 720, 8f + i * 0.25f, 72f + i * 2f, i * 0.56f, 3f, wave);
            }

            for (int i = 0; i < 14; i++)
            {
                float width = Mathf.Lerp(190f, 30f, i / 13f);
                r.RoundRect(565 - width * 0.5f, 245 + i * 18, width, 5, 2,
                    new Color(1f, 0.63f, 0.27f, 0.11f + i * 0.008f));
            }

            r.Polygon(new[]
            {
                new Vector2(0, 0), new Vector2(0, 450), new Vector2(95, 420), new Vector2(170, 335),
                new Vector2(238, 250), new Vector2(273, 150), new Vector2(250, 0)
            }, new Color(0.025f, 0.10f, 0.12f));
            r.Polygon(new[]
            {
                new Vector2(720, 0), new Vector2(720, 320), new Vector2(665, 297), new Vector2(623, 235),
                new Vector2(592, 150), new Vector2(610, 0)
            }, new Color(0.04f, 0.13f, 0.14f));
            r.Line(22, 120, 185, 340, 7, new Color(0.18f, 0.28f, 0.23f, 0.62f));
            r.Line(655, 72, 620, 240, 5, new Color(0.19f, 0.29f, 0.24f, 0.58f));
            r.Noise(3187, 0.016f);
            Save($"{Root}/backgrounds/coastal_sunset.png", r, Vector4.zero, false, 2048);
        }

        private static void GenerateUi()
        {
            var panel = new Raster(256, 256, Color.clear);
            panel.RoundRectGradient(4, 4, 248, 248, 34,
                new Color(0.025f, 0.11f, 0.20f, 0.96f), new Color(0.09f, 0.29f, 0.42f, 0.94f));
            panel.Border(4, 4, 248, 248, 34, 3, new Color(0.96f, 0.74f, 0.31f, 0.88f));
            panel.Border(11, 11, 234, 234, 28, 1, new Color(0.70f, 0.92f, 1f, 0.30f));
            panel.Glow(82, 196, 86, new Color(0.36f, 0.76f, 0.95f, 0.13f));
            Save($"{Root}/ui/panel_glass.png", panel, new Vector4(44, 44, 44, 44), true);

            MakeButton("button_primary", new Color(0.02f, 0.30f, 0.43f), new Color(0.09f, 0.67f, 0.68f), 512, 190);
            MakeButton("button_secondary", new Color(0.02f, 0.10f, 0.20f), new Color(0.10f, 0.31f, 0.47f), 512, 190);
            MakeButton("booster_card", new Color(0.025f, 0.10f, 0.21f), new Color(0.14f, 0.33f, 0.52f), 420, 260);
            MakeButton("goal_chip", new Color(0.025f, 0.12f, 0.22f), new Color(0.11f, 0.38f, 0.51f), 420, 150);

            var track = new Raster(512, 64, Color.clear);
            track.RoundRectGradient(2, 2, 508, 60, 29, new Color(0.01f, 0.04f, 0.08f), new Color(0.03f, 0.13f, 0.20f));
            track.Border(2, 2, 508, 60, 29, 2, new Color(0.68f, 0.86f, 0.92f, 0.42f));
            Save($"{Root}/ui/progress_track.png", track, new Vector4(31, 31, 31, 31), true);

            var fill = new Raster(512, 64, Color.clear);
            fill.RoundRectHorizontalGradient(2, 2, 508, 60, 29,
                new Color(0.05f, 0.64f, 0.70f), new Color(1f, 0.75f, 0.25f));
            fill.Border(2, 2, 508, 60, 29, 2, new Color(1f, 0.91f, 0.58f, 0.82f));
            fill.Glow(420, 32, 85, new Color(1f, 0.90f, 0.46f, 0.25f));
            Save($"{Root}/ui/progress_fill.png", fill, new Vector4(31, 31, 31, 31), true);
        }

        private static void MakeButton(string name, Color bottom, Color top, int width, int height)
        {
            var r = new Raster(width, height, Color.clear);
            r.RoundRectGradient(4, 4, width - 8, height - 8, height * 0.30f, bottom, top);
            r.Border(4, 4, width - 8, height - 8, height * 0.30f, 4, new Color(0.96f, 0.76f, 0.34f, 0.92f));
            r.Border(11, 11, width - 22, height - 22, height * 0.24f, 2, new Color(0.75f, 0.95f, 1f, 0.27f));
            r.Glow(width * 0.28f, height * 0.78f, width * 0.22f, new Color(1f, 1f, 1f, 0.12f));
            float border = height * 0.34f;
            Save($"{Root}/ui/{name}.png", r, new Vector4(border, border, border, border), true);
        }

        private static void GeneratePieces()
        {
            MakePiece(PieceKind.Shell, new Color(0.56f, 0.30f, 0.95f));
            MakePiece(PieceKind.Starfish, new Color(0.98f, 0.29f, 0.17f));
            MakePiece(PieceKind.Lantern, new Color(1f, 0.66f, 0.14f));
            MakePiece(PieceKind.Compass, new Color(0.20f, 0.72f, 0.36f));
            MakePiece(PieceKind.Crystal, new Color(0.12f, 0.68f, 1f));
            MakePiece(PieceKind.Flower, new Color(0.98f, 0.29f, 0.61f));
        }

        private static void MakePiece(PieceKind kind, Color color)
        {
            var r = new Raster(320, 320, Color.clear);
            r.Glow(160, 145, 128, WithAlpha(color, 0.30f));
            r.Ellipse(168, 126, 110, 88, new Color(0f, 0f, 0f, 0.28f));

            switch (kind)
            {
                case PieceKind.Shell: DrawShell(r, color); break;
                case PieceKind.Starfish: DrawStar(r, color); break;
                case PieceKind.Lantern: DrawLantern(r, color); break;
                case PieceKind.Compass: DrawCompass(r, color); break;
                case PieceKind.Crystal: DrawCrystal(r, color); break;
                case PieceKind.Flower: DrawFlower(r, color); break;
            }

            Save($"{Root}/pieces/piece_{kind.ToString().ToLowerInvariant()}.png", r, Vector4.zero, false);
        }

        private static void DrawShell(Raster r, Color c)
        {
            r.EllipseGradient(160, 158, 112, 96, Dark(c, 0.52f), Light(c, 0.32f));
            r.Polygon(new[] { new Vector2(64, 150), new Vector2(256, 150), new Vector2(224, 77), new Vector2(96, 77) }, Dark(c, 0.68f));
            for (int i = -4; i <= 4; i++) r.Line(160, 237, 160 + i * 18, 90, 4, new Color(1f, 0.90f, 1f, 0.32f));
            r.Ring(160, 158, 112, 96, 5, new Color(1f, 0.82f, 0.37f, 0.88f));
            r.Glow(126, 204, 45, new Color(1f, 1f, 1f, 0.20f));
        }

        private static void DrawStar(Raster r, Color c)
        {
            Vector2[] outer = StarPoints(160, 158, 116, 47, 5, -Mathf.PI * 0.5f);
            Vector2[] inner = StarPoints(157, 166, 101, 42, 5, -Mathf.PI * 0.5f);
            r.Polygon(outer, Dark(c, 0.48f));
            r.Polygon(inner, Light(c, 0.14f));
            r.Polyline(outer, 5, new Color(1f, 0.78f, 0.31f, 0.92f), true);
            for (int i = 0; i < 24; i++)
            {
                float angle = i * 2.39996f;
                float radius = 18 + (i % 6) * 12;
                r.Ellipse(160 + Mathf.Cos(angle) * radius, 160 + Mathf.Sin(angle) * radius, 4, 4,
                    new Color(1f, 0.78f, 0.44f, 0.43f));
            }
        }

        private static void DrawLantern(Raster r, Color c)
        {
            r.RoundRectGradient(84, 66, 152, 180, 34, Dark(c, 0.48f), Light(c, 0.28f));
            r.Border(84, 66, 152, 180, 34, 6, new Color(0.39f, 0.22f, 0.07f));
            r.Line(110, 246, 126, 282, 10, new Color(0.44f, 0.26f, 0.08f));
            r.Line(210, 246, 194, 282, 10, new Color(0.44f, 0.26f, 0.08f));
            r.Line(126, 282, 194, 282, 10, new Color(0.44f, 0.26f, 0.08f));
            r.Glow(160, 150, 74, new Color(1f, 0.72f, 0.16f, 0.48f));
            r.Polygon(new[] { new Vector2(160, 92), new Vector2(128, 151), new Vector2(153, 207), new Vector2(190, 151) },
                new Color(1f, 0.91f, 0.40f, 0.95f));
            r.Polygon(new[] { new Vector2(160, 113), new Vector2(145, 154), new Vector2(160, 190), new Vector2(178, 151) },
                new Color(1f, 1f, 0.82f, 0.96f));
        }

        private static void DrawCompass(Raster r, Color c)
        {
            r.EllipseGradient(160, 160, 112, 112, Dark(c, 0.60f), Light(c, 0.26f));
            r.Ring(160, 160, 112, 112, 8, new Color(1f, 0.77f, 0.30f, 0.96f));
            r.Ring(160, 160, 84, 84, 3, new Color(0.84f, 1f, 0.90f, 0.52f));
            r.Polygon(new[] { new Vector2(160, 61), new Vector2(181, 160), new Vector2(160, 144), new Vector2(139, 160) }, new Color(1f, 0.34f, 0.20f));
            r.Polygon(new[] { new Vector2(160, 259), new Vector2(139, 160), new Vector2(160, 176), new Vector2(181, 160) }, new Color(0.82f, 0.96f, 1f));
            r.Ellipse(160, 160, 16, 16, new Color(1f, 0.84f, 0.38f));
        }

        private static void DrawCrystal(Raster r, Color c)
        {
            Vector2[] outer =
            {
                new Vector2(160, 284), new Vector2(64, 178), new Vector2(92, 72),
                new Vector2(160, 36), new Vector2(232, 74), new Vector2(256, 178)
            };
            r.Polygon(outer, Dark(c, 0.52f));
            r.Polygon(new[] { new Vector2(160, 274), new Vector2(86, 176), new Vector2(112, 84), new Vector2(160, 52) }, Light(c, 0.25f));
            r.Polygon(new[] { new Vector2(160, 274), new Vector2(160, 52), new Vector2(216, 87), new Vector2(235, 176) }, Dark(c, 0.16f));
            r.Polygon(new[] { new Vector2(160, 52), new Vector2(112, 84), new Vector2(160, 120), new Vector2(216, 87) }, new Color(0.80f, 0.96f, 1f, 0.76f));
            r.Polyline(outer, 6, new Color(1f, 0.82f, 0.34f, 0.92f), true);
            r.Line(128, 90, 102, 166, 7, new Color(1f, 1f, 1f, 0.38f));
        }

        private static void DrawFlower(Raster r, Color c)
        {
            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI / 4f;
                r.RotatedEllipse(160 + Mathf.Cos(angle) * 64, 160 + Mathf.Sin(angle) * 64, 58, 35, angle,
                    Color.Lerp(Dark(c, 0.14f), Light(c, 0.28f), i / 7f));
            }
            r.EllipseGradient(160, 160, 55, 55, new Color(0.64f, 0.27f, 0.05f), new Color(1f, 0.84f, 0.24f));
            r.Ring(160, 160, 55, 55, 4, new Color(1f, 0.85f, 0.42f, 0.92f));
        }

        private static void GenerateBoosters()
        {
            MakeBooster("lightning", new Color(0.20f, 0.60f, 1f), DrawLightning);
            MakeBooster("anchor", new Color(0.55f, 0.31f, 0.92f), DrawAnchor);
            MakeBooster("shuffle", new Color(0.10f, 0.72f, 0.72f), DrawShuffle);
            MakeBooster("extra_moves", new Color(0.27f, 0.76f, 0.38f), DrawHourglass);
            MakeBooster("harpoon", new Color(0.95f, 0.30f, 0.22f), DrawHarpoon);
        }

        private static void MakeBooster(string name, Color accent, Action<Raster, Color> draw)
        {
            var r = new Raster(320, 320, Color.clear);
            r.Glow(160, 160, 132, WithAlpha(accent, 0.42f));
            r.EllipseGradient(160, 160, 118, 118, new Color(0.02f, 0.10f, 0.20f), new Color(0.12f, 0.34f, 0.53f));
            r.Ring(160, 160, 118, 118, 7, new Color(1f, 0.80f, 0.34f, 0.96f));
            draw(r, accent);
            Save($"{Root}/boosters/{name}.png", r, Vector4.zero, false);
        }

        private static void DrawLightning(Raster r, Color c)
        {
            Vector2[] points =
            {
                new Vector2(178, 54), new Vector2(98, 171), new Vector2(151, 165),
                new Vector2(126, 270), new Vector2(228, 134), new Vector2(174, 141)
            };
            r.Polygon(points, Light(c, 0.34f));
            r.Polyline(points, 5, new Color(1f, 0.88f, 0.38f), true);
        }

        private static void DrawAnchor(Raster r, Color c)
        {
            Color metal = Light(c, 0.34f);
            r.Ring(160, 92, 30, 30, 10, metal);
            r.Line(160, 116, 160, 236, 18, metal);
            r.Line(100, 154, 220, 154, 15, metal);
            r.Line(160, 236, 102, 210, 18, metal);
            r.Line(160, 236, 218, 210, 18, metal);
            r.Line(102, 210, 88, 181, 15, metal);
            r.Line(218, 210, 232, 181, 15, metal);
        }

        private static void DrawShuffle(Raster r, Color c)
        {
            Color arrow = Light(c, 0.34f);
            r.Arc(160, 160, 78, 0.25f, 2.75f, 15, arrow);
            r.Arc(160, 160, 78, 3.40f, 5.95f, 15, arrow);
            r.Polygon(new[] { new Vector2(225, 104), new Vector2(260, 111), new Vector2(237, 139) }, arrow);
            r.Polygon(new[] { new Vector2(95, 216), new Vector2(60, 208), new Vector2(83, 180) }, arrow);
        }

        private static void DrawHourglass(Raster r, Color c)
        {
            r.Line(104, 72, 216, 72, 14, new Color(1f, 0.78f, 0.31f));
            r.Line(104, 248, 216, 248, 14, new Color(1f, 0.78f, 0.31f));
            r.Polygon(new[] { new Vector2(114, 86), new Vector2(206, 86), new Vector2(177, 154), new Vector2(206, 234), new Vector2(114, 234), new Vector2(143, 154) }, new Color(0.74f, 0.94f, 1f, 0.72f));
            r.Polygon(new[] { new Vector2(132, 211), new Vector2(188, 211), new Vector2(160, 165) }, Light(c, 0.30f));
            r.Polygon(new[] { new Vector2(135, 102), new Vector2(185, 102), new Vector2(160, 143) }, Light(c, 0.30f));
        }

        private static void DrawHarpoon(Raster r, Color c)
        {
            r.Line(92, 235, 218, 109, 18, Light(c, 0.30f));
            r.Polygon(new[] { new Vector2(221, 105), new Vector2(250, 64), new Vector2(211, 82), new Vector2(196, 54) }, new Color(1f, 0.86f, 0.42f));
            r.Line(105, 222, 79, 196, 14, new Color(0.50f, 0.24f, 0.09f));
            r.Line(105, 222, 131, 248, 14, new Color(0.50f, 0.24f, 0.09f));
        }

        private static void GenerateLighthouseStates()
        {
            for (int state = 0; state < 32; state++)
            {
                Save($"{Root}/lighthouse/lighthouse_{state:00}.png", DrawLighthouse(state), Vector4.zero, false, 2048);
            }
        }

        private static Raster DrawLighthouse(int state)
        {
            var r = new Raster(768, 768, new Color(0.08f, 0.18f, 0.37f));
            float progress = state / 31f;
            r.VerticalGradient(new Color(0.08f, 0.20f, 0.40f), new Color(0.96f, 0.49f, 0.29f), 210, 767);
            r.VerticalGradient(new Color(0.025f, 0.18f, 0.30f), new Color(0.02f, 0.08f, 0.15f), 0, 220);
            r.Glow(606, 565, 190, new Color(1f, 0.66f, 0.28f, 0.44f));
            r.Ellipse(606, 565, 42, 42, new Color(1f, 0.84f, 0.45f));
            DrawCloud(r, 155, 607, 0.82f, new Color(0.88f, 0.92f, 1f, 0.23f));
            DrawCloud(r, 475, 655, 0.58f, new Color(1f, 0.78f, 0.68f, 0.19f));
            for (int i = 0; i < 10; i++) r.Wave(0, 24 + i * 18, 768, 7 + i * 0.35f, 76 + i * 3, i * 0.55f, 3, new Color(0.15f, 0.55f, 0.67f, 0.40f));

            Vector2[] island =
            {
                new Vector2(92, 0), new Vector2(112, 112), new Vector2(172, 191), new Vector2(251, 236),
                new Vector2(360, 251), new Vector2(470, 230), new Vector2(563, 178), new Vector2(623, 95), new Vector2(646, 0)
            };
            r.Polygon(island, new Color(0.06f, 0.15f, 0.14f));
            r.Polyline(new[] { island[1], island[2], island[3], island[4], island[5], island[6], island[7] }, 10, new Color(0.18f, 0.28f, 0.22f), false);

            if (state >= 6)
            {
                Color grass = Color.Lerp(new Color(0.12f, 0.35f, 0.22f), new Color(0.36f, 0.62f, 0.28f), progress);
                for (int i = 0; i < 70 + state * 3; i++)
                {
                    float x = 145 + (i * 53 % 450);
                    float y = 78 + (i * 29 % 130);
                    r.Line(x, y, x + Mathf.Sin(i) * 4, y + 10 + i % 9, 2, grass);
                }
            }

            Vector2[] tower = { new Vector2(292, 190), new Vector2(476, 190), new Vector2(438, 572), new Vector2(330, 572) };
            r.Polygon(Offset(tower, 10, -10), new Color(0f, 0f, 0f, 0.27f));
            r.Polygon(tower, Color.Lerp(new Color(0.40f, 0.40f, 0.37f), new Color(0.94f, 0.88f, 0.72f), Mathf.Clamp01((state - 4) / 12f)));
            r.Polyline(tower, 7, new Color(0.32f, 0.19f, 0.10f), true);

            int cracks = Mathf.Max(0, 17 - state);
            for (int i = 0; i < cracks; i++)
            {
                float x = 322 + (i * 37 % 112);
                float y = 240 + (i * 61 % 280);
                r.Line(x, y, x + 14 - i % 7, y + 18, 3, new Color(0.17f, 0.16f, 0.14f, 0.62f));
                r.Line(x + 12, y + 18, x + 4, y + 34, 2, new Color(0.17f, 0.16f, 0.14f, 0.48f));
            }

            if (state >= 12)
            {
                int bands = Mathf.Clamp(1 + (state - 12) / 2, 1, 4);
                for (int i = 0; i < bands; i++)
                {
                    float y = 245 + i * 78;
                    float left = Mathf.Lerp(300, 326, (y - 190) / 382f);
                    float right = Mathf.Lerp(468, 442, (y - 190) / 382f);
                    r.Polygon(new[] { new Vector2(left, y), new Vector2(right, y), new Vector2(right - 4, y + 34), new Vector2(left + 4, y + 34) }, new Color(0.77f, 0.16f, 0.13f));
                }
            }

            DrawDoor(r, state);
            DrawWindows(r, state);
            if (state >= 3 && state <= 15) DrawScaffolding(r);
            DrawLanternRoom(r, state);
            DrawYard(r, state);

            if (state >= 23)
            {
                float alpha = Mathf.Lerp(0.15f, 0.46f, (state - 23) / 8f);
                r.Beam(new Vector2(384, 632), new Vector2(72, 708), 54, new Color(1f, 0.87f, 0.45f, alpha));
                r.Beam(new Vector2(384, 632), new Vector2(738, 675), 46, new Color(1f, 0.87f, 0.45f, alpha * 0.75f));
                r.Glow(384, 632, 82, new Color(1f, 0.82f, 0.30f, 0.48f));
            }
            if (state >= 28)
            {
                DrawBird(r, 145, 604, 1.2f);
                DrawBird(r, 205, 644, 0.8f);
                DrawBird(r, 570, 580, 0.9f);
            }
            r.Noise(4100 + state * 97, 0.012f);
            return r;
        }

        private static void DrawDoor(Raster r, int state)
        {
            Color door = state < 8 ? new Color(0.20f, 0.18f, 0.15f) : new Color(0.15f, 0.34f, 0.39f);
            r.RoundRect(343, 191, 82, 112, 35, door);
            r.Border(343, 191, 82, 112, 35, 6, new Color(0.28f, 0.16f, 0.08f));
            r.Ellipse(408, 245, 5, 5, new Color(1f, 0.76f, 0.28f));
            if (state < 8) r.Line(350, 213, 416, 278, 8, new Color(0.48f, 0.29f, 0.12f));
        }

        private static void DrawWindows(Raster r, int state)
        {
            int visible = Mathf.Clamp((state - 8) / 2, 0, 4);
            for (int i = 0; i < 4; i++)
            {
                float y = 327 + i * 61;
                bool repaired = i < visible;
                r.Ellipse(384, y, 25, 30, repaired ? new Color(0.24f, 0.64f, 0.78f) : new Color(0.12f, 0.12f, 0.12f));
                r.Ring(384, y, 25, 30, 4, new Color(0.32f, 0.18f, 0.09f));
                if (repaired) r.Glow(376, y + 8, 22, new Color(0.70f, 0.94f, 1f, 0.20f));
                else
                {
                    r.Line(368, y - 16, 400, y + 18, 3, new Color(0.72f, 0.72f, 0.68f, 0.5f));
                    r.Line(398, y - 18, 372, y + 15, 2, new Color(0.72f, 0.72f, 0.68f, 0.4f));
                }
            }
        }

        private static void DrawScaffolding(Raster r)
        {
            Color wood = new Color(0.54f, 0.32f, 0.13f, 0.92f);
            r.Line(266, 176, 292, 560, 6, wood);
            r.Line(503, 176, 474, 560, 6, wood);
            for (int y = 230; y <= 540; y += 78) r.Line(270, y, 498, y, 7, wood);
            r.Line(270, 230, 493, 465, 4, new Color(0.78f, 0.59f, 0.31f, 0.68f));
            r.Line(498, 230, 276, 465, 4, new Color(0.78f, 0.59f, 0.31f, 0.68f));
        }

        private static void DrawLanternRoom(Raster r, int state)
        {
            Color metal = state < 17 ? new Color(0.26f, 0.25f, 0.23f) : new Color(0.22f, 0.36f, 0.38f);
            r.RoundRect(305, 565, 158, 24, 10, metal);
            r.Border(305, 565, 158, 24, 10, 3, new Color(0.74f, 0.48f, 0.18f));
            if (state >= 17)
            {
                for (int x = 318; x <= 450; x += 22) r.Line(x, 589, x, 615, 4, new Color(0.25f, 0.32f, 0.32f));
                r.Line(308, 615, 460, 615, 5, new Color(0.25f, 0.32f, 0.32f));
            }

            Color glass = state < 19 ? new Color(0.16f, 0.19f, 0.20f, 0.92f) : new Color(0.35f, 0.70f, 0.78f, 0.72f);
            Vector2[] room = { new Vector2(326, 590), new Vector2(442, 590), new Vector2(430, 651), new Vector2(338, 651) };
            r.Polygon(room, glass);
            r.Polyline(room, 5, metal, true);
            for (int x = 350; x <= 418; x += 23) r.Line(x, 592, x, 649, 3, metal);
            if (state >= 21)
            {
                r.EllipseGradient(384, 619, 24, 36, new Color(1f, 0.38f, 0.10f), new Color(1f, 0.96f, 0.58f));
                r.Glow(384, 619, 58, new Color(1f, 0.72f, 0.24f, 0.42f));
            }

            Color roof = state < 18 ? new Color(0.20f, 0.15f, 0.13f) : new Color(0.42f, 0.12f, 0.10f);
            Vector2[] roofPoints = { new Vector2(314, 651), new Vector2(454, 651), new Vector2(384, 710) };
            r.Polygon(roofPoints, roof);
            r.Polyline(roofPoints, 5, new Color(0.65f, 0.38f, 0.14f), true);
            r.Line(384, 708, 384, 733, 5, new Color(0.42f, 0.28f, 0.12f));
            r.Ellipse(384, 738, 7, 7, new Color(1f, 0.74f, 0.28f));
        }

        private static void DrawYard(Raster r, int state)
        {
            int debris = Mathf.Max(0, 15 - state * 2);
            for (int i = 0; i < debris; i++)
            {
                float x = 180 + (i * 83 % 410);
                float y = 104 + (i * 37 % 92);
                r.Line(x, y, x + 34, y + 12, 7, new Color(0.36f, 0.22f, 0.10f));
            }

            if (state >= 5)
            {
                Vector2[] path = { new Vector2(180, 72), new Vector2(250, 108), new Vector2(320, 140), new Vector2(352, 190) };
                r.Polyline(path, 20, new Color(0.62f, 0.49f, 0.30f, 0.92f), false);
                r.Polyline(path, 8, new Color(0.84f, 0.70f, 0.45f, 0.82f), false);
            }

            if (state >= 25)
            {
                for (int i = 0; i < 4; i++)
                {
                    float x = 208 + i * 108;
                    float y = 125 + (i % 2) * 20;
                    r.Line(x, y, x, y + 40, 5, new Color(0.24f, 0.17f, 0.09f));
                    r.Glow(x, y + 43, 26, new Color(1f, 0.72f, 0.22f, 0.34f));
                    r.Ellipse(x, y + 43, 8, 10, new Color(1f, 0.83f, 0.34f));
                }
            }

            if (state >= 27)
            {
                r.Polygon(new[] { new Vector2(515, 90), new Vector2(650, 25), new Vector2(670, 42), new Vector2(535, 108) }, new Color(0.45f, 0.28f, 0.12f));
                for (int i = 0; i < 6; i++)
                {
                    float t = i / 5f;
                    float x = Mathf.Lerp(532, 655, t);
                    float y = Mathf.Lerp(96, 36, t);
                    r.Line(x, y - 20, x, y + 18, 5, new Color(0.30f, 0.19f, 0.09f));
                }
            }

            if (state >= 29)
            {
                for (int i = 0; i < 45; i++)
                {
                    float x = 160 + (i * 67 % 420);
                    float y = 82 + (i * 41 % 78);
                    Color flower = i % 3 == 0 ? new Color(0.72f, 0.44f, 0.90f) : new Color(0.96f, 0.76f, 0.34f);
                    r.Ellipse(x, y, 4 + i % 3, 4 + i % 3, flower);
                }
            }

            if (state >= 30)
            {
                r.RoundRect(128, 172, 104, 52, 24, new Color(0.80f, 0.84f, 0.82f));
                r.Ellipse(180, 222, 53, 42, new Color(0.62f, 0.72f, 0.76f));
                r.Line(180, 222, 218, 250, 8, new Color(0.24f, 0.34f, 0.38f));
                r.Line(218, 250, 250, 263, 10, new Color(0.18f, 0.28f, 0.32f));
            }
        }

        private static void DrawCloud(Raster r, float x, float y, float scale, Color color)
        {
            r.Ellipse(x, y, 62 * scale, 26 * scale, color);
            r.Ellipse(x + 48 * scale, y + 6 * scale, 48 * scale, 32 * scale, color);
            r.Ellipse(x - 45 * scale, y + 4 * scale, 43 * scale, 29 * scale, color);
            r.Ellipse(x + 4 * scale, y + 22 * scale, 45 * scale, 37 * scale, color);
        }

        private static void DrawBird(Raster r, float x, float y, float scale)
        {
            Color c = new Color(0.06f, 0.10f, 0.14f, 0.72f);
            r.Arc(x - 10 * scale, y, 14 * scale, 0.15f, 2.85f, 2.2f * scale, c);
            r.Arc(x + 10 * scale, y, 14 * scale, 0.30f, 3f, 2.2f * scale, c);
        }

        private static Vector2[] StarPoints(float cx, float cy, float outer, float inner, int count, float rotation)
        {
            var points = new Vector2[count * 2];
            for (int i = 0; i < points.Length; i++)
            {
                float radius = i % 2 == 0 ? outer : inner;
                float angle = rotation + i * Mathf.PI / count;
                points[i] = new Vector2(cx + Mathf.Cos(angle) * radius, cy + Mathf.Sin(angle) * radius);
            }
            return points;
        }

        private static Vector2[] Offset(Vector2[] source, float x, float y)
        {
            var result = new Vector2[source.Length];
            for (int i = 0; i < source.Length; i++) result[i] = source[i] + new Vector2(x, y);
            return result;
        }

        private static Color Dark(Color c, float amount) => Color.Lerp(c, new Color(0.01f, 0.02f, 0.05f, c.a), Mathf.Clamp01(amount));
        private static Color Light(Color c, float amount) => Color.Lerp(c, Color.white, Mathf.Clamp01(amount));
        private static Color WithAlpha(Color c, float alpha) => new Color(c.r, c.g, c.b, alpha);

        private static void Save(string path, Raster raster, Vector4 border, bool readable, int maxSize = 1024)
        {
            Texture2D texture = raster.ToTexture();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            if (!(AssetImporter.GetAtPath(path) is TextureImporter importer)) return;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.isReadable = readable;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.maxTextureSize = maxSize;
            importer.spritePixelsPerUnit = 100f;
            importer.spriteBorder = border;
            importer.SaveAndReimport();
        }

        private sealed class Raster
        {
            private readonly int width;
            private readonly int height;
            private readonly Color[] pixels;

            public Raster(int width, int height, Color background)
            {
                this.width = width;
                this.height = height;
                pixels = new Color[width * height];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = background;
            }

            public Texture2D ToTexture()
            {
                var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp
                };
                texture.SetPixels(pixels);
                texture.Apply();
                return texture;
            }

            public void Blend(int x, int y, Color source)
            {
                if (x < 0 || y < 0 || x >= width || y >= height || source.a <= 0f) return;
                int index = y * width + x;
                Color destination = pixels[index];
                float outA = source.a + destination.a * (1f - source.a);
                if (outA <= 0.0001f) { pixels[index] = Color.clear; return; }
                pixels[index] = new Color(
                    (source.r * source.a + destination.r * destination.a * (1f - source.a)) / outA,
                    (source.g * source.a + destination.g * destination.a * (1f - source.a)) / outA,
                    (source.b * source.a + destination.b * destination.a * (1f - source.a)) / outA,
                    outA);
            }

            public void VerticalGradient(Color bottom, Color top, int minY, int maxY)
            {
                minY = Mathf.Clamp(minY, 0, height - 1);
                maxY = Mathf.Clamp(maxY, minY + 1, height - 1);
                for (int y = minY; y <= maxY; y++)
                {
                    Color row = Color.Lerp(bottom, top, (y - minY) / (float)Mathf.Max(1, maxY - minY));
                    for (int x = 0; x < width; x++) pixels[y * width + x] = row;
                }
            }

            public void RoundRect(float x, float y, float w, float h, float radius, Color color)
            {
                IterateBounds(x, y, w, h, (px, py) =>
                {
                    if (InsideRoundRect(px + 0.5f, py + 0.5f, x, y, w, h, radius)) Blend(px, py, color);
                });
            }

            public void RoundRectGradient(float x, float y, float w, float h, float radius, Color bottom, Color top)
            {
                IterateBounds(x, y, w, h, (px, py) =>
                {
                    if (!InsideRoundRect(px + 0.5f, py + 0.5f, x, y, w, h, radius)) return;
                    Blend(px, py, Color.Lerp(bottom, top, Mathf.InverseLerp(y, y + h, py)));
                });
            }

            public void RoundRectHorizontalGradient(float x, float y, float w, float h, float radius, Color left, Color right)
            {
                IterateBounds(x, y, w, h, (px, py) =>
                {
                    if (!InsideRoundRect(px + 0.5f, py + 0.5f, x, y, w, h, radius)) return;
                    Blend(px, py, Color.Lerp(left, right, Mathf.InverseLerp(x, x + w, px)));
                });
            }

            public void Border(float x, float y, float w, float h, float radius, float thickness, Color color)
            {
                IterateBounds(x, y, w, h, (px, py) =>
                {
                    bool outer = InsideRoundRect(px + 0.5f, py + 0.5f, x, y, w, h, radius);
                    bool inner = InsideRoundRect(px + 0.5f, py + 0.5f,
                        x + thickness, y + thickness, w - thickness * 2f, h - thickness * 2f,
                        Mathf.Max(0f, radius - thickness));
                    if (outer && !inner) Blend(px, py, color);
                });
            }

            public void Ellipse(float cx, float cy, float rx, float ry, Color color)
            {
                int minX = Mathf.Max(0, Mathf.FloorToInt(cx - rx - 1));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(cx + rx + 1));
                int minY = Mathf.Max(0, Mathf.FloorToInt(cy - ry - 1));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(cy + ry + 1));
                for (int py = minY; py <= maxY; py++)
                {
                    for (int px = minX; px <= maxX; px++)
                    {
                        float dx = (px + 0.5f - cx) / Mathf.Max(0.001f, rx);
                        float dy = (py + 0.5f - cy) / Mathf.Max(0.001f, ry);
                        float d = Mathf.Sqrt(dx * dx + dy * dy);
                        if (d > 1.02f) continue;
                        Color c = color;
                        c.a *= Mathf.Clamp01((1.02f - d) * 28f);
                        Blend(px, py, c);
                    }
                }
            }

            public void EllipseGradient(float cx, float cy, float rx, float ry, Color edge, Color center)
            {
                int minX = Mathf.Max(0, Mathf.FloorToInt(cx - rx - 1));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(cx + rx + 1));
                int minY = Mathf.Max(0, Mathf.FloorToInt(cy - ry - 1));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(cy + ry + 1));
                for (int py = minY; py <= maxY; py++)
                {
                    for (int px = minX; px <= maxX; px++)
                    {
                        float dx = (px + 0.5f - cx) / rx;
                        float dy = (py + 0.5f - cy) / ry;
                        float d = Mathf.Sqrt(dx * dx + dy * dy);
                        if (d > 1.02f) continue;
                        Color c = Color.Lerp(center, edge, Mathf.Clamp01(d));
                        c = Color.Lerp(c, Color.white, Mathf.Clamp01((0.24f - dx) * (0.45f + dy) * 0.28f));
                        c.a *= Mathf.Clamp01((1.02f - d) * 28f);
                        Blend(px, py, c);
                    }
                }
            }

            public void RotatedEllipse(float cx, float cy, float rx, float ry, float angle, Color color)
            {
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                float extent = Mathf.Max(rx, ry) + 2;
                int minX = Mathf.Max(0, Mathf.FloorToInt(cx - extent));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(cx + extent));
                int minY = Mathf.Max(0, Mathf.FloorToInt(cy - extent));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(cy + extent));
                for (int py = minY; py <= maxY; py++)
                {
                    for (int px = minX; px <= maxX; px++)
                    {
                        float dx = px - cx;
                        float dy = py - cy;
                        float lx = dx * cos + dy * sin;
                        float ly = -dx * sin + dy * cos;
                        float d = Mathf.Sqrt(lx * lx / (rx * rx) + ly * ly / (ry * ry));
                        if (d > 1.02f) continue;
                        Color c = Color.Lerp(Light(color, 0.18f), Dark(color, 0.20f), Mathf.Clamp01(d));
                        c.a *= Mathf.Clamp01((1.02f - d) * 24f);
                        Blend(px, py, c);
                    }
                }
            }

            public void Ring(float cx, float cy, float rx, float ry, float thickness, Color color)
            {
                int minX = Mathf.Max(0, Mathf.FloorToInt(cx - rx - thickness));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(cx + rx + thickness));
                int minY = Mathf.Max(0, Mathf.FloorToInt(cy - ry - thickness));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(cy + ry + thickness));
                float normalized = thickness / Mathf.Max(rx, ry);
                for (int py = minY; py <= maxY; py++)
                {
                    for (int px = minX; px <= maxX; px++)
                    {
                        float dx = (px - cx) / rx;
                        float dy = (py - cy) / ry;
                        float d = Mathf.Sqrt(dx * dx + dy * dy);
                        float alpha = Mathf.Clamp01((normalized - Mathf.Abs(d - 1f)) * Mathf.Max(rx, ry));
                        Color c = color;
                        c.a *= alpha;
                        Blend(px, py, c);
                    }
                }
            }

            public void Glow(float cx, float cy, float radius, Color color)
            {
                int minX = Mathf.Max(0, Mathf.FloorToInt(cx - radius));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(cx + radius));
                int minY = Mathf.Max(0, Mathf.FloorToInt(cy - radius));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(cy + radius));
                for (int py = minY; py <= maxY; py++)
                {
                    for (int px = minX; px <= maxX; px++)
                    {
                        float d = Vector2.Distance(new Vector2(px, py), new Vector2(cx, cy)) / radius;
                        if (d > 1f) continue;
                        Color c = color;
                        c.a *= Mathf.Pow(1f - d, 2.2f);
                        Blend(px, py, c);
                    }
                }
            }

            public void Line(float x0, float y0, float x1, float y1, float thickness, Color color)
            {
                Vector2 a = new Vector2(x0, y0);
                Vector2 b = new Vector2(x1, y1);
                Vector2 ab = b - a;
                float lengthSq = Mathf.Max(0.001f, ab.sqrMagnitude);
                int minX = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(x0, x1) - thickness));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(Mathf.Max(x0, x1) + thickness));
                int minY = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(y0, y1) - thickness));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(Mathf.Max(y0, y1) + thickness));
                for (int py = minY; py <= maxY; py++)
                {
                    for (int px = minX; px <= maxX; px++)
                    {
                        Vector2 point = new Vector2(px, py);
                        float t = Mathf.Clamp01(Vector2.Dot(point - a, ab) / lengthSq);
                        float distance = Vector2.Distance(point, a + ab * t);
                        float alpha = Mathf.Clamp01(thickness * 0.5f + 1f - distance);
                        Color c = color;
                        c.a *= alpha;
                        Blend(px, py, c);
                    }
                }
            }

            public void Polygon(IReadOnlyList<Vector2> points, Color color)
            {
                if (points == null || points.Count < 3) return;
                float minXf = float.MaxValue, maxXf = float.MinValue, minYf = float.MaxValue, maxYf = float.MinValue;
                for (int i = 0; i < points.Count; i++)
                {
                    minXf = Mathf.Min(minXf, points[i].x);
                    maxXf = Mathf.Max(maxXf, points[i].x);
                    minYf = Mathf.Min(minYf, points[i].y);
                    maxYf = Mathf.Max(maxYf, points[i].y);
                }
                int minX = Mathf.Max(0, Mathf.FloorToInt(minXf));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(maxXf));
                int minY = Mathf.Max(0, Mathf.FloorToInt(minYf));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(maxYf));
                for (int py = minY; py <= maxY; py++)
                {
                    for (int px = minX; px <= maxX; px++)
                    {
                        bool inside = false;
                        for (int i = 0, j = points.Count - 1; i < points.Count; j = i++)
                        {
                            Vector2 pi = points[i];
                            Vector2 pj = points[j];
                            bool crosses = (pi.y > py) != (pj.y > py);
                            if (crosses && px < (pj.x - pi.x) * (py - pi.y) / (pj.y - pi.y) + pi.x) inside = !inside;
                        }
                        if (inside) Blend(px, py, color);
                    }
                }
            }

            public void Polyline(IReadOnlyList<Vector2> points, float thickness, Color color, bool closed)
            {
                if (points == null || points.Count < 2) return;
                for (int i = 0; i < points.Count - 1; i++) Line(points[i].x, points[i].y, points[i + 1].x, points[i + 1].y, thickness, color);
                if (closed) Line(points[points.Count - 1].x, points[points.Count - 1].y, points[0].x, points[0].y, thickness, color);
            }

            public void Arc(float cx, float cy, float radius, float start, float end, float thickness, Color color)
            {
                const int segments = 42;
                Vector2 previous = new Vector2(cx + Mathf.Cos(start) * radius, cy + Mathf.Sin(start) * radius);
                for (int i = 1; i <= segments; i++)
                {
                    float angle = Mathf.Lerp(start, end, i / (float)segments);
                    Vector2 current = new Vector2(cx + Mathf.Cos(angle) * radius, cy + Mathf.Sin(angle) * radius);
                    Line(previous.x, previous.y, current.x, current.y, thickness, color);
                    previous = current;
                }
            }

            public void Wave(float x, float y, float length, float amplitude, float wavelength, float phase, float thickness, Color color)
            {
                float px0 = x;
                float py0 = y + Mathf.Sin(phase) * amplitude;
                for (float px1 = x + 4; px1 <= x + length; px1 += 4)
                {
                    float py1 = y + Mathf.Sin((px1 - x) / wavelength * Mathf.PI * 2f + phase) * amplitude;
                    Line(px0, py0, px1, py1, thickness, color);
                    px0 = px1;
                    py0 = py1;
                }
            }

            public void Beam(Vector2 origin, Vector2 end, float endWidth, Color color)
            {
                Vector2 direction = (end - origin).normalized;
                Vector2 normal = new Vector2(-direction.y, direction.x) * endWidth * 0.5f;
                Polygon(new[] { origin, end + normal, end - normal }, color);
            }

            public void Noise(int seed, float strength)
            {
                var random = new System.Random(seed);
                for (int i = 0; i < pixels.Length; i++)
                {
                    float delta = ((float)random.NextDouble() - 0.5f) * strength;
                    Color c = pixels[i];
                    c.r = Mathf.Clamp01(c.r + delta);
                    c.g = Mathf.Clamp01(c.g + delta);
                    c.b = Mathf.Clamp01(c.b + delta);
                    pixels[i] = c;
                }
            }

            private void IterateBounds(float x, float y, float w, float h, Action<int, int> action)
            {
                int minX = Mathf.Max(0, Mathf.FloorToInt(x));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(x + w));
                int minY = Mathf.Max(0, Mathf.FloorToInt(y));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(y + h));
                for (int py = minY; py <= maxY; py++)
                    for (int px = minX; px <= maxX; px++) action(px, py);
            }

            private static bool InsideRoundRect(float px, float py, float x, float y, float w, float h, float radius)
            {
                if (w <= 0 || h <= 0) return false;
                radius = Mathf.Clamp(radius, 0, Mathf.Min(w, h) * 0.5f);
                float cx = Mathf.Clamp(px, x + radius, x + w - radius);
                float cy = Mathf.Clamp(py, y + radius, y + h - radius);
                float dx = px - cx;
                float dy = py - cy;
                return dx * dx + dy * dy <= radius * radius + 0.5f;
            }
        }
    }
}
