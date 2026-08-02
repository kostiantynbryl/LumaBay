using System;
using System.Collections.Generic;

namespace LumaBay
{
    public static class LevelCatalog
    {
        private static readonly List<LevelDefinition> Levels = BuildLevels();

        public static int Count => Levels.Count;

        public static LevelDefinition Get(int levelId)
        {
            int index = Math.Max(0, Math.Min(Levels.Count - 1, levelId - 1));
            LevelDefinition source = Levels[index];
            return new LevelDefinition
            {
                Id = source.Id,
                Width = source.Width,
                Height = source.Height,
                Moves = source.Moves,
                TargetPiece = source.TargetPiece,
                TargetCount = source.TargetCount,
                FogCount = source.FogCount,
                Seed = source.Seed
            };
        }

        private static List<LevelDefinition> BuildLevels()
        {
            var levels = new List<LevelDefinition>();
            for (int id = 1; id <= 30; id++)
            {
                int chapter = (id - 1) / 10;
                int withinChapter = (id - 1) % 10;
                int fog = id < 4 ? 0 : Math.Min(22, 4 + (id - 4) / 2 + chapter * 3);
                int moves = Math.Max(17, 26 - chapter * 2 - withinChapter / 4);
                int target = 10 + id + chapter * 2;

                levels.Add(new LevelDefinition
                {
                    Id = id,
                    Moves = moves,
                    TargetPiece = (PieceKind)((id - 1) % 6),
                    TargetCount = target,
                    FogCount = fog,
                    Seed = 4100 + id * 97
                });
            }

            return levels;
        }
    }
}
