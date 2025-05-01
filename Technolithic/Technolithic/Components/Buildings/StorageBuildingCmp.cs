using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public enum AllowanceMode
    {
        All,
        Partially,
        Nothing
    }

    public class StorageBuildingCmp : BuildingCmp
    {
        
        public int EmptySpaceCount
        {
            get { return CurrentCapacity - (Inventory.TotalItemsCount + Inventory.TotalItemToAddCount); }
        }

        private List<Sprite> itemsImages = new List<Sprite>();

        private Dictionary<Item, bool> filters;

        public Action<StorageBuildingCmp, Item> OnItemsFilterAllowed { get; set; }
        public Action<StorageBuildingCmp, Item> OnItemsFilterForbidden { get; set; }
        public Action<StorageBuildingCmp> OnCapacityChangedCallback { get; set; }

        public AllowanceMode AllowanceMode { get; private set; } = AllowanceMode.Nothing;

        public Storage Storage { get; private set; }

        public int Priority { get; private set; } = 0;

        public int MaxCapacity { get { return Storage.Capacity; } }

        private int currentCapacity;
        public int CurrentCapacity
        {
            get { return currentCapacity; }
            set
            {
                if (currentCapacity == value)
                    return;

                if (value < 0)
                    throw new Exception("Capacity can't be less than zero!");

                if (value > MaxCapacity)
                    throw new Exception("Capacity can't be more than max capacity!");
                
                currentCapacity = value;

                OnCapacityChangedCallback?.Invoke(this);
            }
        }

        public StorageBuildingCmp(BuildingTemplate buildingTemplate, Direction direction) 
            : base(buildingTemplate, direction)
        {
            Storage = buildingTemplate.Storage;

            if (BuildingTemplate.ShowItemOnTop)
            {
                for (int i = 0; i < 4; i++)
                    itemsImages.Add(new Sprite(RenderManager.Pixel, 16, 16) { Active = false });
            }

            currentCapacity = Storage.Capacity;
        }

        public IEnumerable<KeyValuePair<Item, bool>> GetSortedFilters()
        {
            foreach (var icKvp in ItemDatabase.ItemCategories)
            {
                int categoryId = icKvp.Key;
                foreach (var fKvp in filters)
                {
                    Item item = fKvp.Key;
                    if (item.ItemCategory == categoryId)
                    {
                        yield return fKvp;
                    }
                }
            }
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            filters = new Dictionary<Item, bool>();

            Inventory.IsStorage = true;

            GameplayScene.WorldManager.StorageManager.AddStorage(this);

            InitializeFilters();
        }

        protected override void OnItemAdded(Inventory senderInventory, Item item, int weight)
        {
            base.OnItemAdded(senderInventory, item, weight);

            if (IsBuilt)
            {
                if (GameplayScene.WorldManager.ItemsDecayer.Inventories.Contains(Inventory) == false)
                    GameplayScene.WorldManager.ItemsDecayer.Inventories.Add(Inventory);

                UpdateImages();
            }
        }

        protected override void OnItemRemoved(Inventory senderInventory, Item item, int weight)
        {
            base.OnItemRemoved(senderInventory, item, weight);

            if (IsBuilt)
            {
                if (Inventory.TotalItemsCount <= 0)
                {
                    GameplayScene.WorldManager.ItemsDecayer.Inventories.Remove(Inventory);
                }

                int factWeight = Inventory.GetInventoryFactWeight(item);

                if (factWeight == 0)
                {
                    UpdateImages();
                }
            }
        }

        private void InitializeFilters()
        {
            foreach (var kvp in ItemDatabase.Items)
            {
                Item item = kvp.Value;

                // Проверяем, может ли предмет содержаться в этом хранилище
                if (Storage.Filters.Contains(item.ItemCategory))
                {
                    if (filters.ContainsKey(item) == false)
                    {
                        switch (AllowanceMode)
                        {
                            case AllowanceMode.All:
                                filters.Add(item, true);
                                break;
                            case AllowanceMode.Partially:
                            case AllowanceMode.Nothing:
                                filters.Add(item, false);
                                break;
                        }

                        Inventory.ItemItemContainerPair.Add(item, new List<ItemContainer>());
                    }
                }
            }
        }

        public void AllowCategory(int itemCategory)
        {
            foreach(var kvp in ItemDatabase.Items)
            {
                Item item = kvp.Value;

                if(item.ItemCategory == itemCategory && filters.ContainsKey(item))
                {
                    SetItemFilter(item, true);
                }
            }
        }

        public void ClearCategory(int itemCategory)
        {
            foreach (var kvp in ItemDatabase.Items)
            {
                Item item = kvp.Value;

                if (item.ItemCategory == itemCategory && filters.ContainsKey(item))
                {
                    SetItemFilter(item, false);
                }
            }
        }

        public void SetAllowanceMode(AllowanceMode value)
        {
             if (AllowanceMode == value)
                return;

            AllowanceMode = value;

            switch(AllowanceMode)
            {
                case AllowanceMode.All:
                    {
                        for(int i = 0; i < filters.Count; i++)
                        {
                            var kvp = filters.ElementAt(i);

                            Item item = kvp.Key;

                            SetSolidsFilter(item, true);
                        }

                        OnItemsFilterAllowed?.Invoke(this, null);
                    }
                    break;
                case AllowanceMode.Nothing:
                    {
                        for (int i = 0; i < filters.Count; i++)
                        {
                            var kvp = filters.ElementAt(i);

                            Item item = kvp.Key;

                            SetSolidsFilter(item, false);
                        }

                        OnItemsFilterForbidden?.Invoke(this, null);
                    }
                    break;
            }
        }

        private void SetSolidsFilter(Item item, bool allow)
        {
            if (!filters.ContainsKey(item))
                filters.Add(item, allow);

            if (filters[item] == allow)
                return;

            filters[item] = allow;

            if (allow == false)
            {
                int factWeight = Inventory.GetInventoryFactWeight(item);
                if (factWeight == 0)
                    return;

                Tile tile = GetReachableTile();
                foreach (var itemContainer in Inventory.PopItem(item, factWeight))
                {
                    tile.Inventory.AddCargo(itemContainer);
                }
            }
        }

        public void SetItemFilter(Item item, bool allow)
        {
            AllowanceMode = AllowanceMode.Partially;

            SetSolidsFilter(item, allow);

            if(allow)
            {
                OnItemsFilterAllowed?.Invoke(this, item);
            }
            else
            {
                OnItemsFilterForbidden?.Invoke(this, item);
            }
        }

        public bool IsItemAllowed(Item item)
        {
            if (filters.ContainsKey(item) == false)
                return false;

            return filters[item];
        }

        private void UpdateImages()
        {
            if (Storage.Stages > 1)
            {
                int newStage = GetCurrentStorageStage();

                MyTexture newStageTexture = Storage.StagesTextures[newStage];
                Sprite.GetAnimation("Idle").Frames[0] = newStageTexture;
            }


            if (BuildingTemplate.ShowItemOnTop == false)
                return;

            foreach(Sprite image in itemsImages)
                image.Active = false;

            int count = 0;
            foreach (var kvp in Inventory.ItemItemContainerPair)
            {
                Item item = kvp.Key;
                int weight = Inventory.GetInventoryFactWeight(item);

                // Если предмета нету в инвентаре, то не отображаем его
                if (weight == 0)
                    continue;

                Sprite image = itemsImages[count];

                int x = count % 2;
                int y = count / 2;

                image.X = Entity.X + x * image.Width;
                image.Y = Entity.Y + y * image.Height;
                image.Active = true;
                image.Texture = item.Icon;

                count++;

                if(count == 4)
                    break;
            }
        }

        private int GetCurrentStorageStage()
        {
            if (Storage.Stages == 1)
            {
                return 0;
            }
            else if (Storage.Stages == 2)
            {
                if (Inventory.TotalItemsCount == MaxCapacity)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else if (Storage.Stages == 3)
            {
                if (Inventory.TotalItemsCount == MaxCapacity)
                {
                    return 2;
                }
                else if(Inventory.TotalItemsCount == 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }

            return 0;
        }

        public Item GetAvailableToolItem(CreatureCmp creature, ToolType toolType)
        {
            if (ItemDatabase.Tools[creature.CreatureType].ContainsKey(toolType) == false)
                return null;

            foreach(var item in ItemDatabase.Tools[creature.CreatureType][toolType])
            {
                if (Inventory.GetAvailableItemCount(item) > 0)
                    return item;
            }

            return null;
        }

        public override void Render()
        {
            base.Render();

            foreach (Sprite image in itemsImages)
                if (image.Active)
                    image.Render();
        }

        public override BuildingSaveData GetSaveData()
        {
            BuildingSaveData buildingSaveData = base.GetSaveData();

            if (IsBuilt)
            {
                buildingSaveData.StorageFilters = new Dictionary<int, bool>();
                foreach (var kvp in filters)
                {
                    buildingSaveData.StorageFilters.Add(kvp.Key.Id, kvp.Value);
                }

                buildingSaveData.AllowanceMode = AllowanceMode;
                buildingSaveData.StorageCapacity = CurrentCapacity;
                buildingSaveData.StoragePriority = Priority;
            }

            return buildingSaveData;
        }

        public override string GetInformation()
        {
            string info = base.GetInformation();

            if (IsBuilt)
            {
                info += Storage.GetInformation();

                info += $"\n{Localization.GetLocalizedText("priority")}: {Priority}";
            }

            return info;
        }

        public void SetPriority(int value)
        {
            Priority = value;

            GameplayScene.WorldManager.StorageManager.SortStorageThatHaveEmptySpaceForItemsByPriority();
        }
    }
}
