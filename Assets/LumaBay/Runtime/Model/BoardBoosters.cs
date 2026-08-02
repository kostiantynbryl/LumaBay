using UnityEngine;

namespace LumaBay
{
    public static class BoardBoosters
    {
        public static Vector2Int PlaceSpecial(this Match3Board board, SpecialKind kind, Vector2Int? preferred = null)
        {
            Vector2Int position = preferred.HasValue && board.IsInside(preferred.Value.x, preferred.Value.y)
                ? preferred.Value
                : FindBestCell(board);

            BoardCell cell = board.GetCell(position.x, position.y);
            if (cell != null)
            {
                cell.Special = kind;
            }
            return position;
        }

        private static Vector2Int FindBestCell(Match3Board board)
        {
            Vector2Int center = new Vector2Int(board.Width / 2, board.Height / 2);
            for (int radius = 0; radius < Mathf.Max(board.Width, board.Height); radius++)
            {
                for (int x = center.x - radius; x <= center.x + radius; x++)
                {
                    for (int y = center.y - radius; y <= center.y + radius; y++)
                    {
                        if (!board.IsInside(x, y)) continue;
                        BoardCell cell = board.GetCell(x, y);
                        if (cell != null && cell.Piece != PieceKind.None && cell.Special == SpecialKind.None)
                        {
                            return new Vector2Int(x, y);
                        }
                    }
                }
            }
            return Vector2Int.zero;
        }
    }
}
