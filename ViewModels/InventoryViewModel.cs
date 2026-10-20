using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using EpicBattle.Managers;
using EpicBattle.Models;
using EpicBattle.Models.Items;

namespace EpicBattle.ViewModels
{
    public class InventoryViewModel : INotifyPropertyChanged
    {
        private readonly GameState _gameState;
        private static readonly SolidColorBrush GrayBrush = new SolidColorBrush(Colors.Gray);

        static InventoryViewModel()
        {
            GrayBrush.Freeze();
        }

        public InventoryViewModel()
        {
            _gameState = SaveManager.CurrentState ?? new GameState();
            RefreshInventory();
        }

        public Action CloseAction { get; set; }

        public ObservableCollection<Item> InventoryItems { get; set; } = new ObservableCollection<Item>();

        private Item _selectedItem;
        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedItem));
                OnPropertyChanged(nameof(PlaceholderVisibility));
                OnPropertyChanged(nameof(DetailsVisibility));
                OnPropertyChanged(nameof(CanEquipSelected));
                OnPropertyChanged(nameof(CanUnequipSelected));
                // P2 (П4): свойства кнопки «Выпить» для расходников
                OnPropertyChanged(nameof(CanUseSelected));
                OnPropertyChanged(nameof(UseButtonText));
                OnPropertyChanged(nameof(SelectedItemSlot));
                OnPropertyChanged(nameof(SelectedItemName));
                OnPropertyChanged(nameof(SelectedItemRarityName));
                OnPropertyChanged(nameof(SelectedItemSlotName));
                OnPropertyChanged(nameof(SelectedItemStats));
                OnPropertyChanged(nameof(SelectedItemDesc));
                OnPropertyChanged(nameof(SelectedItemGold));
                OnPropertyChanged(nameof(SelectedItemBrush));
            }
        }

        public bool HasSelectedItem => SelectedItem != null;
        public Visibility PlaceholderVisibility => SelectedItem == null ? Visibility.Visible : Visibility.Collapsed;
        public Visibility DetailsVisibility => SelectedItem != null ? Visibility.Visible : Visibility.Collapsed;

        public string SelectedItemName => SelectedItem?.Name ?? string.Empty;
        public string SelectedItemRarityName => SelectedItem?.RarityName ?? string.Empty;
        public string SelectedItemSlotName => SelectedItem?.SlotDisplayName ?? string.Empty;
        public string SelectedItemStats => SelectedItem?.FormattedStats ?? string.Empty;
        public string SelectedItemDesc => SelectedItem?.Description ?? string.Empty;
        public int SelectedItemGold => SelectedItem?.GoldValue ?? 0;
        public SolidColorBrush SelectedItemBrush => SelectedItem?.RarityBrush ?? GrayBrush;

        public bool CanEquipSelected => SelectedItem != null &&
                                        SelectedItem.Slot != ItemSlot.Quest &&
                                        SelectedItem.Slot != ItemSlot.Consumable &&
                                        _gameState.PlayerLevel >= SelectedItem.RequiredLevel &&
                                        _gameState.Inventory != null &&
                                        _gameState.Inventory.Contains(SelectedItem);

        public bool CanUnequipSelected => SelectedItem != null &&
                                          _gameState.GetEquippedList().Contains(SelectedItem);

        // P2-фикс (П4): использование расходников из инвентаря.
        // Раньше enum Consumable существовал, но предметы этого типа нельзя было применить —
        // зелья были недоступны за пределами счётчиков HpPotions/MpPotions.
        public ItemSlot SelectedItemSlot => SelectedItem?.Slot ?? ItemSlot.Weapon;
        public string UseButtonText => SelectedItemSlot switch
        {
            ItemSlot.Consumable => "🧪 Выпить",
            ItemSlot.Quest => "",
            _ => ""
        };
        public bool CanUseSelected => SelectedItem != null &&
                                      SelectedItem.Slot == ItemSlot.Consumable &&
                                      _gameState.Inventory.Contains(SelectedItem);

        public void UseSelectedItem()
        {
            if (!CanUseSelected) return;
            var item = SelectedItem;
            // P2: эффект зелья описан в Description вида "+40 к здоровью" — парсим числа оттуда;
            // если не распарсилось — стандартные значения зелий.
            // Предпочитаем числовые поля предмета (Item.BonusHp/BonusMp), как в экипировке.
            int hp = item.BonusHp > 0 ? item.BonusHp : 0;
            int mp = item.BonusMp > 0 ? item.BonusMp : 0;
            var desc = item.Description ?? string.Empty;
            var mHp = System.Text.RegularExpressions.Regex.Match(desc, @"\+(\d+)[^0-9]*(здоров|HP|hp)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var mMp = System.Text.RegularExpressions.Regex.Match(desc, @"\+(\d+)[^0-9]*(ман|MP|mp)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (mHp.Success) hp = int.Parse(mHp.Groups[1].Value);
            if (mMp.Success) mp = int.Parse(mMp.Groups[1].Value);
            if (hp == 0 && mp == 0) { hp = 40; mp = 25; }

            bool healed = false;
            if (hp > 0 && _gameState.PlayerHp < _gameState.PlayerMaxHp)
            {
                _gameState.PlayerHp = Math.Min(_gameState.PlayerMaxHp, _gameState.PlayerHp + hp);
                healed = true;
            }
            if (mp > 0 && _gameState.PlayerMp < _gameState.PlayerMaxMp)
            {
                _gameState.PlayerMp = Math.Min(_gameState.PlayerMaxMp, _gameState.PlayerMp + mp);
                healed = true;
            }

            if (healed)
            {
                EpicBattle.Services.AudioEngine.Play("waterspray");
                _gameState.Inventory.Remove(item);
                RefreshInventory();
            }
        }

        // Слоты экипировки
                public string EquippedWeaponIcon => _gameState?.EquippedWeapon?.IconPath ?? "🗡️";
        public string EquippedHelmetIcon => _gameState?.EquippedHelmet?.IconPath ?? "👑";
        public string EquippedChestIcon => _gameState?.EquippedChest?.IconPath ?? "🛡️";
        public string EquippedBootsIcon => _gameState?.EquippedBoots?.IconPath ?? "👢";
        public string EquippedRingIcon => _gameState?.EquippedRing?.IconPath ?? "💍";
        public string EquippedAmuletIcon => _gameState?.EquippedAmulet?.IconPath ?? "📿";

public string EquippedWeaponName => _gameState.EquippedWeapon?.Name ?? "[Оружие: пусто]";
        public SolidColorBrush EquippedWeaponBrush => _gameState.EquippedWeapon?.RarityBrush ?? GrayBrush;

        public string EquippedHelmetName => _gameState.EquippedHelmet?.Name ?? "[Шлем: пусто]";
        public SolidColorBrush EquippedHelmetBrush => _gameState.EquippedHelmet?.RarityBrush ?? GrayBrush;

        public string EquippedChestName => _gameState.EquippedChest?.Name ?? "[Доспех: пусто]";
        public SolidColorBrush EquippedChestBrush => _gameState.EquippedChest?.RarityBrush ?? GrayBrush;

        public string EquippedBootsName => _gameState.EquippedBoots?.Name ?? "[Поножи: пусто]";
        public SolidColorBrush EquippedBootsBrush => _gameState.EquippedBoots?.RarityBrush ?? GrayBrush;

        public string EquippedRingName => _gameState.EquippedRing?.Name ?? "[Кольцо: пусто]";
        public SolidColorBrush EquippedRingBrush => _gameState.EquippedRing?.RarityBrush ?? GrayBrush;

        public string EquippedAmuletName => _gameState.EquippedAmulet?.Name ?? "[Амулет: пусто]";
        public SolidColorBrush EquippedAmuletBrush => _gameState.EquippedAmulet?.RarityBrush ?? GrayBrush;

        // Характеристики персонажа
        public string PlayerName => _gameState.PlayerName ?? "Элрик";
        public string PlayerClass => _gameState.PlayerClass ?? "Ветеран";
        public int PlayerLevel => _gameState.PlayerLevel;
        public int Gold => _gameState.Gold;

        public int HP => _gameState.PlayerMaxHp;
        public int MP => _gameState.PlayerMaxMp;
        public int Damage => _gameState.PlayerBaseDamage;
        public int MagicDamage => _gameState.PlayerBaseMagicDamage;
        public int Armor => _gameState.Defense;
        public double DodgeChance => _gameState.DodgeChance;
        public double CritChance => _gameState.CritChance;

        public int STR => _gameState.TotalStrength;
        public int DEX => _gameState.TotalDexterity;
        public int INT => _gameState.TotalIntelligence;
        public int END => _gameState.TotalEndurance;

        public string FormattedHpMp => $"{HP} / {MP}";
        public string FormattedDamage => $"{Damage} / {MagicDamage}";
        public string FormattedCritDodge => $"{CritChance:F1}% / {DodgeChance:F1}%";
        public string FormattedStrDex => $"{STR} / {DEX}";
        public string FormattedIntEnd => $"{INT} / {END}";

        public void RefreshInventory()
        {
            InventoryItems.Clear();
            if (_gameState.Inventory != null)
            {
                foreach (var item in _gameState.Inventory)
                {
                    InventoryItems.Add(item);
                }
            }

            OnPropertyChanged(nameof(EquippedWeaponName));
            OnPropertyChanged(nameof(EquippedWeaponBrush));
            OnPropertyChanged(nameof(EquippedHelmetName));
            OnPropertyChanged(nameof(EquippedHelmetBrush));
            OnPropertyChanged(nameof(EquippedChestName));
            OnPropertyChanged(nameof(EquippedChestBrush));
            OnPropertyChanged(nameof(EquippedBootsName));
            OnPropertyChanged(nameof(EquippedBootsBrush));
            OnPropertyChanged(nameof(EquippedRingName));
            OnPropertyChanged(nameof(EquippedRingBrush));
            OnPropertyChanged(nameof(EquippedAmuletName));
            OnPropertyChanged(nameof(EquippedAmuletBrush));

            OnPropertyChanged(nameof(Gold));
            OnPropertyChanged(nameof(HP));
            OnPropertyChanged(nameof(MP));
            OnPropertyChanged(nameof(Damage));
            OnPropertyChanged(nameof(MagicDamage));
            OnPropertyChanged(nameof(Armor));
            OnPropertyChanged(nameof(DodgeChance));
            OnPropertyChanged(nameof(CritChance));
            OnPropertyChanged(nameof(STR));
            OnPropertyChanged(nameof(DEX));
            OnPropertyChanged(nameof(INT));
            OnPropertyChanged(nameof(END));

            OnPropertyChanged(nameof(FormattedHpMp));
            OnPropertyChanged(nameof(FormattedDamage));
            OnPropertyChanged(nameof(FormattedCritDodge));
            OnPropertyChanged(nameof(FormattedStrDex));
            OnPropertyChanged(nameof(FormattedIntEnd));

            OnPropertyChanged(nameof(HasSelectedItem));
            OnPropertyChanged(nameof(PlaceholderVisibility));
            OnPropertyChanged(nameof(DetailsVisibility));
            OnPropertyChanged(nameof(CanEquipSelected));
            OnPropertyChanged(nameof(CanUnequipSelected));
        }

        public void EquipSelectedItem()
        {
            if (SelectedItem != null && CanEquipSelected)
            {
                var item = SelectedItem;
                if (item.Slot == ItemSlot.Weapon) EpicBattle.Services.AudioEngine.Play("sword_unsheath");
                else EpicBattle.Services.AudioEngine.Play("sword_sheath");
                _gameState.EquipItem(item);
                RefreshInventory();
                SelectedItem = item;
            }
        }

        public void UnequipSelectedItem()
        {
            if (SelectedItem != null && CanUnequipSelected)
            {
                EpicBattle.Services.AudioEngine.Play("click_alt");
                var item = SelectedItem;
                _gameState.UnequipSlot(item.Slot);
                RefreshInventory();
                SelectedItem = item;
            }
        }

        public void SelectSlot(string slotStr)
        {
            if (string.IsNullOrEmpty(slotStr)) return;

            Item found = slotStr switch
            {
                "Weapon" => _gameState.EquippedWeapon,
                "Helmet" => _gameState.EquippedHelmet,
                "Chest" => _gameState.EquippedChest,
                "Boots" => _gameState.EquippedBoots,
                "Ring" => _gameState.EquippedRing,
                "Amulet" => _gameState.EquippedAmulet,
                _ => null
            };

            SelectedItem = found;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
