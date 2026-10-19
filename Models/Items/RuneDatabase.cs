using System.Collections.Generic;
using System.Linq;

namespace EpicBattle.Models.Items
{
    /// <summary>
    /// Этап 3.2 (часть 2): Рунический камень — вставляется в слот экипировки у кузнеца
    /// и даёт плоский бонус (не масштабируется уровнем улучшения предмета).
    /// </summary>
    public class RuneStone
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = "💠";
        public string Description { get; set; } = string.Empty;
        public int GoldValue { get; set; } = 60; // цена покупки у торговца / продажи

        // Бонусы (только те, что nonzero, применяются)
        public int BonusDamage { get; set; } = 0;
        public int BonusMagicDamage { get; set; } = 0;
        public int BonusArmor { get; set; } = 0;
        public int BonusHp { get; set; } = 0;
        public int BonusMp { get; set; } = 0;
        public int BonusStrength { get; set; } = 0;
        public int BonusDexterity { get; set; } = 0;
        public int BonusIntelligence { get; set; } = 0;
        public int BonusEndurance { get; set; } = 0;
        public double BonusCritChance { get; set; } = 0;
        public double BonusDodgeChance { get; set; } = 0;

        public string FormattedBonus
        {
            get
            {
                var list = new List<string>();
                if (BonusDamage > 0) list.Add($"+{BonusDamage} Урон");
                if (BonusMagicDamage > 0) list.Add($"+{BonusMagicDamage} Маг.урон");
                if (BonusArmor > 0) list.Add($"+{BonusArmor} Броня");
                if (BonusHp > 0) list.Add($"+{BonusHp} HP");
                if (BonusMp > 0) list.Add($"+{BonusMp} MP");
                if (BonusStrength > 0) list.Add($"+{BonusStrength} Сила");
                if (BonusDexterity > 0) list.Add($"+{BonusDexterity} Ловкость");
                if (BonusIntelligence > 0) list.Add($"+{BonusIntelligence} Интеллект");
                if (BonusEndurance > 0) list.Add($"+{BonusEndurance} Вын.");
                if (BonusCritChance > 0) list.Add($"+{BonusCritChance:F1}% Крит");
                if (BonusDodgeChance > 0) list.Add($"+{BonusDodgeChance:F1}% Уклон");
                return string.Join(", ", list);
            }
        }
    }

    /// <summary>База рунических камней Кряжа.</summary>
    public static class RuneDatabase
    {
        public static readonly List<RuneStone> AllRunes = new List<RuneStone>
        {
            new RuneStone { Id = "R_Ember",   Name = "Руна Тлеющего Угля",     Icon = "🔥", Description = "Тепло горнища влито в камень.", BonusDamage = 4, GoldValue = 70 },
            new RuneStone { Id = "R_Frost",   Name = "Руна Инеевого Шипа",     Icon = "❄️", Description = "Холод глубин Цитадели.", BonusMagicDamage = 5, GoldValue = 75 },
            new RuneStone { Id = "R_Stone",   Name = "Руна Каменной Кожи",     Icon = "🪨", Description = "Прочность скалы Кряжа.", BonusArmor = 4, GoldValue = 65 },
            new RuneStone { Id = "R_Blood",   Name = "Руна Кровавого Корня",   Icon = "🩸", Description = "Живучесть горного зверья.", BonusHp = 25, GoldValue = 60 },
            new RuneStone { Id = "R_Mana",    Name = "Руна Синей Лазури",      Icon = "🔷", Description = "Осколок магического источника.", BonusMp = 20, GoldValue = 60 },
            new RuneStone { Id = "R_Hawk",    Name = "Руна Буревестника",      Icon = "🦅", Description = "Зоркость горного орла.", BonusDexterity = 2, BonusDodgeChance = 2.0, GoldValue = 85 },
            new RuneStone { Id = "R_Fang",    Name = "Руна Волчьего Клыка",    Icon = "🐺", Description = "Точность хищной ночи.", BonusCritChance = 4.0, GoldValue = 90 },
            new RuneStone { Id = "R_Titan",   Name = "Руна Первородного Титана", Icon = "⛰️", Description = "Редчайшая реликвия глубин: усиливает всё сразу.", BonusStrength = 1, BonusDexterity = 1, BonusIntelligence = 1, BonusEndurance = 1, GoldValue = 160 }
        };

        public static RuneStone? Find(string id) => AllRunes.FirstOrDefault(r => r.Id == id);

        /// <summary>Случайный камень для дропа с элит/боссов (Титан — только с боссов).</summary>
        public static RuneStone RollRandomDrop(bool fromBoss)
        {
            var pool = fromBoss ? AllRunes : AllRunes.Where(r => r.Id != "R_Titan").ToList();
            return pool[Services.GameRng.Next(pool.Count)];
        }
    }
}
