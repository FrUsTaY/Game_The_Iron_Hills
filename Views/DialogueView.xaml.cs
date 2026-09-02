using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EpicBattle.Managers;
using EpicBattle.Models;
using EpicBattle.Models.Items;

namespace EpicBattle.Views
{
    public partial class DialogueView : UserControl
    {
        public DialogueView()
        {
            InitializeComponent();
            this.Loaded += (s, e) => this.Focus();
            LoadNode(SaveManager.CurrentState.CurrentNodeId);
        }

        private void LoadNode(string nodeId)
        {
            if (StoryDatabase.Nodes.TryGetValue(nodeId, out var node))
            {
                SaveManager.CurrentState.CurrentNodeId = nodeId;

                SpeakerText.Text = node.SpeakerName;
                DialogueText.Text = node.Text;

                var filteredChoices = new System.Collections.Generic.List<DialogueChoice>();
                foreach (var choice in node.Choices)
                {
                    if (string.IsNullOrEmpty(choice.RequiredFlag))
                    {
                        filteredChoices.Add(choice);
                    }
                    else if (choice.RequiredFlag == "HasTomeIntact" && SaveManager.CurrentState.HasTomeIntact)
                    {
                        filteredChoices.Add(choice);
                    }
                    else if (choice.RequiredFlag == "HonorGromgar" && SaveManager.CurrentState.HonorGromgar)
                    {
                        filteredChoices.Add(choice);
                    }
                    else if (choice.RequiredFlag == "HasTraitRuthless" && SaveManager.CurrentState.HasTraitRuthless)
                    {
                        filteredChoices.Add(choice);
                    }
                    else if (choice.RequiredFlag == "UnlockedAmbush" && SaveManager.CurrentState.UnlockedAmbush)
                    {
                        filteredChoices.Add(choice);
                    }
                    else if (choice.RequiredFlag == "HasVillageChestKey" && SaveManager.CurrentState.HasVillageChestKey)
                    {
                        filteredChoices.Add(choice);
                    }
                    else if (choice.RequiredFlag == "HasShadowGem" && SaveManager.CurrentState.HasShadowGem)
                    {
                        filteredChoices.Add(choice);
                    }
                    else if (choice.RequiredFlag == "GolemAllied" && SaveManager.CurrentState.GolemAllied)
                    {
                        filteredChoices.Add(choice);
                    }
                    else if (choice.RequiredFlag == "HasPurificationElixir" && SaveManager.CurrentState.HasPurificationElixir)
                    {
                        filteredChoices.Add(choice);
                    }
                    else if (choice.RequiredFlag.StartsWith("Class_"))
                    {
                        string requiredClass = choice.RequiredFlag.Substring(6);
                        if (SaveManager.CurrentState.PlayerClass == requiredClass)
                        {
                            filteredChoices.Add(choice);
                        }
                    }
                }

                ChoicesControl.ItemsSource = filteredChoices;
            }
        }

