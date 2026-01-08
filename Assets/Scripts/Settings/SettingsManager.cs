using System;
using UnityEngine;

namespace Settings
{
    public static class SettingsManager
    {
        private static string SettingsPath => Application.persistentDataPath + "/settings.json";
        public static SettingsData CurrentSettings { get; private set; } = new SettingsData();

        static SettingsManager()
        {
            LoadSettings();
        }

        public static void SaveSettings()
        {
            string json = JsonUtility.ToJson(CurrentSettings, true);
            System.IO.File.WriteAllText(SettingsPath, json);
            Debug.Log($"Settings saved to {SettingsPath}");
        }

        private static void LoadSettings()
        {
            if (System.IO.File.Exists(SettingsPath))
            {
                string json = System.IO.File.ReadAllText(SettingsPath);
                CurrentSettings = JsonUtility.FromJson<SettingsData>(json);
                Debug.Log($"Settings loaded from {SettingsPath}");
            }
            else
            {
                Debug.LogWarning($"Settings file not found at {SettingsPath}. Creating default settings.");
                LoadDefaultSettings();
            }
        }

        private static void LoadDefaultSettings()
        {
            CurrentSettings = new SettingsData
            {
                generateInfiteTerrain = false
            };
            SaveSettings();
            LoadSettings();
        }
}
    [Serializable]
    public class SettingsData
    {
        public Boolean generateInfiteTerrain = true;
    }
}