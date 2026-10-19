using System;
using System.IO;
using System.Windows;

namespace EpicBattle
{
    public partial class App : Application
    {
        private static readonly string LogFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EpicBattle", "error.log");

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += (s, args) =>
            {
                LogError("DispatcherUnhandledException", args.Exception);
                MessageBox.Show("Произошла ошибка: " + args.Exception.Message + "\n\n" + args.Exception.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };

            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                LogError("UnhandledException", ex);
                MessageBox.Show("Критическая ошибка: " + args.ExceptionObject, "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            };
        }

        /// <summary>
        /// Запись стектрейса в %APPDATA%\EpicBattle\error.log для диагностики сбоев у игроков.
        /// </summary>
        private static void LogError(string source, Exception ex)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogFile)!);
                File.AppendAllText(LogFile,
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {source}: {ex}{Environment.NewLine}");
            }
            catch
            {
                // логирование не должно ронять игру
            }
        }
    }
}
