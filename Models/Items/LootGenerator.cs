using EpicBattle.Models.Combat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EpicBattle.Models.Items
{
    public static class LootGenerator
    {
        private static readonly RngProxy rand = new RngProxy();

        /// <summary>Шанс найти Железную Руду Кряжа после боя (материал для Кузнеца, Этап 3.2).</summary>
        public static int RollOreDrop(bool isEliteOrBoss)
        {
            int chance = isEliteOrBoss ? 90 : 35;
            if (rand.Next(100) >= chance) return 0;
            if (isEliteOrBoss && rand.Next(100) < 40) return 2; // элита/босс может дать сразу два куска
            return 1;
        }

        /// <summary>Этап 3.2 (часть 2): шанс выпадения рунического камня с боя.</summary>
        public static bool RollRuneDrop(bool isBoss, out string runeId, out string runeName, out string runeIcon)
        {
            // обычный бой — почти без рун; элита ~35%; босс гарантированно + шанс на Титана
            int chance = isBoss ? 100 : 35;
            runeId = runeName = runeIcon = "";
            if (rand.Next(100) >= chance) return false;
            var stone = RuneDatabase.RollRandomDrop(isBoss);
            runeId = stone.Id; runeName = stone.Name; runeIcon = stone.Icon;
            return true;
        }

        public static List<Item> GenerateBattleLoot(int playerLevel, bool isBossFight, bool isEliteFight, out int goldReward)
        {
            var loot = new List<Item>();
            goldReward = rand.Next(15 + playerLevel * 3, 26 + playerLevel * 5);
            if (isBossFight)
                goldReward = (int)(goldReward * 2.0);
            else if (isEliteFight)
                goldReward = (int)(goldReward * 1.5);

            // P0-баланс: шанс выпадения шмота 35% в обычном бою, 75% элита, 100% босс
            int dropChance = isBossFight ? 100 : (isEliteFight ? 75 : 35);
            if (rand.Next(100) < dropChance)
            {
                ItemRarity rarity;
                int roll = rand.Next(100);
                if (isBossFight)
                {
                    if (roll < 40) rarity = ItemRarity.Rare;
                    else if (roll < 80) rarity = ItemRarity.Epic;
                    else rarity = ItemRarity.Legendary;
                }
                else if (isEliteFight)
                {
                    if (roll < 55) rarity = ItemRarity.Magic;
                    else if (roll < 90) rarity = ItemRarity.Rare;
                    else rarity = ItemRarity.Epic;
                }
                else
                {
                    if (roll < 65) rarity = ItemRarity.Common;
                    else if (roll < 92) rarity = ItemRarity.Magic;
                    else rarity = ItemRarity.Rare;
                }

                var pool = ItemsDatabase.AllItems
                    .Where(i => i.Slot != ItemSlot.Quest && i.Slot != ItemSlot.Consumable && (i.Rarity == rarity || (isBossFight && i.Rarity >= ItemRarity.Rare)))
                    .ToList();

                if (pool.Count > 0)
                {
                    var picked = pool[rand.Next(pool.Count)];
                    loot.Add(ItemsDatabase.Clone(picked.Id));
                }
            }

            return loot;
        }
    }
}
