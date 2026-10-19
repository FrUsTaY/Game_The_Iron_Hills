using System;
using System.Collections.Generic;
using System.Linq;
using EpicBattle.Models.Items;
using EpicBattle.Models.Map;

namespace EpicBattle.Models
{
    public class GameState
    {
        public string SaveName { get; set; } = "Новое сохранение";
        public DateTime SaveDate { get; set; } = DateTime.Now;

        // --- Игрок (Базовые данные) ---
        public string PlayerName { get; set; } = "Элрик";
        public string PlayerClass { get; set; } = "Ветеран"; // Ветеран, Изгой-маг, Наемник
        public int PlayerLevel { get; set; } = 1;
        public int PlayerExperience { get; set; } = 0;
        public int ExperienceToNextLevel { get; set; } = 100;

        // Очки прокачки
        public int StatPoints { get; set; } = 0;
        public int SkillPoints { get; set; } = 0;

        // --- Базовые Характеристики (RPG) ---
        public int Strength { get; set; } = 5;
        public int Dexterity { get; set; } = 5;
        public int Intelligence { get; set; } = 5;
        public int Endurance { get; set; } = 5;

        // --- Экипировка и Инвентарь ---
        public List<Item> Inventory { get; set; } = new List<Item>();
        public Item? EquippedWeapon { get; set; }
        public Item? EquippedHelmet { get; set; }
        public Item? EquippedChest { get; set; }
        public Item? EquippedBoots { get; set; }
        public Item? EquippedRing { get; set; }
        public Item? EquippedAmulet { get; set; }

        // Этап 3.2: эффективные бонусы предмета с учетом кузнечных улучшений (+25% за уровень)
        public static double UpgradeMultiplier(Item item) => 1.0 + 0.25 * (item?.UpgradeLevel ?? 0);

        public List<Item> GetEquippedList()
        {
            var list = new List<Item>();
            if (EquippedWeapon != null) list.Add(EquippedWeapon);
            if (EquippedHelmet != null) list.Add(EquippedHelmet);
            if (EquippedChest != null) list.Add(EquippedChest);
            if (EquippedBoots != null) list.Add(EquippedBoots);
            if (EquippedRing != null) list.Add(EquippedRing);
            if (EquippedAmulet != null) list.Add(EquippedAmulet);
            return list;
        }

        public void EquipItem(Item item)
        {
            if (item == null) return;
            if (Inventory.Contains(item))
            {
                Inventory.Remove(item);
            }

            Item? oldItem = null;
            switch (item.Slot)
            {
                case ItemSlot.Weapon:
                    oldItem = EquippedWeapon;
                    EquippedWeapon = item;
                    break;
                case ItemSlot.Helmet:
                    oldItem = EquippedHelmet;
                    EquippedHelmet = item;
                    break;
                case ItemSlot.Chest:
                    oldItem = EquippedChest;
                    EquippedChest = item;
                    break;
                case ItemSlot.Boots:
                    oldItem = EquippedBoots;
                    EquippedBoots = item;
                    break;
                case ItemSlot.Ring:
                    oldItem = EquippedRing;
                    EquippedRing = item;
                    break;
                case ItemSlot.Amulet:
                    oldItem = EquippedAmulet;
                    EquippedAmulet = item;
                    break;
            }

            if (oldItem != null && !Inventory.Contains(oldItem))
            {
                Inventory.Add(oldItem);
            }

            RecalculateDerivedStats();
        }

        public void UnequipSlot(ItemSlot slot)
        {
            Item? removed = null;
            switch (slot)
            {
                case ItemSlot.Weapon:
                    removed = EquippedWeapon;
                    EquippedWeapon = null;
                    break;
                case ItemSlot.Helmet:
                    removed = EquippedHelmet;
                    EquippedHelmet = null;
                    break;
                case ItemSlot.Chest:
                    removed = EquippedChest;
                    EquippedChest = null;
                    break;
                case ItemSlot.Boots:
                    removed = EquippedBoots;
                    EquippedBoots = null;
                    break;
                case ItemSlot.Ring:
                    removed = EquippedRing;
                    EquippedRing = null;
                    break;
                case ItemSlot.Amulet:
                    removed = EquippedAmulet;
                    EquippedAmulet = null;
                    break;
            }

            if (removed != null)
            {
                if (!Inventory.Contains(removed))
                {
                    Inventory.Add(removed);
                }
                RecalculateDerivedStats();
            }
        }

