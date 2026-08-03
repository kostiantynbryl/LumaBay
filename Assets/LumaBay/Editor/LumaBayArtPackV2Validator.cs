using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LumaBay.Editor
{
    public static class LumaBayArtPackV2Validator
    {
        private const string Root = "Assets/LumaBay/Resources/ArtPackV2/Sheets";

        private static readonly Dictionary<string, int> RequiredSpriteCounts = new Dictionary<string, int>
        {
            { "tiles", 11 },
            { "obstacles", 10 },
            { "ui", 21 },
            { "map", 6 },
            { "lighthouse", 16 },
            { "backgrounds", 4 }
        };

        [MenuItem("Luma Bay/Validate Premium Art Pack V2", priority = 6)]
        public static void ValidateMenu()
        {
            ValidateOrThrow();
            Debug.Log("Luma Bay Art Pack V2 validation passed.");
        }

        public static void ValidateOrThrow()
        {
            var problems = new List<string>();
            foreach (KeyValuePair<string, int> requirement in RequiredSpriteCounts)
            {
                string sheet = requirement.Key;
                string pngPath = $"{Root}/{sheet}.png";
                if (!File.Exists(pngPath))
                {
                    problems.Add($"Missing file: {pngPath}");
                    continue;
                }

                Sprite[] sprites = Resources.LoadAll<Sprite>($"ArtPackV2/Sheets/{sheet}");
                int count = sprites?.Length ?? 0;
                if (count < requirement.Value)
                {
                    problems.Add($"{sheet}.png: expected at least {requirement.Value} sprites, imported {count}");
                    continue;
                }

                var names = new HashSet<string>(StringComparer.Ordinal);
                foreach (Sprite sprite in sprites)
                    if (sprite != null) names.Add(sprite.name);

                for (int index = 0; index < requirement.Value; index++)
                {
                    string expected = $"{sheet}_{index:00}";
                    if (!names.Contains(expected))
                        problems.Add($"{sheet}.png: missing named sprite {expected}; automatic slicing order is invalid");
                }
            }

            ValidateDistinctUiAssignments(problems);

            if (problems.Count == 0) return;
            throw new InvalidOperationException("Premium Art Pack V2 is incomplete or mis-sliced:\n- " +
                                                string.Join("\n- ", problems));
        }

        private static void ValidateDistinctUiAssignments(List<string> problems)
        {
            Sprite[] ui = Resources.LoadAll<Sprite>("ArtPackV2/Sheets/ui");
            if (ui == null || ui.Length < 21) return;

            var byName = new Dictionary<string, Sprite>(StringComparer.Ordinal);
            foreach (Sprite sprite in ui)
                if (sprite != null) byName[sprite.name] = sprite;

            string[] critical = { "ui_00", "ui_03", "ui_05", "ui_07", "ui_17", "ui_18", "ui_20" };
            foreach (string name in critical)
                if (!byName.ContainsKey(name)) problems.Add($"ui.png: critical sprite {name} is missing");

            if (byName.TryGetValue("ui_17", out Sprite track) &&
                byName.TryGetValue("ui_18", out Sprite fill) &&
                track.rect == fill.rect)
            {
                problems.Add("ui.png: progress track and fill resolve to the same slice");
            }
        }
    }
}
