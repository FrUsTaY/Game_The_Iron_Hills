using System;
using System.Collections.Generic;
using System.Linq;
using EpicBattle.Models.RPG;

namespace EpicBattle.Models.Combat
{
    public class BattleEngine
    {
        private static readonly Random rand = new Random();

        public BattlePlayer Player { get; private set; }
        public List<BattleEnemy> Enemies { get; private set; }
        public List<CombatEntity> TurnQueue { get; private set; }

        public int CurrentWave { get; set; } = 1;
        public int TotalWaves { get; set; } = 1;
        public string BattleModifier { get; set; } = string.Empty;

        // Модификаторы боя и фазы боссов
        public bool IsGromgarEncounter { get; set; } = false;
        public bool GromgarEnraged { get; set; } = false;
        public int GromgarRageThreshold { get; set; } = 30; // % HP
        // P0-фикс (В1): единый источник порога Ярости — Intent больше не дублирует фазу
        public BattleEnemy GromgarBossRef { get; private set; }
        public double BloodlustMultiplier { get; set; } = 1.0;
        public bool GromgarChaosAttack { get; set; } = false;
        public int PlayerDamageBuffTurns { get; set; } = 0;

        // P0-фикс (В4): активные кулдауны навыков игрока (Id навыка -> оставшиеся ходы)
        private readonly Dictionary<string, int> _playerSkillCooldowns = new Dictionary<string, int>();

        // P0-фикс (В4): доступ к кулдаунам игрока из UI
        public bool TryGetPlayerCooldown(string skillId, out int remaining)
        {
            remaining = 0;
            return _playerSkillCooldowns.TryGetValue(skillId, out remaining);
        }

        public Action<string> OnLogEvent { get; set; } = delegate { };
        public Action OnQueueUpdated { get; set; } = delegate { };
        public Action OnWaveCompleted { get; set; } = delegate { };

        public BattleEngine(BattlePlayer player, List<BattleEnemy> enemies, int currentWave = 1, int totalWaves = 1, string battleModifier = null)
        {
            Player = player;
            Enemies = enemies;
            CurrentWave = currentWave;
            TotalWaves = totalWaves;
            BattleModifier = battleModifier ?? string.Empty;
            TurnQueue = new List<CombatEntity>();

            InitializeBattleModifiers();
            BuildQueue();
            NextTurn();
        }