        // --- Текущее состояние ---
        public int PlayerHp { get; set; } = 100;
        public int PlayerMp { get; set; } = 50;

        // --- Производные характеристики (включая экипировку) ---
        public int TotalStrength => Strength + (GetEquippedList()?.Sum(i => (int)(i.BonusStrength * UpgradeMultiplier(i))) ?? 0);
        public int TotalDexterity => Dexterity + (GetEquippedList()?.Sum(i => (int)(i.BonusDexterity * UpgradeMultiplier(i))) ?? 0);
        public int TotalIntelligence => Intelligence + (GetEquippedList()?.Sum(i => (int)(i.BonusIntelligence * UpgradeMultiplier(i))) ?? 0);
        public int TotalEndurance => Endurance + (GetEquippedList()?.Sum(i => (int)(i.BonusEndurance * UpgradeMultiplier(i))) ?? 0);

        public int PlayerMaxHp { get; set; } = 100;
        public int PlayerMaxMp { get; set; } = 50;
        public int PlayerBaseDamage { get; set; } = 15;
        public int PlayerBaseMagicDamage { get; set; } = 15;

        public double DodgeChance
        {
            get
            {
                double baseDodge = PlayerClass == "Наемник" ? TotalDexterity * 1.5 + 5.0 : TotalDexterity * 1.5;
                baseDodge += GetEquippedList()?.Sum(i => i.BonusDodgeChance * UpgradeMultiplier(i)) ?? 0;
                return Math.Min(60.0, baseDodge);
            }
        }

        public double CritChance
        {
            get
            {
                double baseCrit = PlayerClass == "Наемник" ? TotalDexterity * 1.5 + 5.0 : TotalDexterity * 1.5;
                baseCrit += GetEquippedList()?.Sum(i => i.BonusCritChance * UpgradeMultiplier(i)) ?? 0;
                return Math.Min(60.0, baseCrit);
            }
        }

        public int Defense
        {
            get
            {
                int baseDef = PlayerClass == "Ветеран" ? TotalEndurance + 2 : TotalEndurance;
                baseDef += GetEquippedList()?.Sum(i => (int)(i.BonusArmor * UpgradeMultiplier(i))) ?? 0;
                return baseDef;
            }
        }

        public void RecalculateDerivedStats()
        {
            var equipped = GetEquippedList();
            int bonusHp = equipped?.Sum(i => (int)(i.BonusHp * UpgradeMultiplier(i))) ?? 0;
            int bonusMp = equipped?.Sum(i => (int)(i.BonusMp * UpgradeMultiplier(i))) ?? 0;
            int bonusDmg = equipped?.Sum(i => (int)(i.BonusDamage * UpgradeMultiplier(i))) ?? 0;
            int bonusMagDmg = equipped?.Sum(i => (int)(i.BonusMagicDamage * UpgradeMultiplier(i))) ?? 0;

            PlayerMaxHp = (PlayerClass == "Ветеран" ? 60 + (TotalEndurance * 10) : 50 + (TotalEndurance * 10)) + bonusHp;
            PlayerMaxMp = 20 + (TotalIntelligence * 6) + bonusMp;
            PlayerBaseDamage = 5 + (TotalStrength * 2) + bonusDmg;
            PlayerBaseMagicDamage = 5 + (TotalIntelligence * 2) + bonusMagDmg;

            if (PlayerHp > PlayerMaxHp) PlayerHp = PlayerMaxHp;
            if (PlayerMp > PlayerMaxMp) PlayerMp = PlayerMaxMp;
        }

        // --- Этап 3.2: Ремесленная система (Кузнец) ---
        public int Ore { get; set; } = 0;                  // ⛏️ Железная руда Кряжа — материал для улучшений
        public int BlacksmithUpgradesDone { get; set; } = 0; // Сколько ковок совершал герой (для статистики)

        // Инвентарь базовый
        public int HpPotions { get; set; } = 3;
        public int MpPotions { get; set; } = 2;
        public int Gold { get; set; } = 50;

