using System;
using System.Windows;

namespace EpicBattle
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show("Произошла ошибка: " + args.Exception.Message + "\n\n" + args.Exception.StackTrace, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };

            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                MessageBox.Show("Критическая ошибка: " + args.ExceptionObject, "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            };
        }
    }
}
