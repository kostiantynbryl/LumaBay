using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LumaBay.Editor
{
    [InitializeOnLoad]
    public static class LumaBayProjectBootstrap
    {
        public const string ScenePath = "Assets/LumaBay/Scenes/LumaBayMain.unity";
        private const string SetupVersionKey = "LumaBay.ProjectSetup.0.1.1";

        static LumaBayProjectBootstrap()
        {
            EditorApplication.delayCall += EnsureProjectOnce;
        }

        [MenuItem("Luma Bay/Setup Project", priority = 1)]
        public static void EnsureProject()
        {
            Directory.CreateDirectory("Assets/LumaBay/Scenes");
            EnsureScene();
            ConfigurePlayer();
            LumaBayBrandingGenerator.EnsureBranding();
            ConfigureBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorPrefs.SetBool(SetupVersionKey, true);
            Debug.Log("Luma Bay project setup completed.");
        }

        private static void EnsureProjectOnce()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            if (!EditorPrefs.GetBool(SetupVersionKey, false) || !File.Exists(ScenePath))
            {
                EnsureProject();
            }
        }

        private static void EnsureScene()
        {
            if (File.Exists(ScenePath)) return;

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject root = new GameObject("LumaBayGame");
            root.AddComponent<LumaBayGame>();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
        }

        private static void ConfigurePlayer()
        {
            PlayerSettings.companyName = "Norvexa Games";
            PlayerSettings.productName = "Luma Bay: Match & Restore";
            PlayerSettings.bundleVersion = "0.1.1-alpha";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.runInBackground = false;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.norvexa.lumabay");
            PlayerSettings.Android.bundleVersionCode = 2;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            EditorUserBuildSettings.buildAppBundle = false;
        }

        private static void ConfigureBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
        }
    }
}
