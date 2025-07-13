using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Technolithic
{
    public class WorldManager
    {
        public StorageManager StorageManager { get; private set; }
        public LaborManager LaborManager { get; private set; }

        private BuildingManager buildingManager;

        public ItemsDecayer ItemsDecayer { get; private set; }

        public MyAction CurrentAction { get; private set; }

        private Tile firstSelectedTile;

        public BuildingTemplate CurrentBuildingTemplate { get; private set; }

        // Сюда попадают предметы которые хоть раз были на тайле
        private Dictionary<Item, bool> openedItemsDictionary = new Dictionary<Item,bool>();
        private Dictionary<Item, bool> visibleItemsDictionary = new Dictionary<Item, bool>();

        public Action<Item> OnItemOpenedCallback { get; set; }
        public Action<Item, bool> ItemVisibilityChanged { get; set; }

        private Dictionary<CraftingRecipe, bool> openedCraftingRecipes = new Dictionary<CraftingRecipe, bool>();

        private NewSprite tileSelector;
        private NewSprite tileSelectorIcon;
        private Rectangle selectedRectangle = Rectangle.Empty;

        private EntitySelector entitySelector;

        private List<AttackInfo> attackInfos;

        private Direction currentBuildingDirection = Direction.DOWN;
        private char[,] buildingGroundPatternMatrix = null;
        private NewSprite buildingImage;
        private NewSprite rotateImage;
        private AnimatedSprite pickSprite;

        public List<HutBuildingCmp> HutBuildingsV2 { get; set; } = new List<HutBuildingCmp>();
        public List<BuildingCmp> FuelConsumerBuildings { get; set; } = new List<BuildingCmp>();
        public List<AnimalPenBuildingCmp> AnimalPenBuildings { get; set; } = new List<AnimalPenBuildingCmp>();

        public HashSet<AnimalCmp> AnimalsToDomesticate { get; set; } = new HashSet<AnimalCmp>();
        public HashSet<AnimalCmp> AnimalsToHunt { get; set; } = new HashSet<AnimalCmp>();

        public Dictionary<LaborType, List<CrafterBuildingCmp>> CrafterBuildings { get; set; } = new Dictionary<LaborType, List<CrafterBuildingCmp>>();

        public Dictionary<LaborType, List<CrafterBuildingCmp>> AutoCrafterBuildings { get; set; } = new Dictionary<LaborType, List<CrafterBuildingCmp>>();

        public List<CreaturePoweredEnergySource> CreaturePoweredEnergySources { get; set; } = new();

        public int TotalSettlersCount { get; private set; } = 0;

        public int WildAnimalsNumber { get; set; } = 0;
        public int DomesticatedAnimalsNumber { get; set; } = 0;


        public List<Item> SettlerBeverageRation = new List<Item>();
        public Dictionary<Item, bool> NewSettlerFoodRationFilters = new Dictionary<Item, bool>();

        
        public Dictionary<int, Dictionary<Item, List<Inventory>>> TilesThatHaveItems { get; private set; } = new Dictionary<int, Dictionary<Item, List<Inventory>>>();
        public Dictionary<Item, List<Inventory>> BuildingsThatRequireItemsV2 { get; private set; } = new Dictionary<Item, List<Inventory>>();
        public Dictionary<Item, List<Inventory>> StoragesThatHaveItemsV2 {get; private set; } = new Dictionary<Item, List<Inventory>>();
        
        private HaulLabor haulLabor = new HaulLabor();
        private SupplyFromStorageLabor supplyStorageLabor = new SupplyFromStorageLabor();
        private SupplyFromTileLabor supplyFromTileLabor = new SupplyFromTileLabor();
        private SupplyFuelLabor supplyFuelLabor = new SupplyFuelLabor();
        private CleanAnimalPenLabor cleanAnimalPenLabor = new CleanAnimalPenLabor();
        private AnimalDomesticateLabor animalDomesticateLabor = new AnimalDomesticateLabor();
        private SettlerHuntLabor settlerHuntLabor = new SettlerHuntLabor();
        private EnergyProductionLabor energyProductionLabor = new EnergyProductionLabor();

        public Action<int> CbOnSettlersCountChanged { get; set; }

        public List<Tile> HomeArea = new List<Tile>();

        public List<BuildingCmp> WallsList = new List<BuildingCmp>();

        public BuildingSaveData BuildingSaveDataForCopy { get; set; } // used to copy settings

        public Dictionary<Item, HashSet<CrafterBuildingCmp>> CraftersSortedByMainItems { get; set; } = new Dictionary<Item, HashSet<CrafterBuildingCmp>>();

        public List<Item> CraftersOpenedItems { get; set; } = new List<Item>();

        private SelectableCmp selectedSelectable;
        private Tile oldSelectedTile;

        private Queue<SelectableCmp> selectablesQueue = new Queue<SelectableCmp>();

        private InteractablesManager interactablesManager;
        private InteractionsDatabase interactionsDatabase;
        private ProgressTree progressTree;

        public WorldManager(WorldManagerSaveData saveData, InteractionsDatabase interactionsDatabase, ProgressTree progressTree)
        {
            ItemsDecayer = new ItemsDecayer();
            StorageManager = new StorageManager();
            LaborManager = new LaborManager();
            buildingManager = new BuildingManager();
            this.interactionsDatabase = interactionsDatabase;
            this.progressTree = progressTree;
            
            interactablesManager = new InteractablesManager(interactionsDatabase);
            
            foreach(LaborType laborType in interactionsDatabase.GetInvolvedTypesOfLabor())
            {
                InteractLabor interactLabor = new InteractLabor(laborType,
                    interactionsDatabase, interactablesManager);
                interactLabor.Repeat = true;
                interactLabor.IsMultiCreatureLabor = true;
                LaborManager.Add(interactLabor);
            }

            haulLabor.Repeat = true;
            haulLabor.IsMultiCreatureLabor = true;
            LaborManager.Add(haulLabor);

            supplyStorageLabor.Repeat = true;
            supplyStorageLabor.IsMultiCreatureLabor = true;
            LaborManager.Add(supplyStorageLabor);

            supplyFromTileLabor.Repeat = true;
            supplyFromTileLabor.IsMultiCreatureLabor = true;
            LaborManager.Add(supplyFromTileLabor);

            supplyFuelLabor.Repeat = true;
            supplyFuelLabor.IsMultiCreatureLabor = true;
            LaborManager.Add(supplyFuelLabor);

            cleanAnimalPenLabor.Repeat = true;
            cleanAnimalPenLabor.IsMultiCreatureLabor = true;
            LaborManager.Add(cleanAnimalPenLabor);

            animalDomesticateLabor.Repeat = true;
            animalDomesticateLabor.IsMultiCreatureLabor = true;
            LaborManager.Add(animalDomesticateLabor);

            settlerHuntLabor.Repeat = true;
            settlerHuntLabor.IsMultiCreatureLabor = true;
            LaborManager.Add(settlerHuntLabor);

            energyProductionLabor.Repeat = true;
            energyProductionLabor.IsMultiCreatureLabor= true;
            LaborManager.Add(energyProductionLabor);

            // Создание CraftLabor на основе LaborType крафтеров
            foreach (var kvp in Engine.Instance.Buildings)
            {
                if (kvp.Value.BuildingType == BuildingType.Crafter)
                {
                    Crafter crafter = kvp.Value.Crafter;

                    if (CrafterBuildings.ContainsKey(crafter.LaborType) == false)
                    {
                        CrafterBuildings.Add(crafter.LaborType, new List<CrafterBuildingCmp>());

                        CraftLabor craftLabor = new CraftLabor(crafter.LaborType);
                        craftLabor.Repeat = true;
                        craftLabor.IsMultiCreatureLabor = true;
                        LaborManager.Add(craftLabor);
                    }

                    if (AutoCrafterBuildings.ContainsKey(crafter.LaborType) == false)
                    {
                        AutoCrafterBuildings.Add(crafter.LaborType, new List<CrafterBuildingCmp>());

                        PrepareCrafterLabor prepareCrafterLabor = new PrepareCrafterLabor(crafter.LaborType);
                        prepareCrafterLabor.Repeat = true;
                        prepareCrafterLabor.IsMultiCreatureLabor = true;
                        LaborManager.Add(prepareCrafterLabor);
                    }
                }
            }

            tileSelector = new NewSprite();
            tileSelector.Texture = TextureBank.UITexture.GetSubtexture(176, 0, 16, 16);
            tileSelectorIcon = new NewSprite();

            entitySelector = new EntitySelector();

            attackInfos = new List<AttackInfo>();

            foreach(var kvp in ItemDatabase.Items)
            {
                openedItemsDictionary.Add(kvp.Value, false);
                visibleItemsDictionary.Add(kvp.Value, true);
            }

            foreach(var food in Engine.Instance.SettlerRation)
            {
                NewSettlerFoodRationFilters.Add(food, true);
            }

            if (saveData != null)
            {
                foreach (var itemId in saveData.OpenedItems)
                {
                    Item item = ItemDatabase.GetItemById(itemId);

                    if (item == null)
                        continue;

                    openedItemsDictionary[item] = true;
                }

                if (saveData.HiddenItems != null)
                {
                    foreach (var itemId in saveData.HiddenItems)
                    {
                        Item item = ItemDatabase.GetItemById(itemId);

                        if (item == null)
                            continue;

                        visibleItemsDictionary[item] = false;
                    }
                }

                if (saveData.NewSettlersFoodRationFilters != null)
                {
                    foreach (var kvp in saveData.NewSettlersFoodRationFilters)
                    {
                        Item item = ItemDatabase.GetItemById(kvp.Key);

                        if (item == null)
                            continue;

                        if (NewSettlerFoodRationFilters.ContainsKey(item) == false)
                            continue;

                        NewSettlerFoodRationFilters[item] = kvp.Value;
                    }
                }
            }

            SettlerBeverageRation.Add(ItemDatabase.GetItemByName("pot_of_wine"));
            SettlerBeverageRation.Add(ItemDatabase.GetItemByName("pot_of_beer"));
            SettlerBeverageRation.Add(ItemDatabase.GetItemByName("pot_of_mead"));

            TilesThatHaveItems.Add(0, new Dictionary<Item, List<Inventory>>());

            foreach(var kvp in ItemDatabase.Items)
            {
                CraftersSortedByMainItems.Add(kvp.Value, new HashSet<CrafterBuildingCmp>());
            }
        }

        public void Begin()
        {
            buildingImage = new NewSprite();
            buildingImage.IsCentered = true;
            buildingImage.Color = Color.White * 0.5f;

            rotateImage = new NewSprite();
            rotateImage.Texture = ResourceManager.RotateSprite;

            pickSprite = new AnimatedSprite(16, 16);
            pickSprite.Add("idle", new Animation(ResourceManager.PickAnimation, 8, 0, 16, 16, 0, 0));
            pickSprite.Active = false;
        }

        public void AddCreature(CreatureCmp creatureCmp)
        {
            creatureCmp.OnCreatureDieCallback += RemoveCreature;

            if (creatureCmp.CreatureType == CreatureType.Settler)
            {
                TotalSettlersCount++;
                CbOnSettlersCountChanged?.Invoke(TotalSettlersCount);
            }
        }

        private void RemoveCreature(CreatureCmp creatureCmp)
        {
            creatureCmp.OnCreatureDieCallback -= RemoveCreature;

            if (creatureCmp.CreatureType == CreatureType.Settler)
            {
                TotalSettlersCount--;
                CbOnSettlersCountChanged?.Invoke(TotalSettlersCount);
            }
        }

        public bool IsItemOpened(Item item)
        {
            if (item == null)
            {
                return false;
            }

            return openedItemsDictionary[item];
        }

        public bool IsItemVisible(Item item)
        {
            if(item == null)
            {
                return false;
            }

            return visibleItemsDictionary[item];
        }

        public Item GetRandomOpenedItem()
        {
            int openedItemsAmount = openedItemsDictionary.Count(x => x.Value);

            if (openedItemsAmount == 0)
            {
                return null;
            }

            return openedItemsDictionary.Where(x => x.Value)
                .ElementAt(MyRandom.Range(0, openedItemsAmount)).Key;
        }

        public void ShowItem(Item item)
        {
            if (visibleItemsDictionary[item])
                return;

            visibleItemsDictionary[item] = true;

            ItemVisibilityChanged?.Invoke(item, true);
        }

        public void HideItem(Item item)
        {
            if (visibleItemsDictionary[item] == false)
                return;

            visibleItemsDictionary[item] = false;

            ItemVisibilityChanged?.Invoke(item, false);
        }

        public void OpenItem(Item item)
        {
            // Пропускаем если предмет уже был открыт
            if (openedItemsDictionary[item])
                return;

            openedItemsDictionary[item] = true;

            OnItemOpenedCallback?.Invoke(item);
        }

        public (Inventory, Item) FindTool(CreatureCmp creature, InteractionType interactionType)
        {
            int zoneId = creature.Movement.CurrentTile.Room.Id;
            StorageBuildingCmp storage = StorageManager.GetStorageWithTool(creature, interactionType);
            if (storage != null)
            {
                Item item = storage.GetAvailableTool(creature, interactionType);
                return (storage.Inventory, item);
            }

            if (TilesThatHaveItems[zoneId].Count != 0)
            {
                foreach (var item in ItemDatabase.GetInteractionTypeTools(creature.CreatureType, interactionType))
                {
                    if (TilesThatHaveItems[zoneId].ContainsKey(item) == false) continue;

                    if (TilesThatHaveItems[zoneId][item].Count == 0) continue;

                    Inventory inventory = TilesThatHaveItems[zoneId][item][0];

                    return (inventory, item);
                }
            }

            return (null, null);
        }

        public Tuple<Inventory, Item> FindTool(CreatureCmp creature, ToolType toolType)
        {
            int creatureRoomId = creature.Movement.CurrentTile.Room.Id;

            // Сначала ищем на складе
            StorageBuildingCmp storage = StorageManager.GetStorageWithToolItem(creature, toolType);
            if (storage != null)
            {
                Item item = storage.GetAvailableToolItem(creature, toolType);
                return Tuple.Create(storage.Inventory, item);
            }

            if (TilesThatHaveItems[creatureRoomId].Count != 0)
            {
                // Теперь ищем на тайлах
                foreach (var item in ItemDatabase.Tools[creature.CreatureType][toolType])
                {
                    if (!TilesThatHaveItems[creatureRoomId].ContainsKey(item))
                        continue;

                    if (TilesThatHaveItems[creatureRoomId][item].Count == 0)
                        continue;

                    Inventory inventory = TilesThatHaveItems[creatureRoomId][item][0];

                    return Tuple.Create(inventory, item);
                }
            }

            return null;
        }

        public bool IsCraftingRecipeOpened(CraftingRecipe craftingRecipe)
        {
            if (openedCraftingRecipes.ContainsKey(craftingRecipe) == false)
                openedCraftingRecipes.Add(craftingRecipe, false);

            if (openedCraftingRecipes[craftingRecipe])
                return true;
            else
            {
                if (progressTree.UnlockedItems[craftingRecipe.MainItem] == false)
                    return false;

                openedCraftingRecipes[craftingRecipe] = true;

                return true;
            }
        }

        public void GenerateWorld()
        {
            World world = GameplayScene.Instance.World;

            // Генерация животных
            for (int i = 0; i < 20; i++)
            {
                Tile randomTile = world.GetTileAt(MyRandom.Range(GameplayScene.WorldSize), MyRandom.Range(GameplayScene.WorldSize));
                if (randomTile.GroundTopType != GroundTopType.Water && randomTile.GroundTopType != GroundTopType.DeepWater)
                {
                    AnimalTemplate animalTemplate = AnimalTemplateDatabase.GetRandomWildAnimalTemplate();

                    int randomDaysUntilAging = MyRandom.Range(1, animalTemplate.DaysUntilAging);

                    GameplayScene.Instance.SpawnAnimal(randomTile.X, randomTile.Y, animalTemplate, randomDaysUntilAging);
                }
            }

            // Генерация дикого улья (10 попыток заспавнить улей)
            List<BeeHiveBuildingCmp> wildBeehives = new List<BeeHiveBuildingCmp>();

            int maxBeeHivesCount = MyRandom.Range(1, 3);
            for (int j = 0; j < maxBeeHivesCount; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    int randomX = MyRandom.Range(GameplayScene.WorldSize);
                    int randomY = MyRandom.Range(GameplayScene.WorldSize);
                    Entity entity = TryToBuild(Engine.Instance.Buildings["wild_bee_hive"], randomX, randomY, Direction.DOWN, true);
                    if (entity != null)
                    {
                        BeeHiveBuildingCmp beehive = entity.Get<BeeHiveBuildingCmp>();
                        // После генерации дикий улей будет иметь рандомный прогресс генерации меда
                        // TODO: Bug: при создании мира дикие улья всегда будут иметь прогресс от 0.0 до 1.0 так как
                        // строение ожидает число от 0 до 100. Пока временно умножим число на 100. Нужно будет исправить в будущем
                        beehive.SetProgress(MyRandom.NextFloat() * 100); // TODO: temp
                        wildBeehives.Add(beehive);
                        break;
                    }
                }
            }

            // Генерация одуванчиков вокруг ульев
            foreach(var wildBeeHive in wildBeehives)
            {
                GenerateRandomlyFlowers(wildBeeHive.RangeTiles.ToList());
            }
        }

        private void GenerateRandomlyFlowers(List<Tile> tiles)
        {
            int flowersCount = MyRandom.Range(1, 5);

            if(tiles.Count < flowersCount)
            {
                flowersCount = tiles.Count;
            }

            List<BuildingTemplate> flowersPlantTemplates = Engine.Instance.Buildings
                .Where(x => x.Value.PlantData != null && x.Value.PlantData.IsFlower)
                .Select(x => x.Value)
                .ToList();

            for (int i = 0; i < flowersCount; i++)
            {
                Tile randomTile = tiles[MyRandom.Range(tiles.Count)];
                tiles.Remove(randomTile);

                BuildingTemplate plantTemplate = flowersPlantTemplates[MyRandom.Range(flowersPlantTemplates.Count)];

                Entity entity = TryToBuild(plantTemplate, randomTile.X, randomTile.Y, Direction.DOWN, true);
                if (entity != null)
                {
                    FarmPlot wildFarmPlot = entity.Get<FarmPlot>();
                    wildFarmPlot.SetPlantParameters(MyRandom.Range(3) == 2 ? 100 : MyRandom.Range(75, 100), 0);
                    wildFarmPlot.MakeWild();
                }
            }
        }

        public void Update(Season currentSeason)
        {
            ItemsDecayer.Update(currentSeason);
            LaborManager.Update();

            for (int i = 0; i < attackInfos.Count; i++)
            {
                AttackInfo attackInfo = attackInfos[i];
                if (attackInfo.IsCompleted)
                {
                    attackInfos.Remove(attackInfo);
                    i--;
                }
                else
                    attackInfos[i].Update();
            }

            if (MInput.Mouse.PressedRightButton)
            {
                SetMyAction(MyAction.None, null);
            }

            if (!GameplayScene.MouseOnUI)
            {
                tileSelector.Position = GameplayScene.MouseTilePosition;
                tileSelectorIcon.Position = GameplayScene.MouseTilePosition;

                switch (CurrentAction)
                {
                    case MyAction.Build:
                        DoBuildAction();
                        break;
                    default:
                        DoLaborAction(CurrentAction);
                        break;
                }
            }

            if (pickSprite.Active)
            {
                pickSprite.Update();
                if (pickSprite.CurrentAnimation.IsLastFrame)
                {
                    pickSprite.Active = false;
                }
            }
        }

        public void CbOnTileWalkableChanged(Tile tile)
        {
            // Перестраиваем путь всем существам
            foreach (var entity in GameplayScene.Instance.CreatureLayer.Entities)
            {
                CreatureCmp creatureCmp = entity.Get<CreatureCmp>();
                if(creatureCmp != null && creatureCmp.IsDead == false)
                {
                    creatureCmp.Movement.RebuildPath(tile);
                }
            }

            // Добавление новых id в коллекции тайлов, строений и складов
            List<int> roomsIds = GameplayScene.Instance.World.GetRoomsIds();
            roomsIds.Add(-1);
            for (int i = 0; i < roomsIds.Count; i++)
            {
                int roomId = roomsIds[i];

                if (TilesThatHaveItems.ContainsKey(roomId) == false)
                    TilesThatHaveItems.Add(roomId, new Dictionary<Item, List<Inventory>>());

                if (StorageManager.StorageThatHaveEmptySpaceForItems.ContainsKey(roomId) == false)
                    StorageManager.StorageThatHaveEmptySpaceForItems.Add(roomId, new Dictionary<Item, List<Inventory>>());
            }

            // Попытка распихнуть все строения id которых уже не существуют в новые коллекции
            List<int> roomIdsToRemove = new List<int>();

            ReallocateInventories(roomsIds, roomIdsToRemove, TilesThatHaveItems);
            ReallocateInventories(roomsIds, roomIdsToRemove, StorageManager.StorageThatHaveEmptySpaceForItems);
            
            // Удаление коллекций id которых не существует
            for (int i = 0; i < roomIdsToRemove.Count; i++)
            {
                int roomId = roomIdsToRemove[i];

                TilesThatHaveItems.Remove(roomId);

                StorageManager.StorageThatHaveEmptySpaceForItems.Remove(roomId);
            }

            if(tile.Inventory.TotalItemsCount > 0)
            {
                if(tile.IsWalkable == false)
                {
                    Tile freeTile = tile.GetAllNeighbourTiles().Where(x => x.IsWalkable).FirstOrDefault();

                    if(freeTile != null)
                    {
                        tile.Inventory.ThrowCargo(freeTile);
                    }
                }
            }
        }

        private void ReallocateInventories(List<int> roomsIds, List<int> roomIdsToRemove, 
            Dictionary<int, Dictionary<Item, List<Inventory>>> collection)
        {
            foreach (var kvp in collection)
            {
                int roomId = kvp.Key;
                Dictionary<Item, List<Inventory>> itemStoragePair = kvp.Value;

                if (roomsIds.Contains(roomId) == false)
                {
                    if(roomIdsToRemove.Contains(roomId) == false)
                        roomIdsToRemove.Add(roomId);

                    foreach (var kvp2 in itemStoragePair)
                    {
                        foreach (var inventory in kvp2.Value)
                        {
                            int inventoryRoomId = inventory.GetRoom() == null ? -1 : inventory.GetRoom().Id;
                            
                            if (collection[inventoryRoomId].ContainsKey(kvp2.Key) == false)
                                collection[inventoryRoomId].Add(kvp2.Key, new List<Inventory>());

                            collection[inventoryRoomId][kvp2.Key].Add(inventory);
                        }
                    }
                }
            }

        }

        public void AddAttackInfo(AttackInfo attackInfo)
        {
            attackInfos.Add(attackInfo);
        }

        public Tile GetBoundedBuildingTemplateTile()
        {
            int x = GameplayScene.MouseTile.X;
            int y = GameplayScene.MouseTile.Y;

            int worldWidth = GameplayScene.Instance.World.Width;
            int worldHeight = GameplayScene.Instance.World.Height;

            int buildingWidth = buildingGroundPatternMatrix.GetLength(0);
            int buildingHeight = buildingGroundPatternMatrix.GetLength(1);

            if (x + buildingWidth >= worldWidth)
            {
                x = worldWidth - buildingWidth;
            }

            if (y + buildingHeight >= worldHeight)
            {
                y = worldHeight - buildingHeight;
            }

            return GameplayScene.Instance.World.GetTileAt(x, y);
        }

        public void Render()
        {
            for(int i = 0; i < attackInfos.Count; i++)
            {
                attackInfos[i].Render();
            }

            switch (CurrentAction)
            {
                case MyAction.Build:
                    if (CurrentBuildingTemplate != null)
                    {
                        if(CurrentBuildingTemplate.Rotatable)
                        {
                            rotateImage.Position = GameplayScene.MouseWorldPosition - (Vector2.One * Engine.TILE_SIZE * 2);
                            rotateImage.Render();
                        }

                        int imageYOffset = buildingGroundPatternMatrix.GetLength(1) * Engine.TILE_SIZE - buildingImage.Texture.Height;
                        int imageXOffset = (buildingGroundPatternMatrix.GetLength(0) * Engine.TILE_SIZE - buildingImage.Texture.Width) / 2;

                        Tile buildingTemplateTile = GetBoundedBuildingTemplateTile();

                        buildingImage.Position = new Vector2((buildingTemplateTile.X * Engine.TILE_SIZE) + imageXOffset + buildingImage.Texture.Width / 2,
                            (buildingTemplateTile.Y * Engine.TILE_SIZE) + imageYOffset + buildingImage.Texture.Height / 2);

                        foreach (var checkTile in GetTilesCoveredByBuildingTemplate(buildingTemplateTile))
                        {
                            Color color = Engine.YELLOW_GREEN;
                            if (buildingManager.CheckTileByGroundPattern(CurrentBuildingTemplate, buildingGroundPatternMatrix,
                                buildingTemplateTile, checkTile) == false)
                            {
                                color = Color.Red;
                            }

                            RenderManager.HollowRect(checkTile.X * Engine.TILE_SIZE, checkTile.Y * Engine.TILE_SIZE,
                                    Engine.TILE_SIZE, Engine.TILE_SIZE, color);
                        }

                        buildingImage.Render();
                    }
                    break;
                case MyAction.None:
                    {
                        if(selectedRectangle != Rectangle.Empty)
                        {
                            RenderManager.BorderRect(
                                new Rectangle(selectedRectangle.X + 1, selectedRectangle.Y + 1, selectedRectangle.Width, selectedRectangle.Height), 
                                Color.Black);
                            RenderManager.BorderRect(selectedRectangle, Color.GreenYellow);
                        }
                        else
                        {
                            tileSelector.Render();
                        }

                        if (selectedSelectable != null)
                        {
                            entitySelector.Render(selectedSelectable.BoundingBox, Color.White);
                        }
                    }
                    break;
                default:
                    {
                        tileSelectorIcon.Render();

                        if (selectedRectangle != Rectangle.Empty)
                        {
                            RenderManager.HollowRect(selectedRectangle, Color.White);
                        }
                    }
                    break;

            }

            if (pickSprite.Active)
            {
                pickSprite.Render();
            }
        }

        public IEnumerable<Tile> GetTilesCoveredByBuildingTemplate(Tile buildingTemplateTile)
        {
            int x = buildingTemplateTile.X;
            int y = buildingTemplateTile.Y;

            int columns = buildingGroundPatternMatrix.GetLength(0);
            int rows = buildingGroundPatternMatrix.GetLength(1);

            for (int i = x; i < x + columns; i++)
            {
                for (int j = y; j < y + rows; j++)
                {
                    yield return GameplayScene.Instance.World.GetTileAt(i, j);
                }
            }
        }

        public void SetMyAction(MyAction myAction, MyTexture icon)
        {
            if (CurrentAction == myAction)
                return;

            CurrentAction = myAction;

            UnselectAllSettlers();
            selectedSelectable = null;
            selectablesQueue.Clear();
            firstSelectedTile = null;
            GameplayScene.UIRootNodeScript.CloseUnitCommandUI();

            switch (myAction)
            {
                case MyAction.Build:
                    tileSelector.Color = Color.White * 0f;

                    GameplayScene.UIRootNodeScript.CloseActionPanel();
                    break;
                case MyAction.None:
                    tileSelector.Color = Color.White;

                    GameplayScene.UIRootNodeScript.CloseActionPanel();
                    GameplayScene.UIRootNodeScript.CloseEntityPanel();
                    BuildingSaveDataForCopy = null;
                    break;
                default:
                    GameplayScene.UIRootNodeScript.CloseEntityPanel();

                    tileSelector.Color = Color.White * 0f;

                    tileSelectorIcon.Color = Color.White;
                    tileSelectorIcon.Texture = icon;
                    break;

            }
        }

        public void SetBuilding(BuildingTemplate buildingTemplate)
        {
            currentBuildingDirection = Direction.DOWN;

            CurrentBuildingTemplate = buildingTemplate;

            if (CurrentBuildingTemplate != null)
            {
                buildingImage.Texture = CurrentBuildingTemplate.Icons[Direction.DOWN];

                buildingGroundPatternMatrix = CurrentBuildingTemplate.GroundPattern;
            }
        }

        private SelectableCmp GetNextSelectable(Tile selectedTile)
        {
            // Если был выделен другой тайл, который не был до этого, то мы очищаем список Selectable
            if(selectedTile != oldSelectedTile)
            {
                selectablesQueue.Clear();

                oldSelectedTile = selectedTile;
            }

            if(selectablesQueue.Count == 0)
            {
                TryToFillSelectablesQueue(selectedTile, selectablesQueue);
            }

            if(selectablesQueue.Count > 0)
            {
                return selectablesQueue.Dequeue();
            }

            return null;
        }

        private void OpenUIForSelectable(SelectableCmp selectable)
        {
            if (selectable == null)
                return;

            switch (selectable.SelectableType)
            {
                case SelectableType.Animal:
                case SelectableType.Settler:
                case SelectableType.Trader:
                    GameplayScene.UIRootNodeScript.OpenCreatureUI(selectable.Entity.Get<CreatureCmp>());
                    break;
                case SelectableType.Building:
                    GameplayScene.UIRootNodeScript.OpenBuildingUI(selectable.Entity.Get<BuildingCmp>(),
                        interactionsDatabase, progressTree);
                    break;
                case SelectableType.ItemContainers:
                    GameplayScene.UIRootNodeScript.OpenItemStackUI(GameplayScene.MouseTile);
                    break;
            }
        }

        private void UnselectAllSettlers()
        {
            foreach (var settler in GetSelectedSettlers())
            {
                settler.IsSelected = false;
            }
        }

        private bool AnySelectedSettler()
        {
            return GetSelectedSettlers().Count() > 0;
        }

        private IEnumerable<SettlerCmp> GetSelectedSettlers()
        {
            foreach(var settler in GameplayScene.Instance.CreatureLayer.Entities
                .Where(x => x.Has<SettlerCmp>())
                .Select(x => x.Get<SettlerCmp>()))
            {
                if(settler.IsSelected)
                {
                    yield return settler;
                }
            }
        }

        private void ShowPickSprite(Tile tile, Color color)
        {
            pickSprite.RenderPosition = tile.GetAsVector() * Engine.TILE_SIZE;
            pickSprite.Active = true;
            pickSprite.Reset();
            pickSprite.Play("Idle");
            pickSprite.Color = color;
        }

        public void DisbandSelectedSettlers()
        {
            foreach (var settler in GetSelectedSettlers())
            {
                settler.Disband();
            }
        }

        public void MoveSelectedSettlersToTile(Tile targetTile)
        {
            if (targetTile == null)
                return;

            if (targetTile.IsWalkable == false)
                return;

            ShowPickSprite(targetTile, Color.GreenYellow);

            foreach (var settler in GetSelectedSettlers())
            {
                foreach (var checkTile in GetUnoccupiedTilesNearOf(targetTile))
                {
                    // Тайл находится вне досягаемости поселенца
                    if (settler.Movement.CurrentTile.GetRoomId() != checkTile.GetRoomId())
                        continue;

                    // Поселенец перестает занимать тайл, который занимал
                    if (settler.OccupiedTile != null)
                    {
                        settler.OccupiedTile.OccupiedBy = null;
                        settler.OccupiedTile = null;
                    }

                    checkTile.OccupiedBy = settler;
                    settler.OccupiedTile = checkTile;

                    settler.SetLabor(new MoveToLabor(checkTile));

                    break;
                }
            }
        }

        public void AttackCreatureWithSelectedSettlers(CreatureCmp targetCreature)
        {
            if (targetCreature == null)
                return;

            if (targetCreature.Movement == null)
                return;

            ShowPickSprite(targetCreature.Movement.CurrentTile, Color.OrangeRed);

            foreach (var settler in GetSelectedSettlers())
            {
                if (settler.OccupiedTile != null)
                {
                    settler.OccupiedTile.OccupiedBy = null;
                    settler.OccupiedTile = null;
                }

                settler.SetLabor(new AttackLabor(targetCreature));
            }
        }

        private IEnumerable<Tile> GetUnoccupiedTilesNearOf(Tile baseTile)
        {
            HashSet<Tile> tilesToCheck = new HashSet<Tile>();
            HashSet<Tile> checkedTiles = new HashSet<Tile>();
            HashSet<Tile> tilesToRemove = new HashSet<Tile>();

            tilesToCheck.Add(baseTile);

            while (true)
            {
                tilesToRemove.Clear();

                if (tilesToCheck.Count == 0)
                    yield break;

                foreach (Tile tile in tilesToCheck)
                {
                    if (tile.IsWalkable && tile.OccupiedBy == null)
                    {
                        yield return tile;
                    }

                    tilesToRemove.Add(tile);
                }

                foreach (Tile tile in tilesToRemove)
                {
                    checkedTiles.Add(tile);
                    tilesToCheck.Remove(tile);
                }

                foreach (Tile tile in tilesToRemove)
                {
                    if (tile.TopTile?.Room != null && tilesToCheck.Contains(tile.TopTile) == false
                        && checkedTiles.Contains(tile.TopTile) == false)
                    {
                        tilesToCheck.Add(tile.TopTile);
                    }

                    if (tile.LeftTile?.Room != null && tilesToCheck.Contains(tile.LeftTile) == false
                       && checkedTiles.Contains(tile.LeftTile) == false)
                    {
                        tilesToCheck.Add(tile.LeftTile);
                    }

                    if (tile.BottomTile?.Room != null && tilesToCheck.Contains(tile.BottomTile) == false
                        && checkedTiles.Contains(tile.BottomTile) == false)
                    {
                        tilesToCheck.Add(tile.BottomTile);
                    }

                    if (tile.RightTile?.Room != null && tilesToCheck.Contains(tile.RightTile) == false
                        && checkedTiles.Contains(tile.RightTile) == false)
                    {
                        tilesToCheck.Add(tile.RightTile);
                    }
                }
            }
        }

        private void DoLaborAction(MyAction action)
        {
            if (action == MyAction.None)
            {
                if (MInput.Mouse.PressedRightButton && AnySelectedSettler())
                {
                    GameplayScene.UIRootNodeScript.OpenUnitCommandUI(MInput.Mouse.X, MInput.Mouse.Y, GameplayScene.MouseTile);
                }
            }

            if (MInput.Mouse.PressedLeftButton)
            {
                if (GameplayScene.UIRootNodeScript.IsUnitCommandUIOpened())
                {
                    GameplayScene.UIRootNodeScript.CloseUnitCommandUI();
                    firstSelectedTile = null;
                    return;
                }

                firstSelectedTile = GameplayScene.MouseTile;
            }

            if (firstSelectedTile != null && MInput.Mouse.CheckLeftButton)
            {
                SelectRect(firstSelectedTile, GameplayScene.MouseTile);
            }

            if (firstSelectedTile != null && MInput.Mouse.ReleasedLeftButton)
            {
                selectedRectangle = Rectangle.Empty;

                Tile[,] tiles = SelectTiles(firstSelectedTile, GameplayScene.MouseTile);

                // TODO: temp (Need to implement a GameAction system like in Paleon Reinvented)
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    for (int y = 0; y < tiles.GetLength(1); y++)
                    {
                        Tile tile = tiles[x, y];

                        if (tile.Entity != null)
                        {
                            Interactable interactable = tile.Entity.Get<Interactable>();
                            if (interactable != null)
                            {
                                switch(action)
                                {
                                    case MyAction.Chop:
                                        interactable.MarkInteraction(InteractionType.Chop);
                                        break;
                                    case MyAction.Mine:
                                        interactable.MarkInteraction(InteractionType.Mine);
                                        break;
                                    case MyAction.GatherStone:
                                        interactable.MarkInteraction(InteractionType.GatherStone);
                                        break;
                                    case MyAction.GatherWood:
                                        interactable.MarkInteraction(InteractionType.GatherWood);
                                        break;
                                    case MyAction.Destruct:
                                        interactable.MarkInteraction(InteractionType.Destruct);
                                        break;
                                    case MyAction.Cancel:
                                        {
                                            interactable.UnmarkInteraction(InteractionType.Chop);
                                            interactable.UnmarkInteraction(InteractionType.Mine);
                                            interactable.UnmarkInteraction(InteractionType.GatherStone);
                                            interactable.UnmarkInteraction(InteractionType.GatherWood);
                                            interactable.UnmarkInteraction(InteractionType.Destruct);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }

                switch (action)
                {
                    case MyAction.None:
                        {
                            GameplayScene.UIRootNodeScript.CloseEntityPanel();
                            selectedSelectable = null;

                            if (IsOnlyOneTileWasSelected(tiles))
                            {
                                UnselectAllSettlers();

                                Tile lastSelectedTile = tiles[0, 0];

                                selectedSelectable = GetNextSelectable(lastSelectedTile);

                                OpenUIForSelectable(selectedSelectable);

                                if (selectedSelectable != null)
                                {
                                    if (selectedSelectable.SelectableType == SelectableType.Settler)
                                    {
                                        Entity settlerEntity = selectedSelectable.Entity;
                                        settlerEntity.Get<SettlerCmp>().IsSelected = true;
                                    }
                                }
                            }
                            else
                            {
                                HashSet<Tile> selectedTilesSet = new HashSet<Tile>();

                                for (int x = 0; x < tiles.GetLength(0); x++)
                                {
                                    for (int y = 0; y < tiles.GetLength(1); y++)
                                    {
                                        selectedTilesSet.Add(tiles[x, y]);
                                    }
                                }

                                if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftShift) == false)
                                {
                                    UnselectAllSettlers();
                                }

                                foreach (var settler in GameplayScene.Instance.CreatureLayer.Entities
                                    .Where(x => x.Has<SettlerCmp>())
                                    .Select(x => x.Get<SettlerCmp>()))
                                {
                                    if (settler.IsDead)
                                        continue;

                                    if (settler.IsHidden)
                                        continue;

                                    if (selectedTilesSet.Contains(settler.Movement.CurrentTile))
                                    {
                                        settler.IsSelected = true;
                                    }
                                }
                            }
                        }
                        break;
                    case MyAction.BuildIrrigationCanal:
                        {
                            for (int x = 0; x < tiles.GetLength(0); x++)
                            {
                                for (int y = 0; y < tiles.GetLength(1); y++)
                                {
                                    Tile tile = tiles[x, y];

                                    TryToBuild(Engine.Instance.Buildings["build_irrigation_canal"], tile.X, tile.Y, Direction.DOWN);
                                }
                            }
                        }
                        break;
                    case MyAction.DestructIrrigationCanal:
                        {
                            for (int x = 0; x < tiles.GetLength(0); x++)
                            {
                                for (int y = 0; y < tiles.GetLength(1); y++)
                                {
                                    Tile tile = tiles[x, y];

                                    TryToBuild(Engine.Instance.Buildings["destruct_irrigation_canal"], tile.X, tile.Y, Direction.DOWN);
                                }
                            }
                        }
                        break;
                    case MyAction.Slaughter:
                        {
                            List<Tile> selectedTiles = new List<Tile>();

                            for (int x = 0; x < tiles.GetLength(0); x++)
                            {
                                for (int y = 0; y < tiles.GetLength(1); y++)
                                {
                                    Tile tile = tiles[x, y];

                                    selectedTiles.Add(tile);
                                }
                            }

                            foreach (var creature in GameplayScene.Instance.CreatureLayer.Entities)
                            {
                                AnimalCmp animal = creature.Get<AnimalCmp>();
                                if (animal != null && animal.IsDomesticated)
                                {
                                    Tile animalTile = animal.Movement.CurrentTile;
                                    if(selectedTiles.Contains(animalTile))
                                    {
                                        animal.Slaughter = true;
                                    }
                                }
                            }
                        }
                        break;
                    case MyAction.Hunt:
                        {
                            List<Tile> selectedTiles = new List<Tile>();

                            for (int x = 0; x < tiles.GetLength(0); x++)
                            {
                                for (int y = 0; y < tiles.GetLength(1); y++)
                                {
                                    Tile tile = tiles[x, y];

                                    selectedTiles.Add(tile);
                                }
                            }

                            foreach (var creature in GameplayScene.Instance.CreatureLayer.Entities)
                            {
                                AnimalCmp animal = creature.Get<AnimalCmp>();
                                if (animal != null && animal.IsDomesticated == false)
                                {
                                    Tile animalTile = animal.Movement.CurrentTile;
                                    if (selectedTiles.Contains(animalTile))
                                    {
                                        animal.Hunt = true;
                                    }
                                }
                            }
                        }
                        break;
                    case MyAction.CutCompletely:
                        for (int x = 0; x < tiles.GetLength(0); x++)
                        {
                            for (int y = 0; y < tiles.GetLength(1); y++)
                            {
                                Tile tile = tiles[x, y];

                                if (tile.Entity != null)
                                {
                                    FarmPlot wildFarmPlot = tile.Entity.Get<FarmPlot>();
                                    if (wildFarmPlot != null && wildFarmPlot.IsBuilt && wildFarmPlot.PlantData.ToolType == ToolType.Harvesting)
                                    {
                                        wildFarmPlot.Chop = true;
                                    }
                                }
                            }
                        }
                        break;
                    case MyAction.Cut:
                        for (int x = 0; x < tiles.GetLength(0); x++)
                        {
                            for (int y = 0; y < tiles.GetLength(1); y++)
                            {
                                Tile tile = tiles[x, y];

                                if (tile.Entity != null)
                                {
                                    FarmPlot wildFarmPlot = tile.Entity.Get<FarmPlot>();
                                    if (wildFarmPlot != null && wildFarmPlot.IsBuilt && wildFarmPlot.PlantData.ToolType == ToolType.Harvesting)
                                    {
                                        wildFarmPlot.Harvest = true;
                                    }
                                }
                            }
                        }
                        break;
                    case MyAction.CopySettings:
                        BuildingTemplate copiedBuildingTemplate = Engine.Instance.Buildings[BuildingSaveDataForCopy.BuildingTemplateName];

                        // Коллекция во избежание повторного вставления копированных данных
                        List<BuildingCmp> wasPastedBuildings = new List<BuildingCmp>();

                        for (int x = 0; x < tiles.GetLength(0); x++)
                        {
                            for (int y = 0; y < tiles.GetLength(1); y++)
                            {
                                Tile tile = tiles[x, y];

                                if(tile.Entity != null)
                                {
                                    switch(copiedBuildingTemplate.BuildingType)
                                    {
                                        case BuildingType.FarmPlot:
                                            {
                                                FarmPlot farmPlot = tile.Entity.Get<FarmPlot>();
                                                if (farmPlot != null && farmPlot.IsWild == false && farmPlot.IsBuilt 
                                                    && wasPastedBuildings.Contains(farmPlot) == false)
                                                {
                                                    wasPastedBuildings.Add(farmPlot);

                                                    GameplayScene.Instance.MessageManager.ShowMessage($"+", farmPlot.Entity.Position);

                                                    farmPlot.Irrigate = BuildingSaveDataForCopy.Irrigate;
                                                    farmPlot.Fertilize = BuildingSaveDataForCopy.Fertilize;
                                                }
                                            }
                                            break;
                                        case BuildingType.AnimalPen:
                                            {
                                                AnimalPenBuildingCmp animalPenBuilding = tile.Entity.Get<AnimalPenBuildingCmp>();
                                                if(animalPenBuilding != null && animalPenBuilding.IsBuilt
                                                    && wasPastedBuildings.Contains(animalPenBuilding) == false
                                                    && animalPenBuilding.BuildingTemplate == copiedBuildingTemplate)
                                                {
                                                    wasPastedBuildings.Add(animalPenBuilding);

                                                    GameplayScene.Instance.MessageManager.ShowMessage($"+", animalPenBuilding.Entity.Position);

                                                    foreach(var kvp in BuildingSaveDataForCopy.AnimalsFilters)
                                                    {
                                                        animalPenBuilding.SetAnimalFilter(
                                                            AnimalTemplateDatabase.GetAnimalTemplateByName(kvp.Key),
                                                            kvp.Value);
                                                    }
                                                }
                                            }
                                            break;
                                        case BuildingType.Stockpile:
                                            {
                                                StorageBuildingCmp storage = tile.Entity.Get<StorageBuildingCmp>();
                                                if(storage != null && storage.IsBuilt 
                                                    && wasPastedBuildings.Contains(storage) == false
                                                    && storage.BuildingTemplate == copiedBuildingTemplate)
                                                {
                                                    wasPastedBuildings.Add(storage);

                                                    GameplayScene.Instance.MessageManager.ShowMessage($"+", storage.Entity.Position);

                                                    foreach (var kvp in BuildingSaveDataForCopy.StorageFilters)
                                                    {
                                                        if (ItemDatabase.GetItemById(kvp.Key) != null)
                                                        {
                                                            storage.SetItemFilter(ItemDatabase.GetItemById(kvp.Key), kvp.Value);
                                                        }
                                                    }

                                                    storage.SetAllowanceMode(BuildingSaveDataForCopy.AllowanceMode);
                                                    storage.CurrentCapacity = BuildingSaveDataForCopy.StorageCapacity;
                                                    storage.SetPriority(BuildingSaveDataForCopy.StoragePriority);
                                                }
                                            }
                                            break;
                                        case BuildingType.Crafter:
                                            {
                                                CrafterBuildingCmp crafter = tile.Entity.Get<CrafterBuildingCmp>();
                                                if (crafter != null && crafter.IsBuilt 
                                                    && wasPastedBuildings.Contains(crafter) == false 
                                                    && crafter.BuildingTemplate == copiedBuildingTemplate)
                                                {
                                                    wasPastedBuildings.Add(crafter);

                                                    GameplayScene.Instance.MessageManager.ShowMessage($"+", crafter.Entity.Position);

                                                    foreach (var kvp in BuildingSaveDataForCopy.CraftingRecipesToCraft)
                                                    {
                                                        int craftingRecipeId = kvp.Key;
                                                        int craftingRecipeCraftNumber = kvp.Value;

                                                        if(crafter.Crafter.IdCraftingRecipePair.ContainsKey(craftingRecipeId))
                                                        {
                                                            CraftingRecipe craftingRecipe = crafter.Crafter.IdCraftingRecipePair[craftingRecipeId];

                                                            crafter.SetCraftingRecipeV2(craftingRecipe, craftingRecipeCraftNumber);
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                    
                                }
                            }
                        }
                        break;
                    case MyAction.Cancel:
                        for (int x = 0; x < tiles.GetLength(0); x++)
                        {
                            for (int y = 0; y < tiles.GetLength(1); y++)
                            {
                                Tile tile = tiles[x, y];

                                if (tile.Entity != null)
                                {
                                    if (tile.Entity.Has<BuildingCmp>())
                                    {
                                        BuildingCmp building = tile.Entity.Get<BuildingCmp>();
                                        if (!building.IsBuilt)
                                        {
                                            building.CancelBuilding();
                                        }
                                        else
                                        {
                                            if(building is FarmPlot)
                                            {
                                                FarmPlot farmPlot = building as FarmPlot;
                                                if(farmPlot.Chop)
                                                {
                                                    farmPlot.Chop = false;
                                                }

                                                if(farmPlot.Harvest)
                                                {
                                                    farmPlot.Harvest = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case MyAction.DestructSurface:
                        for (int x = 0; x < tiles.GetLength(0); x++)
                        {
                            for (int y = 0; y < tiles.GetLength(1); y++)
                            {
                                Tile tile = tiles[x, y];

                                BuildingTemplate destructPathBuildingTemplate = Engine.Instance.Buildings["destruct_surface"];

                                TryToBuild(destructPathBuildingTemplate, tile.X, tile.Y, Direction.DOWN);
                            }
                        }
                        break;
                }
            }
        }

        private bool IsOnlyOneTileWasSelected(Tile[,] tiles)
        {
            return tiles.GetLength(0) == 1 && tiles.GetLength(1) == 1;
        }

        public Entity TryToBuild(BuildingTemplate buildingTemplate, int x, int y, Direction buildingDirection, bool completeImmediately = false)
        {
            var localGroundPatternMatrix = Utils.RotateMatrix(buildingTemplate.GroundPattern, buildingDirection);

            Tile[,] tiles = new Tile[localGroundPatternMatrix.GetLength(0), localGroundPatternMatrix.GetLength(1)];

            bool isValid = true;

            int columns = localGroundPatternMatrix.GetLength(0);
            int rows = localGroundPatternMatrix.GetLength(1);

            for (int i = x; i < x + columns; i++)
            {
                for (int j = y; j < y + rows; j++)
                {
                    Tile checkTile = GameplayScene.Instance.World.GetTileAt(i, j);
                    isValid = buildingManager.CheckTileByGroundPattern(buildingTemplate, localGroundPatternMatrix, 
                        GameplayScene.Instance.World.GetTileAt(x, y), checkTile);
                    if(isValid)
                    {
                        tiles[i - x, j - y] = checkTile;
                    }
                    else
                    {
                        break;
                    }
                }

                if (isValid == false)
                    return null;
            }

            Entity entity = buildingTemplate.CreateEntity(tiles, buildingDirection,
                     GameplayScene.BuildingsRequiredBuilding ? completeImmediately : true, 0, interactablesManager);
                
            BuildingCmp newBuilding = entity.Get<BuildingCmp>();

            if (entity.Get<BuildingCmp>().BuildingTemplate.BuildingType != BuildingType.Wall)
            {
                GameplayScene.Instance.AddEntity(entity);
            }
            else
            {
                WallsList.Add(newBuilding);
            }

            return entity;
        }

        private Tile lastBuildingTemplateTile;

        private void DoBuildAction()
        {
            if(CurrentBuildingTemplate != null)
            {
                if (CurrentBuildingTemplate.Rotatable)
                {
                    if (MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.R))
                    {
                        currentBuildingDirection = currentBuildingDirection.NextEnum();

                        buildingImage.Texture = CurrentBuildingTemplate.Icons[currentBuildingDirection];

                        buildingGroundPatternMatrix = Utils.RotateMatrix(CurrentBuildingTemplate.GroundPattern, currentBuildingDirection);
                    }
                }

                Tile buildingTemplateTile = GetBoundedBuildingTemplateTile();

                if (MInput.Mouse.CheckLeftButton && lastBuildingTemplateTile != buildingTemplateTile)
                {
                    lastBuildingTemplateTile = buildingTemplateTile;

                    int x = buildingTemplateTile.X;
                    int y = buildingTemplateTile.Y;

                    Entity entity = TryToBuild(CurrentBuildingTemplate, x, y, currentBuildingDirection);
                    if (entity != null)
                    {
                        ResourceManager.BuildingSoundEffect.Play();
                    }
                }

                if (MInput.Mouse.ReleasedLeftButton)
                {
                    lastBuildingTemplateTile = null;
                }
            }
            else if(MInput.Mouse.PressedLeftButton)
            {
                SetMyAction(MyAction.None, null);
                lastBuildingTemplateTile = null;
            }
        }

        private Queue<SelectableCmp> TryToFillSelectablesQueue(Tile tile, Queue<SelectableCmp> queue)
        {
            queue.Clear();

            foreach (var creatureSelectable in GetCreature((int)GameplayScene.MouseWorldPosition.X, (int)GameplayScene.MouseWorldPosition.Y))
            {
                queue.Enqueue(creatureSelectable);
            }

            SelectableCmp itemsSelectable = GameplayScene.MouseTile.Inventory.TotalItemsCount > 0 ? new SelectableCmp(tile.X * Engine.TILE_SIZE,
                        tile.Y * Engine.TILE_SIZE, 16, 16, SelectableType.ItemContainers) : null;
            if(itemsSelectable != null)
            {
                queue.Enqueue(itemsSelectable);
            }

            SelectableCmp entitySelectable = tile.Entity?.Get<SelectableCmp>();
            if(entitySelectable != null)
            {
                queue.Enqueue(entitySelectable);
            }

            return queue;
        }

        public IEnumerable<SelectableCmp> GetCreature(int mouseX, int mouseY)
        {
            foreach (Entity entity in GameplayScene.Instance.CreatureLayer.Entities)
            {
                CreatureCmp creature = entity.Get<CreatureCmp>();

                if (creature.IsHidden)
                    continue;

                if (creature.IsDead)
                    continue;

                SelectableCmp selectable = creature.Entity.Get<SelectableCmp>();
                if (selectable.Intersects(mouseX, mouseY))
                {
                    yield return selectable;
                }
            }
        }

        private Tile[,] SelectTiles(Tile firstTile, Tile lastTile)
        {
            int firstX = firstTile.X;
            int firstY = firstTile.Y;

            int lastX = lastTile.X;
            int lastY = lastTile.Y;

            if (firstX > lastX)
                MathUtils.Replace(ref firstX, ref lastX);

            if (firstY > lastY)
                MathUtils.Replace(ref firstY, ref lastY);

            Tile[,] tiles = new Tile[(lastX + 1) - firstX, (lastY + 1) - firstY];

            for (int x = firstX; x < lastX + 1; x++)
            {
                for (int y = firstY; y < lastY + 1; y++)
                {
                    tiles[x - firstX, y - firstY] = GameplayScene.Instance.World.GetTileAt(x, y);
                }
            }

            return tiles;
        }

        private void SelectRect(Tile firstTile, Tile lastTile)
        {
            int firstX = firstTile.X;
            int firstY = firstTile.Y;

            int lastX = lastTile.X;
            int lastY = lastTile.Y;

            if (firstX > lastX)
                MathUtils.Replace(ref firstX, ref lastX);

            if (firstY > lastY)
                MathUtils.Replace(ref firstY, ref lastY);

            int width = lastX - firstX + 1;
            int height = lastY - firstY + 1;

            selectedRectangle.X = firstX * Engine.TILE_SIZE;
            selectedRectangle.Y = firstY * Engine.TILE_SIZE;
            selectedRectangle.Width = width * Engine.TILE_SIZE;
            selectedRectangle.Height = height * Engine.TILE_SIZE;
        }

        public Entity GetSelectedEntity()
        {
            return selectedSelectable?.Entity;
        }

        public WorldManagerSaveData GetSaveData()
        {
            WorldManagerSaveData saveData = new WorldManagerSaveData();
            saveData.OpenedItems = new List<int>();
            foreach (var kvp in openedItemsDictionary)
            {
                Item item = kvp.Key;
                bool isOpened = kvp.Value;

                if(isOpened == false)
                {
                    continue;
                }

                saveData.OpenedItems.Add(item.Id);
            }

            saveData.HiddenItems = new List<int>();
            foreach(var kvp in visibleItemsDictionary)
            {
                Item item = kvp.Key;
                bool isVisible = kvp.Value;

                if(isVisible)
                {
                    continue;
                }

                saveData.HiddenItems.Add(item.Id);
            }

            saveData.NewSettlersFoodRationFilters = new Dictionary<int, bool>();
            foreach(var kvp in NewSettlerFoodRationFilters)
            {
                saveData.NewSettlersFoodRationFilters.Add(kvp.Key.Id, kvp.Value);
            }

            return saveData;
        }

    }
}
