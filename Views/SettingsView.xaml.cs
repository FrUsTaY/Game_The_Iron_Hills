using System.Windows;
using System.Windows.Controls;
using EpicBattle.Managers;

namespace EpicBattle.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            MusicSlider.Value = SaveManager.Settings.MusicVolume;
            SfxSlider.Value = SaveManager.Settings.SfxVolume;
        }

        private void MusicSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SaveManager.Settings != null)
                SaveManager.Settings.MusicVolume = e.NewValue;
        }

        private void SfxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SaveManager.Settings != null)
                SaveManager.Settings.SfxVolume = e.NewValue;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveManager.SaveSettings();
            NavigationManager.NavigateTo(new MainMenu());
        }
    }
}