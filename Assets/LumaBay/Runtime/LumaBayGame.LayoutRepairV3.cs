using System;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private float nextLayoutRepairV3;

        private void ApplyLayoutRepairV3IfNeeded()
        {
            if (screenRoot == null || Time.unscaledTime < nextLayoutRepairV3) return;
            nextLayoutRepairV3 = Time.unscaledTime + 0.15f;

            if (FindInRoot("MainHero") != null) RepairMainMenuV3();
            if (FindInRoot("LighthouseMetaCard") != null) RepairMapV3();
            if (FindInRoot("SettingsHeader") != null) RepairSettingsV3();
            if (FindInRoot("LevelScroll") != null) RepairLevelSelectorV3();
            if (FindInRoot("BoardFrame") != null || FindInRoot("BoardGrid") != null) RepairGameplayV3();
        }

        private void RepairMainMenuV3()
        {
            RectTransform vertical = FindInRoot("VerticalScreen") as RectTransform;
            VerticalLayoutGroup layout = vertical != null ? vertical.GetComponent<VerticalLayoutGroup>() : null;
            if (layout != null)
            {
                layout.padding = new RectOffset(28, 28, 20, 24);
                layout.spacing = 12f;
            }

            SetPreferredV3(FindInRoot("LogoBlock") as RectTransform, 112f);
            SetPreferredV3(FindInRoot("MainHero") as RectTransform, 540f);
            SetPreferredV3(FindInRoot("MainSecondaryRow") as RectTransform, 104f);

            Button[] buttons = screenRoot.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                Text label = button.GetComponentInChildren<Text>(true);
                if (label == null) continue;
                string value = StripRichTextV3(label.text).ToUpperInvariant();
                bool play = value.Contains("ИГРАТЬ") || value == "PLAY";
                bool secondary = value.Contains("УРОВНИ") || value.Contains("LEVELS") ||
                                 value.Contains("НАСТРОЙКИ") || value.Contains("SETTINGS");
                if (!play && !secondary) continue;

                SetPreferredV3(button.transform as RectTransform, play ? 112f : 96f);
                label.fontSize = play ? 30 : 25;
                label.resizeTextForBestFit = true;
                label.resizeTextMinSize = 20;
                label.resizeTextMaxSize = play ? 30 : 25;
            }
        }

        private void RepairMapV3()
        {
            RectTransform vertical = FindInRoot("VerticalScreen") as RectTransform;
            VerticalLayoutGroup layout = vertical != null ? vertical.GetComponent<VerticalLayoutGroup>() : null;
            if (layout != null)
            {
                layout.padding = new RectOffset(20, 20, 14, 18);
                layout.spacing = 10f;
            }

            SetPreferredV3(FindInRoot("MapHeader") as RectTransform, 92f);
            SetPreferredV3(FindInRoot("LighthouseMetaCard") as RectTransform, 540f);
            SetPreferredV3(FindInRoot("TaskCard") as RectTransform, 170f);
            SetPreferredV3(FindInRoot("MetaProgressRow") as RectTransform, 46f);

            Button[] buttons = screenRoot.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                Text label = button.GetComponentInChildren<Text>(true);
                if (label == null) continue;
                string value = StripRichTextV3(label.text).ToUpperInvariant();
                float height = 0f;
                if (value.Contains("НАЧАТЬ") || value.Contains("START")) height = 102f;
                else if (value.Contains("ОТКРОЕТСЯ") || value.Contains("ВОССТ") || value.Contains("RESTORE")) height = 88f;
                else if (value.Contains("УРОВНИ") || value.Contains("LEVELS")) height = 78f;
                if (height <= 0f) continue;

                SetPreferredV3(button.transform as RectTransform, height);
                label.fontSize = height >= 100f ? 27 : 22;
                label.resizeTextForBestFit = true;
                label.resizeTextMinSize = 17;
                label.resizeTextMaxSize = height >= 100f ? 27 : 22;
            }
        }

        private void RepairSettingsV3()
        {
            RectTransform vertical = FindInRoot("VerticalScreen") as RectTransform;
            VerticalLayoutGroup screenLayout = vertical != null ? vertical.GetComponent<VerticalLayoutGroup>() : null;
            if (screenLayout != null)
            {
                screenLayout.padding = new RectOffset(22, 22, 18, 22);
                screenLayout.spacing = 13f;
            }

            SetPreferredV3(FindInRoot("SettingsHeader") as RectTransform, 94f);
            SetPreferredV3(FindInRoot("SettingsCard") as RectTransform, 640f);

            Transform card = FindInRoot("SettingsCard");
            VerticalLayoutGroup cardLayout = card != null ? card.GetComponent<VerticalLayoutGroup>() : null;
            if (cardLayout != null)
            {
                cardLayout.padding = new RectOffset(22, 22, 28, 28);
                cardLayout.spacing = 18f;
            }

            Button[] buttons = screenRoot.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                Text label = button.GetComponentInChildren<Text>(true);
                if (label == null || label.text == "‹") continue;
                SetPreferredV3(button.transform as RectTransform, 94f);
                label.fontSize = 25;
                label.resizeTextForBestFit = true;
                label.resizeTextMinSize = 18;
                label.resizeTextMaxSize = 25;
            }
        }

        private void RepairGameplayV3()
        {
            SetPreferredV3(FindInRoot("GameplayHeader") as RectTransform, 86f);
            SetPreferredV3(FindInRoot("TopHud") as RectTransform, 86f);
            SetPreferredV3(FindInRoot("GoalsPanel") as RectTransform, 110f);

            RectTransform tray = FindInRoot("BoosterTray") as RectTransform;
            if (tray == null) tray = FindInRoot("BoosterRow") as RectTransform;
            if (tray == null) tray = FindInRoot("BoostersRow") as RectTransform;
            if (tray == null) return;

            SetPreferredV3(tray, 174f);
            HorizontalLayoutGroup row = tray.GetComponent<HorizontalLayoutGroup>();
            if (row != null)
            {
                row.padding = new RectOffset(8, 8, 8, 8);
                row.spacing = 8f;
                row.childAlignment = TextAnchor.MiddleCenter;
                row.childControlWidth = true;
                row.childControlHeight = true;
                row.childForceExpandWidth = true;
                row.childForceExpandHeight = false;
            }

            Button[] boosters = tray.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < boosters.Length; i++)
            {
                Button booster = boosters[i];
                RectTransform rect = booster.transform as RectTransform;
                LayoutElement element = rect != null ? rect.GetComponent<LayoutElement>() : null;
                if (element == null && rect != null) element = rect.gameObject.AddComponent<LayoutElement>();
                if (element != null)
                {
                    element.minWidth = 92f;
                    element.preferredWidth = 116f;
                    element.flexibleWidth = 1f;
                    element.minHeight = 146f;
                    element.preferredHeight = 150f;
                    element.flexibleHeight = 0f;
                }

                Image image = booster.GetComponent<Image>();
                Sprite medallion = LumaBayArtPackV2.MapNode(i == 0 ? 2 : 1);
                if (image != null && medallion != null)
                {
                    image.sprite = medallion;
                    image.type = Image.Type.Simple;
                    image.preserveAspect = false;
                    image.color = Color.white;
                }

                Text[] labels = booster.GetComponentsInChildren<Text>(true);
                foreach (Text label in labels)
                {
                    string value = label.text ?? string.Empty;
                    bool price = value.Contains("◆");
                    bool icon = !price && value.Length <= 4;
                    label.gameObject.SetActive(price || icon);
                    if (price)
                    {
                        label.fontSize = 15;
                        label.color = new Color(1f, 0.86f, 0.48f, 1f);
                    }
                    else if (icon)
                    {
                        label.fontSize = 30;
                        label.color = Color.white;
                    }
                }
            }
        }

        private void RepairLevelSelectorV3()
        {
            RectTransform header = FindInRoot("LevelsHeader") as RectTransform;
            if (header != null)
            {
                header.anchorMin = new Vector2(0.025f, 0.905f);
                header.anchorMax = new Vector2(0.975f, 0.99f);
                header.offsetMin = Vector2.zero;
                header.offsetMax = Vector2.zero;
            }

            RectTransform root = FindInRoot("LevelScroll") as RectTransform;
            if (root == null) return;
            root.anchorMin = new Vector2(0.025f, 0.02f);
            root.anchorMax = new Vector2(0.975f, 0.895f);
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;

            ScrollRect scroll = root.GetComponent<ScrollRect>();
            RectTransform viewport = root.Find("Viewport") as RectTransform;
            RectTransform content = viewport != null ? viewport.Find("Content") as RectTransform : null;
            if (scroll == null || viewport == null || content == null) return;

            viewport.gameObject.SetActive(true);
            content.gameObject.SetActive(true);
            viewport.localScale = Vector3.one;
            content.localScale = Vector3.one;
            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.offsetMin = new Vector2(14f, 14f);
            viewport.offsetMax = new Vector2(-14f, -14f);

            CanvasGroup contentGroup = content.GetComponent<CanvasGroup>();
            if (contentGroup == null) contentGroup = content.gameObject.AddComponent<CanvasGroup>();
            contentGroup.alpha = 1f;
            contentGroup.interactable = true;
            contentGroup.blocksRaycasts = true;

            const int columns = 3;
            const float gap = 12f;
            float width = viewport.rect.width > 300f ? viewport.rect.width : 650f;
            float cellWidth = Mathf.Clamp((width - 52f - gap * 2f) / columns, 170f, 208f);
            float cellHeight = 152f;
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
            grid.padding = new RectOffset(14, 14, 18, 24);
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
                if (image != null)
                {
                    Sprite sprite = button != null && button.interactable
                        ? LumaBayArtPackV2.SecondaryButton
                        : LumaBayArtPackV2.MapNode(0);
                    if (sprite != null) image.sprite = sprite;
                    image.type = Image.Type.Sliced;
                    image.preserveAspect = false;
                    image.color = button != null && !button.interactable
                        ? new Color(0.58f, 0.63f, 0.68f, 0.96f)
                        : Color.white;
                }

                Text label = card.GetComponentInChildren<Text>(true);
                if (label != null)
                {
                    label.gameObject.SetActive(true);
                    label.color = Color.white;
                    label.fontSize = 25;
                    label.resizeTextForBestFit = true;
                    label.resizeTextMinSize = 16;
                    label.resizeTextMaxSize = 25;
                    label.rectTransform.anchorMin = new Vector2(0.08f, 0.05f);
                    label.rectTransform.anchorMax = new Vector2(0.92f, 0.48f);
                    label.rectTransform.offsetMin = Vector2.zero;
                    label.rectTransform.offsetMax = Vector2.zero;
                }

                Transform icon = card.Find("LevelTargetIcon");
                if (icon != null)
                {
                    icon.gameObject.SetActive(true);
                    RectTransform iconRect = icon as RectTransform;
                    iconRect.anchorMin = new Vector2(0.30f, 0.48f);
                    iconRect.anchorMax = new Vector2(0.70f, 0.90f);
                    iconRect.offsetMin = Vector2.zero;
                    iconRect.offsetMax = Vector2.zero;
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

        private static void SetPreferredV3(RectTransform rect, float height)
        {
            if (rect == null) return;
            LayoutElement element = rect.GetComponent<LayoutElement>();
            if (element == null) element = rect.gameObject.AddComponent<LayoutElement>();
            element.minHeight = height;
            element.preferredHeight = height;
            element.flexibleHeight = 0f;
        }

        private static string StripRichTextV3(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            System.Text.StringBuilder builder = new System.Text.StringBuilder(value.Length);
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
