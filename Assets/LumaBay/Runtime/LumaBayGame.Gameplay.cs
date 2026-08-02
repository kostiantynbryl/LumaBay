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

            RectTransform header = CreatePanel(screenRoot, "GameHeader", NauticalTheme.Glass);
            header.anchorMin = new Vector2(0.018f, 0.916f);
            header.anchorMax = new Vector2(0.982f, 0.994f);
            header.offsetMin = Vector2.zero;
            header.offsetMax = Vector2.zero;
            HorizontalLayoutGroup headerLayout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            headerLayout.padding = new RectOffset(12, 12, 7, 7);
            headerLayout.spacing = 8f;
            headerLayout.childAlignment = TextAnchor.MiddleCenter;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = true;

            Button back = CreateButton(header, "‹", ShowConfirmExitLevel, NauticalTheme.Navy, NauticalTheme.Pearl, 38);
            SetLayout(back.GetComponent<RectTransform>(), -1f, 64f);
            Text levelTitle = CreateText(header, Localization.T("level", currentLevel.Id), 29, TextAnchor.MiddleLeft, NauticalTheme.Pearl, FontStyle.Bold);
            SetLayout(levelTitle.rectTransform, -1f, 1f);

            RectTransform movesMedallion = CreatePanel(header, "MovesMedallion", NauticalTheme.GlassSoft);
            SetLayout(movesMedallion, -1f, 120f);
            movesLabel = CreateText(movesMedallion, string.Empty, 24, TextAnchor.MiddleCenter, NauticalTheme.Pearl, FontStyle.Bold);
            Stretch(movesLabel.rectTransform, 6f);

            walletLabel = CreateText(header, $"◆ {save.Coins}", 25, TextAnchor.MiddleRight, NauticalTheme.GoldLight, FontStyle.Bold);
            SetLayout(walletLabel.rectTransform, -1f, 132f);

            RectTransform goals = CreatePanel(screenRoot, "Goals", NauticalTheme.Glass);
            goals.anchorMin = new Vector2(0.026f, 0.818f);
            goals.anchorMax = new Vector2(0.974f, 0.906f);
            goals.offsetMin = Vector2.zero;
            goals.offsetMax = Vector2.zero;
            HorizontalLayoutGroup goalsLayout = goals.gameObject.AddComponent<HorizontalLayoutGroup>();
            goalsLayout.padding = new RectOffset(17, 17, 8, 8);
            goalsLayout.spacing = 8f;
            goalsLayout.childAlignment = TextAnchor.MiddleCenter;
            goalsLayout.childForceExpandWidth = true;
            goalsLayout.childForceExpandHeight = true;

            collectGoalLabel = CreateText(goals, string.Empty, 20, TextAnchor.MiddleCenter, NauticalTheme.Pearl, FontStyle.Bold);
            fogGoalLabel = CreateText(goals, string.Empty, 19, TextAnchor.MiddleCenter, new Color(0.74f, 0.93f, 1f), FontStyle.Bold);
            fogGoalLabel.gameObject.SetActive(currentLevel.FogCount > 0);

            RectTransform boardZone = CreateRect(screenRoot, "BoardZone");
            boardZone.anchorMin = new Vector2(0f, 0.235f);
            boardZone.anchorMax = new Vector2(1f, 0.812f);
            boardZone.offsetMin = Vector2.zero;
            boardZone.offsetMax = Vector2.zero;

            boardGrid = CreatePanel(boardZone, "Board", NauticalTheme.Midnight);
            boardGrid.anchorMin = new Vector2(0.5f, 0.5f);
            boardGrid.anchorMax = new Vector2(0.5f, 0.5f);
            boardGrid.pivot = new Vector2(0.5f, 0.5f);
            boardGrid.sizeDelta = new Vector2(694f, 694f);
            RectMask2D boardMask = boardGrid.gameObject.AddComponent<RectMask2D>();
            boardMask.padding = Vector4.zero;

            GridLayoutGroup grid = boardGrid.gameObject.AddComponent<GridLayoutGroup>();
            grid.padding = new RectOffset(13, 13, 13, 13);
            grid.spacing = new Vector2(4f, 4f);
            grid.cellSize = new Vector2(80.5f, 80.5f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = currentLevel.Width;
            grid.childAlignment = TextAnchor.MiddleCenter;

            RectTransform footer = CreatePanel(screenRoot, "BoosterTray", NauticalTheme.Glass);
            footer.anchorMin = new Vector2(0.015f, 0.012f);
            footer.anchorMax = new Vector2(0.985f, 0.222f);
            footer.offsetMin = Vector2.zero;
            footer.offsetMax = Vector2.zero;
            HorizontalLayoutGroup footerLayout = footer.gameObject.AddComponent<HorizontalLayoutGroup>();
            footerLayout.padding = new RectOffset(11, 11, 13, 13);
            footerLayout.spacing = 7f;
            footerLayout.childAlignment = TextAnchor.MiddleCenter;
            footerLayout.childForceExpandWidth = true;
            footerLayout.childForceExpandHeight = true;

            CreateBoosterCard(footer, "ϟ", Localization.T("booster_lightning"), 200, UseLightningBolt, new Color(0.20f, 0.50f, 1f));
            CreateBoosterCard(footer, "⚓", Localization.T("booster_anchor"), 300, UseAnchorBomb, NauticalTheme.Purple);
            CreateBoosterCard(footer, "↻", Localization.T("booster_shuffle"), 50, BuyShuffle, NauticalTheme.OceanBright);
            CreateBoosterCard(footer, "+5", Localization.T("booster_time"), 100, BuyExtraMoves, new Color(0.20f, 0.68f, 0.35f));
            CreateBoosterCard(footer, "✦", Localization.T("booster_harpoon"), 250, UseMagicHarpoon, new Color(0.82f, 0.18f, 0.18f));

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
            if (!board.IsInside(b.x, b.y))
            {
                audioSynth.PlayError();
                return;
            }

            boardBusy = true;
            if (!board.TrySwap(a, b, out MoveResult result))
            {
                boardBusy = false;
                audioSynth.PlayError();
                ShowToast(Localization.T("invalid_move"));
                RefreshBoard();
                return;
            }

            movesRemaining--;
            ApplyMoveResult(result);
            audioSynth.PlayMatch();
            StartCoroutine(FinishMove(result));
        }

        private void ApplyMoveResult(MoveResult result)
        {
            if (result.Collected.TryGetValue(currentLevel.TargetPiece, out int collected))
            {
                collectedTarget += collected;
            }
            clearedFog += result.ClearedFog;
        }

        private IEnumerator FinishMove(MoveResult result)
        {
            RefreshBoard();
            UpdateGameplayLabels();
            if (result.Cascades > 1) ShowToast(Localization.T("cascade", result.Cascades));
            yield return new WaitForSecondsRealtime(0.28f);
            boardBusy = false;

            if (IsGoalComplete())
            {
                CompleteLevel();
            }
            else if (movesRemaining <= 0)
            {
                FailLevel();
            }
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
            {
                save.UnlockedLevel = Mathf.Max(save.UnlockedLevel, currentLevel.Id + 1);
            }
            SaveService.Save(save);
            audioSynth.PlayWin();
            if (save.VibrationEnabled) Handheld.Vibrate();

            string starLine = new string('★', stars) + new string('☆', 3 - stars);
            string body = $"<size=54><color=#FFD56A>{starLine}</color></size>\n\n{Localization.T("win_subtitle")}\n<size=34><color=#FFD56A>+{reward} ◆</color></size>";
            Action primary = currentLevel.Id < LevelCatalog.Count
                ? (Action)(() => StartLevel(currentLevel.Id + 1))
                : ShowMap;
            ShowModal(Localization.T("win"), body,
                currentLevel.Id < LevelCatalog.Count ? Localization.T("next") : Localization.T("map"),
                primary, Localization.T("map"), ShowMap);
        }

        private void FailLevel()
        {
            if (levelFinished) return;
            levelFinished = true;
            audioSynth.PlayError();
            ShowModal(Localization.T("lose"), Localization.T("level", currentLevel.Id),
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
            audioSynth.PlayMatch();
            RefreshBoard();
            UpdateGameplayLabels();
            ShowToast(Localization.T("booster_lightning"));
        }

        private void UseAnchorBomb()
        {
            if (!TrySpendCoins(300)) return;
            selectedCell = board.PlaceSpecial(SpecialKind.Bomb, selectedCell);
            audioSynth.PlayMatch();
            RefreshBoard();
            UpdateGameplayLabels();
            ShowToast(Localization.T("booster_anchor"));
        }

        private void BuyShuffle()
        {
            if (!TrySpendCoins(50)) return;
            board.Shuffle();
            selectedCell = null;
            audioSynth.PlayClick();
            RefreshBoard();
            UpdateGameplayLabels();
            ShowToast(Localization.T("booster_shuffle"));
        }

        private void BuyExtraMoves()
        {
            if (boardBusy || levelFinished) return;
            if (!TrySpendCoins(100)) return;
            movesRemaining += 5;
            audioSynth.PlayClick();
            UpdateGameplayLabels();
            ShowToast(Localization.T("booster_time"));
        }

        private void UseMagicHarpoon()
        {
            if (!TrySpendCoins(250)) return;
            selectedCell = board.PlaceSpecial(SpecialKind.Rainbow, selectedCell);
            audioSynth.PlayWin();
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

                    Color cellColor = selected
                        ? Color.Lerp(NauticalTheme.Gold, NauticalTheme.Navy, 0.36f)
                        : NauticalTheme.PieceCell(x, y);
                    RectTransform cellRoot = CreatePanel(boardGrid, $"Cell_{x}_{y}", cellColor);
                    PieceView view = cellRoot.gameObject.AddComponent<PieceView>();
                    view.Configure(this, x, y);

                    Image pieceGlow = CreateImage(cellRoot, "PieceGlow", ProceduralArt.Pearl($"piece_glow_{x}_{y}"),
                        selected ? new Color(1f, 0.78f, 0.25f, 0.22f) : new Color(0.30f, 0.75f, 1f, 0.08f));
                    Stretch(pieceGlow.rectTransform, 5f);
                    pieceGlow.raycastTarget = false;
                    if (selected) pieceGlow.gameObject.AddComponent<SoftGlowPulse>();

                    Image piece = CreateImage(cellRoot, "Piece", ProceduralArt.Piece(cell.Piece), Color.white);
                    Stretch(piece.rectTransform, 5f);
                    piece.raycastTarget = false;

                    if (cell.Special != SpecialKind.None)
                    {
                        string specialText = cell.Special switch
                        {
                            SpecialKind.ClearRow => "↔",
                            SpecialKind.ClearColumn => "↕",
                            SpecialKind.Bomb => "⚓",
                            SpecialKind.Rainbow => "✦",
                            _ => string.Empty
                        };
                        Text badge = CreateText(cellRoot, specialText, 31, TextAnchor.MiddleCenter, NauticalTheme.GoldLight, FontStyle.Bold);
                        badge.raycastTarget = false;
                        Stretch(badge.rectTransform, 7f);
                        Outline outline = badge.gameObject.AddComponent<Outline>();
                        outline.effectColor = new Color(0.12f, 0.03f, 0f, 0.92f);
                        outline.effectDistance = new Vector2(1.5f, -1.5f);
                    }

                    if (cell.FogLayers > 0)
                    {
                        Image fog = CreateImage(cellRoot, "Fog", ProceduralArt.OrnateFrame("fog", new Color(0.57f, 0.76f, 0.86f, 0.55f), true), Color.white);
                        Stretch(fog.rectTransform, 3f);
                        fog.raycastTarget = false;
                        Text fogText = CreateText(fog.transform, "≈", 34, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
                        Stretch(fogText.rectTransform);
                        fogText.raycastTarget = false;
                    }
                }
            }
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
                fogGoalLabel.text = Localization.T("fog", Mathf.Min(clearedFog, currentLevel.FogCount), currentLevel.FogCount);
            }
            if (walletLabel != null) walletLabel.text = $"◆ {save.Coins}";
        }

        private void ShowConfirmExitLevel()
        {
            ShowModal(Localization.T("map"), Localization.T("level", currentLevel.Id), Localization.T("map"), ShowMap,
                Localization.T("continue"));
        }
    }
}