        private void InitializeBattleModifiers()
        {
            if (string.IsNullOrEmpty(BattleModifier)) return;

            // --- Акт 1 Модификаторы ---
            if (BattleModifier == "Surprise")
            {
                OnLogEvent("⚡ [Внезапный удар] Вы используете Руну Огня! Враги ошеломлены.");
                var first = Enemies.FirstOrDefault();
                if (first != null)
                {
                    first.ApplyEffect(new StatusEffect { Id = "Stun", Name = "Оглушение", Type = StatusEffectType.Debuff, Duration = 1, Value = 1, IconPath = "💫" });
                }
                foreach (var enemy in Enemies)
                {
                    enemy.ApplyEffect(new StatusEffect { Id = "Burn", Name = "Горение", Type = StatusEffectType.Debuff, Duration = 2, Value = 8, IconPath = "🔥" });
                }
            }
            else if (BattleModifier == "Pragmatic")
            {
                PlayerDamageBuffTurns = 3;
                OnLogEvent("🧠 [Прагматик] Вы знаете замысел врагов (+50% к урону на 3 хода)!");
            }
            else if (BattleModifier == "Aggressive")
            {
                foreach (var enemy in Enemies)
                {
                    enemy.BaseDamage = (int)(enemy.BaseDamage * 1.15);
                }
                OnLogEvent("⚔️ [Агрессия] Враги в бешенстве и наносят повышенный урон (+15%)!");
            }
            else if (BattleModifier == "Ambush")
            {
                OnLogEvent("🌲 [Засада на тракте] Враги застигнуты врасплох!");
                foreach (var enemy in Enemies)
                {
                    int ambushDmg = (int)(enemy.MaxHp * 0.25);
                    enemy.Hp -= ambushDmg;
                    enemy.ApplyEffect(new StatusEffect { Id = "Burn", Name = "Горение", Type = StatusEffectType.Debuff, Duration = 2, Value = 6, IconPath = "🔥" });
                }
            }
            else if (BattleModifier == "SkipTurn")
            {
                var first = Enemies.FirstOrDefault();
                if (first != null)
                {
                    first.ApplyEffect(new StatusEffect { Id = "Stun", Name = "Оглушение", Type = StatusEffectType.Debuff, Duration = 1, Value = 1, IconPath = "💫" });
                    OnLogEvent($"📜 Древние знания ошеломляют {first.Name} (пропуск хода)!");
                }
            }
            else if (BattleModifier == "Varg_Honorable")
            {
                foreach (var enemy in Enemies)
                {
                    enemy.BaseDamage = (int)(enemy.BaseDamage * 1.10);
                }
                OnLogEvent("⚔️ Честный поединок! Враги воодушевлены (+10% к урону).");
            }
            else if (BattleModifier.StartsWith("Gromgar"))
            {
                IsGromgarEncounter = true;
                // P0-фикс (В1): фиксируем ссылку на босса — порог Ярости проверяется только в ProcessTurnStart
                GromgarBossRef = Enemies.FirstOrDefault(e => e.Name.Contains("Громгар"));
                if (BattleModifier == "Gromgar_Honor")
                {
                    GromgarRageThreshold = 40;
                    var gromgar = Enemies.FirstOrDefault(e => e.Name.Contains("Громгар"));
                    if (gromgar != null) gromgar.CritChance = 5;
                    OnLogEvent("🛡️ Слова о чести заставляют Громгара сомневаться (крит снижен, порог Ярости 40%).");
                }
                else if (BattleModifier == "Gromgar_Lore")
                {
                    var gromgar = Enemies.FirstOrDefault(e => e.Name.Contains("Громгар"));
                    if (gromgar != null)
                    {
                        int dmg = (int)(gromgar.MaxHp * 0.15);
                        gromgar.Hp -= dmg;
                        OnLogEvent($"🔥 Алтарь взрывается! Громгар получает {dmg} урона на старте.");
                    }
                    GromgarChaosAttack = true;
                }
                else if (BattleModifier == "Gromgar_Ruthless")
                {
                    BloodlustMultiplier = 1.25;
                    OnLogEvent("🩸 ЖАЖДА КРОВИ! Вы и Громгар наносите на 25% больше урона, но защита снижена.");
                }
            }

            // --- Акт 2 Модификаторы ---
            else if (BattleModifier == "Act2_Gates_Assault")
            {
                OnLogEvent("⚔️ [Ветеран] Вы разрушаете баррикаду! Теневой стрелок получает 35 урона и оглушен!");
                if (Enemies.Count > 1)
                {
                    Enemies[1].Hp -= 35;
                    Enemies[1].ApplyEffect(new StatusEffect { Id = "Stun", Name = "Оглушение", Type = StatusEffectType.Debuff, Duration = 1, Value = 1, IconPath = "💫" });
                }
            }
            else if (BattleModifier == "Act2_Gates_Magic")
            {
                Player.Mp = Math.Min(Player.MaxMp, Player.Mp + 35);
                var first = Enemies.FirstOrDefault();
                if (first != null)
                {
                    first.ApplyEffect(new StatusEffect { Id = "Stun", Name = "Оглушение", Type = StatusEffectType.Debuff, Duration = 1, Value = 1, IconPath = "💫" });
                }
                OnLogEvent("🔮 [Изгой-маг] Вы поглощаете теневой щит (+35 MP)! Страж Врат оглушен.");
            }
            else if (BattleModifier == "Act2_Gates_Stealth")
            {
                OnLogEvent("🗡️ [Наемник] Бесшумный удар из тени! Теневой стрелок мгновенно устранен!");
                if (Enemies.Count > 1)
                {
                    Enemies[1].Hp = 0;
                }
            }
            else if (BattleModifier == "Act2_Gates_Dispel")
            {
                OnLogEvent("📜 [Древний Фолиант] Защитный барьер Врат рассеян! Броня врагов сломана.");
                foreach (var enemy in Enemies)
                {
                    enemy.Armor = 0;
                    enemy.Hp -= 20;
                    enemy.ApplyEffect(new StatusEffect { Id = "Burn", Name = "Горение", Type = StatusEffectType.Debuff, Duration = 2, Value = 8, IconPath = "🔥" });
                }
            }
            else if (BattleModifier == "Act2_Dungeon_Assault")
            {
                var inquisitor = Enemies.FirstOrDefault();
                if (inquisitor != null)
                {
                    inquisitor.Hp -= 35;
                    inquisitor.ApplyEffect(new StatusEffect { Id = "Bleed", Name = "Кровотечение", Type = StatusEffectType.Debuff, Duration = 3, Value = 10, IconPath = "🩸" });
                }
                OnLogEvent("⚔️ [Штурм казематов] Инквизитор получает сокрушительный удар (-35 HP, кровотечение)!");
            }
            else if (BattleModifier == "Act2_Malkor_Weakened")
            {
                var malkor = Enemies.FirstOrDefault(e => e.Name.Contains("Малкор"));
                if (malkor != null)
                {
                    int dmg = (int)(malkor.MaxHp * 0.30);
                    malkor.Hp -= dmg;
                    malkor.Armor = Math.Max(0, malkor.Armor - 4);
                }
                OnLogEvent("🔮 [Теневой Кристалл Каэля] Кристалл взрывается, раскалывая магический барьер Малкора (-30% HP, броня снижена)!");
            }
            else if (BattleModifier == "Act2_Golem_Assault")
            {
                foreach (var enemy in Enemies)
                {
                    enemy.Hp -= 60;
                    enemy.ApplyEffect(new StatusEffect { Id = "Burn", Name = "Горение", Type = StatusEffectType.Debuff, Duration = 2, Value = 10, IconPath = "🔥" });
                }
                OnLogEvent("🗿 [Союзный Рунный Голем] Голем сносит миньонов Малкора мощным таранным ударом (-60 урона всем врагам)!");
            }
            else if (BattleModifier == "Act2_Purification")
            {
                PlayerDamageBuffTurns = 99;
                Player.ApplyEffect(new StatusEffect { Id = "Regen", Name = "Регенерация", Type = StatusEffectType.Buff, Duration = 5, Value = 15, IconPath = "✨" });
                OnLogEvent("🧪 [Эликсир Очищения] Священная аура наполняет вас (+50% к урону на весь бой, Регенерация +15 HP)!");
            }
            else if (BattleModifier == "Act2_Orc_Reinforce")
            {
                for (int i = 1; i < Enemies.Count; i++)
                {
                    Enemies[i].Hp -= 50;
                    Enemies[i].ApplyEffect(new StatusEffect { Id = "Stun", Name = "Оглушение", Type = StatusEffectType.Debuff, Duration = 1, Value = 1, IconPath = "💫" });
                }
                OnLogEvent("📯 [Рог Громгара] Воины северных племен атакуют с фланга (-50 урона миньонам, оглушение)!");
            }
        }

