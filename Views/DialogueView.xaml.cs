using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EpicBattle.Managers;
using EpicBattle.Models;

namespace EpicBattle.Views
{
    public partial class DialogueView : UserControl
    {
        public DialogueView()
        {
            InitializeComponent();
            this.Loaded += (s, e) => this.Focus(); // Нужно для перехвата ESC
            LoadNode(SaveManager.CurrentState.CurrentNodeId);
        }

        private void LoadNode(string nodeId)
        {
            if (StoryDatabase.Nodes.TryGetValue(nodeId, out var node))
            {
                SaveManager.CurrentState.CurrentNodeId = nodeId;

                SpeakerText.Text = node.SpeakerName;
                DialogueText.Text = node.Text;

                var filteredChoices = new System.Collections.Generic.List<DialogueChoice>();
                foreach (var choice in node.Choices)
                {
                    if (string.IsNullOrEmpty(choice.RequiredFlag))
                    {
                        filteredChoices.Add(choice);
                    }
                    else if (choice.RequiredFlag == "HasTomeIntact" && SaveManager.CurrentState.HasTomeIntact)
                    {
                        filteredChoices.Add(choice);
                    }
                    else if (choice.RequiredFlag == "HonorGromgar" && SaveManager.CurrentState.HonorGromgar)
                    {
                        filteredChoices.Add(choice);
                    }
                    else if (choice.RequiredFlag.StartsWith("Class_"))
                    {
                        string requiredClass = choice.RequiredFlag.Substring(6); // Убираем "Class_"
                        if (SaveManager.CurrentState.PlayerClass == requiredClass)
                        {
                            filteredChoices.Add(choice);
                        }
                    }
                }

                ChoicesControl.ItemsSource = filteredChoices;
            }
        }

        private void ChoiceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is DialogueChoice choice)
            {
                // Выполнение кастомных действий перед переходом
                if (choice.Action == "StunAndLoot")
                {
                    SaveManager.CurrentState.Gold += 10;
                    MessageBox.Show("Получено 10 золота", "Добыча", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Ruthless")
                {
                    SaveManager.CurrentState.HasTraitRuthless = true;
                    SaveManager.CurrentState.HasForestMap = true;
                    MessageBox.Show("Получена черта: Беспощадный.\nПолучена Карта засады в лесу.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "ExecuteOrc")
                {
                    SaveManager.CurrentState.HasForestMap = true;
                    SaveManager.CurrentState.HasVillageChestKey = true;
                    MessageBox.Show("Получена Карта засады в лесу.\nПолучен Ключ от сундука.", "Добыча", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Brann_Noble")
                {
                    SaveManager.CurrentState.HasQuestAncestorsLegacy = true;
                    SaveManager.CurrentState.HpPotions += 1;
                    MessageBox.Show("Получено Зелье здоровья (+1).\nНачат квест: «Наследие предков».", "Квест", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Brann_Pragmatic")
                {
                    SaveManager.CurrentState.HasQuestAncestorsLegacy = true;
                    SaveManager.CurrentState.KnowsAboutCitadelSeals = true;
                    SaveManager.CurrentState.UnlockedAmbush = true;
                    MessageBox.Show("Начат квест: «Украденные тайны».\nОткрыта метка на карте: Засада на тракте.", "Квест", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Brann_Mercenary")
                {
                    SaveManager.CurrentState.HasQuestAncestorsLegacy = true;
                    SaveManager.CurrentState.PromisedRuneOfProtection = true;
                    MessageBox.Show("Начат квест: «Украденные тайны» (Наемник).\nОбещана награда: Руна Защиты.", "Квест", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                else if (choice.Action == "Gromgar_Honor")
                {
                    SaveManager.CurrentState.HonorGromgar = true;
                }

                // Навигация
                if (choice.Action == "StartBattle")
                {
                    if (choice.ActionParam == "Gromgar_Honor")
                    {
                        SaveManager.CurrentState.HonorGromgar = true;
                    }
                    // Переход в бой, передаем NextNodeId для возврата и ActionParam для модификаторов боя
                    NavigationManager.NavigateTo(new BattleView(isArcade: false, returnNodeId: choice.NextNodeId, battleModifier: choice.ActionParam));
                }
                else if (choice.Action == "MainMenu" || choice.Action == "Ending_Light" || choice.Action == "Ending_Dark" || choice.Action == "Ending_Neutral")
                {
                    if (choice.Action.StartsWith("Ending_"))
                    {
                        MessageBox.Show("Поздравляем с прохождением EpicBattle!\nСпасибо за игру.", "Игра завершена", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    NavigationManager.NavigateTo(new MainMenu());
                }
                else if (choice.Action == "CheckTomeIntact")
                {
                    if (SaveManager.CurrentState.HasTomeIntact)
                    {
                        LoadNode("Scene3_TomeIntact");
                    }
                    else
                    {
                        LoadNode("Scene3_TomeDamaged");
                    }
                }
                else
                {
                    LoadNode(choice.NextNodeId);
                }
            }
        }

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
            NavigationManager.NavigateTo(new SaveLoadView(isSaving: true));
        }

        private void MainMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new MainMenu());
        }
    }
}