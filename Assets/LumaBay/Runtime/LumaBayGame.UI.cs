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
            art.anchorMin = new Vector2(0f, 0.27f);
            art.anchorMax = Vector2.one;
            art.offsetMin = new Vector2(15f, 8f);
            art.offsetMax = new Vector2(-15f, -15f);

            Image sky = CreateImage(art, "Sky", ProceduralArt.Rounded("lighthouse_sky", new Color(0.06f, 0.28f, 0.46f, 1f), 18), Color.white);
            Stretch(sky.rectTransform);

            Image sunGlow = CreateImage(art, "SunGlow", ProceduralArt.Pearl("sun_glow"), new Color(1f, 0.64f, 0.22f, 0.34f));
            sunGlow.rectTransform.anchorMin = new Vector2(0.63f, 0.52f);
            sunGlow.rectTransform.anchorMax = new Vector2(0.98f, 0.98f);
            sunGlow.rectTransform.offsetMin = Vector2.zero;
            sunGlow.rectTransform.offsetMax = Vector2.zero;
            sunGlow.gameObject.AddComponent<SoftGlowPulse>();

            for (int i = 0; i < 4; i++)
            {
                float t = i / 3f;
                Image wave = CreateImage(art, $"Wave{i}", ProceduralArt.Rounded($"premium_wave_{i}",
                    Color.Lerp(new Color(0.08f, 0.34f, 0.54f, 0.92f), new Color(0.12f, 0.66f, 0.76f, 0.66f), t), 9), Color.white);
                RectTransform r = wave.rectTransform;
                r.anchorMin = new Vector2(-0.05f + i * 0.018f, 0.025f + i * 0.040f);
                r.anchorMax = new Vector2(1.05f, 0.10f + i * 0.040f);
                r.offsetMin = Vector2.zero;
                r.offsetMax = Vector2.zero;
            }

            Image island = CreateImage(art, "Island", ProceduralArt.Rounded("premium_island", new Color(0.10f, 0.16f, 0.14f, 1f), 20), Color.white);
            island.rectTransform.anchorMin = new Vector2(0.14f, 0.10f);
            island.rectTransform.anchorMax = new Vector2(0.87f, 0.27f);
            island.rectTransform.offsetMin = Vector2.zero;
            island.rectTransform.offsetMax = Vector2.zero;

            Color towerColor = progress >= 50 ? new Color(0.94f, 0.90f, 0.78f) : new Color(0.54f, 0.56f, 0.53f);
            Image towerShadow = CreateImage(art, "TowerShadow", ProceduralArt.Rounded("tower_shadow", NauticalTheme.Shadow, 12), Color.white);
            towerShadow.rectTransform.anchorMin = new Vector2(0.405f, 0.205f);
            towerShadow.rectTransform.anchorMax = new Vector2(0.625f, 0.775f);
            towerShadow.rectTransform.offsetMin = new Vector2(5f, -5f);
            towerShadow.rectTransform.offsetMax = new Vector2(5f, -5f);

            Image tower = CreateImage(art, "Tower", ProceduralArt.OrnateFrame("tower", towerColor, true), Color.white);
            tower.rectTransform.anchorMin = new Vector2(0.39f, 0.21f);
            tower.rectTransform.anchorMax = new Vector2(0.61f, 0.78f);
            tower.rectTransform.offsetMin = Vector2.zero;
            tower.rectTransform.offsetMax = Vector2.zero;

            for (int i = 0; i < 3; i++)
            {
                Image stripe = CreateImage(tower.rectTransform, $"Stripe{i}", ProceduralArt.Rounded($"premium_stripe_{i}",
                    progress > i * 20 ? NauticalTheme.Coral : new Color(0.28f, 0.29f, 0.28f), 5), Color.white);
                float bottom = 0.13f + i * 0.26f;
                stripe.rectTransform.anchorMin = new Vector2(0.035f, bottom);
                stripe.rectTransform.anchorMax = new Vector2(0.965f, bottom + 0.12f);
                stripe.rectTransform.offsetMin = Vector2.zero;
                stripe.rectTransform.offsetMax = Vector2.zero;
            }

            Image lampRoom = CreateImage(art, "LampRoom", ProceduralArt.OrnateFrame("lamp_room",
                progress >= 80 ? new Color(1f, 0.70f, 0.18f, 1f) : new Color(0.30f, 0.34f, 0.36f, 1f), true), Color.white);
            lampRoom.rectTransform.anchorMin = new Vector2(0.34f, 0.74f);
            lampRoom.rectTransform.anchorMax = new Vector2(0.66f, 0.87f);
            lampRoom.rectTransform.offsetMin = Vector2.zero;
            lampRoom.rectTransform.offsetMax = Vector2.zero;
            if (progress >= 80) lampRoom.gameObject.AddComponent<SoftGlowPulse>();

            Image roof = CreateImage(art, "Roof", ProceduralArt.Rounded("premium_roof", new Color(0.30f, 0.075f, 0.055f), 10), Color.white);
            roof.rectTransform.anchorMin = new Vector2(0.375f, 0.855f);
            roof.rectTransform.anchorMax = new Vector2(0.625f, 0.925f);
            roof.rectTransform.offsetMin = Vector2.zero;
            roof.rectTransform.offsetMax = Vector2.zero;

            if (progress >= 100)
            {
                Image leftBeam = CreateImage(art, "LeftBeam", null, new Color(1f, 0.82f, 0.28f, 0.38f));
                leftBeam.rectTransform.anchorMin = new Vector2(0.01f, 0.79f);
                leftBeam.rectTransform.anchorMax = new Vector2(0.43f, 0.835f);
                leftBeam.rectTransform.offsetMin = Vector2.zero;
                leftBeam.rectTransform.offsetMax = Vector2.zero;
                leftBeam.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 7f);
                leftBeam.gameObject.AddComponent<SoftGlowPulse>();

                Image rightBeam = CreateImage(art, "RightBeam", null, new Color(1f, 0.82f, 0.28f, 0.38f));
                rightBeam.rectTransform.anchorMin = new Vector2(0.57f, 0.79f);
                rightBeam.rectTransform.anchorMax = new Vector2(0.99f, 0.835f);
                rightBeam.rectTransform.offsetMin = Vector2.zero;
                rightBeam.rectTransform.offsetMax = Vector2.zero;
                rightBeam.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -7f);
                rightBeam.gameObject.AddComponent<SoftGlowPulse>();
            }
        }

        private void CreateProgressBar(Transform parent, float value)
        {
            RectTransform track = CreatePanel(parent, "ProgressTrack", NauticalTheme.Midnight);
            SetLayout(track, 30f);
            Image fill = CreateImage(track, "Fill", ProceduralArt.OrnateFrame("progress_fill", NauticalTheme.Gold, true), Color.white);
            fill.rectTransform.anchorMin = new Vector2(0f, 0f);
            fill.rectTransform.anchorMax = new Vector2(Mathf.Clamp01(value), 1f);
            fill.rectTransform.offsetMin = new Vector2(4f, 4f);
            fill.rectTransform.offsetMax = new Vector2(-4f, -4f);
            if (value > 0f) fill.gameObject.AddComponent<SoftGlowPulse>();
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
            Image shadow = CreateImage(rect, "PanelShadow", ProceduralArt.Rounded($"{name}_shadow", NauticalTheme.Shadow, 18), Color.white);
            Stretch(shadow.rectTransform);
            shadow.rectTransform.offsetMin += new Vector2(4f, -7f);
            shadow.rectTransform.offsetMax += new Vector2(4f, -7f);
            shadow.raycastTarget = false;

            Image image = rect.gameObject.AddComponent<Image>();
            image.sprite = ProceduralArt.OrnateFrame(name, color, true);
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            AddEntrance(rect, 0f, new Vector2(0f, -16f), 0.97f);
            return rect;
        }

        private Button CreateButton(Transform parent, string label, Action action, Color background, Color foreground, int size)
        {
            RectTransform rect = CreateRect(parent, "Button");
            Image image = rect.gameObject.AddComponent<Image>();
            image.sprite = ProceduralArt.OrnateFrame($"button_{label}", background, true);
            image.type = Image.Type.Sliced;

            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.10f, 1.08f, 1.02f);
            colors.pressedColor = new Color(0.79f, 0.83f, 0.87f);
            colors.disabledColor = new Color(0.38f, 0.40f, 0.43f, 0.72f);
            colors.fadeDuration = 0.07f;
            button.colors = colors;
            button.onClick.AddListener(() =>
            {
                audioSynth?.PlayClick();
                action?.Invoke();
            });
            rect.gameObject.AddComponent<UiPressFeedback>();

            Text text = CreateText(rect, label, size, TextAnchor.MiddleCenter, foreground, FontStyle.Bold);
            Stretch(text.rectTransform, 12f);
            text.raycastTarget = false;
            return button;
        }

        private Button CreateBoosterCard(Transform parent, string icon, string title, int cost, Action action, Color accent)
        {
            RectTransform card = CreateRect(parent, $"Booster_{title}");
            Image image = card.gameObject.AddComponent<Image>();
            image.sprite = ProceduralArt.OrnateFrame($"booster_{title}", Color.Lerp(NauticalTheme.Navy, accent, 0.22f), true);
            image.type = Image.Type.Sliced;
            Button button = card.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() =>
            {
                audioSynth?.PlayClick();
                action?.Invoke();
            });
            card.gameObject.AddComponent<UiPressFeedback>();

            VerticalLayoutGroup layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(8, 8, 10, 10);
            layout.spacing = 1f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            Text iconText = CreateText(card, icon, 34, TextAnchor.MiddleCenter, NauticalTheme.Pearl, FontStyle.Bold);
            SetLayout(iconText.rectTransform, 42f);
            Text titleText = CreateText(card, title, 16, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            SetLayout(titleText.rectTransform, 42f);
            Text costText = CreateText(card, $"◆ {cost}", 18, TextAnchor.MiddleCenter, NauticalTheme.GoldLight, FontStyle.Bold);
            SetLayout(costText.rectTransform, 28f);
            AddEntrance(card, 0.03f, new Vector2(0f, 24f), 0.92f);
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
            shadow.effectColor = new Color(0f, 0f, 0f, 0.72f);
            shadow.effectDistance = new Vector2(1.8f, -2.4f);
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

        private void AddEntrance(RectTransform rect, float delay, Vector2 offset, float scale)
        {
            UiEntranceMotion motion = rect.gameObject.AddComponent<UiEntranceMotion>();
            motion.Configure(delay, offset, scale);
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
                if (child.name == "Toast" || child.name == "ModalOverlay") Destroy(child.gameObject);
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
