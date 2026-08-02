using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private void StartLevel(int levelId)
        {
            currentLevel = LevelCatalog.Get(levelId);
            board = new Match3Board(currentLevel);
            movesRemaining = currentLevel.Moves;
            collectedTarget = 0;
            clearedFog = 0;
            selectedCell = null;
            boardBusy = false;
            levelFinished = false;
            BuildGameplayScreen();
            ShowTutorialIfNeeded();
        }

        private void BuildGameplayScreen()
        {
            ClearScreen();

            RectTransform header = CreatePanel(screenRoot, "GameHeader", new Color(0.025f, 0.12f, 0.21f, 0.97f));
            header.anchorMin = new Vector2(0.018f, 0.925f);
            header.anchorMax = new Vector2(0.982f, 0.994f);
            header.offsetMin = Vector2.zero;
            header.offsetMax = Vector2.zero;
            HorizontalLayoutGroup headerLayout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            headerLayout.padding = new RectOffset(9, 12, 6, 6);
            headerLayout.spacing = 8f;
            headerLayout.childAlignment = TextAnchor.MiddleCenter;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = true;

            Button back = CreateButton(header, "‹", ShowConfirmExitLevel,
                new Color(0.04f, 0.18f, 0.30f), Color.white, 37);
            SetLayout(back.GetComponent<RectTransform>(), -1f, 60f);

            Text levelTitle = CreateText(header, Localization.T("level", currentLevel.Id), 28,
                TextAnchor.MiddleLeft, Color.white, FontStyle.Bold);
            SetLayout(levelTitle.rectTransform, -1f, 1f);

            RectTransform movesChip = CreateRect(header, "MovesChip");
            Image movesBackground = movesChip.gameObject.AddComponent<Image>();
            movesBackground.sprite = LumaBayArtPack.GoalChip ?? ProceduralArt.Rounded("moves_chip", new Color(0.07f, 0.33f, 0.48f), 18);
            movesBackground.type = Image.Type.Sliced;
            SetLayout(movesChip, -1f, 128f);
            movesLabel = CreateText(movesChip, string.Empty, 22, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            Stretch(movesLabel.rectTransform, 5f);

            walletLabel = CreateText(header, $"◆ {save.Coins}", 23,
                TextAnchor.MiddleRight, NauticalTheme.GoldLight, FontStyle.Bold);
            SetLayout(walletLabel.rectTransform, -1f, 112f);

            RectTransform goals = CreateRect(screenRoot, "Goals");
            Image goalsBackground = goals.gameObject.AddComponent<Image>();
            goalsBackground.sprite = LumaBayArtPack.GoalChip ?? ProceduralArt.Rounded("goal_bar", new Color(0.03f, 0.17f, 0.28f), 20);
            goalsBackground.type = Image.Type.Sliced;
            goals.anchorMin = new Vector2(0.026f, 0.835f);
            goals.anchorMax = new Vector2(0.974f, 0.915f);
            goals.offsetMin = Vector2.zero;
            goals.offsetMax = Vector2.zero;
            HorizontalLayoutGroup goalsLayout = goals.gameObject.AddComponent<HorizontalLayoutGroup>();
            goalsLayout.padding = new RectOffset(15, 15, 7, 7);
            goalsLayout.spacing = 8f;
            goalsLayout.childAlignment = TextAnchor.MiddleCenter;
            goalsLayout.childForceExpandWidth = true;
            goalsLayout.childForceExpandHeight = true;

            collectGoalLabel = CreateText(goals, string.Empty, 19, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            collectGoalLabel.resizeTextForBestFit = true;
            collectGoalLabel.resizeTextMinSize = 14;
            collectGoalLabel.resizeTextMaxSize = 19;
            fogGoalLabel = CreateText(goals, string.Empty, 18, TextAnchor.MiddleCenter,
                new Color(0.72f, 0.93f, 1f), FontStyle.Bold);
            fogGoalLabel.resizeTextForBestFit = true;
            fogGoalLabel.resizeTextMinSize = 13;
            fogGoalLabel.resizeTextMaxSize = 18;
            fogGoalLabel.gameObject.SetActive(currentLevel.FogCount > 0);

            RectTransform boardZone = CreateRect(screenRoot, "BoardZone");
            boardZone.anchorMin = new Vector2(0f, 0.245f);
            boardZone.anchorMax = new Vector2(1f, 0.825f);
            boardZone.offsetMin = Vector2.zero;
            boardZone.offsetMax = Vector2.zero;

            RectTransform boardFrame = CreatePanel(boardZone, "BoardFrame", new Color(0.02f, 0.09f, 0.16f, 0.98f));
            boardFrame.anchorMin = new Vector2(0.5f, 0.5f);
            boardFrame.anchorMax = new Vector2(0.5f, 0.5f);
            boardFrame.pivot = new Vector2(0.5f, 0.5f);
            boardFrame.sizeDelta = new Vector2(706f, 706f);

            boardGrid = CreateRect(boardFrame, "Board");
            Stretch(boardGrid, 10f);
            Image boardBackground = boardGrid.gameObject.AddComponent<Image>();
            boardBackground.sprite = ProceduralArt.Rounded("board_inner", new Color(0.018f, 0.075f, 0.125f, 1f), 13);
            boardBackground.type = Image.Type.Sliced;
            RectMask2D mask = boardGrid.gameObject.AddComponent<RectMask2D>();
            mask.padding = Vector4.zero;

            GridLayoutGroup grid = boardGrid.gameObject.AddComponent<GridLayoutGroup>();
            const int padding = 8;
            const float spacing = 4f;
            float available = 686f - padding * 2f - spacing * (currentLevel.Width - 1);
            float cellSize = available / currentLevel.Width;
            grid.padding = new RectOffset(padding, padding, padding, padding);
            grid.spacing = new Vector2(spacing, spacing);
            grid.cellSize = new Vector2(cellSize, cellSize);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = currentLevel.Width;
            grid.childAlignment = TextAnchor.MiddleCenter;

            RectTransform footer = CreatePanel(screenRoot, "BoosterTray", new Color(0.02f, 0.10f, 0.18f, 0.96f));
            footer.anchorMin = new Vector2(0.015f, 0.012f);
            footer.anchorMax = new Vector2(0.985f, 0.225f);
            footer.offsetMin = Vector2.zero;
            footer.offsetMax = Vector2.zero;
            HorizontalLayoutGroup footerLayout = footer.gameObject.AddComponent<HorizontalLayoutGroup>();
            footerLayout.padding = new RectOffset(9, 9, 10, 10);
            footerLayout.spacing = 6f;
            footerLayout.childAlignment = TextAnchor.MiddleCenter;
            footerLayout.childForceExpandWidth = true;
            footerLayout.childForceExpandHeight = true;

            CreateBoosterCard(footer, string.Empty, Localization.T("booster_lightning"), 200,
                UseLightningBolt, new Color(0.20f, 0.58f, 1f));
            CreateBoosterCard(footer, string.Empty, Localization.T("booster_anchor"), 300,
                UseAnchorBomb, new Color(0.55f, 0.31f, 0.92f));
            CreateBoosterCard(footer, string.Empty, Localization.T("booster_shuffle"), 50,
                BuyShuffle, new Color(0.10f, 0.70f, 0.72f));
            CreateBoosterCard(footer, string.Empty, Localization.T("booster_time"), 100,
                BuyExtraMoves, new Color(0.27f, 0.74f, 0.38f));
            CreateBoosterCard(footer, string.Empty, Localization.T("booster_harpoon"), 250,
                UseMagicHarpoon, new Color(0.92f, 0.28f, 0.20f));

            RefreshBoard();
            UpdateGameplayLabels();
        }

        private void ShowTutorialIfNeeded()
        {
            if (currentLevel.Id == 1 && !save.TutorialSwipeSeen)
            {
                save.TutorialSwipeSeen = true;
                SaveService.Save(save);
                ShowModal(Localization.T("tutorial_title"), Localization.T("tutorial_swipe"),
                    Localization.T("continue"), () => { });
                return;
            }

            if (currentLevel.Id >= 3 && !save.TutorialBoosterSeen)
            {
                save.TutorialBoosterSeen = true;
                SaveService.Save(save);
                ShowModal(Localization.T("tutorial_title"), Localization.T("tutorial_boosters"),
                    Localization.T("continue"), () => { });
            }
        }

        private void AttemptMove(Vector2Int a, Vector2Int b)
        {
            BeginAnimatedMove(a, b);
        }

        private void ApplyMoveResult(MoveResult result)
        {
            if (result.Collected.TryGetValue(currentLevel.TargetPiece, out int collected))
                collectedTarget += collected;
            clearedFog += result.ClearedFog;
        }

        private IEnumerator FinishMove(MoveResult result)
        {
            RefreshBoard();
            UpdateGameplayLabels();
            if (result.Cascades > 1) ShowToast(Localization.T("cascade", result.Cascades));
            yield return new WaitForSecondsRealtime(0.34f);
            boardBusy = false;

            if (IsGoalComplete()) CompleteLevel();
            else if (movesRemaining <= 0) FailLevel();
        }

        private bool IsGoalComplete()
        {
            return collectedTarget >= currentLevel.TargetCount && clearedFog >= currentLevel.FogCount;
        }

        private void CompleteLevel()
        {
            if (levelFinished) return;
            levelFinished = true;

            int stars = CalculateStars();
            int oldStars = save.StarsByLevel[currentLevel.Id - 1];
            int newStars = Mathf.Max(oldStars, stars);
            int delta = newStars - oldStars;
            save.StarsByLevel[currentLevel.Id - 1] = newStars;
            save.TotalStars += delta;
            save.AvailableStars += delta;

            int reward = 40 + stars * 20 + Mathf.Max(0, movesRemaining) * 2;
            save.Coins += reward;
            if (currentLevel.Id < LevelCatalog.Count)
                save.UnlockedLevel = Mathf.Max(save.UnlockedLevel, currentLevel.Id + 1);
            SaveService.Save(save);

            audioSynth.PlayWin();
            if (save.VibrationEnabled) Handheld.Vibrate();

            string starLine = new string('★', stars) + new string('☆', 3 - stars);
            string taskHint = GetUnlockedTaskHint();
            string body = $"<size=55><color=#FFD56A>{starLine}</color></size>\n" +
                          $"<size=26>{Localization.T("win_subtitle")}</size>\n" +
                          $"<size=34><color=#FFD56A>+{reward} ◆</color></size>{taskHint}";
            Action primary = currentLevel.Id < LevelCatalog.Count
                ? (Action)(() => StartLevel(currentLevel.Id + 1))
                : ShowMap;
            ShowModal(Localization.T("win"), body,
                currentLevel.Id < LevelCatalog.Count ? Localization.T("next") : Localization.T("map"),
                primary, Localization.T("map"), ShowMap);
        }

        private string GetUnlockedTaskHint()
        {
            if (save.LighthouseComplete) return string.Empty;
            LighthouseTask task = LighthouseTaskCatalog.Get(save.LighthouseTaskIndex);
            if (task == null || save.UnlockedLevel < task.UnlockLevel) return string.Empty;
            return Localization.Language == "en"
                ? "\n<size=18><color=#8EDDEB>New lighthouse task available</color></size>"
                : "\n<size=18><color=#8EDDEB>Доступно новое задание маяка</color></size>";
        }

        private void FailLevel()
        {
            if (levelFinished) return;
            levelFinished = true;
            audioSynth.PlayLose();
            ShowModal(Localization.T("lose"),
                Localization.Language == "en"
                    ? "The lighthouse still needs your help. Try a new approach or use a booster."
                    : "Маяку всё ещё нужна помощь. Попробуйте другой ход или используйте усилитель.",
                Localization.T("retry"), () => StartLevel(currentLevel.Id), Localization.T("map"), ShowMap);
        }

        private int CalculateStars()
        {
            float ratio = movesRemaining / (float)Mathf.Max(1, currentLevel.Moves);
            if (ratio >= 0.28f) return 3;
            if (ratio >= 0.10f) return 2;
            return 1;
        }

        private bool TrySpendCoins(int amount)
        {
            if (boardBusy || levelFinished) return false;
            if (save.Coins < amount)
            {
                audioSynth.PlayError();
                ShowToast(Localization.T("not_enough"));
                return false;
            }
            save.Coins -= amount;
            SaveService.Save(save);
            return true;
        }

        private void UseLightningBolt()
        {
            if (!TrySpendCoins(200)) return;
            selectedCell = board.PlaceSpecial(SpecialKind.ClearRow, selectedCell);
            audioSynth.PlayBooster();
            RefreshBoard();
            UpdateGameplayLabels();
            ShowToast(Localization.T("booster_lightning"));
        }

        private void UseAnchorBomb()
        {
            if (!TrySpendCoins(300)) return;
            selectedCell = board.PlaceSpecial(SpecialKind.Bomb, selectedCell);
            audioSynth.PlayBooster();
            RefreshBoard();
            UpdateGameplayLabels();
            ShowToast(Localization.T("booster_anchor"));
        }

        private void BuyShuffle()
        {
            if (!TrySpendCoins(50)) return;
            board.Shuffle();
            selectedCell = null;
            audioSynth.PlayBooster();
            RefreshBoard();
            UpdateGameplayLabels();
            ShowToast(Localization.T("booster_shuffle"));
        }

        private void BuyExtraMoves()
        {
            if (boardBusy || levelFinished || !TrySpendCoins(100)) return;
            movesRemaining += 5;
            audioSynth.PlayBooster();
            UpdateGameplayLabels();
            ShowToast("+5");
        }

        private void UseMagicHarpoon()
        {
            if (!TrySpendCoins(250)) return;
            selectedCell = board.PlaceSpecial(SpecialKind.Rainbow, selectedCell);
            audioSynth.PlayBooster();
            RefreshBoard();
            UpdateGameplayLabels();
            ShowToast(Localization.T("booster_harpoon"));
        }

        private void RefreshBoard()
        {
            if (boardGrid == null || board == null) return;
            DestroyChildren(boardGrid);

            for (int displayY = currentLevel.Height - 1; displayY >= 0; displayY--)
            {
                for (int x = 0; x < currentLevel.Width; x++)
                {
                    int y = displayY;
                    BoardCell cell = board.GetCell(x, y);
                    bool selected = selectedCell.HasValue && selectedCell.Value.x == x && selectedCell.Value.y == y;

                    RectTransform cellRoot = CreateRect(boardGrid, $"Cell_{x}_{y}");
                    Image cellImage = cellRoot.gameObject.AddComponent<Image>();
                    Color fill = ((x + y) & 1) == 0
                        ? new Color(0.055f, 0.16f, 0.27f, 0.98f)
                        : new Color(0.075f, 0.20f, 0.32f, 0.98f);
                    if (selected) fill = Color.Lerp(fill, new Color(0.95f, 0.69f, 0.24f), 0.42f);
                    cellImage.sprite = ProceduralArt.Rounded($"cell_{(x + y) & 1}_{selected}", fill, 11);
                    cellImage.type = Image.Type.Sliced;

                    PieceView view = cellRoot.gameObject.AddComponent<PieceView>();
                    view.Configure(this, x, y);

                    if (selected)
                    {
                        Image selection = CreateImage(cellRoot, "SelectionGlow", ProceduralArt.Pearl("selection_glow"),
                            new Color(1f, 0.77f, 0.24f, 0.24f));
                        Stretch(selection.rectTransform, 4f);
                        selection.raycastTarget = false;
                        selection.gameObject.AddComponent<SoftGlowPulse>();
                    }

                    Image piece = CreateImage(cellRoot, "Piece",
                        LumaBayArtPack.Piece(cell.Piece) ?? ProceduralArt.Piece(cell.Piece), Color.white);
                    Stretch(piece.rectTransform, 3f);
                    piece.raycastTarget = false;

                    CreateSpecialPresentation(cellRoot, cell.Special);
                    if (cell.FogLayers > 0) CreateFogPresentation(cellRoot, cell.FogLayers);
                }
            }
        }

        private void CreateSpecialPresentation(RectTransform cellRoot, SpecialKind special)
        {
            if (special == SpecialKind.None) return;

            if (special == SpecialKind.Bomb || special == SpecialKind.Rainbow)
            {
                Sprite icon = special == SpecialKind.Bomb
                    ? LumaBayArtPack.Booster("anchor")
                    : LumaBayArtPack.Booster("harpoon");
                Image image = CreateImage(cellRoot, "SpecialIcon", icon, new Color(1f, 1f, 1f, 0.92f));
                image.rectTransform.anchorMin = new Vector2(0.17f, 0.17f);
                image.rectTransform.anchorMax = new Vector2(0.83f, 0.83f);
                image.rectTransform.offsetMin = Vector2.zero;
                image.rectTransform.offsetMax = Vector2.zero;
                image.raycastTarget = false;
                image.gameObject.AddComponent<SoftGlowPulse>();
                return;
            }

            string arrow = special == SpecialKind.ClearRow ? "↔" : "↕";
            Text badge = CreateText(cellRoot, arrow, 30, TextAnchor.MiddleCenter,
                NauticalTheme.GoldLight, FontStyle.Bold);
            Stretch(badge.rectTransform, 6f);
            badge.raycastTarget = false;
            Outline outline = badge.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.04f, 0.08f, 0.12f, 0.95f);
            outline.effectDistance = new Vector2(2f, -2f);
        }

        private void CreateFogPresentation(RectTransform cellRoot, int layers)
        {
            Image fog = CreateImage(cellRoot, "Fog",
                ProceduralArt.Rounded("fog_clean", new Color(0.64f, 0.84f, 0.92f, 0.47f), 10), Color.white);
            Stretch(fog.rectTransform, 2f);
            fog.raycastTarget = false;
            Text fogText = CreateText(fog.transform, layers > 1 ? $"≈ {layers}" : "≈", 29,
                TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            Stretch(fogText.rectTransform);
            fogText.raycastTarget = false;
        }

        private void UpdateGameplayLabels()
        {
            if (movesLabel != null) movesLabel.text = Localization.T("moves", movesRemaining);
            if (collectGoalLabel != null)
            {
                collectGoalLabel.text = Localization.T("collect", Localization.PieceName(currentLevel.TargetPiece),
                    Mathf.Min(collectedTarget, currentLevel.TargetCount), currentLevel.TargetCount);
            }
            if (fogGoalLabel != null)
            {
                fogGoalLabel.text = Localization.T("fog",
                    Mathf.Min(clearedFog, currentLevel.FogCount), currentLevel.FogCount);
            }
            if (walletLabel != null) walletLabel.text = $"◆ {save.Coins}";
        }

        private void ShowConfirmExitLevel()
        {
            ShowModal(Localization.T("map"),
                Localization.Language == "en" ? "Leave this level? Current progress will be lost." : "Выйти из уровня? Текущий прогресс будет потерян.",
                Localization.T("map"), ShowMap, Localization.T("continue"));
        }
    }
}
