using System.Collections.Generic;

namespace EpicBattle.Models.Map
{
    public class MapNode
    {
        public string Id { get; set; } = string.Empty;
        public int Floor { get; set; }
        public int PositionIndex { get; set; }
        public MapNodeType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public List<string> OutgoingNodeIds { get; set; } = new List<string>();
        
        public bool IsVisited { get; set; } = false;
        public bool IsAvailable { get; set; } = false;
        public bool IsCurrent { get; set; } = false;
        public string? EnemyModifier { get; set; }

        // --- Вспомогательные свойства для UI отображения в XAML ---
        public string NodeTypeName => Type switch
        {
            MapNodeType.NormalBattle => "⚔️ Обычный бой",
            MapNodeType.EliteBattle => "💀 Элитный бой",
            MapNodeType.Encounter => "📜 Событие",
            MapNodeType.Merchant => "💰 Торговец",
            MapNodeType.Campfire => "⛺ Привал",
            MapNodeType.Blacksmith => "🔨 Кузница",
            MapNodeType.Tavern => "🍺 Таверна",
            MapNodeType.Boss => "👑 Босс",
            _ => "Узел"
        };

        public string StatusColor => IsCurrent ? "#00E5FF" : (IsAvailable ? "#FFAA00" : (IsVisited ? "#777777" : "#333333"));

        public string CardBackground => IsCurrent 
            ? "#1C3B45" 
            : (IsAvailable 
                ? (Type == MapNodeType.Boss ? "#4A1515" : (Type == MapNodeType.EliteBattle ? "#381A45" : "#232A23")) 
                : (IsVisited ? "#1E1E1E" : "#181818"));

        public string BadgeBackground => Type switch
        {
            MapNodeType.NormalBattle => "#2E6B30",
            MapNodeType.EliteBattle => "#6A1B9A",
            MapNodeType.Encounter => "#1565C0",
            MapNodeType.Merchant => "#E65100",
            MapNodeType.Campfire => "#2E7D32",
            MapNodeType.Blacksmith => "#8D6E63",
            MapNodeType.Tavern => "#A1887F",
            MapNodeType.Boss => "#B71C1C",
            _ => "#333333"
        };

        public double NodeOpacity => (IsAvailable || IsCurrent) ? 1.0 : (IsVisited ? 0.6 : 0.45);
        public bool IsClickable => IsAvailable && !IsVisited;

        public string ActionButtonText => IsCurrent 
            ? "📍 Вы здесь" 
            : (IsVisited ? "✅ Пройдено" : (IsAvailable ? "▶️ Вступить на путь" : "🔒 Заблокировано"));

        public string ActionButtonBackground => IsCurrent 
            ? "#1565C0" 
            : (IsVisited ? "#222222" : (IsAvailable ? "#2E6B30" : "#333333"));

        public string ActionButtonForeground => IsVisited ? "Gray" : "White";
    }
}
