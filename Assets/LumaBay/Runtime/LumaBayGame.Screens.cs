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

            RectTransform column = CreateVerticalScreen(24, 13f);
            AddFlexibleSpacer(column, 0.32f);

            RectTransform logoPanel = CreatePanel(column, "LogoPanel", NauticalTheme.Glass);
            SetLayout(logoPanel, 134f);
            Text title = CreateText(logoPanel, Localization.T("title"), 65, TextAnchor.MiddleCenter, NauticalTheme.GoldLight, FontStyle.Bold);
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 44;
            title.resizeTextMaxSize = 65;
            Stretch(title.rectTransform, 12f);

            Text subtitle = CreateText(column, Localization.T("subtitle"), 25, TextAnchor.MiddleCenter, NauticalTheme.Pearl, FontStyle.Bold);
            SetLayout(subtitle.rectTransform, 46f);

            RectTransform artCard = CreatePanel(column, "HeroCard", NauticalTheme.Glass);
            SetLayout(artCard, 470f);
            BuildLighthouseArt(artCard, Mathf.RoundToInt(save.RestorationStep / 6f * 100f));

            RectTransform heroCaption = CreatePanel(artCard, "HeroCaption", new Color(0.01f, 0.08f, 0.16f, 0.94f));
            heroCaption.anchorMin = new Vector2(0.06f, 0.025f);
            heroCaption.anchorMax = new Vector2(0.94f, 0.25f);
            heroCaption.offsetMin = Vector2.zero;
            heroCaption.offsetMax = Vector2.zero;
            Text progress = CreateText(heroCaption,
                $"{Localization.T("restoration")}\n{Localization.T("progress", Mathf.RoundToInt(save.RestorationStep / 6f * 100f))}",
                24, TextAnchor.MiddleCenter, NauticalTheme.Pearl, FontStyle.Bold);
            Stretch(progress.rectTransform, 10f);

            Button play = CreateButton(column, Localization.T("play"), ShowMap, NauticalTheme.Gold, NauticalTheme.Navy, 34);
            SetLayout(play.GetComponent<RectTransform>(), 92f);
            Button levels = CreateButton(column, Localization.T("levels"), ShowLevelSelect, NauticalTheme.Ocean, Color.white, 27);
            SetLayout(levels.GetComponent<RectTransform>(), 72f);
            Button settings = CreateButton(column, Localization.T("settings"), ShowSettings, NauticalTheme.Navy, Color.white, 25);
            SetLayout(settings.GetComponent<RectTransform>(), 68f);

            Text wallet = CreateText(column, $"★ {save.AvailableStars}     ◆ {save.Coins}", 22,
                TextAnchor.MiddleCenter, NauticalTheme.GoldLight, FontStyle.Bold);
            SetLayout(wallet.rectTransform, 34f);
            Text version = CreateText(column, $"Norvexa Games • {Version}", 18, TextAnchor.MiddleCenter,
                new Color(0.68f, 0.79f, 0.84f), FontStyle.Normal);
            SetLayout(version.rectTransform, 28f);
            AddFlexibleSpacer(column, 0.18f);
        }

        private void ShowMap()
        {
            levelFinished = false;
            board = null;
            ClearScreen();

            RectTransform column = CreateVerticalScreen(20, 12f);
            RectTransform header = CreatePanel(column, "MapHeader", NauticalTheme.Glass);
            SetLayout(header, 86f);
            HorizontalLayoutGroup headerLayout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            headerLayout.padding = new RectOffset(10, 15, 7, 7);
            headerLayout.spacing = 9f;
            headerLayout.childAlignment = TextAnchor.MiddleCenter;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = true;

            Button back = CreateButton(header, "‹", ShowMainMenu, NauticalTheme.Navy, Color.white, 39);
            SetLayout(back.GetComponent<RectTransform>(), -1f, 68f);
            Text mapTitle = CreateText(header, Localization.T("map"), 35, TextAnchor.MiddleLeft, NauticalTheme.Pearl, FontStyle.Bold);
            SetLayout(mapTitle.rectTransform, -1f, 1f);
            Text wallet = CreateText(header, $"★ {save.AvailableStars}   ◆ {save.Coins}", 24,
                TextAnchor.MiddleRight, NauticalTheme.GoldLight, FontStyle.Bold);
            SetLayout(wallet.rectTransform, -1f, 220f);

            RectTransform card = CreatePanel(column, "RestorationCard", NauticalTheme.Glass);
            SetLayout(card, 555f);
            BuildLighthouseArt(card, Mathf.RoundToInt(save.RestorationStep / 6f * 100f));

            RectTransform progressBlock = CreateVertical(card, "ProgressBlock", 4f, new RectOffset(22, 22, 15, 15));
            progressBlock.anchorMin = new Vector2(0.035f, 0.015f);
            progressBlock.anchorMax = new Vector2(0.965f, 0.36f);
            progressBlock.offsetMin = Vector2.zero;
            progressBlock.offsetMax = Vector2.zero;

            Text restoration = CreateText(progressBlock, Localization.T("restoration"), 29,
                TextAnchor.MiddleCenter, NauticalTheme.GoldLight, FontStyle.Bold);
            SetLayout(restoration.rectTransform, 43f);
            int percent = Mathf.RoundToInt(save.RestorationStep / 6f * 100f);
            Text progress = CreateText(progressBlock, Localization.T("progress", percent), 22,
                TextAnchor.MiddleCenter, NauticalTheme.Pearl, FontStyle.Bold);
            SetLayout(progress.rectTransform, 31f);
            CreateProgressBar(progressBlock, save.RestorationStep / 6f);

            RectTransform storyPanel = CreatePanel(column, "StoryPanel", new Color(0.02f, 0.10f, 0.18f, 0.93f));
            SetLayout(storyPanel, 128f);
            Text story = CreateText(storyPanel, Localization.T($"story_{Mathf.Clamp(save.RestorationStep, 0, 6)}"), 23,
                TextAnchor.MiddleCenter, NauticalTheme.Pearl, FontStyle.Normal);
            story.horizontalOverflow = HorizontalWrapMode.Wrap;
            story.verticalOverflow = VerticalWrapMode.Truncate;
            Stretch(story.rectTransform, 18f);

            if (save.RestorationStep < 6)
            {
                bool canRestore = save.AvailableStars >= 3;
                Button restore = CreateButton(column,
                    canRestore ? Localization.T("restore") : Localization.T("restore_need"),
                    canRestore ? (Action)RestoreLighthouse : () => ShowToast(Localization.T("restore_need")),
                    canRestore ? NauticalTheme.Gold : new Color(0.22f, 0.29f, 0.33f),
                    canRestore ? NauticalTheme.Navy : new Color(0.70f, 0.76f, 0.78f), 26);
                SetLayout(restore.GetComponent<RectTransform>(), 76f);
            }

            Button play = CreateButton(column,
                Localization.T("level", save.UnlockedLevel) + "  •  " + Localization.T("start"),
                () => StartLevel(save.UnlockedLevel), NauticalTheme.Coral, Color.white, 29);
            SetLayout(play.GetComponent<RectTransform>(), 88f);
            Button levels = CreateButton(column, Localization.T("levels"), ShowLevelSelect,
                NauticalTheme.Ocean, Color.white, 24);
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

            RectTransform header = CreatePanel(screenRoot, "LevelsHeader", NauticalTheme.Glass);
            header.anchorMin = new Vector2(0.025f, 0.91f);
            header.anchorMax = new Vector2(0.975f, 0.99f);
            header.offsetMin = Vector2.zero;
            header.offsetMax = Vector2.zero;
            HorizontalLayoutGroup headerLayout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            headerLayout.padding = new RectOffset(10, 14, 7, 7);
            headerLayout.spacing = 10f;
            headerLayout.childAlignment = TextAnchor.MiddleCenter;
            headerLayout.childForceExpandHeight = true;
            headerLayout.childForceExpandWidth = false;

            Button back = CreateButton(header, "‹", ShowMap, NauticalTheme.Navy, Color.white, 39);
            SetLayout(back.GetComponent<RectTransform>(), -1f, 68f);
            Text title = CreateText(header, Localization.T("levels"), 35, TextAnchor.MiddleCenter,
                NauticalTheme.Pearl, FontStyle.Bold);
            SetLayout(title.rectTransform, -1f, 1f);
            Text summary = CreateText(header, $"★ {save.TotalStars}", 26, TextAnchor.MiddleRight,
                NauticalTheme.GoldLight, FontStyle.Bold);
            SetLayout(summary.rectTransform, -1f, 100f);

            RectTransform scrollRectTransform = CreatePanel(screenRoot, "LevelScroll", NauticalTheme.Glass);
            scrollRectTransform.anchorMin = new Vector2(0.025f, 0.02f);
            scrollRectTransform.anchorMax = new Vector2(0.975f, 0.895f);
            scrollRectTransform.offsetMin = Vector2.zero;
            scrollRectTransform.offsetMax = Vector2.zero;

            ScrollRect scroll = scrollRectTransform.gameObject.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.elasticity = 0.08f;
            scroll.decelerationRate = 0.12f;

            RectTransform viewport = CreateRect(scrollRectTransform, "Viewport");
            Stretch(viewport, 14f);
            Image viewportImage = viewport.gameObject.AddComponent<Image>();
            viewportImage.color = new Color(1f, 1f, 1f, 0.001f);
            Mask mask = viewport.gameObject.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            scroll.viewport = viewport;

            RectTransform content = CreateRect(viewport, "Content");
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;

            const int columns = 4;
            const float cellWidth = 145f;
            const float cellHeight = 132f;
            const float spacing = 12f;
            int rows = Mathf.CeilToInt(LevelCatalog.Count / (float)columns);
            content.sizeDelta = new Vector2(0f, rows * cellHeight + (rows - 1) * spacing + 34f);

            GridLayoutGroup grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.padding = new RectOffset(12, 12, 16, 18);
            grid.spacing = new Vector2(spacing, spacing);
            grid.cellSize = new Vector2(cellWidth, cellHeight);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.childAlignment = TextAnchor.UpperCenter;
            scroll.content = content;

            for (int levelId = 1; levelId <= LevelCatalog.Count; levelId++)
            {
                int captured = levelId;
                bool unlocked = levelId <= save.UnlockedLevel;
                int stars = save.StarsByLevel[levelId - 1];
                string starLine = new string('★', stars) + new string('☆', 3 - stars);
                string label = unlocked
                    ? $"<size=34>{levelId}</size>\n<size=18><color=#FFD76A>{starLine}</color></size>"
                    : $"<size=30>{levelId}</size>\n<size=16>{Localization.T("locked")}</size>";
                Button button = CreateButton(content, label,
                    unlocked ? (Action)(() => ShowLevelPreview(captured)) : () => ShowToast(Localization.T("locked")),
                    unlocked ? NauticalTheme.Ocean : new Color(0.11f, 0.15f, 0.19f),
                    unlocked ? Color.white : new Color(0.48f, 0.54f, 0.57f), 25);
                Text buttonText = button.GetComponentInChildren<Text>();
                buttonText.supportRichText = true;
            }
        }

        private void ShowLevelPreview(int levelId)
        {
            LevelDefinition level = LevelCatalog.Get(levelId);
            string fogLine = level.FogCount > 0 ? $"\n{Localization.T("fog", 0, level.FogCount)}" : string.Empty;
            string body = $"<color=#FFD76A>{Localization.T("goal")}</color>\n" +
                          Localization.T("collect", Localization.PieceName(level.TargetPiece), 0, level.TargetCount) + fogLine +
                          $"\n\n{Localization.T("moves", level.Moves)}";
            ShowModal(Localization.T("level", levelId), body, Localization.T("start"),
                () => StartLevel(levelId), Localization.T("back"));
        }

        private void ShowSettings()
        {
            levelFinished = false;
            board = null;
            ClearScreen();

            RectTransform column = CreateVerticalScreen(24, 16f);
            RectTransform header = CreatePanel(column, "SettingsHeader", NauticalTheme.Glass);
            SetLayout(header, 86f);
            HorizontalLayoutGroup headerLayout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            headerLayout.padding = new RectOffset(10, 14, 7, 7);
            headerLayout.spacing = 10f;
            headerLayout.childAlignment = TextAnchor.MiddleCenter;
            headerLayout.childForceExpandHeight = true;
            headerLayout.childForceExpandWidth = false;

            Button back = CreateButton(header, "‹", ShowMainMenu, NauticalTheme.Navy, Color.white, 39);
            SetLayout(back.GetComponent<RectTransform>(), -1f, 68f);
            Text title = CreateText(header, Localization.T("settings"), 36, TextAnchor.MiddleLeft,
                NauticalTheme.Pearl, FontStyle.Bold);
            SetLayout(title.rectTransform, -1f, 1f);

            AddFlexibleSpacer(column, 0.22f);
            RectTransform settingsCard = CreatePanel(column, "SettingsCard", NauticalTheme.Glass);
            SetLayout(settingsCard, 510f);
            VerticalLayoutGroup settingsLayout = settingsCard.gameObject.AddComponent<VerticalLayoutGroup>();
            settingsLayout.padding = new RectOffset(24, 24, 24, 24);
            settingsLayout.spacing = 16f;
            settingsLayout.childAlignment = TextAnchor.UpperCenter;
            settingsLayout.childForceExpandWidth = true;
            settingsLayout.childForceExpandHeight = false;

            Button sound = CreateButton(settingsCard,
                $"{Localization.T("sound")}: {(save.SoundEnabled ? "ON" : "OFF")}", ToggleSound,
                save.SoundEnabled ? new Color(0.06f, 0.47f, 0.42f) : new Color(0.19f, 0.23f, 0.26f), Color.white, 27);
            SetLayout(sound.GetComponent<RectTransform>(), 88f);
            Button vibration = CreateButton(settingsCard,
                $"{Localization.T("vibration")}: {(save.VibrationEnabled ? "ON" : "OFF")}", ToggleVibration,
                save.VibrationEnabled ? new Color(0.06f, 0.47f, 0.42f) : new Color(0.19f, 0.23f, 0.26f), Color.white, 27);
            SetLayout(vibration.GetComponent<RectTransform>(), 88f);
            Button language = CreateButton(settingsCard,
                $"{Localization.T("language")}: {(save.Language == "ru" ? "Русский" : "English")}", ToggleLanguage,
                NauticalTheme.Ocean, Color.white, 27);
            SetLayout(language.GetComponent<RectTransform>(), 88f);

            AddFlexibleSpacer(settingsCard, 0.25f);
            Button reset = CreateButton(settingsCard, Localization.T("reset"), ConfirmReset,
                new Color(0.55f, 0.12f, 0.15f), Color.white, 24);
            SetLayout(reset.GetComponent<RectTransform>(), 78f);

            Text about = CreateText(column, Localization.T("about"), 20, TextAnchor.MiddleCenter,
                new Color(0.70f, 0.82f, 0.86f), FontStyle.Normal);
            SetLayout(about.rectTransform, 50f);
            AddFlexibleSpacer(column, 0.72f);
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
            dim.color = new Color(0f, 0.015f, 0.035f, 0.80f);

            RectTransform panel = CreatePanel(overlay, "ModalPanel", NauticalTheme.GlassSoft);
            panel.anchorMin = new Vector2(0.055f, 0.245f);
            panel.anchorMax = new Vector2(0.945f, 0.755f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;
            VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(30, 30, 32, 30);
            layout.spacing = 14f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            AddModalPearl(panel, new Vector2(0f, 1f), new Vector2(22f, -22f));
            AddModalPearl(panel, new Vector2(1f, 1f), new Vector2(-22f, -22f));
            AddModalPearl(panel, new Vector2(0f, 0f), new Vector2(22f, 22f));
            AddModalPearl(panel, new Vector2(1f, 0f), new Vector2(-22f, 22f));

            Text titleText = CreateText(panel, title, 40, TextAnchor.MiddleCenter,
                NauticalTheme.GoldLight, FontStyle.Bold);
            SetLayout(titleText.rectTransform, 70f);
            Text bodyText = CreateText(panel, body, 26, TextAnchor.MiddleCenter, Color.white, FontStyle.Normal);
            bodyText.supportRichText = true;
            SetLayout(bodyText.rectTransform, 190f);
            AddFlexibleSpacer(panel, 0.25f);

            Button primary = CreateButton(panel, primaryLabel, () =>
            {
                Destroy(overlay.gameObject);
                primaryAction?.Invoke();
            }, NauticalTheme.Ocean, Color.white, 27);
            SetLayout(primary.GetComponent<RectTransform>(), 82f);

            if (!string.IsNullOrEmpty(secondaryLabel))
            {
                Button secondary = CreateButton(panel, secondaryLabel, () =>
                {
                    Destroy(overlay.gameObject);
                    secondaryAction?.Invoke();
                }, NauticalTheme.Navy, Color.white, 24);
                SetLayout(secondary.GetComponent<RectTransform>(), 66f);
            }
        }

        private void AddModalPearl(RectTransform panel, Vector2 anchor, Vector2 position)
        {
            Image pearl = CreateImage(panel, "OrnamentPearl", ProceduralArt.Pearl(), Color.white);
            pearl.rectTransform.anchorMin = anchor;
            pearl.rectTransform.anchorMax = anchor;
            pearl.rectTransform.pivot = anchor;
            pearl.rectTransform.sizeDelta = new Vector2(34f, 34f);
            pearl.rectTransform.anchoredPosition = position;
            pearl.raycastTarget = false;
            LayoutElement ignore = pearl.gameObject.AddComponent<LayoutElement>();
            ignore.ignoreLayout = true;
        }

        private void ShowToast(string message)
        {
            StartCoroutine(ToastRoutine(message));
        }

        private IEnumerator ToastRoutine(string message)
        {
            RectTransform toast = CreatePanel(canvas.transform, "Toast", NauticalTheme.GlassSoft);
            toast.anchorMin = new Vector2(0.12f, 0.785f);
            toast.anchorMax = new Vector2(0.88f, 0.855f);
            toast.offsetMin = Vector2.zero;
            toast.offsetMax = Vector2.zero;
            CanvasGroup group = toast.GetComponent<CanvasGroup>();
            if (group == null) group = toast.gameObject.AddComponent<CanvasGroup>();
            Text text = CreateText(toast, message, 24, TextAnchor.MiddleCenter, NauticalTheme.Pearl, FontStyle.Bold);
            Stretch(text.rectTransform, 10f);

            yield return new WaitForSecondsRealtime(1.10f);
            for (float t = 0f; t < 0.25f; t += Time.unscaledDeltaTime)
            {
                group.alpha = 1f - t / 0.25f;
                yield return null;
            }
            Destroy(toast.gameObject);
        }
    }
}
