using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private void ShowMainMenu()
        {
            levelFinished = false;
            board = null;
            ClearScreen();

            RectTransform column = CreateVerticalScreen(30, 18f);
            AddFlexibleSpacer(column, 1f);

            Text title = CreateText(column, Localization.T("title"), 72, TextAnchor.MiddleCenter, ProceduralArt.Cream, FontStyle.Bold);
            SetLayout(title.rectTransform, 96f);
            Text subtitle = CreateText(column, Localization.T("subtitle"), 27, TextAnchor.MiddleCenter, new Color(0.75f, 0.93f, 0.94f), FontStyle.Normal);
            SetLayout(subtitle.rectTransform, 46f);

            RectTransform artCard = CreatePanel(column, "HeroCard", new Color(0.02f, 0.12f, 0.20f, 0.83f));
            SetLayout(artCard, 430f);
            BuildLighthouseArt(artCard, Mathf.RoundToInt(save.RestorationStep / 6f * 100f));

            AddFlexibleSpacer(column, 0.25f);
            Button play = CreateButton(column, Localization.T("play"), ShowMap, ProceduralArt.Gold, ProceduralArt.Navy, 34);
            SetLayout(play.GetComponent<RectTransform>(), 88f);
            Button levels = CreateButton(column, Localization.T("levels"), ShowLevelSelect, new Color(0.10f, 0.45f, 0.52f), Color.white, 28);
            SetLayout(levels.GetComponent<RectTransform>(), 72f);
            Button settings = CreateButton(column, Localization.T("settings"), ShowSettings, new Color(0.05f, 0.20f, 0.27f), Color.white, 26);
            SetLayout(settings.GetComponent<RectTransform>(), 68f);

            Text version = CreateText(column, $"Norvexa Games • {Version}", 20, TextAnchor.MiddleCenter, new Color(0.65f, 0.80f, 0.82f), FontStyle.Normal);
            SetLayout(version.rectTransform, 38f);
            AddFlexibleSpacer(column, 0.4f);
        }

        private void ShowMap()
        {
            levelFinished = false;
            board = null;
            ClearScreen();

            RectTransform column = CreateVerticalScreen(24, 14f);
            RectTransform header = CreateHorizontal(column, "MapHeader", 12f);
            SetLayout(header, 72f);
            Button back = CreateButton(header, "‹", ShowMainMenu, new Color(0.04f, 0.19f, 0.26f), Color.white, 40);
            SetLayout(back.GetComponent<RectTransform>(), -1f, 72f);
            Text mapTitle = CreateText(header, Localization.T("map"), 34, TextAnchor.MiddleLeft, Color.white, FontStyle.Bold);
            SetLayout(mapTitle.rectTransform, -1f, 1f);
            Text wallet = CreateText(header, $"★ {save.AvailableStars}   ◈ {save.Coins}", 25, TextAnchor.MiddleRight, ProceduralArt.Gold, FontStyle.Bold);
            SetLayout(wallet.rectTransform, -1f, 220f);

            RectTransform card = CreatePanel(column, "RestorationCard", new Color(0.02f, 0.13f, 0.20f, 0.90f));
            SetLayout(card, 520f);
            BuildLighthouseArt(card, Mathf.RoundToInt(save.RestorationStep / 6f * 100f));

            RectTransform progressBlock = CreateVertical(card, "ProgressBlock", 6f, new RectOffset(24, 24, 18, 18));
            RectTransform progressRect = progressBlock;
            progressRect.anchorMin = new Vector2(0f, 0f);
            progressRect.anchorMax = new Vector2(1f, 0.34f);
            progressRect.offsetMin = new Vector2(18f, 18f);
            progressRect.offsetMax = new Vector2(-18f, 0f);

            Text restoration = CreateText(progressBlock, Localization.T("restoration"), 28, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            SetLayout(restoration.rectTransform, 42f);
            int percent = Mathf.RoundToInt(save.RestorationStep / 6f * 100f);
            Text progress = CreateText(progressBlock, Localization.T("progress", percent), 23, TextAnchor.MiddleCenter, new Color(0.74f, 0.93f, 0.94f), FontStyle.Normal);
            SetLayout(progress.rectTransform, 34f);
            CreateProgressBar(progressBlock, save.RestorationStep / 6f);

            Text story = CreateText(column, Localization.T($"story_{Mathf.Clamp(save.RestorationStep, 0, 6)}"), 24, TextAnchor.MiddleCenter, ProceduralArt.Cream, FontStyle.Normal);
            story.horizontalOverflow = HorizontalWrapMode.Wrap;
            story.verticalOverflow = VerticalWrapMode.Truncate;
            SetLayout(story.rectTransform, 118f);

            if (save.RestorationStep < 6)
            {
                bool canRestore = save.AvailableStars >= 3;
                Button restore = CreateButton(column,
                    canRestore ? Localization.T("restore") : Localization.T("restore_need"),
                    canRestore ? (Action)RestoreLighthouse : () => ShowToast(Localization.T("restore_need")),
                    canRestore ? ProceduralArt.Gold : new Color(0.25f, 0.34f, 0.38f),
                    canRestore ? ProceduralArt.Navy : new Color(0.72f, 0.78f, 0.80f),
                    27);
                SetLayout(restore.GetComponent<RectTransform>(), 76f);
            }

            Button play = CreateButton(column, Localization.T("level", save.UnlockedLevel) + "  •  " + Localization.T("start"),
                () => StartLevel(save.UnlockedLevel), ProceduralArt.Coral, Color.white, 30);
            SetLayout(play.GetComponent<RectTransform>(), 88f);
            Button levels = CreateButton(column, Localization.T("levels"), ShowLevelSelect, new Color(0.08f, 0.40f, 0.47f), Color.white, 25);
            SetLayout(levels.GetComponent<RectTransform>(), 64f);
        }

        private void RestoreLighthouse()
        {
            if (save.RestorationStep >= 6 || save.AvailableStars < 3)
            {
                ShowToast(Localization.T("restore_need"));
                return;
            }

            save.AvailableStars -= 3;
            save.RestorationStep++;
            SaveService.Save(save);
            audioSynth.PlayWin();
            if (save.VibrationEnabled) Handheld.Vibrate();
            ShowMap();
        }

        private void ShowLevelSelect()
        {
            levelFinished = false;
            board = null;
            ClearScreen();

            RectTransform header = CreateRect(screenRoot, "LevelsHeader");
            header.anchorMin = new Vector2(0f, 0.91f);
            header.anchorMax = Vector2.one;
            header.offsetMin = new Vector2(20f, 4f);
            header.offsetMax = new Vector2(-20f, -8f);
            HorizontalLayoutGroup headerLayout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            headerLayout.spacing = 12f;
            headerLayout.childAlignment = TextAnchor.MiddleCenter;
            headerLayout.childForceExpandHeight = true;
            headerLayout.childForceExpandWidth = false;

            Button back = CreateButton(header, "‹", ShowMap, new Color(0.04f, 0.19f, 0.26f), Color.white, 40);
            SetLayout(back.GetComponent<RectTransform>(), -1f, 72f);
            Text title = CreateText(header, Localization.T("levels"), 36, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            SetLayout(title.rectTransform, -1f, 1f);
            Text summary = CreateText(header, $"★ {save.TotalStars}", 27, TextAnchor.MiddleRight, ProceduralArt.Gold, FontStyle.Bold);
            SetLayout(summary.rectTransform, -1f, 90f);

            RectTransform scrollRectTransform = CreateRect(screenRoot, "LevelScroll");
            scrollRectTransform.anchorMin = new Vector2(0f, 0f);
            scrollRectTransform.anchorMax = new Vector2(1f, 0.91f);
            scrollRectTransform.offsetMin = new Vector2(18f, 18f);
            scrollRectTransform.offsetMax = new Vector2(-18f, -6f);
            Image scrollBackground = scrollRectTransform.gameObject.AddComponent<Image>();
            scrollBackground.sprite = ProceduralArt.Rounded("level_scroll", new Color(0.02f, 0.12f, 0.18f, 0.72f));
            scrollBackground.type = Image.Type.Sliced;

            ScrollRect scroll = scrollRectTransform.gameObject.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;

            RectTransform viewport = CreateRect(scrollRectTransform, "Viewport");
            Stretch(viewport, 10f);
            Image viewportImage = viewport.gameObject.AddComponent<Image>();
            viewportImage.color = new Color(1f, 1f, 1f, 0.001f);
            Mask mask = viewport.gameObject.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            scroll.viewport = viewport;

            RectTransform content = CreateRect(viewport, "Content");
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.offsetMin = Vector2.zero;
            content.offsetMax = Vector2.zero;
            GridLayoutGroup grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.padding = new RectOffset(12, 12, 16, 16);
            grid.spacing = new Vector2(12f, 12f);
            grid.cellSize = new Vector2(145f, 126f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;
            grid.childAlignment = TextAnchor.UpperCenter;
            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.content = content;

            for (int levelId = 1; levelId <= LevelCatalog.Count; levelId++)
            {
                int captured = levelId;
                bool unlocked = levelId <= save.UnlockedLevel;
                int stars = save.StarsByLevel[levelId - 1];
                string label = unlocked
                    ? $"{levelId}\n<size=19>{new string('★', stars)}{new string('·', 3 - stars)}</size>"
                    : $"{levelId}\n<size=18>{Localization.T("locked")}</size>";
                Button button = CreateButton(content, label,
                    unlocked ? (Action)(() => ShowLevelPreview(captured)) : () => ShowToast(Localization.T("locked")),
                    unlocked ? new Color(0.08f, 0.46f, 0.50f) : new Color(0.15f, 0.21f, 0.24f),
                    unlocked ? Color.white : new Color(0.55f, 0.62f, 0.64f), 27);
                button.GetComponentInChildren<Text>().supportRichText = true;
            }
        }

        private void ShowLevelPreview(int levelId)
        {
            LevelDefinition level = LevelCatalog.Get(levelId);
            string fogLine = level.FogCount > 0 ? $"\n{Localization.T("fog", 0, level.FogCount)}" : string.Empty;
            string body = Localization.T("collect", Localization.PieceName(level.TargetPiece), 0, level.TargetCount) + fogLine +
                          $"\n\n{Localization.T("moves", level.Moves)}";
            ShowModal(Localization.T("level", levelId), body, Localization.T("start"), () => StartLevel(levelId), Localization.T("back"));
        }

        private void ShowSettings()
        {
            levelFinished = false;
            board = null;
            ClearScreen();

            RectTransform column = CreateVerticalScreen(28, 16f);
            RectTransform header = CreateHorizontal(column, "SettingsHeader", 12f);
            SetLayout(header, 74f);
            Button back = CreateButton(header, "‹", ShowMainMenu, new Color(0.04f, 0.19f, 0.26f), Color.white, 40);
            SetLayout(back.GetComponent<RectTransform>(), -1f, 72f);
            Text title = CreateText(header, Localization.T("settings"), 38, TextAnchor.MiddleLeft, Color.white, FontStyle.Bold);
            SetLayout(title.rectTransform, -1f, 1f);

            AddFlexibleSpacer(column, 0.3f);
            Button sound = CreateButton(column, $"{Localization.T("sound")}: {(save.SoundEnabled ? "ON" : "OFF")}", ToggleSound,
                save.SoundEnabled ? new Color(0.08f, 0.52f, 0.49f) : new Color(0.22f, 0.28f, 0.31f), Color.white, 28);
            SetLayout(sound.GetComponent<RectTransform>(), 88f);
            Button vibration = CreateButton(column, $"{Localization.T("vibration")}: {(save.VibrationEnabled ? "ON" : "OFF")}", ToggleVibration,
                save.VibrationEnabled ? new Color(0.08f, 0.52f, 0.49f) : new Color(0.22f, 0.28f, 0.31f), Color.white, 28);
            SetLayout(vibration.GetComponent<RectTransform>(), 88f);
            Button language = CreateButton(column, $"{Localization.T("language")}: {(save.Language == "ru" ? "Русский" : "English")}", ToggleLanguage,
                new Color(0.11f, 0.42f, 0.56f), Color.white, 28);
            SetLayout(language.GetComponent<RectTransform>(), 88f);

            AddFlexibleSpacer(column, 0.4f);
            Button reset = CreateButton(column, Localization.T("reset"), ConfirmReset,
                new Color(0.60f, 0.18f, 0.20f), Color.white, 25);
            SetLayout(reset.GetComponent<RectTransform>(), 76f);
            Text about = CreateText(column, Localization.T("about"), 22, TextAnchor.MiddleCenter, new Color(0.68f, 0.82f, 0.84f), FontStyle.Normal);
            SetLayout(about.rectTransform, 60f);
            AddFlexibleSpacer(column, 1f);
        }

        private void ToggleSound()
        {
            save.SoundEnabled = !save.SoundEnabled;
            audioSynth.Enabled = save.SoundEnabled;
            SaveService.Save(save);
            if (save.SoundEnabled) audioSynth.PlayClick();
            ShowSettings();
        }

        private void ToggleVibration()
        {
            save.VibrationEnabled = !save.VibrationEnabled;
            SaveService.Save(save);
            if (save.VibrationEnabled) Handheld.Vibrate();
            ShowSettings();
        }

        private void ToggleLanguage()
        {
            save.Language = save.Language == "ru" ? "en" : "ru";
            Localization.Language = save.Language;
            SaveService.Save(save);
            audioSynth.PlayClick();
            ShowSettings();
        }

        private void ConfirmReset()
        {
            ShowModal(Localization.T("reset"), Localization.T("about"), Localization.T("reset"), () =>
            {
                save = SaveService.Reset();
                Localization.Language = save.Language;
                audioSynth.Enabled = save.SoundEnabled;
                ShowMainMenu();
                ShowToast(Localization.T("reset_done"));
            }, Localization.T("back"));
        }

        private void ShowModal(string title, string body, string primaryLabel, Action primaryAction,
            string secondaryLabel = null, Action secondaryAction = null)
        {
            RectTransform overlay = CreateRect(canvas.transform, "ModalOverlay");
            Stretch(overlay);
            Image dim = overlay.gameObject.AddComponent<Image>();
            dim.color = new Color(0f, 0f, 0f, 0.70f);

            RectTransform panel = CreatePanel(overlay, "ModalPanel", new Color(0.025f, 0.13f, 0.19f, 0.98f));
            panel.anchorMin = new Vector2(0.08f, 0.30f);
            panel.anchorMax = new Vector2(0.92f, 0.70f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;
            VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(28, 28, 30, 30);
            layout.spacing = 16f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            Text titleText = CreateText(panel, title, 38, TextAnchor.MiddleCenter, ProceduralArt.Gold, FontStyle.Bold);
            SetLayout(titleText.rectTransform, 64f);
            Text bodyText = CreateText(panel, body, 27, TextAnchor.MiddleCenter, Color.white, FontStyle.Normal);
            bodyText.supportRichText = true;
            SetLayout(bodyText.rectTransform, 150f);
            AddFlexibleSpacer(panel, 0.4f);

            Button primary = CreateButton(panel, primaryLabel, () =>
            {
                Destroy(overlay.gameObject);
                primaryAction?.Invoke();
            }, ProceduralArt.Coral, Color.white, 27);
            SetLayout(primary.GetComponent<RectTransform>(), 78f);

            if (!string.IsNullOrEmpty(secondaryLabel))
            {
                Button secondary = CreateButton(panel, secondaryLabel, () =>
                {
                    Destroy(overlay.gameObject);
                    secondaryAction?.Invoke();
                }, new Color(0.10f, 0.37f, 0.43f), Color.white, 24);
                SetLayout(secondary.GetComponent<RectTransform>(), 62f);
            }
        }

        private void ShowToast(string message)
        {
            StartCoroutine(ToastRoutine(message));
        }

        private IEnumerator ToastRoutine(string message)
        {
            RectTransform toast = CreatePanel(canvas.transform, "Toast", new Color(0.02f, 0.08f, 0.12f, 0.94f));
            toast.anchorMin = new Vector2(0.12f, 0.78f);
            toast.anchorMax = new Vector2(0.88f, 0.85f);
            toast.offsetMin = Vector2.zero;
            toast.offsetMax = Vector2.zero;
            CanvasGroup group = toast.gameObject.AddComponent<CanvasGroup>();
            Text text = CreateText(toast, message, 25, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            Stretch(text.rectTransform, 8f);

            yield return new WaitForSecondsRealtime(1.1f);
            for (float t = 0f; t < 0.25f; t += Time.unscaledDeltaTime)
            {
                group.alpha = 1f - t / 0.25f;
                yield return null;
            }
            Destroy(toast.gameObject);
        }
    }
}
