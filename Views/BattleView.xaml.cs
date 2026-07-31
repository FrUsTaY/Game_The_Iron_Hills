using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using EpicBattle.Managers;
using EpicBattle.Models;

namespace EpicBattle.Views
{
    public partial class BattleView : UserControl
    {
        private bool _isArcade;
        private string _returnNodeId;
        private string _battleModifier;

        // Враг
        private int enemyMaxHp;
        private int enemyHp;
        private int enemyDamage;
        private bool isDefending = false;

        // Волны и ходы
        private int _currentWave = 1;
        private int _maxWaves = 1;
        private int _totalTurnsPassed = 0;

        // Модификаторы сюжета
        private bool _orcCannotBlock = false;
        private int _playerDamageBuffTurns = 0;
        private bool _enemiesBurn = false;
        private bool _vargSkipTurn = false;
        private bool _vargAggro = false;
        private bool _enemiesBuffed = false;

        // Босс Громгар
        private bool _isGromgarEncounter = false;
        private int _gromgarRageThreshold = 30;
        private bool _gromgarEnraged = false;
        private double _bloodlustMultiplier = 1.0;
        private bool _gromgarChaosAttack = false;

        private Random random = new Random();

        // Эффекты навыков
        private int _enemyBleedTurns = 0;
        private int _enemyBurnTurns = 0;
        private int _manaShieldTurns = 0;

        // Аудио
        private MediaPlayer bgmPlayer = new MediaPlayer();
        private List<MediaPlayer> sfxPlayers = new List<MediaPlayer>();

        public BattleView(bool isArcade = false, bool isLoaded = false, string returnNodeId = "", string battleModifier = "")
        {
            InitializeComponent();
            _isArcade = isArcade;
            _returnNodeId = returnNodeId;
            _battleModifier = battleModifier;

            this.Loaded += (s, e) => this.Focus();

            if (!isArcade)
            {
                PauseSaveBtn.Visibility = Visibility.Collapsed; // В бою истории не сохраняемся, только в диалогах
            }
            else
            {
                PauseSaveBtn.Visibility = Visibility.Collapsed; // В аркаде тоже не сохраняемся (это рогалик сессия)
                if (!isLoaded)
                {
                    // Сброс стейта для новой аркады
                    SaveManager.CurrentState = new GameState { IsStoryMode = false };
                }
            }

            InitializeAudio();
            GenerateEnemy();
            ApplyBattleModifiers();
            UpdateUI();

            if (_isArcade)
            {
                EventLog.Text = "Аркада началась! Выживайте как можно дольше.";
            }
        }

