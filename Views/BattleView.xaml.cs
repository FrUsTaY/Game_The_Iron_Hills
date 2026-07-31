using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using EpicBattle.ViewModels;

namespace EpicBattle.Views
{
    public partial class BattleView : UserControl
    {
        public BattleView(bool isArcade = true, string returnNodeId = null, string battleModifier = null)
        {
            InitializeComponent();
            var vm = new BattleViewModel(isArcade, returnNodeId, battleModifier);

            vm.RequestOpenSkillTree = () =>
            {
                SkillTreeView skillTreeView = null;
                skillTreeView = new SkillTreeView(() =>
                {
                    if (skillTreeView != null)
                    {
                        MainGrid.Children.Remove(skillTreeView);
                        vm.UpdateSkillPointsDisplay();
                    }
                });

                Grid.SetRowSpan(skillTreeView, 5);
                Panel.SetZIndex(skillTreeView, 200);
                MainGrid.Children.Add(skillTreeView);
            };

            DataContext = vm;
        }

        // Пауза
        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                PauseOverlay.Visibility = PauseOverlay.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void ResumeBtn_Click(object sender, RoutedEventArgs e)
        {
            PauseOverlay.Visibility = Visibility.Collapsed;
            this.Focus();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Здесь сохранения недоступны по логике (только из диалогов)
        }

        private void MainMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            // bgmPlayer.Stop(); - пока отключим, чтобы не заморачиваться
            Managers.NavigationManager.NavigateTo(new MainMenu());
        }
    }
}