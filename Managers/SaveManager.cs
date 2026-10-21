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
        // Сохранения и настройки хранятся в %APPDATA%\EpicBattle (или ~/.local/share на Unix),
        // а не в папке установки — иначе сохранения теряются при обновлении/запуске из bin\Debug
        // и невозможна запись при установке в Program Files.
        private static readonly string AppDataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EpicBattle");
        private static readonly string SavesDir = Path.Combine(AppDataDir, "Saves");
        private static readonly string SettingsFile = Path.Combine(AppDataDir, "settings.json");

        public static GameSettings Settings { get; private set; } = new GameSettings();
        public static GameState CurrentState { get; set; } = new GameState();

        private static readonly JsonSerializerOptions IndentedOptions =
            new JsonSerializerOptions { WriteIndented = true };

        static SaveManager()
        {
            try
            {
                MigrateLegacySaves();
                if (!Directory.Exists(SavesDir))
                    Directory.CreateDirectory(SavesDir);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveManager init: {ex.Message}");
            }

            LoadSettings();
        }

        /// <summary>
        /// Однократный перенос сохранений/настроек из старой папки (bin\...\Saves) в %APPDATA%.
        /// </summary>
        private static void MigrateLegacySaves()
        {
            string legacyDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");
            string legacySettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

            if (Directory.Exists(legacyDir))
            {
                foreach (var file in Directory.GetFiles(legacyDir, "*.json"))
                {
                    string target = Path.Combine(SavesDir, Path.GetFileName(file));
                    if (!File.Exists(target))
                        File.Copy(file, target);
                }
            }

            if (File.Exists(legacySettings) && !File.Exists(SettingsFile))
            {
                Directory.CreateDirectory(AppDataDir);
                File.Copy(legacySettings, SettingsFile);
            }
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
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"LoadSettings: {ex.Message}");
                }
            }
        }

        public static void SaveSettings()
        {
            try
            {
                Directory.CreateDirectory(AppDataDir);
                string json = JsonSerializer.Serialize(Settings, IndentedOptions);
                File.WriteAllText(SettingsFile, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveSettings: {ex.Message}");
            }
        }

        // --- Сохранения Игры ---

        /// <summary>
        /// Защита от пути за пределами папки сохранений (path traversal через имя файла).
        /// </summary>
        private static string SanitizeFileName(string fileName)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                fileName = fileName.Replace(c, '_');
            fileName = fileName.Trim().Trim('.', ' ');
            return string.IsNullOrEmpty(fileName) ? "save" : fileName;
        }

        public static bool SaveGame(string fileName)
        {
            try
            {
                CurrentState.SaveDate = DateTime.Now;
                CurrentState.SaveName = fileName;

                Directory.CreateDirectory(SavesDir);
                string path = Path.Combine(SavesDir, SanitizeFileName(fileName) + ".json");
                string json = JsonSerializer.Serialize(CurrentState, IndentedOptions);
                File.WriteAllText(path, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveGame: {ex.Message}");
                return false;
            }
        }

        public static bool LoadGame(string path)
        {
            try
            {
                if (!File.Exists(path)) return false;

                string json = File.ReadAllText(path);
                var state = JsonSerializer.Deserialize<GameState>(json);
                if (state == null) return false;

                // Миграция старых сохранений: пересчёт производных статов и восстановление карты
                state.RecalculateDerivedStats();
                if (state.IsMapMode && state.ActiveMap == null)
                    state.StartNewMap(state.CurrentAct);

                CurrentState = state;
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadGame: {ex.Message}");
                return false;
            }
        }

        public static void DeleteSave(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteSave: {ex.Message}");
            }
        }

        public static List<FileInfo> GetAllSaves()
        {
            try
            {
                var dirInfo = new DirectoryInfo(SavesDir);
                return dirInfo.GetFiles("*.json").OrderByDescending(f => f.LastWriteTime).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllSaves: {ex.Message}");
                return new List<FileInfo>();
            }
        }
    }
}