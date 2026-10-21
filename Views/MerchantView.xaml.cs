using System;
using System.Windows;
using System.Windows.Controls;
using EpicBattle.ViewModels;

namespace EpicBattle.Views
{
    public partial class MerchantView : UserControl
    {
        private readonly MerchantViewModel _vm;

        public MerchantView(Action onLeave)
        {
            InitializeComponent();
            BgImage.Source = EpicBattle.Services.ImageService.GetImage("Backgrounds/merchant");
            NpcAvatar.Source = EpicBattle.Services.ImageService.GetCharacterImage("npc_merchant");
            _vm = new MerchantViewModel(onLeave);
            DataContext = _vm;
        }

        private void BuyItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is MerchantOfferItem item)
            {
                _vm.BuyItemCommand.Execute(item);
            }
        }

        private void SellItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is PlayerSellItem item)
            {
                _vm.SellItemCommand.Execute(item);
            }
        }

        private void BuyHpPotion_Click(object sender, RoutedEventArgs e)
        {
            _vm.BuyHpPotionCommand.Execute(null);
        }

        private void BuyMpPotion_Click(object sender, RoutedEventArgs e)
        {
            _vm.BuyMpPotionCommand.Execute(null);
        }

        private void BuyStatScroll_Click(object sender, RoutedEventArgs e)
        {
            _vm.BuyStatScrollCommand.Execute(null);
        }

        private void BuySkillTome_Click(object sender, RoutedEventArgs e)
        {
            _vm.BuySkillTomeCommand.Execute(null);
        }

        private void SellOre_Click(object sender, RoutedEventArgs e)
        {
            _vm.SellOreCommand.Execute(null);
        }

        private void LeaveShop_Click(object sender, RoutedEventArgs e)
        {
            _vm.LeaveShopCommand.Execute(null);
        }
    }
}
