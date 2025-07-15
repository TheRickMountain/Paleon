using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Technolithic
{
    public class TradingSystem
    {
        public Dictionary<ITradable, int> ToBuyItems { get; private set; }
        public Dictionary<ITradable, int> ToSellItems { get; private set; }

        public Dictionary<ITradable, int> TraderItems { get; private set; }
        public Dictionary<ITradable, int> StockItems { get; private set; }

        private List<TradingPostBuildingCmp> tradingPosts = new List<TradingPostBuildingCmp>();

        private List<ITradable> unlockedGoods = new List<ITradable>();

        private TradingPostBuildingCmp activeTradingPost;

        public TradingSystem()
        {
            ToBuyItems = new Dictionary<ITradable, int>();
            ToSellItems = new Dictionary<ITradable, int>();

            TraderItems = new Dictionary<ITradable, int>();
            StockItems = new Dictionary<ITradable, int>();
        }

        public void Begin()
        {
            GameplayScene.Instance.WorldState.OnNextDayStartedCallback += OnNextDayStarted;
        }

        private void OnNextDayStarted(int day, Season season)
        {
            UpdateUnlockedGoods();
            UpdateTradingPostsAssortment();

            if (tradingPosts.Count == 0) return;

            GameplayScene.UIRootNodeScript.NotificationsUI.GetComponent<NotificationsUIScript>()
                                    .AddNotification(
                                        Localization.GetLocalizedText("trading_post_assortment_update"),
                                        NotificationLevel.INFO,
                                        null);
        }

        private void UpdateUnlockedGoods()
        {
            // Составляем список всех доступных для продажи товаров
            unlockedGoods.Clear();

            foreach (AnimalTemplate animalTemplate in GameplayScene.Instance.ProgressTree.GetUnlockedAnimalTemplates())
            {
                unlockedGoods.Add(animalTemplate);
            }

            foreach (var kvp in GameplayScene.Instance.ProgressTree.UnlockedItems)
            {
                Item item = kvp.Key;
                bool isUnlocked = kvp.Value;

                if (isUnlocked == false) continue;

                // Товары имеющие цену 0 невозможно купить
                if (item.Value == 0) continue;

                unlockedGoods.Add(item);
            }
        }

        private void UpdateTradingPostsAssortment()
        {
            foreach (var tradingPost in tradingPosts)
            {
                tradingPost.GenerateAssortment(unlockedGoods);
            }
        }

        public void SetActiveTradingPost(TradingPostBuildingCmp tradingPost)
        {
            activeTradingPost = tradingPost;

            UpdateTraderGoods();

            UpdateStockGoods();
        }

        private void UpdateTraderGoods()
        {
            TraderItems.Clear();
            ToBuyItems.Clear();

            foreach (var kvp in activeTradingPost.GetGoods())
            {
                ITradable good = kvp.Item1;
                int quantity = kvp.Item2;

                TraderItems.Add(good, quantity);
            }
        }

        private void UpdateStockGoods()
        {
            StockItems.Clear();
            ToSellItems.Clear();

            foreach (var kvp in ItemDatabase.Items)
            {
                Item item = kvp.Value;

                // Товары имеющие цену в 0 нельзя продавать
                if (item.Value == 0)
                    continue;

                int availableWeight = 0;

                foreach (var storage in GameplayScene.WorldManager.StorageManager.Storages)
                {
                    if (storage.Inventory.GetAvailableItemCount(item) > 0)
                        availableWeight += storage.Inventory.GetAvailableItemCount(item);
                }

                if (availableWeight > 0)
                {
                    StockItems.Add(item, availableWeight);
                }
            }
        }

        public void RearrangeItems(Dictionary<ITradable, int> putTo, Dictionary<ITradable, int> takeFrom, ITradable item, int count)
        {
            if (takeFrom[item] == 0)
                return;

            if (takeFrom[item] > count)
            {
                takeFrom[item] -= count;
            }
            else
            {
                count = takeFrom[item];
                takeFrom[item] = 0;
            }

            if (putTo.ContainsKey(item) == false)
            {
                putTo.Add(item, count);
            }
            else
            {
                putTo[item] += count;
            }
        }

        public void Trade()
        {
            foreach (var kvp in ToSellItems)
            {
                ITradable tradable = kvp.Key;

                if (tradable is Item)
                {
                    Item item = (Item)tradable;

                    int weightToSell = kvp.Value;

                    foreach (var storage in GameplayScene.WorldManager.StorageManager.Storages)
                    {
                        int availableWeight = storage.Inventory.GetAvailableItemCount(item);
                        if (availableWeight <= 0)
                            continue;

                        if (weightToSell <= availableWeight)
                        {
                            storage.Inventory.PopItem(item, weightToSell);
                            break;
                        }
                        else
                        {
                            storage.Inventory.PopItem(item, availableWeight);
                            weightToSell -= availableWeight;
                        }
                    }
                }
            }

            // Achievement

            List<Item> itemsRequiredToAchievement = new List<Item>()
            {
                ItemDatabase.GetItemByName("flatbread"),
                ItemDatabase.GetItemByName("cheese"),
                ItemDatabase.GetItemByName("wool_cloth"),
                ItemDatabase.GetItemByName("pot_of_wine"),
                ItemDatabase.GetItemByName("honeycomb")
            };

            if(itemsRequiredToAchievement.All(key => ToSellItems.ContainsKey(key) && ToSellItems[key] >= 10))
            {
                GameplayScene.Instance.AchievementManager.UnlockAchievement(AchievementId.HOMEMADE_PRODUCTION);
            }

            // Achievement

            ToSellItems.Clear();

            Tile traderCurrentTile = activeTradingPost.GetCenterTile();

            foreach (var kvp in ToBuyItems)
            {
                ITradable tradable = kvp.Key;
                int tradableAmount = kvp.Value;

                if (tradable is Item)
                {
                    Item item = (Item) tradable;

                    traderCurrentTile.Inventory.AddCargo(item, tradableAmount);
                }
                else if(tradable is AnimalTemplate)
                {
                    for (int i = 0; i < tradableAmount; i++)
                    {
                        AnimalTemplate animalTemplate = (AnimalTemplate)tradable;

                        GameplayScene.Instance.SpawnAnimal(traderCurrentTile.X, traderCurrentTile.Y, animalTemplate, animalTemplate.DaysUntilAging);
                    }
                }
            }

            activeTradingPost.SetAssortment(TraderItems);

            ToBuyItems.Clear();
        }

        public bool CanCompleteTrade()
        {
            return GetTotalToSellPrice() >= GetTotalToBuyPrice();
        }

        public int GetTotalToBuyPrice()
        {
            return ToBuyItems.Sum(x => x.Key.GetMarketPrice() * x.Value);
        }

        public int GetTotalToSellPrice()
        {
            return ToSellItems.Sum(x => x.Key.GetMarketPrice() * x.Value);
        }

        public void AddTradingPost(TradingPostBuildingCmp tradingPost)
        {
            tradingPosts.Add(tradingPost);
        
            UpdateUnlockedGoods();

            tradingPost.GenerateAssortment(unlockedGoods);
        }

        public void RemoveTradingPost(TradingPostBuildingCmp tradingPost)
        {
            tradingPosts.Remove(tradingPost);
        }
    }
}