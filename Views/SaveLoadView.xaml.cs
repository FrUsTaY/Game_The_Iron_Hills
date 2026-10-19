using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using EpicBattle.Managers;

namespace EpicBattle.Views
{
    public partial class SaveLoadView : UserControl
    {
        private bool _isSaving;

        public SaveLoadView(bool isSaving)
        {
            InitializeComponent();
            _isSaving = isSaving;

            if (_isSaving)
            {
                TitleText.Text = "СОХРАНИТЬ ИГРУ";
                SaveInputPanel.Visibility = Visibility.Visible;
                ActionBtn.Content = "Перезаписать";
            }
            else
            {
                TitleText.Text = "ЗАГРУЗИТЬ ИГРУ";
            }

            RefreshList();
        }

        private void RefreshList()
        {
            SavesList.ItemsSource = SaveManager.GetAllSaves();
        }

        private void SavesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActionBtn.IsEnabled = SavesList.SelectedItem != null;
            if (SavesList.SelectedItem is FileInfo file && _isSaving)
            {
                SaveNameInput.Text = Path.GetFileNameWithoutExtension(file.Name);
            }
        }

        private void CreateSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SaveNameInput.Text)) return;
            if (SaveManager.SaveGame(SaveNameInput.Text))
            {
                RefreshList();
                MessageBox.Show("Игра сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Не удалось сохранить игру. Проверьте свободное место и права доступа.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SavesList.SelectedItem is FileInfo file)
            {
                if (_isSaving)
                {
                    // Перезапись
                    if (SaveManager.SaveGame(Path.GetFileNameWithoutExtension(file.Name)))
                    {
                        RefreshList();
                        MessageBox.Show("Игра сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось сохранить игру.", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Загрузка
                    if (!SaveManager.LoadGame(file.FullName))
                    {
                        MessageBox.Show("Файл сохранения повреждён или несовместим.", "Ошибка загрузки",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var state = SaveManager.CurrentState;
                    if (state.IsMapMode && state.ActiveMap != null)
                    {
                        NavigationManager.NavigateTo(new MapView());
                    }
                    else if (state.IsStoryMode)
                    {
                        NavigationManager.NavigateTo(new DialogueView());
                    }
                    else
                    {
                        NavigationManager.NavigateTo(new BattleView(isArcade: true));
                    }
                }
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SavesList.SelectedItem is FileInfo file)
            {
                SaveManager.DeleteSave(file.FullName);
                RefreshList();
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            // Выход из окна сохранений — всегда в главное меню.
            // rememberInHistory: false — чтобы в стеке не оставалось устаревшее окно SaveLoadView.
            NavigationManager.NavigateTo(new MainMenu(), rememberInHistory: false);
        }
    }
}