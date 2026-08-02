using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame : MonoBehaviour
    {
        private const string Version = "0.1.2-alpha";

        private Canvas canvas;
        private RectTransform safeRoot;
        private RectTransform screenRoot;
        private Font font;
        private AudioSynth audioSynth;
        private PlayerSave save;

        private LevelDefinition currentLevel;
        private Match3Board board;
        private int movesRemaining;
        private int collectedTarget;
        private int clearedFog;
        private bool boardBusy;
        private bool levelFinished;
        private Vector2Int? selectedCell;

        private RectTransform boardGrid;
        private Text movesLabel;
        private Text collectGoalLabel;
        private Text fogGoalLabel;
        private Text walletLabel;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Input.multiTouchEnabled = false;
            save = SaveService.Load();
            Localization.Language = save.Language;
            BuildApplicationShell();
            ShowMainMenu();
        }

        private void Update()
        {
            RefreshGoalPresentationIfNeeded();
            RefreshBoardPresentationIfNeeded();
            RefreshArtOverridesIfNeeded();
            UpdateHintAnimation();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause && save != null) SaveService.Save(save);
        }

        private void OnApplicationQuit()
        {
            if (save != null) SaveService.Save(save);
        }

        public void OnPieceSwipe(int x, int y, Vector2Int direction)
        {
            if (boardBusy || levelFinished || board == null) return;
            NotifyPlayerInteraction();
            selectedCell = null;
            BeginAnimatedMove(new Vector2Int(x, y), new Vector2Int(x + direction.x, y + direction.y));
        }

        public void OnPieceTapped(int x, int y)
        {
            if (boardBusy || levelFinished || board == null) return;
            NotifyPlayerInteraction();
            Vector2Int tapped = new Vector2Int(x, y);
            if (!selectedCell.HasValue)
            {
                selectedCell = tapped;
                RefreshBoard();
                audioSynth.PlayClick();
                return;
            }

            Vector2Int previous = selectedCell.Value;
            if (previous == tapped)
            {
                selectedCell = null;
                RefreshBoard();
                return;
            }

            int distance = Mathf.Abs(previous.x - tapped.x) + Mathf.Abs(previous.y - tapped.y);
            if (distance == 1)
            {
                selectedCell = null;
                BeginAnimatedMove(previous, tapped);
            }
            else
            {
                selectedCell = tapped;
                RefreshBoard();
                audioSynth.PlayClick();
            }
        }

        private void BuildApplicationShell()
        {
            CreateRuntimeCamera();
            font = ResolveRuntimeFont();

            GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(transform, false);
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(720f, 1280f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            Sprite backgroundSprite = LumaBayArtPack.Background != null
                ? LumaBayArtPack.Background
                : CoastalBackdropArt.Create();
            Image background = CreateImage(canvasObject.transform, "Background", backgroundSprite, Color.white);
            Stretch(background.rectTransform);
            background.preserveAspect = false;
            background.raycastTarget = false;

            RectTransform ambientLayer = CreateRect(canvasObject.transform, "AmbientLayer");
            Stretch(ambientLayer);
            ambientLayer.gameObject.AddComponent<UiAmbientParticles>();

            GameObject safeObject = new GameObject("SafeArea", typeof(RectTransform), typeof(SafeAreaFitter));
            safeObject.transform.SetParent(canvasObject.transform, false);
            safeRoot = safeObject.GetComponent<RectTransform>();
            Stretch(safeRoot);

            screenRoot = CreateRect(safeRoot, "ScreenRoot");
            Stretch(screenRoot);

            audioSynth = gameObject.AddComponent<AudioSynth>();
            audioSynth.Enabled = save.SoundEnabled;
            audioSynth.MusicEnabled = save.MusicEnabled;

            if (EventSystem.current == null)
            {
                GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                eventSystem.transform.SetParent(transform, false);
            }
        }

        private static Font ResolveRuntimeFont()
        {
            string[] preferredFonts =
            {
                "Segoe UI",
                "Roboto",
                "Arial",
                "Noto Sans",
                "DejaVu Sans",
                "sans-serif"
            };

            try
            {
                string[] installedFonts = Font.GetOSInstalledFontNames();
                if (installedFonts != null)
                {
                    foreach (string preferred in preferredFonts)
                    {
                        foreach (string installed in installedFonts)
                        {
                            if (!string.Equals(preferred, installed, StringComparison.OrdinalIgnoreCase)) continue;
                            Font osFont = Font.CreateDynamicFontFromOSFont(installed, 32);
                            if (osFont != null)
                            {
                                Debug.Log($"Luma Bay UI font: {installed}");
                                return osFont;
                            }
                        }
                    }

                    if (installedFonts.Length > 0)
                    {
                        Font firstAvailable = Font.CreateDynamicFontFromOSFont(installedFonts[0], 32);
                        if (firstAvailable != null)
                        {
                            Debug.Log($"Luma Bay UI fallback font: {installedFonts[0]}");
                            return firstAvailable;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Unable to load an operating-system font: {exception.Message}");
            }

            try
            {
                Font legacyFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (legacyFont != null)
                {
                    Debug.Log("Luma Bay UI fallback font: LegacyRuntime.ttf");
                    return legacyFont;
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Unable to load Unity's legacy runtime font: {exception.Message}");
            }

            Debug.LogError("Luma Bay could not load a runtime UI font. Text will not be visible.");
            return null;
        }

        private void CreateRuntimeCamera()
        {
            Camera existingCamera = Camera.main;
            if (existingCamera != null)
            {
                if (existingCamera.GetComponent<AudioListener>() == null)
                    existingCamera.gameObject.AddComponent<AudioListener>();
                return;
            }

            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(transform, false);
            cameraObject.transform.localPosition = new Vector3(0f, 0f, -10f);

            Camera runtimeCamera = cameraObject.GetComponent<Camera>();
            runtimeCamera.clearFlags = CameraClearFlags.SolidColor;
            runtimeCamera.backgroundColor = NauticalTheme.Midnight;
            runtimeCamera.orthographic = true;
            runtimeCamera.orthographicSize = 5f;
            runtimeCamera.cullingMask = 0;
            runtimeCamera.nearClipPlane = 0.1f;
            runtimeCamera.farClipPlane = 100f;
        }
    }
}
