using System;
using System.Windows;
using System.Windows.Controls;
using EpicBattle.Managers;
using EpicBattle.Services;

namespace EpicBattle.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            BgImage.Source = ImageService.GetImage("Backgrounds/main_menu");

            var settings = SaveManager.Settings;
            if (settings != null)
            {
                MusicSlider.Value = settings.MusicVolume;
                SfxSlider.Value = settings.SfxVolume;
                UpdatePercents();

                // Инициализация режима дисплея
                string currentMode = settings.DisplayMode ?? "В окне";
                foreach (ComboBoxItem item in DisplayModeCombo.Items)
                {
                    if (item.Content?.ToString() == currentMode)
                    {
                        DisplayModeCombo.SelectedItem = item;
                        break;
                    }
                }
                if (DisplayModeCombo.SelectedItem == null && DisplayModeCombo.Items.Count > 0)
                {
                    DisplayModeCombo.SelectedIndex = 0;
                }

                // Инициализация разрешения
                string currentRes = settings.Resolution ?? "1280x720";
                foreach (ComboBoxItem item in ResolutionCombo.Items)
                {
                    if (item.Content?.ToString() == currentRes)
                    {
                        ResolutionCombo.SelectedItem = item;
                        break;
                    }
                }
                if (ResolutionCombo.SelectedItem == null && ResolutionCombo.Items.Count > 0)
                {
                    ResolutionCombo.SelectedIndex = 0;
                }
            }
        }

        private void UpdatePercents()
        {
            MusicPercentText.Text = $"{(int)(MusicSlider.Value * 100)}%";
            SfxPercentText.Text = $"{(int)(SfxSlider.Value * 100)}%";
        }

        private void MusicSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SaveManager.Settings != null)
            {
                SaveManager.Settings.MusicVolume = e.NewValue;
                AudioEngine.UpdateMusicVolume();
                if (MusicPercentText != null)
                {
                    MusicPercentText.Text = $"{(int)(e.NewValue * 100)}%";
                }
            }
        }

        private void SfxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SaveManager.Settings != null)
            {
                SaveManager.Settings.SfxVolume = e.NewValue;
                if (SfxPercentText != null)
                {
                    SfxPercentText.Text = $"{(int)(e.NewValue * 100)}%";
                }
            }
        }

        private void ApplyDisplayBtn_Click(object sender, RoutedEventArgs e)
        {
            AudioEngine.Play("click");
            if (SaveManager.Settings == null) return;

            if (DisplayModeCombo.SelectedItem is ComboBoxItem modeItem)
            {
                SaveManager.Settings.DisplayMode = modeItem.Content?.ToString() ?? "В окне";
            }

            if (ResolutionCombo.SelectedItem is ComboBoxItem resItem)
            {
                SaveManager.Settings.Resolution = resItem.Content?.ToString() ?? "1280x720";
            }

            DisplayManager.ApplySettings(Application.Current.MainWindow, SaveManager.Settings);
            SaveManager.SaveSettings();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            AudioEngine.Play("click");
            SaveManager.SaveSettings();
            if (NavigationManager.CanGoBack) NavigationManager.GoBack(); else NavigationManager.NavigateTo(new MainMenu());
        }
    }
}
