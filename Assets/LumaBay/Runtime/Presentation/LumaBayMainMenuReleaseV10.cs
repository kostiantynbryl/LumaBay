using System;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    [DisallowMultipleComponent]
    public sealed class LumaBayMainMenuReleaseV10 : MonoBehaviour
    {
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
            if (appliedRootId == rootId && FindRecursive(screenRoot, "ReleaseMenuV10Marker") != null) return;
            appliedRootId = rootId;
            Apply(screenRoot, mainHero);
        }

        private void Apply(Transform screenRoot, Transform mainHero)
        {
            GameObject marker = new GameObject("ReleaseMenuV10Marker");
            marker.transform.SetParent(screenRoot, false);

            RectTransform vertical = FindRecursive(screenRoot, "VerticalScreen") as RectTransform;
            if (vertical == null) return;

            VerticalLayoutGroup layout = vertical.GetComponent<VerticalLayoutGroup>();
            if (layout != null)
            {
                layout.padding = new RectOffset(18, 18, 8, 12);
                layout.spacing = 7f;
            }

            RectTransform logoBlock = FindRecursive(vertical, "LogoBlock") as RectTransform;
            if (logoBlock != null)
            {
                DestroyChildren(logoBlock);
                SetHeight(logoBlock, 230f);
                Image logo = logoBlock.gameObject.GetComponent<Image>();
                if (logo == null) logo = logoBlock.gameObject.AddComponent<Image>();
                logo.sprite = LumaBayEmbeddedBranding.Logo;
                logo.color = Color.white;
                logo.preserveAspect = true;
                logo.raycastTarget = false;
            }

            HideSubtitle(vertical);

            RectTransform hero = mainHero as RectTransform;
            SetHeight(hero, 570f);
            Image heroBackground = hero != null ? hero.GetComponent<Image>() : null;
            if (heroBackground != null)
            {
                Sprite panel = LumaBayArtPackV2.PanelLarge;
                if (panel != null) heroBackground.sprite = panel;
                heroBackground.type = Image.Type.Sliced;
                heroBackground.color = Color.white;
            }

            RectTransform lighthouse = FindRecursive(mainHero, "PremiumLighthouseVisual") as RectTransform;
            if (lighthouse != null)
            {
                lighthouse.anchorMin = new Vector2(0.015f, 0.075f);
                lighthouse.anchorMax = new Vector2(0.985f, 0.985f);
                lighthouse.offsetMin = Vector2.zero;
                lighthouse.offsetMax = Vector2.zero;
                Image image = lighthouse.GetComponent<Image>();
                if (image != null) image.preserveAspect = true;
            }

            RectTransform caption = FindRecursive(mainHero, "HeroCaption") as RectTransform;
            if (caption != null)
            {
                caption.anchorMin = new Vector2(0.07f, 0.025f);
                caption.anchorMax = new Vector2(0.93f, 0.145f);
                caption.offsetMin = Vector2.zero;
                caption.offsetMax = Vector2.zero;
            }

            Button play = FindFirstButtonDirect(vertical);
            if (play != null)
            {
                SetHeight(play.transform as RectTransform, 108f);
                StyleButton(play, LumaBayArtPackV2.PrimaryButton, 32);
            }

            RectTransform row = FindRecursive(vertical, "MainSecondaryRow") as RectTransform;
            if (row != null)
            {
                RebuildBottomRow(row, play);
                SetHeight(row, 150f);
            }

            CreateTopBubble(screenRoot, "SettingsBubble", new Vector2(0.085f, 0.945f), "⚙", () =>
                game.SendMessage("ShowSettings", SendMessageOptions.DontRequireReceiver));
            CreateTopBubble(screenRoot, "AccountBubble", new Vector2(0.915f, 0.945f), "●", () =>
                ShowPlaceholder("Аккаунт", "Профиль и облачная синхронизация появятся в одной из следующих версий."));

            RectTransform wallet = FindWallet(vertical);
            if (wallet != null)
            {
                SetHeight(wallet, 46f);
                Text text = wallet.GetComponent<Text>();
                if (text != null)
                {
                    text.fontSize = 22;
                    text.color = new Color(1f, 0.85f, 0.44f, 1f);
                }
            }

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(vertical);
        }

        private void RebuildBottomRow(RectTransform row, Button template)
        {
            DestroyChildren(row);
            HorizontalLayoutGroup horizontal = row.GetComponent<HorizontalLayoutGroup>();
            if (horizontal == null) horizontal = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            horizontal.padding = new RectOffset(2, 2, 2, 2);
            horizontal.spacing = 8f;
            horizontal.childAlignment = TextAnchor.MiddleCenter;
            horizontal.childControlWidth = true;
            horizontal.childControlHeight = true;
            horizontal.childForceExpandWidth = true;
            horizontal.childForceExpandHeight = true;

            CreateMenuTile(row, template, "КАРТА", "⌖", () => game.SendMessage("ShowMap", SendMessageOptions.DontRequireReceiver));
            CreateMenuTile(row, template, "УРОВНИ", "★", () => game.SendMessage("ShowLevelSelect", SendMessageOptions.DontRequireReceiver));
            CreateMenuTile(row, template, "МАГАЗИН", "◆", () => ShowPlaceholder("Магазин", "Магазин уже добавлен в навигацию. Каталог товаров будет подключён на следующем этапе."));
        }

        private void CreateMenuTile(Transform parent, Button template, string label, string icon, Action action)
        {
            GameObject go;
            if (template != null)
            {
                go = Instantiate(template.gameObject, parent, false);
                go.name = "Button_" + label;
            }
            else
            {
                go = new GameObject("Button_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
                go.transform.SetParent(parent, false);
            }

            Button button = go.GetComponent<Button>();
            Image image = go.GetComponent<Image>();
            if (image != null)
            {
                Sprite sprite = LumaBayArtPackV2.MediumPanel ?? LumaBayArtPackV2.SecondaryButton;
                if (sprite != null) image.sprite = sprite;
                image.type = Image.Type.Sliced;
                image.color = Color.white;
            }
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action?.Invoke());

            foreach (Text oldText in go.GetComponentsInChildren<Text>(true)) Destroy(oldText.gameObject);
            Text iconText = CreateText(go.transform, icon, 36, TextAnchor.UpperCenter);
            iconText.rectTransform.anchorMin = new Vector2(0.05f, 0.38f);
            iconText.rectTransform.anchorMax = new Vector2(0.95f, 0.92f);
            iconText.rectTransform.offsetMin = Vector2.zero;
            iconText.rectTransform.offsetMax = Vector2.zero;
            iconText.color = new Color(1f, 0.84f, 0.42f, 1f);

            Text labelText = CreateText(go.transform, label, 20, TextAnchor.MiddleCenter);
            labelText.rectTransform.anchorMin = new Vector2(0.03f, 0.04f);
            labelText.rectTransform.anchorMax = new Vector2(0.97f, 0.42f);
            labelText.rectTransform.offsetMin = Vector2.zero;
            labelText.rectTransform.offsetMax = Vector2.zero;
            labelText.fontStyle = FontStyle.Bold;

            LayoutElement element = go.GetComponent<LayoutElement>();
            if (element == null) element = go.AddComponent<LayoutElement>();
            element.minHeight = 142f;
            element.preferredHeight = 142f;
            element.flexibleWidth = 1f;
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
            rect.sizeDelta = new Vector2(82f, 82f);
            rect.anchoredPosition = Vector2.zero;

            Image image = go.GetComponent<Image>();
            image.sprite = LumaBayArtPackV2.Get("ui", 15) ?? LumaBayArtPackV2.CompactButton;
            image.type = Image.Type.Sliced;
            image.color = Color.white;

            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => action?.Invoke());

            Text text = CreateText(go.transform, glyph, 38, TextAnchor.MiddleCenter);
            Stretch(text.rectTransform, 8f);
            text.color = new Color(1f, 0.83f, 0.40f, 1f);
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
            overlay.GetComponent<Image>().color = new Color(0f, 0.02f, 0.05f, 0.78f);
            overlay.GetComponent<Button>().onClick.AddListener(() => Destroy(overlay));

            GameObject panelObject = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panelObject.transform.SetParent(overlay.transform, false);
            RectTransform panel = panelObject.GetComponent<RectTransform>();
            panel.anchorMin = new Vector2(0.10f, 0.37f);
            panel.anchorMax = new Vector2(0.90f, 0.63f);
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
                Text source = canvas.GetComponentInChildren<Text>(true);
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
            return text;
        }

        private static void HideSubtitle(RectTransform vertical)
        {
            for (int i = 0; i < vertical.childCount; i++)
            {
                Transform child = vertical.GetChild(i);
                Text text = child.GetComponent<Text>();
                if (text == null) continue;
                if (text.text != null && (text.text.Contains("История маяка") || text.text.Contains("Lighthouse Story")))
                    child.gameObject.SetActive(false);
            }
        }

        private static Button FindFirstButtonDirect(RectTransform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Button button = parent.GetChild(i).GetComponent<Button>();
                if (button != null) return button;
            }
            return null;
        }

        private static RectTransform FindWallet(RectTransform root)
        {
            foreach (Text text in root.GetComponentsInChildren<Text>(true))
                if (text.text != null && text.text.Contains("★") && text.text.Contains("◆")) return text.rectTransform;
            return null;
        }

        private static void StyleButton(Button button, Sprite sprite, int fontSize)
        {
            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                if (sprite != null) image.sprite = sprite;
                image.type = Image.Type.Sliced;
                image.color = Color.white;
            }
            Text text = button.GetComponentInChildren<Text>(true);
            if (text != null)
            {
                text.fontSize = fontSize;
                text.resizeTextMinSize = 18;
                text.resizeTextMaxSize = fontSize;
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
            for (int i = root.childCount - 1; i >= 0; i--) Destroy(root.GetChild(i).gameObject);
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
