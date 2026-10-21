using System;
using System.Windows;
using System.Windows.Input;
using EpicBattle.Services;

namespace EpicBattle.Views
{
    public partial class RpgMessageBoxWindow : Window
    {
        public RpgMessageBoxWindow(string message, string title = "Весть", MessageBoxImage icon = MessageBoxImage.None)
        {
            InitializeComponent();
            MessageText.Text = message ?? string.Empty;
            TitleText.Text = (title ?? "Весть").ToUpperInvariant();

            // Динамический выбор фэнтезийной иконки по контексту или типу
            HeaderIcon.Text = ResolveIcon(title, icon);

            try
            {
                AudioEngine.Play("click_alt");
            }
            catch
            {
                // Игнорируем ошибки звука
            }
        }

        private static string ResolveIcon(string title, MessageBoxImage icon)
        {
            if (icon == MessageBoxImage.Error) return "⚠️";
            if (icon == MessageBoxImage.Warning) return "⚔️";
            if (icon == MessageBoxImage.Information)
            {
                string t = (title ?? "").ToLowerInvariant();
                if (t.Contains("добыч") || t.Contains("лут") || t.Contains("золот") || t.Contains("наград")) return "🎁";
                if (t.Contains("сохранен") || t.Contains("успех")) return "💾";
                if (t.Contains("клятв") || t.Contains("сюжет") || t.Contains("вест")) return "📜";
                return "✨";
            }

            string tl = (title ?? "").ToLowerInvariant();
            if (tl.Contains("добыч") || tl.Contains("лут") || tl.Contains("золот") || tl.Contains("наград")) return "🎁";
            if (tl.Contains("сохранен") || tl.Contains("успех")) return "💾";
            if (tl.Contains("ошибк") || tl.Contains("урон")) return "⚠️";
            return "📜";
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space || e.Key == Key.Escape)
            {
                DialogResult = (e.Key != Key.Escape);
                Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                try
                {
                    DragMove();
                }
                catch
                {
                }
            }
        }
    }

    /// <summary>
    /// Статический фасад для вызова стилизованных фэнтезийных диалогов.
    /// Полностью заменяет MessageBox.Show без изменения сигнатур в коде игры.
    /// </summary>
    public static class RpgMessageBox
    {
        public static MessageBoxResult Show(string message, string title = "Весть", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None)
        {
            if (Application.Current != null && Application.Current.Dispatcher != null && !Application.Current.Dispatcher.CheckAccess())
            {
                return Application.Current.Dispatcher.Invoke(() => Show(message, title, button, icon));
            }

            var dialog = new RpgMessageBoxWindow(message, title, icon);
            if (Application.Current?.MainWindow != null && Application.Current.MainWindow.IsVisible)
            {
                dialog.Owner = Application.Current.MainWindow;
            }

            dialog.ShowDialog();
            return MessageBoxResult.OK;
        }
    }
}
