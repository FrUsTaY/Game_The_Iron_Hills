using System;
using System.Collections.Generic;
using System.Linq;
using EpicBattle.Models.RPG;

namespace EpicBattle.Models.Combat
{
    public enum EnemyRole
    {
        Aggressor,
        Defender,
        Support,
        Tactician
    }

    public enum IntentType
    {
        None,
        Attack,
        Defend,
        Cast,
        Heal
    }

    public class EnemyIntent
    {
        public IntentType Type { get; set; }
        public int EstimatedValue { get; set; }
        public string IconPath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class BattleEnemy : CombatEntity
    {
        private static readonly Random rand = new Random();

        public EnemyRole Role { get; set; }
        public EnemyIntent CurrentIntent { get; set; } = new EnemyIntent();
        public List<Skill> AvailableSkills { get; set; } = new List<Skill>();

        private bool _isTargeted;
        public bool IsTargeted
        {
            get => _isTargeted;
            set { _isTargeted = value; OnPropertyChanged(); }
        }

        public void DetermineIntent(List<BattleEnemy> activeEnemies, BattlePlayer player)
        {
            if (Hp <= 0)
            {
                CurrentIntent = new EnemyIntent { Type = IntentType.None, IconPath = "💀", Description = "Мертв" };
                return;
            }

            if (ActiveEffects.Any(e => e.Id == "Stun"))
            {
                CurrentIntent = new EnemyIntent { Type = IntentType.None, IconPath = "💫", Description = "Оглушен" };
                return;
            }

            // P0-фикс (В1): фаза Ярости Громгара — единый источник истины в BattleEngine.ProcessTurnStart
            // (GromgarRageThreshold). Здесь больше нет дублирующей проверки 35%, включавшей ярость дважды.

            // 1. Уникальная логика босса Громгара
            if (Name.Contains("Громгар"))
            {
                var cleaveSkill = AvailableSkills.FirstOrDefault(s => s.Id == "S2_Cleave");
                if (cleaveSkill != null && CurrentAP >= cleaveSkill.APCost && rand.Next(100) < 40)
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Cast, EstimatedValue = BaseDamage, IconPath = "⚔️", Description = "Рассекающий удар" };
                    return;
                }

                var strikeSkill = AvailableSkills.FirstOrDefault(s => s.Id == "S1_PowerStrike");
                if (strikeSkill != null && CurrentAP >= strikeSkill.APCost && rand.Next(100) < 50)
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Attack, EstimatedValue = (int)(BaseDamage * 1.5), IconPath = "💥", Description = "Сокрушительный удар" };
                    return;
                }