        public void SpawnNextWave(List<BattleEnemy> nextWaveEnemies, int waveNumber)
        {
            CurrentWave = waveNumber;
            Enemies.Clear();
            foreach (var e in nextWaveEnemies)
            {
                Enemies.Add(e);
                e.DetermineIntent(Enemies, Player);
            }

            // Небольшое восстановление игрока при переходе волны
            Player.CurrentAP = Player.MaxAP;
            Player.Mp = Math.Min(Player.MaxMp, Player.Mp + 15);
            Player.IsDefending = false;

            OnLogEvent($"⚔️ --- ВОЛНА {CurrentWave} ИЗ {TotalWaves} ---");
            OnLogEvent("Новые противники вступают в бой! +15 MP восстановлено.");

            // P0-фикс (В7): стартовые эффекты модификатора (станы, урон по врагам, баффы игрока)
            // применяются ОДИН раз при создании боя. Повторный вызов на волне 2+ кумулятивно
            // усиливал Aggressive (урон ×1.15^n), заново наносил Ambush/Golem_Assault и т.д.
            // При этом ссылки на новых врагов нужно обновить (в т.ч. GromgarBossRef для фазы Ярости).
            if (IsGromgarEncounter)
            {
                GromgarBossRef = Enemies.FirstOrDefault(e => e.Name.Contains("Громгар"));
            }

            BuildQueue();
            NextTurn();
        }

