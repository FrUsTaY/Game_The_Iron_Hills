using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using EpicBattle.Managers;
using EpicBattle.Models;
using EpicBattle.Models.Combat;
using EpicBattle.Models.RPG;
using EpicBattle.Views;

namespace EpicBattle.ViewModels
{
    public class BattleViewModel : INotifyPropertyChanged
    {
        private BattleEngine _engine;
        private GameState _gameState;
        private bool _isArcade;
        private string _returnNodeId;
        private string _battleModifier;
        private int _arcadeLevel = 1;

        public BattlePlayer Player => _engine?.Player;
        public ObservableCollection<BattleEnemy> Enemies { get; private set; } = new ObservableCollection<BattleEnemy>();
        public ObservableCollection<CombatLogMessage> CombatLog { get; private set; } = new ObservableCollection<CombatLogMessage>();
        public ObservableCollection<Skill> PlayerSkills { get; private set; } = new ObservableCollection<Skill>();

        private BattleEnemy _targetedEnemy;
        public BattleEnemy TargetedEnemy
        {
            get => _targetedEnemy;
            set
            {
                if (_targetedEnemy != null) _targetedEnemy.IsTargeted = false;
                _targetedEnemy = value;
                if (_targetedEnemy != null) _targetedEnemy.IsTargeted = true;

                // Триггерим обновление UI врагов
                var temp = Enemies.ToList();
                Enemies.Clear();
                foreach(var e in temp) Enemies.Add(e);

                OnPropertyChanged();
                UpdateCommandStates();
            }
        }

        public bool IsPlayerTurn => _engine?.TurnQueue.FirstOrDefault() is BattlePlayer;

        private bool _isUpgradeOverlayVisible;
        public bool IsUpgradeOverlayVisible
        {
            get => _isUpgradeOverlayVisible;
            set { _isUpgradeOverlayVisible = value; OnPropertyChanged(); }
        }

        private bool _isPlayerDead;
        public bool IsPlayerDead
        {
            get => _isPlayerDead;
            set { _isPlayerDead = value; OnPropertyChanged(); }
        }

        public bool IsStoryMode => !_isArcade;
        public bool IsArcadeMode => _isArcade;

        public BattleViewModel(bool isArcade, string returnNodeId = null, string battleModifier = null)
        {
            _isArcade = isArcade;
            _returnNodeId = returnNodeId;
            _battleModifier = battleModifier;
            _gameState = SaveManager.CurrentState;

            if (_isArcade && returnNodeId == null)
            {
                // Сброс статов при начале нового аркадного рана (как было в старом коде)
                _gameState.PlayerHp = _gameState.PlayerMaxHp;
                _gameState.PlayerMp = _gameState.PlayerMaxMp;
            }

            StartNewBattle();
        }

