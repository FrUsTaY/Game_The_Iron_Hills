using System;
using System.Windows;
using System.Windows.Controls;
using EpicBattle.Managers;
using EpicBattle.Models.Map;
using EpicBattle.ViewModels;

namespace EpicBattle.Views
{
    public partial class MapView : UserControl
    {
        private readonly MapViewModel _vm;

        public MapView()
        {
            InitializeComponent();
            BgImage.Source = EpicBattle.Services.ImageService.GetImage("Backgrounds/map_view");
            _vm = new MapViewModel();
            DataContext = _vm;

            Loaded += (s, e) =>
            {
                ScrollToCurrentFloor();
            };
        }

        private void ScrollToCurrentFloor()
        {
            Dispatcher.InvokeAsync(() =>
            {
                if (MapScrollViewer != null && MapScrollViewer.ScrollableHeight > 0)
                {
                    double ratio = 1.0 - ((double)_vm.CurrentFloor / Math.Max(1, _vm.TotalFloors));
                    MapScrollViewer.ScrollToVerticalOffset(ratio * MapScrollViewer.ScrollableHeight);
                }
                else if (MapScrollViewer != null)
                {
                    MapScrollViewer.ScrollToBottom();
                }
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void NodeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is MapNode node)
            {
                _vm.SelectNodeCommand.Execute(node);
            }
        }

        private void OpenInventory_Click(object sender, RoutedEventArgs e)
        {
            _vm.OpenInventoryCommand.Execute(null);
        }

        private void OpenSkillTree_Click(object sender, RoutedEventArgs e)
        {
            _vm.OpenSkillTreeCommand.Execute(null);
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            _vm.SaveGameCommand.Execute(null);
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click_alt");
            NavigationManager.NavigateTo(new MainMenu());
        }

        private void NextActBtn_Click(object sender, RoutedEventArgs e)
        {
            _vm.ProceedToNextAct();
        }
    }
}
