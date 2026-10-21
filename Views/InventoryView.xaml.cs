using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EpicBattle.Managers;
using EpicBattle.Models.Items;
using EpicBattle.ViewModels;

namespace EpicBattle.Views
{
    public partial class InventoryView : UserControl
    {
        private bool _isArtMode = false;

        public InventoryView(Action closeAction = null)
        {
            InitializeComponent();
            BgImage.Source = EpicBattle.Services.ImageService.GetImage("Backgrounds/inventory");
            var vm = new InventoryViewModel();
            vm.CloseAction = closeAction;
            DataContext = vm;
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
                if (_isArtMode)
                {
                    SetArtMode(false);
                    e.Handled = true;
                    return;
                }
                CloseBtn_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
        }

        private void ToggleArtMode_Click(object sender, RoutedEventArgs e)
        {
            SetArtMode(!_isArtMode);
        }

        private void ArtModeBanner_Click(object sender, RoutedEventArgs e)
        {
            SetArtMode(false);
        }

        private void RootGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isArtMode)
            {
                SetArtMode(false);
                e.Handled = true;
            }
        }

        private void SetArtMode(bool active)
        {
            _isArtMode = active;
            EpicBattle.Services.AudioEngine.Play("click");
            if (active)
            {
                MainPanelsGrid.Visibility = Visibility.Collapsed;
                ArtInspectionBar.Visibility = Visibility.Visible;
            }
            else
            {
                MainPanelsGrid.Visibility = Visibility.Visible;
                ArtInspectionBar.Visibility = Visibility.Collapsed;
            }
        }

        private void SlotBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            if (sender is Button btn && btn.Tag is string slotName && DataContext is InventoryViewModel vm)
            {
                vm.SelectSlot(slotName);
            }
        }

        private void ItemBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            if (sender is Button btn && btn.Tag is Item item && DataContext is InventoryViewModel vm)
            {
                vm.SelectedItem = item;
            }
        }

        private void EquipBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is InventoryViewModel vm)
            {
                vm.EquipSelectedItem();
            }
        }

        private void UseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is InventoryViewModel vm)
            {
                vm.UseSelectedItem();
            }
        }

        private void UnequipBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is InventoryViewModel vm)
            {
                vm.UnequipSelectedItem();
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("chest_close");
            if (DataContext is InventoryViewModel vm && vm.CloseAction != null)
            {
                vm.CloseAction.Invoke();
            }
            else if (NavigationManager.CanGoBack)
            {
                NavigationManager.GoBack();
            }
            else
            {
                NavigationManager.NavigateTo(new MainMenu());
            }
        }
    }
}
