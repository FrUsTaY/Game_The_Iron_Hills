using System;
using System.Collections.Generic;
using System.Windows;
using EpicBattle.Models;

namespace EpicBattle.Managers
{
    public static class DisplayManager
    {
        public static readonly List<string> DisplayModes = new()
        {
            "В окне",
            "В окне без рамки",
            "Во весь экран"
        };

        public static readonly List<string> StandardResolutions = new()
        {
            "1280x720",
            "1366x768",
            "1600x900",
            "1920x1080",
            "2560x1440"
        };

        public static void ApplySettings(Window? window, GameSettings settings)
        {
            if (window == null || settings == null) return;

            string mode = settings.DisplayMode ?? "Windowed";
            string res = settings.Resolution ?? "1280x720";

            // Парсим разрешение
            int width = 1280;
            int height = 720;
            var parts = res.Split('x');
            if (parts.Length == 2 && int.TryParse(parts[0], out int w) && int.TryParse(parts[1], out int h))
            {
                width = w;
                height = h;
            }

            double screenW = SystemParameters.PrimaryScreenWidth;
            double screenH = SystemParameters.PrimaryScreenHeight;

            switch (mode)
            {
                case "Fullscreen":
                case "Во весь экран":
                    window.WindowState = WindowState.Normal;
                    window.WindowStyle = WindowStyle.None;
                    window.ResizeMode = ResizeMode.NoResize;
                    window.WindowState = WindowState.Maximized;
                    window.Topmost = false;
                    break;

                case "Borderless":
                case "В окне без рамки":
                    window.WindowState = WindowState.Normal;
                    window.WindowStyle = WindowStyle.None;
                    window.ResizeMode = ResizeMode.NoResize;
                    window.Left = 0;
                    window.Top = 0;
                    window.Width = screenW;
                    window.Height = screenH;
                    break;

                case "Windowed":
                case "В окне":
                default:
                    window.WindowState = WindowState.Normal;
                    window.WindowStyle = WindowStyle.SingleBorderWindow;
                    window.ResizeMode = ResizeMode.CanResize;

                    // Ограничиваем размер окна размером экрана
                    width = Math.Min(width, (int)screenW);
                    height = Math.Min(height, (int)screenH);

                    window.Width = width;
                    window.Height = height;

                    // Центрируем окно
                    window.Left = Math.Max(0, (screenW - width) / 2);
                    window.Top = Math.Max(0, (screenH - height) / 2);
                    break;
            }
        }
    }
}
