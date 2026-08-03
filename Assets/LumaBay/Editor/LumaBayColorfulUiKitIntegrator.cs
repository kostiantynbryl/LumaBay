using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LumaBay.Editor
{
    public sealed class LumaBayColorfulUiKitIntegrator : EditorWindow
    {
        private const string ThemeAssetPath = "Assets/LumaBay/Resources/LumaBayColorfulUiTheme.asset";

        private readonly List<Sprite> candidates = new List<Sprite>();
        private Vector2 scroll;
        private string status = "Import Colorful UI Kit from Package Manager, then scan and save the theme.";

        private Sprite panelLarge;
        private Sprite headerPanel;
        private Sprite mediumPanel;
        private Sprite compactPanel;
        private Sprite goalsPanel;
        private Sprite taskPanel;
        private Sprite bottomNavigation;
        private Sprite boosterTray;
        private Sprite primaryButton;
        private Sprite secondaryButton;
        private Sprite compactButton;
        private Sprite backButton;
        private Sprite progressTrack;
        private Sprite progressFill;
        private Sprite progressDecor;
        private Sprite iconPlate;

        [MenuItem("Luma Bay/UI/Configure Colorful UI Kit", priority = 12)]
        public static void Open()
        {
            LumaBayColorfulUiKitIntegrator window = GetWindow<LumaBayColorfulUiKitIntegrator>();
            window.titleContent = new GUIContent("Colorful UI Kit");
            window.minSize = new Vector2(510f, 650f);
            window.Show();
        }

        [MenuItem("Luma Bay/UI/Auto-detect and save Colorful UI Kit", priority = 13)]
        public static void AutoDetectAndSaveMenu()
        {
            LumaBayColorfulUiKitIntegrator window = CreateInstance<LumaBayColorfulUiKitIntegrator>();
            window.ScanSprites();
            window.AutoDetect();
            window.SaveTheme();
            DestroyImmediate(window);
        }

        private void OnEnable()
        {
            LoadExistingTheme();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox(
                "1. Add Colorful UI Kit to My Assets and import it through Window → Package Manager → My Assets.\n" +
                "2. Press Scan, then Auto detect.\n" +
                "3. Check the selected sprites and press Save theme.\n\n" +
                "The Asset Store files must remain local and must not be committed to the public repository.",
                MessageType.Info);

            EditorGUILayout.Space(4f);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Scan imported sprites", GUILayout.Height(32f))) ScanSprites();
                if (GUILayout.Button("Auto detect", GUILayout.Height(32f))) AutoDetect();
                if (GUILayout.Button("Save theme", GUILayout.Height(32f))) SaveTheme();
            }

            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField($"Candidates: {candidates.Count}", EditorStyles.miniBoldLabel);
            EditorGUILayout.HelpBox(status, IsComplete() ? MessageType.Info : MessageType.Warning);

            scroll = EditorGUILayout.BeginScrollView(scroll);
            DrawSection("Panels");
            panelLarge = DrawSprite("Large panel", panelLarge);
            headerPanel = DrawSprite("Header panel", headerPanel);
            mediumPanel = DrawSprite("Medium panel", mediumPanel);
            compactPanel = DrawSprite("Compact panel", compactPanel);
            goalsPanel = DrawSprite("Goals panel", goalsPanel);
            taskPanel = DrawSprite("Task panel", taskPanel);
            bottomNavigation = DrawSprite("Bottom navigation", bottomNavigation);
            boosterTray = DrawSprite("Booster tray", boosterTray);

            DrawSection("Buttons");
            primaryButton = DrawSprite("Primary / Play", primaryButton);
            secondaryButton = DrawSprite("Secondary", secondaryButton);
            compactButton = DrawSprite("Compact / round", compactButton);
            backButton = DrawSprite("Back button", backButton);

            DrawSection("Progress and decoration");
            progressTrack = DrawSprite("Progress track", progressTrack);
            progressFill = DrawSprite("Progress fill", progressFill);
            progressDecor = DrawSprite("Progress decoration", progressDecor);
            iconPlate = DrawSprite("Icon plate", iconPlate);
            EditorGUILayout.EndScrollView();
        }

        private static void DrawSection(string title)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        private static Sprite DrawSprite(string label, Sprite value)
        {
            return (Sprite)EditorGUILayout.ObjectField(label, value, typeof(Sprite), false);
        }

        private void ScanSprites()
        {
            candidates.Clear();
            var seen = new HashSet<int>();
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets" });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path) || path.StartsWith("Assets/LumaBay/", StringComparison.OrdinalIgnoreCase))
                    continue;

                foreach (UnityEngine.Object asset in AssetDatabase.LoadAllAssetsAtPath(path))
                {
                    if (!(asset is Sprite sprite) || !seen.Add(sprite.GetInstanceID())) continue;
                    candidates.Add(sprite);
                }
            }

            candidates.Sort((left, right) => string.CompareOrdinal(AssetDatabase.GetAssetPath(left), AssetDatabase.GetAssetPath(right)));
            status = candidates.Count == 0
                ? "No external sprites were found. Import Colorful UI Kit first."
                : $"Found {candidates.Count} external sprites. Auto detection prefers paths containing Colorful, UI Kit or Smashy.";
            Repaint();
        }

        private void AutoDetect()
        {
            if (candidates.Count == 0) ScanSprites();
            if (candidates.Count == 0) return;

            panelLarge = Pick(new[] { "panel", "window", "frame", "background", "popup" }, 1.15f, 2.5f, true);
            headerPanel = Pick(new[] { "header", "title", "top", "banner" }, 2.0f, 6.5f, true) ?? panelLarge;
            mediumPanel = Pick(new[] { "panel", "box", "card", "frame" }, 1.2f, 3.4f, true) ?? panelLarge;
            compactPanel = Pick(new[] { "small", "compact", "badge", "label", "plate" }, 1.1f, 3.8f, true) ?? mediumPanel;
            goalsPanel = Pick(new[] { "goal", "mission", "task", "panel", "card" }, 1.5f, 5.0f, true) ?? mediumPanel;
            taskPanel = Pick(new[] { "task", "quest", "mission", "panel", "card" }, 1.2f, 4.5f, true) ?? goalsPanel;
            bottomNavigation = Pick(new[] { "bottom", "navigation", "navbar", "menu", "footer" }, 3.0f, 10f, true) ?? headerPanel;
            boosterTray = Pick(new[] { "toolbar", "tray", "inventory", "bottom", "panel" }, 2.5f, 8f, true) ?? bottomNavigation;

            primaryButton = Pick(new[] { "button", "btn", "play", "primary", "green", "blue" }, 2.0f, 7.5f, true);
            secondaryButton = Pick(new[] { "button", "btn", "secondary", "orange", "pink", "yellow" }, 2.0f, 7.5f, true, primaryButton) ?? primaryButton;
            compactButton = Pick(new[] { "round", "circle", "small", "icon", "button" }, 0.75f, 1.35f, true);
            backButton = Pick(new[] { "back", "arrow", "left", "previous" }, 0.75f, 1.5f, false) ?? compactButton;

            progressTrack = Pick(new[] { "progress", "loading", "slider", "bar", "track" }, 3.0f, 14f, true);
            progressFill = Pick(new[] { "fill", "progress", "loading", "bar" }, 3.0f, 14f, true, progressTrack);
            progressDecor = Pick(new[] { "progress", "decor", "star", "shine", "sparkle" }, 0.75f, 3.5f, false) ?? compactPanel;
            iconPlate = Pick(new[] { "icon", "plate", "badge", "round", "circle" }, 0.75f, 1.5f, true) ?? compactButton;

            status = IsComplete()
                ? "Required roles were detected. Review the previews, then save the theme."
                : "Auto detection could not fill all required roles. Assign the missing fields manually.";
            Repaint();
        }

        private Sprite Pick(string[] keywords, float minAspect, float maxAspect, bool requireBorder, Sprite excluded = null)
        {
            Sprite best = null;
            float bestScore = float.MinValue;

            foreach (Sprite sprite in candidates)
            {
                if (sprite == null || sprite == excluded) continue;
                string path = AssetDatabase.GetAssetPath(sprite);
                string haystack = (path + "/" + sprite.name).ToLowerInvariant();
                float width = Mathf.Max(1f, sprite.rect.width);
                float height = Mathf.Max(1f, sprite.rect.height);
                float aspect = width / height;
                float score = 0f;

                if (haystack.Contains("colorful")) score += 90f;
                if (haystack.Contains("ui kit") || haystack.Contains("uikit")) score += 65f;
                if (haystack.Contains("smashy")) score += 45f;
                if (haystack.Contains("demo") || haystack.Contains("sample")) score -= 8f;

                for (int i = 0; i < keywords.Length; i++)
                {
                    if (haystack.Contains(keywords[i])) score += 28f - i * 2f;
                }

                if (aspect >= minAspect && aspect <= maxAspect) score += 35f;
                else score -= Mathf.Abs(aspect - Mathf.Clamp(aspect, minAspect, maxAspect)) * 15f;

                float area = width * height;
                score += Mathf.Clamp(Mathf.Log10(Mathf.Max(10f, area)) * 3f, 0f, 18f);

                if (requireBorder && sprite.border.sqrMagnitude > 0.01f) score += 30f;
                if (requireBorder && sprite.border.sqrMagnitude <= 0.01f) score -= 12f;

                if (score <= bestScore) continue;
                bestScore = score;
                best = sprite;
            }

            return best;
        }

        private void SaveTheme()
        {
            if (!IsComplete())
            {
                status = "Large panel and all three button roles are required before saving.";
                Repaint();
                return;
            }

            Directory.CreateDirectory("Assets/LumaBay/Resources");
            LumaBayColorfulUiTheme theme = AssetDatabase.LoadAssetAtPath<LumaBayColorfulUiTheme>(ThemeAssetPath);
            if (theme == null)
            {
                theme = CreateInstance<LumaBayColorfulUiTheme>();
                AssetDatabase.CreateAsset(theme, ThemeAssetPath);
            }

            theme.panelLarge = panelLarge;
            theme.headerPanel = headerPanel ?? panelLarge;
            theme.mediumPanel = mediumPanel ?? panelLarge;
            theme.compactPanel = compactPanel ?? mediumPanel ?? panelLarge;
            theme.goalsPanel = goalsPanel ?? mediumPanel ?? panelLarge;
            theme.taskPanel = taskPanel ?? goalsPanel ?? mediumPanel ?? panelLarge;
            theme.bottomNavigation = bottomNavigation ?? headerPanel ?? panelLarge;
            theme.boosterTray = boosterTray ?? bottomNavigation ?? panelLarge;
            theme.primaryButton = primaryButton;
            theme.secondaryButton = secondaryButton;
            theme.compactButton = compactButton;
            theme.backButton = backButton ?? compactButton;
            theme.progressTrack = progressTrack;
            theme.progressFill = progressFill;
            theme.progressDecor = progressDecor;
            theme.iconPlate = iconPlate ?? compactButton;

            EditorUtility.SetDirty(theme);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            LumaBayColorfulUi.Reload();

            Selection.activeObject = theme;
            EditorGUIUtility.PingObject(theme);
            status = $"Colorful UI theme saved to {ThemeAssetPath}. It will override the legacy UI sprites at runtime.";
            Debug.Log(status);
            Repaint();
        }

        private void LoadExistingTheme()
        {
            LumaBayColorfulUiTheme theme = AssetDatabase.LoadAssetAtPath<LumaBayColorfulUiTheme>(ThemeAssetPath);
            if (theme == null) return;

            panelLarge = theme.panelLarge;
            headerPanel = theme.headerPanel;
            mediumPanel = theme.mediumPanel;
            compactPanel = theme.compactPanel;
            goalsPanel = theme.goalsPanel;
            taskPanel = theme.taskPanel;
            bottomNavigation = theme.bottomNavigation;
            boosterTray = theme.boosterTray;
            primaryButton = theme.primaryButton;
            secondaryButton = theme.secondaryButton;
            compactButton = theme.compactButton;
            backButton = theme.backButton;
            progressTrack = theme.progressTrack;
            progressFill = theme.progressFill;
            progressDecor = theme.progressDecor;
            iconPlate = theme.iconPlate;
            status = "Existing Colorful UI theme loaded.";
        }

        private bool IsComplete()
        {
            return panelLarge != null && primaryButton != null && secondaryButton != null && compactButton != null;
        }
    }
}
