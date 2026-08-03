using System;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private int premiumSkinSignature = int.MinValue;
        private Image shellBackground;

        private void ApplyPremiumSkinV2IfNeeded()
        {
            if (screenRoot == null || !LumaBayArtPackV2.IsAvailable) return;

            int childCount = screenRoot.childCount;
            int firstId = childCount > 0 ? screenRoot.GetChild(0).GetInstanceID() : 0;
            int signature = childCount * 73856093 ^ firstId;
            if (signature == premiumSkinSignature) return;
            premiumSkinSignature = signature;

            ResolveShellBackground();
            ApplyScreenBackground();
            SkinPanelsAndButtons();
            SkinGameplayHud();
            SkinLevelSelector();
            SkinModalLayers();
        }

        private void ResolveShellBackground()
        {
            if (shellBackground != null) return;
            Transform backgroundTransform = canvas != null ? canvas.transform.Find("Background") : null;
            shellBackground = backgroundTransform != null ? backgroundTransform.GetComponent<Image>() : null;
        }

        private void ApplyScreenBackground()
        {
            if (shellBackground == null) return;

            Sprite sprite;
            if (FindInRoot("BoardFrame") != null || FindInRoot("BoardGrid") != null)
                sprite = LumaBayArtPackV2.GameplayBackground;
            else if (FindInRoot("MapHeader") != null)
                sprite = LumaBayArtPackV2.MapBackground;
            else if (FindInRoot("LevelScroll") != null)
                sprite = LumaBayArtPackV2.MapBackground;
            else if (FindInRoot("SettingsHeader") != null)
                sprite = LumaBayArtPackV2.StoryBackground;
            else
                sprite = LumaBayArtPackV2.MainMenuBackground;

            if (sprite != null)
            {
                shellBackground.sprite = sprite;
                shellBackground.color = Color.white;
                shellBackground.preserveAspect = false;
            }
        }

        private void SkinPanelsAndButtons()
        {
            Image[] images = screenRoot.GetComponentsInChildren<Image>(true);
            foreach (Image image in images)
            {
                if (image == null) continue;
                string name = image.name;

                if (name.StartsWith("Cell_", StringComparison.Ordinal) ||
                    name == "Piece" || name.Contains("PremiumLighthouse", StringComparison.Ordinal))
                    continue;

                Button button = image.GetComponent<Button>();
                if (button != null)
                {
                    Sprite buttonSprite = IsPrimaryButton(button)
                        ? LumaBayArtPackV2.PrimaryButton
                        : LumaBayArtPackV2.SecondaryButton;
                    if (buttonSprite != null)
                    {
                        image.sprite = buttonSprite;
                        image.type = Image.Type.Sliced;
                        image.color = Color.white;
                    }
                    ConfigurePremiumButton(button);
                    continue;
                }

                Sprite panelSprite = ResolvePanelSprite(name);
                if (panelSprite == null) continue;
                image.sprite = panelSprite;
                image.type = Image.Type.Sliced;
                image.color = Color.white;

                Shadow shadow = image.GetComponent<Shadow>();
                if (shadow != null)
                {
                    shadow.effectColor = new Color(0f, 0.02f, 0.05f, 0.38f);
                    shadow.effectDistance = new Vector2(2f, -3f);
                }
            }
        }

        private Sprite ResolvePanelSprite(string name)
        {
            if (name.Contains("Goal", StringComparison.OrdinalIgnoreCase)) return LumaBayArtPackV2.GoalsPanel;
            if (name.Contains("Move", StringComparison.OrdinalIgnoreCase)) return LumaBayArtPackV2.MovesPanel;
            if (name.Contains("Booster", StringComparison.OrdinalIgnoreCase)) return LumaBayArtPackV2.BoosterTray;
            if (name.Contains("Progress", StringComparison.OrdinalIgnoreCase)) return LumaBayArtPackV2.ProgressTrack;
            if (name.Contains("Header", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Card", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Panel", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Modal", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Scroll", StringComparison.OrdinalIgnoreCase))
                return LumaBayArtPackV2.PanelLarge;
            return null;
        }

        private static bool IsPrimaryButton(Button button)
        {
            Text label = button.GetComponentInChildren<Text>(true);
            if (label == null) return false;
            string value = label.text ?? string.Empty;
            return value.IndexOf("ИГР", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   value.IndexOf("НАЧ", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   value.IndexOf("ДАЛ", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   value.IndexOf("ВОССТ", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   value.IndexOf("PLAY", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   value.IndexOf("START", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   value.IndexOf("NEXT", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   value.IndexOf("RESTORE", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void ConfigurePremiumButton(Button button)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.06f, 1.06f, 1.06f, 1f);
            colors.pressedColor = new Color(0.76f, 0.88f, 0.94f, 1f);
            colors.selectedColor = Color.white;
            colors.disabledColor = new Color(0.46f, 0.52f, 0.56f, 0.72f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.08f;
            button.colors = colors;
        }

        private void SkinGameplayHud()
        {
            RectTransform boosterRow = FindInRoot("BoosterRow") as RectTransform;
            if (boosterRow == null) boosterRow = FindInRoot("BoostersRow") as RectTransform;
            if (boosterRow != null)
            {
                HorizontalLayoutGroup layout = boosterRow.GetComponent<HorizontalLayoutGroup>();
                if (layout != null)
                {
                    layout.spacing = 8f;
                    layout.padding = new RectOffset(5, 5, 5, 5);
                    layout.childAlignment = TextAnchor.MiddleCenter;
                }

                Button[] boosters = boosterRow.GetComponentsInChildren<Button>(true);
                foreach (Button booster in boosters)
                {
                    RectTransform rect = booster.transform as RectTransform;
                    LayoutElement element = rect != null ? rect.GetComponent<LayoutElement>() : null;
                    if (element == null && rect != null) element = rect.gameObject.AddComponent<LayoutElement>();
                    if (element != null)
                    {
                        element.flexibleWidth = 1f;
                        element.preferredWidth = 118f;
                        element.preferredHeight = 112f;
                    }

                    Text[] labels = booster.GetComponentsInChildren<Text>(true);
                    foreach (Text label in labels)
                    {
                        if (label.fontSize > 20 && label.text.IndexOf("◆", StringComparison.Ordinal) < 0)
                            label.gameObject.SetActive(false);
                        else
                            label.fontSize = Mathf.Clamp(label.fontSize, 14, 18);
                    }
                }
            }

            RectTransform goals = FindInRoot("GoalsPanel") as RectTransform;
            if (goals != null)
            {
                VerticalLayoutGroup vertical = goals.GetComponent<VerticalLayoutGroup>();
                if (vertical != null)
                {
                    vertical.padding = new RectOffset(16, 16, 10, 10);
                    vertical.spacing = 4f;
                }
            }
        }

        private void SkinLevelSelector()
        {
            RectTransform scrollRoot = FindInRoot("LevelScroll") as RectTransform;
            if (scrollRoot == null) return;

            ScrollRect scroll = scrollRoot.GetComponent<ScrollRect>();
            RectTransform viewport = scrollRoot.Find("Viewport") as RectTransform;
            RectTransform content = viewport != null ? viewport.Find("Content") as RectTransform : null;
            if (scroll == null || viewport == null || content == null) return;

            content.gameObject.SetActive(true);
            content.localScale = Vector3.one;
            CanvasGroup group = content.GetComponent<CanvasGroup>();
            if (group == null) group = content.gameObject.AddComponent<CanvasGroup>();
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;

            GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
            if (grid == null) grid = content.gameObject.AddComponent<GridLayoutGroup>();
            const int columns = 3;
            float viewportWidth = viewport.rect.width > 100f ? viewport.rect.width : 660f;
            const float sidePadding = 22f;
            const float spacing = 12f;
            float cellWidth = Mathf.Floor((viewportWidth - sidePadding * 2f - spacing * (columns - 1)) / columns);
            float cellHeight = Mathf.Clamp(cellWidth * 0.86f, 140f, 170f);
            int rows = Mathf.CeilToInt(content.childCount / (float)columns);

            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = new Vector2(0f, rows * cellHeight + Mathf.Max(0, rows - 1) * spacing + 48f);

            grid.padding = new RectOffset(Mathf.RoundToInt(sidePadding), Mathf.RoundToInt(sidePadding), 20, 24);
            grid.spacing = new Vector2(spacing, spacing);
            grid.cellSize = new Vector2(cellWidth, cellHeight);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.childAlignment = TextAnchor.UpperCenter;

            for (int i = 0; i < content.childCount; i++)
            {
                Transform item = content.GetChild(i);
                item.gameObject.SetActive(true);
                item.localScale = Vector3.one;
                Image image = item.GetComponent<Image>();
                Sprite node = LumaBayArtPackV2.MapNode(i == 0 ? 2 : 1);
                if (image != null && node != null)
                {
                    image.sprite = node;
                    image.type = Image.Type.Simple;
                    image.preserveAspect = true;
                    image.color = Color.white;
                }

                Text text = item.GetComponentInChildren<Text>(true);
                if (text != null)
                {
                    text.color = Color.white;
                    text.fontSize = 25;
                    text.resizeTextForBestFit = true;
                    text.resizeTextMinSize = 15;
                    text.resizeTextMaxSize = 25;
                }
            }

            scroll.content = content;
            scroll.viewport = viewport;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            scroll.verticalNormalizedPosition = 1f;
        }

        private void SkinModalLayers()
        {
            if (canvas == null) return;
            Transform overlay = canvas.transform.Find("ModalOverlay");
            if (overlay == null) return;

            Transform toast = canvas.transform.Find("Toast");
            if (toast != null) Destroy(toast.gameObject);
            overlay.SetAsLastSibling();

            RectTransform panel = overlay.Find("ModalPanel") as RectTransform;
            if (panel == null) return;
            Image image = panel.GetComponent<Image>();
            if (image != null && LumaBayArtPackV2.PanelLarge != null)
            {
                image.sprite = LumaBayArtPackV2.PanelLarge;
                image.type = Image.Type.Sliced;
                image.color = Color.white;
            }
        }

        private Transform FindInRoot(string name)
        {
            if (screenRoot == null) return null;
            Transform[] all = screenRoot.GetComponentsInChildren<Transform>(true);
            foreach (Transform item in all)
                if (item.name == name) return item;
            return null;
        }
    }
}
