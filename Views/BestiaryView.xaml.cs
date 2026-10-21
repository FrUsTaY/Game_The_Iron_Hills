using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EpicBattle.Managers;
using EpicBattle.Models.RPG;
using EpicBattle.Services;

namespace EpicBattle.Views
{
    public partial class BestiaryView : UserControl
    {
        private string _selectedCategory = "Все";
        private List<BestiaryEntry> _filteredEntries = new();

        public BestiaryView()
        {
            InitializeComponent();
            BgImage.Source = ImageService.GetImage("Backgrounds/main_menu") ?? ImageService.GetImage("Battle/battle_mountain_pass");

            BuildCategoryFilters();
            ApplyFilter();

            if (EntriesListBox.Items.Count > 0)
            {
                EntriesListBox.SelectedIndex = 0;
            }
        }

        private void BuildCategoryFilters()
        {
            CategoryFiltersPanel.Children.Clear();

            var categories = new List<string> { "Все", "Орочья Орда", "Порождения Тени", "Древние Конструкты", "Боссы и Владыки", "Герои и Спутники" };

            foreach (var cat in categories)
            {
                int count = cat == "Все" ? BestiaryDatabase.Entries.Count : BestiaryDatabase.Entries.Count(e => e.Category == cat);

                var btn = new Button
                {
                    Content = $"{cat} ({count})",
                    Margin = new Thickness(4, 2, 4, 2),
                    Padding = new Thickness(12, 5, 12, 5),
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Tag = cat
                };

                btn.Click += CategoryBtn_Click;
                UpdateCategoryButtonStyle(btn, cat == _selectedCategory);
                CategoryFiltersPanel.Children.Add(btn);
            }
        }

        private void UpdateCategoryButtonStyle(Button btn, bool isSelected)
        {
            if (isSelected)
            {
                btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8A5A20"));
                btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFAA00"));
                btn.Foreground = Brushes.White;
            }
            else
            {
                btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#251F2E"));
                btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A3F55"));
                btn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCCCCC"));
            }
        }

        private void CategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            AudioEngine.Play("click");
            if (sender is Button btn && btn.Tag is string cat)
            {
                _selectedCategory = cat;
                foreach (Button child in CategoryFiltersPanel.Children)
                {
                    UpdateCategoryButtonStyle(child, child.Tag?.ToString() == _selectedCategory);
                }
                ApplyFilter();
            }
        }

        private void ApplyFilter()
        {
            string query = SearchBox.Text?.Trim().ToLowerInvariant() ?? string.Empty;

            _filteredEntries = BestiaryDatabase.Entries.Where(entry =>
            {
                bool matchesCat = _selectedCategory == "Все" || entry.Category == _selectedCategory;
                bool matchesQuery = string.IsNullOrEmpty(query) ||
                                    entry.Name.ToLowerInvariant().Contains(query) ||
                                    entry.Subtitle.ToLowerInvariant().Contains(query) ||
                                    entry.Description.ToLowerInvariant().Contains(query);
                return matchesCat && matchesQuery;
            }).ToList();

            EntriesListBox.ItemsSource = _filteredEntries;

            if (_filteredEntries.Count > 0)
            {
                EntriesListBox.SelectedIndex = 0;
            }
            else
            {
                ClearDetails();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchPlaceholder.Visibility = string.IsNullOrEmpty(SearchBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            ApplyFilter();
        }

        private void EntriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EntriesListBox.SelectedItem is BestiaryEntry entry)
            {
                DisplayEntry(entry);
            }
        }

        private void DisplayEntry(BestiaryEntry entry)
        {
            DetailArtImage.Source = entry.Image;
            DetailTitleText.Text = entry.Name;
            DetailSubtitleText.Text = entry.Subtitle;
            DetailCategoryBadge.Text = entry.Category;
            DetailDangerBadge.Text = $"Угроза: {entry.DangerLevel}";
            DetailEpigraphText.Text = entry.Epigraph;
            DetailDescriptionText.Text = entry.Description;
            DetailTacticsText.Text = entry.Tactics;
        }

        private void ClearDetails()
        {
            DetailArtImage.Source = null;
            DetailTitleText.Text = "Существо не найдено";
            DetailSubtitleText.Text = string.Empty;
            DetailCategoryBadge.Text = string.Empty;
            DetailDangerBadge.Text = string.Empty;
            DetailEpigraphText.Text = string.Empty;
            DetailDescriptionText.Text = "Попробуйте изменить поисковый запрос или выбрать другую категорию.";
            DetailTacticsText.Text = string.Empty;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            AudioEngine.Play("click");
            if (NavigationManager.CanGoBack)
                NavigationManager.GoBack();
            else
                NavigationManager.NavigateTo(new MainMenu());
        }
    }
}
