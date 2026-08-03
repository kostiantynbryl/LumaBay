using System;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private int artOverrideSignature = int.MinValue;

        private void RefreshArtOverridesIfNeeded()
        {
            PolishTransientLayers();

            if (screenRoot == null)
            {
                artOverrideSignature = int.MinValue;
                premiumSkinSignature = int.MinValue;
                finalVisualSignature = int.MinValue;
                return;
            }

            int childCount = screenRoot.childCount;
            int firstId = childCount > 0 ? screenRoot.GetChild(0).GetInstanceID() : 0;
            int signature = childCount * 486187739 ^ firstId ^ (save != null ? save.LighthouseVisualState * 397 : 0);
            if (signature == artOverrideSignature)
            {
                ApplyPremiumSkinV2IfNeeded();
                ApplyFinalVisualPassIfNeeded();
                return;
            }
            artOverrideSignature = signature;

            ApplyScreenPolish(screenRoot);
            ApplyPremiumSkinV2IfNeeded();
            ApplyFinalVisualPassIfNeeded();
            if (!LumaBayArtPack.IsAvailable) return;

            ApplyBoosterArtwork(screenRoot);
            ApplyLighthouseBeam(screenRoot);
        }

        private void RestoreNextLighthouseTask()
        {
            if (save == null || save.LighthouseComplete)
            {
                ShowToast(Localization.Language == "en" ? "The lighthouse is complete" : "Маяк полностью восстановлен");
                return;
            }

            LighthouseTask task = LighthouseTaskCatalog.Get(save.LighthouseTaskIndex);
            if (save.UnlockedLevel < task.UnlockLevel)
            {
                ShowToast(Localization.Language == "en"
                    ? $"Complete level {task.UnlockLevel} first"
                    : $"Сначала пройдите уровень {task.UnlockLevel}");
                return;
            }
            if (save.AvailableStars < task.StarCost)
            {
                ShowToast(Localization.Language == "en"
                    ? $"You need {task.StarCost} stars"
                    : $"Нужно звёзд: {task.StarCost}");
                return;
            }

            save.AvailableStars -= task.StarCost;
            save.LighthouseTaskIndex++;
            save.RestorationStep = Mathf.Clamp(save.LighthouseTaskIndex / 8, 0, 6);
            SaveService.Save(save);
            audioSynth.PlayRestore();
            if (save.VibrationEnabled) Handheld.Vibrate();
            ShowMap();
        }

        private void ApplyBoosterArtwork(Transform root)
        {
            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform transform in transforms)
            {
                if (!transform.name.StartsWith("Booster_", StringComparison.Ordinal) ||
                    transform.Find("PremiumBoosterIcon") != null)
                    continue;

                Text[] labels = transform.GetComponentsInChildren<Text>(true);
                string id = ResolveBoosterId(transform.name, labels);
                if (string.IsNullOrEmpty(id)) continue;

                Sprite sprite = LumaBayArtPackV2.Booster(id) ?? LumaBayArtPack.Booster(id);
                if (sprite == null) continue;

                if (labels.Length > 0 && !string.IsNullOrWhiteSpace(labels[0].text) && labels[0].fontSize >= 25)
                    labels[0].text = string.Empty;

                Image icon = CreateImage(transform, "PremiumBoosterIcon", sprite, Color.white);
                icon.rectTransform.anchorMin = new Vector2(0.14f, 0.33f);
                icon.rectTransform.anchorMax = new Vector2(0.86f, 0.96f);
                icon.rectTransform.offsetMin = Vector2.zero;
                icon.rectTransform.offsetMax = Vector2.zero;
                icon.raycastTarget = false;
                LayoutElement layout = icon.gameObject.AddComponent<LayoutElement>();
                layout.ignoreLayout = true;
                icon.transform.SetAsLastSibling();
                icon.gameObject.AddComponent<SoftGlowPulse>();
            }
        }

        private void ApplyLighthouseBeam(Transform root)
        {
            if (save == null || save.LighthouseVisualState < 23) return;

            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform transform in transforms)
            {
                if (transform.name != "PremiumLighthouseVisual" || transform.Find("AnimatedBeam") != null) continue;

                RectTransform beam = CreateRect(transform, "AnimatedBeam");
                beam.anchorMin = new Vector2(0.50f, 0.82f);
                beam.anchorMax = new Vector2(0.50f, 0.82f);
                beam.pivot = new Vector2(0f, 0.5f);
                beam.anchoredPosition = Vector2.zero;
                beam.sizeDelta = new Vector2(365f, 25f);

                Image beamImage = beam.gameObject.AddComponent<Image>();
                beamImage.sprite = ProceduralArt.Rounded("animated_lighthouse_beam",
                    new Color(1f, 0.86f, 0.42f, 0.24f), 12);
                beamImage.type = Image.Type.Sliced;
                beamImage.raycastTarget = false;
                beam.gameObject.AddComponent<LighthouseBeamSweep>();

                Image sourceGlow = CreateImage(transform, "AnimatedBeamGlow", ProceduralArt.Pearl("beam_source"),
                    new Color(1f, 0.82f, 0.30f, 0.48f));
                sourceGlow.rectTransform.anchorMin = new Vector2(0.46f, 0.78f);
                sourceGlow.rectTransform.anchorMax = new Vector2(0.54f, 0.86f);
                sourceGlow.rectTransform.offsetMin = Vector2.zero;
                sourceGlow.rectTransform.offsetMax = Vector2.zero;
                sourceGlow.raycastTarget = false;
                sourceGlow.gameObject.AddComponent<SoftGlowPulse>();
            }
        }

        private static string ResolveBoosterId(string objectName, Text[] labels)
        {
            string aggregate = objectName;
            foreach (Text label in labels) aggregate += " " + label.text;

            if (aggregate.IndexOf("200", StringComparison.Ordinal) >= 0) return "lightning";
            if (aggregate.IndexOf("300", StringComparison.Ordinal) >= 0) return "anchor";
            if (aggregate.IndexOf(" 50", StringComparison.Ordinal) >= 0 || aggregate.EndsWith("50", StringComparison.Ordinal)) return "shuffle";
            if (aggregate.IndexOf("100", StringComparison.Ordinal) >= 0) return "extra_moves";
            if (aggregate.IndexOf("250", StringComparison.Ordinal) >= 0) return "harpoon";
            return null;
        }
    }

    public sealed class LighthouseIllustrationMotion : MonoBehaviour
    {
        private RectTransform rect;
        private Vector2 basePosition;
        private Vector3 baseScale;
        private float phase;

        private void Awake()
        {
            rect = transform as RectTransform;
            basePosition = rect != null ? rect.anchoredPosition : Vector2.zero;
            baseScale = rect != null ? rect.localScale : Vector3.one;
            phase = UnityEngine.Random.value * Mathf.PI * 2f;
        }

        private void Update()
        {
            if (rect == null) return;
            float time = Time.unscaledTime;
            rect.anchoredPosition = basePosition + new Vector2(
                Mathf.Sin(time * 0.16f + phase) * 1.6f,
                Mathf.Sin(time * 0.22f + phase) * 2.2f);
            float pulse = 1f + Mathf.Sin(time * 0.35f + phase) * 0.0025f;
            rect.localScale = baseScale * pulse;
        }
    }

    public sealed class LighthouseBeamSweep : MonoBehaviour
    {
        private RectTransform rect;
        private Graphic graphic;
        private Color baseColor;
        private float phase;

        private void Awake()
        {
            rect = transform as RectTransform;
            graphic = GetComponent<Graphic>();
            baseColor = graphic != null ? graphic.color : Color.white;
            phase = UnityEngine.Random.value * Mathf.PI * 2f;
        }

        private void Update()
        {
            if (rect == null) return;
            float wave = Mathf.Sin(Time.unscaledTime * 0.38f + phase);
            rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-10f, 12f, (wave + 1f) * 0.5f));
            if (graphic == null) return;
            Color color = baseColor;
            color.a = Mathf.Lerp(0.12f, 0.31f, (Mathf.Sin(Time.unscaledTime * 0.72f + phase) + 1f) * 0.5f);
            graphic.color = color;
        }
    }
}
