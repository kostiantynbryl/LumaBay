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
            if (screenRoot == null || !LumaBayArtPack.IsAvailable)
            {
                artOverrideSignature = int.MinValue;
                return;
            }

            int childCount = screenRoot.childCount;
            int firstId = childCount > 0 ? screenRoot.GetChild(0).GetInstanceID() : 0;
            int signature = childCount * 486187739 ^ firstId;
            if (signature == artOverrideSignature) return;
            artOverrideSignature = signature;
            ApplyBoosterArtwork(screenRoot);
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

                Sprite sprite = LumaBayArtPack.Booster(id);
                if (sprite == null) continue;

                if (labels.Length > 0 && string.IsNullOrWhiteSpace(labels[0].text) == false)
                {
                    // The first child was the old symbol placeholder. Keep actual title/cost labels.
                    if (labels[0].fontSize >= 25) labels[0].text = string.Empty;
                }

                Image icon = CreateImage(transform, "PremiumBoosterIcon", sprite, Color.white);
                icon.rectTransform.anchorMin = new Vector2(0.20f, 0.47f);
                icon.rectTransform.anchorMax = new Vector2(0.80f, 0.96f);
                icon.rectTransform.offsetMin = Vector2.zero;
                icon.rectTransform.offsetMax = Vector2.zero;
                icon.raycastTarget = false;
                LayoutElement layout = icon.gameObject.AddComponent<LayoutElement>();
                layout.ignoreLayout = true;
                icon.transform.SetAsLastSibling();
                icon.gameObject.AddComponent<SoftGlowPulse>();
            }
        }

        private static string ResolveBoosterId(string objectName, Text[] labels)
        {
            string aggregate = objectName;
            foreach (Text label in labels) aggregate += " " + label.text;

            if (aggregate.Contains("200", StringComparison.Ordinal)) return "lightning";
            if (aggregate.Contains("300", StringComparison.Ordinal)) return "anchor";
            if (aggregate.Contains(" 50", StringComparison.Ordinal) || aggregate.EndsWith("50", StringComparison.Ordinal)) return "shuffle";
            if (aggregate.Contains("100", StringComparison.Ordinal)) return "extra_moves";
            if (aggregate.Contains("250", StringComparison.Ordinal)) return "harpoon";
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
}
