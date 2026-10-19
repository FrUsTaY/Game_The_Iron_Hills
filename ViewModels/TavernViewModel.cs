using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using EpicBattle.Managers;
using EpicBattle.Models;
using EpicBattle.Models.Items;
using EpicBattle.Models.Combat;

namespace EpicBattle.ViewModels
{
    /// <summary>Карточка рунического камня для склада в таверне (продажа).</summary>
    public class TavernRuneCard
    {
        public string RuneId { get; set; } = "";
        public int Count { get; set; }
        public string Label { get; set; } = "";
        public int SellPriceEach { get; set; }
    }

    /// <summary>
    /// Этап 3.2 (часть 3): Таверна — отдых за золото (цена растёт при частых визитах),
    /// слухи трактирщика (безбаффовый flavor + мелкая польза) и скупка рунических камней.
    /// </summary>
    public class TavernViewModel : INotifyPropertyChanged
    {
        private readonly GameState _gameState;
        private readonly Action _onLeave;
        private static readonly RngProxy rand = new RngProxy();

        public const int BaseRestCost = 50;   // 💰 цена ночлега
        public const int RestHpPercent = 45;  // % восстановления HP
        public const int RestMpPercent = 60;  // % восстановления MP
        public const int RumorCost = 15;      // 💰 кружка эля за слух
        public const int RuneSellDiscount = 70; // % от GoldValue рун при продаже

        private bool _restUsed;
        private bool _rumorUsed;

        public int PlayerGold => _gameState.Gold;
        public int PlayerHp => _gameState.PlayerHp;
        public int PlayerMaxHp => _gameState.PlayerMaxHp;
        public int PlayerMp => _gameState.PlayerMp;
        public int PlayerMaxMp => _gameState.PlayerMaxMp;

        /// <summary>Цена отдыха: ×1.5 за каждый повторный визит подряд (TavernRestStreak), кап ×4.</summary>
        public int RestCost
        {
            get
            {
                double mult = Math.Min(4.0, Math.Pow(1.5, _gameState.TavernRestStreak));
                return (int)(BaseRestCost * mult / 10) * 10; // округляем до десятков
            }
        }

        public bool CanRest => !_restUsed && _gameState.Gold >= RestCost;
        public bool CanHearRumor => !_rumorUsed && _gameState.Gold >= RumorCost;
        public bool HasRunesToSell => _gameState.RuneStock.Count > 0;

        public string RestButtonLabel => _restUsed
            ? "Вы уже переночевали здесь"
            : $"🛏️ Переночевать (💰 {RestCost})";

        private string _statusMessage = "\"Проходи, гость! У меня тёплый эль, мягкая постель и скупка диковинных камней. Грр... то есть добрый вечер!\"";
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private string _rumorText = "";
        public string RumorText
        {
            get => _rumorText;
            set { _rumorText = value; OnPropertyChanged(); }
        }

        public List<TavernRuneCard> RuneSaleList
        {
            get
            {
                var list = new List<TavernRuneCard>();
                foreach (var kv in _gameState.RuneStock.OrderByDescending(kv => kv.Value))
                {
                    var rune = RuneDatabase.Find(kv.Key);
                    if (rune == null || kv.Value <= 0) continue;
                    int price = Math.Max(10, rune.GoldValue * RuneSellDiscount / 100);
                    list.Add(new TavernRuneCard
                    {
                        RuneId = kv.Key,
                        Count = kv.Value,
                        SellPriceEach = price,
                        Label = $"{rune.Icon} {rune.Name} ×{kv.Value} ({rune.FormattedBonus})"
                    });
                }
                return list;
            }
        }

        public ICommand RestCommand => new Command(_ => Rest());
        public ICommand RumorCommand => new Command(_ => HearRumor());
        public ICommand SellRuneCommand => new Command(param => SellRune(param as string));
        public ICommand LeaveTavernCommand => new Command(_ => _onLeave?.Invoke());

        public TavernViewModel(Action onLeave)
        {
            _gameState = SaveManager.CurrentState;
            _onLeave = onLeave;
        }

