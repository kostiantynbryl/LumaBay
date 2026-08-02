using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace LumaBay.Editor
{
    public static class LumaBayBuildScript
    {
        private const string OutputPath = "Builds/Android/LumaBay-0.1.1-alpha.apk";

        [MenuItem("Luma Bay/Build Android Alpha", priority = 20)]
        public static void BuildAndroid()
        {
            LumaBayProjectBootstrap.EnsureProject();
            Directory.CreateDirectory(Path.GetDirectoryName(OutputPath) ?? "Builds/Android");
            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.connectProfiler = false;
            EditorUserBuildSettings.buildAppBundle = false;

            var options = new BuildPlayerOptions
            {
                scenes = new[] { LumaBayProjectBootstrap.ScenePath },
                locationPathName = OutputPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;
            Debug.Log($"Luma Bay build result: {summary.result}; size: {summary.totalSize} bytes; errors: {summary.totalErrors}");

            if (summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException($"Android build failed with {summary.totalErrors} errors.");
            }
        }
    }
}
