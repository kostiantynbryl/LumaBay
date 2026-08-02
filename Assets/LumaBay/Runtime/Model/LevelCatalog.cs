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
                CrateCount = source.CrateCount,
                IceCount = source.IceCount,
                NetCount = source.NetCount,
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

                int fog = id < 4 ? 0 : Math.Min(18, 3 + (id - 4) / 3 + chapter * 2);
                int crates = id < 6 ? 0 : Math.Min(14, 2 + (id - 6) / 3 + chapter * 2);
                int ice = id < 11 ? 0 : Math.Min(14, 2 + (id - 11) / 3 + chapter * 2);
                int nets = id < 17 ? 0 : Math.Min(9, 1 + (id - 17) / 4 + chapter);

                int moves = 27 - chapter - withinChapter / 4;
                int target = 10 + id + chapter;

                if (id <= 3)
                {
                    moves += 3;
                    target -= 2;
                }
                if (id == 10 || id == 20 || id == 30)
                {
                    moves += 2;
                    target += 5;
                    fog += 2;
                    crates += 2;
                }

                levels.Add(new LevelDefinition
                {
                    Id = id,
                    Moves = Math.Max(18, moves),
                    TargetPiece = (PieceKind)((id - 1) % 6),
                    TargetCount = Math.Max(8, target),
                    FogCount = fog,
                    CrateCount = crates,
                    IceCount = ice,
                    NetCount = nets,
                    Seed = 4100 + id * 97
                });
            }

            return levels;
        }
    }
}
