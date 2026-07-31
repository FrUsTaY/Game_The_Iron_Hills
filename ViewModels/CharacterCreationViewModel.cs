using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using EpicBattle.Managers;
using EpicBattle.Models;
using EpicBattle.Views;

namespace EpicBattle.ViewModels
{
    public class Command : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public Command(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter!);
        public void Execute(object? parameter) => _execute(parameter!);

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public class CharacterCreationViewModel : INotifyPropertyChanged
    {
        private bool _isArcade;
        public CharacterCreationViewModel(bool isArcade)
        {
            _isArcade = isArcade;

            // Устанавливаем дефолтный класс и его статы
            SelectClass("Ветеран");
        }

        private string _playerName = "Элрик";
        public string PlayerName
        {
            get => _playerName;
            set { _playerName = value; OnPropertyChanged(); }
        }

        private string _selectedClass;
        public string SelectedClass
        {
            get => _selectedClass;
            set { _selectedClass = value; OnPropertyChanged(); }
        }

        private string _classDescription;
        public string ClassDescription
        {
            get => _classDescription;
            set { _classDescription = value; OnPropertyChanged(); }
        }

        private int _availablePoints = 5;
        public int AvailablePoints
        {
            get => _availablePoints;
            set { _availablePoints = value; OnPropertyChanged(); StartGameCommand.RaiseCanExecuteChanged(); }
        }

        private int _strength;
        public int Strength
        {
            get => _strength;
            set { _strength = value; OnPropertyChanged(); UpdateDerivedStats(); }
        }

        private int _dexterity;
        public int Dexterity
        {
            get => _dexterity;
            set { _dexterity = value; OnPropertyChanged(); UpdateDerivedStats(); }
        }

        private int _intelligence;
        public int Intelligence
        {
            get => _intelligence;
            set { _intelligence = value; OnPropertyChanged(); UpdateDerivedStats(); }
        }

        private int _endurance;
        public int Endurance
        {
            get => _endurance;
            set { _endurance = value; OnPropertyChanged(); UpdateDerivedStats(); }
        }

        // --- Производные статы ---
        public int HP => SelectedClass == "Ветеран" ? 60 + (Endurance * 10) : 50 + (Endurance * 10);
        public int MP => 20 + (Intelligence * 6);
        public int Damage => 5 + (Strength * 2);
        public int MagicDamage => 5 + (Intelligence * 2);
        public double DodgeChance => SelectedClass == "Наемник" ? Math.Min(50, Dexterity * 1.5 + 5.0) : Math.Min(50, Dexterity * 1.5);
        public double CritChance => SelectedClass == "Наемник" ? Math.Min(50, Dexterity * 1.5 + 5.0) : Math.Min(50, Dexterity * 1.5);

        private void UpdateDerivedStats()
        {
            OnPropertyChanged(nameof(HP));
            OnPropertyChanged(nameof(MP));
            OnPropertyChanged(nameof(Damage));
            OnPropertyChanged(nameof(MagicDamage));
            OnPropertyChanged(nameof(DodgeChance));
            OnPropertyChanged(nameof(CritChance));
        }

        public ICommand SelectClassCommand => new Command(param => SelectClass(param.ToString()));
        public ICommand IncreaseStatCommand => new Command(IncreaseStat, CanIncreaseStat);
        public ICommand DecreaseStatCommand => new Command(DecreaseStat, CanDecreaseStat);

        private Command _startGameCommand;
        public Command StartGameCommand => _startGameCommand ??= new Command(StartGame, _ => AvailablePoints == 0);
        public ICommand BackCommand => new Command(_ => NavigationManager.NavigateTo(new MainMenu()));

        private void SelectClass(string className)
        {
            SelectedClass = className;
            // Сбрасываем очки при смене класса, даем базовые статы класса
            AvailablePoints = 5;

            switch (className)
            {
                case "Ветеран":
                    ClassDescription = "Сбалансированный боец. Упор на физический урон и выносливость.\nПассивно: «Закалка в боях» (+10 к Макс. HP, снижает получаемый урон).";
                    Strength = 6; Dexterity = 5; Intelligence = 3; Endurance = 6;
                    break;
                case "Изгой-маг":
                    ClassDescription = "Использует тайные искусства. Слабое здоровье, но высокий магический урон.\nПассивно: «Тайный источник» (Регенерирует ману в бою).";
                    Strength = 3; Dexterity = 4; Intelligence = 8; Endurance = 3;
                    break;
                case "Наемник":
                    ClassDescription = "Ловкий и смертоносный. Высокий шанс крита и уклонения.\nПассивно: «Инстинкт убийцы» (Повышает шанс крита и уклонения на 5%).";
                    Strength = 4; Dexterity = 8; Intelligence = 3; Endurance = 5;
                    break;
            }
        }

        private void IncreaseStat(object param)
        {
            if (AvailablePoints <= 0) return;
            string stat = param.ToString();
            if (stat == "STR") Strength++;
            else if (stat == "DEX") Dexterity++;
            else if (stat == "INT") Intelligence++;
            else if (stat == "END") Endurance++;

            AvailablePoints--;
        }

        private bool CanIncreaseStat(object param) => AvailablePoints > 0;

        private void DecreaseStat(object param)
        {
            string stat = param.ToString();

            // Не даем опустить стат ниже базового значения класса (упрощенная логика)
            int minStr = 0, minDex = 0, minInt = 0, minEnd = 0;
             switch (SelectedClass)
            {
                case "Ветеран": minStr = 6; minDex = 5; minInt = 3; minEnd = 6; break;
                case "Изгой-маг": minStr = 3; minDex = 4; minInt = 8; minEnd = 3; break;
                case "Наемник": minStr = 4; minDex = 8; minInt = 3; minEnd = 5; break;
            }

            if (stat == "STR" && Strength > minStr) Strength--;
            else if (stat == "DEX" && Dexterity > minDex) Dexterity--;
            else if (stat == "INT" && Intelligence > minInt) Intelligence--;
            else if (stat == "END" && Endurance > minEnd) Endurance--;
            else return;

            AvailablePoints++;
        }

        private bool CanDecreaseStat(object param)
        {
            string stat = param?.ToString();
            if(stat == null) return false;

            int minStr = 0, minDex = 0, minInt = 0, minEnd = 0;
            switch (SelectedClass)
            {
                case "Ветеран": minStr = 6; minDex = 5; minInt = 3; minEnd = 6; break;
                case "Изгой-маг": minStr = 3; minDex = 4; minInt = 8; minEnd = 3; break;
                case "Наемник": minStr = 4; minDex = 8; minInt = 3; minEnd = 5; break;
            }

            if (stat == "STR") return Strength > minStr;
            if (stat == "DEX") return Dexterity > minDex;
            if (stat == "INT") return Intelligence > minInt;
            if (stat == "END") return Endurance > minEnd;
            return false;
        }

        private void StartGame(object param)
        {
            // Формируем GameState
            var state = new GameState
            {
                PlayerName = this.PlayerName ?? "Элрик",
                PlayerClass = this.SelectedClass ?? "Ветеран",
                Strength = this.Strength,
                Dexterity = this.Dexterity,
                Intelligence = this.Intelligence,
                Endurance = this.Endurance,
                IsStoryMode = !_isArcade
            };

            state.RecalculateDerivedStats();

            // Восстанавливаем HP/MP по максимуму
            state.PlayerHp = state.PlayerMaxHp;
            state.PlayerMp = state.PlayerMaxMp;

            // Стартовые навыки убраны. Вместо них работают пассивки классов
            // Дерево навыков в начале игры полностью пустое.

            SaveManager.CurrentState = state;

            if (_isArcade)
            {
                NavigationManager.NavigateTo(new BattleView(isArcade: true));
            }
            else
            {
                NavigationManager.NavigateTo(new DialogueView());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
