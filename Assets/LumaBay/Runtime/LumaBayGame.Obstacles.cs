using System;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private int boardPresentationSignature = int.MinValue;
        private System.Random visualRandom = new System.Random(3187);

        private void LateUpdate()
        {
            if (boardGrid == null || board == null)
            {
                boardPresentationSignature = int.MinValue;
                return;
            }

            int childCount = boardGrid.childCount;
            int firstChildId = childCount > 0 ? boardGrid.GetChild(0).GetInstanceID() : 0;
            int signature = boardGrid.GetInstanceID() ^ (childCount * 397) ^ firstChildId;
            if (signature == boardPresentationSignature) return;

            bool playBurst = boardPresentationSignature != int.MinValue && boardBusy;
            boardPresentationSignature = signature;
            RefreshObstaclePresentation();
            InstallPieceMotion();
            if (playBurst) SpawnBoardBurst();
        }

        internal void RefreshObstaclePresentation()
        {
            if (boardGrid == null || board == null) return;

            for (int i = 0; i < boardGrid.childCount; i++)
            {
                Transform cellTransform = boardGrid.GetChild(i);
                if (!cellTransform.name.StartsWith("Cell_", StringComparison.Ordinal)) continue;
                if (cellTransform.Find("ObstacleOverlay") != null) continue;

                string[] parts = cellTransform.name.Split('_');
                if (parts.Length != 3 || !int.TryParse(parts[1], out int x) || !int.TryParse(parts[2], out int y)) continue;
                BoardCell cell = board.GetCell(x, y);
                if (cell == null || cell.Obstacle == ObstacleKind.None || cell.ObstacleLayers <= 0) continue;

                switch (cell.Obstacle)
                {
                    case ObstacleKind.Crate:
                        BuildCrateOverlay(cellTransform, cell.ObstacleLayers);
                        break;
                    case ObstacleKind.Ice:
                        BuildIceOverlay(cellTransform, cell.ObstacleLayers);
                        break;
                    case ObstacleKind.Net:
                        BuildNetOverlay(cellTransform);
                        break;
                }
            }
        }

        private void InstallPieceMotion()
        {
            if (boardGrid == null) return;
            for (int i = 0; i < boardGrid.childCount; i++)
            {
                Transform cell = boardGrid.GetChild(i);
                if (!cell.name.StartsWith("Cell_", StringComparison.Ordinal)) continue;
                Transform piece = cell.Find("Piece");
                if (piece == null || piece.GetComponent<PieceDropMotion>() != null) continue;
                piece.gameObject.AddComponent<PieceDropMotion>();
            }
        }

        private void SpawnBoardBurst()
        {
            if (boardGrid == null) return;
            int count = 8 + Math.Min(10, Math.Max(0, currentLevel != null ? currentLevel.Id / 3 : 0));
            for (int i = 0; i < count; i++)
            {
                RectTransform particle = CreateRect(boardGrid, "MatchSparkle");
                particle.anchorMin = new Vector2(0.5f, 0.5f);
                particle.anchorMax = new Vector2(0.5f, 0.5f);
                particle.pivot = new Vector2(0.5f, 0.5f);
                float angle = (float)visualRandom.NextDouble() * Mathf.PI * 2f;
                float radius = 40f + (float)visualRandom.NextDouble() * 210f;
                particle.anchoredPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                float size = 8f + (float)visualRandom.NextDouble() * 15f;
                particle.sizeDelta = new Vector2(size, size);

                Image image = particle.gameObject.AddComponent<Image>();
                image.sprite = ProceduralArt.Pearl("match_sparkle");
                image.raycastTarget = false;
                image.color = i % 3 == 0
                    ? new Color(0.42f, 0.86f, 1f, 0.92f)
                    : new Color(1f, 0.80f, 0.30f, 0.92f);

                float speed = 75f + (float)visualRandom.NextDouble() * 125f;
                Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                UiBurstParticle burst = particle.gameObject.AddComponent<UiBurstParticle>();
                burst.Launch(velocity, 0.42f + (float)visualRandom.NextDouble() * 0.30f,
                    -160f + (float)visualRandom.NextDouble() * 320f);
            }
        }

        private void BuildCrateOverlay(Transform cell, int layers)
        {
            RectTransform overlay = CreatePanel(cell, "ObstacleOverlay", new Color(0.34f, 0.17f, 0.06f, 0.92f));
            Stretch(overlay, 3f);
            overlay.GetComponent<Image>().raycastTarget = false;

            Image plankA = CreateImage(overlay, "PlankA", ProceduralArt.Rounded("crate_plank_a", new Color(0.70f, 0.38f, 0.12f, 0.96f), 5), Color.white);
            plankA.rectTransform.anchorMin = new Vector2(0.02f, 0.43f);
            plankA.rectTransform.anchorMax = new Vector2(0.98f, 0.58f);
            plankA.rectTransform.offsetMin = Vector2.zero;
            plankA.rectTransform.offsetMax = Vector2.zero;
            plankA.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 33f);
            plankA.raycastTarget = false;

            Image plankB = CreateImage(overlay, "PlankB", ProceduralArt.Rounded("crate_plank_b", new Color(0.72f, 0.40f, 0.13f, 0.96f), 5), Color.white);
            plankB.rectTransform.anchorMin = new Vector2(0.02f, 0.43f);
            plankB.rectTransform.anchorMax = new Vector2(0.98f, 0.58f);
            plankB.rectTransform.offsetMin = Vector2.zero;
            plankB.rectTransform.offsetMax = Vector2.zero;
            plankB.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -33f);
            plankB.raycastTarget = false;

            if (layers > 1)
            {
                Text layerText = CreateText(overlay, layers.ToString(), 22, TextAnchor.UpperRight, NauticalTheme.Pearl, FontStyle.Bold);
                Stretch(layerText.rectTransform, 7f);
                layerText.raycastTarget = false;
            }
        }

        private void BuildIceOverlay(Transform cell, int layers)
        {
            RectTransform overlay = CreatePanel(cell, "ObstacleOverlay", new Color(0.43f, 0.79f, 0.98f, 0.34f));
            Stretch(overlay, 2f);
            Image image = overlay.GetComponent<Image>();
            image.raycastTarget = false;

            Text ice = CreateText(overlay, "❄", 38, TextAnchor.MiddleCenter, new Color(0.88f, 0.98f, 1f, 0.86f), FontStyle.Bold);
            Stretch(ice.rectTransform, 4f);
            ice.raycastTarget = false;
            if (layers > 1)
            {
                Outline outline = ice.gameObject.AddComponent<Outline>();
                outline.effectColor = new Color(0.15f, 0.45f, 0.75f, 0.90f);
                outline.effectDistance = new Vector2(2f, -2f);
            }
        }

        private void BuildNetOverlay(Transform cell)
        {
            RectTransform overlay = CreateRect(cell, "ObstacleOverlay");
            Stretch(overlay, 3f);
            Image catcher = overlay.gameObject.AddComponent<Image>();
            catcher.color = new Color(1f, 1f, 1f, 0.001f);
            catcher.raycastTarget = false;

            for (int i = -2; i <= 2; i++)
            {
                float offset = i * 0.21f;
                AddNetRope(overlay, offset, 28f);
                AddNetRope(overlay, offset, -28f);
            }

            Image knot = CreateImage(overlay, "Knot", ProceduralArt.Pearl("net_knot"), new Color(0.82f, 0.70f, 0.43f, 0.92f));
            knot.rectTransform.anchorMin = new Vector2(0.40f, 0.40f);
            knot.rectTransform.anchorMax = new Vector2(0.60f, 0.60f);
            knot.rectTransform.offsetMin = Vector2.zero;
            knot.rectTransform.offsetMax = Vector2.zero;
            knot.raycastTarget = false;
        }

        private void AddNetRope(RectTransform parent, float verticalOffset, float angle)
        {
            Image rope = CreateImage(parent, "NetRope", ProceduralArt.Rounded("net_rope", new Color(0.78f, 0.66f, 0.42f, 0.78f), 3), Color.white);
            rope.rectTransform.anchorMin = new Vector2(-0.10f, 0.48f + verticalOffset);
            rope.rectTransform.anchorMax = new Vector2(1.10f, 0.52f + verticalOffset);
            rope.rectTransform.offsetMin = Vector2.zero;
            rope.rectTransform.offsetMax = Vector2.zero;
            rope.rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
            rope.raycastTarget = false;
        }
    }
}
