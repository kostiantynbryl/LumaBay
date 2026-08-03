using System;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private static readonly Color PanelTint = new Color(0.16f, 0.36f, 0.44f, 0.98f);
        private static readonly Color PanelTintSoft = new Color(0.20f, 0.47f, 0.55f, 0.98f);
        private static readonly Color SecondaryTint = new Color(0.18f, 0.50f, 0.58f, 1f);
        private static readonly Color GoldText = new Color(1f, 0.84f, 0.42f, 1f);
        private static readonly Color MutedText = new Color(0.66f, 0.76f, 0.80f, 1f);

        private void ApplyStableInterfaceV4()
        {
            if (screenRoot == null) return;

            Canvas.ForceUpdateCanvases();
            ApplyStableBackgroundV7();
            NormalizeTransientLayersV7();

            if (FindInRoot("MainHero") != null) RepairMainMenuV7();
            else if (FindInRoot("LighthouseMetaCard") != null) RepairMapV7();
            else if (FindInRoot("SettingsHeader") != null) RepairSettingsV7();
            else if (FindInRoot("LevelScroll") != null) RepairLevelSelectorV7();
            else if (FindInRoot("BoardFrame") != null || FindInRoot("Board") != null) RepairGameplayV7();

            Canvas.ForceUpdateCanvases();
        }

        private void ApplyStableBackgroundV7()
        {
            Transform backgroundTransform = canvas != null ? canvas.transform.Find("Background") : null;
            Image background = backgroundTransform != null ? backgroundTransform.GetComponent<Image>() : null;
            if (background == null) return;

            Sprite selected;
            if (FindInRoot("BoardFrame") != null || FindInRoot("Board") != null)
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

        private void NormalizeTransientLayersV7()
        {
            if (canvas == null) return;
            Transform modal = canvas.transform.Find("ModalOverlay");
            if (modal == null) return;

            modal.SetAsLastSibling();
            RectTransform panel = modal.Find("ModalPanel") as RectTransform;
            if (panel == null) return;

            panel.anchorMin = new Vector2(0.075f, 0.27f);
            panel.anchorMax = new Vector2(0.925f, 0.73f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;
            panel.localScale = Vector3.one;
            ApplyPanelV7(panel.GetComponent<Image>(), LumaBayArtPackV2.PanelLarge, PanelTint);

            VerticalLayoutGroup layout = panel.GetComponent<VerticalLayoutGroup>();
            if (layout != null)
            {
                layout.padding = new RectOffset(34, 34, 30, 30);
                layout.spacing = 12f;
            }

            foreach (ButtonRole marker in panel.GetComponentsInChildren<ButtonRole>(true))
            {
                Button button = marker.GetComponent<Button>();
                if (button == null) continue;
                if (marker.Id == ButtonRoleId.ModalPrimary) StyleButtonV6(button, true, 78f);
                else if (marker.Id == ButtonRoleId.ModalSecondary) StyleButtonV6(button, false, 64f);
            }
        }

        private void RepairMainMenuV7()
        {
            RectTransform root = FindInRoot("VerticalScreen") as RectTransform;
            if (root == null) return;

            VerticalLayoutGroup layout = root.GetComponent<VerticalLayoutGroup>();
            if (layout != null)
            {
                layout.padding = new RectOffset(28, 28, 20, 18);
                layout.spacing = 10f;
            }

            SetPreferredV6(FindInRoot("LogoBlock") as RectTransform, 96f);

            RectTransform hero = FindInRoot("MainHero") as RectTransform;
            SetPreferredV6(hero, 430f);
            if (hero != null)
            {
                ApplyPanelV7(hero.GetComponent<Image>(), LumaBayArtPackV2.PanelLarge, PanelTint);
                Transform lighthouse = hero.Find("PremiumLighthouseVisual");
                if (lighthouse is RectTransform imageRect)
                {
                    imageRect.anchorMin = new Vector2(0.055f, 0.055f);
                    imageRect.anchorMax = new Vector2(0.945f, 0.945f);
                    imageRect.offsetMin = Vector2.zero;
                    imageRect.offsetMax = Vector2.zero;
                    Image image = lighthouse.GetComponent<Image>();
                    if (image != null) image.preserveAspect = true;
                }
            }

            RectTransform row = FindInRoot("MainSecondaryRow") as RectTransform;
            SetPreferredV6(row, 78f);
            HorizontalLayoutGroup rowLayout = row != null ? row.GetComponent<HorizontalLayoutGroup>() : null;
            if (rowLayout != null)
            {
                rowLayout.spacing = 12f;
                rowLayout.padding = new RectOffset(0, 0, 1, 1);
                rowLayout.childControlHeight = true;
                rowLayout.childForceExpandHeight = true;
                rowLayout.childForceExpandWidth = true;
            }

            StyleRoleButtonsV7(root);

            foreach (Text text in root.GetComponentsInChildren<Text>(true))
            {
                if (text.text != null && text.text.StartsWith("LUMA BAY", StringComparison.OrdinalIgnoreCase))
                {
                    text.color = GoldText;
                    text.fontSize = 54;
                }
                else if (text.text != null && text.text.Contains("★") && text.text.Contains("◆"))
                {
                    text.fontSize = 20;
                    text.color = GoldText;
                    text.alignment = TextAnchor.MiddleCenter;
                }
            }
        }

        private void RepairMapV7()
        {
            RectTransform root = FindInRoot("VerticalScreen") as RectTransform;
            if (root == null) return;

            VerticalLayoutGroup layout = root.GetComponent<VerticalLayoutGroup>();
            if (layout != null)
            {
                layout.padding = new RectOffset(22, 22, 16, 16);
                layout.spacing = 9f;
            }

            RectTransform header = FindInRoot("MapHeader") as RectTransform;
            SetPreferredV6(header, 82f);
            if (header != null) ApplyPanelV7(header.GetComponent<Image>(), LumaBayArtPackV2.HeaderPanel, PanelTintSoft);

            RectTransform lighthouseCard = FindInRoot("LighthouseMetaCard") as RectTransform;
            SetPreferredV6(lighthouseCard, 410f);
            if (lighthouseCard != null)
            {
                ApplyPanelV7(lighthouseCard.GetComponent<Image>(), LumaBayArtPackV2.PanelLarge, PanelTint);
                Transform lighthouse = lighthouseCard.Find("PremiumLighthouseVisual");
                if (lighthouse is RectTransform imageRect)
                {
                    imageRect.anchorMin = new Vector2(0.055f, 0.055f);
                    imageRect.anchorMax = new Vector2(0.945f, 0.945f);
                    imageRect.offsetMin = Vector2.zero;
                    imageRect.offsetMax = Vector2.zero;
                    Image image = lighthouse.GetComponent<Image>();
                    if (image != null) image.preserveAspect = true;
                }
            }

            RectTransform task = FindInRoot("TaskCard") as RectTransform;
            SetPreferredV6(task, 146f);
            if (task != null) ApplyPanelV7(task.GetComponent<Image>(), LumaBayArtPackV2.TaskPanel, PanelTintSoft);

            SetPreferredV6(FindInRoot("MetaProgressRow") as RectTransform, 38f);
            RectTransform progress = FindInRoot("LongProgressTrack") as RectTransform;
            if (progress != null)
                ApplyPanelV7(progress.GetComponent<Image>(), LumaBayArtPackV2.ProgressTrack, Color.white);

            StyleRoleButtonsV7(root);
        }

        private void RepairSettingsV7()
        {
            RectTransform root = FindInRoot("VerticalScreen") as RectTransform;
            if (root == null) return;

            VerticalLayoutGroup rootLayout = root.GetComponent<VerticalLayoutGroup>();
            if (rootLayout != null)
            {
                rootLayout.padding = new RectOffset(24, 24, 18, 18);
                rootLayout.spacing = 12f;
            }

            RectTransform header = FindInRoot("SettingsHeader") as RectTransform;
            SetPreferredV6(header, 82f);
            if (header != null) ApplyPanelV7(header.GetComponent<Image>(), LumaBayArtPackV2.HeaderPanel, PanelTintSoft);

            RectTransform card = FindInRoot("SettingsCard") as RectTransform;
            SetPreferredV6(card, 456f);
            if (card != null) ApplyPanelV7(card.GetComponent<Image>(), LumaBayArtPackV2.PanelLarge, PanelTint);

            VerticalLayoutGroup cardLayout = card != null ? card.GetComponent<VerticalLayoutGroup>() : null;
            if (cardLayout != null)
            {
                cardLayout.padding = new RectOffset(26, 26, 24, 24);
                cardLayout.spacing = 12f;
            }

            if (card != null)
            {
                foreach (Transform child in card.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name != "Spacer") continue;
                    LayoutElement spacer = child.GetComponent<LayoutElement>();
                    if (spacer == null) continue;
                    spacer.flexibleHeight = 0f;
                    spacer.minHeight = 6f;
                    spacer.preferredHeight = 6f;
                }
            }

            StyleRoleButtonsV7(root);
        }

        private void RepairLevelSelectorV7()
        {
            RectTransform header = FindInRoot("LevelsHeader") as RectTransform;
            if (header != null)
            {
                header.anchorMin = new Vector2(0.025f, 0.915f);
                header.anchorMax = new Vector2(0.975f, 0.988f);
                header.offsetMin = Vector2.zero;
                header.offsetMax = Vector2.zero;
                ApplyPanelV7(header.GetComponent<Image>(), LumaBayArtPackV2.HeaderPanel, PanelTintSoft);
            }

            RectTransform scrollRoot = FindInRoot("LevelScroll") as RectTransform;
            if (scrollRoot == null) return;
            scrollRoot.anchorMin = new Vector2(0.025f, 0.018f);
            scrollRoot.anchorMax = new Vector2(0.975f, 0.900f);
            scrollRoot.offsetMin = Vector2.zero;
            scrollRoot.offsetMax = Vector2.zero;
            ApplyPanelV7(scrollRoot.GetComponent<Image>(), LumaBayArtPackV2.PanelLarge, PanelTint);

            ScrollRect scroll = scrollRoot.GetComponent<ScrollRect>();
            RectTransform viewport = scrollRoot.Find("Viewport") as RectTransform;
            RectTransform content = viewport != null ? viewport.Find("Content") as RectTransform : null;
            if (scroll == null || viewport == null || content == null) return;

            Mask oldMask = viewport.GetComponent<Mask>();
            if (oldMask != null) oldMask.enabled = false;
            if (viewport.GetComponent<RectMask2D>() == null) viewport.gameObject.AddComponent<RectMask2D>();

            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.offsetMin = new Vector2(18f, 18f);
            viewport.offsetMax = new Vector2(-18f, -24f);

            const int columns = 3;
            const float gap = 12f;
            float width = viewport.rect.width > 300f ? viewport.rect.width : 640f;
            float cellWidth = Mathf.Floor((width - gap * 2f - 24f) / columns);
            float cellHeight = 150f;
            int rows = Mathf.CeilToInt(Mathf.Max(1, content.childCount) / (float)columns);

            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = new Vector2(0f, rows * cellHeight + Mathf.Max(0, rows - 1) * gap + 44f);

            GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
            if (grid == null) grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.padding = new RectOffset(12, 12, 12, 28);
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

                bool unlocked = save != null && i < save.UnlockedLevel;
                bool current = save != null && i + 1 == save.UnlockedLevel;
                int stars = save != null && i < save.StarsByLevel.Count ? save.StarsByLevel[i] : 0;
                int nodeIndex = !unlocked ? 0 : Mathf.Clamp(stars + 1, 1, 4);

                Image cardImage = card.GetComponent<Image>();
                if (cardImage != null)
                {
                    Sprite node = LumaBayArtPackV2.MapNode(nodeIndex);
                    if (node != null)
                    {
                        cardImage.sprite = node;
                        cardImage.type = Image.Type.Simple;
                        cardImage.preserveAspect = true;
                        cardImage.color = current
                            ? new Color(0.80f, 1f, 1f, 1f)
                            : Color.white;
                    }
                }

                foreach (Text label in card.GetComponentsInChildren<Text>(true))
                {
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
                    icon.anchorMin = new Vector2(0.31f, 0.50f);
                    icon.anchorMax = new Vector2(0.69f, 0.88f);
                    icon.offsetMin = Vector2.zero;
                    icon.offsetMax = Vector2.zero;
                }
            }

            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;

            int currentRow = save != null ? Mathf.Max(0, (save.UnlockedLevel - 1) / columns) : 0;
            float contentHeight = Mathf.Max(1f, content.sizeDelta.y - viewport.rect.height);
            float currentOffset = Mathf.Clamp(currentRow * (cellHeight + gap) - viewport.rect.height * 0.35f, 0f, contentHeight);
            content.anchoredPosition = new Vector2(0f, currentOffset);

            StyleRoleButtonsV7(header);
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }

        private void RepairGameplayV7()
        {
            RectTransform header = FindInRoot("GameHeader") as RectTransform;
            if (header != null)
            {
                header.anchorMin = new Vector2(0.018f, 0.928f);
                header.anchorMax = new Vector2(0.982f, 0.994f);
                header.offsetMin = Vector2.zero;
                header.offsetMax = Vector2.zero;
                ApplyPanelV7(header.GetComponent<Image>(), LumaBayArtPackV2.HeaderPanel, PanelTintSoft);
            }

            RectTransform moves = FindInRoot("MovesChip") as RectTransform;
            if (moves != null)
                ApplyPanelV7(moves.GetComponent<Image>(), LumaBayArtPackV2.CompactButton, Color.white);

            RectTransform goals = FindInRoot("Goals") as RectTransform;
            if (goals != null)
            {
                goals.anchorMin = new Vector2(0.026f, 0.842f);
                goals.anchorMax = new Vector2(0.974f, 0.918f);
                goals.offsetMin = Vector2.zero;
                goals.offsetMax = Vector2.zero;
                ApplyPanelV7(goals.GetComponent<Image>(), LumaBayArtPackV2.HeaderPanel, PanelTintSoft);
            }

            RectTransform boardZone = FindInRoot("BoardZone") as RectTransform;
            if (boardZone != null)
            {
                boardZone.anchorMin = new Vector2(0f, 0.188f);
                boardZone.anchorMax = new Vector2(1f, 0.832f);
                boardZone.offsetMin = Vector2.zero;
                boardZone.offsetMax = Vector2.zero;
            }

            RectTransform boardFrame = FindInRoot("BoardFrame") as RectTransform;
            if (boardFrame != null)
            {
                float width = screenRoot.rect.width > 100f ? screenRoot.rect.width : 720f;
                float boardSize = Mathf.Min(692f, width - 22f);
                boardFrame.sizeDelta = new Vector2(boardSize, boardSize);
                ApplyPanelV7(boardFrame.GetComponent<Image>(), LumaBayArtPackV2.PanelLarge, PanelTint);
            }

            RectTransform tray = FindInRoot("BoosterTray") as RectTransform;
            if (tray != null)
            {
                tray.anchorMin = new Vector2(0.015f, 0.018f);
                tray.anchorMax = new Vector2(0.985f, 0.172f);
                tray.offsetMin = Vector2.zero;
                tray.offsetMax = Vector2.zero;
                ApplyPanelV7(tray.GetComponent<Image>(), LumaBayArtPackV2.BoosterTray, Color.white);

                HorizontalLayoutGroup row = tray.GetComponent<HorizontalLayoutGroup>();
                if (row != null)
                {
                    row.padding = new RectOffset(27, 27, 20, 18);
                    row.spacing = 6f;
                    row.childAlignment = TextAnchor.MiddleCenter;
                    row.childControlWidth = true;
                    row.childControlHeight = true;
                    row.childForceExpandWidth = true;
                    row.childForceExpandHeight = true;
                }

                foreach (Button booster in tray.GetComponentsInChildren<Button>(true))
                {
                    Image buttonImage = booster.GetComponent<Image>();
                    if (buttonImage != null)
                    {
                        buttonImage.sprite = null;
                        buttonImage.color = new Color(1f, 1f, 1f, 0.001f);
                    }

                    LayoutElement element = booster.GetComponent<LayoutElement>();
                    if (element == null) element = booster.gameObject.AddComponent<LayoutElement>();
                    element.minWidth = 0f;
                    element.preferredWidth = 112f;
                    element.flexibleWidth = 1f;
                    element.minHeight = 102f;
                    element.preferredHeight = 102f;
                    element.flexibleHeight = 1f;

                    RectTransform premiumIcon = booster.transform.Find("PremiumBoosterIcon") as RectTransform;
                    if (premiumIcon != null)
                    {
                        premiumIcon.anchorMin = new Vector2(0.13f, 0.24f);
                        premiumIcon.anchorMax = new Vector2(0.87f, 0.91f);
                        premiumIcon.offsetMin = Vector2.zero;
                        premiumIcon.offsetMax = Vector2.zero;
                    }

                    foreach (Text label in booster.GetComponentsInChildren<Text>(true))
                    {
                        bool price = label.text != null && label.text.Contains("◆");
                        label.gameObject.SetActive(price);
                        if (!price) continue;
                        label.color = GoldText;
                        label.fontSize = 15;
                        label.alignment = TextAnchor.MiddleCenter;
                        label.rectTransform.anchorMin = new Vector2(0.05f, 0.01f);
                        label.rectTransform.anchorMax = new Vector2(0.95f, 0.24f);
                        label.rectTransform.offsetMin = Vector2.zero;
                        label.rectTransform.offsetMax = Vector2.zero;
                    }
                }
            }

            StyleRoleButtonsV7(screenRoot);
        }

        private void StyleRoleButtonsV7(Transform root)
        {
            if (root == null) return;
            foreach (ButtonRole marker in root.GetComponentsInChildren<ButtonRole>(true))
            {
                Button button = marker.GetComponent<Button>();
                if (button == null) continue;

                switch (marker.Id)
                {
                    case ButtonRoleId.Play:
                        StyleButtonV6(button, true, 88f);
                        break;
                    case ButtonRoleId.StartLevel:
                        StyleButtonV6(button, true, 82f);
                        break;
                    case ButtonRoleId.Restore:
                        StyleButtonV6(button, button.interactable, 72f);
                        break;
                    case ButtonRoleId.Levels:
                    case ButtonRoleId.Settings:
                        StyleButtonV6(button, false, 74f);
                        break;
                    case ButtonRoleId.Back:
                        StyleButtonV6(button, false, 58f);
                        break;
                    case ButtonRoleId.SettingToggle:
                    case ButtonRoleId.Language:
                        StyleButtonV6(button, false, 70f);
                        break;
                    case ButtonRoleId.ResetProgress:
                        StyleButtonV6(button, false, 64f, new Color(0.62f, 0.20f, 0.24f, 1f));
                        break;
                    case ButtonRoleId.ModalPrimary:
                        StyleButtonV6(button, true, 78f);
                        break;
                    case ButtonRoleId.ModalSecondary:
                        StyleButtonV6(button, false, 64f);
                        break;
                }
            }
        }

        private static void StyleButtonV6(Button button, bool primary, float height, Color? overrideColor = null)
        {
            if (button == null) return;
            ButtonRole marker = button.GetComponent<ButtonRole>();
            if (marker != null && (marker.Id == ButtonRoleId.LevelCard || marker.Id == ButtonRoleId.Booster)) return;

            SetPreferredV6(button.transform as RectTransform, height);

            Sprite sprite;
            if (marker != null && marker.Id == ButtonRoleId.Back)
                sprite = LumaBayArtPackV2.CompactButton;
            else
                sprite = primary ? LumaBayArtPackV2.PrimaryButton : LumaBayArtPackV2.SecondaryButton;

            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                if (sprite != null) image.sprite = sprite;
                image.type = Image.Type.Sliced;
                image.preserveAspect = false;
                image.color = overrideColor ?? (primary ? Color.white : SecondaryTint);
            }

            Text label = button.GetComponentInChildren<Text>(true);
            if (label != null)
            {
                label.gameObject.SetActive(true);
                label.color = Color.white;
                label.fontSize = primary ? 27 : 21;
                label.resizeTextForBestFit = true;
                label.resizeTextMinSize = 15;
                label.resizeTextMaxSize = primary ? 27 : 21;
            }
        }

        private static void ApplyPanelV7(Image image, Sprite sprite, Color tint)
        {
            if (image == null) return;
            if (sprite != null) image.sprite = sprite;
            image.type = Image.Type.Sliced;
            image.color = tint;
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
    }
}