        private void BuildQueue()
        {
            TurnQueue.Clear();
            if (Player.Hp > 0)
                TurnQueue.Add(Player);

            foreach (var enemy in Enemies.Where(e => e.Hp > 0))
                TurnQueue.Add(enemy);

            TurnQueue = TurnQueue.OrderByDescending(e => e.Agility).ThenBy(e => e is BattlePlayer ? 0 : 1).ToList();
            OnQueueUpdated?.Invoke();
        }

        public void NextTurn()
        {
            if (IsBattleOver())
            {
                CheckBattleEnd();
                return;
            }

            if (TurnQueue.Count == 0)
            {
                BuildQueue();
                if (TurnQueue.Count == 0) return;
            }

            var currentEntity = TurnQueue.First();
            TurnQueue.RemoveAt(0);

            if (currentEntity.Hp <= 0)
            {
                NextTurn();
                return;
            }

            bool skipTurn = ProcessTurnStart(currentEntity);

            if (currentEntity.Hp <= 0)
            {
                if (currentEntity is BattleEnemy e)
                {
                    OnLogEvent($"💀 {e.Name} погибает от эффектов!");
                    CheckEnemyDeaths();
                    NextTurn();
                }
                else
                {
                    OnLogEvent("💀 Вы погибаете от эффектов!");
                    CheckBattleEnd();
                }
                return;
            }

            TurnQueue.Insert(0, currentEntity);
            OnQueueUpdated?.Invoke();

            if (skipTurn)
            {
                EndEntityTurn(currentEntity);
                return;
            }

            if (currentEntity is BattleEnemy enemy)
            {
                ExecuteEnemyTurn(enemy);
            }
            else if (currentEntity is BattlePlayer)
            {
                OnLogEvent("Ваш ход!");
            }
        }

        public void EndEntityTurn(CombatEntity entity)
        {
            if (TurnQueue.Count > 0 && TurnQueue[0] == entity)
            {
                TurnQueue.RemoveAt(0);
                TurnQueue.Add(entity);
            }
            entity.CurrentAP = 0;
            NextTurn();
        }

