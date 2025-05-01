using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class NomadsManager
    {
        public List<SettlerInfo> Settlers { get; private set; }
        public Dictionary<Item, int> Items { get; private set; }

        int hoursSum = 0;

        private const int MIN_ITEMS_COUNT = 1;
        private const int MAX_ITEMS_COUNT = 5;

        private int[] migrantsComingCyclesChance = new int[] { 3, 3, 3, 3, 3, 3, 4, 4, 4, 5};
        private int[] migrantsNumberChance = new int[] { 1, 1, 1, 1, 1, 1, 2, 2, 2, 3 };

        public NomadsManager(NomadsManagerSaveData saveData)
        {
            if(saveData != null)
            {
                hoursSum = saveData.HoursSum;

                // TODO: Костыль для старых сохранений, удалить после пары обнов
                if(hoursSum > 5 * WorldState.HOURS_PER_CYCLE)
                {
                    hoursSum = MyRandom.FromSet(migrantsComingCyclesChance) * WorldState.HOURS_PER_CYCLE;
                }
            }
            else
            {
                hoursSum = MyRandom.FromSet(migrantsComingCyclesChance) * WorldState.HOURS_PER_CYCLE;
            }
        }

        public void Begin()
        {
            Settlers = new List<SettlerInfo>();
            Items = new Dictionary<Item, int>();
        }

        public void Reset()
        {
            Settlers.Clear();
            Items.Clear();

            hoursSum = MyRandom.FromSet(migrantsComingCyclesChance) * WorldState.HOURS_PER_CYCLE;
        }

        public void Spawn()
        {
            foreach(var settlerInfo in Settlers)
            {
                Settler settler = GameplayScene.Instance.SpawnSettler(30, 30, settlerInfo, GameplayScene.WorldManager.NewSettlerFoodRationFilters);
                GameplayScene.UIRootNodeScript.NotificationsUI.GetComponent<NotificationsUIScript>()
                .AddNotification(Localization.GetLocalizedText("has_joined_your_settlement", settler.Get<CreatureCmp>().Name), NotificationLevel.INFO, settler);
            }

            foreach(var kvp in Items)
            {
                Tile tile = GameplayScene.Instance.World.GetTileAt(30, 30);
                tile.Inventory.AddCargo(new ItemContainer(kvp.Key, kvp.Value, kvp.Key.Durability));
            }

            Reset();
        }

        public void NextHour(int hour)
        {
            hoursSum--;

            if (hoursSum <= 0)
            {
                Generate();

                GameplayScene.UIRootNodeScript.OpenNomadsUI();
            }
        }

        private void Generate()
        {
            Settlers.Clear();
            Items.Clear();

            int settlersCount = MyRandom.FromSet(migrantsNumberChance);

            for (int i = 0; i < settlersCount; i++)
            {
                Settlers.Add(SettlerGenerator.GenerateSettler());
            }

            int itemsCount = MyRandom.Range(MIN_ITEMS_COUNT, MAX_ITEMS_COUNT + 1);

            for (int i = 0; i < itemsCount; i++)
            {
                Item item = GameplayScene.WorldManager.GetRandomOpenedItem();

                if (item != null)
                {
                    if (item.Value == 0)
                        continue;

                    if (Items.ContainsKey(item))
                        continue;

                    int count = 10 / item.Value;

                    Items.Add(item, MyRandom.Range(1, count + 1));
                }
            }
        }

        public NomadsManagerSaveData GetSaveData()
        {
            NomadsManagerSaveData saveData = new NomadsManagerSaveData();
            saveData.HoursSum = hoursSum;
            return saveData;
        }
        
    }
}
