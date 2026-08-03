using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace LumaBay.Editor
{
    public static class LumaBayBuildScript
    {
        private const string OutputPath = "Builds/Android/LumaBay-0.1.2-alpha.apk";
        private const string LegacyArtSentinel = "Assets/LumaBay/Resources/ArtPack/lighthouse/lighthouse_31.png";
        private const string V2ArtSentinel = "Assets/LumaBay/Resources/ArtPackV2/Sheets/tiles.png";
        private const string AudioSentinel = "Assets/LumaBay/Resources/Audio/music.wav";

        [MenuItem("Luma Bay/Build Android Alpha", priority = 20)]
        public static void BuildAndroid()
        {
            LumaBayProjectBootstrap.EnsureProject();
            EnsureGeneratedAssets();
            Directory.CreateDirectory(Path.GetDirectoryName(OutputPath) ?? "Builds/Android");
            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.connectProfiler = false;
            EditorUserBuildSettings.buildAppBundle = false;

            var options = new BuildPlayerOptions
            {
                scenes = new[] { LumaBayProjectBootstrap.ScenePath },
                locationPathName = OutputPath,
                target = BuildTarget.Android,
                options = BuildOptions.CleanBuildCache
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;
            Debug.Log($"Luma Bay 0.1.2 build result: {summary.result}; size: {summary.totalSize} bytes; errors: {summary.totalErrors}");

            if (summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException($"Android build failed with {summary.totalErrors} errors.");
        }

        private static void EnsureGeneratedAssets()
        {
            if (!File.Exists(V2ArtSentinel))
                throw new FileNotFoundException(
                    "Premium Art Pack V2 is not installed. Copy LumaBay_ArtPackV2/Assets into the project before building.",
                    V2ArtSentinel);

            if (!File.Exists(LegacyArtSentinel)) LumaBayArtPackGenerator.GenerateAll();
            if (!File.Exists(AudioSentinel)) LumaBayAudioPackGenerator.GenerateAll();

            LumaBayArtPackV2Importer.EnsureImported();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            LumaBayArtPackV2Validator.ValidateOrThrow();

            if (!File.Exists(LegacyArtSentinel) || !File.Exists(AudioSentinel))
                throw new FileNotFoundException("Luma Bay fallback art/audio assets could not be created.");
        }
    }
}
