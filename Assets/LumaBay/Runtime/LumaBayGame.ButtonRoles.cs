using UnityEngine;
using UnityEngine.UI;

namespace LumaBay
{
    public sealed partial class LumaBayGame
    {
        private int buttonRoleSignature = int.MinValue;

        private void ApplyButtonRoles()
        {
            if (screenRoot == null) return;

            int signature = screenRoot.GetInstanceID() * 397 ^ screenRoot.childCount;
            Transform modal = canvas != null ? canvas.transform.Find("ModalOverlay") : null;
            if (modal != null) signature ^= modal.GetInstanceID();
            if (signature == buttonRoleSignature) return;
            buttonRoleSignature = signature;

            AssignScreenButtonRoles();
            AssignModalButtonRoles(modal);
            StyleButtonsByRole();
            WireGameplayBackButtonV7();
        }

        private void AssignScreenButtonRoles()
        {
            if (FindInRoot("MainHero") != null)
            {
                AssignDirectButtons(FindInRoot("VerticalScreen"), ButtonRoleId.Play);
                Transform row = FindInRoot("MainSecondaryRow");
                AssignByIndex(row, 0, ButtonRoleId.Levels);
                AssignByIndex(row, 1, ButtonRoleId.Settings);
                return;
            }

            if (FindInRoot("LighthouseMetaCard") != null)
            {
                AssignHeaderBack("MapHeader");
                Transform root = FindInRoot("VerticalScreen");
                Button[] buttons = root != null ? root.GetComponentsInChildren<Button>(true) : System.Array.Empty<Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    Transform header = FindInRoot("MapHeader");
                    if (header != null && buttons[i].transform.IsChildOf(header)) continue;
                    SetRole(buttons[i], i == buttons.Length - 1 ? ButtonRoleId.Levels :
                        i == buttons.Length - 2 ? ButtonRoleId.StartLevel : ButtonRoleId.Restore);
                }
                return;
            }

            if (FindInRoot("SettingsHeader") != null)
            {
                AssignHeaderBack("SettingsHeader");
                Transform card = FindInRoot("SettingsCard");
                Button[] buttons = card != null ? card.GetComponentsInChildren<Button>(true) : System.Array.Empty<Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    ButtonRoleId role = i == buttons.Length - 1 ? ButtonRoleId.ResetProgress :
                        i == buttons.Length - 2 ? ButtonRoleId.Language : ButtonRoleId.SettingToggle;
                    SetRole(buttons[i], role);
                }
                return;
            }

            if (FindInRoot("LevelScroll") != null)
            {
                AssignHeaderBack("LevelsHeader");
                Transform content = FindInRoot("Content");
                if (content != null)
                {
                    foreach (Button button in content.GetComponentsInChildren<Button>(true))
                        SetRole(button, ButtonRoleId.LevelCard);
                }
                return;
            }

            if (FindInRoot("BoardFrame") != null || FindInRoot("Board") != null)
            {
                AssignHeaderBack("GameHeader");
                Transform tray = FindInRoot("BoosterTray") ?? FindInRoot("BoosterRow") ?? FindInRoot("BoostersRow");
                if (tray != null)
                {
                    foreach (Button button in tray.GetComponentsInChildren<Button>(true))
                        SetRole(button, ButtonRoleId.Booster);
                }
            }
        }

        private void AssignModalButtonRoles(Transform modal)
        {
            if (modal == null) return;
            Button[] buttons = modal.GetComponentsInChildren<Button>(true);
            if (buttons.Length > 0) SetRole(buttons[0], ButtonRoleId.ModalPrimary);
            if (buttons.Length > 1) SetRole(buttons[1], ButtonRoleId.ModalSecondary);
        }

        private void StyleButtonsByRole()
        {
            ButtonRole[] roles = canvas != null
                ? canvas.GetComponentsInChildren<ButtonRole>(true)
                : System.Array.Empty<ButtonRole>();

            foreach (ButtonRole marker in roles)
            {
                Button button = marker.GetComponent<Button>();
                if (button == null) continue;

                switch (marker.Id)
                {
                    case ButtonRoleId.Play:
                    case ButtonRoleId.StartLevel:
                    case ButtonRoleId.Restore:
                    case ButtonRoleId.ModalPrimary:
                        StyleButtonV6(button, true, marker.Id == ButtonRoleId.Play ? 88f : 78f);
                        break;
                    case ButtonRoleId.Levels:
                    case ButtonRoleId.Settings:
                        StyleButtonV6(button, false, 74f);
                        break;
                    case ButtonRoleId.Back:
                        StyleButtonV6(button, false, 58f);
                        break;
                    case ButtonRoleId.ResetProgress:
                        StyleButtonV6(button, false, 64f, new Color(0.62f, 0.20f, 0.24f, 1f));
                        break;
                    case ButtonRoleId.SettingToggle:
                    case ButtonRoleId.Language:
                        StyleButtonV6(button, false, 70f);
                        break;
                    case ButtonRoleId.ModalSecondary:
                        StyleButtonV6(button, false, 64f);
                        break;
                }
            }
        }

        private void WireGameplayBackButtonV7()
        {
            if (FindInRoot("BoardFrame") == null && FindInRoot("Board") == null) return;
            Transform header = FindInRoot("GameHeader");
            if (header == null) return;

            Button back = header.GetComponentInChildren<Button>(true);
            if (back == null) return;

            back.onClick.RemoveAllListeners();
            back.onClick.AddListener(() =>
            {
                audioSynth?.PlayClick();
                ShowExitLevelModalV7();
            });
        }

        private void ShowExitLevelModalV7()
        {
            string title = Localization.Language == "en" ? "Leave the level?" : "Выйти из уровня?";
            string body = Localization.Language == "en"
                ? "Current level progress will be lost."
                : "Текущий прогресс уровня будет потерян.";

            ShowModal(title, body,
                Localization.T("continue"), () => { },
                Localization.T("map"), ShowMap);
        }

        private void AssignHeaderBack(string headerName)
        {
            Transform header = FindInRoot(headerName);
            if (header == null) return;
            Button button = header.GetComponentInChildren<Button>(true);
            if (button != null) SetRole(button, ButtonRoleId.Back);
        }

        private static void AssignDirectButtons(Transform root, ButtonRoleId role)
        {
            if (root == null) return;
            for (int i = 0; i < root.childCount; i++)
            {
                Button button = root.GetChild(i).GetComponent<Button>();
                if (button != null) SetRole(button, role);
            }
        }

        private static void AssignByIndex(Transform parent, int index, ButtonRoleId role)
        {
            if (parent == null || index < 0 || index >= parent.childCount) return;
            Button button = parent.GetChild(index).GetComponent<Button>();
            if (button != null) SetRole(button, role);
        }

        private static void SetRole(Button button, ButtonRoleId role)
        {
            if (button == null) return;
            ButtonRole marker = button.GetComponent<ButtonRole>();
            if (marker == null) marker = button.gameObject.AddComponent<ButtonRole>();
            marker.Id = role;
            button.gameObject.name = "Button_" + role;
        }
    }
}