                CurrentIntent = new EnemyIntent { Type = IntentType.Attack, EstimatedValue = BaseDamage, IconPath = "⚔️", Description = "Тяжелый удар топором" };
                return;
            }

            // 2. Логика Поддержки (Шаманы)
            if (Role == EnemyRole.Support)
            {
                var injuredAlly = activeEnemies
                    .Where(e => e.Hp > 0 && e.Hp < e.MaxHp * 0.75f)
                    .OrderBy(e => (float)e.Hp / e.MaxHp)
                    .FirstOrDefault();

                var healSkill = AvailableSkills.FirstOrDefault(s => s.Id == "V1_FirstAid");
                var fireSkill = AvailableSkills.FirstOrDefault(s => s.Id == "M1_Fireball");

                if (injuredAlly != null && healSkill != null && Mp >= healSkill.ManaCost && CurrentAP >= healSkill.APCost)
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Heal, EstimatedValue = 30, IconPath = "🌿", Description = $"Исцеление ({injuredAlly.Name})" };
                }
                else if (fireSkill != null && Mp >= fireSkill.ManaCost && CurrentAP >= fireSkill.APCost && rand.Next(100) < 70)
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Cast, EstimatedValue = BaseMagicDamage, IconPath = "🔥", Description = "Огненный шар (Магия/AoE)" };
                }
                else if (!player.ActiveEffects.Any(e => e.Id == "Blind") && Mp >= 8 && rand.Next(100) < 40)
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Cast, IconPath = "👁️", Description = "Ослепление" };
                }
                else
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Attack, EstimatedValue = BaseDamage, IconPath = "🪄", Description = "Удар посохом" };
                }
                return;
            }

            // 3. Логика Защитника (Щитоносцы/Телохранители)
            if (Role == EnemyRole.Defender)
            {
                bool allyInDanger = activeEnemies.Any(e => e != this && e.Hp > 0 && (float)e.Hp / e.MaxHp < 0.5f);
                if ((allyInDanger || (float)Hp / MaxHp < 0.4f) && !IsDefending)
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Defend, IconPath = "🛡️", Description = "Глухая защита" };
                }
                else
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Attack, EstimatedValue = BaseDamage, IconPath = "⚔️", Description = "Атака щитом" };
                }
                return;
            }

            // 4. Логика Агрессора (Берсерки/Разведчики)
            if (Role == EnemyRole.Aggressor)
            {
                if ((float)Hp / MaxHp < 0.35f && !ActiveEffects.Any(e => e.Id == "Rage"))
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Cast, IconPath = "😡", Description = "Впасть в ярость" };
                }
                else
                {
                    var strikeSkill = AvailableSkills.FirstOrDefault(s => s.Id == "S1_PowerStrike");
                    if (strikeSkill != null && CurrentAP >= strikeSkill.APCost && rand.Next(100) < 45)
                    {
                        CurrentIntent = new EnemyIntent { Type = IntentType.Attack, EstimatedValue = (int)(BaseDamage * 1.5), IconPath = "💥", Description = "Мощный удар" };
                    }
                    else
                    {
                        CurrentIntent = new EnemyIntent { Type = IntentType.Attack, EstimatedValue = BaseDamage, IconPath = "⚔️", Description = "Яростная атака" };
                    }
                }
                return;
            }

            // 5. Логика Тактика
            if (!player.ActiveEffects.Any(e => e.Id == "Blind"))
            {
                CurrentIntent = new EnemyIntent { Type = IntentType.Cast, IconPath = "👁️", Description = "Ослепление" };
            }
            else
            {
                CurrentIntent = new EnemyIntent { Type = IntentType.Attack, EstimatedValue = BaseDamage, IconPath = "⚔️", Description = "Точный удар" };
            }
        }
    }

    public class BattlePlayer : CombatEntity
    {
        public string PlayerClass { get; set; } = "Ветеран";
        public int HpPotions { get; set; }
        public int MpPotions { get; set; }
        public List<Skill> UnlockedSkills { get; set; } = new List<Skill>();

        public BattlePlayer(GameState state)
        {
            Name = state.PlayerName;
            PlayerClass = state.PlayerClass;
            MaxHp = state.PlayerMaxHp;
            Hp = Math.Min(state.PlayerHp, state.PlayerMaxHp);
            MaxMp = state.PlayerMaxMp;
            Mp = Math.Min(state.PlayerMp, state.PlayerMaxMp);
            MaxAP = 4;
            CurrentAP = 4;
            Agility = state.Dexterity;
            BaseDamage = state.PlayerBaseDamage;
            BaseMagicDamage = state.PlayerBaseMagicDamage;
            Armor = state.Defense;
            DodgeChance = state.DodgeChance;
            CritChance = state.CritChance;
            CritMultiplier = state.PlayerClass == "Наемник" ? 1.75 : 1.5;
            HpPotions = state.HpPotions;
            MpPotions = state.MpPotions;

            foreach (var id in state.UnlockedSkills)
            {
                var skill = SkillsDatabase.AllSkills.Find(s => s.Id == id);
                if (skill != null && !UnlockedSkills.Any(s => s.Id == skill.Id))
                {
                    UnlockedSkills.Add(skill);
                }
            }

            if (UnlockedSkills.Any(s => s.Id == "V2_ThickSkin"))
            {
                Armor += 5;
            }
            if (UnlockedSkills.Any(s => s.Id == "S3_SwordMastery"))
            {
                BaseDamage = (int)(BaseDamage * 1.10);
            }
        }

        public void UpdateGameState(GameState state)
        {
            state.PlayerHp = Hp;
            state.PlayerMp = Mp;
            state.HpPotions = HpPotions;
            state.MpPotions = MpPotions;
        }
    }
}
