using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private void BeginAnimatedMove(Vector2Int a, Vector2Int b)
        {
            if (boardBusy || levelFinished || board == null) return;
            if (!board.IsInside(b.x, b.y))
            {
                audioSynth.PlayError();
                return;
            }
            StartCoroutine(AnimatedMoveRoutine(a, b));
        }

        private IEnumerator AnimatedMoveRoutine(Vector2Int a, Vector2Int b)
        {
            boardBusy = true;
            RectTransform cellA = FindBoardCell(a);
            RectTransform cellB = FindBoardCell(b);
            Image pieceA = cellA != null ? FindPieceImage(cellA) : null;
            Image pieceB = cellB != null ? FindPieceImage(cellB) : null;

            RectTransform cloneA = null;
            RectTransform cloneB = null;
            Vector2 startA = Vector2.zero;
            Vector2 startB = Vector2.zero;

            if (pieceA != null && pieceB != null && boardGrid != null)
            {
                startA = WorldToBoardLocal(pieceA.rectTransform.position);
                startB = WorldToBoardLocal(pieceB.rectTransform.position);
                cloneA = CreateMovingPiece(pieceA, startA, "MovingPieceA");
                cloneB = CreateMovingPiece(pieceB, startB, "MovingPieceB");
                pieceA.enabled = false;
                pieceB.enabled = false;
                audioSynth.PlaySwap();
                yield return AnimatePair(cloneA, cloneB, startA, startB, startB, startA, 0.14f, false);
            }

            if (!board.TrySwap(a, b, out MoveResult result))
            {
                if (cloneA != null && cloneB != null)
                {
                    yield return AnimatePair(cloneA, cloneB, startB, startA, startA, startB, 0.15f, true);
                }
                RestoreAndDestroyMovingPieces(pieceA, pieceB, cloneA, cloneB);
                boardBusy = false;
                audioSynth.PlayError();
                ShowToast(Localization.T("invalid_move"));
                RefreshBoard();
                yield break;
            }

            DestroyMovingPieces(cloneA, cloneB);
            movesRemaining--;
            ApplyMoveResult(result);
            if (result.Cascades >= 3) audioSynth.PlayCascade();
            else audioSynth.PlayMatch();
            StartCoroutine(FinishMove(result));
        }

        private RectTransform FindBoardCell(Vector2Int position)
        {
            if (boardGrid == null) return null;
            Transform transform = boardGrid.Find($"Cell_{position.x}_{position.y}");
            return transform as RectTransform;
        }

        private static Image FindPieceImage(RectTransform cell)
        {
            Transform piece = cell != null ? cell.Find("Piece") : null;
            return piece != null ? piece.GetComponent<Image>() : null;
        }

        private Vector2 WorldToBoardLocal(Vector3 worldPosition)
        {
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(null, worldPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(boardGrid, screen, null, out Vector2 local);
            return local;
        }

        private RectTransform CreateMovingPiece(Image source, Vector2 position, string objectName)
        {
            GameObject gameObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
            gameObject.transform.SetParent(boardGrid, false);
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = source.rectTransform.rect.size;
            rect.SetAsLastSibling();

            LayoutElement layout = gameObject.GetComponent<LayoutElement>();
            layout.ignoreLayout = true;

            Image image = gameObject.GetComponent<Image>();
            image.sprite = source.sprite;
            image.color = source.color;
            image.preserveAspect = true;
            image.raycastTarget = false;

            Shadow shadow = gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.34f);
            shadow.effectDistance = new Vector2(4f, -6f);
            return rect;
        }

        private static IEnumerator AnimatePair(RectTransform first, RectTransform second,
            Vector2 firstStart, Vector2 secondStart, Vector2 firstEnd, Vector2 secondEnd,
            float duration, bool bounce)
        {
            if (first == null || second == null) yield break;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = t < 0.5f
                    ? 4f * t * t * t
                    : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;
                first.anchoredPosition = Vector2.LerpUnclamped(firstStart, firstEnd, eased);
                second.anchoredPosition = Vector2.LerpUnclamped(secondStart, secondEnd, eased);
                float scale = bounce ? 1f + Mathf.Sin(t * Mathf.PI) * 0.10f : 1f + Mathf.Sin(t * Mathf.PI) * 0.045f;
                first.localScale = Vector3.one * scale;
                second.localScale = Vector3.one * scale;
                yield return null;
            }
            first.anchoredPosition = firstEnd;
            second.anchoredPosition = secondEnd;
            first.localScale = Vector3.one;
            second.localScale = Vector3.one;
        }

        private static void RestoreAndDestroyMovingPieces(Image originalA, Image originalB, RectTransform cloneA, RectTransform cloneB)
        {
            if (originalA != null) originalA.enabled = true;
            if (originalB != null) originalB.enabled = true;
            DestroyMovingPieces(cloneA, cloneB);
        }

        private static void DestroyMovingPieces(RectTransform cloneA, RectTransform cloneB)
        {
            if (cloneA != null) Object.Destroy(cloneA.gameObject);
            if (cloneB != null) Object.Destroy(cloneB.gameObject);
        }
    }
}
