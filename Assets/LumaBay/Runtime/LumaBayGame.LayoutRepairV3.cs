using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private float nextStableInterfacePass;

        private void ApplyStableInterfaceV4()
        {
            if (screenRoot == null || Time.unscaledTime < nextStableInterfacePass) return;
            nextStableInterfacePass = Time.unscaledTime + 0.12f;

            LumaBayPremiumCompositionV2 legacy = Object.FindFirstObjectByType<LumaBayPremiumCompositionV2>();
            if (legacy != null) legacy.enabled = false;

            ApplyStableBackgroundV4();
            ApplyStablePanelsV4();
            RepairLevelSelectorV4();
            RepairGameplayBottomBarV4();
            RepairModalV4();
        }

        private void ApplyStableBackgroundV4()
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

        private void ApplyStablePanelsV4()
        {
            Image[] images = screenRoot.GetComponentsInChildren<Image>(true);
            foreach (Image image in images)
            {
                if (image == null) continue;
                string objectName = image.name;
                if (objectName == "Piece" || objectName == "PremiumBoosterIcon" ||
                    objectName == "PremiumLighthouseVisual" || objectName == "LevelTargetIcon" ||
                    objectName.StartsWith("Cell_") || objectName.Contains("Obstacle"))
                    continue;

                Button button = image.GetComponent<Button>();
                if (button != null)
                {
                    bool primary = IsPrimaryV4(button);
                    image.sprite = ProceduralArt.OrnateFrame(primary ? "stable_primary" : "stable_secondary",
                        primary ? new Color(0.035f, 0.48f, 0.55f, 1f) : new Color(0.025f, 0.19f, 0.29f, 1f), true);
                    image.type = Image.Type.Sliced;
                    image.preserveAspect = false;
                    image.color = Color.white;
                    SetButtonHeightV4(button, primary ? 94f : 78f);
                    continue;
                }

                if (!IsPanelNameV4(objectName)) continue;
                Color tone = objectName.Contains("Header")
                    ? new Color(0.018f, 0.10f, 0.17f, 0.97f)
                    : new Color(0.018f, 0.095f, 0.15f, 0.94f);
                image.sprite = ProceduralArt.OrnateFrame("stable_panel_" + objectName, tone, true);
                image.type = Image.Type.Sliced;
                image.preserveAspect = false;
                image.color = Color.white;
            }
        }

        private static bool IsPanelNameV4(string name)
        {
            return name.Contains("Header") || name.Contains("Panel") || name.Contains("Card") ||
                   name.Contains("Scroll") || name.Contains("HeroCaption") ||
                   name.Contains("ProgressTrack") || name.Contains("BoosterTray") ||
                   name.Contains("BoardFrame");
        }

        private static bool IsPrimaryV4(Button button)
        {
            Text label = button.GetComponentInChildren<Text>(true);
            string value = label != null ? (label.text ?? string.Empty).ToUpperInvariant() : string.Empty;
            return value.Contains("ИГРАТЬ") || value.Contains("НАЧАТЬ") || value.Contains("СЛЕДУЮЩ") ||
                   value.Contains("ВОССТ") || value.Contains("PLAY") || value.Contains("START") ||
                   value.Contains("NEXT") || value.Contains("RESTORE");
        }

        private static void SetButtonHeightV4(Button button, float baseHeight)
        {
            RectTransform rect = button.transform as RectTransform;
            if (rect == null) return;
            LayoutElement layout = rect.GetComponent<LayoutElement>();
            if (layout == null) layout = rect.gameObject.AddComponent<LayoutElement>();

            Text label = button.GetComponentInChildren<Text>(true);
            string value = label != null ? (label.text ?? string.Empty).ToUpperInvariant() : string.Empty;
            float height = baseHeight;
            if (value.Contains("ИГРАТЬ") || value == "PLAY") height = 108f;
            else if (value.Contains("НАСТРОЙКИ") || value.Contains("УРОВНИ") ||
                     value.Contains("SETTINGS") || value.Contains("LEVELS")) height = 92f;
            else if (value.Contains("СБРОС") || value.Contains("RESET")) height = 76f;

            layout.minHeight = height;
            layout.preferredHeight = height;
            layout.flexibleHeight = 0f;

            if (label != null)
            {
                label.gameObject.SetActive(true);
                label.fontSize = height >= 100f ? 29 : 23;
                label.resizeTextForBestFit = true;
                label.resizeTextMinSize = 17;
                label.resizeTextMaxSize = height >= 100f ? 29 : 23;
                label.color = Color.white;
            }
        }

        private void RepairLevelSelectorV4()
        {
            RectTransform root = FindInRoot("LevelScroll") as RectTransform;
            if (root == null) return;

            Image rootImage = root.GetComponent<Image>();
            if (rootImage != null)
            {
                rootImage.sprite = ProceduralArt.OrnateFrame("stable_level_scroll",
                    new Color(0.014f, 0.075f, 0.12f, 0.97f), true);
                rootImage.type = Image.Type.Sliced;
                rootImage.color = Color.white;
            }

            root.anchorMin = new Vector2(0.035f, 0.03f);
            root.anchorMax = new Vector2(0.965f, 0.89f);
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;

            ScrollRect scroll = root.GetComponent<ScrollRect>();
            RectTransform viewport = root.Find("Viewport") as RectTransform;
            RectTransform content = viewport != null ? viewport.Find("Content") as RectTransform : null;
            if (scroll == null || viewport == null || content == null) return;

            Mask mask = viewport.GetComponent<Mask>();
            if (mask != null) mask.enabled = false;
            if (viewport.GetComponent<RectMask2D>() == null) viewport.gameObject.AddComponent<RectMask2D>();
            Image viewportImage = viewport.GetComponent<Image>();
            if (viewportImage != null) viewportImage.color = new Color(1f, 1f, 1f, 0.001f);

            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.offsetMin = new Vector2(16f, 16f);
            viewport.offsetMax = new Vector2(-16f, -16f);
            viewport.localScale = Vector3.one;
            viewport.gameObject.SetActive(true);

            content.gameObject.SetActive(true);
            content.localScale = Vector3.one;
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;

            CanvasGroup contentGroup = content.GetComponent<CanvasGroup>();
            if (contentGroup == null) contentGroup = content.gameObject.AddComponent<CanvasGroup>();
            contentGroup.alpha = 1f;
            contentGroup.interactable = true;
            contentGroup.blocksRaycasts = true;

            const int columns = 3;
            const float gap = 14f;
            float width = viewport.rect.width > 300f ? viewport.rect.width : 650f;
            float cellWidth = Mathf.Max(168f, (width - 56f - gap * 2f) / columns);
            float cellHeight = 142f;
            int rows = Mathf.CeilToInt(Mathf.Max(1, content.childCount) / (float)columns);
            content.sizeDelta = new Vector2(0f, 30f + rows * cellHeight + Mathf.Max(0, rows - 1) * gap);

            GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
            if (grid == null) grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.padding = new RectOffset(14, 14, 16, 18);
            grid.spacing = new Vector2(gap, gap);
            grid.cellSize = new Vector2(cellWidth, cellHeight);
            grid.childAlignment = TextAnchor.UpperCenter;

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
                Image image = card.GetComponent<Image>();
                bool unlocked = button == null || button.interactable;
                if (image != null)
                {
                    image.sprite = ProceduralArt.OrnateFrame("stable_level_card_" + i,
                        unlocked ? new Color(0.035f, 0.22f, 0.32f, 1f) : new Color(0.04f, 0.08f, 0.11f, 1f), true);
                    image.type = Image.Type.Sliced;
                    image.preserveAspect = false;
                    image.color = Color.white;
                }

                Text label = card.GetComponentInChildren<Text>(true);
                if (label != null)
                {
                    label.gameObject.SetActive(true);
                    label.rectTransform.anchorMin = new Vector2(0.05f, 0.02f);
                    label.rectTransform.anchorMax = new Vector2(0.95f, 0.50f);
                    label.rectTransform.offsetMin = Vector2.zero;
                    label.rectTransform.offsetMax = Vector2.zero;
                    label.fontSize = 24;
                    label.resizeTextForBestFit = true;
                    label.resizeTextMinSize = 15;
                    label.resizeTextMaxSize = 24;
                    label.color = unlocked ? Color.white : new Color(0.58f, 0.66f, 0.70f, 1f);
                }

                RectTransform icon = card.Find("LevelTargetIcon") as RectTransform;
                if (icon != null)
                {
                    icon.gameObject.SetActive(true);
                    icon.anchorMin = new Vector2(0.31f, 0.49f);
                    icon.anchorMax = new Vector2(0.69f, 0.91f);
                    icon.offsetMin = Vector2.zero;
                    icon.offsetMax = Vector2.zero;
                }
            }

            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }

        private void RepairGameplayBottomBarV4()
        {
            RectTransform tray = FindInRoot("BoosterTray") as RectTransform;
            if (tray == null) tray = FindInRoot("BoosterRow") as RectTransform;
            if (tray == null) tray = FindInRoot("BoostersRow") as RectTransform;
            if (tray == null) return;

            LayoutElement trayLayout = tray.GetComponent<LayoutElement>();
            if (trayLayout == null) trayLayout = tray.gameObject.AddComponent<LayoutElement>();
            trayLayout.minHeight = 154f;
            trayLayout.preferredHeight = 154f;
            trayLayout.flexibleHeight = 0f;

            Image trayImage = tray.GetComponent<Image>();
            if (trayImage != null)
            {
                trayImage.sprite = ProceduralArt.OrnateFrame("stable_booster_tray",
                    new Color(0.012f, 0.065f, 0.105f, 0.97f), true);
                trayImage.type = Image.Type.Sliced;
                trayImage.color = Color.white;
            }

            HorizontalLayoutGroup row = tray.GetComponent<HorizontalLayoutGroup>();
            if (row != null)
            {
                row.padding = new RectOffset(10, 10, 10, 10);
                row.spacing = 8f;
                row.childControlWidth = true;
                row.childControlHeight = true;
                row.childForceExpandWidth = true;
                row.childForceExpandHeight = true;
            }

            Button[] boosters = tray.GetComponentsInChildren<Button>(true);
            foreach (Button booster in boosters)
            {
                RectTransform rect = booster.transform as RectTransform;
                LayoutElement element = rect.GetComponent<LayoutElement>();
                if (element == null) element = rect.gameObject.AddComponent<LayoutElement>();
                element.minWidth = 0f;
                element.preferredWidth = 110f;
                element.flexibleWidth = 1f;
                element.minHeight = 126f;
                element.preferredHeight = 126f;
                element.flexibleHeight = 0f;

                Image image = booster.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = ProceduralArt.OrnateFrame("stable_booster_button",
                        new Color(0.025f, 0.17f, 0.25f, 1f), true);
                    image.type = Image.Type.Sliced;
                    image.preserveAspect = false;
                    image.color = Color.white;
                }

                Text[] labels = booster.GetComponentsInChildren<Text>(true);
                foreach (Text label in labels)
                {
                    bool price = (label.text ?? string.Empty).Contains("◆");
                    label.gameObject.SetActive(price);
                    if (price)
                    {
                        label.fontSize = 15;
                        label.color = new Color(1f, 0.84f, 0.45f, 1f);
                    }
                }
            }
        }

        private void RepairModalV4()
        {
            if (canvas == null) return;
            Transform overlay = canvas.transform.Find("ModalOverlay");
            if (overlay == null) return;
            overlay.SetAsLastSibling();

            RectTransform panel = overlay.Find("ModalPanel") as RectTransform;
            if (panel == null) return;
            panel.anchorMin = new Vector2(0.08f, 0.22f);
            panel.anchorMax = new Vector2(0.92f, 0.78f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;
            panel.localScale = Vector3.one;

            Image image = panel.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = ProceduralArt.OrnateFrame("stable_modal_panel",
                    new Color(0.018f, 0.095f, 0.15f, 0.99f), true);
                image.type = Image.Type.Sliced;
                image.preserveAspect = false;
                image.color = Color.white;
            }
        }
    }
}
