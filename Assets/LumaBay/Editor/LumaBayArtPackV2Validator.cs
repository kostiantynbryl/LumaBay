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

        private static readonly Dictionary<string, int> MinimumSprites = new Dictionary<string, int>
        {
            { "tiles", 11 },
            { "obstacles", 10 },
            { "ui", 8 },
            { "map", 6 },
            { "lighthouse", 8 },
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
            foreach (KeyValuePair<string, int> requirement in MinimumSprites)
            {
                string pngPath = $"{Root}/{requirement.Key}.png";
                if (!File.Exists(pngPath))
                {
                    problems.Add($"Missing file: {pngPath}");
                    continue;
                }

                Sprite[] sprites = Resources.LoadAll<Sprite>($"ArtPackV2/Sheets/{requirement.Key}");
                int count = sprites?.Length ?? 0;
                if (count < requirement.Value)
                    problems.Add($"{requirement.Key}.png: expected at least {requirement.Value} sprites, imported {count}");
            }

            if (problems.Count == 0) return;
            throw new InvalidOperationException("Premium Art Pack V2 is incomplete:\n- " + string.Join("\n- ", problems));
        }
    }
}
