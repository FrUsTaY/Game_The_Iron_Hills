using System;
using System.Collections.Generic;
using System.Linq;

namespace EpicBattle.Models.Combat
{
    public abstract class CombatEntity
    {
        public string Name { get; set; } = string.Empty;

        // Здоровье
        public int Hp { get; set; }
        public int MaxHp { get; set; }

        // Мана
        public int Mp { get; set; }
        public int MaxMp { get; set; }

        // Action Points
        public int CurrentAP { get; set; }
        public int MaxAP { get; set; } = 4;

        // Характеристики
        public int Agility { get; set; }
        public int BaseDamage { get; set; }
        public int BaseMagicDamage { get; set; }
        public int Armor { get; set; }

        // Флаги состояний
        public bool IsDefending { get; set; }

        // Эффекты
        public List<StatusEffect> ActiveEffects { get; set; } = new List<StatusEffect>();

        public void ApplyEffect(StatusEffect effect)
        {
            var existing = ActiveEffects.FirstOrDefault(e => e.Id == effect.Id);
            if (existing != null)
            {
                // По правилу: Длительность обновляется до максимальной, Value берется максимальный
                existing.Duration = Math.Max(existing.Duration, effect.Duration);
                existing.Value = Math.Max(existing.Value, effect.Value);
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
    }
}