        private bool ProcessTurnStart(CombatEntity entity)
        {
            entity.CurrentAP = entity.MaxAP;
            entity.IsDefending = false;
            bool skipTurn = false;

            if (entity is BattlePlayer player)
            {
                // P0-фикс (В4): отсчёт перезарядок навыков игрока
                var readySkills = new List<string>();
                foreach (var kv in _playerSkillCooldowns)
                {
                    if (kv.Value > 1) _playerSkillCooldowns[kv.Key] = kv.Value - 1;
                    else readySkills.Add(kv.Key);
                }
                foreach (var id in readySkills) _playerSkillCooldowns.Remove(id);

                if (PlayerDamageBuffTurns > 0)
                {
                    PlayerDamageBuffTurns--;
                }

                if (player.PlayerClass == "Изгой-маг")
                {
                    int regenMp = 3;
                    player.Mp = Math.Min(player.MaxMp, player.Mp + regenMp);
                    OnLogEvent($"✨ [Тайный источник] восстанавливает {regenMp} MP.");
                }

                if (player.UnlockedSkills.Any(s => s.Id == "M3_ArcaneFocus"))
                {
                    int focusMp = 5;
                    player.Mp = Math.Min(player.MaxMp, player.Mp + focusMp);
                    OnLogEvent($"🔮 [Концентрация] восстанавливает {focusMp} MP.");
                }
            }
            else if (entity is BattleEnemy enemy)
            {
                // P0-фикс (В1): единая точка включения Ярости Громгара — порог из GromgarRageThreshold,
                // без дублирующей проверки 35% в DetermineIntent
                if (IsGromgarEncounter && ReferenceEquals(enemy, GromgarBossRef) && !GromgarEnraged)
                {
                    double hpPct = (double)enemy.Hp / enemy.MaxHp * 100.0;
                    if (hpPct <= GromgarRageThreshold)
                    {
                        GromgarEnraged = true;
                        enemy.ApplyEffect(new StatusEffect { Id = "Rage", Name = "Ярость Вождя", Type = StatusEffectType.Buff, Duration = 99, Value = 50, IconPath = "😡" });
                        OnLogEvent("🔥 Вождь Громгар впадает в безумную ЯРОСТЬ! Его урон увеличен на 50%!");
                    }
                }
            }

            var effectsToRemove = new List<StatusEffect>();
            foreach (var effect in entity.ActiveEffects)
            {
                if (effect.Id == "Bleed")
                {
                    int damage = (int)effect.Value;
                    entity.Hp -= damage;
                    OnLogEvent($"🩸 {entity.Name} получает {damage} урона от кровотечения.");
                }
                else if (effect.Id == "Burn")
                {
                    int damage = (int)effect.Value;
                    entity.Hp -= damage;
                    OnLogEvent($"🔥 {entity.Name} получает {damage} магического урона от горения.");
                }
                else if (effect.Id == "Poison")
                {
                    int turnsActive = (effect.InitialDuration - effect.Duration) + 1;
                    int damage = (int)(effect.Value * turnsActive);
                    entity.Hp -= damage;
                    OnLogEvent($"🤢 {entity.Name} получает {damage} урона от яда.");
                }
                else if (effect.Id == "Regen")
                {
                    bool hasBurn = entity.ActiveEffects.Any(e => e.Id == "Burn");
                    int heal = (int)effect.Value;
                    if (hasBurn) heal = (int)(heal * 0.7f);

                    entity.Hp = Math.Min(entity.MaxHp, entity.Hp + heal);
                    OnLogEvent($"✨ {entity.Name} восстанавливает {heal} HP от регенерации.");
                }
                else if (effect.Id == "Stun")
                {
                    skipTurn = true;
                    OnLogEvent($"💫 {entity.Name} оглушен и пропускает ход!");
                    effect.Duration = 0;
                }

                effect.Duration--;
                if (effect.Duration <= 0)
                    effectsToRemove.Add(effect);
            }

            foreach (var ef in effectsToRemove)
            {
                entity.ActiveEffects.Remove(ef);
            }

            return skipTurn;
        }

        private void CheckBattleEnd()
        {
            if (IsBattleOver())
            {
                OnQueueUpdated?.Invoke();
            }
        }

