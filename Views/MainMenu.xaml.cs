using System.Windows;
using System.Windows.Controls;
using EpicBattle.Managers;

namespace EpicBattle.Views
{
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void StoryBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new CharacterCreationView(isArcade: false));
        }

        private void ArcadeBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new CharacterCreationView(isArcade: true));
        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new SaveLoadView(isSaving: false));
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new SettingsView());
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.ExitGame();
        }
    }
}