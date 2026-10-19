using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using EpicBattle.Managers;
using EpicBattle.Models;
using EpicBattle.Models.Encounters;
using EpicBattle.Models.Map;
using EpicBattle.Views;

namespace EpicBattle.ViewModels
{
    public class MapViewModel : INotifyPropertyChanged
    {
        private readonly GameState _gameState;

        public MapGraph Map => _gameState?.ActiveMap;
        public string ActTitle => Map?.ActTitle ?? "Экспедиция";
        public int CurrentFloor => Map?.CurrentFloor ?? 0;
        public int TotalFloors => Map?.TotalFloors ?? 10;
        public string ProgressText => $"{ActTitle} • Этаж {CurrentFloor} из {TotalFloors}";

        // Статы игрока
        public string PlayerName
        {
            get => _gameState?.PlayerName ?? "Элрик";
            set { if (_gameState != null) { _gameState.PlayerName = value; OnPropertyChanged(); } }
        }

        public string PlayerClass
        {
            get => _gameState?.PlayerClass ?? "Ветеран";
            set { if (_gameState != null) { _gameState.PlayerClass = value; OnPropertyChanged(); } }
        }

        public int PlayerLevel
        {
            get => _gameState?.PlayerLevel ?? 1;
            set { if (_gameState != null) { _gameState.PlayerLevel = value; OnPropertyChanged(); } }
        }

        public int PlayerHp
        {
            get => _gameState?.PlayerHp ?? 100;
            set { if (_gameState != null) { _gameState.PlayerHp = value; OnPropertyChanged(); } }
        }

        public int PlayerMaxHp
        {
            get => _gameState?.PlayerMaxHp ?? 100;
            set { if (_gameState != null) { _gameState.PlayerMaxHp = value; OnPropertyChanged(); } }
        }

        public int PlayerMp
        {
            get => _gameState?.PlayerMp ?? 50;
            set { if (_gameState != null) { _gameState.PlayerMp = value; OnPropertyChanged(); } }
        }

        public int PlayerMaxMp
        {
            get => _gameState?.PlayerMaxMp ?? 50;
            set { if (_gameState != null) { _gameState.PlayerMaxMp = value; OnPropertyChanged(); } }
        }

        public int Gold
        {
            get => _gameState?.Gold ?? 0;
            set { if (_gameState != null) { _gameState.Gold = value; OnPropertyChanged(); } }
        }

        public int OreStock
        {
            get => _gameState?.Ore ?? 0;
            set { if (_gameState != null) { _gameState.Ore = value; OnPropertyChanged(); } }
        }

        // Всего рунических камней на складе (для HUD карты)
        public int RuneStockCount => _gameState?.RuneStock?.Values.Sum() ?? 0;

        public int HpPotions
        {
            get => _gameState?.HpPotions ?? 0;
            set { if (_gameState != null) { _gameState.HpPotions = value; OnPropertyChanged(); } }
        }

        public int MpPotions
        {
            get => _gameState?.MpPotions ?? 0;
            set { if (_gameState != null) { _gameState.MpPotions = value; OnPropertyChanged(); } }
        }

        public int StatPoints
        {
            get => _gameState?.StatPoints ?? 0;
            set { if (_gameState != null) { _gameState.StatPoints = value; OnPropertyChanged(); } }
        }

        public int SkillPoints
        {
            get => _gameState?.SkillPoints ?? 0;
            set { if (_gameState != null) { _gameState.SkillPoints = value; OnPropertyChanged(); } }
        }

        // Победный экран
        private bool _isVictoryVisible = false;
        public Visibility VictoryOverlayVisibility => _isVictoryVisible ? Visibility.Visible : Visibility.Collapsed;

        public string VictoryDescriptionText => Map?.ActNumber switch
        {
            1 => "Верховный Вождь Громгар повержен! Перевал через Железный Кряж свободен от орочьих банд. Впереди перед вами вырастают циклопические стены Древней Цитадели...",
            // P2-фикс (П6): учтён Акт 3 «Сердце Титана», чьи карты уже генерируются MapGenerator
            2 => "Архимаг Малкор повержен, а культ Первородной Тьмы уничтожен! Но земля дрожит — пробуждается что-то древнее в недрах Кряжа...",
            _ => "Сердце Титана сокрушено! Железный Кряж спасён от пробудившегося древнего зла. Ваше имя высечено на руинах Цитадели — вы легенда этих земель!"
        };

        public string NextActButtonText => Map?.ActNumber switch
        {
            1 => "🏰 Перейти к Акту 2 (Тени Цитадели)",
            2 => "🌋 Перейти к Акту 3 (Сердце Титана)",
            _ => "🏆 Завершить триумфальный поход"
        };

        public ObservableCollection<MapFloorGroup> FloorGroups { get; set; } = new ObservableCollection<MapFloorGroup>();

        public ICommand SelectNodeCommand => new Command(OnSelectNode);
        public ICommand OpenInventoryCommand => new Command(_ => OpenInventory());
        public ICommand OpenSkillTreeCommand => new Command(_ => OpenSkillTree());
        public ICommand SaveGameCommand => new Command(_ => SaveGame());
        public ICommand MainMenuCommand => new Command(_ => NavigationManager.NavigateTo(new MainMenu()));

        public MapViewModel()
        {
            _gameState = SaveManager.CurrentState;
            if (_gameState != null && _gameState.ActiveMap == null)
            {
                _gameState.StartNewMap(_gameState.CurrentAct > 0 ? _gameState.CurrentAct : 1);
            }
            if (_gameState?.ActiveMap != null && _gameState.ActiveMap.IsCompleted)
            {
                _isVictoryVisible = true;
            }
            RefreshMap();
        }

