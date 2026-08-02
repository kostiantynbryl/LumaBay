using System;
using System.Collections.Generic;
using UnityEngine;

namespace LumaBay
{
    [Serializable]
    public sealed class PlayerSave
    {
        public int UnlockedLevel = 1;
        public int Coins = 300;
        public int TotalStars;
        public int AvailableStars;
        public int RestorationStep;
        public bool SoundEnabled = true;
        public bool VibrationEnabled = true;
        public string Language = "ru";
        public List<int> StarsByLevel = new List<int>();

        public void EnsureLevelCapacity(int count)
        {
            while (StarsByLevel.Count < count) StarsByLevel.Add(0);
        }
    }

    public static class SaveService
    {
        private const string Key = "lumabay_save_v1";

        public static PlayerSave Load()
        {
            PlayerSave save = null;
            if (PlayerPrefs.HasKey(Key))
            {
                try
                {
                    save = JsonUtility.FromJson<PlayerSave>(PlayerPrefs.GetString(Key));
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"Luma Bay save could not be read: {exception.Message}");
                }
            }

            save ??= CreateDefault();
            save.EnsureLevelCapacity(LevelCatalog.Count);
            save.UnlockedLevel = Mathf.Clamp(save.UnlockedLevel, 1, LevelCatalog.Count);
            return save;
        }

        public static void Save(PlayerSave save)
        {
            save.EnsureLevelCapacity(LevelCatalog.Count);
            PlayerPrefs.SetString(Key, JsonUtility.ToJson(save));
            PlayerPrefs.Save();
        }

        public static PlayerSave Reset()
        {
            PlayerPrefs.DeleteKey(Key);
            PlayerPrefs.Save();
            return CreateDefault();
        }

        private static PlayerSave CreateDefault()
        {
            var save = new PlayerSave
            {
                Language = Application.systemLanguage == SystemLanguage.Russian ||
                           Application.systemLanguage == SystemLanguage.Ukrainian
                    ? "ru"
                    : "en"
            };
            save.EnsureLevelCapacity(LevelCatalog.Count);
            return save;
        }
    }
}
