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
        private const byte AlphaThreshold = 10;
        private const int MinimumArea = 500;
        private const int Padding = 2;

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

            if (!Directory.Exists(Root)) return;
            string[] files = Directory.GetFiles(Root, "*.png", SearchOption.TopDirectoryOnly);
            bool changed = false;
            foreach (string file in files)
            {
                string assetPath = file.Replace('\\', '/');
                changed |= Configure(assetPath);
            }

            if (!changed) return;
            AssetDatabase.Refresh();
            Debug.Log("Luma Bay premium Art Pack V2 imported and sliced.");
        }

        private static bool Configure(string path)
        {
            string id = Path.GetFileNameWithoutExtension(path);
            SpriteMetaData[] metadata = id == "backgrounds"
                ? BuildBackgroundSlices(path, id)
                : BuildAlphaSlices(path, id, id == "ui");
            if (metadata.Length == 0) return false;

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return false;

#pragma warning disable CS0618
            bool configured = importer.textureType == TextureImporterType.Sprite
                && importer.spriteImportMode == SpriteImportMode.Multiple
                && importer.spritesheet != null
                && importer.spritesheet.Length == metadata.Length;
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

#pragma warning disable CS0618
            importer.spritesheet = metadata;
#pragma warning restore CS0618
            importer.SaveAndReimport();
            return true;
        }

        private static SpriteMetaData[] BuildBackgroundSlices(string path, string id)
        {
            Texture2D texture = LoadTexture(path);
            if (texture == null) return Array.Empty<SpriteMetaData>();
            int baseWidth = texture.width / 4;
            SpriteMetaData[] result = new SpriteMetaData[4];
            for (int i = 0; i < result.Length; i++)
            {
                int x = i * baseWidth;
                int width = i == result.Length - 1 ? texture.width - x : baseWidth;
                result[i] = CreateMetadata($"{id}_{i:00}", new Rect(x, 0, width, texture.height), false);
            }
            UnityEngine.Object.DestroyImmediate(texture);
            return result;
        }

        private static SpriteMetaData[] BuildAlphaSlices(string path, string id, bool sliced)
        {
            Texture2D texture = LoadTexture(path);
            if (texture == null) return Array.Empty<SpriteMetaData>();
            Color32[] pixels = texture.GetPixels32();
            int width = texture.width;
            int height = texture.height;
            bool[] visited = new bool[pixels.Length];
            int[] queue = new int[pixels.Length];
            var rects = new List<Rect>();

            for (int start = 0; start < pixels.Length; start++)
            {
                if (visited[start] || pixels[start].a <= AlphaThreshold) continue;
                int head = 0;
                int tail = 0;
                queue[tail++] = start;
                visited[start] = true;
                int minX = width;
                int minY = height;
                int maxX = 0;
                int maxY = 0;
                int area = 0;

                while (head < tail)
                {
                    int index = queue[head++];
                    int x = index % width;
                    int y = index / width;
                    area++;
                    minX = Mathf.Min(minX, x);
                    minY = Mathf.Min(minY, y);
                    maxX = Mathf.Max(maxX, x);
                    maxY = Mathf.Max(maxY, y);

                    Visit(x - 1, y, width, height, pixels, visited, queue, ref tail);
                    Visit(x + 1, y, width, height, pixels, visited, queue, ref tail);
                    Visit(x, y - 1, width, height, pixels, visited, queue, ref tail);
                    Visit(x, y + 1, width, height, pixels, visited, queue, ref tail);
                }

                if (area < MinimumArea) continue;
                minX = Mathf.Max(0, minX - Padding);
                minY = Mathf.Max(0, minY - Padding);
                maxX = Mathf.Min(width - 1, maxX + Padding);
                maxY = Mathf.Min(height - 1, maxY + Padding);
                rects.Add(new Rect(minX, minY, maxX - minX + 1, maxY - minY + 1));
            }

            rects.Sort((left, right) =>
            {
                float rowDelta = right.yMax - left.yMax;
                if (Mathf.Abs(rowDelta) > 40f) return rowDelta > 0f ? 1 : -1;
                return left.x.CompareTo(right.x);
            });

            SpriteMetaData[] metadata = new SpriteMetaData[rects.Count];
            for (int i = 0; i < rects.Count; i++)
            {
                metadata[i] = CreateMetadata($"{id}_{i:00}", rects[i], sliced);
            }
            UnityEngine.Object.DestroyImmediate(texture);
            return metadata;
        }

        private static void Visit(int x, int y, int width, int height, Color32[] pixels, bool[] visited, int[] queue, ref int tail)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return;
            int index = y * width + x;
            if (visited[index] || pixels[index].a <= AlphaThreshold) return;
            visited[index] = true;
            queue[tail++] = index;
        }

        private static SpriteMetaData CreateMetadata(string name, Rect rect, bool sliced)
        {
            float border = sliced ? Mathf.Min(rect.width, rect.height) * 0.18f : 0f;
            return new SpriteMetaData
            {
                name = name,
                rect = rect,
                alignment = (int)SpriteAlignment.Center,
                pivot = new Vector2(0.5f, 0.5f),
                border = new Vector4(border, border, border, border)
            };
        }

        private static Texture2D LoadTexture(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (ImageConversion.LoadImage(texture, bytes, false)) return texture;
            UnityEngine.Object.DestroyImmediate(texture);
            return null;
        }
    }
}
