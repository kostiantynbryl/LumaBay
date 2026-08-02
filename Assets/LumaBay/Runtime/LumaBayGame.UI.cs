using System;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private void BuildLighthouseArt(RectTransform parent, int progress)
        {
            RectTransform art = CreateRect(parent, "LighthouseArt");
            art.anchorMin = new Vector2(0.015f, 0.015f);
            art.anchorMax = new Vector2(0.985f, 0.985f);
            art.offsetMin = Vector2.zero;
            art.offsetMax = Vector2.zero;

            int state = Mathf.Clamp(Mathf.RoundToInt(progress / 100f * 31f), 0, 31);
            Sprite sprite = LumaBayArtPack.LighthouseState(state) ?? CoastalBackdropArt.Create();
            Image image = CreateImage(art, "LighthouseFallbackIllustration", sprite, Color.white);
            Stretch(image.rectTransform);
            image.preserveAspect = false;
            image.raycastTarget = false;
            image.gameObject.AddComponent<LighthouseIllustrationMotion>();
        }

        private void CreateProgressBar(Transform parent, float value)
        {
            RectTransform track = CreateRect(parent, "ProgressTrack");
            Image trackImage = track.gameObject.AddComponent<Image>();
            trackImage.sprite = LumaBayArtPack.ProgressTrack ?? ProceduralArt.Rounded("progress_track", new Color(0.01f, 0.06f, 0.11f, 0.96f), 14);
            trackImage.type = Image.Type.Sliced;
            SetLayout(track, 30f);

            Image fill = CreateImage(track, "Fill",
                LumaBayArtPack.ProgressFill ?? ProceduralArt.Rounded("progress_fill", NauticalTheme.Gold, 14), Color.white);
            fill.type = Image.Type.Sliced;
            fill.rectTransform.anchorMin = Vector2.zero;
            fill.rectTransform.anchorMax = new Vector2(Mathf.Clamp01(value), 1f);
            fill.rectTransform.offsetMin = new Vector2(3f, 3f);
            fill.rectTransform.offsetMax = new Vector2(-3f, -3f);
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
            image.sprite = ProceduralArt.OrnateFrame(name, color, true);
            image.type = Image.Type.Sliced;
            image.color = Color.white;

            if (!name.StartsWith("Cell_", StringComparison.Ordinal) &&
                !name.Contains("Progress", StringComparison.OrdinalIgnoreCase))
            {
                Shadow shadow = rect.gameObject.AddComponent<Shadow>();
                shadow.effectColor = new Color(0f, 0f, 0f, 0.32f);
                shadow.effectDistance = new Vector2(3f, -5f);
                shadow.useGraphicAlpha = true;
                AddEntrance(rect, 0f, new Vector2(0f, -12f), 0.985f);
            }
            return rect;
        }

        private Button CreateButton(Transform parent, string label, Action action, Color background, Color foreground, int size)
        {
            RectTransform rect = CreateRect(parent, "Button");
            Image image = rect.gameObject.AddComponent<Image>();
            float luminance = background.r * 0.30f + background.g * 0.59f + background.b * 0.11f;
            Sprite authored = luminance > 0.36f ? LumaBayArtPack.ButtonPrimary : LumaBayArtPack.ButtonSecondary;
            image.sprite = authored ?? ProceduralArt.OrnateFrame($"button_{label}", background, true);
            image.type = Image.Type.Sliced;
            image.color = authored != null ? Color.Lerp(Color.white, background, 0.16f) : Color.white;

            Shadow shadow = rect.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.34f);
            shadow.effectDistance = new Vector2(2f, -4f);
            shadow.useGraphicAlpha = true;

            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.06f, 1.06f, 1.06f, 1f);
            colors.pressedColor = new Color(0.84f, 0.88f, 0.91f, 1f);
            colors.selectedColor = Color.white;
            colors.disabledColor = new Color(0.40f, 0.44f, 0.48f, 0.74f);
            colors.fadeDuration = 0.07f;
            button.colors = colors;
            button.onClick.AddListener(() =>
            {
                audioSynth?.PlayClick();
                action?.Invoke();
            });
            rect.gameObject.AddComponent<UiPressFeedback>();

            Text text = CreateText(rect, label, size, TextAnchor.MiddleCenter, foreground, FontStyle.Bold);
            Stretch(text.rectTransform, 11f);
            text.raycastTarget = false;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(13, size - 9);
            text.resizeTextMaxSize = size;
            return button;
        }

        private Button CreateBoosterCard(Transform parent, string icon, string title, int cost, Action action, Color accent)
        {
            RectTransform card = CreateRect(parent, $"Booster_{title}");
            Image image = card.gameObject.AddComponent<Image>();
            image.sprite = LumaBayArtPack.BoosterCard ?? ProceduralArt.OrnateFrame($"booster_{title}", Color.Lerp(NauticalTheme.Navy, accent, 0.18f), true);
            image.type = Image.Type.Sliced;
            image.color = Color.white;

            Button button = card.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() =>
            {
                audioSynth?.PlayClick();
                action?.Invoke();
            });
            card.gameObject.AddComponent<UiPressFeedback>();

            VerticalLayoutGroup layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(7, 7, 8, 8);
            layout.spacing = 1f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            Text iconText = CreateText(card, icon, 31, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            SetLayout(iconText.rectTransform, 40f);
            Text titleText = CreateText(card, title, 15, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            titleText.resizeTextForBestFit = true;
            titleText.resizeTextMinSize = 11;
            titleText.resizeTextMaxSize = 15;
            SetLayout(titleText.rectTransform, 38f);
            Text costText = CreateText(card, $"◆ {cost}", 17, TextAnchor.MiddleCenter, NauticalTheme.GoldLight, FontStyle.Bold);
            SetLayout(costText.rectTransform, 26f);
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

            Shadow shadow = rect.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.70f);
            shadow.effectDistance = new Vector2(1.5f, -2f);
            shadow.useGraphicAlpha = true;
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
            if (preferredWidth > 1f) element.preferredWidth = preferredWidth;
            else if (Mathf.Approximately(preferredWidth, 1f)) element.flexibleWidth = 1f;
        }

        private static void AddFlexibleSpacer(Transform parent, float flexibleHeight)
        {
            RectTransform spacer = CreateRect(parent, "Spacer");
            LayoutElement element = spacer.gameObject.AddComponent<LayoutElement>();
            element.flexibleHeight = flexibleHeight;
            element.minHeight = 1f;
        }

        private static void AddEntrance(RectTransform rect, float delay, Vector2 offset, float startScale)
        {
            UiEntranceMotion motion = rect.gameObject.GetComponent<UiEntranceMotion>();
            if (motion == null) motion = rect.gameObject.AddComponent<UiEntranceMotion>();
            motion.Configure(delay, offset, startScale);
        }

        private void ClearScreen()
        {
            StopAllCoroutines();
            for (int i = canvas.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = canvas.transform.GetChild(i);
                if (child.name == "Toast" || child.name == "ModalOverlay") Destroy(child.gameObject);
            }
            DestroyChildren(screenRoot);
            boardGrid = null;
            movesLabel = null;
            collectGoalLabel = null;
            fogGoalLabel = null;
            walletLabel = null;
            artOverrideSignature = int.MinValue;
            configuredGoalsPanelId = int.MinValue;
            boardPresentationSignature = int.MinValue;
        }

        private static void DestroyChildren(Transform parent)
        {
            if (parent == null) return;
            for (int i = parent.childCount - 1; i >= 0; i--) Destroy(parent.GetChild(i).gameObject);
        }
    }
}
