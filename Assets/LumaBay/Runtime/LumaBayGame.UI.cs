using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private void BuildLighthouseArt(RectTransform parent, int progress)
        {
            RectTransform art = CreateRect(parent, "LighthouseArt");
            art.anchorMin = new Vector2(0f, 0.29f);
            art.anchorMax = Vector2.one;
            art.offsetMin = new Vector2(12f, 4f);
            art.offsetMax = new Vector2(-12f, -12f);

            Image sky = CreateImage(art, "Sky", ProceduralArt.Rounded("sky", new Color(0.04f, 0.30f, 0.40f, 1f)), Color.white);
            Stretch(sky.rectTransform);

            for (int i = 0; i < 3; i++)
            {
                Image wave = CreateImage(art, $"Wave{i}", ProceduralArt.Rounded($"wave{i}", new Color(0.18f + i * 0.03f, 0.65f, 0.72f, 0.78f), 8), Color.white);
                RectTransform r = wave.rectTransform;
                r.anchorMin = new Vector2(-0.03f + i * 0.03f, 0.04f + i * 0.045f);
                r.anchorMax = new Vector2(1.03f, 0.11f + i * 0.045f);
                r.offsetMin = Vector2.zero;
                r.offsetMax = Vector2.zero;
            }

            Image island = CreateImage(art, "Island", ProceduralArt.Rounded("island", new Color(0.18f, 0.24f, 0.22f, 1f), 18), Color.white);
            island.rectTransform.anchorMin = new Vector2(0.18f, 0.12f);
            island.rectTransform.anchorMax = new Vector2(0.82f, 0.25f);
            island.rectTransform.offsetMin = Vector2.zero;
            island.rectTransform.offsetMax = Vector2.zero;

            Color towerColor = progress >= 50 ? new Color(0.92f, 0.89f, 0.78f) : new Color(0.58f, 0.60f, 0.56f);
            Image tower = CreateImage(art, "Tower", ProceduralArt.Rounded("tower", towerColor, 10), Color.white);
            tower.rectTransform.anchorMin = new Vector2(0.40f, 0.21f);
            tower.rectTransform.anchorMax = new Vector2(0.60f, 0.76f);
            tower.rectTransform.offsetMin = Vector2.zero;
            tower.rectTransform.offsetMax = Vector2.zero;

            for (int i = 0; i < 3; i++)
            {
                Image stripe = CreateImage(tower.rectTransform, $"Stripe{i}", ProceduralArt.Rounded($"stripe{i}",
                    progress > i * 20 ? ProceduralArt.Coral : new Color(0.30f, 0.31f, 0.30f), 5), Color.white);
                float bottom = 0.13f + i * 0.26f;
                stripe.rectTransform.anchorMin = new Vector2(0f, bottom);
                stripe.rectTransform.anchorMax = new Vector2(1f, bottom + 0.12f);
                stripe.rectTransform.offsetMin = Vector2.zero;
                stripe.rectTransform.offsetMax = Vector2.zero;
            }

            Image lampRoom = CreateImage(art, "LampRoom", ProceduralArt.Rounded("lamp_room", progress >= 80 ? ProceduralArt.Gold : new Color(0.35f, 0.38f, 0.38f), 10), Color.white);
            lampRoom.rectTransform.anchorMin = new Vector2(0.35f, 0.73f);
            lampRoom.rectTransform.anchorMax = new Vector2(0.65f, 0.86f);
            lampRoom.rectTransform.offsetMin = Vector2.zero;
            lampRoom.rectTransform.offsetMax = Vector2.zero;

            Image roof = CreateImage(art, "Roof", ProceduralArt.Rounded("roof", new Color(0.15f, 0.12f, 0.12f), 8), Color.white);
            roof.rectTransform.anchorMin = new Vector2(0.38f, 0.85f);
            roof.rectTransform.anchorMax = new Vector2(0.62f, 0.91f);
            roof.rectTransform.offsetMin = Vector2.zero;
            roof.rectTransform.offsetMax = Vector2.zero;

            if (progress >= 100)
            {
                Image leftBeam = CreateImage(art, "LeftBeam", null, new Color(1f, 0.84f, 0.35f, 0.35f));
                leftBeam.rectTransform.anchorMin = new Vector2(0.04f, 0.78f);
                leftBeam.rectTransform.anchorMax = new Vector2(0.42f, 0.82f);
                leftBeam.rectTransform.offsetMin = Vector2.zero;
                leftBeam.rectTransform.offsetMax = Vector2.zero;
                leftBeam.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 6f);

                Image rightBeam = CreateImage(art, "RightBeam", null, new Color(1f, 0.84f, 0.35f, 0.35f));
                rightBeam.rectTransform.anchorMin = new Vector2(0.58f, 0.78f);
                rightBeam.rectTransform.anchorMax = new Vector2(0.96f, 0.82f);
                rightBeam.rectTransform.offsetMin = Vector2.zero;
                rightBeam.rectTransform.offsetMax = Vector2.zero;
                rightBeam.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -6f);
            }
        }

        private void CreateProgressBar(Transform parent, float value)
        {
            RectTransform track = CreatePanel(parent, "ProgressTrack", new Color(0.02f, 0.07f, 0.10f, 0.90f));
            SetLayout(track, 28f);
            Image fill = CreateImage(track, "Fill", ProceduralArt.Rounded("progress_fill", ProceduralArt.Gold, 8), Color.white);
            fill.rectTransform.anchorMin = new Vector2(0f, 0f);
            fill.rectTransform.anchorMax = new Vector2(Mathf.Clamp01(value), 1f);
            fill.rectTransform.offsetMin = new Vector2(4f, 4f);
            fill.rectTransform.offsetMax = new Vector2(-4f, -4f);
        }

        private RectTransform CreateVerticalScreen(int padding, float spacing)
        {
            RectTransform root = CreateRect(screenRoot, "VerticalScreen");
            Stretch(root);
            VerticalLayoutGroup layout = root.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(padding, padding, padding, padding);
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return root;
        }

        private RectTransform CreateVertical(Transform parent, string name, float spacing, RectOffset padding)
        {
            RectTransform root = CreateRect(parent, name);
            VerticalLayoutGroup layout = root.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = padding;
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return root;
        }

        private RectTransform CreateHorizontal(Transform parent, string name, float spacing)
        {
            RectTransform root = CreateRect(parent, name);
            HorizontalLayoutGroup layout = root.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandHeight = true;
            layout.childForceExpandWidth = false;
            return root;
        }

        private RectTransform CreatePanel(Transform parent, string name, Color color)
        {
            RectTransform rect = CreateRect(parent, name);
            Image image = rect.gameObject.AddComponent<Image>();
            image.sprite = ProceduralArt.Rounded(name, color);
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            return rect;
        }

        private Button CreateButton(Transform parent, string label, Action action, Color background, Color foreground, int size)
        {
            RectTransform rect = CreateRect(parent, "Button");
            Image image = rect.gameObject.AddComponent<Image>();
            image.sprite = ProceduralArt.Rounded("button", background, 14);
            image.type = Image.Type.Sliced;
            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.07f, 1.07f, 1.07f);
            colors.pressedColor = new Color(0.82f, 0.86f, 0.88f);
            colors.disabledColor = new Color(0.42f, 0.46f, 0.48f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;
            button.onClick.AddListener(() =>
            {
                audioSynth?.PlayClick();
                action?.Invoke();
            });

            Text text = CreateText(rect, label, size, TextAnchor.MiddleCenter, foreground, FontStyle.Bold);
            Stretch(text.rectTransform, 8f);
            text.raycastTarget = false;
            return button;
        }

        private Text CreateText(Transform parent, string value, int size, TextAnchor anchor, Color color, FontStyle style)
        {
            RectTransform rect = CreateRect(parent, "Text");
            Text text = rect.gameObject.AddComponent<Text>();
            text.font = font;
            text.text = value;
            text.fontSize = size;
            text.alignment = anchor;
            text.color = color;
            text.fontStyle = style;
            text.resizeTextForBestFit = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private Image CreateImage(Transform parent, string name, Sprite sprite, Color color)
        {
            RectTransform rect = CreateRect(parent, name);
            Image image = rect.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.preserveAspect = sprite != null;
            return image;
        }

        private static RectTransform CreateRect(Transform parent, string name)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.localScale = Vector3.one;
            return rect;
        }

        private static void Stretch(RectTransform rect, float inset = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
        }

        private static void SetLayout(RectTransform rect, float preferredHeight = -1f, float preferredWidth = -1f)
        {
            LayoutElement element = rect.GetComponent<LayoutElement>();
            if (element == null) element = rect.gameObject.AddComponent<LayoutElement>();
            if (preferredHeight >= 0f) element.preferredHeight = preferredHeight;
            if (preferredWidth > 0f) element.preferredWidth = preferredWidth;
            if (preferredWidth == 1f) element.flexibleWidth = 1f;
        }

        private static void AddFlexibleSpacer(Transform parent, float flexibleHeight)
        {
            RectTransform spacer = CreateRect(parent, "Spacer");
            LayoutElement element = spacer.gameObject.AddComponent<LayoutElement>();
            element.flexibleHeight = flexibleHeight;
            element.minHeight = 1f;
        }

        private void ClearScreen()
        {
            StopAllCoroutines();
            for (int i = canvas.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = canvas.transform.GetChild(i);
                if (child.name == "Toast") Destroy(child.gameObject);
            }
            DestroyChildren(screenRoot);
            boardGrid = null;
            movesLabel = null;
            collectGoalLabel = null;
            fogGoalLabel = null;
            walletLabel = null;
        }

        private static void DestroyChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
    }
}
