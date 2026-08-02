using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LumaBay
{
    public sealed class Match3Board
    {
        private readonly System.Random random;
        private readonly BoardCell[,] cells;
        private readonly int pieceKindCount = 6;

        public int Width { get; }
        public int Height { get; }

        public Match3Board(LevelDefinition level)
        {
            Width = level.Width;
            Height = level.Height;
            random = new System.Random(level.Seed);
            cells = new BoardCell[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    cells[x, y] = new BoardCell();
                }
            }

            GenerateBoard();
            PlaceFog(level.FogCount);
        }

        public BoardCell GetCell(int x, int y)
        {
            return IsInside(x, y) ? cells[x, y] : null;
        }

        public bool IsInside(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool TrySwap(Vector2Int a, Vector2Int b, out MoveResult result)
        {
            result = new MoveResult();
            if (!IsInside(a.x, a.y) || !IsInside(b.x, b.y) || Manhattan(a, b) != 1)
            {
                return false;
            }

            SwapPieces(a, b);
            List<MatchGroup> groups = FindMatches();
            bool rainbowCombo = cells[a.x, a.y].Special == SpecialKind.Rainbow ||
                                cells[b.x, b.y].Special == SpecialKind.Rainbow;
            bool specialCombo = cells[a.x, a.y].Special != SpecialKind.None &&
                                cells[b.x, b.y].Special != SpecialKind.None;

            if (groups.Count == 0 && !rainbowCombo && !specialCombo)
            {
                SwapPieces(a, b);
                return false;
            }

            result.Valid = true;

            if (rainbowCombo || specialCombo)
            {
                ResolveSwapSpecials(a, b, result);
                CollapseAndRefill();
                ResolveAutomaticCascades(result);
            }
            else
            {
                ResolveGroups(groups, b, result);
                CollapseAndRefill();
                ResolveAutomaticCascades(result);
            }

            if (!HasValidMove())
            {
                Shuffle();
            }

            return true;
        }

        public void Shuffle()
        {
            var pieces = new List<(PieceKind piece, SpecialKind special)>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    pieces.Add((cells[x, y].Piece, cells[x, y].Special));
                }
            }

            for (int attempt = 0; attempt < 120; attempt++)
            {
                for (int i = pieces.Count - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);
                    (pieces[i], pieces[j]) = (pieces[j], pieces[i]);
                }

                int index = 0;
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        cells[x, y].Piece = pieces[index].piece;
                        cells[x, y].Special = pieces[index].special;
                        index++;
                    }
                }

                if (FindMatches().Count == 0 && HasValidMove())
                {
                    return;
                }
            }

            GenerateBoard();
        }

        private void GenerateBoard()
        {
            for (int attempt = 0; attempt < 60; attempt++)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        cells[x, y].Piece = PickPieceAvoidingInitialMatch(x, y);
                        cells[x, y].Special = SpecialKind.None;
                    }
                }

                if (HasValidMove())
                {
                    return;
                }
            }
        }

        private PieceKind PickPieceAvoidingInitialMatch(int x, int y)
        {
            var allowed = Enumerable.Range(0, pieceKindCount).Select(v => (PieceKind)v).ToList();
            if (x >= 2 && cells[x - 1, y].Piece == cells[x - 2, y].Piece)
            {
                allowed.Remove(cells[x - 1, y].Piece);
            }

            if (y >= 2 && cells[x, y - 1].Piece == cells[x, y - 2].Piece)
            {
                allowed.Remove(cells[x, y - 1].Piece);
            }

            return allowed[random.Next(allowed.Count)];
        }

        private void PlaceFog(int count)
        {
            var positions = new List<Vector2Int>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    positions.Add(new Vector2Int(x, y));
                }
            }

            for (int i = positions.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (positions[i], positions[j]) = (positions[j], positions[i]);
            }

            int safeCount = Math.Min(count, positions.Count);
            for (int i = 0; i < safeCount; i++)
            {
                cells[positions[i].x, positions[i].y].FogLayers = 1;
            }
        }

        private void ResolveSwapSpecials(Vector2Int a, Vector2Int b, MoveResult result)
        {
            var clear = new HashSet<Vector2Int>();
            BoardCell cellA = cells[a.x, a.y];
            BoardCell cellB = cells[b.x, b.y];

            if (cellA.Special == SpecialKind.Rainbow && cellB.Special == SpecialKind.Rainbow)
            {
                AddAll(clear);
            }
            else if (cellA.Special == SpecialKind.Rainbow)
            {
                AddAllOfKind(clear, cellB.Piece);
                clear.Add(a);
                clear.Add(b);
            }
            else if (cellB.Special == SpecialKind.Rainbow)
            {
                AddAllOfKind(clear, cellA.Piece);
                clear.Add(a);
                clear.Add(b);
            }
            else
            {
                clear.Add(a);
                clear.Add(b);
            }

            ExpandSpecialEffects(clear);
            ClearPositions(clear, result);
            result.Cascades++;
        }

        private void ResolveAutomaticCascades(MoveResult result)
        {
            int safety = 0;
            while (safety++ < 30)
            {
                List<MatchGroup> groups = FindMatches();
                if (groups.Count == 0)
                {
                    return;
                }

                ResolveGroups(groups, new Vector2Int(-1, -1), result);
                CollapseAndRefill();
            }
        }

        private void ResolveGroups(List<MatchGroup> groups, Vector2Int preferredAnchor, MoveResult result)
        {
            var clear = new HashSet<Vector2Int>();
            var creations = new Dictionary<Vector2Int, SpecialKind>();
            var orientations = new Dictionary<Vector2Int, int>();

            foreach (MatchGroup group in groups)
            {
                foreach (Vector2Int position in group.Cells)
                {
                    clear.Add(position);
                    if (!orientations.ContainsKey(position)) orientations[position] = 0;
                    orientations[position] |= group.Horizontal ? 1 : 2;
                }
            }

            foreach (KeyValuePair<Vector2Int, int> pair in orientations)
            {
                if (pair.Value == 3)
                {
                    creations[pair.Key] = SpecialKind.Bomb;
                }
            }

            foreach (MatchGroup group in groups)
            {
                SpecialKind special = SpecialKind.None;
                if (group.Cells.Count >= 5)
                {
                    special = SpecialKind.Rainbow;
                }
                else if (group.Cells.Count == 4)
                {
                    special = group.Horizontal ? SpecialKind.ClearRow : SpecialKind.ClearColumn;
                }

                if (special == SpecialKind.None)
                {
                    continue;
                }

                Vector2Int anchor = group.Cells.Contains(preferredAnchor)
                    ? preferredAnchor
                    : group.Cells[group.Cells.Count / 2];

                if (!creations.ContainsKey(anchor) || special == SpecialKind.Rainbow)
                {
                    creations[anchor] = special;
                }
            }

            foreach (Vector2Int creation in creations.Keys)
            {
                clear.Remove(creation);
            }

            ExpandSpecialEffects(clear);
            ClearPositions(clear, result);

            foreach (KeyValuePair<Vector2Int, SpecialKind> creation in creations)
            {
                BoardCell cell = cells[creation.Key.x, creation.Key.y];
                if (cell.Piece == PieceKind.None)
                {
                    cell.Piece = (PieceKind)random.Next(pieceKindCount);
                }
                cell.Special = creation.Value;
            }

            result.Cascades++;
        }

        private void ExpandSpecialEffects(HashSet<Vector2Int> clear)
        {
            var queue = new Queue<Vector2Int>(clear);
            var processed = new HashSet<Vector2Int>();

            while (queue.Count > 0)
            {
                Vector2Int position = queue.Dequeue();
                if (!IsInside(position.x, position.y) || !processed.Add(position))
                {
                    continue;
                }

                BoardCell cell = cells[position.x, position.y];
                var added = new List<Vector2Int>();
                switch (cell.Special)
                {
                    case SpecialKind.ClearRow:
                        for (int x = 0; x < Width; x++) added.Add(new Vector2Int(x, position.y));
                        break;
                    case SpecialKind.ClearColumn:
                        for (int y = 0; y < Height; y++) added.Add(new Vector2Int(position.x, y));
                        break;
                    case SpecialKind.Bomb:
                        for (int x = position.x - 1; x <= position.x + 1; x++)
                        {
                            for (int y = position.y - 1; y <= position.y + 1; y++)
                            {
                                if (IsInside(x, y)) added.Add(new Vector2Int(x, y));
                            }
                        }
                        break;
                    case SpecialKind.Rainbow:
                        AddAllOfKind(clear, MostCommonPiece());
                        foreach (Vector2Int p in clear) queue.Enqueue(p);
                        break;
                }

                foreach (Vector2Int item in added)
                {
                    if (clear.Add(item)) queue.Enqueue(item);
                }
            }
        }

        private PieceKind MostCommonPiece()
        {
            var counts = new int[pieceKindCount];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int index = (int)cells[x, y].Piece;
                    if (index >= 0 && index < counts.Length) counts[index]++;
                }
            }

            int bestIndex = 0;
            for (int i = 1; i < counts.Length; i++)
            {
                if (counts[i] > counts[bestIndex]) bestIndex = i;
            }
            return (PieceKind)bestIndex;
        }

        private void ClearPositions(HashSet<Vector2Int> clear, MoveResult result)
        {
            foreach (Vector2Int position in clear)
            {
                if (!IsInside(position.x, position.y)) continue;
                BoardCell cell = cells[position.x, position.y];
                if (cell.Piece == PieceKind.None) continue;

                result.AddCollected(cell.Piece);
                if (cell.FogLayers > 0)
                {
                    cell.FogLayers--;
                    result.ClearedFog++;
                }

                cell.Piece = PieceKind.None;
                cell.Special = SpecialKind.None;
            }
        }

        private void CollapseAndRefill()
        {
            for (int x = 0; x < Width; x++)
            {
                int writeY = 0;
                for (int readY = 0; readY < Height; readY++)
                {
                    if (cells[x, readY].IsEmpty) continue;
                    if (writeY != readY)
                    {
                        cells[x, writeY].Piece = cells[x, readY].Piece;
                        cells[x, writeY].Special = cells[x, readY].Special;
                        cells[x, readY].Piece = PieceKind.None;
                        cells[x, readY].Special = SpecialKind.None;
                    }
                    writeY++;
                }

                while (writeY < Height)
                {
                    cells[x, writeY].Piece = (PieceKind)random.Next(pieceKindCount);
                    cells[x, writeY].Special = SpecialKind.None;
                    writeY++;
                }
            }
        }

        private List<MatchGroup> FindMatches()
        {
            var groups = new List<MatchGroup>();

            for (int y = 0; y < Height; y++)
            {
                int start = 0;
                while (start < Width)
                {
                    PieceKind kind = cells[start, y].Piece;
                    int end = start + 1;
                    while (end < Width && kind != PieceKind.None && cells[end, y].Piece == kind) end++;
                    if (kind != PieceKind.None && end - start >= 3)
                    {
                        var group = new MatchGroup { Horizontal = true };
                        for (int x = start; x < end; x++) group.Cells.Add(new Vector2Int(x, y));
                        groups.Add(group);
                    }
                    start = Math.Max(end, start + 1);
                }
            }

            for (int x = 0; x < Width; x++)
            {
                int start = 0;
                while (start < Height)
                {
                    PieceKind kind = cells[x, start].Piece;
                    int end = start + 1;
                    while (end < Height && kind != PieceKind.None && cells[x, end].Piece == kind) end++;
                    if (kind != PieceKind.None && end - start >= 3)
                    {
                        var group = new MatchGroup { Horizontal = false };
                        for (int y = start; y < end; y++) group.Cells.Add(new Vector2Int(x, y));
                        groups.Add(group);
                    }
                    start = Math.Max(end, start + 1);
                }
            }

            return groups;
        }

        private bool HasValidMove()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Vector2Int a = new Vector2Int(x, y);
                    if (x + 1 < Width && SwapCreatesMatch(a, new Vector2Int(x + 1, y))) return true;
                    if (y + 1 < Height && SwapCreatesMatch(a, new Vector2Int(x, y + 1))) return true;
                }
            }
            return false;
        }

        private bool SwapCreatesMatch(Vector2Int a, Vector2Int b)
        {
            SwapPieces(a, b);
            bool createsMatch = FindMatches().Count > 0 ||
                                cells[a.x, a.y].Special == SpecialKind.Rainbow ||
                                cells[b.x, b.y].Special == SpecialKind.Rainbow;
            SwapPieces(a, b);
            return createsMatch;
        }

        private void SwapPieces(Vector2Int a, Vector2Int b)
        {
            PieceKind piece = cells[a.x, a.y].Piece;
            SpecialKind special = cells[a.x, a.y].Special;
            cells[a.x, a.y].Piece = cells[b.x, b.y].Piece;
            cells[a.x, a.y].Special = cells[b.x, b.y].Special;
            cells[b.x, b.y].Piece = piece;
            cells[b.x, b.y].Special = special;
        }

        private void AddAll(HashSet<Vector2Int> positions)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++) positions.Add(new Vector2Int(x, y));
            }
        }

        private void AddAllOfKind(HashSet<Vector2Int> positions, PieceKind kind)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (cells[x, y].Piece == kind) positions.Add(new Vector2Int(x, y));
                }
            }
        }

        private static int Manhattan(Vector2Int a, Vector2Int b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }
    }
}
