using System.Collections.Generic;
using UnityEngine;

namespace LumaBay
{
    public static class LumaBayArtPackV2
    {
        private static readonly Dictionary<string, Sprite[]> Sheets = new Dictionary<string, Sprite[]>();
        private static readonly Dictionary<string, Sprite> Named = new Dictionary<string, Sprite>();

        public static bool IsAvailable => Get("tiles", 0) != null;

        public static Sprite Piece(PieceKind kind)
        {
            int index = kind switch
            {
                PieceKind.Starfish => 0,
                PieceKind.Compass => 1,
                PieceKind.Lantern => 2,
                PieceKind.Crystal => 3,
                PieceKind.Shell => 4,
                PieceKind.Flower => 5,
                _ => 0
            };
            return Get("tiles", index);
        }

        public static Sprite Booster(string id)
        {
            int index = id switch
            {
                "lightning" => 6,
                "anchor" => 7,
                "shuffle" => 8,
                "extra_moves" => 9,
                "harpoon" => 10,
                _ => 6
            };
            return Get("tiles", index);
        }

        public static Sprite LighthouseState(int state)
        {
            int mapped = Mathf.RoundToInt(Mathf.InverseLerp(0f, 31f, Mathf.Clamp(state, 0, 31)) * 15f);
            return Get("lighthouse", mapped);
        }

        public static Sprite MainMenuBackground => Get("backgrounds", 0);
        public static Sprite GameplayBackground => Get("backgrounds", 1);
        public static Sprite MapBackground => Get("backgrounds", 2);
        public static Sprite StoryBackground => Get("backgrounds", 3);

        public static Sprite PanelLarge => Get("ui", 0);
        public static Sprite MovesPanel => Get("ui", 1);
        public static Sprite PrimaryButton => Get("ui", 3);
        public static Sprite SecondaryButton => Get("ui", 5);
        public static Sprite GoalsPanel => Get("ui", 7);
        public static Sprite BoosterTray => Get("ui", 20);
        public static Sprite ProgressTrack => Get("ui", 17);
        public static Sprite ProgressFill => Get("ui", 17);

        public static Sprite MapNode(int index) => Get("map", Mathf.Clamp(index, 0, Count("map") - 1));
        public static Sprite Obstacle(int index) => Get("obstacles", Mathf.Clamp(index, 0, Count("obstacles") - 1));

        public static Sprite Get(string sheet, int index)
        {
            Sprite[] sprites = LoadSheet(sheet);
            if (sprites == null || sprites.Length == 0) return null;
            return sprites[Mathf.Clamp(index, 0, sprites.Length - 1)];
        }

        public static Sprite Get(string name)
        {
            if (Named.TryGetValue(name, out Sprite cached)) return cached;
            string sheet = name.Split('_')[0];
            Sprite[] sprites = LoadSheet(sheet);
            if (sprites == null) return null;
            foreach (Sprite sprite in sprites)
            {
                if (sprite == null || sprite.name != name) continue;
                Named[name] = sprite;
                return sprite;
            }
            return null;
        }

        public static int Count(string sheet)
        {
            Sprite[] sprites = LoadSheet(sheet);
            return sprites?.Length ?? 0;
        }

        public static void Clear()
        {
            Sheets.Clear();
            Named.Clear();
        }

        private static Sprite[] LoadSheet(string id)
        {
            if (Sheets.TryGetValue(id, out Sprite[] cached)) return cached;
            Sprite[] loaded = Resources.LoadAll<Sprite>($"ArtPackV2/Sheets/{id}");
            if (loaded != null)
            {
                System.Array.Sort(loaded, (left, right) => string.CompareOrdinal(left?.name, right?.name));
                foreach (Sprite sprite in loaded)
                {
                    if (sprite != null) Named[sprite.name] = sprite;
                }
            }
            Sheets[id] = loaded;
            return loaded;
        }
    }
}