        private void ChoiceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is DialogueChoice choice)
            {
                var state = SaveManager.CurrentState;

                // ==========================================
                // === ДЕЙСТВИЯ АКТА 1 ===
                // ==========================================
                if (choice.Action == "StunAndLoot")
                {
                    state.Gold += 25;
                    MessageBox.Show("Получено 25 золота!", "Добыча", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Ruthless")
                {
                    state.HasTraitRuthless = true;
                    state.HasForestMap = true;
                    var map = ItemsDatabase.Clone("Q_ForestMap");
                    if (map != null && !state.Inventory.Exists(i => i.Id == map.Id)) state.Inventory.Add(map);
                    MessageBox.Show("Получена черта: Беспощадный.\nВ инвентарь добавлена: Карта засады в лесу.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "ExecuteOrc")
                {
                    state.HasForestMap = true;
                    state.HasVillageChestKey = true;
                    var map = ItemsDatabase.Clone("Q_ForestMap");
                    var key = ItemsDatabase.Clone("Q_VillageKey");
                    if (map != null && !state.Inventory.Exists(i => i.Id == map.Id)) state.Inventory.Add(map);
                    if (key != null && !state.Inventory.Exists(i => i.Id == key.Id)) state.Inventory.Add(key);
                    MessageBox.Show("Получены трофеи:\n- Ключ от сундука\n- Карта засады в лесу", "Добыча", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Brann_Noble")
                {
                    state.HasQuestAncestorsLegacy = true;
                    state.HpPotions += 1;
                    var ring = ItemsDatabase.Clone("R_RuneOfProtection");
                    if (ring != null && !state.Inventory.Exists(i => i.Id == ring.Id)) state.Inventory.Add(ring);
                    MessageBox.Show("Получено Зелье здоровья (+1).\nВ инвентарь добавлено: Кольцо Защиты Предков.\nНачат квест: «Наследие предков».", "Квест", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Brann_Pragmatic")
                {
                    state.HasQuestAncestorsLegacy = true;
                    state.KnowsAboutCitadelSeals = true;
                    state.UnlockedAmbush = true;
                    state.Gold += 30;
                    MessageBox.Show("Получено 30 золота.\nНачат квест: «Украденные тайны».\nОткрыта метка: Засада на тракте.", "Квест", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Brann_Mercenary")
                {
                    state.HasQuestAncestorsLegacy = true;
                    state.PromisedRuneOfProtection = true;
                    state.Gold += 50;
                    MessageBox.Show("Получен аванс: 50 золота!\nНачат квест: «Украденные тайны».", "Квест", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Gromgar_Honor")
                {
                    state.HonorGromgar = true;
                }

                // ==========================================
                // === ПЕРЕХОД К АКТУ 2 ===
                // ==========================================
                else if (choice.Action == "StartAct2")
                {
                    state.CurrentAct = 2;
                    state.CurrentNodeId = "Act2_Start";
                    LoadNode("Act2_Start");
                    return;
                }

                // ==========================================
                // === ДЕЙСТВИЯ АКТА 2 ===
                // ==========================================
                else if (choice.Action == "Act2_Gates_Loot")
                {
                    state.Gold += 40;
                    var key = ItemsDatabase.Clone("Q_CitadelKey");
                    if (key != null && !state.Inventory.Exists(i => i.Id == key.Id)) state.Inventory.Add(key);
                    MessageBox.Show("В инвентарь добавлен: Ключ от Внутренней Цитадели.\nПолучено 40 золота!", "Добыча", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Act2_Kael_Mercy")
                {
                    state.SavedPrisonerKael = true;
                    state.HasShadowGem = true;
                    state.KnowsMalkorWeakness = true;
                    var gem = ItemsDatabase.Clone("Q_ShadowGem");
                    if (gem != null && !state.Inventory.Exists(i => i.Id == gem.Id)) state.Inventory.Add(gem);
                    state.Gold += 25;
                    MessageBox.Show("Каэль освобожден!\nВ инвентарь добавлен: Теневой Кристалл Каэля.\nРаскрыта уязвимость Архимага Малкора!", "Союзник", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Act2_Kael_Ruthless")
                {
                    state.HasShadowGem = true;
                    state.KnowsMalkorWeakness = true;
                    state.Gold += 100;
                    state.StatPoints += 1;
                    var gem = ItemsDatabase.Clone("Q_ShadowGem");
                    if (gem != null && !state.Inventory.Exists(i => i.Id == gem.Id)) state.Inventory.Add(gem);
                    MessageBox.Show("Теневой Кристалл отобран силой!\nПолучено: 100 золота, +1 Очко Характеристик.", "Добыча", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Act2_Library_Research")
                {
                    state.KnowsMalkorWeakness = true;
                    state.SkillPoints += 1;
                    var ring = ItemsDatabase.Clone("R_RingOfArcaneFury_Epic");
                    if (ring != null && !state.Inventory.Exists(i => i.Id == ring.Id)) state.Inventory.Add(ring);
                    MessageBox.Show("Древние гримуары расшифрованы!\nПолучен: Перстень Тайной Ярости.\nПолучено: +1 Очко Навыков.", "Исследования", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Act2_Library_Elixir")
                {
                    state.HasPurificationElixir = true;
                    state.HpPotions = Math.Max(state.HpPotions, 4);
                    state.MpPotions = Math.Max(state.MpPotions, 3);
                    var elix = ItemsDatabase.Clone("Q_PurificationElixir");
                    if (elix != null && !state.Inventory.Exists(i => i.Id == elix.Id)) state.Inventory.Add(elix);
                    MessageBox.Show("Сварен: Эликсир Очищения Скверны!\nВсе зелья здоровья и маны пополнены до максимума.", "Алхимия", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Act2_Library_Dark")
                {
                    state.PlayerHp = Math.Max(10, state.PlayerHp - 20);
                    state.StatPoints += 2;
                    MessageBox.Show("Темная магия напитала вашу силу (-20 HP).\nПолучено: +2 Очка Характеристик!", "Темный Алтарь", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Act2_Golem_Repair")
                {
                    state.GolemAllied = true;
                    MessageBox.Show("Рунный Голем восстановлен и признал вас союзником!\nОн нанесет мощный удар в решающей битве!", "Союзник", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Act2_Golem_Drain")
                {
                    var heart = ItemsDatabase.Clone("A_GolemHeart_Epic");
                    if (heart != null && !state.Inventory.Exists(i => i.Id == heart.Id)) state.Inventory.Add(heart);
                    MessageBox.Show("Получен эпический амулет: Сердце Рунного Голема!", "Добыча", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Act2_Ending_Purify")
                {
                    state.Act2Completed = true;
                    var sword = ItemsDatabase.Clone("W_SolarBlade_Legendary");
                    if (sword != null && !state.Inventory.Exists(i => i.Id == sword.Id)) state.Inventory.Add(sword);
                    MessageBox.Show("Светлый финал достигнут!\nВ инвентарь добавлен: Клинок Солнечного Очищения (Легендарный)!", "Триумф", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Act2_Ending_Domination")
                {
                    state.Act2Completed = true;
                    var robe = ItemsDatabase.Clone("C_ArchmageRobe_Legendary");
                    if (robe != null && !state.Inventory.Exists(i => i.Id == robe.Id)) state.Inventory.Add(robe);
                    MessageBox.Show("Темный финал достигнут!\nВ инвентарь добавлена: Мантия Первородной Тьмы (Легендарная)!", "Власть", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (choice.Action == "Act2_Ending_Balance")
                {
                    state.Act2Completed = true;
                    var emblem = ItemsDatabase.Clone("A_SolarEmblem_Legendary");
                    if (emblem != null && !state.Inventory.Exists(i => i.Id == emblem.Id)) state.Inventory.Add(emblem);
                    MessageBox.Show("Финал Равновесия достигнут!\nВ инвентарь добавлена: Эмблема Солнечного Очищения (Легендарная)!", "Равновесие", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // ==========================================
                // === НАВИГАЦИЯ И БОИ ===
                // ==========================================
                if (choice.Action == "StartBattle")
                {
                    if (choice.ActionParam == "Gromgar_Honor")
                    {
                        state.HonorGromgar = true;
                    }
                    NavigationManager.NavigateTo(new BattleView(isArcade: false, returnNodeId: choice.NextNodeId, battleModifier: choice.ActionParam));
                }
                else if (choice.Action == "MainMenu")
                {
                    NavigationManager.NavigateTo(new MainMenu());
                }
                else if (choice.Action == "CheckTomeIntact")
                {
                    if (state.HasTomeIntact)
                    {
                        LoadNode("Scene3_TomeIntact");
                    }
                    else
                    {
                        LoadNode("Scene3_TomeDamaged");
                    }
                }
                else
                {
                    LoadNode(choice.NextNodeId);
                }
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                PauseOverlay.Visibility = PauseOverlay.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void ResumeBtn_Click(object sender, RoutedEventArgs e)
        {
            PauseOverlay.Visibility = Visibility.Collapsed;
            this.Focus();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new SaveLoadView(isSaving: true));
        }

        private void MainMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateTo(new MainMenu());
        }
    }
}
