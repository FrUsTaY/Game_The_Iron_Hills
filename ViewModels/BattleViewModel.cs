using EpicBattle.Models.Encounters;
using System.Windows;
using EpicBattle.Models.Items;
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
using EpicBattle.Services;
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
        private int _currentWave = 1;
        private int _totalWaves = 1;
        private Action _onCompletedFromMap;

        // P0-фикс (Б2): постоянство HP/MP между сценами сюжетного режима.
        // Полный отдых в сюжете разрешён только на «безопасных» узлах (лагеря, привалы),
        // но НЕ перед каждым боем — иначе ставки за весь Акт обнулялись.
        private static readonly HashSet<string> StoryRestNodes = new HashSet<string>
        {
            "Start", "Scene3_Varg", "Act2_Kael_Freed"
        };

        public BattlePlayer Player => _engine?.Player;
        public ObservableCollection<BattleEnemy> Enemies { get; private set; } = new ObservableCollection<BattleEnemy>();
        public ObservableCollection<CombatLogMessage> CombatLog { get; private set; } = new ObservableCollection<CombatLogMessage>();
        public ObservableCollection<Skill> PlayerSkills { get; private set; } = new ObservableCollection<Skill>();

        // Этап 3.3: Floating Combat Text — всплывающие цифры над колонками игрока/врагов.
        public ObservableCollection<FloatingText> FloatingTexts { get; private set; } = new ObservableCollection<FloatingText>();
        private bool _lastWasCrit;
        private const string PlayerFloatKey = "Player";

        // Этап 3.3: вспышка на колонке игрока при получении удара (импульс true→false, перезапускает Storyboard)
        private bool _playerHitPulse;
        public bool PlayerHitPulse
        {
            get => _playerHitPulse;
            set { _playerHitPulse = value; OnPropertyChanged(nameof(PlayerHitPulse)); }
        }

        /// <summary>Этап 3.3: сигнал code-behind подсветить карточку врага (имя цели).</summary>
        public event Action<string> EnemyHitFlashRequested;

        private void PulsePlayerHit()
        {
            if (_playerHitPulse) return;
            PlayerHitPulse = true;
            App.Current.Dispatcher.InvokeAsync(() => PlayerHitPulse = false);
        }

        private void PushFloating(string msg)
        {
            try
            {
                if (FloatingTextParser.IsCritMarker(msg)) { _lastWasCrit = true; return; }
                var ft = FloatingTextParser.Parse(msg, PlayerFloatKey, Player?.Name);
                if (ft == null) { _lastWasCrit = false; return; }

                if (_lastWasCrit)
                {
                    // пересоздаём как крит: жёлтый, крупный; тряска при тяжёлом ударе по игроку
                    ft = new FloatingText(ft.Text, FloatingTextKind.Crit, ft.TargetKey, triggersShake: ft.TriggersShake || ft.TargetKey == PlayerFloatKey);
                }
                _lastWasCrit = false;

                FloatingTexts.Add(ft);
                if (FloatingTexts.Count > 24)
                    FloatingTexts.RemoveAt(0);

                // Этап 3.3: вспышка на колонке игрока при входящем уроне/магии
                if ((ft.Kind == FloatingTextKind.Damage || ft.Kind == FloatingTextKind.Magic || ft.Kind == FloatingTextKind.Crit))
                {
                    if (ft.TargetKey == PlayerFloatKey) PulsePlayerHit();
                    else EnemyHitFlashRequested?.Invoke(ft.TargetKey);
                }
            }
            catch { /* FX не должны ронять бой */ }
        }

        public string WaveText => _battleModifier == "Elite" 
            ? "💀 ЭЛИТНЫЙ БОЙ" 
            : (_battleModifier == "Boss" 
                ? "👑 БИТВА С БОССОМ" 
                : (_totalWaves > 1 
                    ? $"⚔️ Волна {_currentWave} из {_totalWaves}" 
                    : (_isArcade ? $"⚔️ Этаж {_arcadeLevel}" : "⚔️ Сюжетный бой")));

        private BattleEnemy _targetedEnemy;
        public BattleEnemy TargetedEnemy
        {
            get => _targetedEnemy;
            set
            {
                if (_targetedEnemy != null) _targetedEnemy.IsTargeted = false;
                _targetedEnemy = value;
                if (_targetedEnemy != null) _targetedEnemy.IsTargeted = true;

                OnPropertyChanged();
                UpdateCommandStates();
            }
        }

        public bool IsPlayerTurn => _engine?.TurnQueue.FirstOrDefault() is BattlePlayer;

        private string _statusMessage = "Приготовьтесь к бою!";
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

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

        // P0-фикс (Б1): сводка для экрана поражения
        private string _defeatSummaryText = string.Empty;
        public string DefeatSummaryText
        {
            get => _defeatSummaryText;
            set { _defeatSummaryText = value; OnPropertyChanged(); }
        }

        // P0-фикс (В4): кулдаун навыка игрока готов? (для блокировки кнопок в UI)
        public bool IsSkillReady(Skill skill)
        {
            if (skill == null) return false;
            return _engine == null || !_engine.TryGetPlayerCooldown(skill.Id, out int remaining) || remaining <= 0;
        }

        public bool IsStoryMode => !_isArcade && _onCompletedFromMap == null;
        public bool IsArcadeMode => _isArcade && _onCompletedFromMap == null;
        public bool IsMapMode => _onCompletedFromMap != null || (_gameState != null && _gameState.IsMapMode);

        // RPG Характеристики игрока
        public int StatPoints => _gameState.StatPoints;
        public int SkillPoints => _gameState.SkillPoints;
        public int PlayerLevel => _gameState.PlayerLevel;
        public int Strength => _gameState.Strength;
        public int Dexterity => _gameState.Dexterity;
        public int Intelligence => _gameState.Intelligence;
        public int Endurance => _gameState.Endurance;

        public int PlayerMaxHp => _gameState.PlayerMaxHp;
        public int PlayerMaxMp => _gameState.PlayerMaxMp;
        public int PlayerBaseDamage => _gameState.PlayerBaseDamage;
        public int PlayerBaseMagicDamage => _gameState.PlayerBaseMagicDamage;
        public int Defense => _gameState.Defense;
        public double DodgeChance => _gameState.DodgeChance;
        public double CritChance => _gameState.CritChance;

        public System.Windows.Media.Imaging.BitmapImage? BattleBackground { get; private set; }
        public System.Windows.Media.Imaging.BitmapImage? PlayerAvatar => EpicBattle.Services.ImageService.GetCharacterImage(_gameState?.PlayerClass == "Изгой-маг" ? "hero_mage" : _gameState?.PlayerClass == "Наемник" ? "hero_mercenary" : "hero_veteran");

        public BattleViewModel(bool isArcade, string returnNodeId = null, string battleModifier = null, int arcadeLevel = 1, Action onCompletedFromMap = null)
        {
            _isArcade = isArcade;
            _arcadeLevel = arcadeLevel;
            _returnNodeId = returnNodeId;
            _battleModifier = battleModifier;
            _onCompletedFromMap = onCompletedFromMap;
            _gameState = SaveManager.CurrentState;

            if (_isArcade && returnNodeId == null && onCompletedFromMap == null && !_gameState.IsMapMode)
            {
                _gameState.PlayerHp = _gameState.PlayerMaxHp;
                _gameState.PlayerMp = _gameState.PlayerMaxMp;
            }

            int currentFloor = _gameState?.ActiveMap?.CurrentFloor ?? 0;
            BattleBackground = EpicBattle.Services.ImageService.GetBattleBackground(_returnNodeId, _isArcade, _gameState?.IsMapMode == true, currentFloor);
            StartNewBattle();
        }

        private void StartNewBattle()
        {
            CombatLog.Clear();
            Enemies.Clear();
            PlayerSkills.Clear();

            _currentWave = 1;
            if (_battleModifier == "Elite" || _battleModifier == "Boss")
            {
                _totalWaves = 1;
            }
            else if (_isArcade)
            {
                _totalWaves = EnemyFactory.GetArcadeTotalWaves(_arcadeLevel);
            }
            else
            {
                _totalWaves = EnemyFactory.GetStoryTotalWaves(_returnNodeId);
                _gameState.PlayerHp = _gameState.PlayerMaxHp;
                _gameState.PlayerMp = _gameState.PlayerMaxMp;
            }

            OnPropertyChanged(nameof(WaveText));

            var player = new BattlePlayer(_gameState);
            foreach (var s in player.UnlockedSkills)
            {
                if (s.Type == SkillType.Active && !PlayerSkills.Any(ps => ps.Id == s.Id))
                    PlayerSkills.Add(s);
            }

            List<BattleEnemy> enemiesList;
            if (_battleModifier == "Elite")
            {
                enemiesList = EnemyFactory.GenerateEliteWave(_gameState.CurrentAct > 0 ? _gameState.CurrentAct : 1, _gameState.PlayerLevel);
            }
            else if (_battleModifier == "Boss")
            {
                enemiesList = EnemyFactory.GenerateBossWave(_gameState.CurrentAct > 0 ? _gameState.CurrentAct : 1, _gameState.PlayerLevel);
            }
            else if (_isArcade)
            {
                enemiesList = EnemyFactory.GenerateArcadeWave(_arcadeLevel, _currentWave, _totalWaves);
            }
            else
            {
                enemiesList = EnemyFactory.GenerateStoryWave(_returnNodeId, _battleModifier, _currentWave);
            }

            foreach (var e in enemiesList)
                Enemies.Add(e);

            TargetedEnemy = Enemies.FirstOrDefault();

            _engine = new BattleEngine(player, enemiesList, _currentWave, _totalWaves, _battleModifier);

            foreach (var e in enemiesList)
                e.DetermineIntent(enemiesList, player);

            _engine.OnLogEvent += (msg) =>
            {
                // Этап 3.3: звук синтезируется/подбирается до диспатчера — не блокирует UI-поток.
                AudioEngine.PlayForLogLine(msg);
                App.Current.Dispatcher.Invoke(() =>
                {
                    PushFloating(msg);
                    CombatLog.Add(new CombatLogMessage(msg));
                });
            };

            _engine.OnWaveCompleted += () =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (_currentWave < _totalWaves)
                    {
                        _currentWave++;
                        OnPropertyChanged(nameof(WaveText));

                        var nextWaveEnemies = _isArcade
                            ? EnemyFactory.GenerateArcadeWave(_arcadeLevel, _currentWave, _totalWaves)
                            : EnemyFactory.GenerateStoryWave(_returnNodeId, _battleModifier, _currentWave);

                        _engine.SpawnNextWave(nextWaveEnemies, _currentWave);

                        Enemies.Clear();
                        foreach (var e in nextWaveEnemies)
                            Enemies.Add(e);

                        TargetedEnemy = Enemies.FirstOrDefault();
                        OnPropertyChanged(nameof(Enemies));
                        UpdateCommandStates();
                    }
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

                    var deadEnemies = Enemies.Where(e => e.Hp <= 0).ToList();
                    foreach (var dead in deadEnemies)
                    {
                        Enemies.Remove(dead);
                        _engine.Enemies.Remove(dead);
                    }

                    if (TargetedEnemy != null && TargetedEnemy.Hp <= 0)
                        TargetedEnemy = Enemies.FirstOrDefault(e => e.Hp > 0);

                    StatusMessage = IsPlayerTurn ? "Ваш ход! Выберите действие." : "Ход противников...";
                    CheckBattleEnd();
                });
            };

            StatusMessage = "Бой начался!";
            _engine.NextTurn();
        }

        private void CheckBattleEnd()
        {
            if (_engine.IsBattleOver())
            {
                if (_engine.Player.Hp > 0)
                {
                    Log("🏆 Вы победили во всех волнах!");
                    StatusMessage = "Победа!";
                    AudioEngine.Play("victory");
                    _engine.Player.UpdateGameState(_gameState);

                    bool isBoss = _returnNodeId == "PostBattle_Scene4" || _returnNodeId == "Act2_Ending_Decisions" 
                                  || (_isArcade && _arcadeLevel % 3 == 0) || _battleModifier == "Boss";
                    bool isElite = _battleModifier == "Elite";

                    // P0-баланс: Начисление опыта вместо плоского +1 уровня за бой
                    int expEarned = isBoss ? (160 + _gameState.PlayerLevel * 15)
                                  : (isElite ? (90 + _gameState.PlayerLevel * 8)
                                             : (45 + _gameState.PlayerLevel * 5));
                    _gameState.PlayerExperience += expEarned;

                    int levelsGained = 0;
                    while (_gameState.PlayerExperience >= _gameState.ExperienceToNextLevel)
                    {
                        _gameState.PlayerExperience -= _gameState.ExperienceToNextLevel;
                        _gameState.PlayerLevel += 1;
                        _gameState.StatPoints += 2;
                        // Навык выдаётся на каждом 2-м уровне (2, 4, 6, 8, 10...)
                        if (_gameState.PlayerLevel % 2 == 0)
                        {
                            _gameState.SkillPoints += 1;
                        }
                        _gameState.ExperienceToNextLevel = 100 + (_gameState.PlayerLevel - 1) * 75;
                        levelsGained++;
                    }

                    // Генерация лута после победы (однократное умножение золота)
                    int goldEarned = 0;
                    var dropped = LootGenerator.GenerateBattleLoot(_gameState.PlayerLevel, isBoss, isElite, out goldEarned);
                    _gameState.Gold += goldEarned;

                    // Этап 3.2: руда для кузнеца падает с врагов в горах Кряжа
                    int oreFound = LootGenerator.RollOreDrop(isBoss || isElite);
                    _gameState.Ore += oreFound;

                    // Этап 3.2 (часть 2): рунические камни — дроп с элит/боссов
                    bool runeDropped = false; string runeName = "", runeIcon = "";
                    if (isElite || isBoss)
                    {
                        if (LootGenerator.RollRuneDrop(isBoss, out string rid, out runeName, out runeIcon))
                        {
                            _gameState.AddRune(rid);
                            runeDropped = true;
                        }
                    }

                    DroppedLoot.Clear();
                    foreach (var item in dropped)
                    {
                        _gameState.Inventory.Add(item);
                        DroppedLoot.Add(item);
                    }

                    string levelUpNotice = levelsGained > 0 ? $"\n🎉 НОВЫЙ УРОВЕНЬ: {_gameState.PlayerLevel}! (+{levelsGained * 2} характеристик)" : "";
                    LootSummaryText = $"✨ Опыт: +{expEarned} ({_gameState.PlayerExperience}/{_gameState.ExperienceToNextLevel} XP)"
                        + levelUpNotice
                        + $"\n💰 Золото: +{goldEarned} (Всего: {_gameState.Gold})"
                        + (dropped.Count > 0 ? $", Найдено предметов: +{dropped.Count}" : "")
                        + (oreFound > 0 ? $", ⛏️ Руда: +{oreFound} (Всего: {_gameState.Ore})" : "")
                        + (runeDropped ? $"\n💠 Найден рунический камень: {runeIcon} {runeName}!" : "");

                    // P0-фикс (Б2): полное восстановление HP/MP после боя было только в сюжетном режиме
                    // (там же оно принудительно ставилось перед боем). Для карты и аркады HP теперь
                    // переносится между боями — узлы Костёр/Лавка становятся осмысленными.
                    if (!_isArcade && _onCompletedFromMap == null && !_gameState.IsMapMode)
                    {
                        _gameState.PlayerHp = _gameState.PlayerMaxHp;
                        _gameState.PlayerMp = _gameState.PlayerMaxMp;
                    }
                    else
                    {
                        // Щадящий режим: после победы игрок не остаётся на 1-2 HP —
                        // минимум 25% здоровья, чтобы карта не превращалась в unverifiable death-spiral.
                        int floor = Math.Max(1, (int)(_gameState.PlayerMaxHp * 0.25));
                        if (_gameState.PlayerHp < floor) _gameState.PlayerHp = floor;
                    }

                    IsUpgradeOverlayVisible = true;
                    UpdateStatsDisplay();
                    OnPropertyChanged(nameof(IsStoryMode));
                    OnPropertyChanged(nameof(IsArcadeMode));
                    OnPropertyChanged(nameof(IsMapMode));
                }
                else
                {
                    Log("💀 Вы погибли...");
                    AudioEngine.Play("death");
                    StatusMessage = "Поражение";
                    IsPlayerDead = true;

                    // P0-фикс (Б1): поражение откатывает бой, но не весь прогресс.
                    // Если игра шла из сохранения — автоматически перезаписываем слот состоянием
                    // на момент загрузки (последний контрольный пункт). Без этого игрок терял бы
                    // лут/золото/прогресс карты, заработанные после ручного сохранения.
                    try
                    {
                        if (!string.IsNullOrEmpty(_gameState.SaveName) && _gameState.SaveName != "Новое сохранение")
                        {
                            SaveManager.SaveGame(_gameState.SaveName);
                            Log("💾 Прогресс восстановлен по последнему сохранению.");
                        }
                        else
                        {
                            Log("⚠️ Автосохранение недоступно: бой начат без загруженного сейва.");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Defeat autosave: {ex.Message}");
                    }

                    UpdateCommandStates();
                }
            }
        }

        public ICommand AttackCommand => new Command(_ => Attack(), _ => CanAttack());
        public ICommand DefendCommand => new Command(_ => Defend(), _ => CanDefend());
        public ICommand UsePotionHpCommand => new Command(_ => UsePotionHp(), _ => CanUsePotionHp());
        public ICommand UsePotionMpCommand => new Command(_ => UsePotionMp(), _ => CanUsePotionMp());
        public ICommand EndTurnCommand => new Command(_ => EndTurn(), _ => IsPlayerTurn && !IsPlayerDead);
        public ICommand SelectTargetCommand => new Command(SelectTarget);
        public ICommand UseSkillCommand => new Command(UseSkill, CanUseSkill);
        public ICommand ReturnToMenuCommand => new Command(_ => { AudioEngine.Play("click_alt"); NavigationManager.NavigateTo(new MainMenu()); });

        public ICommand NextBattleCommand => new Command(_ => NextBattle());
        public ICommand ContinueStoryCommand => new Command(_ => ContinueStory());
        public ICommand ReturnToMapCommand => new Command(_ => ReturnToMap());
        public ICommand OpenSkillTreeCommand => new Command(_ => OpenSkillTree());
        public ICommand IncreaseStatCommand => new Command(IncreaseStat, CanIncreaseStat);

        public Action RequestOpenSkillTree { get; set; }
        public ObservableCollection<Item> DroppedLoot { get; private set; } = new ObservableCollection<Item>();

        private string _lootSummaryText = string.Empty;
        public string LootSummaryText
        {
            get => _lootSummaryText;
            set { _lootSummaryText = value; OnPropertyChanged(); }
        }

        public Action RequestOpenInventory { get; set; }
        public ICommand OpenInventoryCommand => new Command(_ => RequestOpenInventory?.Invoke());

        private void IncreaseStat(object param)
        {
            AudioEngine.Play("click");
            if (_gameState.StatPoints <= 0) return;
            string stat = param?.ToString();
            if (stat == "STR") _gameState.Strength++;
            else if (stat == "DEX") _gameState.Dexterity++;
            else if (stat == "INT") _gameState.Intelligence++;
            else if (stat == "END") _gameState.Endurance++;

            _gameState.StatPoints--;
            _gameState.RecalculateDerivedStats();
            UpdateStatsDisplay();
        }

        private bool CanIncreaseStat(object param) => _gameState.StatPoints > 0;

        public void UpdateStatsDisplay()
        {
            OnPropertyChanged(nameof(StatPoints));
            OnPropertyChanged(nameof(SkillPoints));
            OnPropertyChanged(nameof(PlayerLevel));
            OnPropertyChanged(nameof(Strength));
            OnPropertyChanged(nameof(Dexterity));
            OnPropertyChanged(nameof(Intelligence));
            OnPropertyChanged(nameof(Endurance));
            OnPropertyChanged(nameof(PlayerMaxHp));
            OnPropertyChanged(nameof(PlayerMaxMp));
            OnPropertyChanged(nameof(PlayerBaseDamage));
            OnPropertyChanged(nameof(PlayerBaseMagicDamage));
            OnPropertyChanged(nameof(Defense));
            OnPropertyChanged(nameof(DodgeChance));
            OnPropertyChanged(nameof(CritChance));
            (IncreaseStatCommand as Command)?.RaiseCanExecuteChanged();
        }

        public void UpdateSkillPointsDisplay()
        {
            UpdateStatsDisplay();
            if (Player != null)
            {
                PlayerSkills.Clear();
                foreach (var id in _gameState.UnlockedSkills)
                {
                    var skill = SkillsDatabase.AllSkills.Find(s => s.Id == id);
                    if (skill != null && skill.Type == SkillType.Active && !PlayerSkills.Any(ps => ps.Id == skill.Id))
                        PlayerSkills.Add(skill);
                }
            }
        }

        private void NextBattle()
        {
            AudioEngine.Play("sword_unsheath");
            IsUpgradeOverlayVisible = false;
            _arcadeLevel++;

            if (_onCompletedFromMap != null)
            {
                _onCompletedFromMap.Invoke();
                return;
            }

            var randomEncounter = EncountersDatabase.GetRandomEncounter();
            NavigationManager.NavigateTo(new EncounterView(randomEncounter, () =>
            {
                NavigationManager.NavigateTo(new BattleView(isArcade: true, arcadeLevel: _arcadeLevel));
            }));
        }

        private void ReturnToMap()
        {
            AudioEngine.Play("footsteps");
            IsUpgradeOverlayVisible = false;
            if (_onCompletedFromMap != null)
            {
                _onCompletedFromMap.Invoke();
            }
            else
            {
                NavigationManager.NavigateTo(new MapView());
            }
        }

        private void ContinueStory()
        {
            AudioEngine.Play("click");
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

        private void SelectTarget(object param)
        {
            AudioEngine.Play("click");
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
                // Этап 3.3: уникальный звук удара по типу экипированного оружия (клинок/посох/кинжалы/топор).
                AudioEngine.PlayWeaponHit(_gameState?.EquippedWeapon?.Name);
                _engine.PlayerAttack(target);
                UpdateProperties();
            }
        }

        private bool CanAttack() => IsPlayerTurn && Player?.CurrentAP >= 2 && Enemies.Any(e => e.Hp > 0);

        private void Defend()
        {
            AudioEngine.Play("defend");
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
            AudioEngine.Play("waterspray");
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
            AudioEngine.Play("firebuff");
            UpdateProperties();
        }

        private bool CanUsePotionMp() => IsPlayerTurn && Player?.MpPotions > 0 && Player?.CurrentAP >= 1;

        private void UseSkill(object param)
        {
            if (param is Skill skill)
            {
                if (_engine != null && _engine.TryGetPlayerCooldown(skill.Id, out int remainingCd) && remainingCd > 0)
                {
                    AudioEngine.Play("click_alt");
                    Log($"⏳ Навык '{skill.Name}' перезаряжается (осталось ходов: {remainingCd}).");
                    return;
                }

                if (Player.CurrentAP < skill.APCost)
                {
                    AudioEngine.Play("click_alt");
                    Log($"⚡ Недостаточно AP для '{skill.Name}'! Требуется {skill.APCost} AP (у вас {Player.CurrentAP} AP).");
                    return;
                }

                if (Player.Mp < skill.ManaCost)
                {
                    AudioEngine.Play("click_alt");
                    Log($"💧 Недостаточно маны для '{skill.Name}'! Требуется {skill.ManaCost} MP (у вас {Player.Mp} MP).");
                    return;
                }

                var target = TargetedEnemy ?? Enemies.FirstOrDefault(e => e.Hp > 0);
                if (skill.TargetType == TargetType.SingleTarget && target == null)
                {
                    Log("Выберите цель для навыка.");
                    return;
                }

                string sName = (skill.Name ?? "").ToLowerInvariant();
                if (sName.Contains("огн") || sName.Contains("плам") || sName.Contains("fire")) AudioEngine.Play("fireball");
                else if (sName.Contains("лед") || sName.Contains("мороз") || sName.Contains("ice")) AudioEngine.Play("ice_freeze");
                else if (sName.Contains("кам") || sName.Contains("метеор") || sName.Contains("rock")) AudioEngine.Play("rock_throw");
                else if (sName.Contains("исцел") || sName.Contains("heal")) AudioEngine.Play("waterspray");
                else if (sName.Contains("щит") || sName.Contains("блок")) AudioEngine.Play("defend");
                else AudioEngine.Play("sword_attack");
                _engine.ExecuteSkill(Player, skill, target);
                UpdateProperties();
            }
        }

        private bool CanUseSkill(object param)
        {
            if (!IsPlayerTurn || Player == null) return false;
            return true;
        }

        private void EndTurn()
        {
            AudioEngine.Play("click");
            // P2-фикс (В9): если у игрока остались неподготовленные AP, предупреждаем —
            // они сгорят при завершении хода (раньше терялись молча).
            if (_engine != null && IsPlayerTurn && _engine.Player.CurrentAP > 0)
            {
                Log($"⚠️ Сгорает {_engine.Player.CurrentAP} неподготовленных AP. Совет: тратьте AP на атаки/навыки до конца хода.");
            }
            _engine.EndPlayerTurn();
        }

        private void Log(string msg)
        {
            CombatLog.Add(new CombatLogMessage(msg));
            PushFloating(msg); // Этап 3.3: FX из ручных сообщений VM (зелья и т.п.)
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
        public string TextColor { get; set; } = "#E0E0E0";

        public CombatLogMessage(string text, string color = null)
        {
            Text = text;
            if (!string.IsNullOrEmpty(color))
            {
                TextColor = color;
            }
            else
            {
                if (text.Contains("КРИТИЧЕСКИЙ УДАР") || text.Contains("💥")) TextColor = "#FF5555";
                else if (text.Contains("атакуете") || text.Contains("⚔️")) TextColor = "#FFAA00";
                else if (text.Contains("Ходит") || text.Contains("наносит вам")) TextColor = "#FF8888";
                else if (text.Contains("уклоняется") || text.Contains("уклонился") || text.Contains("💨")) TextColor = "#55FFFF";
                else if (text.Contains("восстановили") || text.Contains("🧪") || text.Contains("💧")) TextColor = "#55FF55";
                else if (text.Contains("победили") || text.Contains("🏆")) TextColor = "#FFFF55";
                else if (text.Contains("погибли") || text.Contains("💀")) TextColor = "#FF3333";
                else TextColor = "#E0E0E0";
            }
        }
    }
}
