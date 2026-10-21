using System.Collections.Generic;
using System.Linq;
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

        // --- Этап 3.2: Ремесленная система (улучшение у кузнеца) ---
        public int UpgradeLevel { get; set; } = 0;      // +1, +2, +3 (максимум для редкости)
        public bool IsUpgradable { get; set; } = false; // Улучшается только экипируемая базовая экипировка

        // --- Этап 3.2 (часть 2): Рунические камни ---
        public int RuneSlots { get; set; } = 0;                          // количество слотов под руны
        public List<string> InsertedRunes { get; set; } = new List<string>(); // Id вставленных камней (RuneDatabase)

        [JsonIgnore]
        public int FreeRuneSlots => Math.Max(0, RuneSlots - (InsertedRunes?.Count ?? 0));

        /// <summary>Суммарный бонус руны по ключу (руны не масштабируются улучшением предмета).</summary>
        private int RuneSum(Func<RuneStone, int> sel) =>
            (InsertedRunes ?? new List<string>()).Sum(id => { var r = RuneDatabase.Find(id); return r != null ? sel(r) : 0; });
        private double RuneDoubleSum(Func<RuneStone, double> sel) =>
            (InsertedRunes ?? new List<string>()).Sum(id => { var r = RuneDatabase.Find(id); return r != null ? sel(r) : 0.0; });

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

        // --- Эффективные значения (базовый бонус × множитель улучшения + руны) ---
        // P1-баланс: прирост от заточки уменьшен с +25% до +12% за уровень для плавного масштабирования
        [JsonIgnore] public int EffDamage => (int)(BonusDamage * (1.0 + 0.12 * UpgradeLevel)) + RuneSum(r => r.BonusDamage);
        [JsonIgnore] public int EffMagicDamage => (int)(BonusMagicDamage * (1.0 + 0.12 * UpgradeLevel)) + RuneSum(r => r.BonusMagicDamage);
        [JsonIgnore] public int EffArmor => (int)(BonusArmor * (1.0 + 0.12 * UpgradeLevel)) + RuneSum(r => r.BonusArmor);
        [JsonIgnore] public int EffHp => (int)(BonusHp * (1.0 + 0.12 * UpgradeLevel)) + RuneSum(r => r.BonusHp);
        [JsonIgnore] public int EffMp => (int)(BonusMp * (1.0 + 0.12 * UpgradeLevel)) + RuneSum(r => r.BonusMp);
        [JsonIgnore] public int EffStrength => (int)(BonusStrength * (1.0 + 0.12 * UpgradeLevel)) + RuneSum(r => r.BonusStrength);
        [JsonIgnore] public int EffDexterity => (int)(BonusDexterity * (1.0 + 0.12 * UpgradeLevel)) + RuneSum(r => r.BonusDexterity);
        [JsonIgnore] public int EffIntelligence => (int)(BonusIntelligence * (1.0 + 0.12 * UpgradeLevel)) + RuneSum(r => r.BonusIntelligence);
        [JsonIgnore] public int EffEndurance => (int)(BonusEndurance * (1.0 + 0.12 * UpgradeLevel)) + RuneSum(r => r.BonusEndurance);
        [JsonIgnore] public double EffCritChance => BonusCritChance * (1.0 + 0.12 * UpgradeLevel) + RuneDoubleSum(r => r.BonusCritChance);
        [JsonIgnore] public double EffDodgeChance => BonusDodgeChance * (1.0 + 0.12 * UpgradeLevel) + RuneDoubleSum(r => r.BonusDodgeChance);

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
        public int MaxUpgradeLevel => Rarity switch
        {
            ItemRarity.Common => 1,
            ItemRarity.Magic => 2,
            ItemRarity.Rare => 3,
            ItemRarity.Epic => 4,
            ItemRarity.Legendary => 5,
            _ => 1
        };

        [JsonIgnore]
        public bool IsMaxUpgraded => UpgradeLevel >= MaxUpgradeLevel;

        [JsonIgnore]
        public string DisplayName => UpgradeLevel > 0 ? $"{Name} +{UpgradeLevel}" : Name;

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
                // Этап 3.2: показываются эффективные бонусы (учитывают улучшение +X и вставленные руны)
                var list = new System.Collections.Generic.List<string>();
                if (EffDamage > 0) list.Add($"+{EffDamage} Урон");
                if (EffMagicDamage > 0) list.Add($"+{EffMagicDamage} Маг.урон");
                if (EffArmor > 0) list.Add($"+{EffArmor} Броня");
                if (EffHp > 0) list.Add($"+{EffHp} HP");
                if (EffMp > 0) list.Add($"+{EffMp} MP");
                if (EffStrength > 0) list.Add($"+{EffStrength} Сила");
                if (EffDexterity > 0) list.Add($"+{EffDexterity} Ловкость");
                if (EffIntelligence > 0) list.Add($"+{EffIntelligence} Интеллект");
                if (EffEndurance > 0) list.Add($"+{EffEndurance} Выносливость");
                if (EffCritChance > 0) list.Add($"+{EffCritChance:F1}% Крит");
                if (EffDodgeChance > 0) list.Add($"+{EffDodgeChance:F1}% Уклон");

                if (RuneSlots > 0)
                {
                    var runes = (InsertedRunes ?? new List<string>())
                        .Select(id => RuneDatabase.Find(id)?.Icon + " " + RuneDatabase.Find(id)?.Name)
                        .Where(s => !string.IsNullOrWhiteSpace(s));
                    list.Add($"💠 {InsertedRunes?.Count ?? 0}/{RuneSlots}" + (runes.Any() ? $" ({string.Join(", ", runes)})" : ""));
                }
                if (UpgradeLevel > 0) list.Add($"⭐ Ур.{UpgradeLevel}/{MaxUpgradeLevel}");
                if (list.Count == 0) return Description;
                return string.Join(", ", list);
            }
        }
    }
}