        private void ApplyBattleModifiers()
        {
            if (_isArcade) return;

            EventLog.Text = "Сюжетный бой начинается!\n";

            if (_battleModifier == "Surprise")
            {
                int dmg = (int)(enemyMaxHp * 0.2);
                enemyHp -= dmg;
                EventLog.Text += $"💥 Внезапный удар! Вы наносите {dmg} урона.\n";
            }
            else if (_battleModifier == "Pragmatic")
            {
                _playerDamageBuffTurns = 1;
                EventLog.Text += $"👁 Углук теряет бдительность (уязвим на 1 ход).\n";
            }
            else if (_battleModifier == "Aggressive")
            {
                _orcCannotBlock = true;
                enemyDamage = (int)(enemyDamage * 1.15);
                EventLog.Text += $"💢 Углук в ярости! Больше урона, но не может блокировать.\n";
            }
            else if (_battleModifier.StartsWith("Varg"))
            {
                _maxWaves = 3;

                if (_battleModifier == "Varg_Tactical")
                {
                    _enemiesBurn = true;
                    _vargSkipTurn = true;
                    enemyMaxHp = (int)(enemyMaxHp * 0.7);
                    enemyHp = enemyMaxHp;
                    EventLog.Text += "🔥 Костер взрывается! Орки получают ожоги, шаман ошеломлен!\n";
                }
                else if (_battleModifier == "Varg_Provocative")
                {
                    _vargAggro = true;
                    EventLog.Text += "🤬 Шаман взбешен! Он атакует вас магией из-за спин орков!\n";
                    int dmg = random.Next(15, 25);
                    SaveManager.CurrentState.PlayerHp -= dmg;
                    EventLog.Text += $"✨ Огненный шар Шамана наносит вам {dmg} урона!\n";
                }
                else if (_battleModifier == "Varg_Honorable")
                {
                    _enemiesBuffed = true;
                    enemyDamage = (int)(enemyDamage * 1.1);
                    EventLog.Text += "⚔️ Честный бой! Враги воодушевлены (+10% урон).\n";
                }
            }
            else if (_battleModifier.StartsWith("Gromgar"))
            {
                _isGromgarEncounter = true;

                if (_battleModifier == "Gromgar_Honor")
                {
                    _gromgarRageThreshold = 40;
                    EventLog.Text += "🛡 Слова о чести заставляют Громгара сомневаться.\n(Его шанс крита снижен, но Ярость наступит раньше).\n";
                }
                else if (_battleModifier == "Gromgar_Lore")
                {
                    int dmg = (int)(enemyMaxHp * 0.15);
                    enemyHp -= dmg;
                    _gromgarChaosAttack = true;
                    EventLog.Text += $"🔥 Алтарь взрывается! Громгар получает {dmg} урона.\n";
                }
                else if (_battleModifier == "Gromgar_Ruthless")
                {
                    _bloodlustMultiplier = 1.2;
                    EventLog.Text += "🩸 ЖАЖДА КРОВИ! Вы и Громгар наносите больше урона, но защита снижена.\n";
                }
            }
        }

        private void InitializeAudio()
        {
            for (int i = 0; i < 5; i++)
            {
                var player = new MediaPlayer();
                player.MediaEnded += (s, e) => { player.Stop(); player.Position = TimeSpan.Zero; };
                sfxPlayers.Add(player);
            }

            try
            {
                string bgmPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", "bgm.wav");
                if (File.Exists(bgmPath))
                {
                    bgmPlayer.Open(new Uri(bgmPath));
                    bgmPlayer.Volume = SaveManager.Settings.MusicVolume;
                    bgmPlayer.MediaEnded += (s, e) => { bgmPlayer.Position = TimeSpan.Zero; bgmPlayer.Play(); };
                    bgmPlayer.Play();
                }
            }
            catch { }
        }

