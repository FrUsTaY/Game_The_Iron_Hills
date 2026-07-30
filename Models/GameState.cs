using System;

namespace EpicBattle.Models
{
    public class GameState
    {
        public string SaveName { get; set; } = "Новое сохранение";
        public DateTime SaveDate { get; set; } = DateTime.Now;

        // Игрок
        public string PlayerName { get; set; } = "Элрик";
        public int PlayerLevel { get; set; } = 1;
        public int PlayerHp { get; set; } = 100;
        public int PlayerMaxHp { get; set; } = 100;
        public int PlayerMp { get; set; } = 50;
        public int PlayerMaxMp { get; set; } = 50;
        public int PlayerBaseDamage { get; set; } = 15;
        public int PlayerBaseMagicDamage { get; set; } = 30;

        public int HpPotions { get; set; } = 3;
        public int MpPotions { get; set; } = 2;

        // Сюжет (для Режима Истории)
        public string CurrentNodeId { get; set; } = "Start";
        public bool IsStoryMode { get; set; } = true;

        // Инвентарь и Перки (Сюжет)
        public int Gold { get; set; } = 0;
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