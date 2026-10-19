using System;
using System.Collections.Generic;
using System.Linq;

namespace EpicBattle.Models.Map
{
    public static class MapGenerator
    {
        private static readonly Random rand = new Random();

        public static MapGraph GenerateActMap(int actNumber, int totalFloors = 10)
        {
            var map = new MapGraph
            {
                ActNumber = actNumber,
                TotalFloors = totalFloors,
                CurrentFloor = 0,
                IsCompleted = false
            };

            switch (actNumber)
            {
                case 1:
                    map.ActTitle = "Акт 1: Предгорья Железного Кряжа";
                    break;
                case 2:
                    map.ActTitle = "Акт 2: Тени Древней Цитадели";
                    break;
                case 3:
                default:
                    map.ActTitle = "Акт 3: Недра Первородного Титана";
                    break;
            }

            var allNodes = new List<MapNode>();

            // --- ЭТАЖ 0: СТАРТОВЫЕ УЗЛЫ (3-4 узла) ---
            int startCount = 3;
            var floor0Nodes = new List<MapNode>();
            for (int i = 0; i < startCount; i++)
            {
                var type = rand.Next(100) < 65 ? MapNodeType.NormalBattle : MapNodeType.Encounter;
                var node = CreateNode(actNumber, 0, i, type);
                node.IsAvailable = true; // Стартовые узлы доступны для выбора
                allNodes.Add(node);
                floor0Nodes.Add(node);
            }

            var prevFloorNodes = floor0Nodes;

            // --- ПРОМЕЖУТОЧНЫЕ ЭТАЖИ (1 .. totalFloors - 1) ---
            for (int f = 1; f < totalFloors; f++)
            {
                var currFloorNodes = new List<MapNode>();

                if (f == totalFloors - 1)
                {
                    // Предбоссовый этаж: гарантированный привал / лагерь (2 узла)
                    for (int i = 0; i < 2; i++)
                    {
                        var node = CreateNode(actNumber, f, i, MapNodeType.Campfire);
                        allNodes.Add(node);
                        currFloorNodes.Add(node);
                    }
                }
                else
                {
                    int count = (f == 5) ? 3 : rand.Next(3, 5); // 3-4 узла на этаже
                    for (int i = 0; i < count; i++)
                    {
                        MapNodeType type;
                        int roll = rand.Next(100);

                        if (f < 3)
                        {
                            if (roll < 55) type = MapNodeType.NormalBattle;
                            else if (roll < 85) type = MapNodeType.Encounter;
                            else if (roll < 93) type = MapNodeType.Merchant;
                            else type = MapNodeType.Blacksmith;
                        }
                        else if (f == 5)
                        {
                            // Середина пути: привал, торговец или элитный бой
                            if (roll < 40) type = MapNodeType.Campfire;
                            else if (roll < 70) type = MapNodeType.EliteBattle;
                            else if (roll < 90) type = MapNodeType.Merchant;
                            else type = MapNodeType.Blacksmith;
                        }
                        else
                        {
                            if (roll < 40) type = MapNodeType.NormalBattle;
                            else if (roll < 65) type = MapNodeType.EliteBattle;
                            else if (roll < 80) type = MapNodeType.Encounter;
                            else if (roll < 88) type = MapNodeType.Merchant;
                            else if (roll < 94) type = MapNodeType.Blacksmith;
                            else type = MapNodeType.Campfire;
                        }

                        var node = CreateNode(actNumber, f, i, type);
                        allNodes.Add(node);
                        currFloorNodes.Add(node);
                    }
                }

                // Связываем пути: от каждого узла prevFloor к ближайшим узлам currFloor
                for (int pIdx = 0; pIdx < prevFloorNodes.Count; pIdx++)
                {
                    var pNode = prevFloorNodes[pIdx];
                    double pNorm = (double)pIdx / Math.Max(1, prevFloorNodes.Count - 1);

                    for (int cIdx = 0; cIdx < currFloorNodes.Count; cIdx++)
                    {
                        double cNorm = (double)cIdx / Math.Max(1, currFloorNodes.Count - 1);
                        if (Math.Abs(pNorm - cNorm) <= 0.6)
                        {
                            pNode.OutgoingNodeIds.Add(currFloorNodes[cIdx].Id);
                        }
                    }

                    if (pNode.OutgoingNodeIds.Count == 0)
                    {
                        pNode.OutgoingNodeIds.Add(currFloorNodes[0].Id);
                    }
                }

                // Гарантируем, что у каждого узла currFloor есть хотя бы один входящий путь
                foreach (var cNode in currFloorNodes)
                {
                    bool hasIncoming = prevFloorNodes.Any(p => p.OutgoingNodeIds.Contains(cNode.Id));
                    if (!hasIncoming)
                    {
                        var closestP = prevFloorNodes.OrderBy(p => Math.Abs(p.PositionIndex - cNode.PositionIndex)).First();
                        closestP.OutgoingNodeIds.Add(cNode.Id);
                    }
                }

                prevFloorNodes = currFloorNodes;
            }

            // --- ФИНАЛЬНЫЙ ЭТАЖ (totalFloors): БОСС ---
            var bossNode = CreateNode(actNumber, totalFloors, 0, MapNodeType.Boss);
            allNodes.Add(bossNode);

            foreach (var pNode in prevFloorNodes)
            {
                pNode.OutgoingNodeIds.Add(bossNode.Id);
            }

            map.Nodes = allNodes;
            return map;
        }

