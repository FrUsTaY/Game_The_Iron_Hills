using System.Collections.Generic;

namespace EpicBattle.Models.RPG
{
    public enum SkillBranch
    {
        SwordPath,   // Путь Меча
        ArcaneArts,  // Тайные Искусства
        Survival     // Выживание
    }

    public enum SkillType
    {
        Active,
        Passive
    }

    public enum TargetType
    {
        SingleTarget, // Одиночная цель (требует клика по конкретному врагу)
        Cleave,       // Основная цель 100% урона + 50% по соседним
        AoE           // Все живые враги на экране
    }

    public class Skill
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public SkillBranch Branch { get; set; }
        public SkillType Type { get; set; }

        // Требования
        public int RequiredLevel { get; set; } = 1;
        public string RequiredSkillId { get; set; } = string.Empty; // ID навыка, который нужно изучить до этого

        // Для активных навыков
        public int APCost { get; set; } = 0;
        public TargetType TargetType { get; set; } = TargetType.SingleTarget;
        public int ManaCost { get; set; } = 0;
        public int Cooldown { get; set; } = 0; // В ходах

        public string IconPath { get; set; } = string.Empty;
    }

    public static class SkillsDatabase
    {
        public static readonly List<Skill> AllSkills = new List<Skill>
        {
            // --- Путь Меча ---
            new Skill { Id = "S1_PowerStrike", Name = "Мощный Удар", Description = "Наносит 150% физического урона.", Branch = SkillBranch.SwordPath, Type = SkillType.Active, APCost = 3, ManaCost = 10, TargetType = TargetType.SingleTarget },
            new Skill { Id = "S2_Cleave", Name = "Рассечение", Description = "Атака, накладывающая эффект кровотечения (урон каждый ход).", Branch = SkillBranch.SwordPath, Type = SkillType.Active, RequiredLevel = 3, RequiredSkillId = "S1_PowerStrike", APCost = 4, ManaCost = 15, TargetType = TargetType.Cleave },
            new Skill { Id = "S3_SwordMastery", Name = "Мастерство Меча", Description = "Пассивно увеличивает базовый физический урон на 10%.", Branch = SkillBranch.SwordPath, Type = SkillType.Passive, RequiredLevel = 5, RequiredSkillId = "S2_Cleave" },

            // --- Тайные Искусства ---
            new Skill { Id = "M1_Fireball", Name = "Огненный Шар", Description = "Наносит магический урон всем врагам и с небольшим шансом поджигает.", Branch = SkillBranch.ArcaneArts, Type = SkillType.Active, APCost = 3, ManaCost = 15, TargetType = TargetType.AoE },
            new Skill { Id = "M2_ManaShield", Name = "Щит Маны", Description = "В течение 3 ходов 50% получаемого урона поглощается за счет маны.", Branch = SkillBranch.ArcaneArts, Type = SkillType.Active, RequiredLevel = 3, RequiredSkillId = "M1_Fireball", APCost = 2, ManaCost = 20, Cooldown = 5, TargetType = TargetType.SingleTarget },
            new Skill { Id = "M3_ArcaneFocus", Name = "Концентрация", Description = "Пассивно восстанавливает 5 MP каждый ход.", Branch = SkillBranch.ArcaneArts, Type = SkillType.Passive, RequiredLevel = 5, RequiredSkillId = "M2_ManaShield" },

            // --- Выживание ---
            new Skill { Id = "V1_FirstAid", Name = "Первая Помощь", Description = "Восстанавливает 30 HP. Не тратит зелья.", Branch = SkillBranch.Survival, Type = SkillType.Active, APCost = 2, ManaCost = 10, Cooldown = 3, TargetType = TargetType.SingleTarget },
            new Skill { Id = "V2_ThickSkin", Name = "Толстая Кожа", Description = "Пассивно увеличивает защиту на 5.", Branch = SkillBranch.Survival, Type = SkillType.Passive, RequiredLevel = 3, RequiredSkillId = "V1_FirstAid" },
            new Skill { Id = "V3_Vampirism", Name = "Вампиризм", Description = "Ваши физические атаки восстанавливают здоровье в размере 20% от нанесенного урона.", Branch = SkillBranch.Survival, Type = SkillType.Passive, RequiredLevel = 5, RequiredSkillId = "V2_ThickSkin" }
        };
    }
}