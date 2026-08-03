using System;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private int finalVisualSignature = int.MinValue;

        private void ApplyFinalVisualPassIfNeeded()
        {
            if (screenRoot == null || !LumaBayArtPackV2.IsAvailable) return;

            int firstId = screenRoot.childCount > 0 ? screenRoot.GetChild(0).GetInstanceID() : 0;
            int signature = screenRoot.childCount * 19349663 ^ firstId;
            if (signature == finalVisualSignature)
            {
                PolishActiveModal();
                return;
            }

            finalVisualSignature = signature;
            PolishBoardSurface();
            PolishBoosterTrayFinal();
            PolishLevelCardsFinal();
            PolishTypographyFinal();
            PolishActiveModal();
        }

        private void PolishBoardSurface()
        {
            RectTransform boardFrame = FindInRoot("BoardFrame") as RectTransform;
            if (boardFrame != null)
            {
                Image frame = boardFrame.GetComponent<Image>();
                if (frame != null)
                {
                    frame.sprite = LumaBayArtPackV2.PanelLarge;
                    frame.type = Image.Type.Sliced;
                    frame.color = new Color(1f, 1f, 1f, 0.96f);
                }

                Shadow shadow = boardFrame.GetComponent<Shadow>();
                if (shadow != null)
                {
                    shadow.effectColor = new Color(0f, 0.01f, 0.03f, 0.48f);
                    shadow.effectDistance = new Vector2(0f, -5f);
                }
            }

            RectTransform board = FindInRoot("Board") as RectTransform;
            if (board != null)
            {
                Image background = board.GetComponent<Image>();
                if (background != null)
                {
                    background.sprite = null;
                    background.color = new Color(0.015f, 0.075f, 0.12f, 0.92f);
                }
            }

            Transform[] all = screenRoot.GetComponentsInChildren<Transform>(true);
            foreach (Transform item in all)
            {
                if (!item.name.StartsWith("Cell_", StringComparison.Ordinal)) continue;
                Image cell = item.GetComponent<Image>();
                if (cell == null) continue;

                cell.sprite = null;
                bool selected = cell.color.r > 0.25f;
                cell.color = selected
                    ? new Color(0.18f, 0.55f, 0.62f, 0.72f)
                    : new Color(0.08f, 0.22f, 0.30f, 0.34f);
            }
        }

        private void PolishBoosterTrayFinal()
        {
            RectTransform tray = FindInRoot("BoosterTray") as RectTransform;
            if (tray == null) return;

            Image trayImage = tray.GetComponent<Image>();
            if (trayImage != null && LumaBayArtPackV2.BoosterTray != null)
            {
                trayImage.sprite = LumaBayArtPackV2.BoosterTray;
                trayImage.type = Image.Type.Sliced;
                trayImage.color = Color.white;
            }

            HorizontalLayoutGroup row = tray.GetComponent<HorizontalLayoutGroup>();
            if (row != null)
            {
                row.padding = new RectOffset(14, 14, 12, 12);
                row.spacing = 7f;
                row.childAlignment = TextAnchor.MiddleCenter;
                row.childForceExpandWidth = true;
                row.childForceExpandHeight = true;
            }

            Button[] buttons = tray.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                RectTransform rect = button.transform as RectTransform;
                if (rect == null) continue;

                LayoutElement layout = rect.GetComponent<LayoutElement>();
                if (layout == null) layout = rect.gameObject.AddComponent<LayoutElement>();
                layout.minWidth = 92f;
                layout.preferredWidth = 112f;
                layout.flexibleWidth = 1f;
                layout.minHeight = 108f;
                layout.preferredHeight = 124f;

                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = LumaBayArtPackV2.SecondaryButton;
                    image.type = Image.Type.Sliced;
                    image.color = Color.white;
                }

                Text[] labels = button.GetComponentsInChildren<Text>(true);
                foreach (Text label in labels)
                {
                    bool price = label.text != null && label.text.IndexOf("◆", StringComparison.Ordinal) >= 0;
                    if (price)
                    {
                        label.gameObject.SetActive(true);
                        label.fontSize = 16;
                        label.color = new Color(1f, 0.86f, 0.48f, 1f);
                    }
                    else
                    {
                        label.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void PolishLevelCardsFinal()
        {
            RectTransform scrollRoot = FindInRoot("LevelScroll") as RectTransform;
            if (scrollRoot == null) return;

            ScrollRect scroll = scrollRoot.GetComponent<ScrollRect>();
            RectTransform content = scroll != null ? scroll.content : null;
            if (content == null) return;

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

                CanvasGroup cardGroup = card.GetComponent<CanvasGroup>();
                if (cardGroup == null) cardGroup = card.gameObject.AddComponent<CanvasGroup>();
                cardGroup.alpha = 1f;
                cardGroup.interactable = true;
                cardGroup.blocksRaycasts = true;

                Button button = card.GetComponent<Button>();
                bool unlocked = button == null || button.interactable;
                int levelId = i + 1;
                int stars = save != null && save.StarsByLevel != null && i < save.StarsByLevel.Count
                    ? save.StarsByLevel[i]
                    : 0;

                int spriteIndex;
                if (!unlocked) spriteIndex = 0;
                else if (save != null && levelId == save.UnlockedLevel) spriteIndex = 2;
                else spriteIndex = Mathf.Clamp(3 + stars, 3, 6);

                Image image = card.GetComponent<Image>();
                Sprite sprite = LumaBayArtPackV2.MapNode(spriteIndex);
                if (image != null && sprite != null)
                {
                    image.sprite = sprite;
                    image.type = Image.Type.Simple;
                    image.preserveAspect = true;
                    image.color = unlocked ? Color.white : new Color(0.66f, 0.70f, 0.73f, 0.94f);
                }

                Text label = card.GetComponentInChildren<Text>(true);
                if (label != null)
                {
                    label.gameObject.SetActive(true);
                    label.color = Color.white;
                    label.fontSize = 24;
                    label.resizeTextForBestFit = true;
                    label.resizeTextMinSize = 14;
                    label.resizeTextMaxSize = 24;
                }
            }

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            scroll.verticalNormalizedPosition = 1f;
        }

        private void PolishTypographyFinal()
        {
            Text[] labels = screenRoot.GetComponentsInChildren<Text>(true);
            foreach (Text label in labels)
            {
                if (label == null) continue;
                label.horizontalOverflow = HorizontalWrapMode.Wrap;
                label.verticalOverflow = VerticalWrapMode.Truncate;

                Shadow shadow = label.GetComponent<Shadow>();
                if (shadow != null)
                {
                    shadow.effectColor = new Color(0f, 0f, 0f, 0.62f);
                    shadow.effectDistance = new Vector2(1f, -1.5f);
                }

                if (label.fontSize >= 30)
                {
                    label.resizeTextForBestFit = true;
                    label.resizeTextMinSize = Mathf.Max(18, label.fontSize - 16);
                    label.resizeTextMaxSize = label.fontSize;
                }
            }
        }

        private void PolishActiveModal()
        {
            if (canvas == null) return;
            Transform overlay = canvas.transform.Find("ModalOverlay");
            if (overlay == null) return;

            overlay.SetAsLastSibling();
            RectTransform panel = null;
            RectTransform[] rects = overlay.GetComponentsInChildren<RectTransform>(true);
            foreach (RectTransform rect in rects)
            {
                if (rect == null || rect == overlay) continue;
                if (rect.name.IndexOf("Panel", StringComparison.OrdinalIgnoreCase) < 0 &&
                    rect.name.IndexOf("Modal", StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                panel = rect;
                break;
            }
            if (panel == null) return;

            panel.anchorMin = new Vector2(0.08f, 0.24f);
            panel.anchorMax = new Vector2(0.92f, 0.76f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;
            panel.localScale = Vector3.one;

            CanvasGroup group = panel.GetComponent<CanvasGroup>();
            if (group == null) group = panel.gameObject.AddComponent<CanvasGroup>();
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;

            Image image = panel.GetComponent<Image>();
            if (image != null && LumaBayArtPackV2.PanelLarge != null)
            {
                image.sprite = LumaBayArtPackV2.PanelLarge;
                image.type = Image.Type.Sliced;
                image.color = Color.white;
            }
        }
    }
}
