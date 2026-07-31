using System;
using System.Collections.Generic;
using System.Linq;
using EpicBattle.Models.RPG;

namespace EpicBattle.Models.Combat
{
    public class BattleEngine
    {
        public BattlePlayer Player { get; private set; }
        public List<BattleEnemy> Enemies { get; private set; }
        public List<CombatEntity> TurnQueue { get; private set; }

        public Action<string> OnLogEvent { get; set; } = delegate { };
        public Action OnQueueUpdated { get; set; } = delegate { };

        public BattleEngine(BattlePlayer player, List<BattleEnemy> enemies)
        {
            Player = player;
            Enemies = enemies;
            TurnQueue = new List<CombatEntity>();
            BuildQueue();
        }

        private void BuildQueue()
        {
            TurnQueue.Clear();
            if (Player.Hp > 0)
                TurnQueue.Add(Player);

            foreach(var enemy in Enemies.Where(e => e.Hp > 0))
                TurnQueue.Add(enemy);

            // Сортировка по ловкости (по убыванию), при равенстве игрок первее
            TurnQueue = TurnQueue.OrderByDescending(e => e.Agility).ThenBy(e => e is BattlePlayer ? 0 : 1).ToList();
            OnQueueUpdated?.Invoke();
        }

        public void NextTurn()
        {
            if (TurnQueue.Count == 0) return;

            var currentEntity = TurnQueue.First();
            TurnQueue.RemoveAt(0);

            // Обработка начала хода (восстановление AP, статусы)
            bool skipTurn = ProcessTurnStart(currentEntity);

            // Если сущность умерла от статуса - пропускаем ход
            if (currentEntity.Hp <= 0)
            {
                if (currentEntity is BattleEnemy e)
                {
                    OnLogEvent($"💀 {e.Name} погибает от эффектов!");
                    CheckEnemyDeaths();
                    NextTurn(); // Передаем ход дальше, чтобы избежать зависания
                }
                else
                {
                    OnLogEvent($"💀 Вы погибаете от эффектов!");
                    CheckBattleEnd();
                }
                return;
            }

            // Важно: возвращаем сущность обратно в НАЧАЛО очереди для текущего хода,
            // чтобы ViewModel понимала, чей сейчас ход (особенно для игрока)
            TurnQueue.Insert(0, currentEntity);
            OnQueueUpdated?.Invoke();

            if (skipTurn)
            {
                EndEntityTurn(currentEntity);
                return;
            }

            if (currentEntity is BattleEnemy enemy)
            {
                // ИИ выбирает действие
                ExecuteEnemyTurn(enemy);
            }
            else if (currentEntity is BattlePlayer)
            {
                // Ждем действий игрока (в BattleViewModel)
                OnLogEvent("Ваш ход!");
            }
        }

        public void EndEntityTurn(CombatEntity entity)
        {
            if (TurnQueue.Count > 0 && TurnQueue[0] == entity)
            {
                // Убираем из начала и переносим в конец
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

            // Обработка эффектов
            var effectsToRemove = new List<StatusEffect>();
            foreach(var effect in entity.ActiveEffects)
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
                    // Урон увеличивается с каждым ходом
                    int turnsActive = (effect.InitialDuration - effect.Duration) + 1;
                    int damage = (int)(effect.Value * turnsActive);
                    entity.Hp -= damage;
                    OnLogEvent($"🤢 {entity.Name} получает {damage} урона от яда.");
                }
                else if (effect.Id == "Regen")
                {
                    // Горение снижает исцеление на 30%
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
                    effect.Duration = 0; // Снимается сразу
                }

                effect.Duration--;
                if (effect.Duration <= 0)
                    effectsToRemove.Add(effect);
            }

            foreach(var ef in effectsToRemove)
            {
                entity.ActiveEffects.Remove(ef);
            }

            return skipTurn;
        }

        private void CheckBattleEnd()
        {
            if (IsBattleOver())
            {
                // Завершение обрабатывается во View Model, мы просто сигнализируем
                OnQueueUpdated?.Invoke();
            }
        }

