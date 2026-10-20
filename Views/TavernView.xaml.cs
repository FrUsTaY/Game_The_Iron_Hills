using System;
using System.Windows;
using System.Windows.Controls;
using EpicBattle.ViewModels;

namespace EpicBattle.Views
{
    /// <summary>
    /// Этап 3.2 (часть 3): Экран Таверны — отдых за золото, слухи и скупка рун.
    /// </summary>
    public partial class TavernView : UserControl
    {
        private readonly TavernViewModel _vm;

        public TavernView(Action onLeave)
        {
            InitializeComponent();
            BgImage.Source = EpicBattle.Services.ImageService.GetImage("Backgrounds/tavern");
            _vm = new TavernViewModel(onLeave);
            DataContext = _vm;
        }

        private void Rest_Click(object sender, RoutedEventArgs e) => _vm.RestCommand.Execute(null);

        private void Rumor_Click(object sender, RoutedEventArgs e) => _vm.RumorCommand.Execute(null);

        private void SellRune_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is TavernRuneCard card)
                _vm.SellRuneCommand.Execute($"{card.RuneId}|{card.Count}");
        }

        private void LeaveTavern_Click(object sender, RoutedEventArgs e) => _vm.LeaveTavernCommand.Execute(null);
    }
}
