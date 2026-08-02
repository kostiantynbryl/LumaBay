using System;
using System.Collections.Generic;
using UnityEngine;

namespace LumaBay
{
    [Serializable]
    public sealed class PlayerSave
    {
        public int SaveVersion = 3;
        public int UnlockedLevel = 1;
        public int Coins = 300;
        public int TotalStars;
        public int AvailableStars;

        // Kept for backwards compatibility with 0.1.0/0.1.1 saves.
        public int RestorationStep;
        public int LighthouseTaskIndex;

        public bool SoundEnabled = true;
        public bool MusicEnabled = true;
        public bool VibrationEnabled = true;
        public bool TutorialSwipeSeen;
        public bool TutorialBoosterSeen;
        public string Language = "ru";
        public List<int> StarsByLevel = new List<int>();

        public bool LighthouseComplete => LighthouseTaskCatalog.IsComplete(LighthouseTaskIndex);
        public float LighthouseProgress01 => LighthouseTaskCatalog.Progress01(LighthouseTaskIndex);
        public int LighthouseVisualState => LighthouseTaskCatalog.VisualState(LighthouseTaskIndex);

        public void EnsureLevelCapacity(int count)
        {
            while (StarsByLevel.Count < count) StarsByLevel.Add(0);
            if (StarsByLevel.Count > count) StarsByLevel.RemoveRange(count, StarsByLevel.Count - count);
        }
    }

    public static class SaveService
    {
        private const string Key = "lumabay_save_v1";
        private const string BackupKey = "lumabay_save_backup_v3";
        private const int CurrentVersion = 3;

        public static PlayerSave Load()
        {
            PlayerSave save = TryRead(Key) ?? TryRead(BackupKey) ?? CreateDefault();
            Migrate(save);
            Normalize(save);
            return save;
        }

        public static void Save(PlayerSave save)
        {
            if (save == null) return;
            save.SaveVersion = CurrentVersion;
            Normalize(save);

            string json = JsonUtility.ToJson(save);
            if (PlayerPrefs.HasKey(Key))
            {
                PlayerPrefs.SetString(BackupKey, PlayerPrefs.GetString(Key));
            }
            PlayerPrefs.SetString(Key, json);
            PlayerPrefs.Save();
        }

        public static PlayerSave Reset()
        {
            PlayerPrefs.DeleteKey(Key);
            PlayerPrefs.DeleteKey(BackupKey);
            PlayerPrefs.Save();
            return CreateDefault();
        }

        private static PlayerSave TryRead(string key)
        {
            if (!PlayerPrefs.HasKey(key)) return null;
            try
            {
                string json = PlayerPrefs.GetString(key);
                return string.IsNullOrWhiteSpace(json) ? null : JsonUtility.FromJson<PlayerSave>(json);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Luma Bay save '{key}' could not be read: {exception.Message}");
                return null;
            }
        }

        private static void Normalize(PlayerSave save)
        {
            save.EnsureLevelCapacity(LevelCatalog.Count);
            save.UnlockedLevel = Mathf.Clamp(save.UnlockedLevel, 1, LevelCatalog.Count);
            save.Coins = Mathf.Max(0, save.Coins);
            save.TotalStars = Mathf.Max(0, save.TotalStars);
            save.AvailableStars = Mathf.Clamp(save.AvailableStars, 0, save.TotalStars);
            save.LighthouseTaskIndex = Mathf.Clamp(save.LighthouseTaskIndex, 0, LighthouseTaskCatalog.Count);
            save.RestorationStep = Mathf.Clamp(Mathf.FloorToInt(save.LighthouseTaskIndex / 8f), 0, 6);
            if (string.IsNullOrEmpty(save.Language)) save.Language = "ru";
        }

        private static void Migrate(PlayerSave save)
        {
            if (save.SaveVersion < 1)
            {
                save.SaveVersion = 1;
            }

            if (save.SaveVersion < 2)
            {
                save.TutorialSwipeSeen = save.UnlockedLevel > 1;
                save.TutorialBoosterSeen = save.UnlockedLevel > 3;
                save.SaveVersion = 2;
            }

            if (save.SaveVersion < 3)
            {
                // Old builds had only six restoration steps. Map them to the
                // first 32 tasks so existing players keep visible progress.
                save.LighthouseTaskIndex = Mathf.Clamp(save.RestorationStep * 5, 0, 32);
                save.MusicEnabled = save.SoundEnabled;
                save.SaveVersion = 3;
            }
        }

        private static PlayerSave CreateDefault()
        {
            var save = new PlayerSave
            {
                SaveVersion = CurrentVersion,
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
