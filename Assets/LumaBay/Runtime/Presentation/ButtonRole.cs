using UnityEngine;

namespace LumaBay
{
    public enum ButtonRoleId
    {
        Generic = 0,
        PrimaryAction = 1,
        SecondaryAction = 2,
        Back = 3,
        Play = 4,
        Levels = 5,
        Settings = 6,
        Restore = 7,
        StartLevel = 8,
        SettingToggle = 9,
        Language = 10,
        ResetProgress = 11,
        LevelCard = 12,
        Booster = 13,
        ModalPrimary = 14,
        ModalSecondary = 15
    }

    [DisallowMultipleComponent]
    public sealed class ButtonRole : MonoBehaviour
    {
        [SerializeField] private ButtonRoleId id = ButtonRoleId.Generic;

        public ButtonRoleId Id
        {
            get => id;
            set => id = value;
        }
    }
}
