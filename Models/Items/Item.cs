using System.Collections.Generic;
using System;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace EpicBattle.Models.Items
{
    public enum ItemSlot
    {
        Weapon,     // Оружие
        Helmet,     // Шлем
        Chest,      // Доспех
        Boots,      // Поножи
        Ring,       // Кольцо
        Amulet,     // Амулет
        Consumable, // Зелье
        Quest       // Квестовый
    }

    public enum ItemRarity
    {
        Common,    // Обычный (Серый)
        Magic,     // Магический (Зеленый)
        Rare,      // Редкий (Синий)
        Epic,      // Эпический (Фиолетовый)
        Legendary  // Легендарный (Оранжевый)
    }

    public class Item
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ItemSlot Slot { get; set; } = ItemSlot.Weapon;
        public ItemRarity Rarity { get; set; } = ItemRarity.Common;
        public string IconPath { get; set; } = "🗡️";
        public int RequiredLevel { get; set; } = 1;
        public int GoldValue { get; set; } = 10;

        // Бонусы к характеристикам
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

        [JsonIgnore]
        public string RarityName => Rarity switch
        {
            ItemRarity.Common => "Обычный",
            ItemRarity.Magic => "Магический",
            ItemRarity.Rare => "Редкий",
            ItemRarity.Epic => "Эпический",
            ItemRarity.Legendary => "Легендарный",
            _ => "Обычный"
        };

        [JsonIgnore]
        public string RarityColor => Rarity switch
        {
            ItemRarity.Common => "#DDDDDD",
            ItemRarity.Magic => "#44FF44",
            ItemRarity.Rare => "#4488FF",
            ItemRarity.Epic => "#AA44FF",
            ItemRarity.Legendary => "#FFAA00",
            _ => "#FFFFFF"
        };

        private static readonly SolidColorBrush CommonBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
        private static readonly SolidColorBrush MagicBrush = new SolidColorBrush(Color.FromRgb(68, 255, 68));
        private static readonly SolidColorBrush RareBrush = new SolidColorBrush(Color.FromRgb(68, 136, 255));
        private static readonly SolidColorBrush EpicBrush = new SolidColorBrush(Color.FromRgb(170, 68, 255));
        private static readonly SolidColorBrush LegendaryBrush = new SolidColorBrush(Color.FromRgb(255, 170, 0));

        static Item()
        {
            CommonBrush.Freeze();
            MagicBrush.Freeze();
            RareBrush.Freeze();
            EpicBrush.Freeze();
            LegendaryBrush.Freeze();
        }

        [JsonIgnore]
        public SolidColorBrush RarityBrush => Rarity switch
        {
            ItemRarity.Common => CommonBrush,
            ItemRarity.Magic => MagicBrush,
            ItemRarity.Rare => RareBrush,
            ItemRarity.Epic => EpicBrush,
            ItemRarity.Legendary => LegendaryBrush,
            _ => CommonBrush
        };

        [JsonIgnore]
        public string SlotDisplayName => Slot switch
        {
            ItemSlot.Weapon => "Оружие",
            ItemSlot.Helmet => "Шлем",
            ItemSlot.Chest => "Доспех",
            ItemSlot.Boots => "Поножи",
            ItemSlot.Ring => "Кольцо",
            ItemSlot.Amulet => "Амулет",
            ItemSlot.Consumable => "Зелье",
            ItemSlot.Quest => "Квестовый",
            _ => "Предмет"
        };

        [JsonIgnore]
        public string StatsSummary => FormattedStats;
        public string FormattedStats
        {
            get
            {
                var list = new System.Collections.Generic.List<string>();
                if (BonusDamage > 0) list.Add($"+{BonusDamage} Урон");
                if (BonusMagicDamage > 0) list.Add($"+{BonusMagicDamage} Маг.урон");
                if (BonusArmor > 0) list.Add($"+{BonusArmor} Броня");
                if (BonusHp > 0) list.Add($"+{BonusHp} HP");
                if (BonusMp > 0) list.Add($"+{BonusMp} MP");
                if (BonusStrength > 0) list.Add($"+{BonusStrength} Сила");
                if (BonusDexterity > 0) list.Add($"+{BonusDexterity} Ловкость");
                if (BonusIntelligence > 0) list.Add($"+{BonusIntelligence} Интеллект");
                if (BonusEndurance > 0) list.Add($"+{BonusEndurance} Выносливость");
                if (BonusCritChance > 0) list.Add($"+{BonusCritChance:F1}% Крит");
                if (BonusDodgeChance > 0) list.Add($"+{BonusDodgeChance:F1}% Уклон");

                if (list.Count == 0) return Description;
                return string.Join(", ", list);
            }
        }
    }
}
