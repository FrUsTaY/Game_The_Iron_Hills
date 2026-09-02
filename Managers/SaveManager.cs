using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using EpicBattle.Models;

namespace EpicBattle.Managers
{
    public static class SaveManager
    {
        private static readonly string SavesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");
        private static readonly string SettingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public static GameSettings Settings { get; private set; } = new GameSettings();
        public static GameState CurrentState { get; set; } = new GameState();

        static SaveManager()
        {
            if (!Directory.Exists(SavesDir))
                Directory.CreateDirectory(SavesDir);

            LoadSettings();
        }

        // --- Настройки ---
        public static void LoadSettings()
        {
            if (File.Exists(SettingsFile))
            {
                try
                {
                    string json = File.ReadAllText(SettingsFile);
                    var settings = JsonSerializer.Deserialize<GameSettings>(json);
                    if (settings != null) Settings = settings;
                }
                catch { }
            }
        }

        public static void SaveSettings()
        {
            try
            {
                string json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFile, json);
            }
            catch { }
        }

        // --- Сохранения Игры ---
        public static void SaveGame(string fileName)
        {
            CurrentState.SaveDate = DateTime.Now;
            CurrentState.SaveName = fileName;

            string path = Path.Combine(SavesDir, $"{fileName}.json");
            string json = JsonSerializer.Serialize(CurrentState, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        public static void LoadGame(string path)
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var state = JsonSerializer.Deserialize<GameState>(json);
                if (state != null)
                {
                    CurrentState = state;
                }
            }
        }

        public static void DeleteSave(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static List<FileInfo> GetAllSaves()
        {
            var dirInfo = new DirectoryInfo(SavesDir);
            return dirInfo.GetFiles("*.json").OrderByDescending(f => f.LastWriteTime).ToList();
        }
    }
}