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

        // P1-фикс (В4): перезарядка вражеских навыков. Ключ — Id навыка.
        // Лечилки/баффы без КД делали элитных Инквизиторов и боссов «стеной» (бессмертный FirstAid).
        private readonly Dictionary<string, int> _skillCooldowns = new Dictionary<string, int>();

        /// <summary>Тик перезарядок — вызывать в начале хода существа.</summary>
        public void TickSkillCooldowns()
        {
            var ready = new List<string>();
            foreach (var kv in _skillCooldowns)
            {
                if (kv.Value > 1) _skillCooldowns[kv.Key] = kv.Value - 1;
                else ready.Add(kv.Key);
            }
            foreach (var id in ready) _skillCooldowns.Remove(id);
        }

        public bool IsSkillOnCooldown(string skillId) =>
            _skillCooldowns.TryGetValue(skillId, out int cd) && cd > 0;

        public void StartSkillCooldown(string skillId, int turns)
        {
            if (turns > 0) _skillCooldowns[skillId] = turns;
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

            // P1-фикс (В6): Тактик больше не «вечный ослепитель». Ослепление — с перезарядкой
            // и реальным расходом ресурсов (MP/AP списываются в ExecuteEnemyTurn).
            bool tacticianCanBlind = Role == EnemyRole.Tactician
                && !IsSkillOnCooldown("T_Blind")
                && Mp >= 8 && CurrentAP >= 2
                && !player.ActiveEffects.Any(e => e.Id == "Blind");

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

                // P1-фикс (В4): лечение с перезарядкой — иначе шаман «бессмертен» для группы
                if (injuredAlly != null && healSkill != null && Mp >= healSkill.ManaCost && CurrentAP >= healSkill.APCost && !IsSkillOnCooldown("V1_FirstAid"))
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Heal, EstimatedValue = 30, IconPath = "🌿", Description = $"Исцеление ({injuredAlly.Name})" };
                }
                else if (fireSkill != null && Mp >= fireSkill.ManaCost && CurrentAP >= fireSkill.APCost && !IsSkillOnCooldown("M1_Fireball") && rand.Next(100) < 70)
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Cast, EstimatedValue = BaseMagicDamage, IconPath = "🔥", Description = "Огненный шар (Магия/AoE)" };
                }
                else if (!player.ActiveEffects.Any(e => e.Id == "Blind") && Mp >= 8 && CurrentAP >= 2 && !IsSkillOnCooldown("T_Blind") && rand.Next(100) < 40)
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
                    if (strikeSkill != null && CurrentAP >= strikeSkill.APCost && !IsSkillOnCooldown("S1_PowerStrike") && rand.Next(100) < 45)
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
            if (tacticianCanBlind)
            {
                CurrentIntent = new EnemyIntent { Type = IntentType.Cast, IconPath = "👁️", Description = "Ослепление" };
            }
            else
            {
                var tactStrike = AvailableSkills.FirstOrDefault(s => s.Id == "S1_PowerStrike");
                if (tactStrike != null && CurrentAP >= tactStrike.APCost && Mp >= tactStrike.ManaCost && !IsSkillOnCooldown("S1_PowerStrike") && rand.Next(100) < 35)
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Attack, EstimatedValue = (int)(BaseDamage * 1.5), IconPath = "💥", Description = "Мощный удар" };
                }
                else
                {
                    CurrentIntent = new EnemyIntent { Type = IntentType.Attack, EstimatedValue = BaseDamage, IconPath = "⚔️", Description = "Точный удар" };
                }
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
            CritMultiplier = state.PlayerClass == "Наемник" ? 1.6 : 1.5; // P1-баланс: нерф критика Наёмника (было 1.75)
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
