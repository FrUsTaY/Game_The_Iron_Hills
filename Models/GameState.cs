using System;
using System.Collections.Generic;

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

        // --- Текущее состояние ---
        public int PlayerHp { get; set; } = 100;
        public int PlayerMp { get; set; } = 50;

        // --- Производные характеристики (Вычисляются на лету или хранятся тут) ---
        // Делаем сеттеры для совместимости со старым кодом, хотя они теперь должны зависеть от статов.
        // При прокачке/изменении статов нужно будет пересчитывать, либо отказаться от прямой установки.
        // Для совместимости с BattleView оставим auto-properties, но инициализируем их на старте
        public int PlayerMaxHp { get; set; } = 100;
        public int PlayerMaxMp { get; set; } = 50;
        public int PlayerBaseDamage { get; set; } = 15;
        public int PlayerBaseMagicDamage { get; set; } = 15;
        public double DodgeChance => PlayerClass == "Наемник" ? Math.Min(50, Dexterity * 1.5 + 5.0) : Math.Min(50, Dexterity * 1.5); // Max 50%
        public double CritChance => PlayerClass == "Наемник" ? Math.Min(50, Dexterity * 1.5 + 5.0) : Math.Min(50, Dexterity * 1.5); // Max 50%
        public int Defense => PlayerClass == "Ветеран" ? Endurance + 2 : Endurance; // Снижение входящего урона

        public void RecalculateDerivedStats()
        {
            PlayerMaxHp = PlayerClass == "Ветеран" ? 60 + (Endurance * 10) : 50 + (Endurance * 10);
            PlayerMaxMp = 20 + (Intelligence * 6);
            PlayerBaseDamage = 5 + (Strength * 2);
            PlayerBaseMagicDamage = 5 + (Intelligence * 2);
        }

        // Инвентарь базовый
        public int HpPotions { get; set; } = 3;
        public int MpPotions { get; set; } = 2;
        public int Gold { get; set; } = 0;

        // Изученные навыки
        public List<string> UnlockedSkills { get; set; } = new List<string>();

        // Сюжет (для Режима Истории)
        public string CurrentNodeId { get; set; } = "Start";
        public bool IsStoryMode { get; set; } = true;

        // Инвентарь и Перки (Сюжет)
        public bool HasTraitRuthless { get; set; } = false;
        public bool HasForestMap { get; set; } = false;
        public bool HasVillageChestKey { get; set; } = false;

        // Квесты и решения (Сцена 2)
        public bool HasQuestAncestorsLegacy { get; set; } = false;
        public bool KnowsAboutCitadelSeals { get; set; } = false;
        public bool UnlockedAmbush { get; set; } = false;
        public bool PromisedRuneOfProtection { get; set; } = false;

        // Квесты и решения (Сцена 3)
        public bool HasTomeIntact { get; set; } = false;
        public bool HasShamanCharm { get; set; } = false;

        // Квесты и решения (Сцена 4)
        public bool HonorGromgar { get; set; } = false;
    }
}
