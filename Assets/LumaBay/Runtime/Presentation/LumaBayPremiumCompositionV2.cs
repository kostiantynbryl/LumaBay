using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LumaBay
{
    /// <summary>
    /// Final presentation pass for the V2 art pack. It does not own gameplay logic;
    /// it normalizes the runtime-created hierarchy after every screen transition.
    /// </summary>
    public sealed class LumaBayPremiumCompositionV2 : MonoBehaviour
    {
        private Canvas canvas;
        private Transform screenRoot;
        private int signature = int.MinValue;
        private float nextProbe;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindFirstObjectByType<LumaBayPremiumCompositionV2>() != null) return;
            GameObject host = new GameObject("LumaBayPremiumCompositionV2");
            DontDestroyOnLoad(host);
            host.AddComponent<LumaBayPremiumCompositionV2>();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            ResolveHierarchy();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            canvas = null;
            screenRoot = null;
            signature = int.MinValue;
            nextProbe = 0f;
        }

        private void LateUpdate()
        {
            if (!LumaBayArtPackV2.IsAvailable) return;
            if (Time.unscaledTime < nextProbe) return;
            nextProbe = Time.unscaledTime + 0.12f;

            if (screenRoot == null && !ResolveHierarchy()) return;
            int current = BuildSignature(screenRoot);
            if (current == signature) return;
            signature = current;

            Canvas.ForceUpdateCanvases();
            ApplyGlobalTypography();

            if (Find("MainHero") != null) ComposeMainMenu();
            if (Find("LighthouseMetaCard") != null) ComposeMap();
            if (Find("BoardGrid") != null || Find("BoardFrame") != null) ComposeGameplay();
            if (Find("LevelScroll") != null) ComposeLevelSelector();
            if (Find("SettingsHeader") != null) ComposeSettings();

            ComposeModal();
            Canvas.ForceUpdateCanvases();
        }

        private bool ResolveHierarchy()
        {
            canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null) return false;
            Transform safe = canvas.transform.Find("SafeArea");
            screenRoot = safe != null ? safe.Find("ScreenRoot") : null;
            return screenRoot != null;
        }

        private static int BuildSignature(Transform root)
        {
            unchecked
            {
                int result = root.childCount * 486187739;
                for (int i = 0; i < root.childCount; i++)
                {
                    Transform child = root.GetChild(i);
                    result = result * 31 + child.GetInstanceID();
                    result = result * 31 + child.childCount;
                }
                return result;
            }
        }

        private void ComposeMainMenu()
        {
            RectTransform vertical = Find("VerticalScreen") as RectTransform;
            if (vertical != null)
            {
                VerticalLayoutGroup layout = vertical.GetComponent<VerticalLayoutGroup>();
                if (layout != null)
                {
                    layout.padding = new RectOffset(34, 34, 24, 24);
                    layout.spacing = 10f;
                    layout.childAlignment = TextAnchor.UpperCenter;
                }
            }

            RectTransform logo = Find("LogoBlock") as RectTransform;
            SetPreferred(logo, 104f);

            RectTransform hero = Find("MainHero") as RectTransform;
            SetPreferred(hero, 600f);
            if (hero != null)
            {
                Image image = hero.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = LumaBayArtPackV2.PanelLarge;
                    image.type = Image.Type.Sliced;
                    image.color = Color.white;
                }
            }

            RectTransform row = Find("MainSecondaryRow") as RectTransform;
            SetPreferred(row, 78f);
            if (row != null)
            {
                HorizontalLayoutGroup layout = row.GetComponent<HorizontalLayoutGroup>();
                if (layout != null)
                {
                    layout.spacing = 14f;
                    layout.padding = new RectOffset(0, 0, 2, 2);
                }
            }

            Button[] buttons = screenRoot.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                RectTransform rect = button.transform as RectTransform;
                Text label = button.GetComponentInChildren<Text>(true);
                if (label == null) continue;
                bool primary = ContainsAny(label.text, "ИГРАТЬ", "PLAY");
                SetPreferred(rect, primary ? 94f : 76f);
            }

            MoveWalletIntoHero();
        }

        private void MoveWalletIntoHero()
        {
            Transform hero = Find("MainHero");
            if (hero == null) return;
            Text[] texts = screenRoot.GetComponentsInChildren<Text>(true);
            foreach (Text text in texts)
            {
                if (text == null || string.IsNullOrEmpty(text.text)) continue;
                if (!text.text.Contains("★") || !text.text.Contains("◆")) continue;
                if (text.transform.IsChildOf(hero)) return;

                RectTransform rect = text.rectTransform;
                rect.SetParent(hero, false);
                LayoutElement element = rect.GetComponent<LayoutElement>();
                if (element != null) element.ignoreLayout = true;
                rect.anchorMin = new Vector2(0.20f, 0.03f);
                rect.anchorMax = new Vector2(0.80f, 0.13f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                text.alignment = TextAnchor.MiddleCenter;
                text.fontSize = 21;
                text.color = new Color(1f, 0.88f, 0.58f, 1f);
                text.transform.SetAsLastSibling();
                return;
            }
        }

        private void ComposeMap()
        {
            RectTransform vertical = Find("VerticalScreen") as RectTransform;
            if (vertical != null)
            {
                VerticalLayoutGroup layout = vertical.GetComponent<VerticalLayoutGroup>();
                if (layout != null)
                {
                    layout.padding = new RectOffset(24, 24, 18, 20);
                    layout.spacing = 9f;
                }
            }

            SetPreferred(Find("MapHeader") as RectTransform, 86f);
            SetPreferred(Find("LighthouseMetaCard") as RectTransform, 590f);
            SetPreferred(Find("TaskCard") as RectTransform, 154f);
            SetPreferred(Find("MetaProgressRow") as RectTransform, 42f);

            RectTransform card = Find("LighthouseMetaCard") as RectTransform;
            if (card != null)
            {
                Image image = card.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = LumaBayArtPackV2.PanelLarge;
                    image.type = Image.Type.Sliced;
                    image.color = Color.white;
                }
            }

            Button[] buttons = screenRoot.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                Text label = button.GetComponentInChildren<Text>(true);
                if (label == null) continue;
                RectTransform rect = button.transform as RectTransform;
                if (ContainsAny(label.text, "УРОВЕНЬ", "LEVEL", "START", "НАЧАТЬ")) SetPreferred(rect, 88f);
                else if (ContainsAny(label.text, "★", "ВОССТ", "RESTORE")) SetPreferred(rect, 78f);
                else if (ContainsAny(label.text, "УРОВНИ", "LEVELS")) SetPreferred(rect, 64f);
            }
        }

        private void ComposeGameplay()
        {
            RectTransform header = Find("GameplayHeader") as RectTransform;
            if (header == null) header = Find("TopHud") as RectTransform;
            SetPreferred(header, 92f);

            RectTransform goals = Find("GoalsPanel") as RectTransform;
            SetPreferred(goals, 128f);

            RectTransform boardFrame = Find("BoardFrame") as RectTransform;
            if (boardFrame != null)
            {
                Image image = boardFrame.GetComponent<Image>();
                if (image != null)
                {
                    Sprite frame = LumaBayArtPackV2.PanelLarge;
                    if (frame != null) image.sprite = frame;
                    image.type = Image.Type.Sliced;
                    image.color = new Color(1f, 1f, 1f, 0.96f);
                }
            }

            RectTransform boosters = Find("BoosterRow") as RectTransform;
            if (boosters == null) boosters = Find("BoostersRow") as RectTransform;
            if (boosters != null)
            {
                SetPreferred(boosters, 124f);
                HorizontalLayoutGroup layout = boosters.GetComponent<HorizontalLayoutGroup>();
                if (layout != null)
                {
                    layout.spacing = 7f;
                    layout.padding = new RectOffset(4, 4, 4, 4);
                    layout.childControlWidth = true;
                    layout.childForceExpandWidth = true;
                }

                Button[] cards = boosters.GetComponentsInChildren<Button>(true);
                foreach (Button card in cards)
                {
                    RectTransform rect = card.transform as RectTransform;
                    LayoutElement element = EnsureLayout(rect);
                    if (element != null)
                    {
                        element.minWidth = 0f;
                        element.preferredWidth = 116f;
                        element.flexibleWidth = 1f;
                        element.preferredHeight = 116f;
                    }

                    Image image = card.GetComponent<Image>();
                    if (image != null && LumaBayArtPackV2.BoosterTray != null)
                    {
                        image.sprite = LumaBayArtPackV2.BoosterTray;
                        image.type = Image.Type.Sliced;
                        image.color = Color.white;
                    }
                }
            }
        }

        private void ComposeLevelSelector()
        {
            RectTransform header = Find("LevelsHeader") as RectTransform;
            if (header != null)
            {
                header.anchorMin = new Vector2(0.035f, 0.91f);
                header.anchorMax = new Vector2(0.965f, 0.985f);
                header.offsetMin = Vector2.zero;
                header.offsetMax = Vector2.zero;
            }

            RectTransform root = Find("LevelScroll") as RectTransform;
            if (root == null) return;
            root.anchorMin = new Vector2(0.035f, 0.025f);
            root.anchorMax = new Vector2(0.965f, 0.895f);
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;

            ScrollRect scroll = root.GetComponent<ScrollRect>();
            if (scroll == null || scroll.content == null) return;
            RectTransform content = scroll.content;
            content.gameObject.SetActive(true);
            content.localScale = Vector3.one;

            CanvasGroup contentGroup = content.GetComponent<CanvasGroup>();
            if (contentGroup == null) contentGroup = content.gameObject.AddComponent<CanvasGroup>();
            contentGroup.alpha = 1f;
            contentGroup.interactable = true;
            contentGroup.blocksRaycasts = true;

            GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
            if (grid == null) grid = content.gameObject.AddComponent<GridLayoutGroup>();
            float width = scroll.viewport != null && scroll.viewport.rect.width > 200f ? scroll.viewport.rect.width : 650f;
            const float padding = 20f;
            const float gap = 12f;
            float cellWidth = Mathf.Floor((width - padding * 2f - gap * 2f) / 3f);
            float cellHeight = Mathf.Clamp(cellWidth * 0.92f, 150f, 176f);
            int rows = Mathf.CeilToInt(content.childCount / 3f);

            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 3;
            grid.padding = new RectOffset((int)padding, (int)padding, 22, 30);
            grid.spacing = new Vector2(gap, gap);
            grid.cellSize = new Vector2(cellWidth, cellHeight);
            grid.childAlignment = TextAnchor.UpperCenter;

            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = new Vector2(0f, rows * cellHeight + Mathf.Max(0, rows - 1) * gap + 54f);

            for (int i = 0; i < content.childCount; i++)
            {
                Transform card = content.GetChild(i);
                card.gameObject.SetActive(true);
                card.localScale = Vector3.one;
                card.localRotation = Quaternion.identity;

                CanvasGroup cardGroup = card.GetComponent<CanvasGroup>();
                if (cardGroup != null)
                {
                    cardGroup.alpha = 1f;
                    cardGroup.interactable = true;
                    cardGroup.blocksRaycasts = true;
                }

                UiEntranceMotion entrance = card.GetComponent<UiEntranceMotion>();
                if (entrance != null) entrance.enabled = false;

                Image image = card.GetComponent<Image>();
                Button button = card.GetComponent<Button>();
                Sprite node = ResolveLevelNode(button, card, i);
                if (image != null && node != null)
                {
                    image.sprite = node;
                    image.type = Image.Type.Simple;
                    image.preserveAspect = true;
                    image.color = Color.white;
                }
            }

            scroll.vertical = true;
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            scroll.verticalNormalizedPosition = 1f;
        }

        private static Sprite ResolveLevelNode(Button button, Transform card, int index)
        {
            if (button != null && !button.interactable) return LumaBayArtPackV2.MapNode(0);
            Text text = card.GetComponentInChildren<Text>(true);
            string value = text != null ? text.text : string.Empty;
            if (value.Contains("★★★")) return LumaBayArtPackV2.MapNode(5);
            if (value.Contains("★★")) return LumaBayArtPackV2.MapNode(4);
            if (value.Contains("★")) return LumaBayArtPackV2.MapNode(3);
            return LumaBayArtPackV2.MapNode(index == 0 ? 2 : 1);
        }

        private void ComposeSettings()
        {
            RectTransform header = Find("SettingsHeader") as RectTransform;
            SetPreferred(header, 88f);

            Transform[] all = screenRoot.GetComponentsInChildren<Transform>(true);
            foreach (Transform item in all)
            {
                if (!item.name.Contains("Setting", StringComparison.OrdinalIgnoreCase) &&
                    !item.name.Contains("Language", StringComparison.OrdinalIgnoreCase)) continue;
                RectTransform rect = item as RectTransform;
                if (rect != null && rect.GetComponent<Image>() != null) SetPreferred(rect, 78f);
            }
        }

        private void ComposeModal()
        {
            if (canvas == null) return;
            Transform overlay = canvas.transform.Find("ModalOverlay");
            if (overlay == null) return;
            overlay.SetAsLastSibling();

            RectTransform panel = overlay.Find("ModalPanel") as RectTransform;
            if (panel == null)
            {
                Image[] images = overlay.GetComponentsInChildren<Image>(true);
                foreach (Image candidate in images)
                {
                    if (candidate.transform == overlay) continue;
                    panel = candidate.rectTransform;
                    break;
                }
            }
            if (panel == null) return;

            Image image = panel.GetComponent<Image>();
            if (image != null && LumaBayArtPackV2.PanelLarge != null)
            {
                image.sprite = LumaBayArtPackV2.PanelLarge;
                image.type = Image.Type.Sliced;
                image.color = Color.white;
            }

            panel.anchorMin = new Vector2(0.08f, 0.22f);
            panel.anchorMax = new Vector2(0.92f, 0.78f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;
        }

        private void ApplyGlobalTypography()
        {
            Text[] labels = screenRoot.GetComponentsInChildren<Text>(true);
            foreach (Text label in labels)
            {
                if (label == null) continue;
                label.horizontalOverflow = HorizontalWrapMode.Wrap;
                label.verticalOverflow = VerticalWrapMode.Truncate;
                if (label.fontSize >= 26) label.color = Color.white;
                Shadow shadow = label.GetComponent<Shadow>();
                if (shadow != null)
                {
                    shadow.effectColor = new Color(0f, 0.02f, 0.05f, 0.72f);
                    shadow.effectDistance = new Vector2(1.4f, -1.8f);
                }
            }
        }

        private Transform Find(string name)
        {
            if (screenRoot == null) return null;
            Transform[] all = screenRoot.GetComponentsInChildren<Transform>(true);
            foreach (Transform item in all)
                if (item.name == name) return item;
            return null;
        }

        private static bool ContainsAny(string source, params string[] values)
        {
            if (string.IsNullOrEmpty(source)) return false;
            foreach (string value in values)
                if (source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return false;
        }

        private static LayoutElement EnsureLayout(RectTransform rect)
        {
            if (rect == null) return null;
            LayoutElement element = rect.GetComponent<LayoutElement>();
            if (element == null) element = rect.gameObject.AddComponent<LayoutElement>();
            return element;
        }

        private static void SetPreferred(RectTransform rect, float height)
        {
            LayoutElement element = EnsureLayout(rect);
            if (element == null) return;
            element.preferredHeight = height;
            element.minHeight = Mathf.Min(height, 40f);
        }
    }
}