        private void ExecuteEnemyTurn(BattleEnemy enemy)
        {
            OnLogEvent($"Ходит {enemy.Name}.");

            if (enemy.CurrentIntent.Type != IntentType.Defend)
            {
                enemy.IsDefending = false;
            }

            if (enemy.CurrentIntent.Type == IntentType.Attack)
            {
                int damage = CalculateDamage(enemy, Player, enemy.CurrentIntent.EstimatedValue);
                if (damage > 0)
                {
                    // Хаотичный взрыв алтаря при модификаторе Gromgar_Lore
                    if (IsGromgarEncounter && GromgarChaosAttack && rand.NextDouble() > 0.6)
                    {
                        int chaosDmg = (int)(damage * 0.3);
                        damage += chaosDmg;
                        OnLogEvent($"🔥 Хаотичный взрыв алтаря усиливает удар на +{chaosDmg}!");
                    }

                    Player.Hp -= damage;
                    OnLogEvent($"⚔️ {enemy.Name} атакует и наносит {damage} урона.");
                }
            }
            else if (enemy.CurrentIntent.Type == IntentType.Defend)
            {
                enemy.IsDefending = true;
                OnLogEvent($"🛡️ {enemy.Name} встает в глухую защиту.");
            }
            else if (enemy.CurrentIntent.Type == IntentType.Cast)
            {
                if (enemy.CurrentIntent.Description.Contains("ярость"))
                {
                    enemy.ApplyEffect(new StatusEffect { Id = "Rage", Name = "Ярость", Type = StatusEffectType.Buff, Duration = 3, Value = 50, IconPath = "😡" });
                    OnLogEvent($"😡 {enemy.Name} впадает в ярость (+50% к урону)!");
                }
                else if (enemy.CurrentIntent.Description.Contains("Ослепление"))
                {
                    Player.ApplyEffect(new StatusEffect { Id = "Blind", Name = "Слепота", Type = StatusEffectType.Debuff, Duration = 2, Value = 45, IconPath = "👁️" });
                    OnLogEvent($"👁️ {enemy.Name} ослепляет вас на 2 хода!");
                }
                else if (enemy.CurrentIntent.Description.Contains("Рассекающий"))
                {
                    var skill = enemy.AvailableSkills.FirstOrDefault(s => s.Id == "S2_Cleave");
                    if (skill != null) ExecuteSkill(enemy, skill, target: null);
                }
                else if (enemy.CurrentIntent.Description.Contains("Огненный шар"))
                {
                    var skill = enemy.AvailableSkills.FirstOrDefault(s => s.Id == "M1_Fireball");
                    if (skill != null) ExecuteSkill(enemy, skill, target: null);
                }
            }
            else if (enemy.CurrentIntent.Type == IntentType.Heal)
            {
                var injuredAlly = Enemies
                    .Where(e => e.Hp > 0 && e.Hp < e.MaxHp)
                    .OrderBy(e => (float)e.Hp / e.MaxHp)
                    .FirstOrDefault();

                if (injuredAlly != null)
                {
                    int heal = enemy.CurrentIntent.EstimatedValue;
                    if (injuredAlly.ActiveEffects.Any(e => e.Id == "Burn")) heal = (int)(heal * 0.7);
                    injuredAlly.Hp = Math.Min(injuredAlly.MaxHp, injuredAlly.Hp + heal);
                    OnLogEvent($"🌿 {enemy.Name} исцеляет {injuredAlly.Name} на {heal} HP.");
                }
            }

            enemy.DetermineIntent(Enemies, Player);
            EndEntityTurn(enemy);
        }

        public int CalculateDamage(CombatEntity source, CombatEntity target, int baseDamage, bool isMagic = false)
        {
            if (!isMagic && source.ActiveEffects.Any(e => e.Id == "Blind"))
            {
                var blind = source.ActiveEffects.First(e => e.Id == "Blind");
                if (rand.Next(100) < blind.Value)
                {
                    OnLogEvent($"👁️ {source.Name} промахивается из-за слепоты!");
                    return 0;
                }
            }

            if (!isMagic && target.DodgeChance > 0)
            {
                bool isStunned = target.ActiveEffects.Any(e => e.Id == "Stun");
                if (!isStunned && rand.Next(100) < target.DodgeChance)
                {
                    OnLogEvent($"💨 {target.Name} ловко уклоняется от удара!");
                    return 0;
                }
            }

            bool isCrit = false;
            int rawDamage = baseDamage;
            if (source.CritChance > 0 && rand.Next(100) < source.CritChance)
            {
                isCrit = true;
                rawDamage = (int)(rawDamage * source.CritMultiplier);
            }

            if (source is BattlePlayer && PlayerDamageBuffTurns > 0)
            {
                rawDamage = (int)(rawDamage * 1.5);
            }

            if (BloodlustMultiplier > 1.0)
            {
                rawDamage = (int)(rawDamage * BloodlustMultiplier);
            }

            if (source.ActiveEffects.Any(e => e.Id == "Rage"))
            {
                var rage = source.ActiveEffects.First(e => e.Id == "Rage");
                rawDamage = (int)(rawDamage * (1 + rage.Value / 100f));
            }

            int damage = rawDamage;
            if (target.IsDefending)
            {
                if (target is BattlePlayer bp && bp.PlayerClass == "Ветеран")
                    damage = (int)(damage * 0.4);
                else
                    damage /= 2;

                if (BloodlustMultiplier > 1.0)
                {
                    damage = (int)(damage * 1.15);
                }
            }

            damage -= target.Armor;

            if (target.ActiveEffects.Any(e => e.Id == "Rage"))
            {
                damage += (int)(target.Armor * 0.15f);
            }

            if (damage < 1) damage = 1;

            if (target.ActiveEffects.Any(e => e.Id == "ManaShield"))
            {
                var shield = target.ActiveEffects.First(e => e.Id == "ManaShield");
                int manaDamage = (int)(damage * (shield.Value / 100f));
                if (target.Mp >= manaDamage)
                {
                    target.Mp -= manaDamage;
                    damage -= manaDamage;
                    OnLogEvent($"🛡️ Щит маны поглощает {manaDamage} урона (осталось {target.Mp} MP).");
                }
                else if (target.Mp > 0)
                {
                    damage -= target.Mp;
                    target.Mp = 0;
                    OnLogEvent("🛡️ Щит маны разбит!");
                }
            }

            if (isCrit)
            {
                OnLogEvent($"🎯 КРИТИЧЕСКИЙ УДАР ({source.Name})!");
            }

            return damage;
        }

