import re

with open('Views/BattleView.xaml.cs', 'r', encoding='utf-8') as f:
    text = f.read()

# The python scripts had issues with unescaped curly braces in f-strings like $"🔥 Алтарь взрывается! Громгар получает {dmg} урона"
# Let's cleanly replace things via straight string replacements.

text = text.replace(
"        private bool _enemiesBuffed = false;",
"""        private bool _enemiesBuffed = false;

        // Босс Громгар
        private bool _isGromgarEncounter = false;
        private int _gromgarRageThreshold = 30;
        private bool _gromgarEnraged = false;
        private double _bloodlustMultiplier = 1.0;
        private bool _gromgarChaosAttack = false;""")

text = text.replace(
"""                else if (_battleModifier == "Varg_Honorable")
                {
                    _enemiesBuffed = true;
                    enemyDamage = (int)(enemyDamage * 1.1);
                    EventLog.Text += "⚔️ Честный бой! Враги воодушевлены (+10% урон).\\n";
                }
            }
        }""",
"""                else if (_battleModifier == "Varg_Honorable")
                {
                    _enemiesBuffed = true;
                    enemyDamage = (int)(enemyDamage * 1.1);
                    EventLog.Text += "⚔️ Честный бой! Враги воодушевлены (+10% урон).\\n";
                }
            }
            else if (_battleModifier.StartsWith("Gromgar"))
            {
                _isGromgarEncounter = true;

                if (_battleModifier == "Gromgar_Honor")
                {
                    _gromgarRageThreshold = 40;
                    EventLog.Text += "🛡 Слова о чести заставляют Громгара сомневаться.\\n(Его шанс крита снижен, но Ярость наступит раньше).\\n";
                }
                else if (_battleModifier == "Gromgar_Lore")
                {
                    int dmg = (int)(enemyMaxHp * 0.15);
                    enemyHp -= dmg;
                    _gromgarChaosAttack = true;
                    EventLog.Text += $"🔥 Алтарь взрывается! Громгар получает {dmg} урона.\\n";
                }
                else if (_battleModifier == "Gromgar_Ruthless")
                {
                    _bloodlustMultiplier = 1.2;
                    EventLog.Text += "🩸 ЖАЖДА КРОВИ! Вы и Громгар наносите больше урона, но защита снижена.\\n";
                }
            }
        }""")

text = text.replace(
"""            else if (!_isArcade && _returnNodeId == "PostBattle_Scene3")
            {
                if (_currentWave < 3)""",
"""            else if (!_isArcade && _returnNodeId == "PostBattle_Scene4")
            {
                EnemyNameText.Text = "👑 Вождь Громгар";
                enemyMaxHp = 250;
                enemyDamage = 35;
                _isGromgarEncounter = true;
            }
            else if (!_isArcade && _returnNodeId == "PostBattle_Scene3")
            {
                if (_currentWave < 3)""")

text = text.replace(
"""            if (_playerDamageBuffTurns > 0)
            {
                _playerDamageBuffTurns--;
            }

            if (_enemiesBurn && _currentWave < 3)""",
"""            if (_playerDamageBuffTurns > 0)
            {
                _playerDamageBuffTurns--;
            }

            if (_isGromgarEncounter && !_gromgarEnraged)
            {
                double hpPercentage = (double)enemyHp / enemyMaxHp * 100.0;
                if (hpPercentage <= _gromgarRageThreshold)
                {
                    _gromgarEnraged = true;
                    enemyDamage = (int)(enemyDamage * 1.5);
                    Log("\\n🔥 Громгар впадает в ЯРОСТЬ! Его урон чудовищно увеличен!");
                }
            }

            if (_enemiesBurn && _currentWave < 3)""")

text = text.replace(
"""                else
                {
                    int damage = random.Next(enemyDamage - 5, enemyDamage + 5);

                    if (isDefending)
                    {
                        damage /= 2;
                        Log($"🛡 Вы защищаетесь! Урон снижен. Враг наносит {damage} урона.");
                        isDefending = false;
                    }
                    else
                    {
                        Log($"👹 Враг наносит вам {damage} урона.");
                        AnimateScreenShake();
                        AnimateColorFlash(new SolidColorBrush(Colors.DarkRed));
                        PlaySound("hit.wav");
                    }

                    SaveManager.CurrentState.PlayerHp -= damage;
                }""",
"""                else
                {
                    int damage = random.Next(enemyDamage - 5, enemyDamage + 5);

                    if (_isGromgarEncounter)
                    {
                        if (_gromgarChaosAttack && random.NextDouble() > 0.5)
                        {
                            damage = (int)(damage * 1.3);
                            Log("🔥 Хаотичный взрыв алтаря усиливает удар Громгара!");
                        }
                        if (_bloodlustMultiplier > 1.0)
                        {
                            damage = (int)(damage * _bloodlustMultiplier);
                        }
                    }

                    if (isDefending)
                    {
                        damage /= 2;
                        if (_bloodlustMultiplier > 1.0) damage = (int)(damage * 1.15);
                        Log($"🛡 Вы защищаетесь! Урон снижен. Враг наносит {damage} урона.");
                        isDefending = false;
                    }
                    else
                    {
                        Log($"👹 Враг наносит вам {damage} урона.");
                        AnimateScreenShake();
                        AnimateColorFlash(new SolidColorBrush(Colors.DarkRed));
                        PlaySound("hit.wav");
                    }

                    SaveManager.CurrentState.PlayerHp -= damage;
                }""")

text = text.replace(
"""        private async void AttackBtn_Click(object sender, RoutedEventArgs e)
        {
            var state = SaveManager.CurrentState;
            int damage = random.Next(state.PlayerBaseDamage, state.PlayerBaseDamage + 10);

            if (_playerDamageBuffTurns > 0)
            {
                damage = (int)(damage * 1.5); // +50% урон от Pragmatic
            }

            enemyHp -= damage;""",
"""        private async void AttackBtn_Click(object sender, RoutedEventArgs e)
        {
            var state = SaveManager.CurrentState;
            int damage = random.Next(state.PlayerBaseDamage, state.PlayerBaseDamage + 10);

            if (_playerDamageBuffTurns > 0)
            {
                damage = (int)(damage * 1.5);
            }

            if (_bloodlustMultiplier > 1.0)
            {
                damage = (int)(damage * _bloodlustMultiplier);
            }

            enemyHp -= damage;""")

text = text.replace(
"""        private async void MagicBtn_Click(object sender, RoutedEventArgs e)
        {
            var state = SaveManager.CurrentState;
            if (state.PlayerMp >= 10)
            {
                state.PlayerMp -= 10;
                int damage = random.Next(state.PlayerBaseMagicDamage, state.PlayerBaseMagicDamage + 15);
                enemyHp -= damage;""",
"""        private async void MagicBtn_Click(object sender, RoutedEventArgs e)
        {
            var state = SaveManager.CurrentState;
            if (state.PlayerMp >= 10)
            {
                state.PlayerMp -= 10;
                int damage = random.Next(state.PlayerBaseMagicDamage, state.PlayerBaseMagicDamage + 15);

                if (_bloodlustMultiplier > 1.0)
                {
                    damage = (int)(damage * _bloodlustMultiplier);
                }

                enemyHp -= damage;""")

with open('Views/BattleView.xaml.cs', 'w', encoding='utf-8') as f:
    f.write(text)