        private void StartNewBattle()
        {
            CombatLog.Clear();
            Enemies.Clear();
            PlayerSkills.Clear();

            var player = new BattlePlayer(_gameState);
            foreach(var s in player.UnlockedSkills)
            {
                if (s.Type == SkillType.Active)
                    PlayerSkills.Add(s);
            }

            List<BattleEnemy> enemiesList;
            if (_isArcade)
            {
                enemiesList = EnemyFactory.GenerateArcadeEnemies(_arcadeLevel, count: new Random().Next(1, 4));
            }
            else
            {
                enemiesList = EnemyFactory.GenerateStoryEnemies(_returnNodeId, _battleModifier);
            }

            foreach(var e in enemiesList)
                Enemies.Add(e);

            _engine = new BattleEngine(player, enemiesList);

            // Инициализация намерений перед первым ходом
            foreach(var e in enemiesList) e.DetermineIntent(enemiesList, player);

            _engine.OnLogEvent += (msg) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    CombatLog.Add(new CombatLogMessage(msg));
                    // Автоскролл можно реализовать во View
                });
            };

            _engine.OnQueueUpdated += () =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    OnPropertyChanged(nameof(Player));
                    OnPropertyChanged(nameof(Enemies));
                    OnPropertyChanged(nameof(IsPlayerTurn));
                    UpdateCommandStates();

                    // Обновляем UI коллекции врагов, чтобы триггерить биндинги
                    var temp = Enemies.ToList();
                    Enemies.Clear();
                    foreach(var e in temp) Enemies.Add(e);

                    if (TargetedEnemy != null && TargetedEnemy.Hp <= 0)
                        TargetedEnemy = Enemies.FirstOrDefault(e => e.Hp > 0);

                    CheckBattleEnd();
                });
            };

            _engine.NextTurn(); // Запуск боя
        }

        private void CheckBattleEnd()
        {
            if (_engine.IsBattleOver())
            {
                if (_engine.Player.Hp > 0)
                {
                    Log("Вы победили!");
                    _engine.Player.UpdateGameState(_gameState); // Сохраняем состояние

                    // Начисление награды
                    _gameState.StatPoints += 1;
                    _gameState.SkillPoints += 1;

                    // Показываем окно прокачки
                    IsUpgradeOverlayVisible = true;
                    OnPropertyChanged(nameof(IsStoryMode));
                    OnPropertyChanged(nameof(IsArcadeMode));
                }
                else
                {
                    Log("Вы погибли...");
                    IsPlayerDead = true;
                    UpdateCommandStates();
                }
            }
        }

        public ICommand AttackCommand => new Command(_ => Attack(), _ => CanAttack());
        public ICommand UpgradeCommand => new Command(UpgradeStat);
        public ICommand NextBattleCommand => new Command(_ => NextBattle());
        public ICommand ContinueStoryCommand => new Command(_ => ContinueStory());
        public ICommand OpenSkillTreeCommand => new Command(_ => OpenSkillTree());

        private void UpgradeStat(object param)
        {
            string stat = param?.ToString();
            if (stat == "Damage")
            {
                _gameState.PlayerBaseDamage += 5;
                _gameState.PlayerBaseMagicDamage += 5;
            }
            else if (stat == "Health")
            {
                _gameState.PlayerMaxHp += 20;
                _gameState.PlayerHp += 20;
            }
            else if (stat == "Potions")
            {
                _gameState.HpPotions = 3;
                _gameState.MpPotions = 2;
            }

            // Кнопки блокируются в UI (или скрываем окно)
            // Но мы оставляем окно открытым, чтобы игрок нажал "Следующий бой"
            // Чтобы предотвратить повторный клик, можно было бы сделать IsUpgraded флаг
        }

        private void NextBattle()
        {
            IsUpgradeOverlayVisible = false;
            _arcadeLevel++;
            StartNewBattle();
        }

        private void ContinueStory()
        {
            IsUpgradeOverlayVisible = false;
            if (!string.IsNullOrEmpty(_returnNodeId))
            {
                _gameState.CurrentNodeId = _returnNodeId;
            }
            NavigationManager.NavigateTo(new DialogueView());
        }

        private void OpenSkillTree()
        {
            RequestOpenSkillTree?.Invoke();
        }

        public Action RequestOpenSkillTree { get; set; }
        public ICommand DefendCommand => new Command(_ => Defend(), _ => CanDefend());
        public ICommand UsePotionHpCommand => new Command(_ => UsePotionHp(), _ => CanUsePotionHp());
        public ICommand UsePotionMpCommand => new Command(_ => UsePotionMp(), _ => CanUsePotionMp());
        public ICommand EndTurnCommand => new Command(_ => EndTurn(), _ => IsPlayerTurn && !IsPlayerDead);
        public ICommand SelectTargetCommand => new Command(SelectTarget);
        public ICommand UseSkillCommand => new Command(UseSkill, CanUseSkill);
        public ICommand ReturnToMenuCommand => new Command(_ => NavigationManager.NavigateTo(new MainMenu()));

        private void SelectTarget(object param)
        {
            if (param is BattleEnemy enemy)
            {
                TargetedEnemy = enemy;
            }
        }

        private void Attack()
        {
            var target = TargetedEnemy ?? Enemies.FirstOrDefault(e => e.Hp > 0);
            if (target != null)
            {
                _engine.PlayerAttack(target);
                UpdateProperties();
            }
        }

        private bool CanAttack() => IsPlayerTurn && Player?.CurrentAP >= 2 && Enemies.Any(e => e.Hp > 0);

        private void Defend()
        {
            _engine.PlayerDefend();
            UpdateProperties();
        }

        private bool CanDefend() => IsPlayerTurn && Player?.CurrentAP >= 2;

        private void UsePotionHp()
        {
            if (Player.CurrentAP < 1) return;
            Player.HpPotions--;
            Player.CurrentAP -= 1;
            Player.Hp = Math.Min(Player.MaxHp, Player.Hp + 40);
            Log("🧪 Вы восстановили 40 HP.");
            UpdateProperties();
        }

        private bool CanUsePotionHp() => IsPlayerTurn && Player?.HpPotions > 0 && Player?.CurrentAP >= 1;

        private void UsePotionMp()
        {
            if (Player.CurrentAP < 1) return;
            Player.MpPotions--;
            Player.CurrentAP -= 1;
            Player.Mp = Math.Min(Player.MaxMp, Player.Mp + 25);
            Log("💧 Вы восстановили 25 MP.");
            UpdateProperties();
        }

        private bool CanUsePotionMp() => IsPlayerTurn && Player?.MpPotions > 0 && Player?.CurrentAP >= 1;

        private void UseSkill(object param)
        {
            if (param is Skill skill)
            {
                var target = TargetedEnemy ?? Enemies.FirstOrDefault(e => e.Hp > 0);
                if (skill.TargetType == TargetType.SingleTarget && target == null)
                {
                    Log("Выберите цель для навыка.");
                    return;
                }

                _engine.ExecuteSkill(Player, skill, target);
                UpdateProperties();
            }
        }

        private bool CanUseSkill(object param)
        {
            if (!IsPlayerTurn || Player == null) return false;
            if (param is Skill skill)
            {
                return Player.CurrentAP >= skill.APCost && Player.Mp >= skill.ManaCost;
            }
            return false;
        }

        private void EndTurn()
        {
            _engine.EndPlayerTurn();
        }

        private void Log(string msg)
        {
            CombatLog.Add(new CombatLogMessage(msg));
        }

        private void UpdateProperties()
        {
            OnPropertyChanged(nameof(Player));
            UpdateCommandStates();
        }

        private void UpdateCommandStates()
        {
            (AttackCommand as Command)?.RaiseCanExecuteChanged();
            (DefendCommand as Command)?.RaiseCanExecuteChanged();
            (UsePotionHpCommand as Command)?.RaiseCanExecuteChanged();
            (UsePotionMpCommand as Command)?.RaiseCanExecuteChanged();
            (EndTurnCommand as Command)?.RaiseCanExecuteChanged();
            (UseSkillCommand as Command)?.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CombatLogMessage
    {
        public string Text { get; set; } = string.Empty;
        public string TextColor { get; set; } = "#DDDDDD";

        public CombatLogMessage(string text)
        {
            Text = text;
            if (text.Contains("урона") && !text.Contains("восстанавливает"))
                TextColor = "#FF5555"; // Красный урон
            else if (text.Contains("восстанавливает") || text.Contains("лечит"))
                TextColor = "#55FF55"; // Зеленый лечение
            else if (text.Contains("маны") || text.Contains("MP"))
                TextColor = "#5555FF"; // Синий мана
            else if (text.Contains("получает") || text.Contains("эффект") || text.Contains("ослепляет") || text.Contains("оглушен") || text.Contains("ярость"))
                TextColor = "#FFFF55"; // Желтый статусы
        }
    }
}
