using System;
using System.Windows;
using System.Windows.Controls;
using EpicBattle.Models.Items;
using EpicBattle.ViewModels;

namespace EpicBattle.Views
{
    public partial class InventoryView : UserControl
    {
        public InventoryView(Action closeAction = null)
        {
            InitializeComponent();
            var vm = new InventoryViewModel();
            vm.CloseAction = closeAction;
            DataContext = vm;
        }

        private void SlotBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string slotName && DataContext is InventoryViewModel vm)
            {
                vm.SelectSlot(slotName);
            }
        }

        private void ItemBtn_Click(object sender, RoutedEventArgs e)
        {
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
            if (DataContext is InventoryViewModel vm)
            {
                vm.CloseAction?.Invoke();
            }
        }
    }
}
