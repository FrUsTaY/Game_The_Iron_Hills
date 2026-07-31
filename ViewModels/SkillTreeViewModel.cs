using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using EpicBattle.Managers;
using EpicBattle.Models;
using EpicBattle.Models.RPG;

namespace EpicBattle.ViewModels
{
    public class SkillTreeViewModel : INotifyPropertyChanged
    {
        private readonly GameState _gameState;

        public SkillTreeViewModel()
        {
            _gameState = SaveManager.CurrentState;
            LoadSkills();
        }

        public int SkillPoints => _gameState.SkillPoints;

        public ObservableCollection<SkillNodeViewModel> SwordPathSkills { get; set; } = new ObservableCollection<SkillNodeViewModel>();
        public ObservableCollection<SkillNodeViewModel> ArcaneArtsSkills { get; set; } = new ObservableCollection<SkillNodeViewModel>();
        public ObservableCollection<SkillNodeViewModel> SurvivalSkills { get; set; } = new ObservableCollection<SkillNodeViewModel>();

        public ICommand CloseCommand => new Command(_ => CloseAction?.Invoke());
        public Action CloseAction { get; set; }

        private void LoadSkills()
        {
            var allSkills = SkillsDatabase.AllSkills;

            foreach (var skill in allSkills.Where(s => s.Branch == SkillBranch.SwordPath))
                SwordPathSkills.Add(CreateSkillNode(skill));

            foreach (var skill in allSkills.Where(s => s.Branch == SkillBranch.ArcaneArts))
                ArcaneArtsSkills.Add(CreateSkillNode(skill));

            foreach (var skill in allSkills.Where(s => s.Branch == SkillBranch.Survival))
                SurvivalSkills.Add(CreateSkillNode(skill));
        }

        private SkillNodeViewModel CreateSkillNode(Skill skill)
        {
            var node = new SkillNodeViewModel(skill, _gameState);
            node.SkillLearned += OnSkillLearned;
            return node;
        }

        private void OnSkillLearned()
        {
            OnPropertyChanged(nameof(SkillPoints));

            // Обновляем доступность всех скиллов
            foreach (var skill in SwordPathSkills) skill.UpdateState();
            foreach (var skill in ArcaneArtsSkills) skill.UpdateState();
            foreach (var skill in SurvivalSkills) skill.UpdateState();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SkillNodeViewModel : INotifyPropertyChanged
    {
        private readonly Skill _skill;
        private readonly GameState _gameState;

        public SkillNodeViewModel(Skill skill, GameState gameState)
        {
            _skill = skill;
            _gameState = gameState;
            UpdateState();
        }

        public string Name => _skill.Name;
        public string Description => _skill.Description;
        public string Type => _skill.Type == SkillType.Active ? $"Активный (МП: {_skill.ManaCost}, КД: {_skill.Cooldown})" : "Пассивный";
        public string Requirements => $"Требуемый уровень: {_skill.RequiredLevel}";

        public bool IsLearned => _gameState.UnlockedSkills.Contains(_skill.Id);

        public bool CanLearn => !IsLearned &&
                                _gameState.SkillPoints > 0 &&
                                _gameState.PlayerLevel >= _skill.RequiredLevel &&
                                (string.IsNullOrEmpty(_skill.RequiredSkillId) || _gameState.UnlockedSkills.Contains(_skill.RequiredSkillId));

        public ICommand LearnCommand => new Command(_ => LearnSkill(), _ => CanLearn);

        public event Action SkillLearned;

        private void LearnSkill()
        {
            if (CanLearn)
            {
                _gameState.UnlockedSkills.Add(_skill.Id);
                _gameState.SkillPoints--;
                UpdateState();
                SkillLearned?.Invoke();
            }
        }

        public void UpdateState()
        {
            OnPropertyChanged(nameof(IsLearned));
            OnPropertyChanged(nameof(CanLearn));
            (LearnCommand as Command)?.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
