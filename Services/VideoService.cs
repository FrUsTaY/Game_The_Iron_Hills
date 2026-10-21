using System;
using System.Collections.Generic;
using System.IO;

namespace EpicBattle.Services
{
    /// <summary>
    /// Сервис поиска, проверки наличия и управления путями к видеофайлам игры.
    /// </summary>
    public static class VideoService
    {
        private static readonly string[] SearchDirectories = new[]
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Videos"),
            Path.Combine(Directory.GetCurrentDirectory(), "Videos"),
            AppDomain.CurrentDomain.BaseDirectory,
            Directory.GetCurrentDirectory(),
            @"C:\Users\admin\Desktop\The Iron Hills\Videos",
            @"C:\Users\admin\Desktop\Videos"
        };

        /// <summary>
        /// Разрешает абсолютный путь к видеофайлу.
        /// Поддерживает имена с расширением или без (автоматически пробует .mp4, .mkv, .avi, .wmv).
        /// </summary>
        public static string? ResolveVideoPath(string videoFileName)
        {
            if (string.IsNullOrWhiteSpace(videoFileName))
                return null;

            string cleanName = videoFileName.Trim();

            // Если путь уже абсолютный и файл существует
            if (Path.IsPathRooted(cleanName) && File.Exists(cleanName))
                return cleanName;

            var extensionsToTry = new List<string>();
            if (Path.HasExtension(cleanName))
            {
                extensionsToTry.Add(""); // как есть
            }
            else
            {
                extensionsToTry.Add(".mp4");
                extensionsToTry.Add(".mkv");
                extensionsToTry.Add(".avi");
                extensionsToTry.Add(".wmv");
            }

            foreach (var baseDir in SearchDirectories)
            {
                if (!Directory.Exists(baseDir))
                    continue;

                foreach (var ext in extensionsToTry)
                {
                    string candidate = Path.Combine(baseDir, cleanName + ext);
                    if (File.Exists(candidate))
                    {
                        return Path.GetFullPath(candidate);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Проверяет существование видеофайла.
        /// </summary>
        public static bool HasVideo(string videoFileName)
        {
            return ResolveVideoPath(videoFileName) != null;
        }

        /// <summary>
        /// Проверяет, доступны ли вступительные видео (логотип или сюжетный пролог).
        /// </summary>
        public static bool HasIntroVideos()
        {
            return HasVideo("intro_logo.mp4") || HasVideo("intro_story.mp4");
        }
    }
}