        public void RefreshMap()
        {
            FloorGroups.Clear();
            if (Map != null)
            {
                foreach (var group in Map.GetFloorGroups(reverseForVerticalClimb: true))
                {
                    FloorGroups.Add(group);
                }
            }

            OnPropertyChanged(nameof(Map));
            OnPropertyChanged(nameof(ProgressText));
            OnPropertyChanged(nameof(PlayerHp));
            OnPropertyChanged(nameof(PlayerMaxHp));
            OnPropertyChanged(nameof(PlayerMp));
            OnPropertyChanged(nameof(PlayerMaxMp));
            OnPropertyChanged(nameof(Gold));
            OnPropertyChanged(nameof(OreStock));
            OnPropertyChanged(nameof(RuneStockCount));
            OnPropertyChanged(nameof(HpPotions));
            OnPropertyChanged(nameof(MpPotions));
            OnPropertyChanged(nameof(StatPoints));
            OnPropertyChanged(nameof(SkillPoints));
            OnPropertyChanged(nameof(PlayerLevel));
            OnPropertyChanged(nameof(VictoryOverlayVisibility));
        }

        private void OnSelectNode(object param)
        {
            if (param is MapNode node)
            {
                if (!node.IsAvailable || node.IsVisited) return;

                Map.SelectNode(node.Id);

                switch (node.Type)
                {
                    case MapNodeType.NormalBattle:
                        NavigationManager.NavigateTo(new BattleView(isArcade: true, returnNodeId: null, battleModifier: null, arcadeLevel: node.Floor + 1, onCompletedFromMap: OnNodeCompleted));
                        break;

                    case MapNodeType.EliteBattle:
                        NavigationManager.NavigateTo(new BattleView(isArcade: true, returnNodeId: null, battleModifier: "Elite", arcadeLevel: node.Floor + 2, onCompletedFromMap: OnNodeCompleted));
                        break;

                    case MapNodeType.Encounter:
                        var encounter = EncountersDatabase.GetRandomEncounter();
                        NavigationManager.NavigateTo(new EncounterView(encounter, OnNodeCompleted));
                        break;

                    case MapNodeType.Merchant:
                        NavigationManager.NavigateTo(new MerchantView(OnNodeCompleted));
                        break;

                    case MapNodeType.Campfire:
                        NavigationManager.NavigateTo(new CampfireView(OnNodeCompleted));
                        break;

                    case MapNodeType.Blacksmith:
                        NavigationManager.NavigateTo(new BlacksmithView(OnNodeCompleted));
                        break;

                    case MapNodeType.Tavern:
                        NavigationManager.NavigateTo(new TavernView(OnNodeCompleted));
                        break;

                    case MapNodeType.Boss:
                        NavigationManager.NavigateTo(new BattleView(isArcade: true, returnNodeId: null, battleModifier: "Boss", arcadeLevel: node.Floor + 3, onCompletedFromMap: OnBossCompleted));
                        break;
                }
            }
        }

        private void OnNodeCompleted()
        {
            Map.CompleteCurrentNode();
            AutoSaveIfNeeded(); // P2-фикс (П5): прогресс карты больше не теряется при закрытии игры
            NavigationManager.NavigateTo(new MapView());
        }

        private void OnBossCompleted()
        {
            Map.CompleteCurrentNode();
            
            // P2-фикс (П7): масштаб наград по акту (было константное +150 вне зависимости от акта)
            int act = Map.ActNumber;
            _gameState.Gold += 150 * act;
            _gameState.StatPoints += 2;
            _gameState.SkillPoints += 1;
            _gameState.Ore += 3; // Этап 3.2: жилы руды с босса — подарок кузнецу не нужны
            if (act > _gameState.CurrentAct) _gameState.CurrentAct = act; // прогрессия кампании

            _isVictoryVisible = true;
            AutoSaveIfNeeded(); // P2-фикс (П5)
            NavigationManager.NavigateTo(new MapView());
        }

        /// <summary>P2: автосейв в режиме Карты — только если игра уже была начата из сейва.</summary>
        private void AutoSaveIfNeeded()
        {
            try
            {
                if (!string.IsNullOrEmpty(_gameState?.SaveName))
                    SaveManager.SaveGame(_gameState.SaveName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AutoSave] пропуск: {ex.Message}");
            }
        }

        public void ProceedToNextAct()
        {
            _isVictoryVisible = false;
            // P2-фикс (П7): раньше переход был только до Акта 2 (условие < 2),
            // карта Акта 3 не открывалась, несмотря на поддержку в MapGenerator.
            if (Map.ActNumber < 3)
            {
                _gameState.StartNewMap(Map.ActNumber + 1);
                NavigationManager.NavigateTo(new MapView());
            }
            else
            {
                NavigationManager.NavigateTo(new MainMenu());
            }
        }

        private void OpenInventory()
        {
            NavigationManager.NavigateTo(new InventoryView(() =>
            {
                NavigationManager.NavigateTo(new MapView());
            }));
        }

        private void OpenSkillTree()
        {
            NavigationManager.NavigateTo(new SkillTreeView(() =>
            {
                NavigationManager.NavigateTo(new MapView());
            }));
        }

        private void SaveGame()
        {
            NavigationManager.NavigateTo(new SaveLoadView(isSaving: true));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
