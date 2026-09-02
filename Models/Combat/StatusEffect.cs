using System;
using System.Collections.Generic;

namespace EpicBattle.Models.Combat
{
    public enum StatusEffectType
    {
        Buff,
        Debuff
    }

    public class StatusEffect
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public StatusEffectType Type { get; set; }
        public int Duration { get; set; }
        public int InitialDuration { get; set; }
        public float Value { get; set; }
        public string IconPath { get; set; } = string.Empty;
    }
}
