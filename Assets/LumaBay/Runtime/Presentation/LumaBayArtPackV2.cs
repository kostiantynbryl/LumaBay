using System.Collections.Generic;
using UnityEngine;

namespace LumaBay
{
    public static class LumaBayArtPackV2
    {
        private static readonly Dictionary<string, Sprite[]> Sheets = new Dictionary<string, Sprite[]>();
        private static readonly Dictionary<string, Sprite> Named = new Dictionary<string, Sprite>();
        private static readonly HashSet<string> MissingLogged = new HashSet<string>();

        public static bool IsAvailable =>
            LumaBayColorfulUi.IsAvailable ||
            (GetNamed("tiles", 0) != null && GetNamed("ui", 20) != null);

        public static bool IsColorfulUiActive => LumaBayColorfulUi.IsAvailable;

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

        // Colorful UI Kit is the preferred UI source when a configured theme asset exists.
        // The authored Art Pack V2 remains the automatic fallback for clean checkouts and CI.
        public static Sprite PanelLarge => LumaBayColorfulUi.PanelLarge ?? GetNamed("ui", 0);
        public static Sprite HeaderPanel => LumaBayColorfulUi.HeaderPanel ?? GetNamed("ui", 2);
        public static Sprite MediumPanel => LumaBayColorfulUi.MediumPanel ?? GetNamed("ui", 3);
        public static Sprite PrimaryButton => LumaBayColorfulUi.PrimaryButton ?? GetNamed("ui", 4);
        public static Sprite SecondaryButton => LumaBayColorfulUi.SecondaryButton ?? GetNamed("ui", 5);
        public static Sprite CompactButton => LumaBayColorfulUi.CompactButton ?? GetNamed("ui", 6);
        public static Sprite GoalsPanel => LumaBayColorfulUi.GoalsPanel ?? GetNamed("ui", 7);
        public static Sprite ProgressDecor => LumaBayColorfulUi.ProgressDecor ?? GetNamed("ui", 8);
        public static Sprite BackButton => LumaBayColorfulUi.BackButton ?? GetNamed("ui", 15);
        public static Sprite CompactPanel => LumaBayColorfulUi.CompactPanel ?? GetNamed("ui", 16);
        public static Sprite ProgressTrack => LumaBayColorfulUi.ProgressTrack ?? GetNamed("ui", 17);
        public static Sprite TaskPanel => LumaBayColorfulUi.TaskPanel ?? GetNamed("ui", 18);
        public static Sprite BottomNavigation => LumaBayColorfulUi.BottomNavigation ?? GetNamed("ui", 19);
        public static Sprite BoosterTray => LumaBayColorfulUi.BoosterTray ?? GetNamed("ui", 20);
        public static Sprite ProgressFill => LumaBayColorfulUi.ProgressFill;
        public static Sprite IconPlate => LumaBayColorfulUi.IconPlate ?? CompactButton;

        public static Sprite UiBoosterMedallion(int index)
        {
            if (LumaBayColorfulUi.IconPlate != null) return LumaBayColorfulUi.IconPlate;
            return index >= 0 && index < 7 ? GetNamed("ui", 9 + index) : null;
        }

        public static Sprite MapNode(int index) => GetNamed("map", index);
        public static Sprite Obstacle(int index) => GetNamed("obstacles", index);

        public static Sprite Get(string sheet, int index)
        {
            if (sheet == "ui")
            {
                Sprite themed = GetThemedUiSprite(index);
                if (themed != null) return themed;
            }

            return GetNamed(sheet, index);
        }

        public static Sprite Get(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (Named.TryGetValue(name, out Sprite cached)) return cached;

            int separator = name.LastIndexOf('_');
            if (separator <= 0) return null;
            string sheet = name.Substring(0, separator);
            if (sheet == "ui" && int.TryParse(name.Substring(separator + 1), out int index))
            {
                Sprite themed = GetThemedUiSprite(index);
                if (themed != null) return themed;
            }

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
            LumaBayColorfulUi.Reload();
        }

        private static Sprite GetThemedUiSprite(int index)
        {
            if (!LumaBayColorfulUi.IsAvailable) return null;

            return index switch
            {
                0 => LumaBayColorfulUi.PanelLarge,
                2 => LumaBayColorfulUi.HeaderPanel,
                3 => LumaBayColorfulUi.MediumPanel,
                4 => LumaBayColorfulUi.PrimaryButton,
                5 => LumaBayColorfulUi.SecondaryButton,
                6 => LumaBayColorfulUi.CompactButton,
                7 => LumaBayColorfulUi.GoalsPanel,
                8 => LumaBayColorfulUi.ProgressDecor,
                >= 9 and <= 14 => LumaBayColorfulUi.IconPlate,
                15 => LumaBayColorfulUi.BackButton,
                16 => LumaBayColorfulUi.CompactPanel,
                17 => LumaBayColorfulUi.ProgressTrack,
                18 => LumaBayColorfulUi.TaskPanel,
                19 => LumaBayColorfulUi.BottomNavigation,
                20 => LumaBayColorfulUi.BoosterTray,
                _ => null
            };
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
