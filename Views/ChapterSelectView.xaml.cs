using System.Windows;
using System.Windows.Controls;
using EpicBattle.Managers;
using EpicBattle.Models;

namespace EpicBattle.Views
{
    public partial class ChapterSelectView : UserControl
    {
        public ChapterSelectView()
        {
            InitializeComponent();
            var state = SaveManager.CurrentState;
            if (state == null || state.PlayerLevel < 2)
            {
                ContinueCurrentToAct2Btn.Visibility = Visibility.Collapsed;
            }
            if (state == null || (state.PlayerLevel < 4 && !state.Act2Completed))
            {
                ContinueCurrentToAct3Btn.Visibility = Visibility.Collapsed;
            }
        }

        private void StartAct1Btn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            NavigationManager.NavigateTo(new CharacterCreationView(isArcade: false));
        }

        private void StartAct2NewBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            // Создаем героя 3-го уровня с готовым билдом и отправляем в Акт 2
            var state = new GameState();
            state.SetupForAct2("Ветеран");
            SaveManager.CurrentState = state;
            NavigationManager.NavigateTo(new DialogueView());
        }

        private void ContinueToAct2CurrentBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            var state = SaveManager.CurrentState;
            if (state != null)
            {
                state.CurrentAct = 2;
                state.CurrentNodeId = "Act2_Start";
                state.IsStoryMode = true;
                state.RecalculateDerivedStats();
                state.PlayerHp = state.PlayerMaxHp;
                state.PlayerMp = state.PlayerMaxMp;
                NavigationManager.NavigateTo(new DialogueView());
            }
        }

        private void StartAct3NewBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            // Создаем героя 6-го уровня с готовым билдом и отправляем в Акт 3
            var state = new GameState();
            state.SetupForAct3("Ветеран");
            SaveManager.CurrentState = state;
            NavigationManager.NavigateTo(new DialogueView());
        }

        private void ContinueToAct3CurrentBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            var state = SaveManager.CurrentState;
            if (state != null)
            {
                state.SetupForAct3(state.PlayerClass);
                NavigationManager.NavigateTo(new DialogueView());
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click_alt");
            NavigationManager.NavigateTo(new MainMenu());
        }
    }
}
