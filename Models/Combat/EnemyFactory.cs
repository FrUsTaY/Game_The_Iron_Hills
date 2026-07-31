using System;
using System.Collections.Generic;
using EpicBattle.Models.RPG;

namespace EpicBattle.Models.Combat
{
    public static class EnemyFactory
    {
        private static Random rand = new Random();

        public static List<BattleEnemy> GenerateArcadeEnemies(int level, int count = 1)
        {
            var enemies = new List<BattleEnemy>();
            for(int i = 0; i < count; i++)
            {
                int type = rand.Next(3);
                BattleEnemy enemy = new BattleEnemy();

                enemy.MaxAP = 4;
                enemy.CurrentAP = 4;

                if (type == 0) // Aggressor
                {
                    enemy.Name = "Орк-берсерк";
                    enemy.Role = EnemyRole.Aggressor;
                    enemy.MaxHp = 50 + (level * 10);
                    enemy.Agility = 8 + level;
                    enemy.BaseDamage = 15 + (level * 2);
                    enemy.Armor = 2;
                }
                else if (type == 1) // Defender
                {
                    enemy.Name = "Орк-щитоносец";
                    enemy.Role = EnemyRole.Defender;
                    enemy.MaxHp = 80 + (level * 15);
                    enemy.Agility = 4 + level;
                    enemy.BaseDamage = 8 + level;
                    enemy.Armor = 10 + level;
                }
                else // Support
                {
                    enemy.Name = "Орк-шаман";
                    enemy.Role = EnemyRole.Support;
                    enemy.MaxHp = 40 + (level * 8);
                    enemy.MaxMp = 30 + (level * 10);
                    enemy.Mp = enemy.MaxMp;
                    enemy.Agility = 6 + level;
                    enemy.BaseDamage = 5 + level;
                    enemy.BaseMagicDamage = 12 + level;
                    enemy.Armor = 1;
                    enemy.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "M1_Fireball"));
                    enemy.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "V1_FirstAid"));
                }

                enemy.Hp = enemy.MaxHp;
                enemies.Add(enemy);
            }
            return enemies;
        }

        public static List<BattleEnemy> GenerateStoryEnemies(string returnNodeId, string battleModifier)
        {
            var enemies = new List<BattleEnemy>();

            if (returnNodeId == "PostBattle1")
            {
                var enemy = new BattleEnemy
                {
                    Name = "Орк Углук",
                    Role = EnemyRole.Aggressor,
                    MaxHp = 100,
                    Agility = 10,
                    BaseDamage = 15,
                    Armor = 3,
                    MaxAP = 4, CurrentAP = 4
                };
                enemy.Hp = enemy.MaxHp;
                enemies.Add(enemy);
            }
            else if (returnNodeId == "PostBattle_Scene4")
            {
                var boss = new BattleEnemy
                {
                    Name = "Вождь Громгар",
                    Role = EnemyRole.Aggressor,
                    MaxHp = 200,
                    Agility = 12,
                    BaseDamage = 22,
                    Armor = 5,
                    MaxAP = 4, CurrentAP = 4
                };
                boss.Hp = boss.MaxHp;
                boss.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S2_Cleave"));
                enemies.Add(boss);
            }
            else if (returnNodeId == "PostBattle_Scene3")
            {
                // Для простоты объединим волны в один бой или заспавним Варга сразу с охраной
                var varg = new BattleEnemy
                {
                    Name = "Шаман Варг",
                    Role = EnemyRole.Support,
                    MaxHp = 150,
                    MaxMp = 100,
                    Mp = 100,
                    Agility = 8,
                    BaseDamage = 10,
                    BaseMagicDamage = 25,
                    Armor = 2,
                    MaxAP = 4, CurrentAP = 4
                };
                varg.Hp = varg.MaxHp;
                varg.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "M1_Fireball"));
                varg.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "V1_FirstAid"));

                if (battleModifier == "SkipTurn")
                {
                    varg.ApplyEffect(new StatusEffect { Id = "Stun", Name = "Оглушение", Type = StatusEffectType.Debuff, Duration = 1, Value = 1, IconPath = "💫" });
                }

                enemies.Add(varg);

                var guard = new BattleEnemy
                {
                    Name = "Телохранитель",
                    Role = EnemyRole.Defender,
                    MaxHp = 80,
                    Agility = 5,
                    BaseDamage = 10,
                    Armor = 10,
                    MaxAP = 4, CurrentAP = 4
                };
                guard.Hp = guard.MaxHp;
                enemies.Add(guard);
            }
            else
            {
                // Фолбэк
                enemies = GenerateArcadeEnemies(1, 1);
            }

            return enemies;
        }
    }
}
