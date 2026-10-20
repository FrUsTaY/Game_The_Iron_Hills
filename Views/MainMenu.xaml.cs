using System.Windows;
using System.Windows.Controls;
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
        }

        private void StoryBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            NavigationManager.NavigateTo(new CharacterCreationView(isArcade: false, isMapMode: false));
        }

        private void ChapterSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            NavigationManager.NavigateTo(new ChapterSelectView());
        }

        private void MapModeBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
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
            EpicBattle.Services.AudioEngine.Play("click");
            NavigationManager.NavigateTo(new CharacterCreationView(isArcade: true, isMapMode: false));
        }

        private void InventoryBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("chest_open");
            NavigationManager.NavigateTo(new InventoryView(() =>
            {
                NavigationManager.NavigateTo(new MainMenu());
            }));
        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            NavigationManager.NavigateTo(new SaveLoadView(isSaving: false));
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            NavigationManager.NavigateTo(new SettingsView());
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
