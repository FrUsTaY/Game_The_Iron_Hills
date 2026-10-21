using System.ComponentModel;
using EpicBattle.Services;

namespace EpicBattle.Models.Combat
{
    public enum FloatingTextKind
    {
        Damage,     // белый урон
        Crit,       // критический (жёлтый, крупный)
        Magic,      // магический (фиолетовый)
        Heal,       // исцеление (зелёный)
        Miss,       // промах/уклонение (серый)
        Info        // прочее (оглушение, щит и т.п.)
    }

    /// <summary>
    /// Всплывающая боевая цифра (Floating Combat Text) — Этап 3.3.
    /// Создаётся парсером лога боя, анимируется в BattleView и самоудаляется по таймеру.
    /// </summary>
    public class FloatingText : INotifyPropertyChanged
    {
        private double _offsetY;
        public double OffsetY
        {
            get => _offsetY;
            set { _offsetY = value; OnPropertyChanged(nameof(OffsetY)); }
        }

        private double _opacity = 1.0;
        public double Opacity
        {
            get => _opacity;
            set { _opacity = value; OnPropertyChanged(nameof(Opacity)); }
        }

        private double _scale = 1.0;
        public double Scale
        {
            get => _scale;
            set { _scale = value; OnPropertyChanged(nameof(Scale)); }
        }

        public string Text { get; }
        public FloatingTextKind Kind { get; }

        /// <summary>"Player" или имя конкретного врага — над кем показывать.</summary>
        public string TargetKey { get; }

        /// <summary>Смещение внутри колонки, чтобы одновременные цифры не слипались.</summary>
        public double OffsetX { get; }

        /// <summary>Тряска экрана при тяжёлом ударе (босс/элитный удар по игроку).</summary>
        public bool TriggersShake { get; }

        public string ForegroundBrush => Kind switch
        {
            FloatingTextKind.Damage => "#F2F2F2",
            FloatingTextKind.Crit => "#FFD24A",
            FloatingTextKind.Magic => "#C77DFF",
            FloatingTextKind.Heal => "#55FF88",
            FloatingTextKind.Miss => "#9E9E9E",
            _ => "#BFD7ED"
        };

        public int FontSize => Kind switch
        {
            FloatingTextKind.Crit => 30,
            FloatingTextKind.Magic => 22,
            FloatingTextKind.Miss => 16,
            _ => 20
        };

        public FloatingText(string text, FloatingTextKind kind, string targetKey, bool triggersShake = false)
        {
            Text = text;
            Kind = kind;
            TargetKey = targetKey;
            TriggersShake = triggersShake;
            var rng = EpicBattle.Services.GameRng.Current;
            OffsetX = rng.NextDouble() * 44 - 22;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
