using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private void ApplyScreenPolish(Transform root)
        {
            if (root == null) return;

            PolishMainMenu(root);
            PolishMap(root);
            PolishSettings(root);
            PolishLevelSelector(root);
            AddLighthouseAtmosphere(root);
        }

        private void PolishTransientLayers()
        {
            if (canvas == null) return;

            Transform modal = canvas.transform.Find("ModalOverlay");
            Transform toast = canvas.transform.Find("Toast");
            if (modal != null && toast != null) Destroy(toast.gameObject);
            if (modal == null) return;

            modal.SetAsLastSibling();
            RectTransform panel = modal.Find("ModalPanel") as RectTransform;
            if (panel == null) return;

            SetAnchoredRect(panel, 0.075f, 0.285f, 0.925f, 0.715f);
            VerticalLayoutGroup layout = panel.GetComponent<VerticalLayoutGroup>();
            if (layout != null)
            {
                layout.padding = new RectOffset(25, 25, 24, 24);
                layout.spacing = 11f;
            }

            Text[] texts = panel.GetComponentsInChildren<Text>(true);
            if (texts.Length > 0) SetPreferredHeight(texts[0].rectTransform, 58f);
            if (texts.Length > 1) SetPreferredHeight(texts[1].rectTransform, 150f);

            Button[] buttons = panel.GetComponentsInChildren<Button>(true);
            if (buttons.Length > 0) SetPreferredHeight(buttons[0].transform as RectTransform, 68f);
            if (buttons.Length > 1) SetPreferredHeight(buttons[1].transform as RectTransform, 54f);
        }

        private void PolishMainMenu(Transform root)
        {
            RectTransform vertical = FindDirectRect(root, "VerticalScreen");
            if (vertical == null || vertical.Find("MainHero") == null) return;

            DisableVerticalLayout(vertical);
            HideDirectSpacers(vertical);

            RectTransform logo = vertical.Find("LogoBlock") as RectTransform;
            RectTransform hero = vertical.Find("MainHero") as RectTransform;
            RectTransform secondary = vertical.Find("MainSecondaryRow") as RectTransform;
            List<RectTransform> directButtons = GetDirectButtons(vertical);
            List<RectTransform> directTexts = GetDirectTexts(vertical);

            SetAnchoredRect(logo, 0.05f, 0.845f, 0.95f, 0.965f);
            if (directTexts.Count > 0) SetAnchoredRect(directTexts[0], 0.08f, 0.795f, 0.92f, 0.845f);
            SetAnchoredRect(hero, 0.055f, 0.355f, 0.945f, 0.785f);
            if (directButtons.Count > 0) SetAnchoredRect(directButtons[0], 0.06f, 0.255f, 0.94f, 0.335f);
            SetAnchoredRect(secondary, 0.06f, 0.145f, 0.94f, 0.235f);
            if (directTexts.Count > 1) SetAnchoredRect(directTexts[1], 0.18f, 0.083f, 0.82f, 0.125f);
            if (directTexts.Count > 2) SetAnchoredRect(directTexts[2], 0.18f, 0.035f, 0.82f, 0.072f);

            if (hero != null)
            {
                Image heroImage = hero.GetComponent<Image>();
                if (heroImage != null) heroImage.color = new Color(0.82f, 0.92f, 1f, 0.96f);
            }
        }

        private void PolishMap(Transform root)
        {
            RectTransform vertical = FindDirectRect(root, "VerticalScreen");
            if (vertical == null || vertical.Find("MapHeader") == null) return;

            DisableVerticalLayout(vertical);
            HideDirectSpacers(vertical);

            SetAnchoredRect(vertical.Find("MapHeader") as RectTransform, 0.035f, 0.900f, 0.965f, 0.985f);
            SetAnchoredRect(vertical.Find("LighthouseMetaCard") as RectTransform, 0.04f, 0.505f, 0.96f, 0.885f);
            SetAnchoredRect(vertical.Find("TaskCard") as RectTransform, 0.04f, 0.318f, 0.96f, 0.485f);
            SetAnchoredRect(vertical.Find("MetaProgressRow") as RectTransform, 0.05f, 0.270f, 0.95f, 0.305f);

            List<RectTransform> directButtons = GetDirectButtons(vertical);
            if (directButtons.Count >= 2)
            {
                RectTransform levels = directButtons[directButtons.Count - 1];
                RectTransform play = directButtons[directButtons.Count - 2];
                SetAnchoredRect(levels, 0.05f, 0.022f, 0.95f, 0.072f);
                SetAnchoredRect(play, 0.05f, 0.090f, 0.95f, 0.155f);
                if (directButtons.Count >= 3)
                    SetAnchoredRect(directButtons[0], 0.05f, 0.178f, 0.95f, 0.245f);
            }

            RectTransform progressRow = vertical.Find("MetaProgressRow") as RectTransform;
            if (progressRow != null)
            {
                HorizontalLayoutGroup layout = progressRow.GetComponent<HorizontalLayoutGroup>();
                if (layout != null)
                {
                    layout.padding = new RectOffset(0, 0, 0, 0);
                    layout.spacing = 10f;
                }
            }
        }

        private void PolishSettings(Transform root)
        {
            RectTransform vertical = FindDirectRect(root, "VerticalScreen");
            if (vertical == null || vertical.Find("SettingsHeader") == null) return;

            DisableVerticalLayout(vertical);
            HideDirectSpacers(vertical);

            SetAnchoredRect(vertical.Find("SettingsHeader") as RectTransform, 0.04f, 0.855f, 0.96f, 0.965f);
            RectTransform card = vertical.Find("SettingsCard") as RectTransform;
            SetAnchoredRect(card, 0.045f, 0.205f, 0.955f, 0.805f);

            List<RectTransform> directTexts = GetDirectTexts(vertical);
            if (directTexts.Count > 0) SetAnchoredRect(directTexts[directTexts.Count - 1], 0.18f, 0.075f, 0.82f, 0.145f);

            if (card != null)
            {
                VerticalLayoutGroup layout = card.GetComponent<VerticalLayoutGroup>();
                if (layout != null)
                {
                    layout.padding = new RectOffset(20, 20, 22, 22);
                    layout.spacing = 12f;
                }

                Button[] buttons = card.GetComponentsInChildren<Button>(true);
                for (int i = 0; i < buttons.Length; i++)
                {
                    float height = i == buttons.Length - 1 ? 62f : 74f;
                    SetPreferredHeight(buttons[i].transform as RectTransform, height);
                }
            }
        }

        private void PolishLevelSelector(Transform root)
        {
            RectTransform scrollRoot = root.Find("LevelScroll") as RectTransform;
            if (scrollRoot == null) return;

            RectTransform viewport = scrollRoot.Find("Viewport") as RectTransform;
            RectTransform content = viewport != null ? viewport.Find("Content") as RectTransform : null;
            ScrollRect scroll = scrollRoot.GetComponent<ScrollRect>();
            GridLayoutGroup grid = content != null ? content.GetComponent<GridLayoutGroup>() : null;
            if (viewport == null || content == null || scroll == null || grid == null) return;

            SetAnchoredRect(scrollRoot, 0.025f, 0.025f, 0.975f, 0.895f);
            Stretch(viewport, 10f);

            float viewportWidth = viewport.rect.width;
            if (viewportWidth < 100f) viewportWidth = 650f;
            const int columns = 3;
            float horizontalPadding = 22f;
            float spacing = 12f;
            float cellWidth = Mathf.Floor((viewportWidth - horizontalPadding * 2f - spacing * (columns - 1)) / columns);
            float cellHeight = Mathf.Clamp(cellWidth * 0.78f, 128f, 156f);
            int rows = Mathf.CeilToInt(content.childCount / (float)columns);
            float contentHeight = horizontalPadding + rows * cellHeight + Mathf.Max(0, rows - 1) * spacing + 24f;

            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(0f, 1f);
            content.pivot = new Vector2(0f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = new Vector2(viewportWidth, contentHeight);
            content.localScale = Vector3.one;
            content.gameObject.SetActive(true);

            grid.padding = new RectOffset(Mathf.RoundToInt(horizontalPadding), Mathf.RoundToInt(horizontalPadding), 18, 20);
            grid.spacing = new Vector2(spacing, spacing);
            grid.cellSize = new Vector2(cellWidth, cellHeight);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.childAlignment = TextAnchor.UpperCenter;

            CanvasGroup group = content.GetComponent<CanvasGroup>();
            if (group == null) group = content.gameObject.AddComponent<CanvasGroup>();
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;

            for (int i = 0; i < content.childCount; i++)
            {
                Transform child = content.GetChild(i);
                child.gameObject.SetActive(true);
                child.localScale = Vector3.one;
                CanvasGroup childGroup = child.GetComponent<CanvasGroup>();
                if (childGroup != null) childGroup.alpha = 1f;

                Image image = child.GetComponent<Image>();
                if (image != null)
                    image.color = Color.Lerp(image.color, Color.white, 0.12f);
            }

            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            scroll.verticalNormalizedPosition = 1f;
        }

        private void AddLighthouseAtmosphere(Transform root)
        {
            Image[] images = root.GetComponentsInChildren<Image>(true);
            foreach (Image image in images)
            {
                if (image.name != "PremiumLighthouseVisual" || image.transform.Find("LivingSceneLayer") != null) continue;

                RectTransform layer = CreateRect(image.transform, "LivingSceneLayer");
                Stretch(layer);
                LayoutElement ignore = layer.gameObject.AddComponent<LayoutElement>();
                ignore.ignoreLayout = true;
                layer.SetAsLastSibling();

                AddDriftingCloud(layer, new Vector2(0.12f, 0.72f), new Vector2(0.37f, 0.85f), 0.12f, 0.55f);
                AddDriftingCloud(layer, new Vector2(0.60f, 0.66f), new Vector2(0.84f, 0.76f), 0.16f, -0.38f);

                Image lensGlow = CreateImage(layer, "LensGlow", ProceduralArt.Pearl("living_lens_glow"),
                    new Color(1f, 0.78f, 0.25f, save != null && save.LighthouseVisualState >= 23 ? 0.68f : 0.20f));
                lensGlow.rectTransform.anchorMin = new Vector2(0.455f, 0.735f);
                lensGlow.rectTransform.anchorMax = new Vector2(0.545f, 0.825f);
                lensGlow.rectTransform.offsetMin = Vector2.zero;
                lensGlow.rectTransform.offsetMax = Vector2.zero;
                lensGlow.raycastTarget = false;
                lensGlow.gameObject.AddComponent<SoftGlowPulse>();

                AddBird(layer, 0.22f, 0.72f, 0.026f, 12f);
                AddBird(layer, 0.74f, 0.63f, 0.021f, -8f);
                AddBird(layer, 0.82f, 0.78f, 0.016f, 5f);
            }
        }

        private void AddDriftingCloud(RectTransform parent, Vector2 min, Vector2 max, float alpha, float speed)
        {
            Image cloud = CreateImage(parent, "MovingCloud", ProceduralArt.Pearl("living_cloud"),
                new Color(0.94f, 0.94f, 0.96f, alpha));
            cloud.rectTransform.anchorMin = min;
            cloud.rectTransform.anchorMax = max;
            cloud.rectTransform.offsetMin = Vector2.zero;
            cloud.rectTransform.offsetMax = Vector2.zero;
            cloud.raycastTarget = false;
            LighthouseSceneDrift drift = cloud.gameObject.AddComponent<LighthouseSceneDrift>();
            drift.Configure(speed, 2.2f);
        }

        private void AddBird(RectTransform parent, float x, float y, float size, float angle)
        {
            RectTransform bird = CreateRect(parent, "SeaBird");
            bird.anchorMin = new Vector2(x, y);
            bird.anchorMax = new Vector2(x, y);
            bird.pivot = new Vector2(0.5f, 0.5f);
            bird.sizeDelta = new Vector2(1000f * size, 360f * size);
            bird.localRotation = Quaternion.Euler(0f, 0f, angle);

            for (int i = 0; i < 2; i++)
            {
                Image wing = CreateImage(bird, "Wing", ProceduralArt.Rounded("bird_wing", Color.white, 5),
                    new Color(0.92f, 0.96f, 1f, 0.68f));
                wing.rectTransform.anchorMin = i == 0 ? new Vector2(0.02f, 0.34f) : new Vector2(0.50f, 0.34f);
                wing.rectTransform.anchorMax = i == 0 ? new Vector2(0.52f, 0.62f) : new Vector2(0.98f, 0.62f);
                wing.rectTransform.offsetMin = Vector2.zero;
                wing.rectTransform.offsetMax = Vector2.zero;
                wing.rectTransform.localRotation = Quaternion.Euler(0f, 0f, i == 0 ? 16f : -16f);
                wing.raycastTarget = false;
            }

            LighthouseSceneDrift drift = bird.gameObject.AddComponent<LighthouseSceneDrift>();
            drift.Configure(0.22f + x * 0.12f, 1.4f);
        }

        private static RectTransform FindDirectRect(Transform root, string name)
        {
            if (root == null) return null;
            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                if (child.name == name) return child as RectTransform;
            }
            return null;
        }

        private static List<RectTransform> GetDirectButtons(Transform parent)
        {
            var result = new List<RectTransform>();
            if (parent == null) return result;
            for (int i = 0; i < parent.childCount; i++)
            {
                RectTransform child = parent.GetChild(i) as RectTransform;
                if (child != null && child.GetComponent<Button>() != null) result.Add(child);
            }
            return result;
        }

        private static List<RectTransform> GetDirectTexts(Transform parent)
        {
            var result = new List<RectTransform>();
            if (parent == null) return result;
            for (int i = 0; i < parent.childCount; i++)
            {
                RectTransform child = parent.GetChild(i) as RectTransform;
                if (child != null && child.GetComponent<Text>() != null) result.Add(child);
            }
            return result;
        }

        private static void DisableVerticalLayout(RectTransform root)
        {
            VerticalLayoutGroup layout = root != null ? root.GetComponent<VerticalLayoutGroup>() : null;
            if (layout != null) layout.enabled = false;
        }

        private static void HideDirectSpacers(Transform root)
        {
            if (root == null) return;
            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                if (child.name == "Spacer") child.gameObject.SetActive(false);
            }
        }

        private static void SetAnchoredRect(RectTransform rect, float minX, float minY, float maxX, float maxY)
        {
            if (rect == null) return;
            rect.anchorMin = new Vector2(minX, minY);
            rect.anchorMax = new Vector2(maxX, maxY);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
        }

        private static void SetPreferredHeight(RectTransform rect, float height)
        {
            if (rect == null) return;
            LayoutElement element = rect.GetComponent<LayoutElement>();
            if (element == null) element = rect.gameObject.AddComponent<LayoutElement>();
            element.preferredHeight = height;
            element.minHeight = 0f;
        }
    }

    public sealed class LighthouseSceneDrift : MonoBehaviour
    {
        private RectTransform rect;
        private Vector2 origin;
        private float speed;
        private float distance;
        private float phase;

        public void Configure(float movementSpeed, float movementDistance)
        {
            speed = movementSpeed;
            distance = movementDistance;
        }

        private void Awake()
        {
            rect = transform as RectTransform;
            origin = rect != null ? rect.anchoredPosition : Vector2.zero;
            phase = UnityEngine.Random.value * Mathf.PI * 2f;
        }

        private void Update()
        {
            if (rect == null) return;
            float t = Time.unscaledTime;
            rect.anchoredPosition = origin + new Vector2(
                Mathf.Sin(t * speed + phase) * distance,
                Mathf.Sin(t * speed * 0.63f + phase) * distance * 0.34f);
        }
    }
}
