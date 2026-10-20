using EpicBattle.Models.Combat;

using System;

using System.Collections.Generic;

using System.Collections.ObjectModel;

using System.ComponentModel;

using System.Linq;

using System.Runtime.CompilerServices;

using System.Windows.Input;

using EpicBattle.Managers;

using EpicBattle.Models;

using EpicBattle.Models.Items;



namespace EpicBattle.ViewModels

{

    public class MerchantOfferItem : INotifyPropertyChanged

    {

        public Item Item { get; set; } = new Item();

        public int Price { get; set; }

        public string Title => Item.Name;

        public string Description => Item.Description;

        public string Icon => Item.IconPath;

        public string RarityName => Item.Rarity.ToString();

        public string RarityColor => Item.RarityColor;

        public string StatsSummary => Item.StatsSummary;

        public string PriceText => $"💰 {Price}";



        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)

        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

    }



    public class PlayerSellItem : INotifyPropertyChanged

    {

        public Item Item { get; set; } = new Item();

        public int SellPrice { get; set; }

        public string Title => Item.Name;

        public string Description => Item.Description;

        public string Icon => Item.IconPath;

        public string RarityColor => Item.RarityColor;

        public string StatsSummary => Item.StatsSummary;

        public string PriceText => $"💰 +{SellPrice}";



        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)

        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

    }



    public class MerchantViewModel : INotifyPropertyChanged

    {

        private readonly GameState _gameState;

        private readonly Action _onLeave;

        private static readonly RngProxy rand = new RngProxy();



        public ObservableCollection<MerchantOfferItem> ShopItems { get; set; } = new ObservableCollection<MerchantOfferItem>();

        public ObservableCollection<PlayerSellItem> PlayerItems { get; set; } = new ObservableCollection<PlayerSellItem>();



        public int PlayerGold => _gameState.Gold;

        public int PlayerHp => _gameState.PlayerHp;

        public int PlayerMaxHp => _gameState.PlayerMaxHp;

        public int PlayerMp => _gameState.PlayerMp;

        public int PlayerMaxMp => _gameState.PlayerMaxMp;

        public int OreStock => _gameState.Ore;

        public int HpPotions => _gameState.HpPotions;

        public int MpPotions => _gameState.MpPotions;



        private string _statusMessage = "Добро пожаловать в мою лавку! Выбирай с умом — на перевале скидок не будет.";

        public string StatusMessage

        {

            get => _statusMessage;

            set { _statusMessage = value; OnPropertyChanged(); }

        }



        public ICommand BuyItemCommand => new Command(BuyItem);

        public ICommand SellItemCommand => new Command(SellItem);

        public ICommand BuyHpPotionCommand => new Command(_ => BuyPotion(true));

        public ICommand BuyMpPotionCommand => new Command(_ => BuyPotion(false));

        public ICommand BuyStatScrollCommand => new Command(_ => BuyStatScroll());

        public ICommand BuySkillTomeCommand => new Command(_ => BuySkillTome());

        public ICommand SellOreCommand => new Command(_ => SellOre());

        public ICommand LeaveShopCommand => new Command(_ => { EpicBattle.Services.AudioEngine.Play("door_wood"); _onLeave?.Invoke(); });



        public MerchantViewModel(Action onLeave)

        {

            _gameState = SaveManager.CurrentState;

            _onLeave = onLeave;

            GenerateShopAssortment();

            RefreshPlayerItems();

        }



        private void GenerateShopAssortment()

        {

            ShopItems.Clear();



            // Выбираем 4-6 случайных предметов под уровень

            int count = rand.Next(4, 7);

            var candidates = ItemsDatabase.AllItems

                .Where(i => i.Slot != ItemSlot.Quest && i.Slot != ItemSlot.Consumable)

                .OrderBy(_ => rand.Next())

                .Take(count);



            foreach (var orig in candidates)

            {

                var clone = ItemsDatabase.Clone(orig.Id);

                if (clone != null)

                {

                    int price = Math.Max(25, clone.GoldValue);

                    ShopItems.Add(new MerchantOfferItem { Item = clone, Price = price });

                }

            }

        }



        private void RefreshPlayerItems()

        {

            PlayerItems.Clear();

            foreach (var item in _gameState.Inventory)

            {

                if (item.Slot != ItemSlot.Quest)

                {

                    int sellPrice = Math.Max(8, (int)(item.GoldValue * 0.35));

                    PlayerItems.Add(new PlayerSellItem { Item = item, SellPrice = sellPrice });

                }

            }

        }



        private void BuyItem(object param)

        {

            if (param is MerchantOfferItem offer)

            {
                EpicBattle.Services.AudioEngine.Play("click");

                if (_gameState.Gold < offer.Price)

                {

                    StatusMessage = "❌ Недостаточно золота для покупки этого предмета!";

                    return;

                }



                _gameState.Gold -= offer.Price;

                _gameState.Inventory.Add(offer.Item);

                ShopItems.Remove(offer);



                StatusMessage = $"✅ Вы приобрели [{offer.Item.Name}] за {offer.Price} 💰.";

                RefreshUI();

            }

        }



        private void SellItem(object param)

        {

            if (param is PlayerSellItem sell)

            {
                EpicBattle.Services.AudioEngine.Play("click_alt");

                _gameState.Inventory.Remove(sell.Item);

                _gameState.Gold += sell.SellPrice;

                PlayerItems.Remove(sell);



                StatusMessage = $"💰 Вы продали [{sell.Item.Name}] и получили +{sell.SellPrice} 💰.";

                RefreshUI();

            }

        }



        private void BuyPotion(bool isHp)

        {
            EpicBattle.Services.AudioEngine.Play("click");

            int cost = isHp ? 20 : 15;

            if (_gameState.Gold < cost)

            {

                StatusMessage = $"❌ Недостаточно золота для покупки зелья ({cost} 💰)!";

                return;

            }



            _gameState.Gold -= cost;

            if (isHp) _gameState.HpPotions++;

            else _gameState.MpPotions++;



            StatusMessage = isHp ? "🧪 Вы купили Зелье Здоровья (+1) за 20 💰." : "💧 Вы купили Зелье Маны (+1) за 15 💰.";

            RefreshUI();

        }



        private bool _statScrollBought = false;

        private bool _skillTomeBought = false;



        private void BuyStatScroll()

        {
            EpicBattle.Services.AudioEngine.Play("click");

            if (_statScrollBought)

            {

                StatusMessage = "❌ У купца больше нет Свитков Силы на сегодня!";

                return;

            }

            int cost = 110;

            if (_gameState.Gold < cost)

            {

                StatusMessage = $"❌ Недостаточно золота для покупки Свитка Силы ({cost} 💰)!";

                return;

            }



            _gameState.Gold -= cost;

            _gameState.StatPoints += 1;

            _statScrollBought = true;

            StatusMessage = "📜 Вы прочли Свиток Силы и получили +1 Очко Характеристик! (Товар распродан)";

            RefreshUI();

        }



        private void BuySkillTome()

        {
            EpicBattle.Services.AudioEngine.Play("click");

            if (_skillTomeBought)

            {

                StatusMessage = "❌ У купца больше нет Фолиантов Знаний на сегодня!";

                return;

            }

            int cost = 140;

            if (_gameState.Gold < cost)

            {

                StatusMessage = $"❌ Недостаточно золота для покупки Фолианта Знаний ({cost} 💰)!";

                return;

            }



            _gameState.Gold -= cost;

            _gameState.SkillPoints += 1;

            _skillTomeBought = true;

            StatusMessage = "📖 Вы изучили Фолиант Знаний и получили +1 Очко Навыков! (Товар распродан)";

            RefreshUI();

        }



        private void SellOre()

        {

            if (_gameState.Ore <= 0)

            {

                StatusMessage = "❌ У тебя нет Железной Руды Кряжа! Ищи её в горах после боёв.";

                return;

            }



            // P1-фикс (Б4): раньше продавалась ВСЯ руда одним кликом без подтверждения —

            // случайный клик уничтожал ресурсную базу крафта. Теперь продажа порционная (по 1).

            const int orePrice = 25;

            _gameState.Ore -= 1;

            _gameState.Gold += orePrice;

            StatusMessage = $"⛏️ Ростовщик купил 1 кусок руды за {orePrice} 💰. Осталось ⛏️ {_gameState.Ore}. Помни: кузнецу без руды не поможетешь...";

            RefreshUI();

        }



        private void RefreshUI()

        {

            OnPropertyChanged(nameof(PlayerGold));

            OnPropertyChanged(nameof(OreStock));

            OnPropertyChanged(nameof(HpPotions));

            OnPropertyChanged(nameof(MpPotions));

            RefreshPlayerItems();

        }



        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)

        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

    }

}

