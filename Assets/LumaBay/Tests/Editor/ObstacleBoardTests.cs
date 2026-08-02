using NUnit.Framework;
using UnityEngine;

namespace LumaBay.Tests
{
    public sealed class ObstacleBoardTests
    {
        [Test]
        public void BoardPlacesConfiguredObstacleCounts()
        {
            var level = new LevelDefinition
            {
                Id = 20,
                Width = 8,
                Height = 8,
                CrateCount = 6,
                IceCount = 5,
                NetCount = 4,
                Seed = 9341
            };

            var board = new Match3Board(level);
            int crates = 0;
            int ice = 0;
            int nets = 0;
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    switch (board.GetCell(x, y).Obstacle)
                    {
                        case ObstacleKind.Crate: crates++; break;
                        case ObstacleKind.Ice: ice++; break;
                        case ObstacleKind.Net: nets++; break;
                    }
                }
            }

            Assert.AreEqual(6, crates);
            Assert.AreEqual(5, ice);
            Assert.AreEqual(4, nets);
        }

        [Test]
        public void FullyNettedBoardRejectsDirectSwap()
        {
            var level = new LevelDefinition
            {
                Id = 20,
                Width = 8,
                Height = 8,
                NetCount = 64,
                Seed = 7412
            };

            var board = new Match3Board(level);
            bool valid = board.TrySwap(Vector2Int.zero, Vector2Int.right, out MoveResult result);

            Assert.IsFalse(valid);
            Assert.IsFalse(result.Valid);
        }

        [Test]
        public void CrateLayerReductionConsumesCurrentPiece()
        {
            var cell = new BoardCell
            {
                Piece = PieceKind.Crystal,
                Special = SpecialKind.Bomb,
                Obstacle = ObstacleKind.Crate,
                ObstacleLayers = 2
            };

            cell.ObstacleLayers = 1;

            Assert.AreEqual(PieceKind.None, cell.Piece);
            Assert.AreEqual(SpecialKind.None, cell.Special);
            Assert.AreEqual(1, cell.ObstacleLayers);
        }
    }
}
