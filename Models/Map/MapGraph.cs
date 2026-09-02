using System.Collections.Generic;
using System.Linq;

namespace EpicBattle.Models.Map
{
    public class MapFloorGroup
    {
        public int FloorNumber { get; set; }
        public string FloorTitle { get; set; } = string.Empty;
        public List<MapNode> Nodes { get; set; } = new List<MapNode>();
        public bool IsCurrentFloor { get; set; }
    }

    public class MapGraph
    {
        public int ActNumber { get; set; } = 1;
        public string ActTitle { get; set; } = "Акт 1: Предгорья Железного Кряжа";
        public int TotalFloors { get; set; } = 10;
        public int CurrentFloor { get; set; } = 0;
        public string? CurrentNodeId { get; set; }
        public List<MapNode> Nodes { get; set; } = new List<MapNode>();
        public List<string> VisitedNodeIds { get; set; } = new List<string>();
        public bool IsCompleted { get; set; } = false;

        public MapNode? GetNode(string id) => Nodes.FirstOrDefault(n => n.Id == id);

        public List<MapNode> GetNodesOnFloor(int floor) => Nodes.Where(n => n.Floor == floor).OrderBy(n => n.PositionIndex).ToList();

        public List<MapNode> GetAvailableNodes() => Nodes.Where(n => n.IsAvailable && !n.IsVisited).ToList();

        /// <summary>
        /// Переход в выбранный узел карты
        /// </summary>
        public void SelectNode(string nodeId)
        {
            var target = GetNode(nodeId);
            if (target == null || !target.IsAvailable) return;

            foreach (var n in Nodes)
            {
                n.IsCurrent = false;
                n.IsAvailable = false;
            }

            target.IsCurrent = true;
            CurrentNodeId = target.Id;
            CurrentFloor = target.Floor;
        }

        /// <summary>
        /// Завершение текущего узла и открытие путей на следующий этаж
        /// </summary>
        public void CompleteCurrentNode()
        {
            if (string.IsNullOrEmpty(CurrentNodeId)) return;
            var current = GetNode(CurrentNodeId);
            if (current == null) return;

            current.IsVisited = true;
            current.IsCurrent = false;
            current.IsAvailable = false;
            if (!VisitedNodeIds.Contains(current.Id))
            {
                VisitedNodeIds.Add(current.Id);
            }

            if (current.Type == MapNodeType.Boss)
            {
                IsCompleted = true;
                return;
            }

            foreach (var nextId in current.OutgoingNodeIds)
            {
                var nextNode = GetNode(nextId);
                if (nextNode != null && !nextNode.IsVisited)
                {
                    nextNode.IsAvailable = true;
                }
            }
        }

        public List<MapFloorGroup> GetFloorGroups(bool reverseForVerticalClimb = true)
        {
            var groups = new List<MapFloorGroup>();
            for (int f = 0; f <= TotalFloors; f++)
            {
                var nodes = GetNodesOnFloor(f);
                string title = f == TotalFloors 
                    ? "👑 ВЕРШИНА: БОСС" 
                    : (f == 0 ? "🚩 СТАРТ ПОХОДА" : $"Этаж {f}");

                groups.Add(new MapFloorGroup
                {
                    FloorNumber = f,
                    FloorTitle = title,
                    Nodes = nodes,
                    IsCurrentFloor = f == CurrentFloor
                });
            }

            if (reverseForVerticalClimb)
            {
                groups.Reverse();
            }

            return groups;
        }
    }
}
