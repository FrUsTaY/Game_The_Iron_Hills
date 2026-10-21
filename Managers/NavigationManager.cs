using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using EpicBattle.Services;
using EpicBattle.Views;

namespace EpicBattle.Managers
{
    public static class NavigationManager
    {
        private static ContentControl? _mainContainer;
        private static Window? _mainWindow;
        private static readonly Stack<UserControl> _history = new Stack<UserControl>();

        public static void Initialize(ContentControl mainContainer, Window mainWindow)
        {
            _mainContainer = mainContainer;
            _mainWindow = mainWindow;
            _history.Clear();
        }

        public static void NavigateTo(UserControl view, bool rememberInHistory = true)
        {
            if (_mainContainer != null)
            {
                if (rememberInHistory && _mainContainer.Content is UserControl currentView && currentView != view)
                {
                    _history.Push(currentView);
                }
                _mainContainer.Content = view;
                OnViewChanged(view);
            }
        }

        public static bool CanGoBack => _history.Count > 0;

        public static void GoBack()
        {
            if (_mainContainer != null && _history.Count > 0)
            {
                var previousView = _history.Pop();
                _mainContainer.Content = previousView;
                OnViewChanged(previousView);
            }
        }

        public static void ClearHistory()
        {
            _history.Clear();
        }

        public static void ExitGame()
        {
            _mainWindow?.Close();
        }

        private static void OnViewChanged(UserControl view)
        {
            switch (view)
            {
                case MainMenu:
                case ChapterSelectView:
                    AudioEngine.PlayMusic("Menu");
                    break;
                case CharacterCreationView:
                    AudioEngine.PlayMusic("CharCreation");
                    break;
                case MapView:
                    AudioEngine.PlayMusic("Map");
                    break;
                case MerchantView:
                    AudioEngine.PlayMusic("Merchant");
                    break;
                case TavernView:
                    AudioEngine.PlayMusic("Tavern");
                    break;
                case CampfireView:
                    AudioEngine.PlayMusic("Campfire");
                    break;
                case BlacksmithView:
                    AudioEngine.PlayMusic("Blacksmith");
                    break;
                case SkillTreeView:
                    AudioEngine.PlayMusic("SkillTree");
                    break;
            }
        }
    }
}
