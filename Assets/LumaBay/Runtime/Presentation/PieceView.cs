using UnityEngine;
using UnityEngine.EventSystems;

namespace LumaBay
{
    public sealed class PieceView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        private Vector2 pointerDown;

        public int X { get; private set; }
        public int Y { get; private set; }
        public LumaBayGame Owner { get; private set; }

        public void Configure(LumaBayGame owner, int x, int y)
        {
            Owner = owner;
            X = x;
            Y = y;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pointerDown = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Vector2 delta = eventData.position - pointerDown;
            if (delta.magnitude < 35f || Owner == null) return;

            Vector2Int direction = Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
                ? new Vector2Int(delta.x > 0f ? 1 : -1, 0)
                : new Vector2Int(0, delta.y > 0f ? 1 : -1);
            Owner.OnPieceSwipe(X, Y, direction);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if ((eventData.position - pointerDown).magnitude <= 35f && Owner != null)
            {
                Owner.OnPieceTapped(X, Y);
            }
        }
    }
}
