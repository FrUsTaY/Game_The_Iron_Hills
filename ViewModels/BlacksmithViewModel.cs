using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using EpicBattle.Managers;
using EpicBattle.Models;
using EpicBattle.Models.Items;

namespace EpicBattle.ViewModels
{
    /// <summary>
    /// Этап 3.2: Карточка предмета в кузнице — показывает текущие и будущие бонусы улучшения.
    /// </summary>
    public class ForgeItemCard : INotifyPropertyChanged
    {
        private static readonly Random Rand = new Random();

        public Item Item { get; set; } = new Item();
        public bool IsEquipped { get; set; }
        public int OreCount { get; set; }
        public int GoldCost { get; set; }
        public bool CanAfford => OreCount >= 1 && GoldCost > 0;

        public string Title => Item.DisplayName;
        public string Icon => Item.IconPath;
        public string RarityColor => Item.RarityColor;
        public string SlotName => Item.SlotDisplayName;
        public string CurrentStats => Item.FormattedStats;
        public string SourceLabel => IsEquipped ? "🛡️ Надето" : "🎒 Инвентарь";

        public string CostText => GoldCost <= 0
            ? "⭐ Максимум"
            : $"⛏️ {OreCount} + 💰 {GoldCost}";

        /// <summary>Предпросмотр: как изменятся ключевые бонусы после +1 уровня.</summary>
        public string PreviewText
        {
            get
            {
                if (GoldCost <= 0) return "Дальше улучшать нельзя.";
                double cur = 1.0 + 0.25 * Item.UpgradeLevel;
                double next = cur + 0.25;
                var parts = new List<string>();
                if (Item.BonusDamage > 0) parts.Add($"Урон {(int)(Item.BonusDamage * cur)}→{(int)(Item.BonusDamage * next)}");
                if (Item.BonusMagicDamage > 0) parts.Add($"Маг.урон {(int)(Item.BonusMagicDamage * cur)}→{(int)(Item.BonusMagicDamage * next)}");
                if (Item.BonusArmor > 0) parts.Add($"Броня {(int)(Item.BonusArmor * cur)}→{(int)(Item.BonusArmor * next)}");
                if (Item.BonusHp > 0) parts.Add($"HP {(int)(Item.BonusHp * cur)}→{(int)(Item.BonusHp * next)}");
                if (Item.BonusMp > 0) parts.Add($"MP {(int)(Item.BonusMp * cur)}→{(int)(Item.BonusMp * next)}");
                if (Item.BonusStrength > 0) parts.Add($"Сила {(int)(Item.BonusStrength * cur)}→{(int)(Item.BonusStrength * next)}");
                if (Item.BonusDexterity > 0) parts.Add($"Ловк. {(int)(Item.BonusDexterity * cur)}→{(int)(Item.BonusDexterity * next)}");
                if (Item.BonusIntelligence > 0) parts.Add($"Инт. {(int)(Item.BonusIntelligence * cur)}→{(int)(Item.BonusIntelligence * next)}");
                if (Item.BonusEndurance > 0) parts.Add($"Вын. {(int)(Item.BonusEndurance * cur)}→{(int)(Item.BonusEndurance * next)}");
                if (parts.Count == 0) return "+25% к скрытым свойствам.";
                return "После улучшения: " + string.Join(", ", parts);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Этап 3.2: Кузнец — улучшение экипировки (+1..+N) за золото и руду,
    /// перековка базовых характеристик и заточка оружия.
    /// </summary>
    public class BlacksmithViewModel : INotifyPropertyChanged
    {
        private readonly GameState _gameState;
        private readonly Action _onLeave;
        private static readonly Random rand = new Random();

        public ObservableCollection<ForgeItemCard> ForgeableItems { get; set; } = new ObservableCollection<ForgeItemCard>();

        public const int OrePrice = 30;   // 💰 стоимость одного куска руды у кузнеца
        public const int ReforgeCost = 80; // 💰 стоимость перековки
        public const int SharpenCost = 40; // 💰 стоимость заточки оружия

        public int PlayerGold => _gameState.Gold;
        public int OreStock => _gameState.Ore;
        public int UpgradeCount => _gameState.BlacksmithUpgradesDone;
        public bool CanReforge => _gameState.Gold >= CurrentReforgeCost; // P1 (Б5): цена растёт за спам
        public bool CanSharpen => _gameState.Gold >= SharpenCost;
        public bool CanBuyOre => _gameState.Gold >= OrePrice;

        private string _statusMessage = "Грр! Молот не дремлет! Приноси снаряжение — сделаю из него настоящее оружие. За золото продам и руду.";
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private string _reforgePreview = "";
        public string ReforgePreview
        {
            get => _reforgePreview;
            set { _reforgePreview = value; OnPropertyChanged(); }
        }

        public ICommand UpgradeItemCommand => new Command(param => UpgradeItem(param as ForgeItemCard));
        public ICommand BuyOreCommand => new Command(_ => BuyOre());
        public ICommand ReforgeCommand => new Command(_ => ReforgeStats());
        public ICommand SharpenCommand => new Command(_ => SharpenBlades());
        public ICommand LeaveSmithCommand => new Command(_ => _onLeave?.Invoke());

        public BlacksmithViewModel(Action onLeave)
        {
            _gameState = SaveManager.CurrentState;
            _onLeave = onLeave;
            RefreshForgeList();
        }

        private void RefreshForgeList()
        {
            ForgeableItems.Clear();

            var candidates = new List<(Item item, bool equipped)>();
            foreach (var eq in _gameState.GetEquippedList())
                candidates.Add((eq, true));
            foreach (var inv in _gameState.Inventory)
                if (inv.Slot != ItemSlot.Quest && inv.Slot != ItemSlot.Consumable)
                    candidates.Add((inv, false));

            foreach (var (item, equipped) in candidates.OrderByDescending(c => c.equipped))
            {
                int max = item.MaxUpgradeLevel;
                bool upgradable = item.IsUpgradable && item.UpgradeLevel < max;
                int cost = upgradable ? ComputeUpgradeCost(item) : 0;
                ForgeableItems.Add(new ForgeItemCard
                {
                    Item = item,
                    IsEquipped = equipped,
                    OreCount = Math.Min(_gameState.Ore, 1),
                    GoldCost = cost
                });
            }

            UpdateDerivedFlags();
        }

        private int ComputeUpgradeCost(Item item)
        {
            // Цена растёт с редкостью и текущим уровнем улучшения
            int rarityBase = item.Rarity switch
            {
                ItemRarity.Common => 20,
                ItemRarity.Magic => 35,
                ItemRarity.Rare => 60,
                ItemRarity.Epic => 95,
                ItemRarity.Legendary => 140,
                _ => 20
            };
            return rarityBase * (item.UpgradeLevel + 1);
        }

        private void UpgradeItem(ForgeItemCard? card)
        {
            if (card == null) return;
            if (card.GoldCost <= 0)
            {
                StatusMessage = $"❌ [{card.Title}] уже улучшен до максимума!";
                return;
            }
            if (_gameState.Ore < 1)
            {
                StatusMessage = $"❌ Не хватает руды ⛏️! Купи её у меня за {OrePrice} 💰.";
                return;
            }
            if (_gameState.Gold < card.GoldCost)
            {
                StatusMessage = $"❌ Недостаточно золота для ковки ({card.GoldCost} 💰)!";
                return;
            }

            _gameState.Gold -= card.GoldCost;
            _gameState.Ore -= 1;
            card.Item.UpgradeLevel += 1;
            _gameState.BlacksmithUpgradesDone += 1;
            _gameState.RecalculateDerivedStats();

            StatusMessage = $"🔨 КУЗНЕЦ: «Отличная работа!» [{card.Item.DisplayName}] улучшено до +{card.Item.UpgradeLevel}! Свойства усилены на 25%.";
            RefreshForgeList();
        }

        private void BuyOre()
        {
            if (_gameState.Gold < OrePrice)
            {
                StatusMessage = $"❌ Золота не хватит на руду ({OrePrice} 💰)!";
                return;
            }
            _gameState.Gold -= OrePrice;
            _gameState.Ore += 1;
            StatusMessage = "⛏️ Кузнец продал тебе кусок Железной Руды Кряжа.";
            RefreshForgeList();
        }

        private int _reforgeStreak = 0; // P1-фикс (Б5): растущая цена за спам перековкой подряд

        private int CurrentReforgeCost => ReforgeCost * (int)Math.Pow(2, Math.Min(_reforgeStreak, 3)); // 80 → 160 → 320 → 640 (кап ×8)

        private void ReforgeStats()
        {
            int cost = CurrentReforgeCost;
            if (_gameState.Gold < cost)
            {
                StatusMessage = $"❌ Перековка стоит {cost} 💰" + (_reforgeStreak > 0 ? " (цена растёт при частых перековках!)" : "") + "!";
                return;
            }

            // P1-фикс (Б5): гарантируем, что понижаемая характеристика > 1 — иначе очко
            // просто исчезало (золото списано, сумма статов падала). Сумма характеристик
            // теперь всегда сохраняется: −1 одной (только если она ≥ 2), +1 другой.
            var stats = new Dictionary<string, int>
            {
                ["Сила"] = _gameState.Strength,
                ["Ловкость"] = _gameState.Dexterity,
                ["Интеллект"] = _gameState.Intelligence,
                ["Выносливость"] = _gameState.Endurance
            };

            var decCandidates = stats.Where(kv => kv.Value > 1).Select(kv => kv.Key).ToList();
            if (decCandidates.Count == 0)
            {
                StatusMessage = "⚗️ Все характеристики уже на минимуме — перековать нечего!";
                return;
            }

            string dec = decCandidates[rand.Next(decCandidates.Count)];
            string inc = keys_excluding(stats.Keys.ToList(), dec)[rand.Next(stats.Count - 1)];

            _gameState.Gold -= cost;
            _reforgeStreak++;
            SetStat(dec, stats[dec] - 1);
            SetStat(inc, stats[inc] + 1);
            _gameState.RecalculateDerivedStats();

            StatusMessage = $"⚗️ ПЕРЕКОВКА ХАРАКТЕРИСТИК (−{cost} 💰): характер переплавлен! −1 {dec}, +1 {inc}." +
                            (_reforgeStreak >= 2 ? $" Следующая: {CurrentReforgeCost} 💰." : "");
            ReforgePreview = $"Новый облик героя: Сила {_gameState.Strength} • Ловкость {_gameState.Dexterity} • Интеллект {_gameState.Intelligence} • Выносливость {_gameState.Endurance}";
            RefreshForgeList();
            UpdateDerivedFlags();
        }

        private static List<string> keys_excluding(List<string> keys, string exclude)
        {
            var result = new List<string>(keys);
            result.Remove(exclude);
            return result;
        }

        private void SetStat(string name, int value)
        {
            switch (name)
            {
                case "Сила": _gameState.Strength = value; break;
                case "Ловкость": _gameState.Dexterity = value; break;
                case "Интеллект": _gameState.Intelligence = value; break;
                case "Выносливость": _gameState.Endurance = value; break;
            }
        }

        private void SharpenBlades()
        {
            if (_gameState.Gold < SharpenCost)
            {
                StatusMessage = $"❌ Заточка стоит {SharpenCost} 💰!";
                return;
            }
            _gameState.Gold -= SharpenCost;
            _gameState.PlayerBaseDamage += 1;
            _gameState.PlayerBaseMagicDamage += 1;
            StatusMessage = "⚔️ Клинок и посох наточены на вулканическом камне: +1 к базовому урону и магии (навсегда).";
            RefreshForgeList();
        }

        private void UpdateDerivedFlags()
        {
            OnPropertyChanged(nameof(PlayerGold));
            OnPropertyChanged(nameof(OreStock));
            OnPropertyChanged(nameof(UpgradeCount));
            OnPropertyChanged(nameof(CanReforge));
            OnPropertyChanged(nameof(CanSharpen));
            OnPropertyChanged(nameof(CanBuyOre));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
