using System.Windows.Controls;
using System.Windows;

namespace EpicBattle.Managers
{
    public static class NavigationManager
    {
        private static ContentControl? _mainContainer;
        private static Window? _mainWindow;

        public static void Initialize(ContentControl mainContainer, Window mainWindow)
        {
            _mainContainer = mainContainer;
            _mainWindow = mainWindow;
        }

        public static void NavigateTo(UserControl view)
        {
            if (_mainContainer != null)
            {
                _mainContainer.Content = view;
            }
        }

        public static void ExitGame()
        {
            _mainWindow?.Close();
        }
    }
}