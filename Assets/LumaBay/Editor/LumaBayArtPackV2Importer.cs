using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LumaBay.Editor
{
    [InitializeOnLoad]
    public static class LumaBayArtPackV2Importer
    {
        private const string Root = "Assets/LumaBay/Resources/ArtPackV2/Sheets";
        private const string ManifestPath = "Assets/LumaBay/Resources/ArtPackV2/artpack-v2.json";

        [Serializable]
        private sealed class Manifest
        {
            public Sheet[] sheets;
        }

        [Serializable]
        private sealed class Sheet
        {
            public string id;
            public Slice[] slices;
        }

        [Serializable]
        private sealed class Slice
        {
            public string name;
            public float x;
            public float y;
            public float width;
            public float height;
            public bool sliced;
        }

        static LumaBayArtPackV2Importer()
        {
            EditorApplication.delayCall += EnsureImported;
        }

        [MenuItem("Luma Bay/Import Premium Art Pack V2")]
        public static void EnsureImported()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += EnsureImported;
                return;
            }

            if (!File.Exists(ManifestPath)) return;
            Manifest manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(ManifestPath));
            if (manifest?.sheets == null) return;

            bool changed = false;
            foreach (Sheet sheet in manifest.sheets)
            {
                if (sheet == null || string.IsNullOrWhiteSpace(sheet.id) || sheet.slices == null) continue;
                string path = $"{Root}/{sheet.id}.png";
                if (!File.Exists(path)) continue;
                changed |= Configure(path, sheet.slices);
            }

            if (!changed) return;
            AssetDatabase.Refresh();
            Debug.Log("Luma Bay premium Art Pack V2 imported and sliced.");
        }

        private static bool Configure(string path, Slice[] slices)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return false;

#pragma warning disable CS0618
            bool configured = importer.textureType == TextureImporterType.Sprite
                && importer.spriteImportMode == SpriteImportMode.Multiple
                && importer.spritesheet != null
                && importer.spritesheet.Length == slices.Length;
#pragma warning restore CS0618
            if (configured) return false;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.maxTextureSize = 2048;

            SpriteMetaData[] metadata = new SpriteMetaData[slices.Length];
            for (int i = 0; i < slices.Length; i++)
            {
                Slice slice = slices[i];
                float border = slice.sliced ? Mathf.Min(slice.width, slice.height) * 0.18f : 0f;
                metadata[i] = new SpriteMetaData
                {
                    name = slice.name,
                    rect = new Rect(slice.x, slice.y, slice.width, slice.height),
                    alignment = (int)SpriteAlignment.Center,
                    pivot = new Vector2(0.5f, 0.5f),
                    border = new Vector4(border, border, border, border)
                };
            }

#pragma warning disable CS0618
            importer.spritesheet = metadata;
#pragma warning restore CS0618
            importer.SaveAndReimport();
            return true;
        }
    }
}