        private void ExecuteEnemyTurn(BattleEnemy enemy)
        {
            OnLogEvent($"Ходит {enemy.Name}.");

            // Сбрасываем защиту в начале хода врага, если он не планирует защищаться
            if (enemy.CurrentIntent.Type != IntentType.Defend)
            {
                 enemy.IsDefending = false;
            }

            // Применяем намерение
            if (enemy.CurrentIntent.Type == IntentType.Attack)
            {
                int damage = CalculateDamage(enemy, Player, enemy.CurrentIntent.EstimatedValue);
                if (damage > 0)
                {
                    Player.Hp -= damage;
                    OnLogEvent($"⚔️ {enemy.Name} атакует и наносит {damage} урона.");
                }
            }
            else if (enemy.CurrentIntent.Type == IntentType.Defend)
            {
                enemy.IsDefending = true;
                OnLogEvent($"🛡️ {enemy.Name} встает в защиту.");
            }
            else if (enemy.CurrentIntent.Type == IntentType.Cast)
            {
                if (enemy.CurrentIntent.Description == "Впасть в ярость")
                {
                    enemy.ApplyEffect(new StatusEffect { Id = "Rage", Name = "Ярость", Type = StatusEffectType.Buff, Duration = 3, Value = 50, IconPath = "😡" });
                    OnLogEvent($"😡 {enemy.Name} впадает в ярость!");
                }
                else if (enemy.CurrentIntent.Description == "Ослепление")
                {
                    Player.ApplyEffect(new StatusEffect { Id = "Blind", Name = "Слепота", Type = StatusEffectType.Debuff, Duration = 2, Value = 50, IconPath = "👁️" });
                    OnLogEvent($"👁️ {enemy.Name} ослепляет вас!");
                }
                else if (enemy.CurrentIntent.Description.Contains("Огненный шар"))
                {
                    var skill = enemy.AvailableSkills.FirstOrDefault(s => s.Id == "M1_Fireball");
                    if (skill != null) ExecuteSkill(enemy, skill);
                }
            }
            else if (enemy.CurrentIntent.Type == IntentType.Heal)
            {
                var injuredAlly = Enemies.FirstOrDefault(e => e.Hp > 0 && e.Hp < e.MaxHp);
                if (injuredAlly != null)
                {
                    int heal = enemy.CurrentIntent.EstimatedValue;
                    if (injuredAlly.ActiveEffects.Any(e => e.Id == "Burn")) heal = (int)(heal * 0.7);
                    injuredAlly.Hp = Math.Min(injuredAlly.MaxHp, injuredAlly.Hp + heal);
                    OnLogEvent($"🌿 {enemy.Name} лечит {injuredAlly.Name} на {heal} HP.");
                }
            }

            enemy.DetermineIntent(Enemies, Player); // Планирует на следующий ход
            EndEntityTurn(enemy); // Завершаем ход и передаем дальше
        }

        private int CalculateDamage(CombatEntity source, CombatEntity target, int baseDamage, bool isMagic = false)
        {
            int damage = baseDamage;

            // Слепота
            if (!isMagic && source.ActiveEffects.Any(e => e.Id == "Blind"))
            {
                var blind = source.ActiveEffects.First(e => e.Id == "Blind");
                if (new Random().Next(100) < blind.Value)
                {
                    OnLogEvent($"👁️ {source.Name} промахивается из-за слепоты!");
                    return 0;
                }
            }

            // Ярость
            if (source.ActiveEffects.Any(e => e.Id == "Rage"))
            {
                var rage = source.ActiveEffects.First(e => e.Id == "Rage");
                damage = (int)(damage * (1 + rage.Value / 100f));
            }

            // Защита и Броня
            if (target.IsDefending) damage /= 2;
            damage -= target.Armor;

            // Штраф к броне от Ярости (если цель в ярости)
            if (target.ActiveEffects.Any(e => e.Id == "Rage"))
            {
                damage += (int)(target.Armor * 0.15f); // Игнорируем часть брони
            }

            if (damage < 1) damage = 1; // Минимальный урон 1, если попали

            // Щит Маны
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
                    OnLogEvent($"🛡️ Щит маны разбит!");
                }
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

