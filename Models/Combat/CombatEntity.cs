using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EpicBattle.Models.Combat
{
    public abstract class CombatEntity : INotifyPropertyChanged
    {
        public string Name { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty; // P0-фикс (В1): стабильный идентификатор врага вместо магических строк в Name

        private int _hp;
        public int Hp
        {
            get => _hp;
            set { _hp = Math.Max(0, Math.Min(MaxHp, value)); OnPropertyChanged(); }
        }

        private int _maxHp;
        public int MaxHp
        {
            get => _maxHp;
            set { _maxHp = value; OnPropertyChanged(); }
        }

        private int _mp;
        public int Mp
        {
            get => _mp;
            set { _mp = Math.Max(0, Math.Min(MaxMp, value)); OnPropertyChanged(); }
        }

        private int _maxMp;
        public int MaxMp
        {
            get => _maxMp;
            set { _maxMp = value; OnPropertyChanged(); }
        }

        private int _currentAP;
        public int CurrentAP
        {
            get => _currentAP;
            set { _currentAP = value; OnPropertyChanged(); }
        }

        public int MaxAP { get; set; } = 4;

        // RPG Характеристики
        public int Agility { get; set; }
        public int BaseDamage { get; set; }
        public int BaseMagicDamage { get; set; }
        public int Armor { get; set; }

        public double DodgeChance { get; set; } = 0;
        public double CritChance { get; set; } = 0;
        public double CritMultiplier { get; set; } = 1.5;

        private bool _isDefending;
        public bool IsDefending
        {
            get => _isDefending;
            set { _isDefending = value; OnPropertyChanged(); }
        }

        // Эффекты
        public ObservableCollection<StatusEffect> ActiveEffects { get; set; } = new ObservableCollection<StatusEffect>();

        public void ApplyEffect(StatusEffect effect)
        {
            var existing = ActiveEffects.FirstOrDefault(e => e.Id == effect.Id);
            if (existing != null)
            {
                existing.Duration = Math.Max(existing.Duration, effect.Duration);
                existing.Value = Math.Max(existing.Value, effect.Value);
                var index = ActiveEffects.IndexOf(existing);
                ActiveEffects.RemoveAt(index);
                ActiveEffects.Insert(index, existing);
            }
            else
            {
                ActiveEffects.Add(new StatusEffect
                {
                    Id = effect.Id,
                    Name = effect.Name,
                    Type = effect.Type,
                    Duration = effect.Duration,
                    InitialDuration = effect.Duration,
                    Value = effect.Value,
                    IconPath = effect.IconPath
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
