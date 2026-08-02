using System;
using System.Collections.Generic;
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
            int taskIndex = save != null ? save.LighthouseTaskIndex : 0;
            int signature = childCount * 486187739 ^ firstId ^ taskIndex * 397;
            if (signature == artOverrideSignature) return;
            artOverrideSignature = signature;

            ApplyLighthouseIllustrations(screenRoot);
            ApplyLighthouseTaskControls(screenRoot);
            ApplyBoosterArtwork(screenRoot);
            ApplyCleanerBoardCells(screenRoot);
        }

        private void ApplyLighthouseIllustrations(Transform root)
        {
            foreach (Transform lighthouseArt in FindRecursive(root, "LighthouseArt"))
            {
                Transform existing = lighthouseArt.Find("PremiumLighthouseIllustration");
                if (existing != null) Destroy(existing.gameObject);

                Sprite sprite = LumaBayArtPack.LighthouseState(save != null ? save.LighthouseVisualState : 0);
                if (sprite == null) continue;

                Image illustration = CreateImage(lighthouseArt, "PremiumLighthouseIllustration", sprite, Color.white);
                Stretch(illustration.rectTransform);
                illustration.preserveAspect = false;
                illustration.raycastTarget = false;
                illustration.transform.SetAsLastSibling();
                illustration.gameObject.AddComponent<LighthouseIllustrationMotion>();

                Transform oldBadge = lighthouseArt.Find("CurrentTaskBadge");
                if (oldBadge != null) Destroy(oldBadge.gameObject);

                RectTransform badge = CreatePanel(lighthouseArt, "CurrentTaskBadge", new Color(0.018f, 0.09f, 0.16f, 0.90f));
                badge.anchorMin = new Vector2(0.055f, 0.025f);
                badge.anchorMax = new Vector2(0.945f, 0.215f);
                badge.offsetMin = Vector2.zero;
                badge.offsetMax = Vector2.zero;
                badge.transform.SetAsLastSibling();

                LighthouseTask task = save != null && !save.LighthouseComplete
                    ? LighthouseTaskCatalog.Get(save.LighthouseTaskIndex)
                    : null;
                string caption = task != null
                    ? $"<size=17><color=#93DCEB>{task.Chapter}</color></size>\n<size=24>{task.Title}</size>"
                    : (Localization.Language == "en" ? "The lighthouse watches over Luma Bay" : "Маяк снова хранит Лума-Бэй");
                Text text = CreateText(badge, caption, 24, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
                text.supportRichText = true;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 15;
                text.resizeTextMaxSize = 24;
                Stretch(text.rectTransform, 10f);
                text.raycastTarget = false;
            }
        }

        private void ApplyLighthouseTaskControls(Transform root)
        {
            if (save == null) return;
            LighthouseTask task = save.LighthouseComplete ? null : LighthouseTaskCatalog.Get(save.LighthouseTaskIndex);
            int percent = Mathf.RoundToInt(save.LighthouseProgress01 * 100f);

            foreach (Text text in root.GetComponentsInChildren<Text>(true))
            {
                if (text == null) continue;
                string value = text.text ?? string.Empty;
                if (value.StartsWith("Прогресс:", StringComparison.OrdinalIgnoreCase) ||
                    value.StartsWith("Progress:", StringComparison.OrdinalIgnoreCase))
                {
                    text.text = Localization.Language == "en" ? $"Progress: {percent}%" : $"Прогресс: {percent}%";
                }
            }

            foreach (Transform track in FindRecursive(root, "ProgressTrack"))
            {
                Transform fill = track.Find("Fill");
                if (fill is RectTransform fillRect)
                {
                    fillRect.anchorMax = new Vector2(save.LighthouseProgress01, 1f);
                }
            }

            foreach (Transform storyPanel in FindRecursive(root, "StoryPanel"))
            {
                Text story = storyPanel.GetComponentInChildren<Text>(true);
                if (story != null)
                {
                    story.text = task != null
                        ? task.Description
                        : (Localization.Language == "en"
                            ? "The restored lighthouse now guides ships and reveals a signal from the distant island."
                            : "Восстановленный маяк ведёт корабли и принимает сигнал с далёкого острова.");
                }
            }

            foreach (Transform heroCaption in FindRecursive(root, "HeroCaption"))
            {
                Text caption = heroCaption.GetComponentInChildren<Text>(true);
                if (caption != null)
                {
                    string title = Localization.Language == "en" ? "Lighthouse restoration" : "Восстановление маяка";
                    caption.text = $"{title}\n{percent}% • {save.LighthouseTaskIndex}/{LighthouseTaskCatalog.Count}";
                }
            }

            foreach (Button button in root.GetComponentsInChildren<Button>(true))
            {
                Text label = button.GetComponentInChildren<Text>(true);
                if (label == null || !LooksLikeRestoreButton(label.text)) continue;

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(RestoreNextLighthouseTask);
                button.interactable = true;

                if (task == null)
                {
                    label.text = Localization.Language == "en" ? "LIGHTHOUSE COMPLETE" : "МАЯК ПОЛНОСТЬЮ ВОССТАНОВЛЕН";
                    button.interactable = false;
                }
                else if (save.UnlockedLevel < task.UnlockLevel)
                {
                    label.text = Localization.Language == "en"
                        ? $"UNLOCKS AFTER LEVEL {task.UnlockLevel}"
                        : $"ОТКРОЕТСЯ ПОСЛЕ УРОВНЯ {task.UnlockLevel}";
                }
                else
                {
                    label.text = $"{task.StarCost} ★  •  {task.Title.ToUpperInvariant()}";
                }

                label.resizeTextForBestFit = true;
                label.resizeTextMinSize = 15;
                label.resizeTextMaxSize = 26;
            }
        }

        private static bool LooksLikeRestoreButton(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            string upper = value.ToUpperInvariant();
            return upper.Contains("ВОССТАНОВ") || upper.Contains("РЕМОНТ") ||
                   upper.Contains("RESTORE") || upper.Contains("RESTORATION REQUIRES");
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
            Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Booster_Lightning"] = "lightning",
                ["Booster_Anchor"] = "anchor",
                ["Booster_Shuffle"] = "shuffle",
                ["Booster_Extra time"] = "extra_moves",
                ["Booster_Harpoon"] = "harpoon"
            };

            foreach (Transform transform in FindRecursivePrefix(root, "Booster_"))
            {
                if (transform.Find("PremiumBoosterIcon") != null) continue;
                if (!map.TryGetValue(transform.name, out string id)) continue;
                Sprite sprite = LumaBayArtPack.Booster(id);
                if (sprite == null) continue;

                Text[] labels = transform.GetComponentsInChildren<Text>(true);
                if (labels.Length > 0) labels[0].text = string.Empty;

                Image icon = CreateImage(transform, "PremiumBoosterIcon", sprite, Color.white);
                icon.rectTransform.anchorMin = new Vector2(0.18f, 0.42f);
                icon.rectTransform.anchorMax = new Vector2(0.82f, 0.96f);
                icon.rectTransform.offsetMin = Vector2.zero;
                icon.rectTransform.offsetMax = Vector2.zero;
                icon.raycastTarget = false;
                icon.transform.SetAsFirstSibling();
                icon.gameObject.AddComponent<SoftGlowPulse>();
            }
        }

        private static void ApplyCleanerBoardCells(Transform root)
        {
            foreach (Transform cell in FindRecursivePrefix(root, "Cell_"))
            {
                Image background = cell.GetComponent<Image>();
                if (background == null) continue;
                background.color = new Color(0.13f, 0.25f, 0.38f, 0.92f);
                Shadow shadow = cell.GetComponent<Shadow>();
                if (shadow != null) shadow.enabled = false;
            }
        }

        private static IEnumerable<Transform> FindRecursive(Transform root, string exactName)
        {
            if (root == null) yield break;
            var stack = new Stack<Transform>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                Transform current = stack.Pop();
                if (current.name == exactName) yield return current;
                for (int i = current.childCount - 1; i >= 0; i--) stack.Push(current.GetChild(i));
            }
        }

        private static IEnumerable<Transform> FindRecursivePrefix(Transform root, string prefix)
        {
            if (root == null) yield break;
            var stack = new Stack<Transform>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                Transform current = stack.Pop();
                if (current.name.StartsWith(prefix, StringComparison.Ordinal)) yield return current;
                for (int i = current.childCount - 1; i >= 0; i--) stack.Push(current.GetChild(i));
            }
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
            rect.anchoredPosition = basePosition + new Vector2(Mathf.Sin(time * 0.16f + phase) * 1.6f, Mathf.Sin(time * 0.22f + phase) * 2.2f);
            float pulse = 1f + Mathf.Sin(time * 0.35f + phase) * 0.0025f;
            rect.localScale = baseScale * pulse;
        }
    }
}
