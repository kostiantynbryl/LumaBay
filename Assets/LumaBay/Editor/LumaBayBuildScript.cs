using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LumaBay.Editor
{
    public static class LumaBayBuildScript
    {
        private const string OutputPath = "Builds/Android/LumaBay-0.1.2-alpha.apk";
        private const string ReportPath = "Builds/Android/LumaBay-0.1.2-alpha-build-report.txt";
        private const string LegacyArtSentinel = "Assets/LumaBay/Resources/ArtPack/lighthouse/lighthouse_31.png";
        private const string V2ArtSentinel = "Assets/LumaBay/Resources/ArtPackV2/Sheets/tiles.png";
        private const string AudioSentinel = "Assets/LumaBay/Resources/Audio/music.wav";

        [MenuItem("Luma Bay/Build Android Alpha", priority = 20)]
        public static void BuildAndroid()
        {
            EnsureAndroidBuildTarget();
            LumaBayProjectBootstrap.EnsureProject();
            EnsureGeneratedAssets();

            string outputDirectory = Path.GetDirectoryName(OutputPath) ?? "Builds/Android";
            Directory.CreateDirectory(outputDirectory);
            DeletePreviousOutput();

            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveOpenScenes();

            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.connectProfiler = false;
            EditorUserBuildSettings.buildAppBundle = false;

            var options = new BuildPlayerOptions
            {
                scenes = new[] { LumaBayProjectBootstrap.ScenePath },
                locationPathName = OutputPath,
                targetGroup = BuildTargetGroup.Android,
                target = BuildTarget.Android,
                options = BuildOptions.CleanBuildCache
            };

            Debug.Log($"Starting Luma Bay Android build. Active target: {EditorUserBuildSettings.activeBuildTarget}; output: {OutputPath}");

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;
            WriteBuildReport(report);

            Debug.Log(
                $"Luma Bay 0.1.2 build result: {summary.result}; " +
                $"size: {summary.totalSize} bytes; errors: {summary.totalErrors}; " +
                $"warnings: {summary.totalWarnings}; duration: {summary.totalTime}.");

            if (summary.result == BuildResult.Succeeded && File.Exists(OutputPath))
            {
                Debug.Log($"APK created successfully: {Path.GetFullPath(OutputPath)}");
                return;
            }

            string details = CollectImportantMessages(report);
            string editorLog = Application.consoleLogPath;
            string resultDescription = summary.result == BuildResult.Cancelled
                ? "The Android build was cancelled before completion."
                : $"The Android build finished with result '{summary.result}'.";

            throw new BuildFailedException(
                $"{resultDescription}\n" +
                $"Unity build report: {Path.GetFullPath(ReportPath)}\n" +
                $"Unity editor log: {editorLog}\n" +
                $"Reported errors: {summary.totalErrors}; warnings: {summary.totalWarnings}.\n" +
                (string.IsNullOrWhiteSpace(details)
                    ? "Unity did not attach an error message to the BuildReport. Check the editor log path above, especially the lines immediately before this exception."
                    : $"Important build messages:\n{details}"));
        }

        private static void EnsureAndroidBuildTarget()
        {
            const BuildTarget target = BuildTarget.Android;
            const BuildTargetGroup group = BuildTargetGroup.Android;

            if (!BuildPipeline.IsBuildTargetSupported(group, target))
            {
                throw new BuildFailedException(
                    "Android Build Support is not installed for this Unity editor. " +
                    "Open Unity Hub → Installs → Unity 6000.3.18f1 → Add modules and install " +
                    "Android Build Support, Android SDK & NDK Tools, and OpenJDK.");
            }

            if (EditorUserBuildSettings.activeBuildTarget == target) return;

            if (Application.isBatchMode)
            {
                throw new BuildFailedException(
                    "Unity was started in batch mode with a non-Android active target. " +
                    "Restart the command with '-buildTarget Android'. The repository Build-Android-Alpha.bat already includes this argument.");
            }

            Debug.Log("Switching the active Unity build target to Android before building...");
            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(group, target))
            {
                throw new BuildFailedException(
                    "Unity could not switch the active build target to Android. " +
                    "Open File → Build Profiles, select Android, and press Switch Platform, then retry.");
            }
        }

        private static void DeletePreviousOutput()
        {
            if (File.Exists(OutputPath)) File.Delete(OutputPath);
            if (File.Exists(ReportPath)) File.Delete(ReportPath);
        }

        private static void WriteBuildReport(BuildReport report)
        {
            try
            {
                var text = new StringBuilder(8192);
                BuildSummary summary = report.summary;
                text.AppendLine("Luma Bay Android build report");
                text.AppendLine($"Result: {summary.result}");
                text.AppendLine($"Output: {summary.outputPath}");
                text.AppendLine($"Platform: {summary.platform}");
                text.AppendLine($"Started: {summary.buildStartedAt:O}");
                text.AppendLine($"Ended: {summary.buildEndedAt:O}");
                text.AppendLine($"Duration: {summary.totalTime}");
                text.AppendLine($"Size: {summary.totalSize} bytes");
                text.AppendLine($"Errors: {summary.totalErrors}");
                text.AppendLine($"Warnings: {summary.totalWarnings}");
                text.AppendLine();

                foreach (BuildStep step in report.steps)
                {
                    text.AppendLine($"## {step.name} ({step.duration})");
                    foreach (BuildStepMessage message in step.messages)
                        text.AppendLine($"[{message.type}] {message.content}");
                    text.AppendLine();
                }

                File.WriteAllText(ReportPath, text.ToString(), Encoding.UTF8);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Could not write the standalone build report: {exception.Message}");
            }
        }

        private static string CollectImportantMessages(BuildReport report)
        {
            var text = new StringBuilder(4096);
            int count = 0;

            foreach (BuildStep step in report.steps)
            {
                foreach (BuildStepMessage message in step.messages)
                {
                    bool important = message.type == LogType.Error ||
                                     message.type == LogType.Exception ||
                                     message.type == LogType.Assert ||
                                     message.type == LogType.Warning;
                    if (!important) continue;

                    text.AppendLine($"[{message.type}] {step.name}: {message.content}");
                    count++;
                    if (count >= 40)
                    {
                        text.AppendLine("Further messages were omitted; see the full build report.");
                        return text.ToString().TrimEnd();
                    }
                }
            }

            return text.ToString().TrimEnd();
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
