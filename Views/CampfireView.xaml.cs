using System;
using System.Windows;
using System.Windows.Controls;
using EpicBattle.Managers;
using EpicBattle.Models;

namespace EpicBattle.Views
{
    public partial class CampfireView : UserControl
    {
        private readonly Action _onCompleted;
        private readonly GameState _state;
        private bool _actionUsed = false;

        public CampfireView(Action onCompleted)
        {
            InitializeComponent();
            _onCompleted = onCompleted;
            _state = SaveManager.CurrentState;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            HpText.Text = $"{_state.PlayerHp} / {_state.PlayerMaxHp}";
            MpText.Text = $"{_state.PlayerMp} / {_state.PlayerMaxMp}";
            PotionsText.Text = $"{_state.HpPotions} HP / {_state.MpPotions} MP";
            PointsText.Text = $"{_state.StatPoints} Статов / {_state.SkillPoints} Навыков";
        }

        private void DisableCampfireActions()
        {
            _actionUsed = true;
            RestBtn.IsEnabled = false;
            MeditateBtn.IsEnabled = false;
            SharpenBtn.IsEnabled = false;
        }

        private void RestBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_actionUsed) return;
            DisableCampfireActions();

            int healHp = (int)(_state.PlayerMaxHp * 0.40);
            int healMp = (int)(_state.PlayerMaxMp * 0.50);
            _state.PlayerHp = Math.Min(_state.PlayerMaxHp, _state.PlayerHp + healHp);
            _state.PlayerMp = Math.Min(_state.PlayerMaxMp, _state.PlayerMp + healMp);

            if (_state.HpPotions == 0) _state.HpPotions = 1;

            ResultText.Text = $"🔥 Вы славно отдохнули у огня (+{healHp} HP, +{healMp} MP)!";
            UpdateDisplay();
        }

        private void MeditateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_actionUsed) return;
            DisableCampfireActions();

            _state.StatPoints += 1;
            _state.SkillPoints += 1;

            ResultText.Text = "🧘 Медитация открыла вам новые грани мастерства (+1 Очко Характеристик, +1 Очко Навыков)!";
            UpdateDisplay();
        }

        private void SharpenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_actionUsed) return;

            // P1-фикс (баланс): бесплатная перманентная заточка +2 за каждый костёр была
            // доминирующей стратегией (за акт — до +8..+12 урона). Теперь заточка требует
            // руду (1 ⛏️) и даёт умеренный бонус +1. Без руды кнопка не сработает.
            if (_state.Ore < 1)
            {
                ResultText.Text = "⚒️ Для заточки нужна Железная Руда Кряжа (⛏️ 1 шт.)! Добудьте её в бою или купите у кузнеца.";
                return; // действие НЕ расходуется — можно выбрать другое
            }

            DisableCampfireActions();

            _state.Ore -= 1;
            // P2-доп: бонус заточки — в SharpenBonus (устойчив к пересчёту производных статов)
            _state.SharpenBonus += 1;
            _state.RecalculateDerivedStats();

            ResultText.Text = "⚔️ Вы наточили клинки на вулканической руде (+1 к Базовому Урону и Магии навсегда, −1 ⛏️)!";
            UpdateDisplay();
        }

        private void OpenInventory_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new InventoryView(() =>
            {
                NavigationManager.NavigateTo(this);
                UpdateDisplay();
            }));
        }

        private void OpenSkillTree_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new SkillTreeView(() =>
            {
                NavigationManager.NavigateTo(this);
                UpdateDisplay();
            }));
        }

        private void ContinueBtn_Click(object sender, RoutedEventArgs e)
        {
            _onCompleted?.Invoke();
        }
    }
}
