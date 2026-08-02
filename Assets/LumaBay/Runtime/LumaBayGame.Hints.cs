using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private Match3Board hintBoard;
        private float nextHintTime;

        private void NotifyPlayerInteraction()
        {
            nextHintTime = Time.unscaledTime + 5.5f;
        }

        private void UpdateHintAnimation()
        {
            if (board == null || boardGrid == null || boardBusy || levelFinished)
            {
                hintBoard = board;
                nextHintTime = Time.unscaledTime + 5.5f;
                return;
            }

            if (!ReferenceEquals(hintBoard, board))
            {
                hintBoard = board;
                nextHintTime = Time.unscaledTime + 4.5f;
                return;
            }

            if (Time.unscaledTime < nextHintTime) return;
            nextHintTime = Time.unscaledTime + 6.5f;

            if (!BoardHintFinder.TryFind(board, out Vector2Int first, out Vector2Int second)) return;
            AnimateHintPiece(first, second - first);
            AnimateHintPiece(second, first - second);
        }

        private void AnimateHintPiece(Vector2Int position, Vector2Int direction)
        {
            Transform cell = boardGrid.Find($"Cell_{position.x}_{position.y}");
            Transform piece = cell != null ? cell.Find("Piece") : null;
            if (piece == null || piece.GetComponent<PieceHintMotion>() != null) return;
            PieceHintMotion motion = piece.gameObject.AddComponent<PieceHintMotion>();
            motion.Configure(new Vector2(direction.x, direction.y) * 9f);
        }
    }

    public static class BoardHintFinder
    {
        public static bool TryFind(Match3Board board, out Vector2Int first, out Vector2Int second)
        {
            first = default;
            second = default;
            if (board == null) return false;

            var pieces = new PieceKind[board.Width, board.Height];
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++) pieces[x, y] = board.GetCell(x, y).Piece;
            }

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    Vector2Int a = new Vector2Int(x, y);
                    if (x + 1 < board.Width && CanSwap(board, a, new Vector2Int(x + 1, y), pieces))
                    {
                        first = a;
                        second = new Vector2Int(x + 1, y);
                        return true;
                    }
                    if (y + 1 < board.Height && CanSwap(board, a, new Vector2Int(x, y + 1), pieces))
                    {
                        first = a;
                        second = new Vector2Int(x, y + 1);
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool CanSwap(Match3Board board, Vector2Int a, Vector2Int b, PieceKind[,] pieces)
        {
            BoardCell cellA = board.GetCell(a.x, a.y);
            BoardCell cellB = board.GetCell(b.x, b.y);
            if (cellA.Piece == PieceKind.None || cellB.Piece == PieceKind.None) return false;
            if ((cellA.Obstacle == ObstacleKind.Net && cellA.ObstacleLayers > 0) ||
                (cellB.Obstacle == ObstacleKind.Net && cellB.ObstacleLayers > 0)) return false;

            PieceKind temp = pieces[a.x, a.y];
            pieces[a.x, a.y] = pieces[b.x, b.y];
            pieces[b.x, b.y] = temp;
            bool valid = HasMatchAt(pieces, board.Width, board.Height, a) ||
                         HasMatchAt(pieces, board.Width, board.Height, b);
            temp = pieces[a.x, a.y];
            pieces[a.x, a.y] = pieces[b.x, b.y];
            pieces[b.x, b.y] = temp;
            return valid;
        }

        private static bool HasMatchAt(PieceKind[,] pieces, int width, int height, Vector2Int position)
        {
            PieceKind kind = pieces[position.x, position.y];
            if (kind == PieceKind.None) return false;

            int horizontal = 1;
            for (int x = position.x - 1; x >= 0 && pieces[x, position.y] == kind; x--) horizontal++;
            for (int x = position.x + 1; x < width && pieces[x, position.y] == kind; x++) horizontal++;
            if (horizontal >= 3) return true;

            int vertical = 1;
            for (int y = position.y - 1; y >= 0 && pieces[position.x, y] == kind; y--) vertical++;
            for (int y = position.y + 1; y < height && pieces[position.x, y] == kind; y++) vertical++;
            return vertical >= 3;
        }
    }

    public sealed class PieceHintMotion : MonoBehaviour
    {
        private RectTransform rect;
        private Vector2 offset;
        private Vector2 basePosition;
        private Vector3 baseScale;
        private float startTime;
        private const float Duration = 1.25f;

        public void Configure(Vector2 movement)
        {
            offset = movement;
        }

        private void Start()
        {
            rect = transform as RectTransform;
            if (rect == null)
            {
                Destroy(this);
                return;
            }
            basePosition = rect.anchoredPosition;
            baseScale = rect.localScale;
            startTime = Time.unscaledTime;
        }

        private void Update()
        {
            if (rect == null) return;
            float t = Mathf.Clamp01((Time.unscaledTime - startTime) / Duration);
            float wave = Mathf.Sin(t * Mathf.PI * 4f) * Mathf.Pow(1f - t, 0.55f);
            rect.anchoredPosition = basePosition + offset * wave;
            rect.localScale = baseScale * (1f + Mathf.Abs(wave) * 0.055f);
            if (t < 1f) return;
            rect.anchoredPosition = basePosition;
            rect.localScale = baseScale;
            Destroy(this);
        }

        private void OnDisable()
        {
            if (rect == null) return;
            rect.anchoredPosition = basePosition;
            rect.localScale = baseScale;
        }
    }
}
