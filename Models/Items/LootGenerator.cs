using System;
using System.Collections.Generic;
using System.Linq;

namespace EpicBattle.Models.Items
{
    public static class LootGenerator
    {
        private static readonly Random rand = new Random();

        public static List<Item> GenerateBattleLoot(int playerLevel, bool isBossFight, out int goldReward)
        {
            var loot = new List<Item>();
            goldReward = rand.Next(15 + playerLevel * 5, 30 + playerLevel * 10);
            if (isBossFight) goldReward *= 2;

            // Шанс выпадения предмета: 70% обычный бой, 100% босс
            if (isBossFight || rand.Next(100) < 75)
            {
                ItemRarity rarity;
                int roll = rand.Next(100);
                if (isBossFight)
                {
                    if (roll < 40) rarity = ItemRarity.Rare;
                    else if (roll < 80) rarity = ItemRarity.Epic;
                    else rarity = ItemRarity.Legendary;
                }
                else
                {
                    if (roll < 50) rarity = ItemRarity.Common;
                    else if (roll < 80) rarity = ItemRarity.Magic;
                    else if (roll < 95) rarity = ItemRarity.Rare;
                    else rarity = ItemRarity.Epic;
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
