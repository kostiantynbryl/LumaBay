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
        private const string VersionKey = "LumaBay.ArtPack.0.1.2.v1";

        static LumaBayArtPackGenerator()
        {
            EditorApplication.delayCall += GenerateIfNeeded;
        }

        [MenuItem("Luma Bay/Generate Premium Art Pack", priority = 3)]
        public static void GenerateAll()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;

            Directory.CreateDirectory(Root);
            Directory.CreateDirectory($"{Root}/backgrounds");
            Directory.CreateDirectory($"{Root}/ui");
            Directory.CreateDirectory($"{Root}/pieces");
            Directory.CreateDirectory($"{Root}/boosters");
            Directory.CreateDirectory($"{Root}/lighthouse");

            GenerateBackground();
            GenerateUiPack();
            GeneratePieces();
            GenerateBoosters();
            GenerateLighthouseStates();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorPrefs.SetBool(VersionKey, true);
            Debug.Log("Luma Bay premium art pack generated: 32 lighthouse states, UI, pieces and boosters.");
        }

        private static void GenerateIfNeeded()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            string sentinel = $"{Root}/lighthouse/lighthouse_31.png";
            if (!EditorPrefs.GetBool(VersionKey, false) || !File.Exists(sentinel)) GenerateAll();
        }

        private static void GenerateBackground()
        {
            var p = new Painter(720, 1280);
            p.VerticalGradient(new Color(0.025f, 0.055f, 0.13f), new Color(0.23f, 0.46f, 0.66f), 0f, 0.62f);
            p.VerticalGradient(new Color(0.23f, 0.46f, 0.66f), new Color(0.98f, 0.51f, 0.28f), 0.55f, 0.82f, true);
            p.VerticalGradient(new Color(0.04f, 0.20f, 0.33f), new Color(0.015f, 0.07f, 0.13f), 0.78f, 1f, true);
            p.Glow(560, 790, 210, new Color(1f, 0.62f, 0.25f, 0.55f));
            p.Ellipse(560, 790, 58, 58, new Color(1f, 0.83f, 0.42f, 1f), 2f);

            DrawCloud(p, 140, 930, 1.15f, new Color(0.82f, 0.88f, 0.96f, 0.25f));
            DrawCloud(p, 485, 1035, 0.85f, new Color(0.96f, 0.72f, 0.62f, 0.22f));
            DrawCloud(p, 340, 845, 0.7f, new Color(0.92f, 0.83f, 0.84f, 0.14f));

            // Sea and layered waves.
            p.Rect(0, 0, 720, 360, new Color(0.015f, 0.13f, 0.23f, 1f));
            for (int i = 0; i < 16; i++)
            {
                float y = 26 + i * 20;
                float phase = i * 0.65f;
                Color c = Color.Lerp(new Color(0.05f, 0.28f, 0.43f, 0.75f), new Color(0.27f, 0.68f, 0.74f, 0.44f), i / 15f);
                p.Wave(0, y, 720, 9f + i * 0.25f, 68f + i * 2f, phase, 3f, c);
            }

            // Warm reflection from sunset.
            for (int i = 0; i < 12; i++)
            {
                float width = Mathf.Lerp(180f, 34f, i / 11f);
                p.RoundedRect(560 - width * 0.5f, 245 + i * 20, width, 5f, 2f,
                    new Color(1f, 0.61f, 0.24f, 0.16f + i * 0.012f));
            }

            // Coastal cliffs framing the scene.
            p.Polygon(new[]
            {
                new Vector2(0, 0), new Vector2(0, 440), new Vector2(110, 410), new Vector2(170, 330),
                new Vector2(235, 260), new Vector2(270, 170), new Vector2(245, 0)
            }, new Color(0.025f, 0.10f, 0.12f, 1f));
            p.Polygon(new[]
            {
                new Vector2(720, 0), new Vector2(720, 315), new Vector2(665, 295), new Vector2(625, 235),
                new Vector2(590, 155), new Vector2(610, 0)
            }, new Color(0.04f, 0.13f, 0.14f, 1f));
            AddRockHighlights(p, 30, 115, 1f);
            AddRockHighlights(p, 650, 90, 0.65f);

            p.Noise(3187, 0.018f);
            WriteSprite($"{Root}/backgrounds/coastal_sunset.png", p, Vector4.zero, 100f, false);
        }

        private static void GenerateUiPack()
        {
            var panel = new Painter(256, 256, Color.clear);
            panel.Glow(128, 128, 124, new Color(0.06f, 0.23f, 0.35f, 0.82f));
            panel.RoundedRect(5, 5, 246, 246, 35, new Color(0.035f, 0.15f, 0.24f, 0.94f));
            panel.RoundedBorder(5, 5, 246, 246, 35, 3f, new Color(0.98f, 0.75f, 0.32f, 0.88f));
            panel.RoundedBorder(11, 11, 234, 234, 29, 1.5f, new Color(0.56f, 0.80f, 0.90f, 0.34f));
            panel.Glow(82, 198, 95, new Color(0.31f, 0.70f, 0.88f, 0.14f));
            WriteSprite($"{Root}/ui/panel_glass.png", panel, new Vector4(44, 44, 44, 44), 100f, true);

            GenerateButton("button_primary", new Color(0.06f, 0.58f, 0.64f), new Color(0.02f, 0.28f, 0.42f));
            GenerateButton("button_secondary", new Color(0.11f, 0.28f, 0.43f), new Color(0.03f, 0.12f, 0.24f));
            GenerateButton("booster_card", new Color(0.15f, 0.30f, 0.48f), new Color(0.035f, 0.12f, 0.24f), 420, 260);
            GenerateButton("goal_chip", new Color(0.10f, 0.34f, 0.48f), new Color(0.025f, 0.13f, 0.23f), 420, 150);

            var track = new Painter(512, 64, Color.clear);
            track.RoundedRect(2, 2, 508, 60, 28, new Color(0.015f, 0.07f, 0.12f, 0.95f));
            track.RoundedBorder(2, 2, 508, 60, 28, 2f, new Color(0.70f, 0.84f, 0.90f, 0.40f));
            WriteSprite($"{Root}/ui/progress_track.png", track, new Vector4(30, 30, 30, 30), 100f, true);

            var fill = new Painter(512, 64, Color.clear);
            fill.HorizontalGradientRounded(2, 2, 508, 60, 28,
                new Color(0.08f, 0.65f, 0.70f), new Color(1f, 0.76f, 0.28f));
            fill.Glow(410, 32, 90, new Color(1f, 0.88f, 0.44f, 0.35f));
            fill.RoundedBorder(2, 2, 508, 60, 28, 2f, new Color(1f, 0.91f, 0.58f, 0.75f));
            WriteSprite($"{Root}/ui/progress_fill.png", fill, new Vector4(30, 30, 30, 30), 100f, true);
        }

        private static void GenerateButton(string name, Color top, Color bottom, int width = 512, int height = 190)
        {
            var p = new Painter(width, height, Color.clear);
            p.Glow(width * 0.5f, height * 0.42f, width * 0.48f, new Color(top.r, top.g, top.b, 0.30f));
            p.VerticalGradientRounded(4, 4, width - 8, height - 8, height * 0.30f, bottom, top);
            p.RoundedBorder(4, 4, width - 8, height - 8, height * 0.30f, 4f, new Color(0.98f, 0.78f, 0.36f, 0.92f));
            p.RoundedBorder(11, 11, width - 22, height - 22, height * 0.25f, 2f, new Color(0.74f, 0.94f, 1f, 0.30f));
            p.Glow(width * 0.30f, height * 0.78f, width * 0.22f, new Color(1f, 1f, 1f, 0.14f));
            WriteSprite($"{Root}/ui/{name}.png", p,
                new Vector4(height * 0.34f, height * 0.34f, height * 0.34f, height * 0.34f), 100f, true);
        }

        private static void GeneratePieces()
        {
            GeneratePiece(PieceKind.Shell, new Color(0.54f, 0.30f, 0.94f));
            GeneratePiece(PieceKind.Starfish, new Color(0.98f, 0.30f, 0.18f));
            GeneratePiece(PieceKind.Lantern, new Color(1f, 0.68f, 0.16f));
            GeneratePiece(PieceKind.Compass, new Color(0.22f, 0.73f, 0.38f));
            GeneratePiece(PieceKind.Crystal, new Color(0.13f, 0.68f, 1f));
            GeneratePiece(PieceKind.Flower, new Color(0.98f, 0.30f, 0.62f));
        }

        private static void GeneratePiece(PieceKind kind, Color baseColor)
        {
            const int size = 320;
            var p = new Painter(size, size, Color.clear);
            p.Glow(160, 135, 125, new Color(baseColor.r, baseColor.g, baseColor.b, 0.28f));
            p.Ellipse(166, 132, 112, 96, new Color(0f, 0f, 0f, 0.30f), 14f);

            switch (kind)
            {
                case PieceKind.Shell:
                    DrawShell(p, baseColor);
                    break;
                case PieceKind.Starfish:
                    DrawStarfish(p, baseColor);
                    break;
                case PieceKind.Lantern:
                    DrawLantern(p, baseColor);
                    break;
                case PieceKind.Compass:
                    DrawCompass(p, baseColor);
                    break;
                case PieceKind.Crystal:
                    DrawCrystal(p, baseColor);
                    break;
                case PieceKind.Flower:
                    DrawFlower(p, baseColor);
                    break;
            }

            WriteSprite($"{Root}/pieces/piece_{kind.ToString().ToLowerInvariant()}.png", p, Vector4.zero, 100f, false);
        }

        private static void DrawShell(Painter p, Color c)
        {
            p.EllipseGradient(160, 158, 110, 94, Darken(c, 0.52f), Lighten(c, 0.34f));
            p.Polygon(new[] { new Vector2(65, 150), new Vector2(255, 150), new Vector2(226, 78), new Vector2(94, 78) }, Darken(c, 0.72f));
            for (int i = -4; i <= 4; i++)
            {
                float x = 160 + i * 18f;
                p.Line(160, 235, x, 92, 4.5f, new Color(1f, 0.88f, 1f, 0.34f));
            }
            p.Ellipse(127, 195, 30, 22, new Color(1f, 1f, 1f, 0.24f), 8f);
            p.Ring(160, 158, 111, 95, 5f, new Color(1f, 0.82f, 0.38f, 0.88f));
        }

        private static void DrawStarfish(Painter p, Color c)
        {
            Vector2[] points = StarPoints(160, 157, 116, 48, 5, -Mathf.PI * 0.5f);
            p.Polygon(points, Darken(c, 0.56f));
            Vector2[] inner = StarPoints(156, 166, 101, 43, 5, -Mathf.PI * 0.5f);
            p.Polygon(inner, Lighten(c, 0.16f));
            for (int i = 0; i < 24; i++)
            {
                float angle = i * 2.39996f;
                float radius = 18f + (i % 6) * 12f;
                p.Ellipse(160 + Mathf.Cos(angle) * radius, 160 + Mathf.Sin(angle) * radius, 4.5f, 4.5f,
                    new Color(1f, 0.78f, 0.45f, 0.46f), 1f);
            }
            p.Glow(124, 204, 52, new Color(1f, 1f, 1f, 0.24f));
            p.Polyline(points, 5f, new Color(1f, 0.78f, 0.31f, 0.88f), true);
        }

        private static void DrawLantern(Painter p, Color c)
        {
            p.RoundedRect(84, 66, 152, 180, 35, Darken(c, 0.52f));
            p.VerticalGradientRounded(94, 77, 132, 158, 28, Darken(c, 0.22f), Lighten(c, 0.34f));
            p.RoundedBorder(84, 66, 152, 180, 35, 6f, new Color(0.40f, 0.23f, 0.08f, 1f));
            p.Line(110, 246, 125, 282, 10f, new Color(0.45f, 0.27f, 0.09f, 1f));
            p.Line(210, 246, 195, 282, 10f, new Color(0.45f, 0.27f, 0.09f, 1f));
            p.Line(125, 282, 195, 282, 10f, new Color(0.45f, 0.27f, 0.09f, 1f));
            p.Glow(160, 148, 74, new Color(1f, 0.72f, 0.18f, 0.48f));
            p.Polygon(new[] { new Vector2(160, 94), new Vector2(128, 151), new Vector2(152, 205), new Vector2(188, 153) },
                new Color(1f, 0.91f, 0.42f, 0.94f));
            p.Polygon(new[] { new Vector2(160, 113), new Vector2(145, 154), new Vector2(160, 188), new Vector2(177, 151) },
                new Color(1f, 1f, 0.82f, 0.95f));
        }

        private static void DrawCompass(Painter p, Color c)
        {
            p.EllipseGradient(160, 160, 112, 112, Darken(c, 0.62f), Lighten(c, 0.30f));
            p.Ring(160, 160, 112, 112, 8f, new Color(1f, 0.77f, 0.30f, 0.96f));
            p.Ring(160, 160, 86, 86, 3f, new Color(0.82f, 1f, 0.90f, 0.55f));
            p.Polygon(new[] { new Vector2(160, 61), new Vector2(181, 160), new Vector2(160, 144), new Vector2(139, 160) },
                new Color(1f, 0.34f, 0.20f, 1f));
            p.Polygon(new[] { new Vector2(160, 259), new Vector2(139, 160), new Vector2(160, 176), new Vector2(181, 160) },
                new Color(0.82f, 0.96f, 1f, 1f));
            p.Ellipse(160, 160, 16, 16, new Color(1f, 0.84f, 0.38f, 1f), 2f);
            p.Glow(121, 205, 55, new Color(1f, 1f, 1f, 0.20f));
        }

        private static void DrawCrystal(Painter p, Color c)
        {
            Vector2[] outer =
            {
                new Vector2(160, 284), new Vector2(64, 178), new Vector2(92, 72),
                new Vector2(160, 36), new Vector2(232, 74), new Vector2(256, 178)
            };
            p.Polygon(outer, Darken(c, 0.56f));
            p.Polygon(new[] { new Vector2(160, 274), new Vector2(86, 176), new Vector2(112, 84), new Vector2(160, 52) },
                Lighten(c, 0.28f));
            p.Polygon(new[] { new Vector2(160, 274), new Vector2(160, 52), new Vector2(216, 87), new Vector2(235, 176) },
                Darken(c, 0.18f));
            p.Polygon(new[] { new Vector2(160, 52), new Vector2(112, 84), new Vector2(160, 120), new Vector2(216, 87) },
                new Color(0.80f, 0.96f, 1f, 0.76f));
            p.Polyline(outer, 6f, new Color(1f, 0.82f, 0.34f, 0.92f), true);
            p.Line(128, 90, 102, 166, 7f, new Color(1f, 1f, 1f, 0.40f));
        }

        private static void DrawFlower(Painter p, Color c)
        {
            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI / 4f;
                float cx = 160 + Mathf.Cos(angle) * 64f;
                float cy = 160 + Mathf.Sin(angle) * 64f;
                p.RotatedEllipse(cx, cy, 58, 35, angle, Color.Lerp(Darken(c, 0.18f), Lighten(c, 0.30f), i / 7f));
            }
            p.EllipseGradient(160, 160, 55, 55, new Color(0.64f, 0.27f, 0.05f), new Color(1f, 0.84f, 0.24f));
            p.Ring(160, 160, 55, 55, 4f, new Color(1f, 0.85f, 0.42f, 0.92f));
            p.Glow(125, 205, 48, new Color(1f, 1f, 1f, 0.20f));
        }

        private static void GenerateBoosters()
        {
            GenerateBooster("lightning", DrawLightning, new Color(0.20f, 0.60f, 1f));
            GenerateBooster("anchor", DrawAnchor, new Color(0.55f, 0.31f, 0.92f));
            GenerateBooster("shuffle", DrawShuffle, new Color(0.10f, 0.72f, 0.72f));
            GenerateBooster("extra_moves", DrawHourglass, new Color(0.27f, 0.76f, 0.38f));
            GenerateBooster("harpoon", DrawHarpoon, new Color(0.95f, 0.30f, 0.22f));
        }

        private static void GenerateBooster(string name, Action<Painter, Color> draw, Color accent)
        {
            var p = new Painter(320, 320, Color.clear);
            p.Glow(160, 160, 132, new Color(accent.r, accent.g, accent.b, 0.42f));
            p.EllipseGradient(160, 160, 118, 118, new Color(0.025f, 0.12f, 0.24f, 1f), new Color(0.10f, 0.31f, 0.50f, 1f));
            p.Ring(160, 160, 118, 118, 7f, new Color(1f, 0.80f, 0.34f, 0.95f));
            draw(p, accent);
            WriteSprite($"{Root}/boosters/{name}.png", p, Vector4.zero, 100f, false);
        }

        private static void DrawLightning(Painter p, Color c)
        {
            p.Polygon(new[]
            {
                new Vector2(178, 54), new Vector2(98, 171), new Vector2(151, 165),
                new Vector2(126, 270), new Vector2(228, 134), new Vector2(174, 141)
            }, Lighten(c, 0.35f));
            p.Polyline(new[]
            {
                new Vector2(178, 54), new Vector2(98, 171), new Vector2(151, 165),
                new Vector2(126, 270), new Vector2(228, 134), new Vector2(174, 141)
            }, 5f, new Color(1f, 0.88f, 0.38f, 1f), true);
        }

        private static void DrawAnchor(Painter p, Color c)
        {
            p.Ring(160, 92, 30, 30, 10f, Lighten(c, 0.40f));
            p.Line(160, 116, 160, 236, 18f, Lighten(c, 0.24f));
            p.Line(100, 154, 220, 154, 15f, Lighten(c, 0.24f));
            p.Line(160, 236, 102, 210, 18f, Lighten(c, 0.24f));
            p.Line(160, 236, 218, 210, 18f, Lighten(c, 0.24f));
            p.Line(102, 210, 88, 181, 15f, Lighten(c, 0.24f));
            p.Line(218, 210, 232, 181, 15f, Lighten(c, 0.24f));
        }

        private static void DrawShuffle(Painter p, Color c)
        {
            p.Arc(160, 160, 78, 0.25f, 2.75f, 15f, Lighten(c, 0.32f));
            p.Arc(160, 160, 78, 3.40f, 5.95f, 15f, Lighten(c, 0.32f));
            p.Polygon(new[] { new Vector2(225, 104), new Vector2(260, 111), new Vector2(237, 139) }, Lighten(c, 0.32f));
            p.Polygon(new[] { new Vector2(95, 216), new Vector2(60, 208), new Vector2(83, 180) }, Lighten(c, 0.32f));
        }

        private static void DrawHourglass(Painter p, Color c)
        {
            p.Line(104, 72, 216, 72, 14f, new Color(1f, 0.78f, 0.31f, 1f));
            p.Line(104, 248, 216, 248, 14f, new Color(1f, 0.78f, 0.31f, 1f));
            p.Polygon(new[] { new Vector2(114, 86), new Vector2(206, 86), new Vector2(177, 154), new Vector2(206, 234), new Vector2(114, 234), new Vector2(143, 154) },
                new Color(0.74f, 0.94f, 1f, 0.72f));
            p.Polygon(new[] { new Vector2(132, 211), new Vector2(188, 211), new Vector2(160, 165) }, Lighten(c, 0.30f));
            p.Polygon(new[] { new Vector2(135, 102), new Vector2(185, 102), new Vector2(160, 143) }, Lighten(c, 0.30f));
        }

        private static void DrawHarpoon(Painter p, Color c)
        {
            p.Line(92, 235, 218, 109, 18f, Lighten(c, 0.30f));
            p.Polygon(new[] { new Vector2(221, 105), new Vector2(250, 64), new Vector2(211, 82), new Vector2(196, 54) },
                new Color(1f, 0.86f, 0.42f, 1f));
            p.Line(105, 222, 79, 196, 14f, new Color(0.50f, 0.24f, 0.09f, 1f));
            p.Line(105, 222, 131, 248, 14f, new Color(0.50f, 0.24f, 0.09f, 1f));
        }

        private static void GenerateLighthouseStates()
        {
            for (int state = 0; state < 32; state++)
            {
                Painter p = DrawLighthouseScene(state);
                WriteSprite($"{Root}/lighthouse/lighthouse_{state:00}.png", p, Vector4.zero, 100f, false, 2048);
            }
        }

        private static Painter DrawLighthouseScene(int state)
        {
            const int size = 768;
            var p = new Painter(size, size);
            float progress = state / 31f;

            p.VerticalGradient(new Color(0.08f, 0.20f, 0.40f), new Color(0.96f, 0.49f, 0.29f), 0f, 1f);
            p.Glow(604, 565, 190, new Color(1f, 0.68f, 0.30f, 0.45f));
            p.Ellipse(604, 565, 42, 42, new Color(1f, 0.84f, 0.46f, 1f), 2f);
            DrawCloud(p, 158, 604, 0.82f, new Color(0.88f, 0.91f, 0.98f, 0.24f));
            DrawCloud(p, 480, 650, 0.58f, new Color(1f, 0.78f, 0.68f, 0.20f));

            p.Rect(0, 0, size, 214, new Color(0.025f, 0.19f, 0.31f, 1f));
            for (int i = 0; i < 9; i++)
            {
                p.Wave(0, 26 + i * 18, size, 7 + i * 0.35f, 76 + i * 3f, i * 0.55f, 3f,
                    new Color(0.15f, 0.55f, 0.67f, 0.42f));
            }

            // Island and cliff are organic polygons, not rectangular blocks.
            p.Polygon(new[]
            {
                new Vector2(94, 0), new Vector2(112, 112), new Vector2(172, 191), new Vector2(251, 236),
                new Vector2(360, 251), new Vector2(470, 230), new Vector2(563, 178), new Vector2(623, 95),
                new Vector2(646, 0)
            }, new Color(0.06f, 0.15f, 0.14f, 1f));
            p.Polyline(new[]
            {
                new Vector2(112, 112), new Vector2(172, 191), new Vector2(251, 236), new Vector2(360, 251),
                new Vector2(470, 230), new Vector2(563, 178), new Vector2(623, 95)
            }, 10f, new Color(0.18f, 0.28f, 0.22f, 1f));

            // Grass gradually returns after the structural restoration.
            if (state >= 6)
            {
                for (int i = 0; i < 70 + state * 3; i++)
                {
                    float x = 145 + (i * 53 % 450);
                    float y = 78 + (i * 29 % 130);
                    float h = 10 + (i % 9);
                    Color grass = Color.Lerp(new Color(0.12f, 0.35f, 0.22f), new Color(0.36f, 0.62f, 0.28f), progress);
                    p.Line(x, y, x + Mathf.Sin(i) * 4f, y + h, 2.2f, grass);
                }
            }

            int repairedWalls = Mathf.Clamp(state - 4, 0, 12);
            float towerBottomY = 190;
            Vector2[] tower =
            {
                new Vector2(292, towerBottomY), new Vector2(476, towerBottomY),
                new Vector2(438, 572), new Vector2(330, 572)
            };

            p.Polygon(Offset(tower, 10, -10), new Color(0f, 0f, 0f, 0.28f));
            Color damagedWall = new Color(0.40f, 0.40f, 0.37f);
            Color restoredWall = new Color(0.94f, 0.88f, 0.72f);
            p.Polygon(tower, Color.Lerp(damagedWall, restoredWall, repairedWalls / 12f));
            p.Polyline(tower, 7f, new Color(0.32f, 0.19f, 0.10f, 0.92f), true);

            // Cracks and missing plaster fade gradually.
            int cracks = Mathf.Max(0, 17 - state);
            for (int i = 0; i < cracks; i++)
            {
                float x = 322 + (i * 37 % 112);
                float y = 240 + (i * 61 % 280);
                p.Line(x, y, x + 14 - i % 7, y + 18, 3f, new Color(0.17f, 0.16f, 0.14f, 0.62f));
                p.Line(x + 12, y + 18, x + 4, y + 34, 2.2f, new Color(0.17f, 0.16f, 0.14f, 0.50f));
            }

            // Paint bands appear in multiple visible steps.
            if (state >= 12)
            {
                int bands = Mathf.Clamp(1 + (state - 12) / 2, 1, 4);
                for (int i = 0; i < bands; i++)
                {
                    float y = 245 + i * 78;
                    float left = Mathf.Lerp(300, 326, (y - towerBottomY) / 382f);
                    float right = Mathf.Lerp(468, 442, (y - towerBottomY) / 382f);
                    p.Polygon(new[]
                    {
                        new Vector2(left, y), new Vector2(right, y), new Vector2(right - 4, y + 34), new Vector2(left + 4, y + 34)
                    }, new Color(0.77f, 0.16f, 0.13f, 0.98f));
                }
            }

            // Door and windows.
            DrawDoor(p, state);
            DrawWindows(p, state);

            // Scaffolding exists during the mid-restoration arc.
            if (state >= 3 && state <= 15)
            {
                Color wood = new Color(0.54f, 0.32f, 0.13f, 0.92f);
                p.Line(266, 176, 292, 560, 6f, wood);
                p.Line(503, 176, 474, 560, 6f, wood);
                for (int y = 230; y <= 540; y += 78) p.Line(270, y, 498, y, 7f, wood);
                p.Line(270, 230, 493, 465, 4f, new Color(0.78f, 0.59f, 0.31f, 0.68f));
                p.Line(498, 230, 276, 465, 4f, new Color(0.78f, 0.59f, 0.31f, 0.68f));
            }

            // Gallery, lantern room, roof and lens.
            DrawLanternRoom(p, state);

            // Foreground restoration details.
            DrawYardAndUpgrades(p, state);

            // Final lighthouse beam and ambient life.
            if (state >= 23)
            {
                float alpha = Mathf.Lerp(0.16f, 0.48f, (state - 23) / 8f);
                p.Beam(new Vector2(384, 632), new Vector2(75, 708), 54, new Color(1f, 0.87f, 0.45f, alpha));
                p.Beam(new Vector2(384, 632), new Vector2(735, 675), 46, new Color(1f, 0.87f, 0.45f, alpha * 0.76f));
                p.Glow(384, 632, 82, new Color(1f, 0.82f, 0.30f, 0.50f));
            }

            if (state >= 28)
            {
                DrawBird(p, 145, 604, 1.2f);
                DrawBird(p, 202, 642, 0.8f);
                DrawBird(p, 568, 578, 0.9f);
            }

            p.Noise(4100 + state * 97, 0.012f);
            return p;
        }

        private static void DrawDoor(Painter p, int state)
        {
            Color frame = new Color(0.28f, 0.16f, 0.08f, 1f);
            Color door = state < 8 ? new Color(0.20f, 0.18f, 0.15f) : new Color(0.15f, 0.34f, 0.39f);
            p.RoundedRect(343, 191, 82, 112, 35, door);
            p.RoundedBorder(343, 191, 82, 112, 35, 6f, frame);
            p.Ellipse(408, 245, 5, 5, new Color(1f, 0.76f, 0.28f, 1f), 1f);
            if (state < 8)
            {
                p.Line(350, 213, 416, 278, 8f, new Color(0.48f, 0.29f, 0.12f, 1f));
            }
        }

        private static void DrawWindows(Painter p, int state)
        {
            int visible = Mathf.Clamp((state - 8) / 2, 0, 4);
            for (int i = 0; i < 4; i++)
            {
                float y = 327 + i * 61;
                bool repaired = i < visible;
                Color glass = repaired ? new Color(0.24f, 0.64f, 0.78f, 0.94f) : new Color(0.12f, 0.12f, 0.12f, 0.88f);
                p.Ellipse(384, y, 25, 30, glass, 2f);
                p.Ring(384, y, 25, 30, 4f, new Color(0.32f, 0.18f, 0.09f, 1f));
                if (repaired) p.Glow(376, y + 8, 22, new Color(0.70f, 0.94f, 1f, 0.22f));
                else
                {
                    p.Line(368, y - 16, 400, y + 18, 3f, new Color(0.72f, 0.72f, 0.68f, 0.5f));
                    p.Line(398, y - 18, 372, y + 15, 2f, new Color(0.72f, 0.72f, 0.68f, 0.4f));
                }
            }
        }

        private static void DrawLanternRoom(Painter p, int state)
        {
            Color metal = state < 17 ? new Color(0.26f, 0.25f, 0.23f) : new Color(0.22f, 0.36f, 0.38f);
            p.RoundedRect(305, 565, 158, 24, 10, metal);
            p.RoundedBorder(305, 565, 158, 24, 10, 3f, new Color(0.74f, 0.48f, 0.18f, 0.92f));

            if (state >= 17)
            {
                for (int x = 318; x <= 450; x += 22) p.Line(x, 589, x, 615, 4f, new Color(0.25f, 0.32f, 0.32f, 1f));
                p.Line(308, 615, 460, 615, 5f, new Color(0.25f, 0.32f, 0.32f, 1f));
            }

            Color glass = state < 19 ? new Color(0.16f, 0.19f, 0.20f, 0.92f) : new Color(0.35f, 0.70f, 0.78f, 0.72f);
            p.Polygon(new[]
            {
                new Vector2(326, 590), new Vector2(442, 590), new Vector2(430, 651), new Vector2(338, 651)
            }, glass);
            p.Polyline(new[]
            {
                new Vector2(326, 590), new Vector2(442, 590), new Vector2(430, 651), new Vector2(338, 651)
            }, 5f, metal, true);
            for (int x = 350; x <= 418; x += 23) p.Line(x, 592, x, 649, 3f, metal);

            if (state >= 21)
            {
                p.EllipseGradient(384, 619, 24, 36, new Color(1f, 0.38f, 0.10f), new Color(1f, 0.96f, 0.58f));
                p.Glow(384, 619, 58, new Color(1f, 0.72f, 0.24f, 0.42f));
            }

            Color roof = state < 18 ? new Color(0.20f, 0.15f, 0.13f) : new Color(0.42f, 0.12f, 0.10f);
            p.Polygon(new[] { new Vector2(314, 651), new Vector2(454, 651), new Vector2(384, 710) }, roof);
            p.Polyline(new[] { new Vector2(314, 651), new Vector2(454, 651), new Vector2(384, 710) }, 5f,
                new Color(0.65f, 0.38f, 0.14f, 1f), true);
            p.Line(384, 708, 384, 733, 5f, new Color(0.42f, 0.28f, 0.12f, 1f));
            p.Ellipse(384, 738, 7, 7, new Color(1f, 0.74f, 0.28f, 1f), 2f);
        }

        private static void DrawYardAndUpgrades(Painter p, int state)
        {
            // Debris disappears over the first tasks.
            int debris = Mathf.Max(0, 15 - state * 2);
            for (int i = 0; i < debris; i++)
            {
                float x = 180 + (i * 83 % 410);
                float y = 104 + (i * 37 % 92);
                p.Line(x, y, x + 34, y + 12, 7f, new Color(0.36f, 0.22f, 0.10f, 1f));
            }

            if (state >= 5)
            {
                p.CurvePath(new[] { new Vector2(180, 72), new Vector2(250, 108), new Vector2(320, 140), new Vector2(352, 190) },
                    20f, new Color(0.62f, 0.49f, 0.30f, 0.92f));
                p.CurvePath(new[] { new Vector2(180, 72), new Vector2(250, 108), new Vector2(320, 140), new Vector2(352, 190) },
                    8f, new Color(0.84f, 0.70f, 0.45f, 0.82f));
            }

            if (state >= 25)
            {
                // Warm exterior lamps.
                for (int i = 0; i < 4; i++)
                {
                    float x = 208 + i * 108;
                    float y = 125 + (i % 2) * 20;
                    p.Line(x, y, x, y + 40, 5f, new Color(0.24f, 0.17f, 0.09f, 1f));
                    p.Glow(x, y + 43, 26, new Color(1f, 0.72f, 0.22f, 0.35f));
                    p.Ellipse(x, y + 43, 8, 10, new Color(1f, 0.83f, 0.34f, 1f), 2f);
                }
            }

            if (state >= 27)
            {
                // Small pier.
                p.Polygon(new[] { new Vector2(515, 90), new Vector2(650, 25), new Vector2(670, 42), new Vector2(535, 108) },
                    new Color(0.45f, 0.28f, 0.12f, 1f));
                for (int i = 0; i < 6; i++)
                {
                    float t = i / 5f;
                    float x = Mathf.Lerp(532, 655, t);
                    float y = Mathf.Lerp(96, 36, t);
                    p.Line(x, y - 20, x, y + 18, 5f, new Color(0.30f, 0.19f, 0.09f, 1f));
                }
            }

            if (state >= 29)
            {
                // Coastal garden.
                for (int i = 0; i < 45; i++)
                {
                    float x = 160 + (i * 67 % 420);
                    float y = 82 + (i * 41 % 78);
                    Color flower = i % 3 == 0
                        ? new Color(0.72f, 0.44f, 0.90f, 0.95f)
                        : new Color(0.96f, 0.76f, 0.34f, 0.95f);
                    p.Ellipse(x, y, 4 + i % 3, 4 + i % 3, flower, 1f);
                }
            }

            if (state >= 30)
            {
                // Observatory dome in the background.
                p.RoundedRect(128, 172, 104, 52, 24, new Color(0.80f, 0.84f, 0.82f, 0.92f));
                p.Ellipse(180, 222, 53, 42, new Color(0.62f, 0.72f, 0.76f, 0.95f), 2f);
                p.Line(180, 222, 218, 250, 8f, new Color(0.24f, 0.34f, 0.38f, 1f));
                p.Line(218, 250, 250, 263, 10f, new Color(0.18f, 0.28f, 0.32f, 1f));
            }
        }

        private static void DrawCloud(Painter p, float x, float y, float scale, Color c)
        {
            p.Ellipse(x, y, 62 * scale, 26 * scale, c, 10f);
            p.Ellipse(x + 48 * scale, y + 6 * scale, 48 * scale, 32 * scale, c, 10f);
            p.Ellipse(x - 45 * scale, y + 4 * scale, 43 * scale, 29 * scale, c, 10f);
            p.Ellipse(x + 4 * scale, y + 22 * scale, 45 * scale, 37 * scale, c, 10f);
        }

        private static void DrawBird(Painter p, float x, float y, float scale)
        {
            Color c = new Color(0.06f, 0.10f, 0.14f, 0.72f);
            p.Arc(x - 10 * scale, y, 14 * scale, 0.15f, 2.85f, 2.2f * scale, c);
            p.Arc(x + 10 * scale, y, 14 * scale, 0.30f, 3.00f, 2.2f * scale, c);
        }

        private static void AddRockHighlights(Painter p, float x, float y, float scale)
        {
            Color c = new Color(0.19f, 0.28f, 0.24f, 0.62f);
            p.Line(x, y, x + 110 * scale, y + 180 * scale, 7f * scale, c);
            p.Line(x + 28 * scale, y - 12 * scale, x + 165 * scale, y + 120 * scale, 4f * scale, c);
            p.Line(x + 12 * scale, y + 60 * scale, x + 90 * scale, y + 105 * scale, 3f * scale, c);
        }

        private static Vector2[] StarPoints(float cx, float cy, float outer, float inner, int points, float rotation)
        {
            var result = new Vector2[points * 2];
            for (int i = 0; i < result.Length; i++)
            {
                float radius = i % 2 == 0 ? outer : inner;
                float angle = rotation + i * Mathf.PI / points;
                result[i] = new Vector2(cx + Mathf.Cos(angle) * radius, cy + Mathf.Sin(angle) * radius);
            }
            return result;
        }

        private static Vector2[] Offset(Vector2[] source, float x, float y)
        {
            var result = new Vector2[source.Length];
            for (int i = 0; i < source.Length; i++) result[i] = source[i] + new Vector2(x, y);
            return result;
        }

        private static Color Darken(Color c, float amount)
        {
            return Color.Lerp(c, new Color(0.015f, 0.025f, 0.05f, c.a), Mathf.Clamp01(amount));
        }

        private static Color Lighten(Color c, float amount)
        {
            return Color.Lerp(c, Color.white, Mathf.Clamp01(amount));
        }

        private static void WriteSprite(string path, Painter painter, Vector4 border, float pixelsPerUnit,
            bool readable, int maxSize = 1024)
        {
            File.WriteAllBytes(path, painter.ToTexture().EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.isReadable = readable;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.maxTextureSize = maxSize;
            importer.spritePixelsPerUnit = pixelsPerUnit;
            importer.spriteBorder = border;
            importer.SaveAndReimport();
        }

        private sealed class Painter
        {
            private readonly int width;
            private readonly int height;
            private readonly Color[] pixels;

            public Painter(int width, int height) : this(width, height, Color.black) { }

            public Painter(int width, int height, Color clear)
            {
                this.width = width;
                this.height = height;
                pixels = new Color[width * height];
                for (int i = 0; i < pixels.Length; i++) pixels[i] = clear;
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
                if (outA <= 0.0001f)
                {
                    pixels[index] = Color.clear;
                    return;
                }
                pixels[index] = new Color(
                    (source.r * source.a + destination.r * destination.a * (1f - source.a)) / outA,
                    (source.g * source.a + destination.g * destination.a * (1f - source.a)) / outA,
                    (source.b * source.a + destination.b * destination.a * (1f - source.a)) / outA,
                    outA);
            }

            public void Rect(float x, float y, float w, float h, Color c)
            {
                int minX = Mathf.Max(0, Mathf.FloorToInt(x));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(x + w));
                int minY = Mathf.Max(0, Mathf.FloorToInt(y));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(y + h));
                for (int py = minY; py <= maxY; py++)
                    for (int px = minX; px <= maxX; px++) Blend(px, py, c);
            }

            public void VerticalGradient(Color bottom, Color top, float start01 = 0f, float end01 = 1f, bool overlay = false)
            {
                int minY = Mathf.Clamp(Mathf.RoundToInt(start01 * height), 0, height - 1);
                int maxY = Mathf.Clamp(Mathf.RoundToInt(end01 * height), minY + 1, height);
                for (int y = minY; y < maxY; y++)
                {
                    float t = (y - minY) / (float)Mathf.Max(1, maxY - minY - 1);
                    Color c = Color.Lerp(bottom, top, t);
                    for (int x = 0; x < width; x++)
                    {
                        if (overlay) Blend(x, y, c); else pixels[y * width + x] = c;
                    }
                }
            }

            public void RoundedRect(float x, float y, float w, float h, float radius, Color c)
            {
                int minX = Mathf.Max(0, Mathf.FloorToInt(x));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(x + w));
                int minY = Mathf.Max(0, Mathf.FloorToInt(y));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(y + h));
                float cx0 = x + radius;
                float cx1 = x + w - radius;
                float cy0 = y + radius;
                float cy1 = y + h - radius;
                for (int py = minY; py <= maxY; py++)
                {
                    for (int px = minX; px <= maxX; px++)
                    {
                        float qx = Mathf.Max(cx0 - px, px - cx1, 0f);
                        float qy = Mathf.Max(cy0 - py, py - cy1, 0f);
                        float d = Mathf.Sqrt(qx * qx + qy * qy);
                        float alpha = Mathf.Clamp01(radius + 1f - d);
                        Color cc = c;
                        cc.a *= alpha;
                        Blend(px, py, cc);
                    }
                }
            }

            public void RoundedBorder(float x, float y, float w, float h, float radius, float thickness, Color c)
            {
                RoundedRect(x, y, w, h, radius, c);
                Color erase = new Color(0f, 0f, 0f, 0f);
                // Reconstruct the interior by reducing the border contribution.
                int minX = Mathf.Max(0, Mathf.FloorToInt(x + thickness));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(x + w - thickness));
                int minY = Mathf.Max(0, Mathf.FloorToInt(y + thickness));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(y + h - thickness));
                float innerRadius = Mathf.Max(1f, radius - thickness);
                float cx0 = x + thickness + innerRadius;
                float cx1 = x + w - thickness - innerRadius;
                float cy0 = y + thickness + innerRadius;
                float cy1 = y + h - thickness - innerRadius;
                for (int py = minY; py <= maxY; py++)
                {
                    for (int px = minX; px <= maxX; px++)
                    {
                        float qx = Mathf.Max(cx0 - px, px - cx1, 0f);
                        float qy = Mathf.Max(cy0 - py, py - cy1, 0f);
                        if (Mathf.Sqrt(qx * qx + qy * qy) <= innerRadius - 0.5f)
                        {
                            // Remove only the just-added border color approximately.
                            Color current = pixels[py * width + px];
                            current = Color.Lerp(current, erase, Mathf.Clamp01(c.a * 0.97f));
                            pixels[py * width + px] = current;
                        }
                    }
                }
            }

            public void VerticalGradientRounded(float x, float y, float w, float h, float radius, Color bottom, Color top)
            {
                int minY = Mathf.Max(0, Mathf.FloorToInt(y));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(y + h));
                for (int py = minY; py <= maxY; py++)
                {
                    float t = Mathf.InverseLerp(y, y + h, py);
                    Color row = Color.Lerp(bottom, top, t);
                    RoundedRect(x, py, w, 1.2f, Mathf.Min(radius, 1f), row);
                }
                // Restore smooth outer corners with a translucent mask-like rim.
                RoundedBorder(x, y, w, h, radius, 1.5f, new Color(top.r, top.g, top.b, 0.18f));
            }

            public void HorizontalGradientRounded(float x, float y, float w, float h, float radius, Color left, Color right)
            {
                int minX = Mathf.Max(0, Mathf.FloorToInt(x));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(x + w));
                for (int px = minX; px <= maxX; px++)
                {
                    float t = Mathf.InverseLerp(x, x + w, px);
                    Color column = Color.Lerp(left, right, t);
                    RoundedRect(px, y, 1.2f, h, Mathf.Min(radius, 1f), column);
                }
                RoundedBorder(x, y, w, h, radius, 1.5f, new Color(right.r, right.g, right.b, 0.18f));
            }

            public void Ellipse(float cx, float cy, float rx, float ry, Color c, float feather = 1f)
            {
                int minX = Mathf.Max(0, Mathf.FloorToInt(cx - rx - feather));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(cx + rx + feather));
                int minY = Mathf.Max(0, Mathf.FloorToInt(cy - ry - feather));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(cy + ry + feather));
                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        float dx = (x - cx) / Mathf.Max(0.001f, rx);
                        float dy = (y - cy) / Mathf.Max(0.001f, ry);
                        float d = Mathf.Sqrt(dx * dx + dy * dy);
                        float alpha = Mathf.Clamp01((1f - d) * Mathf.Max(rx, ry) / Mathf.Max(1f, feather));
                        Color cc = c;
                        cc.a *= alpha;
                        Blend(x, y, cc);
                    }
                }
            }

            public void EllipseGradient(float cx, float cy, float rx, float ry, Color edge, Color center)
            {
                int minX = Mathf.Max(0, Mathf.FloorToInt(cx - rx - 2));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(cx + rx + 2));
                int minY = Mathf.Max(0, Mathf.FloorToInt(cy - ry - 2));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(cy + ry + 2));
                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        float dx = (x - cx) / rx;
                        float dy = (y - cy) / ry;
                        float d = Mathf.Sqrt(dx * dx + dy * dy);
                        if (d > 1.02f) continue;
                        Color cc = Color.Lerp(center, edge, Mathf.Clamp01(d));
                        cc = Color.Lerp(cc, Color.white, Mathf.Clamp01((0.25f - dx) * (0.45f + dy) * 0.30f));
                        cc.a *= Mathf.Clamp01((1.02f - d) * 28f);
                        Blend(x, y, cc);
                    }
                }
            }

            public void RotatedEllipse(float cx, float cy, float rx, float ry, float angle, Color c)
            {
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                float extent = Mathf.Max(rx, ry) + 3f;
                int minX = Mathf.Max(0, Mathf.FloorToInt(cx - extent));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(cx + extent));
                int minY = Mathf.Max(0, Mathf.FloorToInt(cy - extent));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(cy + extent));
                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        float dx = x - cx;
                        float dy = y - cy;
                        float lx = dx * cos + dy * sin;
                        float ly = -dx * sin + dy * cos;
                        float d = Mathf.Sqrt(lx * lx / (rx * rx) + ly * ly / (ry * ry));
                        if (d > 1.02f) continue;
                        Color cc = Color.Lerp(Lighten(c, 0.20f), Darken(c, 0.22f), Mathf.Clamp01(d));
                        cc.a *= Mathf.Clamp01((1.02f - d) * 24f);
                        Blend(x, y, cc);
                    }
                }
            }

            public void Ring(float cx, float cy, float rx, float ry, float thickness, Color c)
            {
                int minX = Mathf.Max(0, Mathf.FloorToInt(cx - rx - thickness));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(cx + rx + thickness));
                int minY = Mathf.Max(0, Mathf.FloorToInt(cy - ry - thickness));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(cy + ry + thickness));
                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        float dx = (x - cx) / rx;
                        float dy = (y - cy) / ry;
                        float d = Mathf.Sqrt(dx * dx + dy * dy);
                        float normalizedThickness = thickness / Mathf.Max(rx, ry);
                        float alpha = Mathf.Clamp01((normalizedThickness - Mathf.Abs(d - 1f)) * Mathf.Max(rx, ry));
                        Color cc = c;
                        cc.a *= alpha;
                        Blend(x, y, cc);
                    }
                }
            }

            public void Glow(float cx, float cy, float radius, Color c)
            {
                int minX = Mathf.Max(0, Mathf.FloorToInt(cx - radius));
                int maxX = Mathf.Min(width - 1, Mathf.CeilToInt(cx + radius));
                int minY = Mathf.Max(0, Mathf.FloorToInt(cy - radius));
                int maxY = Mathf.Min(height - 1, Mathf.CeilToInt(cy + radius));
                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        float d = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy)) / radius;
                        if (d > 1f) continue;
                        Color cc = c;
                        cc.a *= Mathf.Pow(1f - d, 2.2f);
                        Blend(x, y, cc);
                    }
                }
            }

            public void Line(float x0, float y0, float x1, float y1, float thickness, Color c)
            {
                float minX = Mathf.Min(x0, x1) - thickness;
                float maxX = Mathf.Max(x0, x1) + thickness;
                float minY = Mathf.Min(y0, y1) - thickness;
                float maxY = Mathf.Max(y0, y1) + thickness;
                Vector2 a = new Vector2(x0, y0);
                Vector2 b = new Vector2(x1, y1);
                Vector2 ab = b - a;
                float lengthSq = Mathf.Max(0.001f, ab.sqrMagnitude);
                for (int y = Mathf.Max(0, Mathf.FloorToInt(minY)); y <= Mathf.Min(height - 1, Mathf.CeilToInt(maxY)); y++)
                {
                    for (int x = Mathf.Max(0, Mathf.FloorToInt(minX)); x <= Mathf.Min(width - 1, Mathf.CeilToInt(maxX)); x++)
                    {
                        Vector2 point = new Vector2(x, y);
                        float t = Mathf.Clamp01(Vector2.Dot(point - a, ab) / lengthSq);
                        float distance = Vector2.Distance(point, a + ab * t);
                        float alpha = Mathf.Clamp01(thickness * 0.5f + 1f - distance);
                        Color cc = c;
                        cc.a *= alpha;
                        Blend(x, y, cc);
                    }
                }
            }

            public void Polygon(IReadOnlyList<Vector2> points, Color c)
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
                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        bool inside = false;
                        for (int i = 0, j = points.Count - 1; i < points.Count; j = i++)
                        {
                            Vector2 pi = points[i];
                            Vector2 pj = points[j];
                            bool intersects = (pi.y > y) != (pj.y > y) &&
                                              x < (pj.x - pi.x) * (y - pi.y) / Mathf.Max(0.0001f, pj.y - pi.y) + pi.x;
                            if (intersects) inside = !inside;
                        }
                        if (inside) Blend(x, y, c);
                    }
                }
            }

            public void Polyline(IReadOnlyList<Vector2> points, float thickness, Color c, bool closed)
            {
                if (points == null || points.Count < 2) return;
                for (int i = 0; i < points.Count - 1; i++)
                    Line(points[i].x, points[i].y, points[i + 1].x, points[i + 1].y, thickness, c);
                if (closed) Line(points[points.Count - 1].x, points[points.Count - 1].y, points[0].x, points[0].y, thickness, c);
            }

            public void Arc(float cx, float cy, float radius, float start, float end, float thickness, Color c)
            {
                const int segments = 42;
                Vector2 previous = new Vector2(cx + Mathf.Cos(start) * radius, cy + Mathf.Sin(start) * radius);
                for (int i = 1; i <= segments; i++)
                {
                    float t = i / (float)segments;
                    float angle = Mathf.Lerp(start, end, t);
                    Vector2 current = new Vector2(cx + Mathf.Cos(angle) * radius, cy + Mathf.Sin(angle) * radius);
                    Line(previous.x, previous.y, current.x, current.y, thickness, c);
                    previous = current;
                }
            }

            public void Wave(float x, float y, float length, float amplitude, float wavelength, float phase, float thickness, Color c)
            {
                float previousX = x;
                float previousY = y + Mathf.Sin(phase) * amplitude;
                for (float px = x + 4f; px <= x + length; px += 4f)
                {
                    float py = y + Mathf.Sin((px - x) / wavelength * Mathf.PI * 2f + phase) * amplitude;
                    Line(previousX, previousY, px, py, thickness, c);
                    previousX = px;
                    previousY = py;
                }
            }

            public void Beam(Vector2 origin, Vector2 end, float endWidth, Color c)
            {
                Vector2 direction = (end - origin).normalized;
                Vector2 normal = new Vector2(-direction.y, direction.x) * endWidth * 0.5f;
                Polygon(new[] { origin, end + normal, end - normal }, c);
            }

            public void CurvePath(IReadOnlyList<Vector2> points, float thickness, Color c)
            {
                if (points == null || points.Count < 2) return;
                var sampled = new List<Vector2>();
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Vector2 a = points[Mathf.Max(0, i - 1)];
                    Vector2 b = points[i];
                    Vector2 d = points[i + 1];
                    Vector2 e = points[Mathf.Min(points.Count - 1, i + 2)];
                    for (int s = 0; s < 12; s++)
                    {
                        float t = s / 12f;
                        Vector2 value = 0.5f * ((2f * b) + (-a + d) * t +
                            (2f * a - 5f * b + 4f * d - e) * t * t +
                            (-a + 3f * b - 3f * d + e) * t * t * t);
                        sampled.Add(value);
                    }
                }
                sampled.Add(points[points.Count - 1]);
                Polyline(sampled, thickness, c, false);
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
        }
    }
}