        // Изученные навыки
        public List<string> UnlockedSkills { get; set; } = new List<string>();

        // Сюжет (для Режима Истории)
        public string CurrentNodeId { get; set; } = "Start";
        public bool IsStoryMode { get; set; } = true;
        public int CurrentAct { get; set; } = 1;

        // --- Карта Мира (Этап 3.1: Интерактивная Карта Похода / Roguelike) ---
        public bool IsMapMode { get; set; } = false;
        public MapGraph? ActiveMap { get; set; }

        public void StartNewMap(int actNumber = 1)
        {
            IsMapMode = true;
            IsStoryMode = false;
            CurrentAct = actNumber;
            ActiveMap = MapGenerator.GenerateActMap(actNumber);
        }

        // Инвентарь и Перки (Акт 1)
        public bool HasTraitRuthless { get; set; } = false;
        public bool HasForestMap { get; set; } = false;
        public bool HasVillageChestKey { get; set; } = false;

        // Квесты и решения (Акт 1)
        public bool HasQuestAncestorsLegacy { get; set; } = false;
        public bool KnowsAboutCitadelSeals { get; set; } = false;
        public bool UnlockedAmbush { get; set; } = false;
        public bool PromisedRuneOfProtection { get; set; } = false;
        public bool HasTomeIntact { get; set; } = false;
        public bool HasShamanCharm { get; set; } = false;
        public bool HonorGromgar { get; set; } = false;

        // --- Квесты и решения (Акт 2: Тени Цитадели) ---
        public bool HasShadowGem { get; set; } = false;
        public bool GolemAllied { get; set; } = false;
        public bool SavedPrisonerKael { get; set; } = false;
        public bool KnowsMalkorWeakness { get; set; } = false;
        public bool HasPurificationElixir { get; set; } = false;
        public bool Act2Completed { get; set; } = false;

        public void SetupForAct2(string playerClass = "Ветеран")
        {
            PlayerClass = playerClass;
            PlayerLevel = 3;
            CurrentAct = 2;
            CurrentNodeId = "Act2_Start";
            IsStoryMode = true;
            Gold = 120;
            HpPotions = 4;
            MpPotions = 3;
            StatPoints = 2;
            SkillPoints = 1;

            Inventory.Clear();
            EquippedWeapon = null;
            EquippedHelmet = null;
            EquippedChest = null;
            EquippedBoots = null;
            EquippedRing = null;
            EquippedAmulet = null;
            UnlockedSkills.Clear();

            if (playerClass == "Ветеран")
            {
                Strength = 9; Dexterity = 6; Intelligence = 3; Endurance = 9;
                EquipItem(ItemsDatabase.Clone("W_Broadsword_Rare"));
                EquipItem(ItemsDatabase.Clone("C_Chainmail"));
                EquipItem(ItemsDatabase.Clone("H_IronHelm"));
                EquipItem(ItemsDatabase.Clone("B_IronGreaves"));
                UnlockedSkills.Add("S1_PowerStrike");
            }
            else if (playerClass == "Изгой-маг")
            {
                Strength = 3; Dexterity = 5; Intelligence = 12; Endurance = 5;
                EquipItem(ItemsDatabase.Clone("W_ArchmageStaff_Epic"));
                EquipItem(ItemsDatabase.Clone("C_MageRobe"));
                EquipItem(ItemsDatabase.Clone("H_CrownOfFire"));
                EquipItem(ItemsDatabase.Clone("R_SapphireBand"));
                UnlockedSkills.Add("M1_Fireball");
            }
            else // Наемник
            {
                Strength = 6; Dexterity = 12; Intelligence = 4; Endurance = 7;
                EquipItem(ItemsDatabase.Clone("W_Daggers_Common"));
                EquipItem(ItemsDatabase.Clone("C_LeatherArmor"));
                EquipItem(ItemsDatabase.Clone("B_WindWalkers"));
                EquipItem(ItemsDatabase.Clone("R_RubyRing"));
                UnlockedSkills.Add("V1_FirstAid");
            }

            RecalculateDerivedStats();
            PlayerHp = PlayerMaxHp;
            PlayerMp = PlayerMaxMp;
        }
    }
}
