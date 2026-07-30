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
            SaveManager.SaveGame(SaveNameInput.Text);
            RefreshList();
            MessageBox.Show("Игра сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ActionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SavesList.SelectedItem is FileInfo file)
            {
                if (_isSaving)
                {
                    // Перезапись
                    SaveManager.SaveGame(Path.GetFileNameWithoutExtension(file.Name));
                    RefreshList();
                    MessageBox.Show("Игра сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Загрузка
                    SaveManager.LoadGame(file.FullName);
                    if (SaveManager.CurrentState.IsStoryMode)
                    {
                        NavigationManager.NavigateTo(new DialogueView());
                    }
                    else
                    {
                        NavigationManager.NavigateTo(new BattleView(isArcade: true, isLoaded: true));
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
            // Возвращаемся в игру или в меню в зависимости от контекста
            // Простейший вариант - если мы из игры, надо бы вернуться в игру.
            // Для упрощения пока всегда возвращаем в главное меню, если не настроить стек навигации.
            NavigationManager.NavigateTo(new MainMenu());
        }
    }
}