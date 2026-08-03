using System;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    [DefaultExecutionOrder(9000)]
    [DisallowMultipleComponent]
    public sealed class LumaBayMainMenuReleaseV10 : MonoBehaviour
    {
        private const float LogoHeight = 164f;
        private const float HeroHeight = 612f;
        private const float PlayHeight = 126f;
        private const float NavigationHeight = 144f;
        private const float WalletHeight = 40f;
        private const float VersionHeight = 24f;

        private LumaBayGame game;
        private Canvas canvas;
        private int appliedRootId;
        private Font font;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            LumaBayGame target = FindFirstObjectByType<LumaBayGame>();
            if (target != null && target.GetComponent<LumaBayMainMenuReleaseV10>() == null)
                target.gameObject.AddComponent<LumaBayMainMenuReleaseV10>();
        }

        private void Awake()
        {
            game = GetComponent<LumaBayGame>();
        }

        private void LateUpdate()
        {
            if (game == null) return;
            if (canvas == null) canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null) return;

            Transform screenRoot = FindRecursive(canvas.transform, "ScreenRoot");
            Transform mainHero = screenRoot != null ? FindRecursive(screenRoot, "MainHero") : null;
            if (screenRoot == null || mainHero == null)
            {
                appliedRootId = 0;
                return;
            }

            int rootId = screenRoot.GetInstanceID();
            if (appliedRootId == rootId && FindRecursive(screenRoot, "ReleaseMenuV10Marker") != null)
                return;

            if (Apply(screenRoot, mainHero))
                appliedRootId = rootId;
        }

        private bool Apply(Transform screenRoot, Transform mainHero)
        {
            RectTransform vertical = FindRecursive(screenRoot, "VerticalScreen") as RectTransform;
            if (vertical == null) return false;

            RectTransform logoBlock = FindRecursive(vertical, "LogoBlock") as RectTransform;
            if (logoBlock != null) RebuildLogo(logoBlock);

            HideSubtitle(vertical);

            RectTransform hero = mainHero as RectTransform;
            Image heroBackground = hero != null ? hero.GetComponent<Image>() : null;
            if (heroBackground != null)
            {
                Sprite panel = LumaBayArtPackV2.PanelLarge;
                if (panel != null) heroBackground.sprite = panel;
                heroBackground.type = Image.Type.Sliced;
                heroBackground.color = Color.white;
                heroBackground.preserveAspect = false;
            }

            RectTransform caption = FindRecursive(mainHero, "HeroCaption") as RectTransform;
            if (caption != null) RebuildHeroCaption(caption);

            Button play = FindFirstButtonDirect(vertical);
            if (play != null) StylePlayButton(play);

            RectTransform row = FindRecursive(vertical, "MainSecondaryRow") as RectTransform;
            if (row != null) RebuildBottomNavigation(row);

            CreateTopBubble(screenRoot, "SettingsBubble", new Vector2(0.075f, 0.958f), "⚙", () =>
                game.SendMessage("ShowSettings", SendMessageOptions.DontRequireReceiver));
            CreateTopBubble(screenRoot, "AccountBubble", new Vector2(0.925f, 0.958f), "●", () =>
                ShowPlaceholder("Аккаунт", "Профиль и облачная синхронизация появятся в одной из следующих версий."));

            RectTransform wallet = FindWallet(vertical);
            if (wallet != null)
            {
                Text text = wallet.GetComponent<Text>();
                if (text != null)
                {
                    text.fontSize = 22;
                    text.resizeTextForBestFit = true;
                    text.resizeTextMinSize = 18;
                    text.resizeTextMaxSize = 22;
                    text.color = new Color(1f, 0.86f, 0.48f, 1f);
                }
            }

            RectTransform version = FindVersion(vertical);
            if (version != null)
            {
                Text text = version.GetComponent<Text>();
                if (text != null)
                {
                    text.fontSize = 16;
                    text.resizeTextForBestFit = true;
                    text.resizeTextMinSize = 13;
                    text.resizeTextMaxSize = 16;
                    text.color = new Color(0.74f, 0.86f, 0.90f, 0.92f);
                }
            }

            GameObject marker = new GameObject("ReleaseMenuV10Marker");
            marker.transform.SetParent(screenRoot, false);

            MaintainGeometry(screenRoot);
            return true;
        }

        private void RebuildLogo(RectTransform logoBlock)
        {
            DestroyChildren(logoBlock);

            Image rootImage = logoBlock.GetComponent<Image>();
            if (rootImage == null) rootImage = logoBlock.gameObject.AddComponent<Image>();
            rootImage.sprite = null;
            rootImage.color = new Color(1f, 1f, 1f, 0.001f);
            rootImage.raycastTarget = false;

            Sprite frameSprite = LumaBayArtPackV2.CompactPanel ?? LumaBayArtPackV2.MediumPanel;

            GameObject glowObject = new GameObject("LogoGlow", typeof(RectTransform), typeof(Image));
            glowObject.transform.SetParent(logoBlock, false);
            RectTransform glowRect = glowObject.GetComponent<RectTransform>();
            glowRect.anchorMin = new Vector2(0.245f, 0.035f);
            glowRect.anchorMax = new Vector2(0.755f, 0.945f);
            glowRect.offsetMin = new Vector2(-7f, -4f);
            glowRect.offsetMax = new Vector2(7f, 4f);
            Image glow = glowObject.GetComponent<Image>();
            glow.sprite = frameSprite;
            glow.type = Image.Type.Sliced;
            glow.color = new Color(0.03f, 0.20f, 0.27f, 0.68f);
            glow.raycastTarget = false;

            Shadow shadow = glowObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.46f);
            shadow.effectDistance = new Vector2(2f, -5f);
            shadow.useGraphicAlpha = true;

            GameObject maskObject = new GameObject("LogoMask", typeof(RectTransform), typeof(Image), typeof(Mask));
            maskObject.transform.SetParent(logoBlock, false);
            RectTransform maskRect = maskObject.GetComponent<RectTransform>();
            maskRect.anchorMin = new Vector2(0.27f, 0.06f);
            maskRect.anchorMax = new Vector2(0.73f, 0.94f);
            maskRect.offsetMin = Vector2.zero;
            maskRect.offsetMax = Vector2.zero;

            Image maskImage = maskObject.GetComponent<Image>();
            maskImage.sprite = frameSprite;
            maskImage.type = Image.Type.Sliced;
            maskImage.color = Color.white;
            maskImage.raycastTarget = false;
            maskObject.GetComponent<Mask>().showMaskGraphic = false;

            Sprite logoSprite = LumaBayEmbeddedBranding.Logo;
            if (logoSprite != null)
            {
                GameObject logoObject = new GameObject("LogoArtwork", typeof(RectTransform), typeof(RawImage));
                logoObject.transform.SetParent(maskObject.transform, false);
                RawImage logo = logoObject.GetComponent<RawImage>();
                logo.texture = logoSprite.texture;
                logo.uvRect = new Rect(0f, 0.23f, 1f, 0.54f);
                logo.color = Color.white;
                logo.raycastTarget = false;
                Stretch(logo.rectTransform);
            }
            else
            {
                Text fallback = CreateText(maskObject.transform, "LUMA BAY", 54, TextAnchor.MiddleCenter);
                Stretch(fallback.rectTransform, 10f);
                fallback.color = new Color(1f, 0.84f, 0.42f, 1f);
                fallback.fontStyle = FontStyle.Bold;
            }
        }

        private void RebuildHeroCaption(RectTransform caption)
        {
            Image background = caption.GetComponent<Image>();
            if (background != null)
            {
                Sprite panel = LumaBayArtPackV2.CompactPanel;
                if (panel != null) background.sprite = panel;
                background.type = Image.Type.Sliced;
                background.color = Color.white;
            }

            Text captionText = caption.GetComponentInChildren<Text>(true);
            if (captionText != null)
            {
                captionText.rectTransform.anchorMin = new Vector2(0.06f, 0.41f);
                captionText.rectTransform.anchorMax = new Vector2(0.94f, 0.92f);
                captionText.rectTransform.offsetMin = Vector2.zero;
                captionText.rectTransform.offsetMax = Vector2.zero;
                captionText.fontSize = 22;
                captionText.resizeTextForBestFit = true;
                captionText.resizeTextMinSize = 16;
                captionText.resizeTextMaxSize = 22;
                captionText.alignment = TextAnchor.MiddleCenter;
                captionText.color = Color.white;
                captionText.fontStyle = FontStyle.Bold;
            }

            Transform oldProgress = caption.Find("HeroProgressBar");
            if (oldProgress != null)
            {
                oldProgress.gameObject.SetActive(false);
                Destroy(oldProgress.gameObject);
            }

            float progress = captionText != null ? ExtractProgress(captionText.text) : 0f;

            GameObject trackObject = new GameObject("HeroProgressBar", typeof(RectTransform), typeof(Image));
            trackObject.transform.SetParent(caption, false);
            RectTransform track = trackObject.GetComponent<RectTransform>();
            track.anchorMin = new Vector2(0.12f, 0.13f);
            track.anchorMax = new Vector2(0.88f, 0.31f);
            track.offsetMin = Vector2.zero;
            track.offsetMax = Vector2.zero;

            Image trackBackground = trackObject.GetComponent<Image>();
            trackBackground.sprite = null;
            trackBackground.color = new Color(0.01f, 0.08f, 0.12f, 0.92f);
            trackBackground.raycastTarget = false;

            GameObject fillObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillObject.transform.SetParent(track, false);
            RectTransform fill = fillObject.GetComponent<RectTransform>();
            fill.anchorMin = new Vector2(0.045f, 0.25f);
            fill.anchorMax = new Vector2(Mathf.Lerp(0.045f, 0.955f, progress), 0.75f);
            fill.offsetMin = Vector2.zero;
            fill.offsetMax = Vector2.zero;
            Image fillImage = fillObject.GetComponent<Image>();
            fillImage.sprite = null;
            fillImage.color = new Color(0.13f, 0.73f, 0.79f, 1f);
            fillImage.raycastTarget = false;

            GameObject frameObject = new GameObject("Frame", typeof(RectTransform), typeof(Image));
            frameObject.transform.SetParent(track, false);
            Image frame = frameObject.GetComponent<Image>();
            frame.sprite = LumaBayArtPackV2.ProgressTrack;
            frame.type = Image.Type.Sliced;
            frame.color = frame.sprite != null ? Color.white : new Color(1f, 1f, 1f, 0.001f);
            frame.raycastTarget = false;
            Stretch(frame.rectTransform);
        }

        private void StylePlayButton(Button play)
        {
            StyleButton(play, LumaBayArtPackV2.PrimaryButton, 34);
            Text text = play.GetComponentInChildren<Text>(true);
            if (text != null)
            {
                Stretch(text.rectTransform, 24f);
                text.fontStyle = FontStyle.Bold;
                text.color = Color.white;
            }
        }

        private void RebuildBottomNavigation(RectTransform row)
        {
            DestroyChildren(row);

            Image background = row.GetComponent<Image>();
            if (background == null) background = row.gameObject.AddComponent<Image>();
            background.sprite = LumaBayArtPackV2.BottomNavigation ?? LumaBayArtPackV2.PanelLarge;
            background.type = Image.Type.Sliced;
            background.color = Color.white;
            background.raycastTarget = false;

            Shadow shadow = row.GetComponent<Shadow>();
            if (shadow == null) shadow = row.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.36f);
            shadow.effectDistance = new Vector2(2f, -5f);
            shadow.useGraphicAlpha = true;

            HorizontalLayoutGroup horizontal = row.GetComponent<HorizontalLayoutGroup>();
            if (horizontal == null) horizontal = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            horizontal.padding = new RectOffset(22, 22, 12, 12);
            horizontal.spacing = 4f;
            horizontal.childAlignment = TextAnchor.MiddleCenter;
            horizontal.childControlWidth = true;
            horizontal.childControlHeight = true;
            horizontal.childForceExpandWidth = true;
            horizontal.childForceExpandHeight = true;

            CreateMenuTab(row, "КАРТА", "⌖", () =>
                game.SendMessage("ShowMap", SendMessageOptions.DontRequireReceiver));
            CreateMenuTab(row, "УРОВНИ", "★", () =>
                game.SendMessage("ShowLevelSelect", SendMessageOptions.DontRequireReceiver));
            CreateMenuTab(row, "МАГАЗИН", "◆", () =>
                ShowPlaceholder("Магазин", "Каталог товаров будет подключён на следующем этапе."));
        }

        private void CreateMenuTab(Transform parent, string label, string glyph, Action action)
        {
            GameObject go = new GameObject("Button_" + label, typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            go.transform.SetParent(parent, false);

            Image touchTarget = go.GetComponent<Image>();
            touchTarget.sprite = null;
            touchTarget.color = new Color(1f, 1f, 1f, 0.001f);

            Button button = go.GetComponent<Button>();
            button.targetGraphic = touchTarget;
            button.onClick.AddListener(() => action?.Invoke());
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 1f, 1f, 0.92f);
            colors.pressedColor = new Color(0.74f, 0.86f, 0.90f, 0.88f);
            colors.selectedColor = Color.white;
            colors.fadeDuration = 0.07f;
            button.colors = colors;

            LayoutElement element = go.GetComponent<LayoutElement>();
            element.minWidth = 0f;
            element.flexibleWidth = 1f;
            element.minHeight = 112f;
            element.preferredHeight = 112f;
            element.flexibleHeight = 1f;

            GameObject plateObject = new GameObject("IconPlate", typeof(RectTransform), typeof(Image));
            plateObject.transform.SetParent(go.transform, false);
            RectTransform plateRect = plateObject.GetComponent<RectTransform>();
            plateRect.anchorMin = new Vector2(0.5f, 0.47f);
            plateRect.anchorMax = new Vector2(0.5f, 0.47f);
            plateRect.pivot = new Vector2(0.5f, 0.5f);
            plateRect.sizeDelta = new Vector2(68f, 68f);
            plateRect.anchoredPosition = Vector2.zero;

            Image plate = plateObject.GetComponent<Image>();
            plate.sprite = LumaBayArtPackV2.CompactButton;
            plate.type = Image.Type.Sliced;
            plate.color = Color.white;
            plate.raycastTarget = false;

            Text icon = CreateText(plateObject.transform, glyph, 34, TextAnchor.MiddleCenter);
            Stretch(icon.rectTransform, 8f);
            icon.color = new Color(1f, 0.83f, 0.39f, 1f);
            icon.fontStyle = FontStyle.Bold;

            Text title = CreateText(go.transform, label, 18, TextAnchor.MiddleCenter);
            title.rectTransform.anchorMin = new Vector2(0.02f, 0.02f);
            title.rectTransform.anchorMax = new Vector2(0.98f, 0.29f);
            title.rectTransform.offsetMin = Vector2.zero;
            title.rectTransform.offsetMax = Vector2.zero;
            title.fontStyle = FontStyle.Bold;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 14;
            title.resizeTextMaxSize = 18;
        }

        private void CreateTopBubble(Transform root, string name, Vector2 anchor, string glyph, Action action)
        {
            Transform existing = FindRecursive(root, name);
            if (existing != null) return;

            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(root, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(74f, 74f);
            rect.anchoredPosition = Vector2.zero;

            Image image = go.GetComponent<Image>();
            image.sprite = LumaBayArtPackV2.CompactButton ?? LumaBayArtPackV2.BackButton;
            image.type = Image.Type.Sliced;
            image.color = Color.white;

            Shadow shadow = go.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.42f);
            shadow.effectDistance = new Vector2(2f, -4f);
            shadow.useGraphicAlpha = true;

            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => action?.Invoke());

            Text text = CreateText(go.transform, glyph, glyph == "⚙" ? 34 : 30, TextAnchor.MiddleCenter);
            Stretch(text.rectTransform, 10f);
            text.color = new Color(1f, 0.84f, 0.42f, 1f);
            text.fontStyle = FontStyle.Bold;
        }

        private void ShowPlaceholder(string title, string body)
        {
            if (canvas == null) return;
            Transform old = canvas.transform.Find("ReleasePlaceholder");
            if (old != null) Destroy(old.gameObject);

            GameObject overlay = new GameObject("ReleasePlaceholder", typeof(RectTransform), typeof(Image), typeof(Button));
            overlay.transform.SetParent(canvas.transform, false);
            RectTransform root = overlay.GetComponent<RectTransform>();
            Stretch(root);
            overlay.GetComponent<Image>().color = new Color(0f, 0.02f, 0.05f, 0.80f);
            overlay.GetComponent<Button>().onClick.AddListener(() => Destroy(overlay));

            GameObject panelObject = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panelObject.transform.SetParent(overlay.transform, false);
            RectTransform panel = panelObject.GetComponent<RectTransform>();
            panel.anchorMin = new Vector2(0.10f, 0.36f);
            panel.anchorMax = new Vector2(0.90f, 0.64f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;
            Image panelImage = panelObject.GetComponent<Image>();
            panelImage.sprite = LumaBayArtPackV2.PanelLarge;
            panelImage.type = Image.Type.Sliced;
            panelImage.color = Color.white;

            Text titleText = CreateText(panel, title, 32, TextAnchor.MiddleCenter);
            titleText.rectTransform.anchorMin = new Vector2(0.08f, 0.62f);
            titleText.rectTransform.anchorMax = new Vector2(0.92f, 0.92f);
            titleText.rectTransform.offsetMin = Vector2.zero;
            titleText.rectTransform.offsetMax = Vector2.zero;
            titleText.color = new Color(1f, 0.84f, 0.42f, 1f);
            titleText.fontStyle = FontStyle.Bold;

            Text bodyText = CreateText(panel, body, 21, TextAnchor.MiddleCenter);
            bodyText.rectTransform.anchorMin = new Vector2(0.10f, 0.16f);
            bodyText.rectTransform.anchorMax = new Vector2(0.90f, 0.64f);
            bodyText.rectTransform.offsetMin = Vector2.zero;
            bodyText.rectTransform.offsetMax = Vector2.zero;
        }

        private Text CreateText(Transform parent, string value, int size, TextAnchor alignment)
        {
            GameObject go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            Text text = go.GetComponent<Text>();
            if (font == null)
            {
                Text source = canvas != null ? canvas.GetComponentInChildren<Text>(true) : null;
                font = source != null ? source.font : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            text.font = font;
            text.text = value;
            text.fontSize = size;
            text.alignment = alignment;
            text.color = Color.white;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(12, size - 8);
            text.resizeTextMaxSize = size;
            text.raycastTarget = false;

            Shadow shadow = go.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.70f);
            shadow.effectDistance = new Vector2(1.5f, -2f);
            shadow.useGraphicAlpha = true;
            return text;
        }

        internal static void MaintainGeometry(Transform screenRoot)
        {
            if (screenRoot == null) return;

            RectTransform vertical = FindRecursive(screenRoot, "VerticalScreen") as RectTransform;
            RectTransform logo = FindRecursive(screenRoot, "LogoBlock") as RectTransform;
            RectTransform hero = FindRecursive(screenRoot, "MainHero") as RectTransform;
            RectTransform row = FindRecursive(screenRoot, "MainSecondaryRow") as RectTransform;
            RectTransform lighthouse = hero != null ? FindRecursive(hero, "PremiumLighthouseVisual") as RectTransform : null;
            RectTransform caption = hero != null ? FindRecursive(hero, "HeroCaption") as RectTransform : null;

            SetHeight(logo, LogoHeight);
            SetHeight(hero, HeroHeight);
            SetHeight(row, NavigationHeight);

            if (vertical != null)
            {
                VerticalLayoutGroup layout = vertical.GetComponent<VerticalLayoutGroup>();
                if (layout != null)
                {
                    layout.padding = new RectOffset(18, 18, 8, 10);
                    layout.spacing = 6f;
                    layout.childAlignment = TextAnchor.MiddleCenter;
                    layout.childForceExpandHeight = false;
                    layout.childForceExpandWidth = true;
                }

                CollapseSpacers(vertical);

                Button play = FindFirstButtonDirect(vertical);
                if (play != null)
                {
                    SetHeight(play.transform as RectTransform, PlayHeight);
                    StyleButton(play, LumaBayArtPackV2.PrimaryButton, 34);
                }

                RectTransform wallet = FindWallet(vertical);
                SetHeight(wallet, WalletHeight);
                RectTransform version = FindVersion(vertical);
                SetHeight(version, VersionHeight);
            }

            if (lighthouse != null)
            {
                lighthouse.anchorMin = new Vector2(0.025f, 0.055f);
                lighthouse.anchorMax = new Vector2(0.975f, 0.988f);
                lighthouse.offsetMin = Vector2.zero;
                lighthouse.offsetMax = Vector2.zero;
                Image image = lighthouse.GetComponent<Image>();
                if (image != null)
                {
                    image.preserveAspect = true;
                    image.raycastTarget = false;
                }
            }

            if (caption != null)
            {
                caption.anchorMin = new Vector2(0.075f, 0.022f);
                caption.anchorMax = new Vector2(0.925f, 0.165f);
                caption.offsetMin = Vector2.zero;
                caption.offsetMax = Vector2.zero;
            }

            if (row != null)
            {
                Image nav = row.GetComponent<Image>();
                if (nav != null)
                {
                    nav.sprite = LumaBayArtPackV2.BottomNavigation ?? LumaBayArtPackV2.PanelLarge;
                    nav.type = Image.Type.Sliced;
                    nav.color = Color.white;
                }

                HorizontalLayoutGroup horizontal = row.GetComponent<HorizontalLayoutGroup>();
                if (horizontal != null)
                {
                    horizontal.padding = new RectOffset(22, 22, 12, 12);
                    horizontal.spacing = 4f;
                    horizontal.childForceExpandWidth = true;
                    horizontal.childForceExpandHeight = true;
                    horizontal.childControlWidth = true;
                    horizontal.childControlHeight = true;
                }
            }

            Canvas.ForceUpdateCanvases();
            if (vertical != null) LayoutRebuilder.ForceRebuildLayoutImmediate(vertical);
        }

        private static void CollapseSpacers(RectTransform vertical)
        {
            for (int i = 0; i < vertical.childCount; i++)
            {
                Transform child = vertical.GetChild(i);
                if (child.name != "Spacer") continue;
                RectTransform rect = child as RectTransform;
                SetHeight(rect, 2f);
                LayoutElement element = child.GetComponent<LayoutElement>();
                if (element != null) element.flexibleHeight = 0f;
            }
        }

        private static float ExtractProgress(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0f;
            int percent = value.LastIndexOf('%');
            if (percent <= 0) return 0f;

            int start = percent - 1;
            while (start >= 0 && char.IsDigit(value[start])) start--;
            start++;

            if (start >= percent) return 0f;
            if (!int.TryParse(value.Substring(start, percent - start), out int parsed)) return 0f;
            return Mathf.Clamp01(parsed / 100f);
        }

        private static void HideSubtitle(RectTransform vertical)
        {
            for (int i = 0; i < vertical.childCount; i++)
            {
                Transform child = vertical.GetChild(i);
                Text text = child.GetComponent<Text>();
                if (text == null) continue;
                if (text.text != null &&
                    (text.text.Contains("История маяка") || text.text.Contains("Lighthouse Story")))
                    child.gameObject.SetActive(false);
            }
        }

        private static Button FindFirstButtonDirect(RectTransform parent)
        {
            if (parent == null) return null;
            for (int i = 0; i < parent.childCount; i++)
            {
                Button button = parent.GetChild(i).GetComponent<Button>();
                if (button != null) return button;
            }
            return null;
        }

        private static RectTransform FindWallet(RectTransform root)
        {
            if (root == null) return null;
            foreach (Text text in root.GetComponentsInChildren<Text>(true))
                if (text.text != null && text.text.Contains("★") && text.text.Contains("◆"))
                    return text.rectTransform;
            return null;
        }

        private static RectTransform FindVersion(RectTransform root)
        {
            if (root == null) return null;
            foreach (Text text in root.GetComponentsInChildren<Text>(true))
                if (text.text != null && text.text.Contains("Norvexa Games"))
                    return text.rectTransform;
            return null;
        }

        private static void StyleButton(Button button, Sprite sprite, int fontSize)
        {
            if (button == null) return;
            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                if (sprite != null) image.sprite = sprite;
                image.type = Image.Type.Sliced;
                image.color = Color.white;
                image.preserveAspect = false;
            }

            Text text = button.GetComponentInChildren<Text>(true);
            if (text != null)
            {
                text.fontSize = fontSize;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 20;
                text.resizeTextMaxSize = fontSize;
                text.fontStyle = FontStyle.Bold;
            }
        }

        private static void SetHeight(RectTransform rect, float height)
        {
            if (rect == null) return;
            LayoutElement element = rect.GetComponent<LayoutElement>();
            if (element == null) element = rect.gameObject.AddComponent<LayoutElement>();
            element.minHeight = height;
            element.preferredHeight = height;
            element.flexibleHeight = 0f;
        }

        private static Transform FindRecursive(Transform root, string name)
        {
            if (root == null) return null;
            if (root.name == name) return root;
            for (int i = 0; i < root.childCount; i++)
            {
                Transform result = FindRecursive(root.GetChild(i), name);
                if (result != null) return result;
            }
            return null;
        }

        private static void DestroyChildren(Transform root)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                GameObject child = root.GetChild(i).gameObject;
                child.SetActive(false);
                Destroy(child);
            }
        }

        private static void Stretch(RectTransform rect, float inset = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
        }
    }
}
