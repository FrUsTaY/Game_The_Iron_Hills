using EpicBattle.Models.Combat;
using System;
using System.Collections.Generic;
using EpicBattle.Models.Items;

namespace EpicBattle.Models.Encounters
{
    public class EncounterChoice
    {
        public string Text { get; set; } = string.Empty;
        public string RequiredStat { get; set; } = string.Empty; // "STR", "DEX", "INT", "END", "Gold", "Hp"
        public int RequiredValue { get; set; } = 0;
        
        public string SuccessText { get; set; } = string.Empty;
        public string FailText { get; set; } = string.Empty;
        
        // Награды при успехе
        public int GoldReward { get; set; } = 0;
        public int HpReward { get; set; } = 0;
        public int MpReward { get; set; } = 0;
        public int StatPointsReward { get; set; } = 0;
        public int SkillPointsReward { get; set; } = 0;
        public string ItemIdReward { get; set; } = string.Empty;
        public int HpPotionsReward { get; set; } = 0;
        public int MpPotionsReward { get; set; } = 0;

        // Штрафы при неудаче или плата
        public int GoldCost { get; set; } = 0;
        public int HpDamage { get; set; } = 0;
        public int MpDamage { get; set; } = 0;
    }

    public class RandomEncounter
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = "📜";
        public string Description { get; set; } = string.Empty;
        public List<EncounterChoice> Choices { get; set; } = new List<EncounterChoice>();
        public System.Windows.Media.Imaging.BitmapImage? Image => EpicBattle.Services.ImageService.GetEncounterImage(
            Id switch
            {
                "BloodAltar" => "encounter_blood_altar",
                "GoblinMerchant" => "encounter_merchant_grix",
                "CampfireRest" => "encounter_abandoned_camp",
                "AncientChest" => "encounter_iron_chest",
                _ => "encounter_blood_altar"
            }
        );
    }

    public static class EncountersDatabase
    {
        public static List<RandomEncounter> Encounters = new List<RandomEncounter>
        {
            // 1. Древний Кровавый Алтарь (Risk vs Reward)
            new RandomEncounter
            {
                Id = "BloodAltar",
                Title = "Кровавый Алтарь Предков",
                Icon = "🩸",
                Description = "В глубине каменных руин вы натыкаетесь на пульсирующий багровым светом алтарь. Древние письмена гласят: «Отдай частицу жизненной силы в обмен на тайное могущество».",
                Choices = new List<EncounterChoice>
                {
                    new EncounterChoice
                    {
                        Text = "Принести жертву кровью (-30 HP -> +1 Очко Навыков)",
                        RequiredStat = "Hp",
                        RequiredValue = 35,
                        HpDamage = 30,
                        SkillPointsReward = 1,
                        SuccessText = "Острая боль пронзает ваше тело, но древняя магия алтаря открывает в вашем разуме новые боевые техники (+1 Очко Навыков, -30 HP)!",
                        FailText = "Вы слишком слабы, чтобы рисковать жизнью у алтаря."
                    },
                    new EncounterChoice
                    {
                        Text = "[Интеллект 7+] Расшифровать руны без жертвоприношения",
                        RequiredStat = "INT",
                        RequiredValue = 7,
                        StatPointsReward = 1,
                        SuccessText = "Благодаря знанию магии вы обходите защитный барьер алтаря и впитываете чистую энергию (+1 Очко Характеристик)!",
                        FailText = "Руны вспыхивают слепящим пламенем, отбрасывая вас назад (-15 HP)." ,
                        HpDamage = 15
                    },
                    new EncounterChoice
                    {
                        Text = "Осторожно пройти мимо",
                        SuccessText = "Вы решаете не искушать судьбу и продолжаете путь."
                    }
                }
            },

            // 2. Бродячий Торговец-Гоблин (Магазин и расходники)
            new RandomEncounter
            {
                Id = "GoblinMerchant",
                Title = "Бродячий Торговец Грикс",
                Icon = "👺",
                Description = "Из-за валуна робко выглядывает гоблин с гигантским мешком на спине: «Пс-с, путник! Не убивай Грикса! У Грикса есть первоклассные товары для храброго воина!»",
                Choices = new List<EncounterChoice>
                {
                    new EncounterChoice
                    {
                        Text = "Купить партию зелий (35 золота -> +2 Зелья HP, +1 Зелье MP)",
                        RequiredStat = "Gold",
                        RequiredValue = 35,
                        GoldCost = 35,
                        HpPotionsReward = 2,
                        MpPotionsReward = 1,
                        SuccessText = "Грикс довольно хихикает и вручает вам свежие, пахнущие мятой склянки с зельями!",
                        FailText = "У вас недостаточно золота. Грикс разочарованно качает головой."
                    },
                    new EncounterChoice
                    {
                        Text = "Купить Загадочный Мешок со снаряжением (50 золота)",
                        RequiredStat = "Gold",
                        RequiredValue = 50,
                        GoldCost = 50,
                        ItemIdReward = "R_RuneOfProtection",
                        SuccessText = "Внутри мешка оказывается старинное защитное кольцо с рунической гравировкой!",
                        FailText = "У вас не хватает золота на этот загадочный предмет."
                    },
                    new EncounterChoice
                    {
                        Text = "[Сила 8+] Припугнуть гоблина и забрать провиант",
                        RequiredStat = "STR",
                        RequiredValue = 8,
                        HpPotionsReward = 1,
                        GoldReward = 20,
                        SuccessText = "Вы грозно вынимаете оружие, и испуганный Грикс бросает кошель с 20 золота и зелье здоровья, улепетывая в кусты!",
                        FailText = "Грикс ловко бросает вам под ноги дымовую шашку и мгновенно исчезает."
                    },
                    new EncounterChoice
                    {
                        Text = "Пожелать торговцу удачи и идти дальше",
                        SuccessText = "Вы киваете гоблину и продолжаете свое путешествие."
                    }
                }
            },

            // 3. Заброшенный привал следопыта (Campfire / Rest & Upgrade)
            new RandomEncounter
            {
                Id = "CampfireRest",
                Title = "Заброшенный Привал у Ручья",
                Icon = "⛺",
                Description = "Вы находите безопасную расщелину в скалах с остатками старого кострища и чистым горным источником. Отличное место, чтобы перевести дух.",
                Choices = new List<EncounterChoice>
                {
                    new EncounterChoice
                    {
                        Text = "Отдохнуть и напиться воды (Полное восстановление HP и MP)",
                        HpReward = 999,
                        MpReward = 999,
                        SuccessText = "Прохладная родниковая вода и короткий сон полностью восстанавливают ваши жизненные силы и ману!"
                    },
                    new EncounterChoice
                    {
                        Text = "[Выносливость 7+] Заняться тренировкой и заточкой оружия (+1 Очко Характеристик)",
                        RequiredStat = "END",
                        RequiredValue = 7,
                        StatPointsReward = 1,
                        SuccessText = "Вы проводите время в усердных упражнениях, закаляя тело и повышая боевую форму (+1 Очко Характеристик)!",
                        FailText = "Усталость берет свое, тренировка не приносит плодов."
                    },
                    new EncounterChoice
                    {
                        Text = "[Ловкость 7+] Обыскать окрестные скалы на предмет тайников",
                        RequiredStat = "DEX",
                        RequiredValue = 7,
                        GoldReward = 40,
                        ItemIdReward = "B_LeatherBoots",
                        SuccessText = "В расселине скалы вы обнаруживаете спрятанный кожаный мешочек с 40 золота и легкие походные сапоги!",
                        FailText = "Вы оступаетесь на скользких камнях и ушибаете ногу (-10 HP).",
                        HpDamage = 10
                    }
                }
            },

            // 4. Запертый Сундук Древней Стражи (Lock & Trap)
            new RandomEncounter
            {
                Id = "AncientChest",
                Title = "Запертый Кованый Сундук",
                Icon = "📦",
                Description = "Посреди тропы стоит массивный сундук из черного железа, запечатанный сложным замком с арбалетной ловушкой на крышке.",
                Choices = new List<EncounterChoice>
                {
                    new EncounterChoice
                    {
                        Text = "[Ловкость 8+] Обезвредить скрытую ловушку и взломать замок",
                        RequiredStat = "DEX",
                        RequiredValue = 8,
                        GoldReward = 60,
                        ItemIdReward = "C_Chainmail",
                        SuccessText = "Вы аккуратно перерезаете натяжную нить арбалета и отмычкой щелкаете замок. Внутри — 60 золота и добротный кольчужный доспех!",
                        FailText = "Щелчок! Ловушка срабатывает, и отравленный дротик вонзается вам в плечо (-25 HP)!",
                        HpDamage = 25
                    },
                    new EncounterChoice
                    {
                        Text = "[Сила 9+] Сбить замок одним сокрушительным ударом",
                        RequiredStat = "STR",
                        RequiredValue = 9,
                        GoldReward = 45,
                        SuccessText = "Мощный удар разносит замок в щепки! Вы успеваете закрыться щитом от сработавшей ловушки и забираете 45 золота!",
                        FailText = "Сталь сундука оказывается слишком прочной, отдача от удара повреждает вам кисть (-15 HP).",
                        HpDamage = 15
                    },
                    new EncounterChoice
                    {
                        Text = "Не рисковать и обойти сундук стороной",
                        SuccessText = "Вы благоразумно обходите подозрительный сундук и продолжаете путь."
                    }
                }
            }
        };

        private static readonly RngProxy _rnd = new RngProxy();

        public static RandomEncounter GetRandomEncounter()
        {
            int idx = _rnd.Next(Encounters.Count);
            return Encounters[idx];
        }
    }
}
