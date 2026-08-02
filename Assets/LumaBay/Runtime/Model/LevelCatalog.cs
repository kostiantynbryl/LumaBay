using System;
using System.Collections.Generic;

namespace LumaBay
{
    public static class LevelCatalog
    {
        private const int CampaignLevelCount = 60;
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
            var levels = new List<LevelDefinition>(CampaignLevelCount);
            for (int id = 1; id <= CampaignLevelCount; id++)
            {
                int chapter = (id - 1) / 10;
                int withinChapter = (id - 1) % 10;

                // Mechanics are introduced one at a time, then combined.
                int fog = id < 4 ? 0 : Math.Min(22, 3 + (id - 4) / 4 + chapter);
                int crates = id < 7 ? 0 : Math.Min(18, 2 + (id - 7) / 4 + chapter);
                int ice = id < 13 ? 0 : Math.Min(18, 2 + (id - 13) / 4 + chapter);
                int nets = id < 20 ? 0 : Math.Min(12, 1 + (id - 20) / 5 + chapter / 2);

                int moves = 30 - Math.Min(7, chapter) - withinChapter / 5;
                int target = 9 + id / 2 + chapter * 2;

                // Gentle onboarding and recovery beats after chapter finales.
                if (id <= 5)
                {
                    moves += 4;
                    target = Math.Max(7, target - 3);
                }
                if (withinChapter == 0 && id > 1)
                {
                    moves += 2;
                    target = Math.Max(8, target - 2);
                }

                // Chapter finales are memorable but not pure difficulty walls.
                if (id % 10 == 0)
                {
                    moves += 3;
                    target += 4;
                    fog += id >= 10 ? 2 : 0;
                    crates += id >= 20 ? 2 : 0;
                    ice += id >= 30 ? 2 : 0;
                }

                // Avoid overcrowding the 8x8 board.
                int obstacleBudget = 30;
                int total = fog + crates + ice + nets;
                if (total > obstacleBudget)
                {
                    float scale = obstacleBudget / (float)total;
                    fog = Math.Max(0, (int)Math.Round(fog * scale));
                    crates = Math.Max(0, (int)Math.Round(crates * scale));
                    ice = Math.Max(0, (int)Math.Round(ice * scale));
                    nets = Math.Max(0, (int)Math.Round(nets * scale));
                }

                levels.Add(new LevelDefinition
                {
                    Id = id,
                    Moves = Math.Max(18, moves),
                    TargetPiece = (PieceKind)((id + chapter) % 6),
                    TargetCount = Math.Max(7, target),
                    FogCount = fog,
                    CrateCount = crates,
                    IceCount = ice,
                    NetCount = nets,
                    Seed = 9107 + id * 173
                });
            }

            return levels;
        }
    }
}