        public void ExecuteSkill(CombatEntity caster, Skill skill, BattleEnemy target = null)
        {
            if (caster.CurrentAP < skill.APCost)
            {
                OnLogEvent("Недостаточно AP!");
                return;
            }
            if (caster.Mp < skill.ManaCost)
            {
                OnLogEvent("Недостаточно маны!");
                return;
            }

            // P0-фикс (В4): реальная проверка кулдауна навыка
            if (caster is BattlePlayer && _playerSkillCooldowns.TryGetValue(skill.Id, out int remainingCd) && remainingCd > 0)
            {
                OnLogEvent($"⏳ '{skill.Name}' на перезарядке: ещё {remainingCd} ход(ов).");
                return;
            }

            caster.CurrentAP -= skill.APCost;
            caster.Mp -= skill.ManaCost;

            // P0-фикс (В4): запуск перезарядки после использования
            if (caster is BattlePlayer && skill.Cooldown > 0)
            {
                _playerSkillCooldowns[skill.Id] = skill.Cooldown;
            }

            if (skill.Id == "S1_PowerStrike")
            {
                var actualTarget = target ?? (caster is BattlePlayer ? Enemies.FirstOrDefault(e => e.Hp > 0) : (CombatEntity)Player);
                if (actualTarget != null)
                {
                    int dmg = CalculateDamage(caster, actualTarget, (int)(caster.BaseDamage * 1.5));
                    if (dmg > 0)
                    {
                        actualTarget.Hp -= dmg;
                        OnLogEvent($"💥 {caster.Name} использует '{skill.Name}' и наносит {dmg} урона {actualTarget.Name}.");
                        ApplyVampirismIfPlayer(caster, dmg);
                    }
                }
            }
            else if (skill.Id == "S2_Cleave")
            {
                var mainTarget = target ?? (caster is BattlePlayer ? Enemies.FirstOrDefault(e => e.Hp > 0) : (CombatEntity)Player);
                if (mainTarget != null)
                {
                    int mainDmg = CalculateDamage(caster, mainTarget, caster.BaseDamage);
                    if (mainDmg > 0)
                    {
                        mainTarget.Hp -= mainDmg;
                        mainTarget.ApplyEffect(new StatusEffect { Id = "Bleed", Name = "Кровотечение", Type = StatusEffectType.Debuff, Duration = 3, Value = 5, IconPath = "🩸" });
                        OnLogEvent($"⚔️ {caster.Name} использует '{skill.Name}'! {mainTarget.Name} получает {mainDmg} урона и кровотечение.");
                        ApplyVampirismIfPlayer(caster, mainDmg);
                    }

                    if (caster is BattlePlayer)
                    {
                        var adjacentEnemies = Enemies.Where(e => e != mainTarget && e.Hp > 0).ToList();
                        foreach (var adj in adjacentEnemies)
                        {
                            int adjDmg = CalculateDamage(caster, adj, (int)(caster.BaseDamage * 0.5));
                            if (adjDmg > 0)
                            {
                                adj.Hp -= adjDmg;
                                OnLogEvent($"⚔️ {adj.Name} получает {adjDmg} урона от рассечения.");
                                ApplyVampirismIfPlayer(caster, adjDmg);
                            }
                        }
                    }
                }
            }
            else if (skill.Id == "M1_Fireball")
            {
                OnLogEvent($"🔥 {caster.Name} кастует '{skill.Name}'!");
                var targets = caster is BattlePlayer ? Enemies.Cast<CombatEntity>().Where(e => e.Hp > 0).ToList() : new List<CombatEntity> { Player };

                foreach (var t in targets)
                {
                    int dmg = CalculateDamage(caster, t, caster.BaseMagicDamage, isMagic: true);
                    if (dmg > 0)
                    {
                        t.Hp -= dmg;
                        OnLogEvent($"💥 {t.Name} получает {dmg} маг. урона.");
                        if (rand.Next(100) < 30)
                        {
                            t.ApplyEffect(new StatusEffect { Id = "Burn", Name = "Горение", Type = StatusEffectType.Debuff, Duration = 2, Value = 8, IconPath = "🔥" });
                            OnLogEvent($"🔥 {t.Name} подожжен!");
                        }
                    }
                }
            }
            else if (skill.Id == "M2_ManaShield")
            {
                caster.ApplyEffect(new StatusEffect { Id = "ManaShield", Name = "Щит Маны", Type = StatusEffectType.Buff, Duration = 3, Value = 50, IconPath = "🛡️" });
                OnLogEvent($"✨ {caster.Name} накладывает на себя Щит Маны.");
            }
            else if (skill.Id == "V1_FirstAid")
            {
                int heal = 30;
                if (caster.ActiveEffects.Any(e => e.Id == "Burn")) heal = (int)(heal * 0.7);
                caster.Hp = Math.Min(caster.MaxHp, caster.Hp + heal);
                OnLogEvent($"🌿 {caster.Name} применяет Первую помощь и восстанавливает {heal} HP.");
            }

            CheckEnemyDeaths();
        }

