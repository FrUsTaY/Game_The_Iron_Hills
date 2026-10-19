using System;
using System.Collections.Generic;
using EpicBattle.Models.RPG;

namespace EpicBattle.Models.Combat
{
    public static class EnemyFactory
    {
        private static readonly Random rand = new Random();

        public static int GetArcadeTotalWaves(int level)
        {
            if (level <= 2) return 1;
            if (level <= 5) return 2;
            return 3;
        }

        public static int GetStoryTotalWaves(string returnNodeId)
        {
            if (returnNodeId == "PostBattle_Scene3") return 3; // Акт 1: 3 волны перед лагерем Варга
            if (returnNodeId == "Act2_Ending_Decisions") return 2; // Акт 2: 2 фазы финального боя с Малкором
            return 1; // Все остальные боссы и мини-боссы - 1 волна
        }

        public static List<BattleEnemy> GenerateArcadeWave(int level, int wave, int totalWaves)
        {
            var enemies = new List<BattleEnemy>();
            int count = (wave == totalWaves && level >= 3) ? 1 : rand.Next(1, 3);

            for (int i = 0; i < count; i++)
            {
                int type;
                if (wave == totalWaves && level >= 3)
                {
                    type = rand.Next(2) == 0 ? 0 : 2;
                }
                else
                {
                    type = rand.Next(3);
                }

                BattleEnemy enemy = new BattleEnemy();
                enemy.MaxAP = 4;
                enemy.CurrentAP = 4;

                if (type == 0) // Берсерк
                {
                    enemy.Name = wave == totalWaves && totalWaves > 1 ? "Элитный Берсерк" : "Орк-берсерк";
                    enemy.Role = EnemyRole.Aggressor;
                    enemy.MaxHp = 55 + (level * 12) + (wave * 5);
                    enemy.Agility = 8 + level;
                    enemy.BaseDamage = 14 + (level * 2);
                    enemy.Armor = 2 + (wave > 1 ? 1 : 0);
                    enemy.CritChance = 15;
                    enemy.DodgeChance = 5;
                    enemy.CritMultiplier = 1.5;
                    enemy.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S1_PowerStrike"));
                }
                else if (type == 1) // Щитоносец
                {
                    enemy.Name = "Орк-щитоносец";
                    enemy.Role = EnemyRole.Defender;
                    enemy.MaxHp = 75 + (level * 14) + (wave * 8);
                    enemy.Agility = 4 + level;
                    enemy.BaseDamage = 8 + level;
                    enemy.Armor = 7 + level;
                    enemy.CritChance = 5;
                    enemy.DodgeChance = 0;
                    enemy.CritMultiplier = 1.3;
                }
                else // Шаман
                {
                    enemy.Name = wave == totalWaves && totalWaves > 1 ? "Верховный Шаман" : "Орк-шаман";
                    enemy.Role = EnemyRole.Support;
                    enemy.MaxHp = 45 + (level * 8) + (wave * 5);
                    enemy.MaxMp = 40 + (level * 10);
                    enemy.Mp = enemy.MaxMp;
                    enemy.Agility = 6 + level;
                    enemy.BaseDamage = 6 + level;
                    enemy.BaseMagicDamage = 13 + level;
                    enemy.Armor = 1;
                    enemy.CritChance = 5;
                    enemy.DodgeChance = 6;
                    enemy.CritMultiplier = 1.4;
                    enemy.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "M1_Fireball"));
                    enemy.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "V1_FirstAid"));
                }

                enemy.Hp = enemy.MaxHp;
                enemies.Add(enemy);
            }

            return enemies;
        }

        public static List<BattleEnemy> GenerateStoryWave(string returnNodeId, string battleModifier, int wave)
        {
            var enemies = new List<BattleEnemy>();

            // ==========================================
            // === АКТ 1 БОИ ===
            // ==========================================
            if (returnNodeId == "PostBattle1")
            {
                var enemy = new BattleEnemy
                {
                    Name = "Орк Углук",
                    Role = EnemyRole.Aggressor,
                    MaxHp = 100,
                    Agility = 10,
                    BaseDamage = 16,
                    Armor = 3,
                    CritChance = 10,
                    DodgeChance = 5,
                    CritMultiplier = 1.5,
                    MaxAP = 4,
                    CurrentAP = 4
                };
                enemy.Hp = enemy.MaxHp;
                enemy.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S1_PowerStrike"));
                enemies.Add(enemy);
            }
            else if (returnNodeId == "PostBattle_Scene3")
            {
                if (wave == 1)
                {
                    var scout = new BattleEnemy
                    {
                        Name = "Орк-разведчик",
                        Role = EnemyRole.Aggressor,
                        MaxHp = 60,
                        Agility = 11,
                        BaseDamage = 12,
                        Armor = 2,
                        CritChance = 12,
                        DodgeChance = 10,
                        CritMultiplier = 1.5,
                        MaxAP = 4,
                        CurrentAP = 4
                    };
                    scout.Hp = scout.MaxHp;
                    enemies.Add(scout);
                }
                else if (wave == 2)
                {
                    var guard1 = new BattleEnemy
                    {
                        Name = "Орочий страж",
                        Role = EnemyRole.Defender,
                        MaxHp = 75,
                        Agility = 5,
                        BaseDamage = 10,
                        Armor = 6,
                        CritChance = 5,
                        DodgeChance = 0,
                        CritMultiplier = 1.3,
                        MaxAP = 4,
                        CurrentAP = 4
                    };
                    guard1.Hp = guard1.MaxHp;
                    enemies.Add(guard1);

                    var berserk = new BattleEnemy
                    {
                        Name = "Орк-рубака",
                        Role = EnemyRole.Aggressor,
                        MaxHp = 70,
                        Agility = 8,
                        BaseDamage = 14,
                        Armor = 3,
                        CritChance = 10,
                        DodgeChance = 5,
                        CritMultiplier = 1.5,
                        MaxAP = 4,
                        CurrentAP = 4
                    };
                    berserk.Hp = berserk.MaxHp;
                    enemies.Add(berserk);
                }
                else // Волна 3: Шаман Варг + Телохранитель
                {
                    var varg = new BattleEnemy
                    {
                        Name = "Шаман Варг",
                        Role = EnemyRole.Support,
                        MaxHp = 140,
                        MaxMp = 100,
                        Mp = 100,
                        Agility = 8,
                        BaseDamage = 10,
                        BaseMagicDamage = 22,
                        Armor = 3,
                        CritChance = 10,
                        DodgeChance = 5,
                        CritMultiplier = 1.5,
                        MaxAP = 4,
                        CurrentAP = 4
                    };
                    varg.Hp = varg.MaxHp;
                    varg.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "M1_Fireball"));
                    varg.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "V1_FirstAid"));
                    enemies.Add(varg);

                    var bodyguard = new BattleEnemy
                    {
                        Name = "Телохранитель",
                        Role = EnemyRole.Defender,
                        MaxHp = 90,
                        Agility = 5,
                        BaseDamage = 11,
                        Armor = 8,
                        CritChance = 5,
                        DodgeChance = 0,
                        CritMultiplier = 1.3,
                        MaxAP = 4,
                        CurrentAP = 4
                    };
                    bodyguard.Hp = bodyguard.MaxHp;
                    enemies.Add(bodyguard);
                }
            }
            else if (returnNodeId == "PostBattle_Scene4")
            {
                var boss = new BattleEnemy
                {
                    Id = "boss_gromgar", // P0-фикс (В1): стабильный Id вместо проверки по имени
                    Name = "Вождь Громгар",
                    Role = EnemyRole.Aggressor,
                    MaxHp = 250,
                    Agility = 12,
                    BaseDamage = 24,
                    Armor = 6,
                    CritChance = 20,
                    DodgeChance = 8,
                    CritMultiplier = 1.6,
                    MaxAP = 4,
                    CurrentAP = 4
                };
                boss.Hp = boss.MaxHp;
                boss.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S1_PowerStrike"));
                boss.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S2_Cleave"));
                enemies.Add(boss);
            }

            // ==========================================
            // === АКТ 2 БОИ ===
            // ==========================================
            else if (returnNodeId == "Act2_PostBattle_Gates")
            {
                // Бой у Черных Врат Цитадели
                var warden = new BattleEnemy
                {
                    Name = "Страж Черных Врат",
                    Role = EnemyRole.Defender,
                    MaxHp = 160,
                    Agility = 6,
                    BaseDamage = 18,
                    Armor = 8,
                    CritChance = 8,
                    DodgeChance = 0,
                    CritMultiplier = 1.4,
                    MaxAP = 4,
                    CurrentAP = 4
                };
                warden.Hp = warden.MaxHp;
                warden.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S1_PowerStrike"));
                enemies.Add(warden);

                var archer = new BattleEnemy
                {
                    Name = "Теневой Лучник",
                    Role = EnemyRole.Aggressor,
                    MaxHp = 75,
                    Agility = 12,
                    BaseDamage = 16,
                    Armor = 2,
                    CritChance = 20,
                    DodgeChance = 8,
                    CritMultiplier = 1.5,
                    MaxAP = 4,
                    CurrentAP = 4
                };
                archer.Hp = archer.MaxHp;
                enemies.Add(archer);
            }
            else if (returnNodeId == "Act2_Dungeons_PostBattle")
            {
                // Тюремные Казематы: Инквизитор + Гончая Скверны
                var inquisitor = new BattleEnemy
                {
                    Name = "Теневой Инквизитор",
                    Role = EnemyRole.Support,
                    MaxHp = 150,
                    MaxMp = 90,
                    Mp = 90,
                    Agility = 8,
                    BaseDamage = 12,
                    BaseMagicDamage = 22,
                    Armor = 4,
                    CritChance = 10,
                    DodgeChance = 5,
                    CritMultiplier = 1.5,
                    MaxAP = 4,
                    CurrentAP = 4
                };
                inquisitor.Hp = inquisitor.MaxHp;
                inquisitor.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "M1_Fireball"));
                inquisitor.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "V1_FirstAid"));
                enemies.Add(inquisitor);

                var hound = new BattleEnemy
                {
                    Name = "Гончая Скверны",
                    Role = EnemyRole.Aggressor,
                    MaxHp = 85,
                    Agility = 14,
                    BaseDamage = 17,
                    Armor = 2,
                    CritChance = 15,
                    DodgeChance = 12,
                    CritMultiplier = 1.6,
                    MaxAP = 4,
                    CurrentAP = 4
                };
                hound.Hp = hound.MaxHp;
                hound.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S1_PowerStrike"));
                enemies.Add(hound);
            }
            else if (returnNodeId == "Act2_PostBattle_Golem")
            {
                // Древняя Кузница: Взбесившийся Рунный Голем (Мини-босс)
                var golem = new BattleEnemy
                {
                    Name = "Древний Рунный Голем",
                    Role = EnemyRole.Defender,
                    MaxHp = 320,
                    Agility = 4,
                    BaseDamage = 28,
                    Armor = 14,
                    CritChance = 10,
                    DodgeChance = 0,
                    CritMultiplier = 1.5,
                    MaxAP = 4,
                    CurrentAP = 4
                };
                golem.Hp = golem.MaxHp;
                golem.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S1_PowerStrike"));
                golem.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S2_Cleave"));
                enemies.Add(golem);
            }
            else if (returnNodeId == "Act2_Ending_Decisions")
            {
                // Финальный Босс Акта 2: Теневой Архимаг Малкор (2 фазы)
                if (wave == 1)
                {
                    var malkor = new BattleEnemy
                    {
                        Name = "Архимаг Малкор",
                        Role = EnemyRole.Support,
                        MaxHp = 280,
                        MaxMp = 160,
                        Mp = 160,
                        Agility = 10,
                        BaseDamage = 14,
                        BaseMagicDamage = 30,
                        Armor = 6,
                        CritChance = 15,
                        DodgeChance = 8,
                        CritMultiplier = 1.6,
                        MaxAP = 4,
                        CurrentAP = 4
                    };
                    malkor.Hp = malkor.MaxHp;
                    malkor.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "M1_Fireball"));
                    malkor.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "V1_FirstAid"));
                    enemies.Add(malkor);

                    var adept1 = new BattleEnemy
                    {
                        Name = "Теневой Адепт",
                        Role = EnemyRole.Aggressor,
                        MaxHp = 80,
                        Agility = 9,
                        BaseDamage = 14,
                        Armor = 3,
                        CritChance = 10,
                        DodgeChance = 5,
                        CritMultiplier = 1.4,
                        MaxAP = 4,
                        CurrentAP = 4
                    };
                    adept1.Hp = adept1.MaxHp;
                    enemies.Add(adept1);

                    var adept2 = new BattleEnemy
                    {
                        Name = "Культист Скверны",
                        Role = EnemyRole.Support,
                        MaxHp = 70,
                        MaxMp = 50,
                        Mp = 50,
                        Agility = 7,
                        BaseDamage = 8,
                        BaseMagicDamage = 16,
                        Armor = 2,
                        CritChance = 5,
                        DodgeChance = 5,
                        CritMultiplier = 1.3,
                        MaxAP = 4,
                        CurrentAP = 4
                    };
                    adept2.Hp = adept2.MaxHp;
                    adept2.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "M1_Fireball"));
                    enemies.Add(adept2);
                }
                else // Волна 2: Воплощение Теневой Скверны (Малкор в истинной форме)
                {
                    var malkorPhase2 = new BattleEnemy
                    {
                        Name = "Малкор: Воплощение Тьмы",
                        Role = EnemyRole.Aggressor,
                        MaxHp = 360,
                        MaxMp = 200,
                        Mp = 200,
                        Agility = 12,
                        BaseDamage = 26,
                        BaseMagicDamage = 38,
                        Armor = 8,
                        CritChance = 25,
                        DodgeChance = 10,
                        CritMultiplier = 1.75,
                        MaxAP = 4,
                        CurrentAP = 4
                    };
                    malkorPhase2.Hp = malkorPhase2.MaxHp;
                    malkorPhase2.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "M1_Fireball"));
                    malkorPhase2.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S2_Cleave"));
                    malkorPhase2.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "V1_FirstAid"));
                    enemies.Add(malkorPhase2);
                }
            }
            else
            {
                enemies = GenerateArcadeWave(1, 1, 1);
            }

            return enemies;
        }
    
        public static List<BattleEnemy> GenerateEliteWave(int actNumber, int playerLevel)
        {
            var enemies = new List<BattleEnemy>();
            int roll = rand.Next(100);

            if (actNumber == 1)
            {
                if (roll < 50)
                {
                    var berserkLord = new BattleEnemy
                    {
                        Name = "Воевода Железного Клыка",
                        Role = EnemyRole.Aggressor,
                        MaxHp = 160 + (playerLevel * 15),
                        Agility = 10 + playerLevel,
                        BaseDamage = 20 + (playerLevel * 3),
                        Armor = 4,
                        CritChance = 20,
                        DodgeChance = 8,
                        CritMultiplier = 1.6,
                        MaxAP = 4,
                        CurrentAP = 4
                    };
                    berserkLord.Hp = berserkLord.MaxHp;
                    berserkLord.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S2_Cleave"));
                    berserkLord.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S1_PowerStrike"));
                    enemies.Add(berserkLord);

                    var minion = new BattleEnemy
                    {
                        Name = "Орочий Телохранитель",
                        Role = EnemyRole.Defender,
                        MaxHp = 70 + (playerLevel * 8),
                        Agility = 6,
                        BaseDamage = 12,
                        Armor = 5,
                        CritChance = 5,
                        DodgeChance = 5,
                        MaxAP = 4,
                        CurrentAP = 4
                    };
                    minion.Hp = minion.MaxHp;
                    enemies.Add(minion);
                }
                else
                {
                    var troll = new BattleEnemy
                    {
                        Name = "Пещерный Тролль-Крушитель",
                        Role = EnemyRole.Aggressor,
                        MaxHp = 220 + (playerLevel * 20),
                        Agility = 5,
                        BaseDamage = 26 + (playerLevel * 3),
                        Armor = 7,
                        CritChance = 10,
                        DodgeChance = 0,
                        CritMultiplier = 1.7,
                        MaxAP = 4,
                        CurrentAP = 4
                    };
                    troll.Hp = troll.MaxHp;
                    troll.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S2_Cleave"));
                    enemies.Add(troll);
                }
            }
            else // Акт 2 и выше
            {
                if (roll < 50)
                {
                    var inquisitor = new BattleEnemy
                    {
                        Name = "Теневой Инквизитор Цитадели",
                        Role = EnemyRole.Tactician,
                        MaxHp = 240 + (playerLevel * 18),
                        MaxMp = 100,
                        Mp = 100,
                        Agility = 12 + playerLevel,
                        BaseDamage = 24 + (playerLevel * 2),
                        BaseMagicDamage = 26 + (playerLevel * 3),
                        Armor = 5,
                        CritChance = 25,
                        DodgeChance = 12,
                        CritMultiplier = 1.7,
                        MaxAP = 4,
                        CurrentAP = 4
                    };
                    inquisitor.Hp = inquisitor.MaxHp;
                    inquisitor.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "M1_Fireball"));
                    inquisitor.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S1_PowerStrike"));
                    inquisitor.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "V1_FirstAid"));
                    enemies.Add(inquisitor);
                }
                else
                {
                    var golem = new BattleEnemy
                    {
                        Name = "Древний Рунный Голем",
                        Role = EnemyRole.Defender,
                        MaxHp = 300 + (playerLevel * 22),
                        Agility = 6,
                        BaseDamage = 30 + (playerLevel * 3),
                        Armor = 10,
                        CritChance = 10,
                        DodgeChance = 0,
                        CritMultiplier = 1.6,
                        MaxAP = 4,
                        CurrentAP = 4
                    };
                    golem.Hp = golem.MaxHp;
                    golem.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S2_Cleave"));
                    enemies.Add(golem);
                }
            }

            return enemies;
        }

        public static List<BattleEnemy> GenerateBossWave(int actNumber, int playerLevel)
        {
            var enemies = new List<BattleEnemy>();

            if (actNumber == 1)
            {
                var gromgar = new BattleEnemy
                {
                    Id = "boss_gromgar", // P0-фикс (В1): стабильный Id — GromgarBossRef находится и в аркадном боссе
                    Name = "Верховный Вождь Громгар",
                    Role = EnemyRole.Aggressor,
                    MaxHp = 280 + (playerLevel * 15),
                    Agility = 12,
                    BaseDamage = 26 + (playerLevel * 2),
                    Armor = 6,
                    CritChance = 25,
                    DodgeChance = 10,
                    CritMultiplier = 1.75,
                    MaxAP = 4,
                    CurrentAP = 4
                };
                gromgar.Hp = gromgar.MaxHp;
                gromgar.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S2_Cleave"));
                gromgar.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S1_PowerStrike"));
                gromgar.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "V1_FirstAid"));
                enemies.Add(gromgar);

                var add1 = new BattleEnemy
                {
                    Name = "Орочий Ветеран-Гвардеец",
                    Role = EnemyRole.Defender,
                    MaxHp = 85,
                    Agility = 8,
                    BaseDamage = 14,
                    Armor = 4,
                    CritChance = 10,
                    DodgeChance = 5,
                    MaxAP = 4,
                    CurrentAP = 4
                };
                add1.Hp = add1.MaxHp;
                enemies.Add(add1);
            }
            else // Акт 2: Малкор
            {
                var malkor = new BattleEnemy
                {
                    Name = "Архимаг Малкор: Теневой Владыка",
                    Role = EnemyRole.Aggressor,
                    MaxHp = 380 + (playerLevel * 20),
                    MaxMp = 200,
                    Mp = 200,
                    Agility = 14,
                    BaseDamage = 26,
                    BaseMagicDamage = 36 + (playerLevel * 2),
                    Armor = 7,
                    CritChance = 25,
                    DodgeChance = 10,
                    CritMultiplier = 1.8,
                    MaxAP = 4,
                    CurrentAP = 4
                };
                malkor.Hp = malkor.MaxHp;
                malkor.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "M1_Fireball"));
                malkor.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "S2_Cleave"));
                malkor.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "V1_FirstAid"));
                enemies.Add(malkor);

                var adept = new BattleEnemy
                {
                    Name = "Теневой Адепт Культа",
                    Role = EnemyRole.Support,
                    MaxHp = 90,
                    MaxMp = 50,
                    Mp = 50,
                    Agility = 9,
                    BaseDamage = 10,
                    BaseMagicDamage = 18,
                    Armor = 3,
                    CritChance = 10,
                    DodgeChance = 5,
                    MaxAP = 4,
                    CurrentAP = 4
                };
                adept.Hp = adept.MaxHp;
                adept.AvailableSkills.Add(SkillsDatabase.AllSkills.Find(s => s.Id == "M1_Fireball"));
                enemies.Add(adept);
            }

            return enemies;
        }

    }
}
