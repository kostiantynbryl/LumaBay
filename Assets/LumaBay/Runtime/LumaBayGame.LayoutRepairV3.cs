using System;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private float nextStableInterfacePass;
        private bool legacyVisualLayersDisabled;

        private static readonly Color PanelDark = new Color(0.018f, 0.085f, 0.135f, 0.97f);
        private static readonly Color PanelSoft = new Color(0.025f, 0.135f, 0.195f, 0.97f);
        private static readonly Color ButtonDark = new Color(0.025f, 0.20f, 0.28f, 0.99f);
        private static readonly Color ButtonPrimary = new Color(0.02f, 0.60f, 0.66f, 1f);
        private static readonly Color GoldText = new Color(1f, 0.84f, 0.42f, 1f);
        private static readonly Color MutedText = new Color(0.66f, 0.76f, 0.80f, 1f);

        private void ApplyStableInterfaceV4()
        {
            if (screenRoot == null || Time.unscaledTime < nextStableInterfacePass) return;
            nextStableInterfacePass = Time.unscaledTime + 0.12f;

            DisableLegacyVisualLayersV6();
            ApplyStableBackgroundV6();
            NormalizeTransientLayersV6();

            if (FindInRoot("MainHero") != null) RepairMainMenuV6();
            else if (FindInRoot("LighthouseMetaCard") != null) RepairMapV6();
            else if (FindInRoot("SettingsHeader") != null) RepairSettingsV6();
            else if (FindInRoot("LevelScroll") != null) RepairLevelSelectorV6();
            else if (FindInRoot("BoardFrame") != null || FindInRoot("BoardGrid") != null) RepairGameplayV6();
        }

        private void DisableLegacyVisualLayersV6()
        {
            if (legacyVisualLayersDisabled) return;
            legacyVisualLayersDisabled = true;
            LumaBayPremiumCompositionV2 composition = FindFirstObjectByType<LumaBayPremiumCompositionV2>();
            if (composition != null) composition.enabled = false;
        }

        private void ApplyStableBackgroundV6()
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

        private void NormalizeTransientLayersV6()
        {
            if (canvas == null) return;
            Transform modal = canvas.transform.Find("ModalOverlay");
            if (modal == null) return;

            modal.SetAsLastSibling();
            RectTransform panel = modal.Find("ModalPanel") as RectTransform;
            if (panel == null) return;

            panel.anchorMin = new Vector2(0.075f, 0.22f);
            panel.anchorMax = new Vector2(0.925f, 0.78f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;
            panel.localScale = Vector3.one;
            SetSolidPanelV6(panel.GetComponent<Image>(), PanelDark, "v6_modal", 30);

            VerticalLayoutGroup layout = panel.GetComponent<VerticalLayoutGroup>();
            if (layout != null)
            {
                layout.padding = new RectOffset(34, 34, 30, 30);
                layout.spacing = 14f;
            }

            Button[] buttons = panel.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++) StyleButtonV6(buttons[i], i == 0, i == 0 ? 84f : 68f);
        }

        private void RepairMainMenuV6()
        {
            RectTransform root = FindInRoot("VerticalScreen") as RectTransform;
            if (root == null) return;
            VerticalLayoutGroup layout = root.GetComponent<VerticalLayoutGroup>();
            if (layout != null)
            {
                layout.padding = new RectOffset(32, 32, 20, 20);
                layout.spacing = 12f;
            }

            SetPreferredV6(FindInRoot("LogoBlock") as RectTransform, 98f);
            RectTransform hero = FindInRoot("MainHero") as RectTransform;
            SetPreferredV6(hero, 480f);
            if (hero != null) SetSolidPanelV6(hero.GetComponent<Image>(), PanelDark, "v6_main_hero", 30);

            RectTransform row = FindInRoot("MainSecondaryRow") as RectTransform;
            SetPreferredV6(row, 92f);
            HorizontalLayoutGroup rowLayout = row != null ? row.GetComponent<HorizontalLayoutGroup>() : null;
            if (rowLayout != null)
            {
                rowLayout.spacing = 14f;
                rowLayout.padding = new RectOffset(0, 0, 2, 2);
                rowLayout.childControlHeight = true;
                rowLayout.childForceExpandHeight = true;
            }

            foreach (Button button in root.GetComponentsInChildren<Button>(true))
            {
                Text label = button.GetComponentInChildren<Text>(true);
                if (label == null) continue;
                string value = StripRichTextV6(label.text).ToUpperInvariant();
                if (value.Contains("ИГРАТЬ") || value == "PLAY") StyleButtonV6(button, true, 96f);
                else if (value.Contains("УРОВНИ") || value.Contains("LEVELS") || value.Contains("НАСТРОЙКИ") || value.Contains("SETTINGS"))
                    StyleButtonV6(button, false, 84f);
            }

            foreach (Text text in root.GetComponentsInChildren<Text>(true))
            {
                if (text.text != null && text.text.StartsWith("LUMA BAY", StringComparison.OrdinalIgnoreCase))
                {
                    text.color = GoldText;
                    text.fontSize = 56;
                }
                else if (text.text != null && text.text.Contains("★") && text.text.Contains("◆"))
                {
                    text.fontSize = 20;
                    text.color = GoldText;
                }
            }
        }

        private void RepairMapV6()
        {
            StylePanelByNameV6("MapHeader", PanelDark, 86f);
            StylePanelByNameV6("LighthouseMetaCard", PanelDark, 480f);
            StylePanelByNameV6("TaskCard", PanelDark, 156f);
            SetPreferredV6(FindInRoot("MetaProgressRow") as RectTransform, 42f);

            RectTransform progressTrack = FindInRoot("LongProgressTrack") as RectTransform;
            if (progressTrack != null)
                SetSolidPanelV6(progressTrack.GetComponent<Image>(), new Color(0.004f, 0.035f, 0.055f, 0.98f), "v6_progress", 12);

            Text[] texts = screenRoot.GetComponentsInChildren<Text>(true);
            foreach (Text text in texts)
            {
                if (text == null || string.IsNullOrEmpty(text.text)) continue;
                if (text.text.Contains("СПАСЕНИЕ") || text.text.Contains("RESCUE"))
                {
                    text.fontSize = 17;
                    text.color = new Color(0.48f, 0.90f, 0.96f, 1f);
                }
                else if (text.text.Contains("Восстанов") || text.text.Contains("Restore"))
                {
                    text.resizeTextForBestFit = true;
                }
            }

            foreach (Button button in screenRoot.GetComponentsInChildren<Button>(true))
            {
                Text label = button.GetComponentInChildren<Text>(true);
                if (label == null) continue;
                string value = StripRichTextV6(label.text).ToUpperInvariant();
                if (value.Contains("НАЧАТЬ") || value.Contains("START")) StyleButtonV6(button, true, 92f);
                else if (value.Contains("ВОССТ") || value.Contains("RESTORE") || value.Contains("ОТКРОЕТСЯ") || value.Contains("UNLOCKS")) StyleButtonV6(button, false, 76f);
                else if (value.Contains("УРОВНИ") || value.Contains("LEVELS")) StyleButtonV6(button, false, 66f);
            }
        }

        private void RepairSettingsV6()
        {
            StylePanelByNameV6("SettingsHeader", PanelDark, 88f);
            RectTransform card = FindInRoot("SettingsCard") as RectTransform;
            SetPreferredV6(card, 500f);
            if (card != null) SetSolidPanelV6(card.GetComponent<Image>(), PanelDark, "v6_settings_card", 30);

            VerticalLayoutGroup cardLayout = card != null ? card.GetComponent<VerticalLayoutGroup>() : null;
            if (cardLayout != null)
            {
                cardLayout.padding = new RectOffset(28, 28, 26, 26);
                cardLayout.spacing = 14f;
            }

            if (card != null)
            {
                Transform[] children = card.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in children)
                {
                    if (child.name == "Spacer")
                    {
                        LayoutElement spacer = child.GetComponent<LayoutElement>();
                        if (spacer != null)
                        {
                            spacer.flexibleHeight = 0f;
                            spacer.minHeight = 8f;
                            spacer.preferredHeight = 8f;
                        }
                    }
                }
            }

            Button[] buttons = card != null ? card.GetComponentsInChildren<Button>(true) : Array.Empty<Button>();
            foreach (Button button in buttons)
            {
                Text label = button.GetComponentInChildren<Text>(true);
                bool reset = label != null && (label.text.Contains("Сброс", StringComparison.OrdinalIgnoreCase) || label.text.Contains("Reset", StringComparison.OrdinalIgnoreCase));
                StyleButtonV6(button, false, reset ? 70f : 78f,
                    reset ? new Color(0.40f, 0.10f, 0.14f, 0.99f) : ButtonDark);
            }
        }

        private void RepairLevelSelectorV6()
        {
            RectTransform scrollRoot = FindInRoot("LevelScroll") as RectTransform;
            if (scrollRoot == null) return;
            SetSolidPanelV6(scrollRoot.GetComponent<Image>(), new Color(0.006f, 0.045f, 0.072f, 0.96f), "v6_level_scroll", 28);

            ScrollRect scroll = scrollRoot.GetComponent<ScrollRect>();
            RectTransform viewport = scrollRoot.Find("Viewport") as RectTransform;
            RectTransform content = viewport != null ? viewport.Find("Content") as RectTransform : null;
            if (scroll == null || viewport == null || content == null) return;

            Mask oldMask = viewport.GetComponent<Mask>();
            if (oldMask != null) oldMask.enabled = false;
            if (viewport.GetComponent<RectMask2D>() == null) viewport.gameObject.AddComponent<RectMask2D>();

            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.offsetMin = new Vector2(14f, 14f);
            viewport.offsetMax = new Vector2(-14f, -14f);
            content.gameObject.SetActive(true);
            content.localScale = Vector3.one;

            const int columns = 3;
            const float gap = 12f;
            float width = viewport.rect.width > 300f ? viewport.rect.width : 640f;
            float cellWidth = Mathf.Floor((width - gap * 2f - 24f) / columns);
            float cellHeight = 142f;
            int rows = Mathf.CeilToInt(Mathf.Max(1, content.childCount) / (float)columns);

            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = new Vector2(0f, rows * cellHeight + Mathf.Max(0, rows - 1) * gap + 28f);

            GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
            if (grid == null) grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.padding = new RectOffset(12, 12, 12, 16);
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
                bool current = save != null && i + 1 == save.UnlockedLevel;
                Color cardColor = !unlocked
                    ? new Color(0.018f, 0.052f, 0.07f, 0.96f)
                    : current ? new Color(0.02f, 0.34f, 0.39f, 0.99f) : PanelSoft;
                SetSolidPanelV6(card.GetComponent<Image>(), cardColor, "v6_level_card_" + i, 22);

                Text[] labels = card.GetComponentsInChildren<Text>(true);
                foreach (Text label in labels)
                {
                    if (label == null) continue;
                    if ((label.text ?? string.Empty).Trim() == "◆")
                    {
                        label.gameObject.SetActive(false);
                        continue;
                    }
                    label.gameObject.SetActive(true);
                    label.color = unlocked ? Color.white : MutedText;
                    label.resizeTextForBestFit = true;
                    label.resizeTextMinSize = 13;
                    label.resizeTextMaxSize = 24;
                }

                RectTransform icon = card.Find("LevelTargetIcon") as RectTransform;
                if (icon != null)
                {
                    icon.gameObject.SetActive(unlocked);
                    icon.anchorMin = new Vector2(0.30f, 0.50f);
                    icon.anchorMax = new Vector2(0.70f, 0.92f);
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

        private void RepairGameplayV6()
        {
            StylePanelByNameV6("GameplayHeader", PanelDark, 78f);
            StylePanelByNameV6("TopHud", PanelDark, 78f);
            StylePanelByNameV6("GoalsPanel", PanelDark, 96f);

            RectTransform tray = FindInRoot("BoosterTray") as RectTransform;
            if (tray == null) tray = FindInRoot("BoosterRow") as RectTransform;
            if (tray == null) tray = FindInRoot("BoostersRow") as RectTransform;
            if (tray == null) return;

            SetPreferredV6(tray, 138f);
            SetSolidPanelV6(tray.GetComponent<Image>(), new Color(0.006f, 0.045f, 0.072f, 0.97f), "v6_booster_tray", 24);

            HorizontalLayoutGroup row = tray.GetComponent<HorizontalLayoutGroup>();
            if (row != null)
            {
                row.padding = new RectOffset(8, 8, 8, 8);
                row.spacing = 7f;
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
                element.preferredWidth = 110f;
                element.flexibleWidth = 1f;
                element.minHeight = 118f;
                element.preferredHeight = 118f;
                element.flexibleHeight = 0f;
                SetSolidPanelV6(booster.GetComponent<Image>(), ButtonDark, "v6_booster_" + booster.GetInstanceID(), 18);

                RectTransform premiumIcon = booster.transform.Find("PremiumBoosterIcon") as RectTransform;
                if (premiumIcon != null)
                {
                    premiumIcon.anchorMin = new Vector2(0.16f, 0.24f);
                    premiumIcon.anchorMax = new Vector2(0.84f, 0.90f);
                    premiumIcon.offsetMin = Vector2.zero;
                    premiumIcon.offsetMax = Vector2.zero;
                }

                foreach (Text label in booster.GetComponentsInChildren<Text>(true))
                {
                    bool price = label.text != null && label.text.Contains("◆");
                    label.gameObject.SetActive(price);
                    if (!price) continue;
                    label.color = GoldText;
                    label.fontSize = 14;
                    label.alignment = TextAnchor.MiddleCenter;
                    label.rectTransform.anchorMin = new Vector2(0.08f, 0.02f);
                    label.rectTransform.anchorMax = new Vector2(0.92f, 0.25f);
                    label.rectTransform.offsetMin = Vector2.zero;
                    label.rectTransform.offsetMax = Vector2.zero;
                }
            }
        }

        private void StylePanelByNameV6(string name, Color color, float height)
        {
            RectTransform rect = FindInRoot(name) as RectTransform;
            if (rect == null) return;
            SetPreferredV6(rect, height);
            SetSolidPanelV6(rect.GetComponent<Image>(), color, "v6_" + name, 24);
        }

        private static void StyleButtonV6(Button button, bool primary, float height, Color? overrideColor = null)
        {
            if (button == null) return;
            SetPreferredV6(button.transform as RectTransform, height);
            SetSolidPanelV6(button.GetComponent<Image>(), overrideColor ?? (primary ? ButtonPrimary : ButtonDark),
                "v6_button_" + button.GetInstanceID(), 22);

            Text label = button.GetComponentInChildren<Text>(true);
            if (label != null)
            {
                label.gameObject.SetActive(true);
                label.color = Color.white;
                label.fontSize = primary ? 28 : 22;
                label.resizeTextForBestFit = true;
                label.resizeTextMinSize = 16;
                label.resizeTextMaxSize = primary ? 28 : 22;
            }
        }

        private static void SetSolidPanelV6(Image image, Color color, string key, int radius)
        {
            if (image == null) return;
            image.sprite = ProceduralArt.Rounded(key, Color.white, radius);
            image.type = Image.Type.Sliced;
            image.color = color;
            image.preserveAspect = false;
        }

        private static void SetPreferredV6(RectTransform rect, float height)
        {
            if (rect == null) return;
            LayoutElement element = rect.GetComponent<LayoutElement>();
            if (element == null) element = rect.gameObject.AddComponent<LayoutElement>();
            element.minHeight = height;
            element.preferredHeight = height;
            element.flexibleHeight = 0f;
        }

        private static string StripRichTextV6(string value)
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
