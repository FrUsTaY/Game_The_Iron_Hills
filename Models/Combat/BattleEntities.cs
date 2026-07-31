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
        public int EstimatedValue { get; set; } // Например, урон или объем лечения
        public string IconPath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class BattleEnemy : CombatEntity
    {
        public EnemyRole Role { get; set; }
        public EnemyIntent CurrentIntent { get; set; } = new EnemyIntent();
        public List<Skill> AvailableSkills { get; set; } = new List<Skill>();
        public bool IsTargeted { get; set; }

        public void DetermineIntent(List<BattleEnemy> activeEnemies, BattlePlayer player)
        {
            if (Hp <= 0)
            {
                CurrentIntent = new EnemyIntent { Type = IntentType.None, IconPath = "💀", Description = "Мертв" };
                return;
            }

            if (Role == EnemyRole.Aggressor)
            {
                if ((float)Hp / MaxHp < 0.3f && !ActiveEffects.Any(e => e.Id == "Rage"))
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Cast, IconPath = "😡", Description = "Впасть в ярость" };
                }
                else
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Attack, EstimatedValue = (int)(BaseDamage * 1.5), IconPath = "⚔️", Description = "Тяжелая атака" };
                }
            }
            else if (Role == EnemyRole.Defender)
            {
                bool lowAlly = activeEnemies.Any(e => e.Hp > 0 && (float)e.Hp / e.MaxHp < 0.5f);
                // Если уже защищается, то не уходит в бесконечный цикл защиты, а переходит в контратаку
                if ((lowAlly || (float)Hp / MaxHp < 0.5f) && !IsDefending)
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Defend, IconPath = "🛡️", Description = "Защита" };
                }
                else
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Attack, EstimatedValue = BaseDamage, IconPath = "⚔️", Description = "Атака" };
                }
            }
            else if (Role == EnemyRole.Support)
            {
                var injuredAlly = activeEnemies.FirstOrDefault(e => e.Hp > 0 && e.Hp < e.MaxHp);
                var healSkill = AvailableSkills.FirstOrDefault(s => s.Id == "V1_FirstAid");
                var fireSkill = AvailableSkills.FirstOrDefault(s => s.Id == "M1_Fireball");

                if (injuredAlly != null && healSkill != null && Mp >= healSkill.ManaCost && CurrentAP >= healSkill.APCost)
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Heal, EstimatedValue = 30, IconPath = "🌿", Description = "Исцеление союзника" };
                }
                else if (fireSkill != null && Mp >= fireSkill.ManaCost && CurrentAP >= fireSkill.APCost)
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Cast, EstimatedValue = BaseMagicDamage, IconPath = "🔥", Description = "Огненный шар (AoE)" };
                }
                else
                {
                    // Fallback, если нет маны или AP
                    CurrentIntent = new EnemyIntent { Type = IntentType.Attack, EstimatedValue = BaseDamage, IconPath = "🪄", Description = "Атака посохом" };
                }
            }
            else // Tactician
            {
                if (!player.ActiveEffects.Any(e => e.Id == "Blind"))
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Cast, IconPath = "👁️", Description = "Ослепление" };
                }
                else
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Attack, EstimatedValue = BaseDamage, IconPath = "⚔️", Description = "Атака" };
                }
            }
        }
    }

    public class BattlePlayer : CombatEntity
    {
        public int HpPotions { get; set; }
        public int MpPotions { get; set; }
        public List<Skill> UnlockedSkills { get; set; } = new List<Skill>();

        public BattlePlayer(GameState state)
        {
            Name = state.PlayerName;
            MaxHp = state.PlayerMaxHp;
            Hp = state.PlayerHp;
            MaxMp = state.PlayerMaxMp;
            Mp = state.PlayerMp;
            MaxAP = 4;
            CurrentAP = 4;
            Agility = state.Dexterity;
            BaseDamage = state.PlayerBaseDamage;
            BaseMagicDamage = state.PlayerBaseMagicDamage;
            Armor = state.Defense;
            HpPotions = state.HpPotions;
            MpPotions = state.MpPotions;

            foreach(var id in state.UnlockedSkills)
            {
                var skill = SkillsDatabase.AllSkills.Find(s => s.Id == id);
                if(skill != null) UnlockedSkills.Add(skill);
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
