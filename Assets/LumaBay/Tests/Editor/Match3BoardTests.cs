using NUnit.Framework;
using UnityEngine;

namespace LumaBay.Tests
{
    public sealed class Match3BoardTests
    {
        [Test]
        public void CatalogContainsThirtyLevels()
        {
            Assert.AreEqual(30, LevelCatalog.Count);
        }

        [Test]
        public void GeneratedBoardContainsNoEmptyCells()
        {
            Match3Board board = new Match3Board(LevelCatalog.Get(1));
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    Assert.AreNotEqual(PieceKind.None, board.GetCell(x, y).Piece);
                }
            }
        }

        [Test]
        public void InvalidNonAdjacentSwapIsRejected()
        {
            Match3Board board = new Match3Board(LevelCatalog.Get(1));
            bool valid = board.TrySwap(new Vector2Int(0, 0), new Vector2Int(2, 0), out MoveResult result);
            Assert.IsFalse(valid);
            Assert.IsFalse(result.Valid);
        }

        [Test]
        public void LevelDifficultyProgresses()
        {
            LevelDefinition first = LevelCatalog.Get(1);
            LevelDefinition last = LevelCatalog.Get(30);
            Assert.Greater(last.TargetCount, first.TargetCount);
            Assert.Greater(last.FogCount, first.FogCount);
            Assert.LessOrEqual(last.Moves, first.Moves);
        }
    }
}
