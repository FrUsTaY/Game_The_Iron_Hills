using System;
using System.Windows;
using System.Windows.Controls;
using EpicBattle.ViewModels;

namespace EpicBattle.Views
{
    /// <summary>
    /// Этап 3.2: Экран Кузнеца — улучшение экипировки, покупка руды, заточка и перековка.
    /// </summary>
    public partial class BlacksmithView : UserControl
    {
        private readonly BlacksmithViewModel _vm;

        public BlacksmithView(Action onLeave)
        {
            InitializeComponent();
            BgImage.Source = EpicBattle.Services.ImageService.GetImage("Backgrounds/blacksmith");
            _vm = new BlacksmithViewModel(onLeave);
            DataContext = _vm;
        }

        private void UpgradeItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ForgeItemCard card)
            {
                _vm.UpgradeItemCommand.Execute(card);
            }
        }

        private void BuyOre_Click(object sender, RoutedEventArgs e)
        {
            _vm.BuyOreCommand.Execute(null);
        }

        private void Sharpen_Click(object sender, RoutedEventArgs e)
        {
            _vm.SharpenCommand.Execute(null);
        }

        private void Reforge_Click(object sender, RoutedEventArgs e)
        {
            _vm.ReforgeCommand.Execute(null);
        }

        private void SelectRune_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string key)
                _vm.SelectRuneCommand.Execute(key);
        }

        private void InsertRune_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ForgeItemCard card)
                _vm.InsertRuneCommand.Execute(card);
        }

        private void LeaveSmith_Click(object sender, RoutedEventArgs e)
        {
            _vm.LeaveSmithCommand.Execute(null);
        }
    }
}
