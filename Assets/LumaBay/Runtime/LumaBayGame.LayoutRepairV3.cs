using System;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private float nextStableInterfacePass;
        private bool legacyVisualLayersDisabled;

        private static readonly Color PanelDark = new Color(0.018f, 0.085f, 0.135f, 0.96f);
        private static readonly Color PanelSoft = new Color(0.025f, 0.125f, 0.185f, 0.95f);
        private static readonly Color ButtonDark = new Color(0.035f, 0.22f, 0.30f, 0.98f);
        private static readonly Color ButtonPrimary = new Color(0.025f, 0.57f, 0.62f, 1f);
        private static readonly Color GoldText = new Color(1f, 0.84f, 0.42f, 1f);

        private void ApplyStableInterfaceV4()
        {
            if (screenRoot == null || Time.unscaledTime < nextStableInterfacePass) return;
            nextStableInterfacePass = Time.unscaledTime + 0.12f;

            DisableLegacyVisualLayersV5();
            ApplyStableBackgroundV5();
            NormalizeTransientLayersV5();

            if (FindInRoot("MainHero") != null) RepairMainMenuV5();
            else if (FindInRoot("LighthouseMetaCard") != null) RepairMapV5();
            else if (FindInRoot("SettingsHeader") != null) RepairSettingsV5();
            else if (FindInRoot("LevelScroll") != null) RepairLevelSelectorV5();
            else if (FindInRoot("BoardFrame") != null || FindInRoot("BoardGrid") != null) RepairGameplayV5();
        }

        private void DisableLegacyVisualLayersV5()
        {
            if (legacyVisualLayersDisabled) return;
            legacyVisualLayersDisabled = true;

            LumaBayPremiumCompositionV2 composition = FindFirstObjectByType<LumaBayPremiumCompositionV2>();
            if (composition != null) composition.enabled = false;

            foreach (LumaBayPanelToneGuard guard in FindObjectsByType<LumaBayPanelToneGuard>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (guard != null) guard.enabled = false;
            foreach (PanelShadowLayoutGuard guard in FindObjectsByType<PanelShadowLayoutGuard>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (guard != null) guard.enabled = false;
        }

        private void ApplyStableBackgroundV5()
        {
            Transform backgroundTransform = canvas != null ? canvas.transform.Find("Background") : null;
            Image background = backgroundTransform != null ? backgroundTransform.GetComponent<Image>() : null;
            if (background == null) return;

            Sprite selected;
            if (FindInRoot("BoardFrame") != null || FindInRoot("BoardGrid") != null)
                selected = LumaBayArtPackV2.GameplayBackground;
            else if (FindInRoot("MapHeader") != null || FindInRoot("LevelScroll") != null)
                selected = LumaBayArtPackV2.MapBackground;
            else if (FindInRoot("SettingsHeader") != null)
                selected = LumaBayArtPackV2.StoryBackground;
            else
                selected = LumaBayArtPackV2.MainMenuBackground;

            if (selected != null) background.sprite = selected;
            background.color = Color.white;
            background.preserveAspect = false;
        }

        private void NormalizeTransientLayersV5()
        {
            if (canvas == null) return;
            Transform modal = canvas.transform.Find("ModalOverlay");
            if (modal == null) return;

            modal.SetAsLastSibling();
            RectTransform panel = modal.Find("ModalPanel") as RectTransform;
            if (panel == null) return;

            panel.anchorMin = new Vector2(0.075f, 0.235f);
            panel.anchorMax = new Vector2(0.925f, 0.765f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;
            panel.localScale = Vector3.one;
            SetSolidPanelV5(panel.GetComponent<Image>(), PanelDark, "v5_modal", 28);

            VerticalLayoutGroup layout = panel.GetComponent<VerticalLayoutGroup>();
            if (layout != null)
            {
                layout.padding = new RectOffset(34, 34, 30, 30);
                layout.spacing = 16f;
            }

            Button[] buttons = panel.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++) StyleButtonV5(buttons[i], i == 0, i == 0 ? 82f : 68f);
        }

        private void RepairMainMenuV5()
        {
            RectTransform root = FindInRoot("VerticalScreen") as RectTransform;
            if (root == null) return;
            VerticalLayoutGroup layout = root.GetComponent<VerticalLayoutGroup>();
            if (layout != null)
            {
                layout.padding = new RectOffset(32, 32, 24, 24);
                layout.spacing = 14f;
            }

            SetPreferredV5(FindInRoot("LogoBlock") as RectTransform, 112f);
            RectTransform hero = FindInRoot("MainHero") as RectTransform;
            SetPreferredV5(hero, 510f);
            if (hero != null) SetSolidPanelV5(hero.GetComponent<Image>(), PanelDark, "v5_main_hero", 28);

            RectTransform row = FindInRoot("MainSecondaryRow") as RectTransform;
            SetPreferredV5(row, 102f);
            HorizontalLayoutGroup rowLayout = row != null ? row.GetComponent<HorizontalLayoutGroup>() : null;
            if (rowLayout != null)
            {
                rowLayout.spacing = 16f;
                rowLayout.padding = new RectOffset(0, 0, 4, 4);
                rowLayout.childControlHeight = true;
                rowLayout.childForceExpandHeight = true;
            }

            foreach (Button button in root.GetComponentsInChildren<Button>(true))
            {
                Text label = button.GetComponentInChildren<Text>(true);
                if (label == null) continue;
                string value = StripRichTextV5(label.text).ToUpperInvariant();
                if (value.Contains("ИГРАТЬ") || value == "PLAY") StyleButtonV5(button, true, 102f);
                else if (value.Contains("УРОВНИ") || value.Contains("LEVELS") || value.Contains("НАСТРОЙКИ") || value.Contains("SETTINGS"))
                    StyleButtonV5(button, false, 94f);
            }

            foreach (Text text in root.GetComponentsInChildren<Text>(true))
            {
                if (text.text != null && text.text.StartsWith("LUMA BAY", StringComparison.OrdinalIgnoreCase))
                {
                    text.color = GoldText;
                    text.fontSize = 58;
                }
            }
        }

        private void RepairMapV5()
        {
            RectTransform root = FindInRoot("VerticalScreen") as RectTransform;
            if (root == null) return;
            VerticalLayoutGroup layout = root.GetComponent<VerticalLayoutGroup>();
            if (layout != null)
            {
                layout.padding = new RectOffset(24, 24, 18, 20);
                layout.spacing = 12f;
            }

            StylePanelByNameV5("MapHeader", PanelDark, 90f);
            StylePanelByNameV5("LighthouseMetaCard", PanelDark, 500f);
            StylePanelByNameV5("TaskCard", PanelDark, 168f);
            SetPreferredV5(FindInRoot("MetaProgressRow") as RectTransform, 46f);

            RectTransform progressTrack = FindInRoot("LongProgressTrack") as RectTransform;
            if (progressTrack != null) SetSolidPanelV5(progressTrack.GetComponent<Image>(), new Color(0.005f, 0.035f, 0.055f, 0.96f), "v5_progress", 12);

            foreach (Button button in root.GetComponentsInChildren<Button>(true))
            {
                Text label = button.GetComponentInChildren<Text>(true);
                if (label == null) continue;
                string value = StripRichTextV5(label.text).ToUpperInvariant();
                if (value.Contains("НАЧАТЬ") || value.Contains("START")) StyleButtonV5(button, true, 96f);
                else if (value.Contains("ВОССТ") || value.Contains("RESTORE") || value.Contains("ОТКРОЕТСЯ") || value.Contains("UNLOCKS")) StyleButtonV5(button, false, 82f);
                else if (value.Contains("УРОВНИ") || value.Contains("LEVELS")) StyleButtonV5(button, false, 72f);
            }
        }

        private void RepairSettingsV5()
        {
            RectTransform root = FindInRoot("VerticalScreen") as RectTransform;
            if (root == null) return;
            VerticalLayoutGroup screenLayout = root.GetComponent<VerticalLayoutGroup>();
            if (screenLayout != null)
            {
                screenLayout.padding = new RectOffset(28, 28, 24, 24);
                screenLayout.spacing = 16f;
            }

            StylePanelByNameV5("SettingsHeader", PanelDark, 94f);
            RectTransform card = FindInRoot("SettingsCard") as RectTransform;
            SetPreferredV5(card, 610f);
            if (card != null) SetSolidPanelV5(card.GetComponent<Image>(), PanelDark, "v5_settings_card", 28);

            VerticalLayoutGroup cardLayout = card != null ? card.GetComponent<VerticalLayoutGroup>() : null;
            if (cardLayout != null)
            {
                cardLayout.padding = new RectOffset(28, 28, 30, 30);
                cardLayout.spacing = 18f;
            }

            Button[] buttons = card != null ? card.GetComponentsInChildren<Button>(true) : Array.Empty<Button>();
            foreach (Button button in buttons)
            {
                Text label = button.GetComponentInChildren<Text>(true);
                bool reset = label != null && (label.text.Contains("Сброс", StringComparison.OrdinalIgnoreCase) || label.text.Contains("Reset", StringComparison.OrdinalIgnoreCase));
                StyleButtonV5(button, false, reset ? 74f : 88f, reset ? new Color(0.38f, 0.10f, 0.13f, 0.98f) : ButtonDark);
            }
        }

        private void RepairLevelSelectorV5()
        {
            RectTransform header = FindInRoot("LevelsHeader") as RectTransform;
            if (header != null)
            {
                header.anchorMin = new Vector2(0.03f, 0.905f);
                header.anchorMax = new Vector2(0.97f, 0.985f);
                header.offsetMin = Vector2.zero;
                header.offsetMax = Vector2.zero;
                SetSolidPanelV5(header.GetComponent<Image>(), PanelDark, "v5_levels_header", 22);
            }

            RectTransform scrollRoot = FindInRoot("LevelScroll") as RectTransform;
            if (scrollRoot == null) return;
            scrollRoot.anchorMin = new Vector2(0.03f, 0.025f);
            scrollRoot.anchorMax = new Vector2(0.97f, 0.89f);
            scrollRoot.offsetMin = Vector2.zero;
            scrollRoot.offsetMax = Vector2.zero;
            SetSolidPanelV5(scrollRoot.GetComponent<Image>(), new Color(0.008f, 0.052f, 0.082f, 0.94f), "v5_level_scroll", 28);

            ScrollRect scroll = scrollRoot.GetComponent<ScrollRect>();
            RectTransform viewport = scrollRoot.Find("Viewport") as RectTransform;
            RectTransform content = viewport != null ? viewport.Find("Content") as RectTransform : null;
            if (scroll == null || viewport == null || content == null) return;

            Mask oldMask = viewport.GetComponent<Mask>();
            if (oldMask != null) oldMask.enabled = false;
            if (viewport.GetComponent<RectMask2D>() == null) viewport.gameObject.AddComponent<RectMask2D>();
            Image viewportImage = viewport.GetComponent<Image>();
            if (viewportImage != null) viewportImage.color = new Color(1f, 1f, 1f, 0.001f);

            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.offsetMin = new Vector2(18f, 18f);
            viewport.offsetMax = new Vector2(-18f, -18f);
            viewport.localScale = Vector3.one;
            content.gameObject.SetActive(true);
            content.localScale = Vector3.one;

            const int columns = 3;
            const float gap = 14f;
            float width = viewport.rect.width > 300f ? viewport.rect.width : 640f;
            float cellWidth = Mathf.Floor((width - gap * 2f - 28f) / columns);
            float cellHeight = 148f;
            int rows = Mathf.CeilToInt(Mathf.Max(1, content.childCount) / (float)columns);

            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = new Vector2(0f, rows * cellHeight + Mathf.Max(0, rows - 1) * gap + 32f);

            GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
            if (grid == null) grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.padding = new RectOffset(14, 14, 14, 18);
            grid.spacing = new Vector2(gap, gap);
            grid.cellSize = new Vector2(cellWidth, cellHeight);
            grid.childAlignment = TextAnchor.UpperCenter;

            CanvasGroup contentGroup = content.GetComponent<CanvasGroup>();
            if (contentGroup == null) contentGroup = content.gameObject.AddComponent<CanvasGroup>();
            contentGroup.alpha = 1f;
            contentGroup.interactable = true;
            contentGroup.blocksRaycasts = true;

            for (int i = 0; i < content.childCount; i++)
            {
                Transform card = content.GetChild(i);
                card.gameObject.SetActive(true);
                card.localScale = Vector3.one;
                card.localRotation = Quaternion.identity;
                UiEntranceMotion motion = card.GetComponent<UiEntranceMotion>();
                if (motion != null) motion.enabled = false;

                CanvasGroup group = card.GetComponent<CanvasGroup>();
                if (group == null) group = card.gameObject.AddComponent<CanvasGroup>();
                group.alpha = 1f;
                group.interactable = true;
                group.blocksRaycasts = true;

                Button button = card.GetComponent<Button>();
                bool unlocked = button == null || button.interactable;
                SetSolidPanelV5(card.GetComponent<Image>(), unlocked ? PanelSoft : new Color(0.025f, 0.06f, 0.08f, 0.95f), "v5_level_card_" + i, 22);

                Text label = card.GetComponentInChildren<Text>(true);
                if (label != null)
                {
                    label.gameObject.SetActive(true);
                    label.color = unlocked ? Color.white : new Color(0.55f, 0.62f, 0.66f, 1f);
                    label.fontSize = 25;
                    label.resizeTextForBestFit = true;
                    label.resizeTextMinSize = 15;
                    label.resizeTextMaxSize = 25;
                    label.rectTransform.anchorMin = new Vector2(0.06f, 0.03f);
                    label.rectTransform.anchorMax = new Vector2(0.94f, 0.46f);
                    label.rectTransform.offsetMin = Vector2.zero;
                    label.rectTransform.offsetMax = Vector2.zero;
                }

                RectTransform icon = card.Find("LevelTargetIcon") as RectTransform;
                if (icon != null)
                {
                    icon.gameObject.SetActive(unlocked);
                    icon.anchorMin = new Vector2(0.30f, 0.48f);
                    icon.anchorMax = new Vector2(0.70f, 0.91f);
                    icon.offsetMin = Vector2.zero;
                    icon.offsetMax = Vector2.zero;
                }
            }

            scroll.content = content;
            scroll.viewport = viewport;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }

        private void RepairGameplayV5()
        {
            StylePanelByNameV5("GameplayHeader", PanelDark, 82f);
            StylePanelByNameV5("TopHud", PanelDark, 82f);
            StylePanelByNameV5("GoalsPanel", PanelDark, 108f);

            RectTransform boardFrame = FindInRoot("BoardFrame") as RectTransform;
            if (boardFrame != null) SetSolidPanelV5(boardFrame.GetComponent<Image>(), new Color(0.005f, 0.045f, 0.07f, 0.98f), "v5_board", 24);

            RectTransform tray = FindInRoot("BoosterTray") as RectTransform;
            if (tray == null) tray = FindInRoot("BoosterRow") as RectTransform;
            if (tray == null) tray = FindInRoot("BoostersRow") as RectTransform;
            if (tray == null) return;

            SetPreferredV5(tray, 154f);
            SetSolidPanelV5(tray.GetComponent<Image>(), new Color(0.008f, 0.052f, 0.08f, 0.94f), "v5_booster_tray", 24);
            HorizontalLayoutGroup row = tray.GetComponent<HorizontalLayoutGroup>();
            if (row != null)
            {
                row.padding = new RectOffset(10, 10, 10, 10);
                row.spacing = 9f;
                row.childAlignment = TextAnchor.MiddleCenter;
                row.childControlWidth = true;
                row.childControlHeight = true;
                row.childForceExpandWidth = true;
                row.childForceExpandHeight = false;
            }

            foreach (Button booster in tray.GetComponentsInChildren<Button>(true))
            {
                LayoutElement element = booster.GetComponent<LayoutElement>();
                if (element == null) element = booster.gameObject.AddComponent<LayoutElement>();
                element.minWidth = 0f;
                element.preferredWidth = 112f;
                element.flexibleWidth = 1f;
                element.minHeight = 128f;
                element.preferredHeight = 128f;
                element.flexibleHeight = 0f;
                SetSolidPanelV5(booster.GetComponent<Image>(), ButtonDark, "v5_booster_" + booster.GetInstanceID(), 20);

                foreach (Text label in booster.GetComponentsInChildren<Text>(true))
                {
                    bool price = label.text != null && label.text.Contains("◆");
                    bool shortIcon = !price && !string.IsNullOrEmpty(label.text) && label.text.Length <= 4;
                    label.gameObject.SetActive(price || shortIcon);
                    label.color = price ? GoldText : Color.white;
                    label.fontSize = price ? 14 : 27;
                }
            }
        }

        private void StylePanelByNameV5(string name, Color color, float height)
        {
            RectTransform rect = FindInRoot(name) as RectTransform;
            if (rect == null) return;
            SetPreferredV5(rect, height);
            SetSolidPanelV5(rect.GetComponent<Image>(), color, "v5_" + name, 24);
        }

        private static void StyleButtonV5(Button button, bool primary, float height, Color? overrideColor = null)
        {
            if (button == null) return;
            SetPreferredV5(button.transform as RectTransform, height);
            Color color = overrideColor ?? (primary ? ButtonPrimary : ButtonDark);
            SetSolidPanelV5(button.GetComponent<Image>(), color, "v5_button_" + button.GetInstanceID(), 22);

            Text label = button.GetComponentInChildren<Text>(true);
            if (label != null)
            {
                label.gameObject.SetActive(true);
                label.color = Color.white;
                label.fontSize = primary ? 29 : 23;
                label.resizeTextForBestFit = true;
                label.resizeTextMinSize = 17;
                label.resizeTextMaxSize = primary ? 29 : 23;
            }
        }

        private static void SetSolidPanelV5(Image image, Color color, string key, int radius)
        {
            if (image == null) return;
            image.sprite = ProceduralArt.Rounded(key, Color.white, radius);
            image.type = Image.Type.Sliced;
            image.color = color;
            image.preserveAspect = false;
        }

        private static void SetPreferredV5(RectTransform rect, float height)
        {
            if (rect == null) return;
            LayoutElement element = rect.GetComponent<LayoutElement>();
            if (element == null) element = rect.gameObject.AddComponent<LayoutElement>();
            element.minHeight = height;
            element.preferredHeight = height;
            element.flexibleHeight = 0f;
        }

        private static string StripRichTextV5(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var builder = new System.Text.StringBuilder(value.Length);
            bool insideTag = false;
            foreach (char character in value)
            {
                if (character == '<') { insideTag = true; continue; }
                if (character == '>') { insideTag = false; continue; }
                if (!insideTag) builder.Append(character);
            }
            return builder.ToString().Trim();
        }
    }
}
