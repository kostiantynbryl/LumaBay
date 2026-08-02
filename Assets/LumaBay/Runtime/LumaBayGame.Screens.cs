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
            currentLevel = null;
            ClearScreen();

            RectTransform column = CreateVerticalScreen(28, 13f);
            AddFlexibleSpacer(column, 0.15f);

            RectTransform logoBlock = CreateRect(column, "LogoBlock");
            SetLayout(logoBlock, 126f);
            Text title = CreateText(logoBlock, Localization.T("title"), 67, TextAnchor.MiddleCenter,
                NauticalTheme.GoldLight, FontStyle.Bold);
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 46;
            title.resizeTextMaxSize = 67;
            Stretch(title.rectTransform);

            Text subtitle = CreateText(column, Localization.T("subtitle"), 24, TextAnchor.MiddleCenter,
                new Color(0.82f, 0.95f, 1f), FontStyle.Bold);
            SetLayout(subtitle.rectTransform, 38f);

            RectTransform hero = CreatePanel(column, "MainHero", new Color(0.03f, 0.13f, 0.22f, 0.94f));
            SetLayout(hero, 520f);
            CreateLighthouseVisual(hero, true);

            Button play = CreateButton(column, Localization.T("play"), ShowMap,
                new Color(0.06f, 0.62f, 0.65f), Color.white, 34);
            SetLayout(play.GetComponent<RectTransform>(), 88f);

            RectTransform secondaryRow = CreateHorizontal(column, "MainSecondaryRow", 12f);
            SetLayout(secondaryRow, 70f);
            Button levels = CreateButton(secondaryRow, Localization.T("levels"), ShowLevelSelect,
                new Color(0.08f, 0.30f, 0.46f), Color.white, 25);
            SetLayout(levels.GetComponent<RectTransform>(), -1f, 1f);
            Button settings = CreateButton(secondaryRow, Localization.T("settings"), ShowSettings,
                new Color(0.05f, 0.20f, 0.32f), Color.white, 24);
            SetLayout(settings.GetComponent<RectTransform>(), -1f, 1f);

            Text wallet = CreateText(column, $"★ {save.AvailableStars}     ◆ {save.Coins}", 22,
                TextAnchor.MiddleCenter, NauticalTheme.GoldLight, FontStyle.Bold);
            SetLayout(wallet.rectTransform, 32f);
            Text version = CreateText(column, $"Norvexa Games • {Version}", 17,
                TextAnchor.MiddleCenter, new Color(0.68f, 0.81f, 0.88f), FontStyle.Normal);
            SetLayout(version.rectTransform, 25f);
            AddFlexibleSpacer(column, 0.10f);
        }

        private void ShowMap()
        {
            levelFinished = false;
            board = null;
            currentLevel = null;
            ClearScreen();

            RectTransform column = CreateVerticalScreen(22, 11f);
            RectTransform header = CreatePanel(column, "MapHeader", new Color(0.025f, 0.12f, 0.21f, 0.95f));
            SetLayout(header, 82f);
            HorizontalLayoutGroup headerLayout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            headerLayout.padding = new RectOffset(10, 16, 7, 7);
            headerLayout.spacing = 10f;
            headerLayout.childAlignment = TextAnchor.MiddleCenter;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = true;

            Button back = CreateButton(header, "‹", ShowMainMenu, new Color(0.04f, 0.18f, 0.30f), Color.white, 39);
            SetLayout(back.GetComponent<RectTransform>(), -1f, 66f);
            Text mapTitle = CreateText(header, Localization.T("map"), 34, TextAnchor.MiddleLeft, Color.white, FontStyle.Bold);
            SetLayout(mapTitle.rectTransform, -1f, 1f);
            Text wallet = CreateText(header, $"★ {save.AvailableStars}   ◆ {save.Coins}", 23,
                TextAnchor.MiddleRight, NauticalTheme.GoldLight, FontStyle.Bold);
            SetLayout(wallet.rectTransform, -1f, 215f);

            RectTransform lighthouseCard = CreatePanel(column, "LighthouseMetaCard", new Color(0.025f, 0.12f, 0.20f, 0.95f));
            SetLayout(lighthouseCard, 510f);
            CreateLighthouseVisual(lighthouseCard, false);

            LighthouseTask task = save.LighthouseComplete ? null : LighthouseTaskCatalog.Get(save.LighthouseTaskIndex);
            RectTransform taskCard = CreatePanel(column, "TaskCard", new Color(0.025f, 0.12f, 0.20f, 0.96f));
            SetLayout(taskCard, 142f);
            VerticalLayoutGroup taskLayout = taskCard.gameObject.AddComponent<VerticalLayoutGroup>();
            taskLayout.padding = new RectOffset(18, 18, 13, 13);
            taskLayout.spacing = 3f;
            taskLayout.childAlignment = TextAnchor.MiddleCenter;
            taskLayout.childForceExpandWidth = true;
            taskLayout.childForceExpandHeight = false;

            string chapter = task != null ? task.Chapter : (Localization.Language == "en" ? "Lighthouse complete" : "Маяк восстановлен");
            Text chapterText = CreateText(taskCard, chapter.ToUpperInvariant(), 18, TextAnchor.MiddleCenter,
                new Color(0.52f, 0.88f, 0.96f), FontStyle.Bold);
            SetLayout(chapterText.rectTransform, 25f);
            string taskTitle = task != null ? task.Title :
                (Localization.Language == "en" ? "The light protects Luma Bay" : "Свет защищает Лума-Бэй");
            Text taskText = CreateText(taskCard, taskTitle, 27, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            taskText.resizeTextForBestFit = true;
            taskText.resizeTextMinSize = 20;
            taskText.resizeTextMaxSize = 27;
            SetLayout(taskText.rectTransform, 38f);
            Text description = CreateText(taskCard,
                task != null ? task.Description :
                    (Localization.Language == "en" ? "A signal now answers from the distant island." : "С далёкого острова приходит ответный сигнал."),
                19, TextAnchor.MiddleCenter, new Color(0.82f, 0.90f, 0.94f), FontStyle.Normal);
            description.resizeTextForBestFit = true;
            description.resizeTextMinSize = 15;
            description.resizeTextMaxSize = 19;
            SetLayout(description.rectTransform, 45f);

            RectTransform progressRow = CreateHorizontal(column, "MetaProgressRow", 10f);
            SetLayout(progressRow, 36f);
            Text progressLabel = CreateText(progressRow,
                $"{Mathf.RoundToInt(save.LighthouseProgress01 * 100f)}%   •   {save.LighthouseTaskIndex}/{LighthouseTaskCatalog.Count}",
                20, TextAnchor.MiddleLeft, Color.white, FontStyle.Bold);
            SetLayout(progressLabel.rectTransform, -1f, 155f);
            RectTransform progressTrack = CreatePanel(progressRow, "LongProgressTrack", new Color(0.01f, 0.06f, 0.11f, 0.95f));
            SetLayout(progressTrack, 28f, 1f);
            Image progressFill = CreateImage(progressTrack, "LongProgressFill",
                LumaBayArtPack.ProgressFill ?? ProceduralArt.Rounded("long_progress", NauticalTheme.Gold, 12), Color.white);
            progressFill.type = Image.Type.Sliced;
            progressFill.rectTransform.anchorMin = Vector2.zero;
            progressFill.rectTransform.anchorMax = new Vector2(save.LighthouseProgress01, 1f);
            progressFill.rectTransform.offsetMin = new Vector2(3f, 3f);
            progressFill.rectTransform.offsetMax = new Vector2(-3f, -3f);

            if (task != null)
            {
                Button restore = CreateButton(column, GetRestoreButtonLabel(task), RestoreNextLighthouseTask,
                    CanStartTask(task) ? new Color(0.06f, 0.61f, 0.64f) : new Color(0.14f, 0.23f, 0.29f),
                    Color.white, 24);
                SetLayout(restore.GetComponent<RectTransform>(), 75f);
            }

            Button play = CreateButton(column,
                Localization.T("level", save.UnlockedLevel) + "  •  " + Localization.T("start"),
                () => StartLevel(save.UnlockedLevel), new Color(0.88f, 0.30f, 0.22f), Color.white, 28);
            SetLayout(play.GetComponent<RectTransform>(), 84f);
            Button levels = CreateButton(column, Localization.T("levels"), ShowLevelSelect,
                new Color(0.07f, 0.32f, 0.48f), Color.white, 23);
            SetLayout(levels.GetComponent<RectTransform>(), 60f);
        }

        private void CreateLighthouseVisual(RectTransform parent, bool showProgressCaption)
        {
            Sprite sprite = LumaBayArtPack.LighthouseState(save.LighthouseVisualState);
            if (sprite == null)
            {
                BuildLighthouseArt(parent, Mathf.RoundToInt(save.LighthouseProgress01 * 100f));
                return;
            }

            Image image = CreateImage(parent, "PremiumLighthouseVisual", sprite, Color.white);
            image.rectTransform.anchorMin = new Vector2(0.015f, 0.015f);
            image.rectTransform.anchorMax = new Vector2(0.985f, 0.985f);
            image.rectTransform.offsetMin = Vector2.zero;
            image.rectTransform.offsetMax = Vector2.zero;
            image.preserveAspect = false;
            image.raycastTarget = false;
            image.gameObject.AddComponent<LighthouseIllustrationMotion>();

            if (!showProgressCaption) return;
            RectTransform caption = CreatePanel(parent, "HeroCaption", new Color(0.01f, 0.07f, 0.13f, 0.90f));
            caption.anchorMin = new Vector2(0.07f, 0.035f);
            caption.anchorMax = new Vector2(0.93f, 0.20f);
            caption.offsetMin = Vector2.zero;
            caption.offsetMax = Vector2.zero;
            Text text = CreateText(caption,
                $"{(Localization.Language == "en" ? "Lighthouse restoration" : "Восстановление маяка")}  •  " +
                $"{Mathf.RoundToInt(save.LighthouseProgress01 * 100f)}%",
                23, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            Stretch(text.rectTransform, 8f);
        }

        private string GetRestoreButtonLabel(LighthouseTask task)
        {
            if (save.UnlockedLevel < task.UnlockLevel)
            {
                return Localization.Language == "en"
                    ? $"UNLOCKS AFTER LEVEL {task.UnlockLevel}"
                    : $"ОТКРОЕТСЯ ПОСЛЕ УРОВНЯ {task.UnlockLevel}";
            }
            if (save.AvailableStars < task.StarCost)
            {
                return Localization.Language == "en"
                    ? $"NEED {task.StarCost} ★  •  {task.Title.ToUpperInvariant()}"
                    : $"НУЖНО {task.StarCost} ★  •  {task.Title.ToUpperInvariant()}";
            }
            return $"{task.StarCost} ★  •  {task.Title.ToUpperInvariant()}";
        }

        private bool CanStartTask(LighthouseTask task)
        {
            return task != null && save.UnlockedLevel >= task.UnlockLevel && save.AvailableStars >= task.StarCost;
        }

        private void RestoreLighthouse()
        {
            RestoreNextLighthouseTask();
        }

        private void ShowLevelSelect()
        {
            levelFinished = false;
            board = null;
            currentLevel = null;
            ClearScreen();

            RectTransform header = CreatePanel(screenRoot, "LevelsHeader", new Color(0.025f, 0.12f, 0.21f, 0.96f));
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

            Button back = CreateButton(header, "‹", ShowMap, new Color(0.04f, 0.18f, 0.30f), Color.white, 39);
            SetLayout(back.GetComponent<RectTransform>(), -1f, 66f);
            Text title = CreateText(header, Localization.T("levels"), 34, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            SetLayout(title.rectTransform, -1f, 1f);
            Text summary = CreateText(header, $"★ {save.TotalStars}", 25, TextAnchor.MiddleRight,
                NauticalTheme.GoldLight, FontStyle.Bold);
            SetLayout(summary.rectTransform, -1f, 100f);

            RectTransform scrollRoot = CreatePanel(screenRoot, "LevelScroll", new Color(0.02f, 0.09f, 0.16f, 0.91f));
            scrollRoot.anchorMin = new Vector2(0.025f, 0.02f);
            scrollRoot.anchorMax = new Vector2(0.975f, 0.895f);
            scrollRoot.offsetMin = Vector2.zero;
            scrollRoot.offsetMax = Vector2.zero;

            ScrollRect scroll = scrollRoot.gameObject.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.elasticity = 0.07f;
            scroll.decelerationRate = 0.12f;

            RectTransform viewport = CreateRect(scrollRoot, "Viewport");
            Stretch(viewport, 13f);
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

            const int columns = 3;
            const float cellWidth = 198f;
            const float cellHeight = 164f;
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
                LevelDefinition level = LevelCatalog.Get(levelId);
                string starLine = new string('★', stars) + new string('☆', 3 - stars);
                string label = unlocked
                    ? $"<size=31>{levelId}</size>\n<size=17><color=#FFD76A>{starLine}</color></size>"
                    : $"<size=29>{levelId}</size>\n<size=15>{Localization.T("locked")}</size>";
                Button button = CreateButton(content, label,
                    unlocked ? (Action)(() => ShowLevelPreview(captured)) : () => ShowToast(Localization.T("locked")),
                    unlocked ? new Color(0.055f, 0.30f, 0.47f) : new Color(0.08f, 0.13f, 0.18f),
                    unlocked ? Color.white : new Color(0.46f, 0.52f, 0.56f), 24);
                Text buttonText = button.GetComponentInChildren<Text>();
                buttonText.supportRichText = true;
                buttonText.rectTransform.anchorMin = new Vector2(0f, 0f);
                buttonText.rectTransform.anchorMax = new Vector2(1f, 0.52f);
                buttonText.rectTransform.offsetMin = new Vector2(5f, 4f);
                buttonText.rectTransform.offsetMax = new Vector2(-5f, -2f);

                if (unlocked)
                {
                    Image icon = CreateImage(button.transform, "LevelTargetIcon",
                        LumaBayArtPack.Piece(level.TargetPiece) ?? ProceduralArt.Piece(level.TargetPiece), Color.white);
                    icon.rectTransform.anchorMin = new Vector2(0.27f, 0.47f);
                    icon.rectTransform.anchorMax = new Vector2(0.73f, 0.93f);
                    icon.rectTransform.offsetMin = Vector2.zero;
                    icon.rectTransform.offsetMax = Vector2.zero;
                    icon.raycastTarget = false;
                    icon.transform.SetAsFirstSibling();
                }
                else
                {
                    Text lockIcon = CreateText(button.transform, "◆", 34, TextAnchor.MiddleCenter,
                        new Color(0.38f, 0.45f, 0.50f), FontStyle.Bold);
                    lockIcon.rectTransform.anchorMin = new Vector2(0.25f, 0.53f);
                    lockIcon.rectTransform.anchorMax = new Vector2(0.75f, 0.91f);
                    lockIcon.rectTransform.offsetMin = Vector2.zero;
                    lockIcon.rectTransform.offsetMax = Vector2.zero;
                    lockIcon.raycastTarget = false;
                }
            }

            int unlockedRow = Mathf.Max(0, (save.UnlockedLevel - 1) / columns);
            float normalized = rows <= 1 ? 1f : 1f - unlockedRow / (float)(rows - 1);
            scroll.verticalNormalizedPosition = Mathf.Clamp01(normalized);
        }

        private void ShowLevelPreview(int levelId)
        {
            LevelDefinition level = LevelCatalog.Get(levelId);
            string fogLine = level.FogCount > 0 ? $"\n{Localization.T("fog", 0, level.FogCount)}" : string.Empty;
            string obstacleLine = BuildObstaclePreview(level);
            string body = $"<color=#FFD76A>{Localization.T("goal")}</color>\n" +
                          Localization.T("collect", Localization.PieceName(level.TargetPiece), 0, level.TargetCount) + fogLine +
                          obstacleLine + $"\n\n{Localization.T("moves", level.Moves)}";
            ShowModal(Localization.T("level", levelId), body, Localization.T("start"),
                () => StartLevel(levelId), Localization.T("back"));
        }

        private string BuildObstaclePreview(LevelDefinition level)
        {
            string result = string.Empty;
            if (level.CrateCount > 0) result += Localization.Language == "en" ? $"\nCrates: {level.CrateCount}" : $"\nЯщики: {level.CrateCount}";
            if (level.IceCount > 0) result += Localization.Language == "en" ? $"\nIce: {level.IceCount}" : $"\nЛёд: {level.IceCount}";
            if (level.NetCount > 0) result += Localization.Language == "en" ? $"\nNets: {level.NetCount}" : $"\nСети: {level.NetCount}";
            return result;
        }

        private void ShowSettings()
        {
            levelFinished = false;
            board = null;
            currentLevel = null;
            ClearScreen();

            RectTransform column = CreateVerticalScreen(26, 15f);
            RectTransform header = CreatePanel(column, "SettingsHeader", new Color(0.025f, 0.12f, 0.21f, 0.96f));
            SetLayout(header, 84f);
            HorizontalLayoutGroup headerLayout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            headerLayout.padding = new RectOffset(10, 14, 7, 7);
            headerLayout.spacing = 10f;
            headerLayout.childAlignment = TextAnchor.MiddleCenter;
            headerLayout.childForceExpandHeight = true;
            headerLayout.childForceExpandWidth = false;

            Button back = CreateButton(header, "‹", ShowMainMenu, new Color(0.04f, 0.18f, 0.30f), Color.white, 39);
            SetLayout(back.GetComponent<RectTransform>(), -1f, 66f);
            Text title = CreateText(header, Localization.T("settings"), 35, TextAnchor.MiddleLeft, Color.white, FontStyle.Bold);
            SetLayout(title.rectTransform, -1f, 1f);

            AddFlexibleSpacer(column, 0.20f);
            RectTransform card = CreatePanel(column, "SettingsCard", new Color(0.025f, 0.12f, 0.20f, 0.96f));
            SetLayout(card, 575f);
            VerticalLayoutGroup layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(24, 24, 25, 25);
            layout.spacing = 15f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateSettingsButton(card, Localization.T("sound"), save.SoundEnabled, ToggleSound);
            CreateSettingsButton(card, Localization.Language == "en" ? "Music" : "Музыка", save.MusicEnabled, ToggleMusic);
            CreateSettingsButton(card, Localization.T("vibration"), save.VibrationEnabled, ToggleVibration);

            Button language = CreateButton(card,
                $"{Localization.T("language")}: {(save.Language == "ru" ? "Русский" : "English")}",
                ToggleLanguage, new Color(0.07f, 0.34f, 0.50f), Color.white, 26);
            SetLayout(language.GetComponent<RectTransform>(), 84f);

            AddFlexibleSpacer(card, 0.15f);
            Button reset = CreateButton(card, Localization.T("reset"), ConfirmReset,
                new Color(0.48f, 0.12f, 0.16f), Color.white, 23);
            SetLayout(reset.GetComponent<RectTransform>(), 68f);

            Text about = CreateText(column,
                $"Luma Bay {Version}\nNorvexa Games",
                19, TextAnchor.MiddleCenter, new Color(0.70f, 0.83f, 0.89f), FontStyle.Normal);
            SetLayout(about.rectTransform, 56f);
            AddFlexibleSpacer(column, 0.55f);
        }

        private void CreateSettingsButton(Transform parent, string title, bool enabled, Action action)
        {
            string state = enabled ? "ON" : "OFF";
            Button button = CreateButton(parent, $"{title}     {state}", action,
                enabled ? new Color(0.06f, 0.54f, 0.53f) : new Color(0.13f, 0.20f, 0.26f),
                Color.white, 27);
            SetLayout(button.GetComponent<RectTransform>(), 84f);
        }

        private void ToggleSound()
        {
            save.SoundEnabled = !save.SoundEnabled;
            audioSynth.Enabled = save.SoundEnabled;
            SaveService.Save(save);
            if (save.SoundEnabled) audioSynth.PlayClick();
            ShowSettings();
        }

        private void ToggleMusic()
        {
            save.MusicEnabled = !save.MusicEnabled;
            audioSynth.MusicEnabled = save.MusicEnabled;
            SaveService.Save(save);
            audioSynth.PlayClick();
            ShowSettings();
        }

        private void ToggleVibration()
        {
            save.VibrationEnabled = !save.VibrationEnabled;
            SaveService.Save(save);
            if (save.VibrationEnabled) Handheld.Vibrate();
            audioSynth.PlayClick();
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
            string warning = Localization.Language == "en"
                ? "All level, star and lighthouse progress will be deleted."
                : "Весь прогресс уровней, звёзд и маяка будет удалён.";
            ShowModal(Localization.T("reset"), warning, Localization.T("reset"), () =>
            {
                save = SaveService.Reset();
                Localization.Language = save.Language;
                audioSynth.Enabled = save.SoundEnabled;
                audioSynth.MusicEnabled = save.MusicEnabled;
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
            dim.color = new Color(0.005f, 0.015f, 0.03f, 0.82f);

            RectTransform panel = CreatePanel(overlay, "ModalPanel", new Color(0.025f, 0.13f, 0.22f, 0.99f));
            panel.anchorMin = new Vector2(0.07f, 0.25f);
            panel.anchorMax = new Vector2(0.93f, 0.75f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;
            VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(28, 28, 28, 28);
            layout.spacing = 14f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            Text titleText = CreateText(panel, title, 37, TextAnchor.MiddleCenter,
                NauticalTheme.GoldLight, FontStyle.Bold);
            titleText.resizeTextForBestFit = true;
            titleText.resizeTextMinSize = 25;
            titleText.resizeTextMaxSize = 37;
            SetLayout(titleText.rectTransform, 65f);

            Text bodyText = CreateText(panel, body, 25, TextAnchor.MiddleCenter, Color.white, FontStyle.Normal);
            bodyText.supportRichText = true;
            bodyText.resizeTextForBestFit = true;
            bodyText.resizeTextMinSize = 18;
            bodyText.resizeTextMaxSize = 25;
            SetLayout(bodyText.rectTransform, 185f);
            AddFlexibleSpacer(panel, 0.30f);

            Button primary = CreateButton(panel, primaryLabel, () =>
            {
                Destroy(overlay.gameObject);
                primaryAction?.Invoke();
            }, new Color(0.06f, 0.61f, 0.64f), Color.white, 26);
            SetLayout(primary.GetComponent<RectTransform>(), 76f);

            if (!string.IsNullOrEmpty(secondaryLabel))
            {
                Button secondary = CreateButton(panel, secondaryLabel, () =>
                {
                    Destroy(overlay.gameObject);
                    secondaryAction?.Invoke();
                }, new Color(0.06f, 0.25f, 0.39f), Color.white, 23);
                SetLayout(secondary.GetComponent<RectTransform>(), 61f);
            }
        }

        private void ShowToast(string message)
        {
            StartCoroutine(ToastRoutine(message));
        }

        private IEnumerator ToastRoutine(string message)
        {
            Transform existing = canvas.transform.Find("Toast");
            if (existing != null) Destroy(existing.gameObject);

            RectTransform toast = CreatePanel(canvas.transform, "Toast", new Color(0.015f, 0.07f, 0.13f, 0.97f));
            toast.anchorMin = new Vector2(0.10f, 0.78f);
            toast.anchorMax = new Vector2(0.90f, 0.86f);
            toast.offsetMin = Vector2.zero;
            toast.offsetMax = Vector2.zero;
            CanvasGroup group = toast.gameObject.AddComponent<CanvasGroup>();
            Text text = CreateText(toast, message, 24, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 17;
            text.resizeTextMaxSize = 24;
            Stretch(text.rectTransform, 12f);

            yield return new WaitForSecondsRealtime(1.25f);
            float elapsed = 0f;
            while (elapsed < 0.24f)
            {
                elapsed += Time.unscaledDeltaTime;
                group.alpha = 1f - Mathf.Clamp01(elapsed / 0.24f);
                yield return null;
            }
            Destroy(toast.gameObject);
        }
    }
}
