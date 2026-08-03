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
            Sprite sprite = LumaBayArtPackV2.LighthouseState(state) ??
                            LumaBayArtPack.LighthouseState(state) ?? CoastalBackdropArt.Create();
            Image image = CreateImage(art, "LighthouseFallbackIllustration", sprite, Color.white);
            Stretch(image.rectTransform);
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.gameObject.AddComponent<LighthouseIllustrationMotion>();
        }

        private void CreateProgressBar(Transform parent, float value)
        {
            RectTransform track = CreateRect(parent, "ProgressTrack");
            Image trackImage = track.gameObject.AddComponent<Image>();
            trackImage.sprite = LumaBayArtPackV2.ProgressTrack ??
                                ProceduralArt.Rounded("progress_track", new Color(0.01f, 0.06f, 0.11f, 0.96f), 14);
            trackImage.type = Image.Type.Sliced;
            trackImage.color = Color.white;
            SetLayout(track, 30f);

            RectTransform fillRoot = CreateRect(track, "FillClip");
            fillRoot.anchorMin = new Vector2(0.055f, 0.19f);
            fillRoot.anchorMax = new Vector2(Mathf.Lerp(0.055f, 0.945f, Mathf.Clamp01(value)), 0.81f);
            fillRoot.offsetMin = Vector2.zero;
            fillRoot.offsetMax = Vector2.zero;

            Image fill = fillRoot.gameObject.AddComponent<Image>();
            fill.sprite = ProceduralArt.Rounded("progress_fill_v7", new Color(0.10f, 0.72f, 0.83f, 1f), 12);
            fill.type = Image.Type.Sliced;
            fill.color = Color.white;
            fill.raycastTarget = false;
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
            Sprite authored = ResolvePanelSprite(name);
            image.sprite = authored ?? ProceduralArt.OrnateFrame(name, color, true);
            image.type = Image.Type.Sliced;
            image.color = authored != null ? ResolvePanelTint(name, color) : Color.white;
            image.preserveAspect = false;

            if (!name.StartsWith("Cell_", StringComparison.Ordinal) &&
                !name.Contains("Progress", StringComparison.OrdinalIgnoreCase))
            {
                Shadow shadow = rect.gameObject.AddComponent<Shadow>();
                shadow.effectColor = new Color(0f, 0f, 0f, 0.30f);
                shadow.effectDistance = new Vector2(2f, -4f);
                shadow.useGraphicAlpha = true;
            }
            return rect;
        }

        private static Sprite ResolvePanelSprite(string name)
        {
            if (!LumaBayArtPackV2.IsAvailable) return null;
            return name switch
            {
                "BoosterTray" => LumaBayArtPackV2.BoosterTray,
                "Goals" => LumaBayArtPackV2.GoalsPanel,
                "TaskCard" => LumaBayArtPackV2.TaskPanel,
                "MapHeader" => LumaBayArtPackV2.HeaderPanel,
                "LevelsHeader" => LumaBayArtPackV2.HeaderPanel,
                "SettingsHeader" => LumaBayArtPackV2.HeaderPanel,
                "GameHeader" => LumaBayArtPackV2.HeaderPanel,
                "LongProgressTrack" => LumaBayArtPackV2.ProgressTrack,
                "ProgressTrack" => LumaBayArtPackV2.ProgressTrack,
                "MainHero" => LumaBayArtPackV2.PanelLarge,
                "LighthouseMetaCard" => LumaBayArtPackV2.PanelLarge,
                "SettingsCard" => LumaBayArtPackV2.PanelLarge,
                "LevelScroll" => LumaBayArtPackV2.PanelLarge,
                "ModalPanel" => LumaBayArtPackV2.PanelLarge,
                "BoardFrame" => LumaBayArtPackV2.PanelLarge,
                _ => LumaBayArtPackV2.MediumPanel
            };
        }

        private static Color ResolvePanelTint(string name, Color requested)
        {
            if (name == "BoosterTray") return Color.white;
            if (name == "LongProgressTrack" || name == "ProgressTrack") return Color.white;

            bool large = name == "MainHero" || name == "LighthouseMetaCard" ||
                         name == "SettingsCard" || name == "LevelScroll" ||
                         name == "ModalPanel" || name == "BoardFrame";
            Color baseTint = large
                ? new Color(0.16f, 0.34f, 0.42f, Mathf.Clamp01(requested.a))
                : new Color(0.20f, 0.46f, 0.55f, Mathf.Clamp01(requested.a));
            return baseTint;
        }

        private Button CreateButton(Transform parent, string label, Action action, Color background, Color foreground, int size)
        {
            RectTransform rect = CreateRect(parent, "Button");
            Image image = rect.gameObject.AddComponent<Image>();
            float luminance = background.r * 0.30f + background.g * 0.59f + background.b * 0.11f;
            bool primary = luminance > 0.36f;
            Sprite authored = primary ? LumaBayArtPackV2.PrimaryButton : LumaBayArtPackV2.SecondaryButton;
            if (authored == null)
                authored = primary ? LumaBayArtPack.ButtonPrimary : LumaBayArtPack.ButtonSecondary;

            image.sprite = authored ?? ProceduralArt.OrnateFrame($"button_{label}", background, true);
            image.type = Image.Type.Sliced;
            image.color = authored != null
                ? (primary ? Color.white : Color.Lerp(new Color(0.12f, 0.38f, 0.48f, 1f), background, 0.46f))
                : Color.white;
            image.preserveAspect = false;

            Shadow shadow = rect.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.32f);
            shadow.effectDistance = new Vector2(2f, -4f);
            shadow.useGraphicAlpha = true;

            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.04f, 1.04f, 1.04f, 1f);
            colors.pressedColor = new Color(0.78f, 0.86f, 0.89f, 1f);
            colors.selectedColor = Color.white;
            colors.disabledColor = new Color(0.30f, 0.36f, 0.40f, 0.70f);
            colors.fadeDuration = 0.06f;
            button.colors = colors;
            button.onClick.AddListener(() =>
            {
                audioSynth?.PlayClick();
                action?.Invoke();
            });
            rect.gameObject.AddComponent<UiPressFeedback>();

            Text text = CreateText(rect, label, size, TextAnchor.MiddleCenter, foreground, FontStyle.Bold);
            Stretch(text.rectTransform, 16f);
            text.raycastTarget = false;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(13, size - 9);
            text.resizeTextMaxSize = size;
            return button;
        }

        private Button CreateBoosterCard(Transform parent, string icon, string title, int cost, Action action, Color accent)
        {
            string boosterId = ResolveBoosterIdByCost(cost);
            RectTransform card = CreateRect(parent, $"Booster_{boosterId}");
            Image image = card.gameObject.AddComponent<Image>();
            image.sprite = null;
            image.color = new Color(1f, 1f, 1f, 0.001f);

            Button button = card.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() =>
            {
                audioSynth?.PlayClick();
                action?.Invoke();
            });
            card.gameObject.AddComponent<UiPressFeedback>();
            ButtonRole role = card.gameObject.AddComponent<ButtonRole>();
            role.Id = ButtonRoleId.Booster;

            Sprite boosterSprite = LumaBayArtPackV2.Booster(boosterId) ?? LumaBayArtPack.Booster(boosterId);
            Image boosterIcon = CreateImage(card, "PremiumBoosterIcon", boosterSprite, Color.white);
            boosterIcon.rectTransform.anchorMin = new Vector2(0.14f, 0.22f);
            boosterIcon.rectTransform.anchorMax = new Vector2(0.86f, 0.90f);
            boosterIcon.rectTransform.offsetMin = Vector2.zero;
            boosterIcon.rectTransform.offsetMax = Vector2.zero;
            boosterIcon.preserveAspect = true;
            boosterIcon.raycastTarget = false;

            Text costText = CreateText(card, $"◆ {cost}", 16, TextAnchor.MiddleCenter,
                NauticalTheme.GoldLight, FontStyle.Bold);
            costText.rectTransform.anchorMin = new Vector2(0.05f, 0.01f);
            costText.rectTransform.anchorMax = new Vector2(0.95f, 0.23f);
            costText.rectTransform.offsetMin = Vector2.zero;
            costText.rectTransform.offsetMax = Vector2.zero;
            costText.raycastTarget = false;
            return button;
        }

        private static string ResolveBoosterIdByCost(int cost)
        {
            return cost switch
            {
                200 => "lightning",
                300 => "anchor",
                50 => "shuffle",
                100 => "extra_moves",
                250 => "harpoon",
                _ => "lightning"
            };
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
            buttonRoleSignature = int.MinValue;
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
