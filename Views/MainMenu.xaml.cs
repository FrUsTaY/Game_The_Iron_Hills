using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EpicBattle.Managers;
using EpicBattle.Services;

namespace EpicBattle.Views
{
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();
            BgImage.Source = ImageService.GetImage("Backgrounds/main_menu");
            var logo = ImageService.GetUiImage("game_logo");
            if (logo != null)
            {
                LogoImage.Source = logo;
                TitleText.Visibility = Visibility.Collapsed;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Focusable = true;
            Focus();
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (ModesPanel.Visibility == Visibility.Visible)
                {
                    BackBtn_Click(this, new RoutedEventArgs());
                    e.Handled = true;
                }
            }
        }

        // --- Переходы по подменю ---

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            AudioEngine.Play("click");
            MainButtonsPanel.Visibility = Visibility.Collapsed;
            ModesPanel.Visibility = Visibility.Visible;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            AudioEngine.Play("click_alt");
            ModesPanel.Visibility = Visibility.Collapsed;
            MainButtonsPanel.Visibility = Visibility.Visible;
        }

        // --- Режимы игры ---

        private void StoryBtn_Click(object sender, RoutedEventArgs e)
        {
            AudioEngine.Play("click");
            NavigationManager.NavigateTo(new CharacterCreationView(isArcade: false, isMapMode: false));
        }

        private void ChapterSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            AudioEngine.Play("click");
            NavigationManager.NavigateTo(new ChapterSelectView());
        }

        private void MapModeBtn_Click(object sender, RoutedEventArgs e)
        {
            AudioEngine.Play("click");
            var state = SaveManager.CurrentState;
            if (state != null && state.IsMapMode && state.ActiveMap != null && !state.ActiveMap.IsCompleted)
            {
                NavigationManager.NavigateTo(new MapView());
            }
            else
            {
                NavigationManager.NavigateTo(new CharacterCreationView(isArcade: true, isMapMode: true));
            }
        }

        private void ArcadeBtn_Click(object sender, RoutedEventArgs e)
        {
            AudioEngine.Play("click");
            NavigationManager.NavigateTo(new CharacterCreationView(isArcade: true, isMapMode: false));
        }

        // --- Системные разделы ---

        private void BestiaryBtn_Click(object sender, RoutedEventArgs e)
        {
            AudioEngine.Play("click");
            NavigationManager.NavigateTo(new BestiaryView());
        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            AudioEngine.Play("click");
            NavigationManager.NavigateTo(new SaveLoadView(isSaving: false));
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            AudioEngine.Play("click");
            NavigationManager.NavigateTo(new SettingsView());
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            AudioEngine.Play("click_alt");
            Application.Current.Shutdown();
        }
    }
}
