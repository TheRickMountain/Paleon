using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class TradingPostBuildingCmp : BuildingCmp
    {

        private Dictionary<ITradable, int> goods;

        private const int NUMBER_OF_POSITIONS = 8;

        // Максимальное количество для самого дорогого товара
        private const int BASE_QUANTITY = 5;

        public TradingPostBuildingCmp(BuildingTemplate buildingTemplate, Direction direction)
            : base(buildingTemplate, direction)
        {
            goods = new Dictionary<ITradable, int>();
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            GameplayScene.Instance.TradingSystem.AddTradingPost(this);
        }

        public override void DestructBuilding()
        {
            base.DestructBuilding();

            GameplayScene.Instance.TradingSystem.RemoveTradingPost(this);
        }

        public void GenerateAssortment(List<ITradable> unlockedGoods)
        {
            goods.Clear();

            int maxGoodPrice = GetMaxPrice(unlockedGoods);

            List<ITradable> randomGoods = Calc.Random.GetRandomUniqueItems(unlockedGoods, NUMBER_OF_POSITIONS);

            foreach (var good in randomGoods)
            {
                if (good is Item)
                {
                    float priceRatio = (float)maxGoodPrice / good.GetMarketPrice();
                    int quantity = (int)Math.Round(BASE_QUANTITY * priceRatio);

                    goods.Add(good, quantity);
                }
                else if (good is AnimalTemplate)
                {
                    int quantity = MyRandom.FromSet(1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4, 5);

                    goods.Add(good, quantity);
                }
            }
        }

        public void SetAssortment(Dictionary<ITradable, int> goodsAssortment)
        {
            goods.Clear();

            foreach (var kvp in goodsAssortment)
            {
                goods.Add(kvp.Key, kvp.Value);
            }
        }

        private int GetMaxPrice(List<ITradable> tradables)
        {
            return tradables.Where(x => x is Item).Max(y => y.GetMarketPrice());
        }

        public override BuildingSaveData GetSaveData()
        {
            BuildingSaveData data = base.GetSaveData();

            data.TradingPostAnimals = new Dictionary<string, int>();
            data.TradingPostItems = new Dictionary<int, int>();

            foreach(var kvp in goods)
            {
                ITradable good = kvp.Key;
                int quantity = kvp.Value;

                if(good is AnimalTemplate)
                {
                    data.TradingPostAnimals.Add((good as AnimalTemplate).Json, quantity);
                }
                else if(good is Item)
                {
                    data.TradingPostItems.Add((good as Item).Id, quantity);
                }
            }

            return data;
        }

        public IEnumerable<(ITradable, int)> GetGoods()
        {
            foreach (var kvp in goods)
            {
                yield return (kvp.Key, kvp.Value); 
            }
        }

    }
}
