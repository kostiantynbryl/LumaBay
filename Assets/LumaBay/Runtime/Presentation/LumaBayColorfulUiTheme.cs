using UnityEngine;

namespace LumaBay
{
    [CreateAssetMenu(fileName = "LumaBayColorfulUiTheme", menuName = "Luma Bay/Colorful UI Theme")]
    public sealed class LumaBayColorfulUiTheme : ScriptableObject
    {
        [Header("Panels")]
        public Sprite panelLarge;
        public Sprite headerPanel;
        public Sprite mediumPanel;
        public Sprite compactPanel;
        public Sprite goalsPanel;
        public Sprite taskPanel;
        public Sprite bottomNavigation;
        public Sprite boosterTray;

        [Header("Buttons")]
        public Sprite primaryButton;
        public Sprite secondaryButton;
        public Sprite compactButton;
        public Sprite backButton;

        [Header("Progress")]
        public Sprite progressTrack;
        public Sprite progressFill;
        public Sprite progressDecor;

        [Header("Optional")]
        public Sprite iconPlate;

        public bool IsUsable =>
            panelLarge != null &&
            primaryButton != null &&
            secondaryButton != null &&
            compactButton != null;
    }

    public static class LumaBayColorfulUi
    {
        private const string ResourceName = "LumaBayColorfulUiTheme";
        private static LumaBayColorfulUiTheme cached;
        private static bool loadAttempted;

        public static LumaBayColorfulUiTheme Theme
        {
            get
            {
                if (!loadAttempted)
                {
                    cached = Resources.Load<LumaBayColorfulUiTheme>(ResourceName);
                    loadAttempted = true;
                }

                return cached;
            }
        }

        public static bool IsAvailable => Theme != null && Theme.IsUsable;

        public static Sprite PanelLarge => Theme != null ? Theme.panelLarge : null;
        public static Sprite HeaderPanel => Theme != null ? Theme.headerPanel : null;
        public static Sprite MediumPanel => Theme != null ? Theme.mediumPanel : null;
        public static Sprite CompactPanel => Theme != null ? Theme.compactPanel : null;
        public static Sprite GoalsPanel => Theme != null ? Theme.goalsPanel : null;
        public static Sprite TaskPanel => Theme != null ? Theme.taskPanel : null;
        public static Sprite BottomNavigation => Theme != null ? Theme.bottomNavigation : null;
        public static Sprite BoosterTray => Theme != null ? Theme.boosterTray : null;
        public static Sprite PrimaryButton => Theme != null ? Theme.primaryButton : null;
        public static Sprite SecondaryButton => Theme != null ? Theme.secondaryButton : null;
        public static Sprite CompactButton => Theme != null ? Theme.compactButton : null;
        public static Sprite BackButton => Theme != null ? Theme.backButton : null;
        public static Sprite ProgressTrack => Theme != null ? Theme.progressTrack : null;
        public static Sprite ProgressFill => Theme != null ? Theme.progressFill : null;
        public static Sprite ProgressDecor => Theme != null ? Theme.progressDecor : null;
        public static Sprite IconPlate => Theme != null ? Theme.iconPlate : null;

        public static void Reload()
        {
            cached = null;
            loadAttempted = false;
        }
    }
}
