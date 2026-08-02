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
        }

        private void BuildGameplayScreen()
        {
            ClearScreen();

            RectTransform header = CreateRect(screenRoot, "GameHeader");
            header.anchorMin = new Vector2(0f, 0.925f);
            header.anchorMax = Vector2.one;
            header.offsetMin = new Vector2(16f, 2f);
            header.offsetMax = new Vector2(-16f, -8f);
            HorizontalLayoutGroup headerLayout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            headerLayout.spacing = 10f;
            headerLayout.childAlignment = TextAnchor.MiddleCenter;
            headerLayout.childForceExpandWidth = false;

            Button back = CreateButton(header, "‹", () => ShowConfirmExitLevel(), new Color(0.04f, 0.18f, 0.25f), Color.white, 38);
            SetLayout(back.GetComponent<RectTransform>(), -1f, 68f);
            Text levelTitle = CreateText(header, Localization.T("level", currentLevel.Id), 31, TextAnchor.MiddleLeft, Color.white, FontStyle.Bold);
            SetLayout(levelTitle.rectTransform, -1f, 1f);
            walletLabel = CreateText(header, $"◈ {save.Coins}", 26, TextAnchor.MiddleRight, ProceduralArt.Gold, FontStyle.Bold);
            SetLayout(walletLabel.rectTransform, -1f, 130f);

            RectTransform goals = CreatePanel(screenRoot, "Goals", new Color(0.02f, 0.12f, 0.18f, 0.84f));
            goals.anchorMin = new Vector2(0.025f, 0.83f);
            goals.anchorMax = new Vector2(0.975f, 0.92f);
            goals.offsetMin = Vector2.zero;
            goals.offsetMax = Vector2.zero;
            HorizontalLayoutGroup goalsLayout = goals.gameObject.AddComponent<HorizontalLayoutGroup>();
            goalsLayout.padding = new RectOffset(18, 18, 8, 8);
            goalsLayout.spacing = 10f;
            goalsLayout.childAlignment = TextAnchor.MiddleCenter;
            goalsLayout.childForceExpandWidth = true;

            movesLabel = CreateText(goals, string.Empty, 25, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            collectGoalLabel = CreateText(goals, string.Empty, 21, TextAnchor.MiddleCenter, ProceduralArt.Cream, FontStyle.Normal);
            fogGoalLabel = CreateText(goals, string.Empty, 21, TextAnchor.MiddleCenter, new Color(0.75f, 0.93f, 0.95f), FontStyle.Normal);
            fogGoalLabel.gameObject.SetActive(currentLevel.FogCount > 0);

            RectTransform boardZone = CreateRect(screenRoot, "BoardZone");
            boardZone.anchorMin = new Vector2(0f, 0.205f);
            boardZone.anchorMax = new Vector2(1f, 0.825f);
            boardZone.offsetMin = Vector2.zero;
            boardZone.offsetMax = Vector2.zero;

            boardGrid = CreatePanel(boardZone, "Board", new Color(0.01f, 0.07f, 0.12f, 0.88f));
            boardGrid.anchorMin = new Vector2(0.5f, 0.5f);
            boardGrid.anchorMax = new Vector2(0.5f, 0.5f);
            boardGrid.pivot = new Vector2(0.5f, 0.5f);
            boardGrid.sizeDelta = new Vector2(684f, 684f);
            GridLayoutGroup grid = boardGrid.gameObject.AddComponent<GridLayoutGroup>();
            grid.padding = new RectOffset(12, 12, 12, 12);
            grid.spacing = new Vector2(4f, 4f);
            grid.cellSize = new Vector2(78.5f, 78.5f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = currentLevel.Width;
            grid.childAlignment = TextAnchor.MiddleCenter;

            RectTransform footer = CreateRect(screenRoot, "Boosters");
            footer.anchorMin = new Vector2(0.025f, 0.015f);
            footer.anchorMax = new Vector2(0.975f, 0.19f);
            footer.offsetMin = Vector2.zero;
            footer.offsetMax = Vector2.zero;
            HorizontalLayoutGroup footerLayout = footer.gameObject.AddComponent<HorizontalLayoutGroup>();
            footerLayout.padding = new RectOffset(12, 12, 12, 12);
            footerLayout.spacing = 14f;
            footerLayout.childAlignment = TextAnchor.MiddleCenter;
            footerLayout.childForceExpandWidth = true;

            Button shuffle = CreateButton(footer, Localization.T("shuffle"), BuyShuffle, new Color(0.10f, 0.47f, 0.55f), Color.white, 23);
            Button moves = CreateButton(footer, Localization.T("extra_moves"), BuyExtraMoves, new Color(0.56f, 0.32f, 0.70f), Color.white, 23);
            SetLayout(shuffle.GetComponent<RectTransform>(), 96f);
            SetLayout(moves.GetComponent<RectTransform>(), 96f);

            RefreshBoard();
            UpdateGameplayLabels();
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
            if (result.Collected.TryGetValue(currentLevel.TargetPiece, out int collected))
            {
                collectedTarget += collected;
            }
            clearedFog += result.ClearedFog;
            audioSynth.PlayMatch();
            StartCoroutine(FinishMove(result));
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
            save.Coins += 40 + stars * 20 + Mathf.Max(0, movesRemaining) * 2;
            if (currentLevel.Id < LevelCatalog.Count)
            {
                save.UnlockedLevel = Mathf.Max(save.UnlockedLevel, currentLevel.Id + 1);
            }
            SaveService.Save(save);
            audioSynth.PlayWin();
            if (save.VibrationEnabled) Handheld.Vibrate();

            string starLine = new string('★', stars) + new string('☆', 3 - stars);
            string body = $"<size=48>{starLine}</size>\n\n+{40 + stars * 20 + Mathf.Max(0, movesRemaining) * 2} ◈";
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

        private void BuyShuffle()
        {
            if (boardBusy || levelFinished) return;
            if (save.Coins < 50)
            {
                audioSynth.PlayError();
                ShowToast(Localization.T("not_enough"));
                return;
            }

            save.Coins -= 50;
            SaveService.Save(save);
            board.Shuffle();
            audioSynth.PlayClick();
            RefreshBoard();
            UpdateGameplayLabels();
        }

        private void BuyExtraMoves()
        {
            if (levelFinished) return;
            if (save.Coins < 100)
            {
                audioSynth.PlayError();
                ShowToast(Localization.T("not_enough"));
                return;
            }

            save.Coins -= 100;
            movesRemaining += 5;
            SaveService.Save(save);
            audioSynth.PlayClick();
            UpdateGameplayLabels();
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

                    RectTransform cellRoot = CreatePanel(boardGrid, $"Cell_{x}_{y}", selected
                        ? new Color(1f, 0.78f, 0.23f, 0.62f)
                        : new Color(0.10f, 0.27f, 0.34f, 0.62f));
                    PieceView view = cellRoot.gameObject.AddComponent<PieceView>();
                    view.Configure(this, x, y);

                    Image piece = CreateImage(cellRoot, "Piece", ProceduralArt.Piece(cell.Piece), Color.white);
                    Stretch(piece.rectTransform, 5f);
                    piece.raycastTarget = false;

                    Text symbol = CreateText(cellRoot, PieceSymbols[Mathf.Clamp((int)cell.Piece, 0, PieceSymbols.Length - 1)], 21,
                        TextAnchor.MiddleCenter, new Color(0.03f, 0.09f, 0.13f, 0.83f), FontStyle.Bold);
                    symbol.raycastTarget = false;
                    Stretch(symbol.rectTransform);

                    if (cell.Special != SpecialKind.None)
                    {
                        string specialText = cell.Special switch
                        {
                            SpecialKind.ClearRow => "H",
                            SpecialKind.ClearColumn => "V",
                            SpecialKind.Bomb => "B",
                            SpecialKind.Rainbow => "R",
                            _ => string.Empty
                        };
                        Text badge = CreateText(cellRoot, specialText, 25, TextAnchor.LowerRight, Color.white, FontStyle.Bold);
                        badge.raycastTarget = false;
                        Stretch(badge.rectTransform, 6f);
                    }

                    if (cell.FogLayers > 0)
                    {
                        Image fog = CreateImage(cellRoot, "Fog", ProceduralArt.Rounded("fog", new Color(0.77f, 0.92f, 0.96f, 0.48f), 12), Color.white);
                        Stretch(fog.rectTransform, 3f);
                        fog.raycastTarget = false;
                        Text fogText = CreateText(fog.transform, "~", 36, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
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
            if (walletLabel != null) walletLabel.text = $"◈ {save.Coins}";
        }

        private void ShowConfirmExitLevel()
        {
            ShowModal(Localization.T("map"), Localization.T("level", currentLevel.Id), Localization.T("map"), ShowMap,
                Localization.T("continue"));
        }
    }
}