            caster.CurrentAP -= skill.APCost;
            caster.Mp -= skill.ManaCost;

            if (skill.Id == "S1_PowerStrike")
            {
                int dmg = CalculateDamage(caster, target, (int)(caster.BaseDamage * 1.5));
                if (dmg > 0)
                {
                    target.Hp -= dmg;
                    OnLogEvent($"💥 {caster.Name} использует '{skill.Name}' и наносит {dmg} урона {target.Name}.");
                }
            }
            else if (skill.Id == "S2_Cleave")
            {
                // Урон главной цели
                int mainDmg = CalculateDamage(caster, target, caster.BaseDamage);
                if (mainDmg > 0)
                {
                    target.Hp -= mainDmg;
                    target.ApplyEffect(new StatusEffect { Id = "Bleed", Name = "Кровотечение", Type = StatusEffectType.Debuff, Duration = 3, Value = 5, IconPath = "🩸" });
                    OnLogEvent($"⚔️ {caster.Name} использует '{skill.Name}'! {target.Name} получает {mainDmg} урона и кровотечение.");
                }

                // Урон соседним целям (допустим, всем остальным врагам)
                var adjacentEnemies = Enemies.Where(e => e != target && e.Hp > 0).ToList();
                foreach (var adj in adjacentEnemies)
                {
                    int adjDmg = CalculateDamage(caster, adj, (int)(caster.BaseDamage * 0.5));
                    if (adjDmg > 0)
                    {
                        adj.Hp -= adjDmg;
                        OnLogEvent($"⚔️ {adj.Name} получает {adjDmg} урона от рассечения.");
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
                    t.Hp -= dmg;
                    OnLogEvent($"💥 {t.Name} получает {dmg} урона.");
                    if (new Random().Next(100) < 30) // 30% шанс поджога
                    {
                        t.ApplyEffect(new StatusEffect { Id = "Burn", Name = "Горение", Type = StatusEffectType.Debuff, Duration = 2, Value = 8, IconPath = "🔥" });
                        OnLogEvent($"🔥 {t.Name} подожжен!");
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
            foreach(var enemy in Enemies.ToList())
            {
                if (enemy.Hp <= 0)
                {
                    // Проверяем, был ли он уже мертв (убран из очереди или нет)
                    // Но теперь мы удаляем его из Enemies полностью во ViewModel, так что здесь можно просто логировать
                    // Добавим свойство IsDead если нужно, но проще ориентироваться на Hp.
                    // Если он еще в очереди, значит умер только что
                    if (TurnQueue.Contains(enemy))
                    {
                        OnLogEvent($"💀 {enemy.Name} повержен!");
                        TurnQueue.Remove(enemy);
                        died = true;
                    }
                    else if (!died)
                    {
                        // Подстраховка: если он не в очереди, но HP = 0, возможно он умер от ДоТы до своего хода.
                        // Гарантируем, что он убран из очереди.
                        TurnQueue.Remove(enemy);
                        // Чтобы не спамить лог, мы полагаемся на то, что это обработано.
                        // Но если метод вызвали, значит кто-то мог умереть.
                        died = true;
                    }
                }
            }
            if (died)
            {
                // Уведомляем ViewModel для пересчета TargetedEnemy и обновления UI без полной перестройки очереди
                OnQueueUpdated?.Invoke();

                // Проверяем, остались ли враги, если нет - битва завершена
                if (Enemies.All(e => e.Hp <= 0))
                {
                    CheckBattleEnd();
                }
            }
        }

        public bool IsBattleOver()
        {
            return Player.Hp <= 0 || Enemies.All(e => e.Hp <= 0);
        }
    }
}