        private void Rest()
        {
            if (_restUsed) return;
            int cost = RestCost;
            if (_gameState.Gold < cost)
            {
                StatusMessage = $"❌ Ночлег стоит {cost} 💰! Трактирщик презрительно хмыкает.";
                return;
            }

            _restUsed = true;
            _gameState.Gold -= cost;
            _gameState.TavernRestStreak += 1;

            int hpGain = Math.Max(0, _gameState.PlayerMaxHp * RestHpPercent / 100);
            int mpGain = Math.Max(0, _gameState.PlayerMaxMp * RestMpPercent / 100);
            _gameState.PlayerHp = Math.Min(_gameState.PlayerMaxHp, _gameState.PlayerHp + hpGain);
            _gameState.PlayerMp = Math.Min(_gameState.PlayerMaxMp, _gameState.PlayerMp + mpGain);

            StatusMessage = $"🛏️ Вы проспали до утра как убитый: +{hpGain} ❤️, +{mpGain} 🔷 (−{cost} 💰). "
                          + (_gameState.TavernRestStreak > 1 ? "Но завсегдатаи платят больше — следующий ночлёг дороже." : "");
            RefreshAll();
        }

        private void HearRumor()
        {
            if (_rumorUsed) return;
            if (_gameState.Gold < RumorCost)
            {
                StatusMessage = "❌ Кружка эля стоит 15 💰! Без эля трактирщик молчит как рыба.";
                return;
            }

            _rumorUsed = true;
            _gameState.Gold -= RumorCost;

            // Слухи: flavor + мелкая механическая польза (одноразовая)
            int roll = rand.Next(100);
            if (roll < 30)
            {
                _gameState.HpPotions += 1;
                RumorText = "\"Ходят слухи, что в подвале бочка с целебным отваром...\" 🧪 +1 Зелье лечения!";
                StatusMessage = "🗣️ Слух оказался прибыльным!";
            }
            else if (roll < 55)
            {
                var stone = RuneDatabase.RollRandomDrop(fromBoss: false);
                _gameState.AddRune(stone.Id);
                RumorText = $"\"Говорят, в обломках кузни лежит {stone.Icon} {stone.Name}...\" 💠 Камень добавлен в ваш склад!";
                StatusMessage = "🗣️ Слух привёл вас к находке!";
            }
            else if (roll < 80)
            {
                int gold = 20 + rand.Next(40);
                _gameState.Gold += gold;
                RumorText = $"\"За столом забыли кошелёк...\" 💰 +{gold} золота! Трактирщик делает вид, что не заметил.";
                StatusMessage = "🗣️ Повезло с компанией!";
            }
            else
            {
                RumorText = rand.Next(2) == 0
                    ? "\"Слышал, на верхних этажах перевала орки варят похлёбку из грибов-дождевиков. Не ешьте.\" — бесполезный, но правдивый слух."
                    : "\"Громгар снова ворчит на своих берсерков. Говорят, он в ярости страшен... а до ярости ещё страшнее.\" — трактирщик хихикает.";
                StatusMessage = "🗣️ За 15 💰 вы узнали, что эль тут разбавленный. Классика.";
            }
            RefreshAll();
        }

        private void SellRune(string? runeIdKey)
        {
            if (string.IsNullOrEmpty(runeIdKey)) return;
            string id = runeIdKey.Split('|')[0];
            var rune = RuneDatabase.Find(id);
            if (rune == null || !_gameState.TryTakeRune(id))
            {
                StatusMessage = "❌ Такого камня у вас нет.";
                return;
            }
            int price = Math.Max(10, rune.GoldValue * RuneSellDiscount / 100);
            _gameState.Gold += price;
            StatusMessage = $"💰 Трактирщик с недоумением рассматривает {rune.Icon} «{rune.Name}» и отсчитывает {price} 💰. \"На подстаканники пойдёт...\"";
            RefreshAll();
        }

        private void RefreshAll()
        {
            OnPropertyChanged(nameof(PlayerGold));
            OnPropertyChanged(nameof(PlayerHp));
            OnPropertyChanged(nameof(PlayerMaxHp));
            OnPropertyChanged(nameof(PlayerMp));
            OnPropertyChanged(nameof(PlayerMaxMp));
            OnPropertyChanged(nameof(RestCost));
            OnPropertyChanged(nameof(RestButtonLabel));
            OnPropertyChanged(nameof(RuneSaleList));
            OnPropertyChanged(nameof(CanRest));
            OnPropertyChanged(nameof(CanHearRumor));
            OnPropertyChanged(nameof(HasRunesToSell));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
