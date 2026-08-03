using System.Collections.Generic;
using UnityEngine;

namespace LumaBay
{
    public static class LumaBayArtPackV2
    {
        private static readonly Dictionary<string, Sprite[]> Sheets = new Dictionary<string, Sprite[]>();
        private static readonly Dictionary<string, Sprite> Named = new Dictionary<string, Sprite>();
        private static readonly HashSet<string> MissingLogged = new HashSet<string>();

        public static bool IsAvailable => GetNamed("tiles", 0) != null && GetNamed("ui", 20) != null;

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
            return GetNamed("tiles", index);
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
                _ => -1
            };
            return index >= 0 ? GetNamed("tiles", index) : null;
        }

        public static Sprite LighthouseState(int state)
        {
            int mapped = Mathf.RoundToInt(Mathf.InverseLerp(0f, 31f, Mathf.Clamp(state, 0, 31)) * 15f);
            return GetNamed("lighthouse", mapped);
        }

        public static Sprite MainMenuBackground => GetNamed("backgrounds", 0);
        public static Sprite GameplayBackground => GetNamed("backgrounds", 1);
        public static Sprite MapBackground => GetNamed("backgrounds", 2);
        public static Sprite StoryBackground => GetNamed("backgrounds", 3);

        // ui.png was authored as a fixed 21-element sheet. These mappings match the
        // actual top-to-bottom/left-to-right importer order and intentionally avoid
        // the baked English "Moves 16" panel at ui_01.
        public static Sprite PanelLarge => GetNamed("ui", 0);
        public static Sprite HeaderPanel => GetNamed("ui", 3);
        public static Sprite MediumPanel => GetNamed("ui", 5);
        public static Sprite PrimaryButton => GetNamed("ui", 6);
        public static Sprite GoalsPanel => GetNamed("ui", 7);
        public static Sprite ProgressDecor => GetNamed("ui", 8);
        public static Sprite CompactButton => GetNamed("ui", 16);
        public static Sprite ProgressTrack => GetNamed("ui", 17);
        public static Sprite TaskPanel => GetNamed("ui", 18);
        public static Sprite SecondaryButton => GetNamed("ui", 19);
        public static Sprite BoosterTray => GetNamed("ui", 20);

        public static Sprite UiBoosterMedallion(int index)
        {
            return index >= 0 && index < 7 ? GetNamed("ui", 9 + index) : null;
        }

        public static Sprite MapNode(int index) => GetNamed("map", index);
        public static Sprite Obstacle(int index) => GetNamed("obstacles", index);

        public static Sprite Get(string sheet, int index)
        {
            return GetNamed(sheet, index);
        }

        public static Sprite Get(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (Named.TryGetValue(name, out Sprite cached)) return cached;

            int separator = name.LastIndexOf('_');
            if (separator <= 0) return null;
            string sheet = name.Substring(0, separator);
            LoadSheet(sheet);
            return Named.TryGetValue(name, out cached) ? cached : null;
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
            MissingLogged.Clear();
        }

        private static Sprite GetNamed(string sheet, int index)
        {
            if (index < 0) return null;
            string expectedName = $"{sheet}_{index:00}";
            Sprite[] sprites = LoadSheet(sheet);
            if (sprites == null || sprites.Length == 0)
            {
                LogMissingOnce(expectedName, $"sheet '{sheet}' is empty or missing");
                return null;
            }

            if (Named.TryGetValue(expectedName, out Sprite named)) return named;

            LogMissingOnce(expectedName,
                $"expected named sprite '{expectedName}', imported {sprites.Length} sprites");
            return null;
        }

        private static void LogMissingOnce(string key, string detail)
        {
            if (!MissingLogged.Add(key)) return;
            Debug.LogError($"Luma Bay Art Pack V2: {detail}. Reimport and validate the pack.");
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
