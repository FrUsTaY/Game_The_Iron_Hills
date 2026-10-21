using System;
using System.Windows;
using System.Windows.Controls;
using EpicBattle.Managers;
using EpicBattle.Models;
using EpicBattle.Models.Encounters;
using EpicBattle.Models.Items;

namespace EpicBattle.Views
{
    public partial class EncounterView : UserControl
    {
        private readonly RandomEncounter _encounter;
        private readonly Action _onCompleted;
        private bool _isResolved = false;

        public EncounterView(RandomEncounter encounter, Action onCompleted)
        {
            InitializeComponent();
            _encounter = encounter;
            _onCompleted = onCompleted;

            LoadEncounter();
        }

        private void LoadEncounter()
        {
            var state = SaveManager.CurrentState;
            IconText.Text = _encounter.Icon;
            TitleText.Text = _encounter.Title;
            DescriptionText.Text = _encounter.Description;
            HpText.Text = $"{state.PlayerHp}/{state.PlayerMaxHp}";
            GoldText.Text = state.Gold.ToString();

            var img = _encounter.Image;
            if (img != null)
            {
                BgImage.Source = img;
                BannerImage.Source = img;
            }

            ChoicesControl.ItemsSource = _encounter.Choices;
        }

        private void ChoiceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isResolved) return;
            if (sender is Button btn && btn.Tag is EncounterChoice choice)
            {
                var state = SaveManager.CurrentState;
                bool success = true;

                // Проверка условий выбора
                if (!string.IsNullOrEmpty(choice.RequiredStat))
                {
                    int playerStatVal = choice.RequiredStat switch
                    {
                        "STR" => state.TotalStrength,
                        "DEX" => state.TotalDexterity,
                        "INT" => state.TotalIntelligence,
                        "END" => state.TotalEndurance,
                        "Gold" => state.Gold,
                        "Hp" => state.PlayerHp,
                        _ => 0
                    };

                    if (playerStatVal < choice.RequiredValue)
                    {
                        success = false;
                    }
                }

                _isResolved = true;
                ChoicesControl.Visibility = Visibility.Collapsed;
                ResultPanel.Visibility = Visibility.Visible;
                ContinueBtn.Visibility = Visibility.Visible;

                if (success)
                {
                    EpicBattle.Services.AudioEngine.Play("chest_open");
                    ResultText.Foreground = System.Windows.Media.Brushes.LightGreen;
                    ResultText.Text = "✅ УСПЕХ: " + choice.SuccessText;

                    // Награды
                    if (choice.GoldCost > 0) state.Gold -= choice.GoldCost;
                    if (choice.GoldReward > 0) state.Gold += choice.GoldReward;
                    if (choice.HpReward > 0) state.PlayerHp = Math.Min(state.PlayerMaxHp, state.PlayerHp + choice.HpReward);
                    if (choice.MpReward > 0) state.PlayerMp = Math.Min(state.PlayerMaxMp, state.PlayerMp + choice.MpReward);
                    if (choice.HpDamage > 0) state.PlayerHp = Math.Max(1, state.PlayerHp - choice.HpDamage);
                    if (choice.StatPointsReward > 0) state.StatPoints += choice.StatPointsReward;
                    if (choice.SkillPointsReward > 0) state.SkillPoints += choice.SkillPointsReward;
                    if (choice.HpPotionsReward > 0) state.HpPotions += choice.HpPotionsReward;
                    if (choice.MpPotionsReward > 0) state.MpPotions += choice.MpPotionsReward;
                    
                    if (!string.IsNullOrEmpty(choice.ItemIdReward))
                    {
                        var rewardItem = ItemsDatabase.Clone(choice.ItemIdReward);
                        if (rewardItem != null && !state.Inventory.Exists(i => i.Id == rewardItem.Id))
                        {
                            state.Inventory.Add(rewardItem);
                        }
                    }
                }
                else
                {
                    EpicBattle.Services.AudioEngine.Play("gate_close");
                    ResultText.Foreground = System.Windows.Media.Brushes.Tomato;
                    ResultText.Text = "❌ НЕУДАЧА: " + choice.FailText;
                    if (choice.HpDamage > 0) state.PlayerHp = Math.Max(1, state.PlayerHp - choice.HpDamage);
                }

                HpText.Text = $"{state.PlayerHp}/{state.PlayerMaxHp}";
                GoldText.Text = state.Gold.ToString();
            }
        }

        private void ContinueBtn_Click(object sender, RoutedEventArgs e)
        {
            EpicBattle.Services.AudioEngine.Play("click");
            _onCompleted?.Invoke();
        }
    }
}
