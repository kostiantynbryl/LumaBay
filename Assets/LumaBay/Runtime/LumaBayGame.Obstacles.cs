using System;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private int boardPresentationSignature = int.MinValue;
        private readonly System.Random visualRandom = new System.Random(3187);

        private void RefreshBoardPresentationIfNeeded()
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
                LayoutElement layoutElement = particle.gameObject.AddComponent<LayoutElement>();
                layoutElement.ignoreLayout = true;
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
            RectTransform overlay = CreateRect(cell, "ObstacleOverlay");
            Stretch(overlay, 2f);
            Image baseImage = overlay.gameObject.AddComponent<Image>();
            baseImage.sprite = ProceduralArt.Rounded("crate_base", new Color(0.38f, 0.20f, 0.075f, 0.95f), 8);
            baseImage.type = Image.Type.Sliced;
            baseImage.raycastTarget = false;

            AddCratePlank(overlay, "Top", new Vector2(0.06f, 0.75f), new Vector2(0.94f, 0.91f), 0f,
                new Color(0.72f, 0.42f, 0.16f, 0.98f));
            AddCratePlank(overlay, "Bottom", new Vector2(0.06f, 0.09f), new Vector2(0.94f, 0.25f), 0f,
                new Color(0.63f, 0.34f, 0.12f, 0.98f));
            AddCratePlank(overlay, "CrossA", new Vector2(0.02f, 0.42f), new Vector2(0.98f, 0.57f), 34f,
                new Color(0.78f, 0.46f, 0.18f, 0.97f));
            AddCratePlank(overlay, "CrossB", new Vector2(0.02f, 0.42f), new Vector2(0.98f, 0.57f), -34f,
                new Color(0.69f, 0.38f, 0.14f, 0.97f));

            for (int i = 0; i < 4; i++)
            {
                float x = i < 2 ? 0.13f : 0.87f;
                float y = i % 2 == 0 ? 0.18f : 0.82f;
                Image nail = CreateImage(overlay, $"Nail{i}", ProceduralArt.Pearl("crate_nail"),
                    new Color(0.35f, 0.30f, 0.24f, 0.95f));
                nail.rectTransform.anchorMin = new Vector2(x - 0.045f, y - 0.045f);
                nail.rectTransform.anchorMax = new Vector2(x + 0.045f, y + 0.045f);
                nail.rectTransform.offsetMin = Vector2.zero;
                nail.rectTransform.offsetMax = Vector2.zero;
                nail.raycastTarget = false;
            }

            if (layers > 1)
            {
                RectTransform badge = CreateRect(overlay, "LayerBadge");
                badge.anchorMin = new Vector2(0.66f, 0.64f);
                badge.anchorMax = new Vector2(0.96f, 0.94f);
                badge.offsetMin = Vector2.zero;
                badge.offsetMax = Vector2.zero;
                Image badgeImage = badge.gameObject.AddComponent<Image>();
                badgeImage.sprite = ProceduralArt.Pearl("crate_layer_badge");
                badgeImage.color = new Color(0.12f, 0.08f, 0.04f, 0.88f);
                badgeImage.raycastTarget = false;
                Text layerText = CreateText(badge, layers.ToString(), 20, TextAnchor.MiddleCenter,
                    Color.white, FontStyle.Bold);
                Stretch(layerText.rectTransform, 2f);
                layerText.raycastTarget = false;
            }
        }

        private void AddCratePlank(RectTransform parent, string name, Vector2 anchorMin, Vector2 anchorMax,
            float rotation, Color color)
        {
            Image plank = CreateImage(parent, name,
                ProceduralArt.Rounded($"crate_{name}", color, 5), Color.white);
            plank.rectTransform.anchorMin = anchorMin;
            plank.rectTransform.anchorMax = anchorMax;
            plank.rectTransform.offsetMin = Vector2.zero;
            plank.rectTransform.offsetMax = Vector2.zero;
            plank.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotation);
            plank.raycastTarget = false;
        }

        private void BuildIceOverlay(Transform cell, int layers)
        {
            RectTransform overlay = CreateRect(cell, "ObstacleOverlay");
            Stretch(overlay, 2f);
            Image ice = overlay.gameObject.AddComponent<Image>();
            ice.sprite = ProceduralArt.Rounded("ice_glass", new Color(0.55f, 0.84f, 0.98f, 0.34f), 10);
            ice.type = Image.Type.Sliced;
            ice.raycastTarget = false;

            AddIceCrack(overlay, new Vector2(0.18f, 0.84f), new Vector2(0.52f, 0.48f), 2.5f);
            AddIceCrack(overlay, new Vector2(0.52f, 0.48f), new Vector2(0.84f, 0.68f), 2f);
            AddIceCrack(overlay, new Vector2(0.52f, 0.48f), new Vector2(0.68f, 0.14f), 2.5f);
            AddIceCrack(overlay, new Vector2(0.52f, 0.48f), new Vector2(0.18f, 0.25f), 1.8f);

            Image shine = CreateImage(overlay, "IceShine", ProceduralArt.Rounded("ice_shine",
                new Color(1f, 1f, 1f, 0.22f), 6), Color.white);
            shine.rectTransform.anchorMin = new Vector2(0.10f, 0.70f);
            shine.rectTransform.anchorMax = new Vector2(0.42f, 0.80f);
            shine.rectTransform.offsetMin = Vector2.zero;
            shine.rectTransform.offsetMax = Vector2.zero;
            shine.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -18f);
            shine.raycastTarget = false;

            if (layers > 1)
            {
                Text layerText = CreateText(overlay, layers.ToString(), 19, TextAnchor.UpperRight,
                    Color.white, FontStyle.Bold);
                Stretch(layerText.rectTransform, 6f);
                layerText.raycastTarget = false;
            }
        }

        private void AddIceCrack(RectTransform parent, Vector2 from, Vector2 to, float width)
        {
            Vector2 delta = to - from;
            float length = delta.magnitude;
            Image crack = CreateImage(parent, "IceCrack", ProceduralArt.Rounded("ice_crack",
                new Color(0.82f, 0.97f, 1f, 0.80f), 2), Color.white);
            crack.rectTransform.anchorMin = from;
            crack.rectTransform.anchorMax = from;
            crack.rectTransform.pivot = new Vector2(0f, 0.5f);
            crack.rectTransform.anchoredPosition = Vector2.zero;
            crack.rectTransform.sizeDelta = new Vector2(length * 80f, width);
            crack.rectTransform.localRotation = Quaternion.Euler(0f, 0f,
                Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
            crack.raycastTarget = false;
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

            Image knot = CreateImage(overlay, "Knot", ProceduralArt.Pearl("net_knot"),
                new Color(0.82f, 0.70f, 0.43f, 0.92f));
            knot.rectTransform.anchorMin = new Vector2(0.40f, 0.40f);
            knot.rectTransform.anchorMax = new Vector2(0.60f, 0.60f);
            knot.rectTransform.offsetMin = Vector2.zero;
            knot.rectTransform.offsetMax = Vector2.zero;
            knot.raycastTarget = false;
        }

        private void AddNetRope(RectTransform parent, float verticalOffset, float angle)
        {
            Image rope = CreateImage(parent, "NetRope", ProceduralArt.Rounded("net_rope",
                new Color(0.78f, 0.66f, 0.42f, 0.74f), 3), Color.white);
            rope.rectTransform.anchorMin = new Vector2(-0.10f, 0.48f + verticalOffset);
            rope.rectTransform.anchorMax = new Vector2(1.10f, 0.52f + verticalOffset);
            rope.rectTransform.offsetMin = Vector2.zero;
            rope.rectTransform.offsetMax = Vector2.zero;
            rope.rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
            rope.raycastTarget = false;
        }
    }
}
