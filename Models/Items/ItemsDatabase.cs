using System.Collections.Generic;

namespace EpicBattle.Models.Items
{
    public static class ItemsDatabase
    {
        public static readonly List<Item> AllItems = new List<Item>
        {
            // ==========================================
            // === АКТ 1: ОРУЖИЕ ===
            // ==========================================
            new Item { Id = "W_Sword_Common", Name = "Железный меч", Description = "Надежный клинок пехотинца.", Slot = ItemSlot.Weapon, Rarity = ItemRarity.Common, IconPath = "🗡️", BonusDamage = 6, GoldValue = 15 , IsUpgradable = true},
            new Item { Id = "W_Staff_Common", Name = "Посох ученика", Description = "Деревянный посох, резонирующий с маной.", Slot = ItemSlot.Weapon, Rarity = ItemRarity.Common, IconPath = "🪄", BonusMagicDamage = 8, BonusMp = 15, GoldValue = 15 , IsUpgradable = true},
            new Item { Id = "W_Daggers_Common", Name = "Охотничьи кинжалы", Description = "Быстрые парные лезвия.", Slot = ItemSlot.Weapon, Rarity = ItemRarity.Common, IconPath = "🔪", BonusDamage = 5, BonusCritChance = 5.0, BonusDodgeChance = 3.0, GoldValue = 15 , IsUpgradable = true},
            new Item { Id = "W_Broadsword_Rare", Name = "Клеймор Закалки", Description = "Тяжелый меч из черной руды.", Slot = ItemSlot.Weapon, Rarity = ItemRarity.Rare, IconPath = "⚔️", RequiredLevel = 2, BonusDamage = 14, BonusStrength = 3, BonusArmor = 2, GoldValue = 50 , IsUpgradable = true},
            new Item { Id = "W_ArchmageStaff_Epic", Name = "Посох Тайного Огня", Description = "Наконечник посоха пылает первородной магией.", Slot = ItemSlot.Weapon, Rarity = ItemRarity.Epic, IconPath = "🔥", RequiredLevel = 3, BonusMagicDamage = 20, BonusIntelligence = 5, BonusMp = 30, GoldValue = 120 , IsUpgradable = true},
            new Item { Id = "W_ShadowBlades_Legendary", Name = "Клинки Громгара", Description = "Трофейное оружие верховного вождя, несущее смерть.", Slot = ItemSlot.Weapon, Rarity = ItemRarity.Legendary, IconPath = "⚡", RequiredLevel = 4, BonusDamage = 25, BonusDexterity = 6, BonusCritChance = 12.0, BonusDodgeChance = 8.0, GoldValue = 250 },

            // ==========================================
            // === АКТ 2: НОВОЕ ОРУЖИЕ ===
            // ==========================================
            new Item { Id = "W_DarkBlade_Epic", Name = "Теневой Клинок Малкора", Description = "Лезвие источает холодную тьму, пожирая плоть врагов.", Slot = ItemSlot.Weapon, Rarity = ItemRarity.Epic, IconPath = "🗡️", RequiredLevel = 3, BonusDamage = 22, BonusStrength = 4, BonusCritChance = 8.0, GoldValue = 140 , IsUpgradable = true},
            new Item { Id = "W_GolemCleaver_Epic", Name = "Рассекатель Голема", Description = "Выкован из монолитных обломков рунного хранителя кузницы.", Slot = ItemSlot.Weapon, Rarity = ItemRarity.Epic, IconPath = "🪓", RequiredLevel = 4, BonusDamage = 26, BonusStrength = 6, BonusArmor = 5, GoldValue = 160 , IsUpgradable = true},
            new Item { Id = "W_ShadowStaff_Epic", Name = "Посох Теневого Культа", Description = "Стеклянное навершие пульсирует маной глубин Бездны.", Slot = ItemSlot.Weapon, Rarity = ItemRarity.Epic, IconPath = "🔮", RequiredLevel = 3, BonusMagicDamage = 25, BonusIntelligence = 6, BonusMp = 45, GoldValue = 150 , IsUpgradable = true},
            new Item { Id = "W_AssassinDaggers_Epic", Name = "Клыки Ночной Тени", Description = "Острые как бритва клинки с гравировкой бесшумного убийцы.", Slot = ItemSlot.Weapon, Rarity = ItemRarity.Epic, IconPath = "🔪", RequiredLevel = 3, BonusDamage = 18, BonusDexterity = 7, BonusCritChance = 15.0, BonusDodgeChance = 10.0, GoldValue = 150 , IsUpgradable = true},
            new Item { Id = "W_SolarBlade_Legendary", Name = "Клинок Солнечного Очищения", Description = "Древний освященный меч, раскалывающий любую тьму и скверну.", Slot = ItemSlot.Weapon, Rarity = ItemRarity.Legendary, IconPath = "☀️", RequiredLevel = 5, BonusDamage = 32, BonusStrength = 7, BonusCritChance = 15.0, BonusHp = 50, GoldValue = 350 },

            // ==========================================
            // === ШЛЕМЫ ===
            // ==========================================
            new Item { Id = "H_LeatherCap", Name = "Кожаный капюшон", Description = "Легкий головной убор разведчика.", Slot = ItemSlot.Helmet, Rarity = ItemRarity.Common, IconPath = "🧢", BonusArmor = 2, BonusDodgeChance = 2.0, GoldValue = 10 , IsUpgradable = true},
            new Item { Id = "H_IronHelm", Name = "Стальной шлем", Description = "Прочная защита головы от рубящих ударов.", Slot = ItemSlot.Helmet, Rarity = ItemRarity.Magic, IconPath = "👑", BonusArmor = 4, BonusHp = 15, GoldValue = 25 , IsUpgradable = true},
            new Item { Id = "H_CrownOfFire", Name = "Орочий Венец Шамана", Description = "Костяной венец, усиливающий магическую концентрацию.", Slot = ItemSlot.Helmet, Rarity = ItemRarity.Rare, IconPath = "👑", RequiredLevel = 2, BonusArmor = 3, BonusIntelligence = 4, BonusMp = 20, GoldValue = 60 , IsUpgradable = true},
            new Item { Id = "H_ShadowHood_Rare", Name = "Теневой Капюшон Лазутчика", Description = "Скрывает лицо воина во тьме, обостряя слух и зрение.", Slot = ItemSlot.Helmet, Rarity = ItemRarity.Rare, IconPath = "🥷", RequiredLevel = 3, BonusArmor = 5, BonusDexterity = 4, BonusDodgeChance = 6.0, GoldValue = 80 , IsUpgradable = true},
            new Item { Id = "H_ArchmageCrown_Epic", Name = "Корона Теневого Архимага", Description = "Фокусирует потоки магии, снижая затраты на заклинания.", Slot = ItemSlot.Helmet, Rarity = ItemRarity.Epic, IconPath = "👑", RequiredLevel = 4, BonusArmor = 6, BonusIntelligence = 7, BonusMp = 40, GoldValue = 170 , IsUpgradable = true},
            new Item { Id = "H_DwarvenHelm_Legendary", Name = "Шлем Горных Королей", Description = "Кованое золото и мифрил древней цитадели.", Slot = ItemSlot.Helmet, Rarity = ItemRarity.Legendary, IconPath = "👑", RequiredLevel = 5, BonusArmor = 12, BonusHp = 50, BonusStrength = 5, GoldValue = 280 },

            // ==========================================
            // === ДОСПЕХИ ===
            // ==========================================
            new Item { Id = "C_LeatherArmor", Name = "Кожаная куртка", Description = "Гибкая броня для быстрого перемещения.", Slot = ItemSlot.Chest, Rarity = ItemRarity.Common, IconPath = "🥋", BonusArmor = 3, BonusDexterity = 1, GoldValue = 15 , IsUpgradable = true},
            new Item { Id = "C_Chainmail", Name = "Кольчужный доспех", Description = "Тяжелые звенья стали отлично гасят удары.", Slot = ItemSlot.Chest, Rarity = ItemRarity.Magic, IconPath = "🛡️", BonusArmor = 7, BonusHp = 25, BonusEndurance = 2, GoldValue = 40 , IsUpgradable = true},
            new Item { Id = "C_MageRobe", Name = "Мантия Тайных Сил", Description = "Ткань с рунической вышивкой поглощает чары.", Slot = ItemSlot.Chest, Rarity = ItemRarity.Rare, IconPath = "🥻", RequiredLevel = 2, BonusArmor = 4, BonusIntelligence = 5, BonusMp = 35, GoldValue = 75 , IsUpgradable = true},
            new Item { Id = "C_GromgarChestplate", Name = "Панцирь Несокрушимости", Description = "Выкован из обломков орочьих алтарей.", Slot = ItemSlot.Chest, Rarity = ItemRarity.Epic, IconPath = "🛡️", RequiredLevel = 3, BonusArmor = 12, BonusHp = 50, BonusStrength = 4, BonusEndurance = 4, GoldValue = 180 , IsUpgradable = true},
            new Item { Id = "C_ShadowArmor_Epic", Name = "Доспех Теневого Плетения", Description = "Ткань пропитана теневой пылью, снижающей урон от магии и стрел.", Slot = ItemSlot.Chest, Rarity = ItemRarity.Epic, IconPath = "🥻", RequiredLevel = 4, BonusArmor = 14, BonusHp = 60, BonusEndurance = 5, GoldValue = 200 , IsUpgradable = true},
            new Item { Id = "C_ArchmageRobe_Legendary", Name = "Мантия Первородной Тьмы", Description = "Одеяние самого Малкора, пульсирующее мощью Бездны.", Slot = ItemSlot.Chest, Rarity = ItemRarity.Legendary, IconPath = "✨", RequiredLevel = 5, BonusArmor = 10, BonusMagicDamage = 15, BonusIntelligence = 9, BonusMp = 70, GoldValue = 350 },
            new Item { Id = "C_DwarvenPlatemail_Legendary", Name = "Панцирь Горного Титана", Description = "Легендарная броня защитников Сердца Кряжа.", Slot = ItemSlot.Chest, Rarity = ItemRarity.Legendary, IconPath = "🛡️", RequiredLevel = 5, BonusArmor = 20, BonusHp = 90, BonusEndurance = 7, BonusStrength = 5, GoldValue = 380 },

            // ==========================================
            // === ПОНОЖИ ===
            // ==========================================
            new Item { Id = "B_LeatherBoots", Name = "Кожаные сапоги", Description = "Удобная обувь для долгих переходов.", Slot = ItemSlot.Boots, Rarity = ItemRarity.Common, IconPath = "👢", BonusArmor = 2, BonusDodgeChance = 3.0, GoldValue = 10 , IsUpgradable = true},
            new Item { Id = "B_IronGreaves", Name = "Латные сапоги", Description = "Тяжелые стальные сапоги стража.", Slot = ItemSlot.Boots, Rarity = ItemRarity.Magic, IconPath = "🥾", BonusArmor = 5, BonusEndurance = 1, GoldValue = 25 , IsUpgradable = true},
            new Item { Id = "B_WindWalkers", Name = "Сапоги Ветроступа", Description = "Делают шаги легкими как дуновение ветра.", Slot = ItemSlot.Boots, Rarity = ItemRarity.Rare, IconPath = "💨", RequiredLevel = 2, BonusArmor = 3, BonusDexterity = 4, BonusDodgeChance = 7.0, GoldValue = 70 , IsUpgradable = true},
            new Item { Id = "B_ShadowStriders_Epic", Name = "Поножи Теневого Шага", Description = "Позволяют мгновенно уклоняться от самых стремительных атак.", Slot = ItemSlot.Boots, Rarity = ItemRarity.Epic, IconPath = "🥾", RequiredLevel = 3, BonusArmor = 7, BonusDexterity = 6, BonusDodgeChance = 11.0, GoldValue = 130 , IsUpgradable = true},
            new Item { Id = "B_TitanGreaves_Legendary", Name = "Сапоги Каменного Титана", Description = "Несокрушимые гранитные пластины защищают ноги героя.", Slot = ItemSlot.Boots, Rarity = ItemRarity.Legendary, IconPath = "🥾", RequiredLevel = 5, BonusArmor = 10, BonusHp = 40, BonusEndurance = 5, GoldValue = 240 },

            // ==========================================
            // === КОЛЬЦА И АМУЛЕТЫ ===
            // ==========================================
            new Item { Id = "R_RubyRing", Name = "Кольцо Рубиновой Ярости", Description = "Камень пульсирует жаждой битвы.", Slot = ItemSlot.Ring, Rarity = ItemRarity.Magic, IconPath = "💍", BonusStrength = 3, BonusCritChance = 4.0, GoldValue = 35 , IsUpgradable = true},
            new Item { Id = "R_SapphireBand", Name = "Сапфировый перстень", Description = "Хранит запас тайной энергии.", Slot = ItemSlot.Ring, Rarity = ItemRarity.Magic, IconPath = "💍", BonusIntelligence = 3, BonusMp = 25, GoldValue = 35 , IsUpgradable = true},
            new Item { Id = "R_RuneOfProtection", Name = "Кольцо Защиты Предков", Description = "Древнее кольцо клана Бранна.", Slot = ItemSlot.Ring, Rarity = ItemRarity.Rare, IconPath = "✨", BonusArmor = 4, BonusHp = 30, GoldValue = 80 , IsUpgradable = true},
            new Item { Id = "R_ShadowRing_Rare", Name = "Кольцо Затмения", Description = "Преломляет свет, даруя невидимость и ловкость.", Slot = ItemSlot.Ring, Rarity = ItemRarity.Rare, IconPath = "💍", RequiredLevel = 3, BonusCritChance = 7.0, BonusDodgeChance = 6.0, BonusDexterity = 4, GoldValue = 90 , IsUpgradable = true},
            new Item { Id = "R_RingOfArcaneFury_Epic", Name = "Перстень Тайной Ярости", Description = "Древнее кольцо, многократно усиливающее мощь атакующей магии.", Slot = ItemSlot.Ring, Rarity = ItemRarity.Epic, IconPath = "💍", RequiredLevel = 4, BonusMagicDamage = 12, BonusIntelligence = 6, BonusMp = 35, GoldValue = 160 , IsUpgradable = true},

            new Item { Id = "A_WolfTooth", Name = "Амулет Волчьего Клыка", Description = "Обостряет реакцию и инстинкты.", Slot = ItemSlot.Amulet, Rarity = ItemRarity.Magic, IconPath = "📿", BonusDexterity = 3, BonusCritChance = 5.0, BonusDodgeChance = 4.0, GoldValue = 40 , IsUpgradable = true},
            new Item { Id = "A_ShamanCharm", Name = "Оберег Шамана Варга", Description = "Священный тотем шаманов орды.", Slot = ItemSlot.Amulet, Rarity = ItemRarity.Rare, IconPath = "🧿", RequiredLevel = 2, BonusIntelligence = 4, BonusEndurance = 3, BonusHp = 25, BonusMp = 25, GoldValue = 90 , IsUpgradable = true},
            new Item { Id = "A_GolemHeart_Epic", Name = "Сердце Рунного Голема", Description = "Пульсирующий монолитный кристалл, дарующий силу камня.", Slot = ItemSlot.Amulet, Rarity = ItemRarity.Epic, IconPath = "💎", RequiredLevel = 3, BonusArmor = 8, BonusHp = 60, BonusEndurance = 5, GoldValue = 190 , IsUpgradable = true},
            new Item { Id = "A_EyeOfShadows_Rare", Name = "Око Бездны", Description = "Теневой амулет, открывающий слабости в броне врагов.", Slot = ItemSlot.Amulet, Rarity = ItemRarity.Rare, IconPath = "👁️", RequiredLevel = 3, BonusIntelligence = 5, BonusDexterity = 5, BonusCritChance = 8.0, GoldValue = 110 , IsUpgradable = true},
            new Item { Id = "A_SolarEmblem_Legendary", Name = "Эмблема Солнечного Очищения", Description = "Благословение предков, хранящее баланс сил и гармонию.", Slot = ItemSlot.Amulet, Rarity = ItemRarity.Legendary, IconPath = "☀️", RequiredLevel = 5, BonusHp = 60, BonusMp = 40, BonusDamage = 10, BonusArmor = 7, GoldValue = 320 },

            // ==========================================
            // === КВЕСТОВЫЕ ПРЕДМЕТЫ ===
            // ==========================================
            new Item { Id = "Q_VillageKey", Name = "Ключ от сундука в Пепельном Ручье", Description = "Старый железный ключ старосты деревни.", Slot = ItemSlot.Quest, Rarity = ItemRarity.Rare, IconPath = "🗝️", GoldValue = 0 },
            new Item { Id = "Q_ForestMap", Name = "Карта засады в лесу", Description = "План скрытых орочьих троп через горное ущелье.", Slot = ItemSlot.Quest, Rarity = ItemRarity.Rare, IconPath = "🗺️", GoldValue = 0 },
            new Item { Id = "Q_AncientTome", Name = "Древний фолиант тайн", Description = "Уцелевшая рукопись с описанием печатей Цитадели.", Slot = ItemSlot.Quest, Rarity = ItemRarity.Epic, IconPath = "📜", GoldValue = 0 },
            new Item { Id = "Q_CitadelKey", Name = "Ключ от Внутренней Цитадели", Description = "Тяжелый черный ключ, снятый со стража Врат.", Slot = ItemSlot.Quest, Rarity = ItemRarity.Rare, IconPath = "🗝️", GoldValue = 0 },
            new Item { Id = "Q_ShadowGem", Name = "Теневой Кристалл Каэля", Description = "Ключевой камень ритуала, способный расколоть магический барьер Малкора.", Slot = ItemSlot.Quest, Rarity = ItemRarity.Epic, IconPath = "🔮", GoldValue = 0 },
            new Item { Id = "Q_PurificationElixir", Name = "Эликсир Очищения Скверны", Description = "Священное снадобье, дающее иммунитет к теневой магии.", Slot = ItemSlot.Quest, Rarity = ItemRarity.Epic, IconPath = "🧪", GoldValue = 0 }
        };

        public static Item? Clone(string id)
        {
            var orig = AllItems.Find(i => i.Id == id);
            if (orig == null) return null;
            return new Item
            {
                Id = orig.Id,
                Name = orig.Name,
                Description = orig.Description,
                Slot = orig.Slot,
                Rarity = orig.Rarity,
                IconPath = orig.IconPath,
                RequiredLevel = orig.RequiredLevel,
                GoldValue = orig.GoldValue,
                BonusDamage = orig.BonusDamage,
                BonusMagicDamage = orig.BonusMagicDamage,
                BonusArmor = orig.BonusArmor,
                BonusHp = orig.BonusHp,
                BonusMp = orig.BonusMp,
                BonusStrength = orig.BonusStrength,
                BonusDexterity = orig.BonusDexterity,
                BonusIntelligence = orig.BonusIntelligence,
                BonusEndurance = orig.BonusEndurance,
                BonusCritChance = orig.BonusCritChance,
                BonusDodgeChance = orig.BonusDodgeChance,
                IsUpgradable = orig.IsUpgradable,
                UpgradeLevel = orig.UpgradeLevel
            };
        }
    }
}