        private static MapNode CreateNode(int actNumber, int floor, int pos, MapNodeType type)
        {
            var node = new MapNode
            {
                Id = $"node_{floor}_{pos}",
                Floor = floor,
                PositionIndex = pos,
                Type = type
            };

            switch (type)
            {
                case MapNodeType.NormalBattle:
                    node.Icon = "⚔️";
                    node.Title = GetBattleTitle(actNumber);
                    node.Description = "Отряд врагов блокирует проход. Сразитесь с ними, чтобы добыть опыт и золото.";
                    break;
                case MapNodeType.EliteBattle:
                    node.Icon = "💀";
                    node.Title = GetEliteTitle(actNumber);
                    node.Description = "Опасный элитный противник! Гарантирует редкую/эпическую добычу и много золота.";
                    node.EnemyModifier = "Elite";
                    break;
                case MapNodeType.Encounter:
                    node.Icon = "📜";
                    node.Title = GetEncounterTitle(actNumber);
                    node.Description = "Загадочное событие. Проверьте характеристики героя и получите награды.";
                    break;
                case MapNodeType.Merchant:
                    node.Icon = "💰";
                    node.Title = GetMerchantTitle(actNumber);
                    node.Description = "Торговая лавка. Покупка редкого снаряжения, зелий и свитков, а также продажа лута.";
                    break;
                case MapNodeType.Blacksmith:
                    node.Icon = "🔨";
                    node.Title = GetBlacksmithTitle(actNumber);
                    node.Description = "Забытая кузница Кряжа. Мастер улучшит вашу экипировку за золото и редкие материалы.";
                    break;
                case MapNodeType.Campfire:
                    node.Icon = "⛺";
                    node.Title = GetCampfireTitle(actNumber);
                    node.Description = "Безопасный привал. Отдохните у костра, восстановите HP/MP или усильте оружие.";
                    break;
                case MapNodeType.Boss:
                    node.Icon = "👑";
                    node.Title = GetBossTitle(actNumber);
                    node.Description = "Финальный босс Акта! Величайшее испытание на вершине перевала.";
                    node.EnemyModifier = "Boss";
                    break;
            }

            return node;
        }

        private static string GetBattleTitle(int act) => act switch
        {
            1 => PickRandom("Орочий авангард", "Засада в ущелье", "Патруль орков", "Лагерь разбойников", "Орочьи застрельщики"),
            2 => PickRandom("Теневой караул", "Культисты Бездны", "Адепты Малкора", "Тюремная стража", "Оскверненные фанатики"),
            _ => PickRandom("Рунные големы", "Стражи магмы", "Огненные элементали")
        };

        private static string GetEliteTitle(int act) => act switch
        {
            1 => PickRandom("Воевода Железного Клыка", "Орочий Вождь-Берсерк", "Тролль-Крушитель"),
            2 => PickRandom("Теневой Инквизитор", "Древний Рунный Голем", "Верховный Жрец Тьмы"),
            _ => PickRandom("Рунный Колосс", "Хранитель Ядра")
        };

        private static string GetEncounterTitle(int act) => act switch
        {
            1 => PickRandom("Странный алтарь", "Лагерь следопыта", "Забытый тайник", "Каменный монолит"),
            2 => PickRandom("Запретный архив", "Алтарь Бездны", "Камера смертников", "Оскверненный фонтан"),
            _ => PickRandom("Древняя кузница", "Рунические письмена")
        };

        private static string GetMerchantTitle(int act) => act switch
        {
            1 => PickRandom("Караван рудокопов", "Оружейник-отшельник", "Странствующий купец"),
            2 => PickRandom("Контрабандист Цитадели", "Теневой ростовщик", "Маг-ренегат"),
            _ => PickRandom("Дворфийский мастер", "Дух древнего кузнеца")
        };

        private static string GetBlacksmithTitle(int act) => act switch
        {
            1 => PickRandom("Кузня Бородана", "Наковальня Рудокопа", "Горнище в скале"),
            2 => PickRandom("Рунная Кузница Голема", "Мастерская Теневых Литьев", "Заброшенный Молот"),
            _ => PickRandom("Сердце Кузни Титанов", "Магматический Горн")
        };

        private static string GetCampfireTitle(int act) => act switch
        {
            1 => PickRandom("Укромная расщелина", "Горный привал", "Костер следопытов"),
            2 => PickRandom("Освященный зал", "Башня стражи", "Тайное убежище"),
            _ => PickRandom("Зал Древних Королей", "Магматический очаг")
        };

        private static string GetBossTitle(int act) => act switch
        {
            1 => "Верховный Вождь Громгар",
            2 => "Архимаг Малкор (Теневое Воплощение)",
            _ => "Первородный Титан Кряжа"
        };

        private static string PickRandom(params string[] items) => items[rand.Next(items.Length)];
    }
}
