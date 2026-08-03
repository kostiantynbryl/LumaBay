using System;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private void ApplyFinalLayoutV8()
        {
            if (screenRoot == null) return;

            Canvas.ForceUpdateCanvases();
            RepairModalV8();

            if (FindInRoot("MainHero") != null) RepairMainMenuV8();
            else if (FindInRoot("LighthouseMetaCard") != null) RepairMapV8();
            else if (FindInRoot("SettingsHeader") != null) RepairSettingsV8();
            else if (FindInRoot("LevelScroll") != null) RepairLevelSelectorV8();
            else if (FindInRoot("BoardFrame") != null || FindInRoot("Board") != null) RepairGameplayV8();

            Canvas.ForceUpdateCanvases();
        }

        private void RepairModalV8()
        {
            if (canvas == null) return;
            RectTransform overlay = canvas.transform.Find("ModalOverlay") as RectTransform;
            RectTransform panel = overlay != null ? overlay.Find("ModalPanel") as RectTransform : null;
            if (panel == null) return;

            panel.anchorMin = new Vector2(0.075f, 0.29f);
            panel.anchorMax = new Vector2(0.925f, 0.71f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;
            panel.localScale = Vector3.one;
            ApplyPanelV7(panel.GetComponent<Image>(), LumaBayArtPackV2.PanelLarge, Color.white);

            VerticalLayoutGroup layout = panel.GetComponent<VerticalLayoutGroup>();
            if (layout != null)
            {
                layout.padding = new RectOffset(38, 38, 34, 30);
                layout.spacing = 10f;
                layout.childControlHeight = true;
                layout.childForceExpandHeight = false;
                layout.childForceExpandWidth = true;
            }

            int directTextIndex = 0;
            for (int i = 0; i < panel.childCount; i++)
            {
                Transform child = panel.GetChild(i);
                Text directText = child.GetComponent<Text>();
                if (directText != null)
                {
                    if (directTextIndex == 0)
                    {
                        directText.fontSize = 34;
                        directText.resizeTextMinSize = 25;
                        directText.resizeTextMaxSize = 34;
                        SetPreferredV6(directText.rectTransform, 62f);
                    }
                    else
                    {
                        directText.fontSize = 21;
                        directText.resizeTextMinSize = 17;
                        directText.resizeTextMaxSize = 21;
                        SetPreferredV6(directText.rectTransform, 118f);
                    }
                    directTextIndex++;
                }

                if (child.name == "Spacer")
                {
                    LayoutElement spacer = child.GetComponent<LayoutElement>();
                    if (spacer == null) spacer = child.gameObject.AddComponent<LayoutElement>();
                    spacer.flexibleHeight = 0f;
                    spacer.minHeight = 6f;
                    spacer.preferredHeight = 6f;
                }
            }

            Button[] buttons = panel.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                bool primary = i == 0;
                StyleButtonV6(buttons[i], primary, primary ? 78f : 70f);
                Image image = buttons[i].GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = primary ? LumaBayArtPackV2.PrimaryButton : LumaBayArtPackV2.SecondaryButton;
                    image.type = Image.Type.Sliced;
                    image.color = Color.white;
                    image.preserveAspect = false;
                }
            }
        }

        private void RepairMainMenuV8()
        {
            RectTransform root = FindInRoot("VerticalScreen") as RectTransform;
            if (root == null) return;

            VerticalLayoutGroup layout = root.GetComponent<VerticalLayoutGroup>();
            if (layout != null)
            {
                layout.padding = new RectOffset(30, 30, 18, 18);
                layout.spacing = 10f;
            }

            SetPreferredV6(FindInRoot("LogoBlock") as RectTransform, 92f);

            RectTransform hero = FindInRoot("MainHero") as RectTransform;
            SetPreferredV6(hero, 404f);
            ScaleLighthouseV8(hero, new Vector2(-0.015f, 0.015f), new Vector2(1.015f, 1.035f));

            RectTransform caption = FindInRoot("HeroCaption") as RectTransform;
            if (caption != null)
            {
                caption.anchorMin = new Vector2(0.10f, 0.075f);
                caption.anchorMax = new Vector2(0.90f, 0.19f);
                caption.offsetMin = Vector2.zero;
                caption.offsetMax = Vector2.zero;
                ApplyPanelV7(caption.GetComponent<Image>(), LumaBayArtPackV2.CompactPanel, Color.white);
                Text captionText = caption.GetComponentInChildren<Text>(true);
                if (captionText != null)
                {
                    captionText.fontSize = 21;
                    captionText.resizeTextForBestFit = true;
                    captionText.resizeTextMinSize = 15;
                    captionText.resizeTextMaxSize = 21;
                }
            }

            RectTransform secondaryRow = FindInRoot("MainSecondaryRow") as RectTransform;
            SetPreferredV6(secondaryRow, 86f);
            HorizontalLayoutGroup row = secondaryRow != null ? secondaryRow.GetComponent<HorizontalLayoutGroup>() : null;
            if (row != null)
            {
                row.spacing = 14f;
                row.padding = new RectOffset(0, 0, 2, 2);
                row.childControlHeight = true;
                row.childForceExpandHeight = true;
                row.childForceExpandWidth = true;
            }

            foreach (ButtonRole marker in root.GetComponentsInChildren<ButtonRole>(true))
            {
                Button button = marker.GetComponent<Button>();
                if (button == null) continue;
                if (marker.Id == ButtonRoleId.Play) StyleButtonV6(button, true, 94f);
                else if (marker.Id == ButtonRoleId.Levels || marker.Id == ButtonRoleId.Settings)
                    StyleButtonV6(button, false, 82f);
            }

            foreach (Text text in root.GetComponentsInChildren<Text>(true))
            {
                if (text.text != null && text.text.StartsWith("LUMA BAY", StringComparison.OrdinalIgnoreCase))
                {
                    text.fontSize = 55;
                    text.resizeTextForBestFit = true;
                    text.resizeTextMinSize = 42;
                    text.resizeTextMaxSize = 55;
                }
                else if (text.text != null && text.text.Contains("★") && text.text.Contains("◆"))
                {
                    text.fontSize = 21;
                    text.color = GoldText;
                }
            }
        }

        private void RepairMapV8()
        {
            RectTransform root = FindInRoot("VerticalScreen") as RectTransform;
            if (root == null) return;

            VerticalLayoutGroup layout = root.GetComponent<VerticalLayoutGroup>();
            if (layout != null)
            {
                layout.padding = new RectOffset(20, 20, 14, 14);
                layout.spacing = 8f;
            }

            RectTransform header = FindInRoot("MapHeader") as RectTransform;
            SetPreferredV6(header, 78f);
            StyleBackButtonsV8(header);

            RectTransform lighthouseCard = FindInRoot("LighthouseMetaCard") as RectTransform;
            SetPreferredV6(lighthouseCard, 370f);
            ScaleLighthouseV8(lighthouseCard, new Vector2(-0.025f, -0.015f), new Vector2(1.025f, 1.035f));

            RectTransform task = FindInRoot("TaskCard") as RectTransform;
            SetPreferredV6(task, 174f);
            if (task != null)
            {
                VerticalLayoutGroup taskLayout = task.GetComponent<VerticalLayoutGroup>();
                if (taskLayout != null)
                {
                    taskLayout.padding = new RectOffset(24, 24, 18, 18);
                    taskLayout.spacing = 3f;
                    taskLayout.childControlHeight = true;
                    taskLayout.childForceExpandHeight = false;
                }

                Text[] taskTexts = task.GetComponentsInChildren<Text>(true);
                for (int i = 0; i < taskTexts.Length; i++)
                {
                    Text text = taskTexts[i];
                    if (i == 0)
                    {
                        text.fontSize = 16;
                        SetPreferredV6(text.rectTransform, 24f);
                    }
                    else if (i == 1)
                    {
                        text.fontSize = 25;
                        text.resizeTextMinSize = 19;
                        text.resizeTextMaxSize = 25;
                        SetPreferredV6(text.rectTransform, 38f);
                    }
                    else
                    {
                        text.fontSize = 17;
                        text.resizeTextMinSize = 14;
                        text.resizeTextMaxSize = 17;
                        SetPreferredV6(text.rectTransform, 52f);
                    }
                }
            }

            RectTransform progressRow = FindInRoot("MetaProgressRow") as RectTransform;
            SetPreferredV6(progressRow, 44f);
            if (progressRow != null)
            {
                Text label = progressRow.GetComponentInChildren<Text>(true);
                if (label != null)
                {
                    label.fontSize = 18;
                    SetWidthV8(label.rectTransform, 150f);
                }
            }

            foreach (ButtonRole marker in root.GetComponentsInChildren<ButtonRole>(true))
            {
                Button button = marker.GetComponent<Button>();
                if (button == null) continue;
                switch (marker.Id)
                {
                    case ButtonRoleId.Restore:
                        StyleButtonV6(button, button.interactable, 78f);
                        break;
                    case ButtonRoleId.StartLevel:
                        StyleButtonV6(button, true, 88f);
                        break;
                    case ButtonRoleId.Levels:
                        StyleButtonV6(button, false, 70f);
                        break;
                }
            }
        }

        private void RepairSettingsV8()
        {
            RectTransform root = FindInRoot("VerticalScreen") as RectTransform;
            if (root == null) return;

            RectTransform header = FindInRoot("SettingsHeader") as RectTransform;
            SetPreferredV6(header, 78f);
            StyleBackButtonsV8(header);

            RectTransform card = FindInRoot("SettingsCard") as RectTransform;
            SetPreferredV6(card, 440f);
            if (card != null)
            {
                VerticalLayoutGroup layout = card.GetComponent<VerticalLayoutGroup>();
                if (layout != null)
                {
                    layout.padding = new RectOffset(28, 28, 26, 26);
                    layout.spacing = 12f;
                }
            }

            foreach (ButtonRole marker in root.GetComponentsInChildren<ButtonRole>(true))
            {
                Button button = marker.GetComponent<Button>();
                if (button == null) continue;
                if (marker.Id == ButtonRoleId.SettingToggle || marker.Id == ButtonRoleId.Language)
                    StyleButtonV6(button, false, 72f);
                else if (marker.Id == ButtonRoleId.ResetProgress)
                    StyleButtonV6(button, false, 68f, new Color(0.72f, 0.28f, 0.30f, 1f));
            }
        }

        private void RepairLevelSelectorV8()
        {
            RectTransform header = FindInRoot("LevelsHeader") as RectTransform;
            if (header != null)
            {
                header.anchorMin = new Vector2(0.025f, 0.918f);
                header.anchorMax = new Vector2(0.975f, 0.99f);
                header.offsetMin = Vector2.zero;
                header.offsetMax = Vector2.zero;
                StyleBackButtonsV8(header);

                Text[] texts = header.GetComponentsInChildren<Text>(true);
                foreach (Text text in texts)
                {
                    if (text.text == "‹") continue;
                    text.resizeTextForBestFit = true;
                    text.resizeTextMinSize = 18;
                    text.resizeTextMaxSize = 32;
                }
            }

            RectTransform scrollRoot = FindInRoot("LevelScroll") as RectTransform;
            if (scrollRoot != null)
            {
                scrollRoot.anchorMin = new Vector2(0.025f, 0.018f);
                scrollRoot.anchorMax = new Vector2(0.975f, 0.904f);
                scrollRoot.offsetMin = Vector2.zero;
                scrollRoot.offsetMax = Vector2.zero;
            }
        }

        private void RepairGameplayV8()
        {
            RectTransform header = FindInRoot("GameHeader") as RectTransform;
            if (header != null)
            {
                header.anchorMin = new Vector2(0.018f, 0.932f);
                header.anchorMax = new Vector2(0.982f, 0.995f);
                header.offsetMin = Vector2.zero;
                header.offsetMax = Vector2.zero;
                StyleBackButtonsV8(header);

                Text[] headerTexts = header.GetComponentsInChildren<Text>(true);
                foreach (Text text in headerTexts)
                {
                    if (text.text == "‹") continue;
                    text.resizeTextForBestFit = true;
                    text.resizeTextMinSize = 17;
                    text.resizeTextMaxSize = 27;
                }
            }

            RectTransform goals = FindInRoot("Goals") as RectTransform;
            if (goals != null)
            {
                goals.anchorMin = new Vector2(0.026f, 0.838f);
                goals.anchorMax = new Vector2(0.974f, 0.924f);
                goals.offsetMin = Vector2.zero;
                goals.offsetMax = Vector2.zero;
                Text[] goalTexts = goals.GetComponentsInChildren<Text>(true);
                for (int i = 0; i < goalTexts.Length; i++)
                {
                    goalTexts[i].fontSize = i == 0 ? 20 : 19;
                    goalTexts[i].resizeTextMinSize = 15;
                    goalTexts[i].resizeTextMaxSize = i == 0 ? 20 : 19;
                }
            }

            RectTransform boardZone = FindInRoot("BoardZone") as RectTransform;
            if (boardZone != null)
            {
                boardZone.anchorMin = new Vector2(0f, 0.212f);
                boardZone.anchorMax = new Vector2(1f, 0.832f);
                boardZone.offsetMin = Vector2.zero;
                boardZone.offsetMax = Vector2.zero;
            }

            RectTransform boardFrame = FindInRoot("BoardFrame") as RectTransform;
            if (boardFrame != null)
            {
                float width = screenRoot.rect.width > 100f ? screenRoot.rect.width : 720f;
                float size = Mathf.Min(682f, width - 26f);
                boardFrame.sizeDelta = new Vector2(size, size);
            }

            RectTransform tray = FindInRoot("BoosterTray") as RectTransform;
            if (tray == null) return;

            tray.anchorMin = new Vector2(0.015f, 0.016f);
            tray.anchorMax = new Vector2(0.985f, 0.202f);
            tray.offsetMin = Vector2.zero;
            tray.offsetMax = Vector2.zero;
            ApplyPanelV7(tray.GetComponent<Image>(), LumaBayArtPackV2.BoosterTray, Color.white);

            HorizontalLayoutGroup row = tray.GetComponent<HorizontalLayoutGroup>();
            if (row != null)
            {
                row.padding = new RectOffset(24, 24, 26, 20);
                row.spacing = 6f;
                row.childControlWidth = true;
                row.childControlHeight = true;
                row.childForceExpandWidth = true;
                row.childForceExpandHeight = true;
            }

            foreach (Button booster in tray.GetComponentsInChildren<Button>(true))
            {
                LayoutElement element = booster.GetComponent<LayoutElement>();
                if (element == null) element = booster.gameObject.AddComponent<LayoutElement>();
                element.minWidth = 0f;
                element.preferredWidth = 112f;
                element.flexibleWidth = 1f;
                element.minHeight = 132f;
                element.preferredHeight = 132f;
                element.flexibleHeight = 1f;

                Image buttonImage = booster.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.sprite = null;
                    buttonImage.color = new Color(1f, 1f, 1f, 0.001f);
                }

                RectTransform iconRect = booster.transform.Find("PremiumBoosterIcon") as RectTransform;
                Image iconImage = iconRect != null ? iconRect.GetComponent<Image>() : null;
                if (iconRect != null)
                {
                    iconRect.anchorMin = new Vector2(0.10f, 0.23f);
                    iconRect.anchorMax = new Vector2(0.90f, 0.94f);
                    iconRect.offsetMin = Vector2.zero;
                    iconRect.offsetMax = Vector2.zero;
                }

                int price = 0;
                Text priceLabel = null;
                foreach (Text label in booster.GetComponentsInChildren<Text>(true))
                {
                    bool isPrice = label.text != null && label.text.Contains("◆");
                    label.gameObject.SetActive(isPrice);
                    if (!isPrice) continue;
                    priceLabel = label;
                    price = ParsePriceV8(label.text);
                    label.fontSize = 17;
                    label.resizeTextForBestFit = false;
                    label.alignment = TextAnchor.MiddleCenter;
                    label.rectTransform.anchorMin = new Vector2(0.04f, 0.015f);
                    label.rectTransform.anchorMax = new Vector2(0.96f, 0.25f);
                    label.rectTransform.offsetMin = Vector2.zero;
                    label.rectTransform.offsetMax = Vector2.zero;
                }

                bool affordable = save == null || price <= 0 || save.Coins >= price;
                if (iconImage != null) iconImage.color = affordable ? Color.white : new Color(0.48f, 0.55f, 0.58f, 0.82f);
                if (priceLabel != null) priceLabel.color = affordable ? GoldText : new Color(1f, 0.48f, 0.42f, 1f);
            }
        }

        private static void ScaleLighthouseV8(RectTransform card, Vector2 min, Vector2 max)
        {
            if (card == null) return;
            RectTransform lighthouse = card.Find("PremiumLighthouseVisual") as RectTransform;
            if (lighthouse == null) return;
            lighthouse.anchorMin = min;
            lighthouse.anchorMax = max;
            lighthouse.offsetMin = Vector2.zero;
            lighthouse.offsetMax = Vector2.zero;
            Image image = lighthouse.GetComponent<Image>();
            if (image != null)
            {
                image.preserveAspect = true;
                image.color = Color.white;
            }
        }

        private static void StyleBackButtonsV8(Transform root)
        {
            if (root == null) return;
            foreach (ButtonRole marker in root.GetComponentsInChildren<ButtonRole>(true))
            {
                if (marker.Id != ButtonRoleId.Back) continue;
                Button button = marker.GetComponent<Button>();
                if (button == null) continue;

                RectTransform rect = button.transform as RectTransform;
                SetPreferredV6(rect, 60f);
                SetWidthV8(rect, 60f);

                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = LumaBayArtPackV2.BackButton;
                    image.type = Image.Type.Simple;
                    image.preserveAspect = true;
                    image.color = Color.white;
                }

                Text label = button.GetComponentInChildren<Text>(true);
                if (label != null)
                {
                    label.fontSize = 34;
                    label.resizeTextForBestFit = false;
                    label.color = Color.white;
                }
            }
        }

        private static void SetWidthV8(RectTransform rect, float width)
        {
            if (rect == null) return;
            LayoutElement element = rect.GetComponent<LayoutElement>();
            if (element == null) element = rect.gameObject.AddComponent<LayoutElement>();
            element.minWidth = width;
            element.preferredWidth = width;
            element.flexibleWidth = 0f;
        }

        private static int ParsePriceV8(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            int result = 0;
            foreach (char character in value)
            {
                if (!char.IsDigit(character)) continue;
                result = result * 10 + (character - '0');
            }
            return result;
        }
    }
}