        private void PlaySound(string fileName)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", fileName);
                if (File.Exists(path))
                {
                    foreach (var player in sfxPlayers)
                    {
                        if (player.Position == TimeSpan.Zero)
                        {
                            player.Open(new Uri(path));
                            player.Volume = SaveManager.Settings.SfxVolume;
                            player.Play();
                            return;
                        }
                    }
                    sfxPlayers[0].Stop();
                    sfxPlayers[0].Open(new Uri(path));
                    sfxPlayers[0].Volume = SaveManager.Settings.SfxVolume;
                    sfxPlayers[0].Play();
                }
            }
            catch { }
        }

        private void Log(string message)
        {
            EventLog.Text += $"\n{message}";
            LogScrollViewer.ScrollToEnd();
        }

        private void ApplyClassPassivesPerTurn(Models.GameState state)
        {
            if (state.PlayerClass == "Изгой-маг")
            {
                if (state.PlayerMp < state.PlayerMaxMp)
                {
                    state.PlayerMp = Math.Min(state.PlayerMaxMp, state.PlayerMp + 2);
                    // Не будем спамить в лог каждый ход, просто восстанавливаем ману
                }
            }

            // M3_ArcaneFocus (Концентрация) - восстанавливает +5 MP каждый ход
            if (state.UnlockedSkills.Contains("M3_ArcaneFocus"))
            {
                if (state.PlayerMp < state.PlayerMaxMp)
                {
                    state.PlayerMp = Math.Min(state.PlayerMaxMp, state.PlayerMp + 5);
                }
            }
        }

        private void UpdateUI()
        {
            var state = SaveManager.CurrentState;

            PlayerLevelText.Text = $"⚔️ {state.PlayerName} - {state.PlayerClass} (Уровень {state.PlayerLevel})";
            PlayerHpText.Text = $"HP: {state.PlayerHp} / {state.PlayerMaxHp}";
            PlayerHpBar.Value = state.PlayerHp;
            PlayerHpBar.Maximum = state.PlayerMaxHp;

            PlayerMpText.Text = $"MP: {state.PlayerMp} / {state.PlayerMaxMp}";
            PlayerMpBar.Value = state.PlayerMp;
            PlayerMpBar.Maximum = state.PlayerMaxMp;

            EnemyHpText.Text = $"HP: {enemyHp} / {enemyMaxHp}";
            EnemyHpBar.Value = enemyHp;
            EnemyHpBar.Maximum = enemyMaxHp;

            HpPotionText.Text = $"🧪 Зелья HP: {state.HpPotions}";
            MpPotionText.Text = $"💧 Зелья MP: {state.MpPotions}";

            UpdateActionBar();
            SetButtonsEnabled(true); // Обновит доступность
        }

        private void UpdateActionBar()
        {
            ActionBarPanel.Children.Clear();
            var state = SaveManager.CurrentState;
            var allSkills = EpicBattle.Models.RPG.SkillsDatabase.AllSkills;

            foreach (var skillId in state.UnlockedSkills)
            {
                var skill = allSkills.Find(s => s.Id == skillId);
                if (skill != null && skill.Type == EpicBattle.Models.RPG.SkillType.Active)
                {
                    var btn = new Button
                    {
                        Content = $"{skill.Name} ({skill.ManaCost} MP)",
                        MinWidth = 120,
                        Padding = new Thickness(10, 5, 10, 5),
                        Height = 40,
                        Margin = new Thickness(5),
                        Tag = skill
                    };
                    btn.Click += SkillBtn_Click;
                    ActionBarPanel.Children.Add(btn);
                }
            }
        }

        private async void SkillBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is EpicBattle.Models.RPG.Skill skill)
            {
                var state = SaveManager.CurrentState;
                if (state.PlayerMp >= skill.ManaCost)
                {
                    state.PlayerMp -= skill.ManaCost;

                    // Упрощенная реализация эффектов скиллов для примера
                    if (skill.Id == "S1_PowerStrike")
                    {
                        int dmg = (int)(state.PlayerBaseDamage * 1.5);
                        enemyHp -= dmg;
                        Log($"⚔️ Вы используете '{skill.Name}' и наносите {dmg} урона!");
                    }
                    else if (skill.Id == "S2_Cleave")
                    {
                        int dmg = state.PlayerBaseDamage;
                        enemyHp -= dmg;
                        _enemyBleedTurns = 3;
                        Log($"⚔️ Вы используете '{skill.Name}'! Враг получает {dmg} урона и будет истекать кровью.");
                    }
                    else if (skill.Id == "M1_Fireball")
                    {
                        int dmg = (int)(state.PlayerBaseMagicDamage * 1.5);
                        enemyHp -= dmg;
                        _enemyBurnTurns = 2;
                        Log($"🔥 Вы используете '{skill.Name}'! Враг получает {dmg} урона и горит.");
                    }
                    else if (skill.Id == "M2_ManaShield")
                    {
                        _manaShieldTurns = 3;
                        Log($"🛡️ Вы используете '{skill.Name}'! Часть урона будет поглощаться маной.");
                    }
                    else if (skill.Id == "V1_FirstAid")
                    {
                        int heal = 30;
                        state.PlayerHp = Math.Min(state.PlayerMaxHp, state.PlayerHp + heal);
                        Log($"🩹 Вы используете '{skill.Name}'. Восстановлено {heal} HP.");
                    }

                    UpdateUI();
                    if (enemyHp <= 0) { enemyHp = 0; await EnemyTurnAsync(); return; }
                    await EnemyTurnAsync();
                }
                else
                {
                    Log("Недостаточно маны!");
                }
            }
        }

        private void SetButtonsEnabled(bool enabled)
        {
            var state = SaveManager.CurrentState;
            AttackBtn.IsEnabled = enabled;
            DefendBtn.IsEnabled = enabled;
            PotionHpBtn.IsEnabled = enabled && state.HpPotions > 0;
            PotionMpBtn.IsEnabled = enabled && state.MpPotions > 0;

            foreach (var child in ActionBarPanel.Children)
            {
                if (child is Button btn && btn.Tag is EpicBattle.Models.RPG.Skill skill)
                {
                    btn.IsEnabled = enabled && state.PlayerMp >= skill.ManaCost;
                }
            }

            if (state.PlayerHp <= 0)
            {
                NewGameBtn.Visibility = Visibility.Visible;
                NewGameBtn.IsEnabled = true;
            }
        }

        private void AnimateScreenShake()
        {
            var shakeAnimation = new ThicknessAnimation
            {
                From = new Thickness(20), To = new Thickness(10, 20, 30, 20),
                Duration = TimeSpan.FromMilliseconds(50), AutoReverse = true, RepeatBehavior = new RepeatBehavior(3)
            };
            MainGrid.BeginAnimation(Grid.MarginProperty, shakeAnimation);
        }

        private void AnimateColorFlash(SolidColorBrush flashColor)
        {
            var colorAnimation = new ColorAnimation
            {
                From = flashColor.Color,
                To = ((SolidColorBrush)new BrushConverter().ConvertFrom("#1E1E1E")!).Color,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            var brush = new SolidColorBrush(flashColor.Color);
            this.Background = brush;
            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        }

        private async Task EnemyTurnAsync()
        {
            SetButtonsEnabled(false);
            StatusText.Text = "Враг атакует...";
            _totalTurnsPassed++;
            var state = SaveManager.CurrentState;

            // Применение классовых пассивок, действующих каждый ход
            ApplyClassPassivesPerTurn(state);

            // Применение эффектов на врага перед его ходом
            if (_enemyBleedTurns > 0)
            {
                int bleedDmg = state.PlayerBaseDamage / 2;
                enemyHp -= bleedDmg;
                Log($"🩸 Враг истекает кровью на {bleedDmg} урона!");
                _enemyBleedTurns--;
            }

            if (_enemyBurnTurns > 0)
            {
                int burnDmg = 10;
                enemyHp -= burnDmg;
                Log($"🔥 Враг горит на {burnDmg} урона!");
                _enemyBurnTurns--;
            }

            UpdateUI();

            if (enemyHp <= 0)
            {
                enemyHp = 0;
                UpdateUI();
                await Task.Delay(1000);
                if (_currentWave < _maxWaves)
                {
                    _currentWave++;
                    Log($"\nВраг повержен! Наступает следующий противник.");
                    GenerateEnemy();
                    UpdateUI();
                    StatusText.Text = "Ваш ход!";
                    SetButtonsEnabled(true);
                    return;
                }
                else
                {
                    WinLevel();
                    return;
                }
            }

            await Task.Delay(1500);

            if (enemyHp <= 0)
            {
                if (_currentWave < _maxWaves)
                {
                    _currentWave++;
                    Log($"\nВраг повержен! Наступает следующий противник.");
                    GenerateEnemy();
                    UpdateUI();
                    StatusText.Text = "Ваш ход!";
                    SetButtonsEnabled(true);
                    return;
                }
                else
                {
                    WinLevel();
                    return;
                }
            }

            if (_playerDamageBuffTurns > 0)
            {
                _playerDamageBuffTurns--;
            }

            if (_isGromgarEncounter && !_gromgarEnraged)
            {
                double hpPercentage = (double)enemyHp / enemyMaxHp * 100.0;
                if (hpPercentage <= _gromgarRageThreshold)
                {
                    _gromgarEnraged = true;
                    enemyDamage = (int)(enemyDamage * 1.5);
                    Log("\n🔥 Громгар впадает в ЯРОСТЬ! Его урон чудовищно увеличен!");
                }
            }

            if (_enemiesBurn && _currentWave < 3)
            {
                enemyHp -= 5;
                Log($"🔥 Орк получает 5 урона от ожога.");
                if (enemyHp <= 0) { enemyHp = 0; UpdateUI(); _ = EnemyTurnAsync(); return; }
            }

            if (_vargSkipTurn && _currentWave == 3)
            {
                Log($"😵 Шаман Варг пропускает ход!");
                _vargSkipTurn = false;
            }
            else
            {
                // Шанс орка на блок (если он не в ярости)
                bool orcBlocks = false;
                if (!_orcCannotBlock && random.NextDouble() > 0.7)
                {
                    orcBlocks = true;
                }

                if (orcBlocks)
                {
                    Log($"🛡 Враг уходит в глухую защиту!");
                }
                else
                {
                    int damage = random.Next(enemyDamage - 5, enemyDamage + 5);

                    if (_isGromgarEncounter)
                    {
                        if (_gromgarChaosAttack && random.NextDouble() > 0.5)
                        {
                            damage = (int)(damage * 1.3);
                            Log("🔥 Хаотичный взрыв алтаря усиливает удар Громгара!");
                        }
                        if (_bloodlustMultiplier > 1.0)
                        {
                            damage = (int)(damage * _bloodlustMultiplier);
                        }
                    }

                    // Расчет уклонения
                    if (random.NextDouble() * 100 < state.DodgeChance)
                    {
                        Log($"💨 Вы уклонились от атаки врага!");
                        damage = 0;
                    }
                    else
                    {
                        // Учет брони/защиты
                        int currentDefense = state.Defense;
                        // V2_ThickSkin (Толстая Кожа) - пассивно увеличивает защиту на 5
                        if (state.UnlockedSkills.Contains("V2_ThickSkin"))
                        {
                            currentDefense += 5;
                        }
                        damage = Math.Max(1, damage - currentDefense);

                        if (isDefending)
                        {
                            damage /= 2;
                            if (_bloodlustMultiplier > 1.0) damage = (int)(damage * 1.15);
                            Log($"🛡 Вы защищаетесь! Урон снижен. Враг наносит {damage} урона.");
                            isDefending = false;
                        }
                        else
                        {
                            // Учет Щита Маны
                            if (_manaShieldTurns > 0)
                            {
                                int manaDamage = damage / 2;
                                if (state.PlayerMp >= manaDamage)
                                {
                                    state.PlayerMp -= manaDamage;
                                    damage -= manaDamage;
                                    Log($"🛡️ Щит Маны поглотил {manaDamage} урона.");
                                }
                                _manaShieldTurns--;
                            }

                            Log($"👹 Враг наносит вам {damage} урона.");
                            AnimateScreenShake();
                            AnimateColorFlash(new SolidColorBrush(Colors.DarkRed));
                            PlaySound("hit.wav");
                        }

                        SaveManager.CurrentState.PlayerHp -= damage;
                    }
                }
            }

            if (SaveManager.CurrentState.PlayerHp <= 0)
            {
                SaveManager.CurrentState.PlayerHp = 0;
                UpdateUI();
                Log("💀 Вы погибли. Игра окончена.");
                StatusText.Text = "Game Over!";
                bgmPlayer.Stop();
                PlaySound("gameover.wav");
                SetButtonsEnabled(false);
                return;
            }

            UpdateUI();
            StatusText.Text = "Ваш ход!";
        }

        private void WinLevel()
        {
            Log("\n🏆 ВЫ ПОБЕДИЛИ!");
            StatusText.Text = "Победа!";
            SetButtonsEnabled(false);
            PlaySound("victory.wav");

            // Начисление опыта и очков за уровень
            var state = SaveManager.CurrentState;
            state.PlayerLevel++;
            state.StatPoints += 2;
            state.SkillPoints += 1;

            if (_isArcade)
            {
                ShowUpgradeScreen();
            }
            else
            {
                // Логика победы для Сюжета
                ShowUpgradeScreen();
                UpgradeGrid.Visibility = Visibility.Collapsed;

                string lootMsg = "Сюжетный бой завершен.";

                // Проверка исхода боя с Варгом
                if (_returnNodeId == "PostBattle_Scene3")
                {
                    if (_totalTurnsPassed <= 12)
                    {
                        SaveManager.CurrentState.HasTomeIntact = true;
                        lootMsg += "\nВы успели! Рунический Фолиант цел.";
                    }
                    else
                    {
                        SaveManager.CurrentState.HasTomeIntact = false;
                        lootMsg += "\nВы бились слишком долго. Шаман успел сжечь часть страниц.";
                    }

                    if (_battleModifier == "Varg_Honorable")
                    {
                        SaveManager.CurrentState.HasShamanCharm = true;
                        lootMsg += "\nПолучен трофей: Оберег Шамана.";
                    }
                }

                WinSubTitleText.Text = lootMsg;
                ContinueStoryBtn.Visibility = Visibility.Visible;
            }
        }

        private void ShowUpgradeScreen()
        {
            UpgradeOverlay.Visibility = Visibility.Visible;

            var state = SaveManager.CurrentState;

            string[] allUpgrades = { "Урон (+5)", "Здоровье (+20 HP)", "Магия (+10 Рун. Урон)", "Зелья (+1 HP, +1 MP)", "Мана (+20 MP)" };
            var selectedUpgrades = new List<string>();

            while (selectedUpgrades.Count < 3)
            {
                string upgrade = allUpgrades[random.Next(allUpgrades.Length)];
                if (!selectedUpgrades.Contains(upgrade)) selectedUpgrades.Add(upgrade);
            }

            Upgrade1Btn.Content = selectedUpgrades[0];
            Upgrade2Btn.Content = selectedUpgrades[1];
            Upgrade3Btn.Content = selectedUpgrades[2];

            if (_isArcade)
            {
                NextBattleBtn.Visibility = Visibility.Visible;
                ContinueStoryBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                NextBattleBtn.Visibility = Visibility.Collapsed;
                // ContinueStoryBtn.Visibility уже управляется логикой в EnemyDie
            }
        }

        private void UpgradeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.Content == null) return;
            string upgrade = btn.Content.ToString() ?? "";
            var state = SaveManager.CurrentState;

            // Оставляем гибридную систему (и карточка, и очки)
            if (upgrade.Contains("Урон")) state.Strength += 2;
            else if (upgrade.Contains("Здоровье")) state.Endurance += 2;
            else if (upgrade.Contains("Магия")) state.Intelligence += 2;
            else if (upgrade.Contains("Зелья")) { state.HpPotions++; state.MpPotions++; }
            else if (upgrade.Contains("Мана")) state.Intelligence += 2;

            state.RecalculateDerivedStats();
            Log($"🌟 Вы выбрали улучшение: {upgrade}");

            // Дисейблим кнопки выбора, чтобы не накликали дважды
            Upgrade1Btn.IsEnabled = false;
            Upgrade2Btn.IsEnabled = false;
            Upgrade3Btn.IsEnabled = false;
        }

        private void OpenSkillTreeBtn_Click(object sender, RoutedEventArgs e)
        {
            SkillTreeView skillTreeView = null;
            // Открываем дерево навыков поверх всего (через новый View)
            skillTreeView = new SkillTreeView(() =>
            {
                // Колбэк при закрытии дерева навыков
                if (skillTreeView != null)
                {
                    MainGrid.Children.Remove(skillTreeView);
                }
                UpdateUI(); // Обновляем Action Bar, если выучили скилл
            });

            // Добавляем его в MainGrid
            Grid.SetRowSpan(skillTreeView, 5);
            Panel.SetZIndex(skillTreeView, 200);
            MainGrid.Children.Add(skillTreeView);
        }

        private void NextBattleBtn_Click(object sender, RoutedEventArgs e)
        {
            UpgradeOverlay.Visibility = Visibility.Collapsed;
            Upgrade1Btn.IsEnabled = true;
            Upgrade2Btn.IsEnabled = true;
            Upgrade3Btn.IsEnabled = true;
            StartNewLevel();
        }

        private void StartNewLevel()
        {
            var state = SaveManager.CurrentState;
            GenerateEnemy();

            state.PlayerHp = state.PlayerMaxHp;
            state.PlayerMp = state.PlayerMaxMp;

            Log($"\n--- УРОВЕНЬ {state.PlayerLevel} ---");
            Log($"Появляется новый враг: {EnemyNameText.Text}!");

            UpdateUI();
            StatusText.Text = "Ваш ход!";
        }

        private void ContinueStoryBtn_Click(object sender, RoutedEventArgs e)
        {
            bgmPlayer.Stop();
            var state = SaveManager.CurrentState;
            state.PlayerHp = state.PlayerMaxHp;
            state.PlayerMp = state.PlayerMaxMp;

            if (!string.IsNullOrEmpty(_returnNodeId))
            {
                state.CurrentNodeId = _returnNodeId;
            }

            NavigationManager.NavigateTo(new DialogueView());
        }

        private void GenerateEnemy()
        {
            int lvl = SaveManager.CurrentState.PlayerLevel;
            int baseEnemyHp = 60 + (lvl * 20);
            int baseEnemyDamage = 10 + (lvl * 5);

            if (!_isArcade && _returnNodeId == "PostBattle1")
            {
                EnemyNameText.Text = "👺 Орк Углук";
                enemyMaxHp = 100;
                enemyDamage = 15;
            }
            else if (!_isArcade && _returnNodeId == "PostBattle_Scene4")
            {
                EnemyNameText.Text = "👑 Вождь Громгар";
                enemyMaxHp = 200;
                enemyDamage = 22;
                _isGromgarEncounter = true;
            }
            else if (!_isArcade && _returnNodeId == "PostBattle_Scene3")
            {
                if (_currentWave < 3)
                {
                    EnemyNameText.Text = $"👺 Орочий воин ({_currentWave}/2)";
                    enemyMaxHp = 80;
                    enemyDamage = 10;

                    if (_enemiesBurn)
                    {
                        enemyMaxHp = (int)(enemyMaxHp * 0.7);
                    }
                    if (_enemiesBuffed)
                    {
                        enemyDamage = (int)(enemyDamage * 1.1);
                    }
                }
                else
                {
                    EnemyNameText.Text = "💀 Шаман Варг";
                    enemyMaxHp = 150;
                    enemyDamage = 25; // Магический урон

                    if (_vargSkipTurn)
                    {
                        Log("Шаман Варг приходит в себя после взрыва и пропускает первый ход.");
                    }
                }
            }
            else
            {
                int enemyType = random.Next(3);
                if (enemyType == 0)
                {
                    EnemyNameText.Text = "👹 Орк-берсерк";
                    enemyMaxHp = (int)(baseEnemyHp * 0.7);
                    enemyDamage = (int)(baseEnemyDamage * 1.5);
                    Log("Этот орк выглядит очень агрессивно.");
                }
                else if (enemyType == 1)
                {
                    EnemyNameText.Text = "🛡 Бронированный орк";
                    enemyMaxHp = (int)(baseEnemyHp * 1.5);
                    enemyDamage = (int)(baseEnemyDamage * 0.7);
                    Log("Этот орк закован в тяжелую броню.");
                }
                else
                {
                    EnemyNameText.Text = "👺 Орк-воин";
                    enemyMaxHp = baseEnemyHp;
                    enemyDamage = baseEnemyDamage;
                    Log("Стандартный боец ближнего боя.");
                }
            }

            enemyHp = enemyMaxHp;
        }

        private void NewGameBtn_Click(object sender, RoutedEventArgs e)
        {
            bgmPlayer.Stop();
            NavigationManager.NavigateTo(new MainMenu());
        }

        private async void AttackBtn_Click(object sender, RoutedEventArgs e)
        {
            var state = SaveManager.CurrentState;
            int damage = random.Next(state.PlayerBaseDamage, state.PlayerBaseDamage + 10);

            // S3_SwordMastery (Мастерство Меча) - увеличивает физический урон на 10%
            if (state.UnlockedSkills.Contains("S3_SwordMastery"))
            {
                damage = (int)(damage * 1.1);
            }

            // Проверка на крит
            bool isCrit = random.NextDouble() * 100 < state.CritChance;
            if (isCrit)
            {
                damage = (int)(damage * 1.5); // Крит х1.5
                Log("💥 КРИТИЧЕСКИЙ УДАР!");
            }

            if (_playerDamageBuffTurns > 0)
            {
                damage = (int)(damage * 1.5);
            }

            if (_bloodlustMultiplier > 1.0)
            {
                damage = (int)(damage * _bloodlustMultiplier);
            }

            enemyHp -= damage;
            Log($"⚔️ Вы бьете мечом и наносите {damage} урона!");

            // Вампиризм (Выживание 3)
            if (state.UnlockedSkills.Contains("V3_Vampirism"))
            {
                int heal = (int)(damage * 0.2);
                state.PlayerHp = Math.Min(state.PlayerMaxHp, state.PlayerHp + heal);
                if(heal > 0) Log($"🦇 Вампиризм восстановил вам {heal} HP.");
            }

            AnimateColorFlash(new SolidColorBrush(Colors.LightGray));
            PlaySound("attack.wav");
            UpdateUI();

            if (enemyHp <= 0) { enemyHp = 0; await EnemyTurnAsync(); return; }
            await EnemyTurnAsync();
        }

        private async void DefendBtn_Click(object sender, RoutedEventArgs e)
        {
            isDefending = true;
            Log("🛡 Вы встаете в защитную стойку.");
            await EnemyTurnAsync();
        }

        private async void MagicBtn_Click(object sender, RoutedEventArgs e)
        {
            var state = SaveManager.CurrentState;
            if (state.PlayerMp >= 10)
            {
                state.PlayerMp -= 10;
                int damage = random.Next(state.PlayerBaseMagicDamage, state.PlayerBaseMagicDamage + 15);

                if (_bloodlustMultiplier > 1.0)
                {
                    damage = (int)(damage * _bloodlustMultiplier);
                }

                enemyHp -= damage;
                Log($"✨ Вы используете Руну Огня! Враг получает {damage} урона.");
                AnimateColorFlash(new SolidColorBrush(Colors.Orange));
                PlaySound("magic.wav");
                UpdateUI();

                if (enemyHp <= 0) { enemyHp = 0; await EnemyTurnAsync(); return; }
                await EnemyTurnAsync();
            }
        }

        private async void PotionHpBtn_Click(object sender, RoutedEventArgs e)
        {
            var state = SaveManager.CurrentState;
            if (state.HpPotions > 0)
            {
                state.HpPotions--;
                int heal = 40;
                state.PlayerHp = Math.Min(state.PlayerMaxHp, state.PlayerHp + heal);
                Log($"🧪 Вы выпили зелье здоровья. Восстановлено {heal} HP.");
                AnimateColorFlash(new SolidColorBrush(Colors.LightGreen));
                PlaySound("heal.wav");
                UpdateUI();
                await EnemyTurnAsync();
            }
        }

        private async void PotionMpBtn_Click(object sender, RoutedEventArgs e)
        {
            var state = SaveManager.CurrentState;
            if (state.MpPotions > 0)
            {
                state.MpPotions--;
                int mana = 25;
                state.PlayerMp = Math.Min(state.PlayerMaxMp, state.PlayerMp + mana);
                Log($"💧 Вы выпили зелье маны. Восстановлено {mana} MP.");
                AnimateColorFlash(new SolidColorBrush(Colors.LightBlue));
                PlaySound("heal.wav");
                UpdateUI();
                await EnemyTurnAsync();
            }
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
            bgmPlayer.Stop();
            NavigationManager.NavigateTo(new MainMenu());
        }
    }
}