        private void ApplyVampirismIfPlayer(CombatEntity caster, int damageDealt)
        {
            if (caster is BattlePlayer bp && bp.UnlockedSkills.Any(s => s.Id == "V3_Vampirism") && damageDealt > 0)
            {
                int heal = Math.Max(1, (int)(damageDealt * 0.20));
                bp.Hp = Math.Min(bp.MaxHp, bp.Hp + heal);
                OnLogEvent($"🩸 [Вампиризм] восстанавливает {heal} HP.");
            }
        }

        public void PlayerAttack(BattleEnemy target)
        {
            if (Player.CurrentAP < 2)
            {
                OnLogEvent("Недостаточно AP!");
                return;
            }

            Player.CurrentAP -= 2;
            int damage = CalculateDamage(Player, target, Player.BaseDamage);

            if (damage > 0)
            {
                target.Hp -= damage;
                OnLogEvent($"⚔️ Вы атакуете {target.Name} и наносите {damage} урона.");
                ApplyVampirismIfPlayer(Player, damage);
            }

            CheckEnemyDeaths();
        }

        public void PlayerDefend()
        {
            if (Player.CurrentAP < 2)
            {
                OnLogEvent("Недостаточно AP!");
                return;
            }

            Player.CurrentAP -= 2;
            Player.IsDefending = true;
            OnLogEvent("🛡️ Вы приготовились к защите.");
        }

        public void EndPlayerTurn()
        {
            EndEntityTurn(Player);
        }

        public void CheckEnemyDeaths()
        {
            bool died = false;
            foreach (var enemy in Enemies.ToList())
            {
                if (enemy.Hp <= 0)
                {
                    if (TurnQueue.Contains(enemy))
                    {
                        OnLogEvent($"💀 {enemy.Name} повержен!");
                        TurnQueue.Remove(enemy);
                        died = true;
                    }
                    else if (!died)
                    {
                        TurnQueue.Remove(enemy);
                        died = true;
                    }
                }
            }

            if (died)
            {
                OnQueueUpdated?.Invoke();
                if (Enemies.All(e => e.Hp <= 0))
                {
                    if (CurrentWave < TotalWaves)
                    {
                        OnWaveCompleted?.Invoke();
                    }
                    else
                    {
                        CheckBattleEnd();
                    }
                }
            }
        }

        public bool IsBattleOver()
        {
            return Player.Hp <= 0 || (Enemies.All(e => e.Hp <= 0) && CurrentWave >= TotalWaves);
        }
    }
}
