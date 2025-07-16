using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Penumbra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Technolithic
{
    public enum BackgroundSongState
    {
        Pause,
        Play
    }

    public class GameplayScene : Scene
    {
        public static GameplayScene Instance { get; private set; }

        public static int WorldSize { get; private set; }
        public static string WorldName { get; private set; }

        public static Tile MouseTile { get; private set; }
        public static Vector2 MouseTilePosition { get; private set; }
        public static Vector2 MouseWorldPosition { get; private set; }

        private static UIRootNode uiRootNode;
        public static UIRootNodeScript UIRootNodeScript { get; private set; }

        public static bool MouseOnUI { get; set; } = false;
        public static bool OnGameMenu { get; set; } = false;

        public AchievementManager AchievementManager { get; private set; }
        public ResourcesLimitManager ResourcesLimitManager { get; private set; }
        public TotalResourcesChart TotalResourcesChart { get; private set; }
        public ProblemIndicatorManager ProblemIndicatorManager { get; private set; }
        public NomadsManager NomadsManager { get; private set; }
        public AnimalSpawnManager AnimalSpawnManager { get; private set; }
        public ProgressTree ProgressTree { get; private set; }
        public TradingSystem TradingSystem { get; private set; }
        public MMessageManager MessageManager { get; private set; }
        public WaterChunkManager WaterChunkManager { get; private set; }
        public PrecipitationManager PrecipitationManager { get; private set; }
        public EnergyManager EnergyManager { get; private set; }
        public SmokeManager SmokeManager { get; private set; }
        public WeatherSoundManager WeatherSoundManager { get; private set; }
        public BuildingRangeRenderer BuildingRangeRenderer { get; private set; }

        public WorldState WorldState { get; private set; }

        public static WorldManager WorldManager { get; private set; }

        public World World { get; private set; } 

        private Layer entityLayer;
        public Layer CreatureLayer { get; private set; }

        public PenumbraComponent Penumbra { get; private set; }

        public static bool BuildingsRequiredDestructing { get; set; } = true;
        public static bool BuildingsRequiredBuilding { get; set; } = true;
        public static bool ShowWaterChunks { get; set; } = false;
        public static bool ShowIrrigatedTiles { get; set; } = false;
        public static bool ShowIlluminatedTiles { get; set; } = false;

        public GameplayCamera GameplayCamera { get; private set; }

        public StormManager StormManager { get; private set; }

        public bool IsAutosaveTriggered { get; set; } = false;

        private List<Song> backgroundSongs;
        private int backgroundSongIndex = 0;
        private BackgroundSongState backgroundSongState = BackgroundSongState.Play;
        private float backgroundSongPauseTimer = 0;
        private int pauseBetweenBackgroundSongs = 600;

        public GameplayScene(string saveFileName, int worldSize, string worldName)
        {
            MediaPlayer.Stop();
            MediaPlayer.IsMuted = false;

            backgroundSongs = new List<Song>
            {
                ResourceManager.GetSong("music_1"),
                ResourceManager.GetSong("music_2")
            };

            backgroundSongIndex = MyRandom.Range(0, backgroundSongs.Count);

            MediaPlayer.Play(backgroundSongs[backgroundSongIndex]);

            WorldName = worldName;

            if (string.IsNullOrEmpty(saveFileName))
            {
                WorldSize = worldSize;
            }
            else
            {
                SaveManager saveManager = new StorageSaveManager(Path.Combine("Saves", worldName), saveFileName);
                saveManager.Load();
                WorldSize = saveManager.Data.WorldSize;
                if (WorldSize == 0)
                {
                    WorldSize = 128;
                }
            }

            MouseOnUI = false;
            OnGameMenu = false;

            Instance = this;

            entityLayer = CreateLayer("Entity");
            entityLayer.Entities.SortByYAxisWhenAdded = true;
            CreatureLayer = CreateLayer("Creature");
            CreatureLayer.Entities.SortByYAxisAlways = true;

            Penumbra = new PenumbraComponent(Engine.Instance);
            Penumbra.Initialize();

            GameplayCamera = new GameplayCamera(false);
            entityLayer.Add(GameplayCamera);

            if (string.IsNullOrEmpty(saveFileName))
            {
                ProgressTree = new ProgressTree(null);

                WorldManager = new WorldManager(null, ProgressTree);
                WorldManager.Begin();

                AchievementManager = new AchievementManager(null);
                TotalResourcesChart = new TotalResourcesChart();
                ResourcesLimitManager = new ResourcesLimitManager(null);
                ProblemIndicatorManager = new ProblemIndicatorManager();
                World = new World(WorldSize, WorldSize, null);
                World.Begin(null);
                WorldState = new WorldState(null);
                WeatherSoundManager = new WeatherSoundManager(WorldState);
                NomadsManager = new NomadsManager(null);
                AnimalSpawnManager = new AnimalSpawnManager();
                TradingSystem = new TradingSystem();
                
                WorldGenerator worldGenerator = new WorldGenerator(World, WorldManager);

                WorldManager.GenerateWorld();

                WaterChunkManager = new WaterChunkManager();
                EnergyManager = new EnergyManager();
                PrecipitationManager = new PrecipitationManager(WorldState.CurrentSeason);

                // Initializing water chunk manager
                Dictionary<MChunk, WaterChunk> chunkWaterChunkKvp = new Dictionary<MChunk, WaterChunk>();
                List<WaterChunk> waterChunks = new List<WaterChunk>();

                for (int x = 0; x < World.Width; x++)
                {
                    for (int y = 0; y < World.Height; y++)
                    {
                        Tile tile = World.GetTileAt(x, y);

                        if (tile.GroundTopType == GroundTopType.Water || tile.GroundTopType == GroundTopType.DeepWater)
                        {
                            if (chunkWaterChunkKvp.ContainsKey(tile.Chunk) == false)
                            {
                                Color color = ((tile.Chunk.Y * (World.Width / MChunk.CHUNK_SIZE) + tile.Chunk.X) + tile.Chunk.Y % 2) % 2 == 1 ? Color.Yellow : Color.Orange;
                                WaterChunk waterChunk = new WaterChunk();
                                waterChunk.Color = color * 0.25f;
                                waterChunks.Add(waterChunk);
                                chunkWaterChunkKvp.Add(tile.Chunk, waterChunk);
                            }

                            chunkWaterChunkKvp[tile.Chunk].AddTile(tile);
                        }
                    }
                }

                // Удаление чанков, которые не соответствуют требованиям, иначе привязываем к тайлам
                for (int i = waterChunks.Count - 1; i >= 0; i--)
                {
                    if (waterChunks[i].IsUnrequired)
                    {
                        waterChunks.RemoveAt(i);
                    }
                    else
                    {
                        WaterChunkManager.AddWaterChunk(waterChunks[i]);
                        waterChunks[i].Initialize(-1);
                    }
                }

                SpawnSettler(80, 50, SettlerGenerator.GenerateSettler());
                SpawnSettler(81, 50, SettlerGenerator.GenerateSettler());
                SpawnSettler(80, 51, SettlerGenerator.GenerateSettler());
                SpawnSettler(81, 51, SettlerGenerator.GenerateSettler());
                SpawnSettler(82, 50, SettlerGenerator.GenerateSettler());

                RenderManager.MainCamera.Position = new Vector2(80 * Engine.TILE_SIZE, 51 * Engine.TILE_SIZE);
                RenderManager.MainCamera.Zoom = 2.0f;
            }
            else
            {
                SaveManager saveManager = new StorageSaveManager(Path.Combine("Saves", worldName), saveFileName);
                saveManager.Load();

                OldSaveConverter.Convert(saveManager);

                ProgressTree = new ProgressTree(saveManager.Data.ProgressTreeSaveData);

                WorldManager = new WorldManager(saveManager.Data.WorldManagerSaveData, ProgressTree);
                WorldManager.Begin();

                AchievementManager = new AchievementManager(saveManager.Data.UnlockedAchievements);
                TotalResourcesChart = new TotalResourcesChart();
                ResourcesLimitManager = new ResourcesLimitManager(saveManager.Data.ResourcesLimits);
                ProblemIndicatorManager = new ProblemIndicatorManager();

                World = new World(WorldSize, WorldSize, saveManager.Data.WorldSaveData);
                World.Begin(saveManager.Data.WorldSaveData);
                WorldState = new WorldState(saveManager.Data.WorldStateSaveData);
                WeatherSoundManager = new WeatherSoundManager(WorldState);
                NomadsManager = new NomadsManager(saveManager.Data.NomadsManagerSaveData);
                AnimalSpawnManager = new AnimalSpawnManager();
                TradingSystem = new TradingSystem();
                
                WaterChunkManager = new WaterChunkManager();
                EnergyManager = new EnergyManager();
                PrecipitationManager = new PrecipitationManager(WorldState.CurrentSeason);

                if (saveManager.Data.IrrigationCanalsToCheck != null)
                {
                    foreach (var point in saveManager.Data.IrrigationCanalsToCheck)
                    {
                        irrigationCanalsToCheck.Add(World.GetTileAt(point.X, point.Y));
                    }
                }

                foreach (var waterChunkSaveData in saveManager.Data.WaterChunkSaveDatas)
                {
                    WaterChunk waterChunk = new WaterChunk();
                    WaterChunkManager.AddWaterChunk(waterChunk);

                    foreach (var point in waterChunkSaveData.Tiles)
                    {
                        Tile tile = World.GetTileAt(point.Item1, point.Item2);
                        Color color = ((tile.Chunk.Y * (World.Width / MChunk.CHUNK_SIZE) + tile.Chunk.X) + tile.Chunk.Y % 2) % 2 == 1 ? Color.Yellow : Color.Orange;
                        waterChunk.Color = color * 0.25f;
                        waterChunk.AddTile(tile);
                    }

                    waterChunk.Initialize(waterChunkSaveData.CurrentFishCount);
                }

                Dictionary<HutBuildingCmp, List<Guid>> hutAssignedCreaturesPairs = new Dictionary<HutBuildingCmp, List<Guid>>();

                foreach (var buildingSaveData in saveManager.Data.BuildingSaveDatas)
                {
                    string buildingTemplateName = buildingSaveData.BuildingTemplateName;

                    if(Engine.Instance.Buildings.ContainsKey(buildingTemplateName) == false)
                    {
                        continue;
                    }

                    BuildingTemplate buildingTemplateToCheck = Engine.Instance.Buildings[buildingTemplateName];

                    Tile[,] tiles = new Tile[buildingSaveData.Tiles.GetLength(0), buildingSaveData.Tiles.GetLength(1)];

                    for (int x = 0; x < tiles.GetLength(0); x++)
                    {
                        for (int y = 0; y < tiles.GetLength(1); y++)
                        {
                            tiles[x, y] = World.GetTileAt(buildingSaveData.Tiles[x, y]);
                        }
                    }

                    Entity buildingEntity = WorldManager.TryToBuild(buildingTemplateToCheck, tiles[0, 0].X, tiles[0, 0].Y, 
                        buildingSaveData.Direction, buildingSaveData.IsBuilt);

                    if(buildingEntity == null)
                    {
                        continue;
                    }

                    BuildingCmp buildingCmp = buildingEntity.Get<BuildingCmp>();

                    buildingCmp.ConstructionProgress = buildingSaveData.BuildingProgress;

                    if (buildingCmp.IsBuilt)
                    {
                        if (buildingCmp.BuildingTemplate.FuelConsumer != null)
                        {
                            if (buildingSaveData.ConsumableFuel != null)
                            {
                                buildingCmp.CurrentFuelCondition = buildingSaveData.CurrentFuelCondition;

                                foreach (var kvp in buildingSaveData.ConsumableFuel)
                                {
                                    if (ItemDatabase.GetItemById(kvp.Key) != null)
                                    {
                                        buildingCmp.SetFuelFilter(ItemDatabase.GetItemById(kvp.Key), kvp.Value);
                                    }
                                }
                            }
                        }

                        switch (buildingCmp.BuildingTemplate.BuildingType)
                        {
                            case BuildingType.FarmPlot:
                                {
                                    FarmPlot farmPlot = buildingCmp as FarmPlot;
                                    farmPlot.SetPlantParameters(
                                            buildingSaveData.GrowthPercent,
                                            buildingSaveData.AdditionalHarvestScrore);

                                    if (buildingSaveData.IsWild)
                                    {
                                        farmPlot.MakeWild();
                                    }

                                    farmPlot.Harvest = buildingSaveData.Harvest;
                                    farmPlot.Chop = buildingSaveData.Chop;
                                    farmPlot.Fertilize = buildingSaveData.Fertilize;
                                    farmPlot.Irrigate = buildingSaveData.Irrigate;
                                    farmPlot.DestructingCurrentProgress = buildingSaveData.HarvestingCurrentProgress;
                                }
                                break;
                            case BuildingType.Gate:
                                {
                                    GateCmp gateCmp = buildingCmp as GateCmp;
                                    gateCmp.SetState(buildingSaveData.GateState);
                                }
                                break;
                            case BuildingType.Stockpile:
                                {
                                    StorageBuildingCmp storageBuildingCmp = buildingCmp as StorageBuildingCmp;

                                    foreach (var filterKVP in buildingSaveData.StorageFilters)
                                    {
                                        if (ItemDatabase.GetItemById(filterKVP.Key) == null)
                                            continue;

                                        storageBuildingCmp.SetItemFilter(ItemDatabase.GetItemById(filterKVP.Key), filterKVP.Value);
                                    }

                                    storageBuildingCmp.SetAllowanceMode(buildingSaveData.AllowanceMode);
                                    storageBuildingCmp.CurrentCapacity = buildingSaveData.StorageCapacity;
                                    storageBuildingCmp.SetPriority(buildingSaveData.StoragePriority);
                                }
                                break;
                            case BuildingType.Crafter:
                                {
                                    CrafterBuildingCmp crafterBuildingCmp = buildingCmp as CrafterBuildingCmp;

                                    crafterBuildingCmp.CurrentCraftingProgress = buildingSaveData.CurrentCraftingProgress;
                                    crafterBuildingCmp.IsPrepared = buildingSaveData.IsCrafterPrepared;

                                    foreach (var kvp in buildingSaveData.CraftingRecipesToCraft)
                                    {
                                        if (!crafterBuildingCmp.Crafter.IdCraftingRecipePair.ContainsKey(kvp.Key))
                                            continue;

                                        CraftingRecipe craftingRecipe = crafterBuildingCmp.Crafter.IdCraftingRecipePair[kvp.Key];

                                        if (kvp.Value == 100)
                                        {
                                            crafterBuildingCmp.RepeatCraftingRecipe(craftingRecipe);
                                        }
                                        else if (kvp.Value > 0)
                                        {
                                            crafterBuildingCmp.SetCraftingRecipe(craftingRecipe, kvp.Value);
                                        }
                                    }
                                }
                                break;
                            case BuildingType.FishTrap:
                                {
                                    FishTrap fishTrap = buildingCmp as FishTrap;

                                    fishTrap.CatchedItem = ItemDatabase.GetItemById(buildingSaveData.FishTrapCatchedItemId);
                                    fishTrap.CurrentTime = buildingSaveData.FishTrapCurrentTime;
                                    fishTrap.CurrentNumberOfUses = buildingSaveData.FishTrapCurrentNumberOfUses;
                                }
                                break;
                            case BuildingType.Deposit:
                                {
                                    DepositCmp depositCmp = buildingCmp as DepositCmp;
                                    depositCmp.CurrentStage = buildingSaveData.DepositCurrentStage;
                                }
                                break;
                            case BuildingType.AnimalPen:
                                {
                                    AnimalPenBuildingCmp animalPenBuilding = buildingCmp as AnimalPenBuildingCmp;

                                    animalPenBuilding.CurrentManureProgress = buildingSaveData.CurrentManureProgress;

                                    if (buildingSaveData.AnimalsFilters != null)
                                    {
                                        foreach (var kvp in buildingSaveData.AnimalsFilters)
                                        {
                                            AnimalTemplate animalTemplate = AnimalTemplateDatabase.GetAnimalTemplateByName(kvp.Key);
                                            bool flag = kvp.Value;
                                            animalPenBuilding.SetAnimalFilter(animalTemplate, flag);
                                        }
                                    }
                                }
                                break;
                            case BuildingType.BeeHive:
                                {
                                    BeeHiveBuildingCmp beeHiveBuildingCmp = buildingCmp as BeeHiveBuildingCmp;
                                    beeHiveBuildingCmp.SetSaveData(buildingSaveData);
                                }
                                break;
                            case BuildingType.Mine:
                                {
                                    MineBuildingCmp mineBuildingCmp = buildingCmp as MineBuildingCmp;
                                    mineBuildingCmp.Timer = buildingSaveData.MineBuildingTimer;
                                    mineBuildingCmp.AutoMineSpawnedDeposits = buildingSaveData.MineBuildingAutoMineSpawnedDeposits;
                                }
                                break;
                            case BuildingType.TradingPost:
                                {
                                    TradingPostBuildingCmp tradingPost = buildingCmp as TradingPostBuildingCmp;
                                    Dictionary<ITradable, int> goods = new Dictionary<ITradable, int>();

                                    foreach(var kvp in buildingSaveData.TradingPostAnimals)
                                    {
                                        AnimalTemplate animalTemplate = AnimalTemplateDatabase.GetAnimalTemplateByName(kvp.Key);
                                        if(animalTemplate != null)
                                        {
                                            goods.Add(animalTemplate, kvp.Value);
                                        }
                                    }

                                    foreach(var kvp in buildingSaveData.TradingPostItems)
                                    {
                                        Item item = ItemDatabase.GetItemById(kvp.Key);
                                        if (item != null)
                                        {
                                            goods.Add(item, kvp.Value);
                                        }
                                    }

                                    tradingPost.SetAssortment(goods);
                                }
                                break;
                            case BuildingType.Hut:
                                {
                                    HutBuildingCmp hut = buildingCmp as HutBuildingCmp;
                                    hutAssignedCreaturesPairs.Add(hut, buildingSaveData.HutAssignedCreatures);
                                }
                                break;
                            case BuildingType.Tree:
                                {
                                    TreeBuilding treeBuilding = buildingCmp as TreeBuilding;
                                    treeBuilding.SetGrowthProgress(buildingSaveData.TreeBuildingGrowthProgress);
                                }
                                break;
                        }
                    }

                    if (buildingSaveData.InventoryItems != null)
                    {
                        foreach (var itemContainer in buildingSaveData.InventoryItems)
                        {
                            Item item = ItemDatabase.GetItemById(itemContainer.Item1);

                            if (item == null)
                                continue;

                            int factWeight = itemContainer.Item2;
                            if (factWeight == 0)
                                continue;

                            float durability = itemContainer.Item3;

                            if (factWeight != 0)
                            {
                                buildingCmp.Inventory.AddCargo(new ItemContainer(item, factWeight, durability));
                            }
                        }
                    }

                    if (buildingSaveData.MarkedInteractions != null)
                    {
                        foreach(InteractionType interactionType in buildingCmp.AvailableInteractions)
                        {
                            if(buildingSaveData.MarkedInteractions.Contains(interactionType))
                            {
                                buildingCmp.MarkInteraction(interactionType);
                            }
                            else
                            {
                                buildingCmp.UnmarkInteraction(interactionType);
                            }
                        }
                    }

                    if (buildingSaveData.InteractionPercentProgressDict != null)
                    {
                        foreach(InteractionType interactionType in buildingCmp.AvailableInteractions)
                        {
                            float percentProgress = 0.0f;

                            if(buildingSaveData.InteractionPercentProgressDict.ContainsKey(interactionType))
                            {
                                percentProgress = buildingSaveData.InteractionPercentProgressDict[interactionType];
                            }

                            buildingCmp.SetInteractionProgressPercent(interactionType, percentProgress);
                        }
                    }

                    buildingCmp.Priority = buildingSaveData.Priority;
                }

                Dictionary<Guid, CreatureCmp> creaturesById = new Dictionary<Guid, CreatureCmp>();

                Dictionary<Guid, Guid> parentChildBinding = new Dictionary<Guid, Guid>();

                foreach (var creatureSaveData in saveManager.Data.CreatureSaveDatas)
                {
                    CreatureCmp creature = null;

                    switch (creatureSaveData.CreatureType)
                    {
                        case CreatureType.Settler:
                            {
                                SettlerInfo settlerInfo = new SettlerInfo(creatureSaveData.Name,
                                    creatureSaveData.HairColor, creatureSaveData.BodyTextureId, creatureSaveData.HairTextureId);

                                Settler settlerEntity = SpawnSettler(creatureSaveData.X, creatureSaveData.Y, settlerInfo);
                                SettlerCmp settlerCmp = settlerEntity.Get<SettlerCmp>();
                                creature = settlerCmp;
                                settlerCmp.Id = creatureSaveData.Id;
                                if(settlerCmp.Id == Guid.Empty)
                                {
                                    settlerCmp.Id = Guid.NewGuid();
                                }

                                creaturesById.Add(settlerCmp.Id, settlerCmp);

                                if(creatureSaveData.ChildId != Guid.Empty)
                                {
                                    parentChildBinding.Add(settlerCmp.Id, creatureSaveData.ChildId);
                                }

                                foreach (var foodItem in Engine.Instance.SettlerRation)
                                {
                                    bool allowed = true;

                                    if(creatureSaveData.Ration.ContainsKey(foodItem.Id))
                                    {
                                        allowed = creatureSaveData.Ration[foodItem.Id];
                                    }

                                    settlerCmp.FoodRation.SetFilter(foodItem, allowed);
                                }

                                if(creatureSaveData.Tools != null)
                                {
                                    foreach(var tpl in creatureSaveData.Tools)
                                    {
                                        int itemId = tpl.Item1;
                                        int itemAmount = tpl.Item2;
                                        float itemDurability = tpl.Item3;

                                        Item item = ItemDatabase.GetItemById(itemId);

                                        ItemContainer itemContainer = new ItemContainer(item, itemAmount, itemDurability);

                                        settlerCmp.CreatureEquipment.EquipTool(itemContainer);
                                    }
                                }

                                if (creatureSaveData.OutfitId != -1)
                                {
                                    settlerCmp.CreatureEquipment.ClothingItemContainer = new ItemContainer(
                                        ItemDatabase.GetItemById(creatureSaveData.OutfitId),
                                        creatureSaveData.OutfitFactWeight,
                                        creatureSaveData.OutfitDurability);
                                }

                                if (creatureSaveData.TopOutfitId != -1)
                                {
                                    settlerCmp.CreatureEquipment.TopClothingItemContainer = new ItemContainer(
                                        ItemDatabase.GetItemById(creatureSaveData.TopOutfitId),
                                        creatureSaveData.TopOutfitFactWeight,
                                        creatureSaveData.TopOutfitDurability);
                                }

                                foreach (var kvp in creatureSaveData.LaborTypePriorityPair.OrEmptyIfNull())
                                {
                                    settlerCmp.SetLaborPriority(Enum.Parse<LaborType>(kvp.Key), kvp.Value);
                                }

                                settlerCmp.CreatureStats.Hunger.CurrentValue = creatureSaveData.Hunger;
                                settlerCmp.CreatureStats.Health.CurrentValue = creatureSaveData.Health;
                                settlerCmp.CreatureStats.Energy.CurrentValue = creatureSaveData.Energy;
                                settlerCmp.CreatureStats.Temperature.CurrentValue = creatureSaveData.Temperature;
                                settlerCmp.CreatureStats.Happiness.CurrentValue = creatureSaveData.Happiness;

                                // TODO: Костыль, т.к. при загрузке старых сохранений у всех существ настроение будет равно 0 и они сразу уйдут с поселения
                                if(settlerCmp.CreatureStats.Happiness.CurrentValue <= 0)
                                {
                                    settlerCmp.CreatureStats.Happiness.CurrentValue = settlerCmp.CreatureStats.Happiness.MaxValue;
                                }

                                Tile tile = World.GetTileAt(creatureSaveData.X, creatureSaveData.Y);

                                // TODO: костыль (в идеале поселенец должен знать куда положить эти предметы)
                                foreach (var inventoryItem in creatureSaveData.InventoryItems)
                                {
                                    if (inventoryItem.Item2 != 0)
                                    {
                                        if (ItemDatabase.GetItemById(inventoryItem.Item1) != null)
                                        {
                                            tile.Inventory.AddCargo(new ItemContainer(ItemDatabase.GetItemById(inventoryItem.Item1),
                                                inventoryItem.Item2, inventoryItem.Item3));
                                        }
                                    }
                                }

                                if (creatureSaveData.StatusEffects != null)
                                {
                                    foreach (var kvp in creatureSaveData.StatusEffects)
                                    {
                                        StatusEffectId statusEffectId = kvp.Key;
                                        float statusEffectProgress = kvp.Value;
                                        StatusEffect statusEffect = settlerCmp.StatusEffectsManager.AddStatusEffect(kvp.Key);
                                        statusEffect.ResetProgress(statusEffectProgress);
                                    }
                                }
                            }
                            break;
                        case CreatureType.Animal:
                            {
                                AnimalTemplate animalTemplate = AnimalTemplateDatabase.GetAnimalTemplateByName(creatureSaveData.AnimalTemplateName);
                                AnimalCmp animalCmp = SpawnAnimal(creatureSaveData.X, creatureSaveData.Y, animalTemplate, creatureSaveData.DaysUntilAging);
                                creature = animalCmp;
                                animalCmp.Name = creatureSaveData.Name;
                                animalCmp.Id = creatureSaveData.Id;
                                if (animalCmp.Id == Guid.Empty)
                                {
                                    animalCmp.Id = Guid.NewGuid();
                                }

                                creaturesById.Add(animalCmp.Id, animalCmp);

                                if (creatureSaveData.ChildId != Guid.Empty)
                                {
                                    parentChildBinding.Add(animalCmp.Id, creatureSaveData.ChildId);
                                }

                                animalCmp.CreatureStats.Hunger.CurrentValue = creatureSaveData.Hunger;
                                animalCmp.CreatureStats.Health.CurrentValue = creatureSaveData.Health;
                                animalCmp.CreatureStats.Energy.CurrentValue = creatureSaveData.Energy;
                                animalCmp.CreatureStats.Temperature.CurrentValue = creatureSaveData.Temperature;
                                animalCmp.CreatureStats.Happiness.CurrentValue = creatureSaveData.Happiness;

                                // TODO: Костыль, т.к. при загрузке старых сохранений у всех существ настроение будет равно 0 и они сразу уйдут с поселения
                                if (animalCmp.CreatureStats.Happiness.CurrentValue <= 0)
                                {
                                    animalCmp.CreatureStats.Happiness.CurrentValue = animalCmp.CreatureStats.Happiness.MaxValue;
                                }

                                Tile tile = World.GetTileAt(creatureSaveData.X, creatureSaveData.Y);

                                // TODO: костыль (в идеале поселенец должен знать куда положить эти предметы)
                                foreach (var inventoryItem in creatureSaveData.InventoryItems)
                                {
                                    if (inventoryItem.Item2 != 0)
                                    {
                                        tile.Inventory.AddCargo(new ItemContainer(ItemDatabase.GetItemById(inventoryItem.Item1),
                                        inventoryItem.Item2, inventoryItem.Item3));
                                    }
                                }

                                foreach (var kvp in creatureSaveData.LaborTypePriorityPair.OrEmptyIfNull())
                                {
                                    animalCmp.SetLaborPriority(Enum.Parse<LaborType>(kvp.Key), kvp.Value);
                                }

                                if (creatureSaveData.Tools != null)
                                {
                                    foreach (var tpl in creatureSaveData.Tools)
                                    {
                                        int itemId = tpl.Item1;
                                        int itemAmount = tpl.Item2;
                                        float itemDurability = tpl.Item3;

                                        Item item = ItemDatabase.GetItemById(itemId);

                                        ItemContainer itemContainer = new ItemContainer(item, itemAmount, itemDurability);

                                        animalCmp.CreatureEquipment.EquipTool(itemContainer);
                                    }
                                }

                                if (creatureSaveData.OutfitId != -1)
                                {
                                    animalCmp.CreatureEquipment.ClothingItemContainer = new ItemContainer(
                                        ItemDatabase.GetItemById(creatureSaveData.OutfitId),
                                        creatureSaveData.OutfitFactWeight,
                                        creatureSaveData.OutfitDurability);
                                }

                                if (creatureSaveData.TopOutfitId != -1)
                                {
                                    animalCmp.CreatureEquipment.TopClothingItemContainer = new ItemContainer(
                                        ItemDatabase.GetItemById(creatureSaveData.TopOutfitId),
                                        creatureSaveData.TopOutfitFactWeight,
                                        creatureSaveData.TopOutfitDurability);
                                }

                                animalCmp.WasAttacked = creatureSaveData.WasAttacked;

                                animalCmp.ProductReadyPercent = creatureSaveData.ProductReadyPercent;

                                animalCmp.IsPregnant = creatureSaveData.IsPregnant;
                                animalCmp.PregnancyProgressInDays = creatureSaveData.PregnancyProgressInDays;

                                animalCmp.HoursPassedFromLastFertilization = creatureSaveData.HoursPassedFromLastFertilization;
                                animalCmp.NextFertilizationHoursSum = creatureSaveData.NextFertilizationHoursSum;
                                animalCmp.IsReadyToFertilization = creatureSaveData.IsReadyToFertilization;

                                animalCmp.Hunt = creatureSaveData.Hunt;

                                if (creatureSaveData.StatusEffects != null)
                                {
                                    foreach (var kvp in creatureSaveData.StatusEffects)
                                    {
                                        StatusEffectId statusEffectId = kvp.Key;
                                        float statusEffectProgress = kvp.Value;
                                        StatusEffect statusEffect = animalCmp.StatusEffectsManager.AddStatusEffect(kvp.Key);
                                        statusEffect.ResetProgress(statusEffectProgress);
                                    }
                                }
                            }
                            break;
                    }

                    if(creature != null)
                    {
                        if (creatureSaveData.MarkedInteractions != null)
                        {
                            foreach (InteractionType interactionType in creature.AvailableInteractions)
                            {
                                if (creatureSaveData.MarkedInteractions.Contains(interactionType))
                                {
                                    creature.MarkInteraction(interactionType);
                                }
                                else
                                {
                                    creature.UnmarkInteraction(interactionType);
                                }
                            }
                        }

                        if (creatureSaveData.InteractionPercentProgressDict != null)
                        {
                            foreach (InteractionType interactionType in creature.AvailableInteractions)
                            {
                                float percentProgress = 0.0f;

                                if (creatureSaveData.InteractionPercentProgressDict.ContainsKey(interactionType))
                                {
                                    percentProgress = creatureSaveData.InteractionPercentProgressDict[interactionType];
                                }

                                creature.SetInteractionProgressPercent(interactionType, percentProgress);
                            }
                        }

                        creature.Priority = creatureSaveData.Priority;
                    }
                }

                int tilemapHalf = (WorldSize * Engine.TILE_SIZE) / 2;
                RenderManager.MainCamera.Position = new Vector2(tilemapHalf, tilemapHalf);

                foreach(var kvp in parentChildBinding)
                {
                    Guid parentId = kvp.Key;
                    Guid childId = kvp.Value;

                    CreatureCmp parentCreature = creaturesById[parentId];
                    CreatureCmp childCreature = creaturesById[childId];

                    childCreature.Parent = parentCreature;
                    parentCreature.Child = childCreature;
                }


                foreach (var kvp in hutAssignedCreaturesPairs)
                {
                    HutBuildingCmp hut = kvp.Key;
                    List<Guid> assignedCreaturesIds = kvp.Value;
                    
                    int slotIndex = 0;
                    foreach (var creatureId in assignedCreaturesIds)
                    {
                        if (creatureId != Guid.Empty)
                        {
                            CreatureCmp creature = creaturesById[creatureId];

                            hut.AssignCreature(creature, slotIndex);
                        }

                        slotIndex++;
                    }
                }
            }

            WorldState.OnNextDayStartedCallback += PrecipitationManager.OnDayChanged;

            NomadsManager.Begin();
            WorldState.NextHourStarted += NomadsManager.NextHour;

            AnimalSpawnManager.Begin();
            WorldState.NextHourStarted += AnimalSpawnManager.NextHour;

            // Если на карте нет ни одного улья, то раз в день, с 5-ти процентным шансом, на карте может заспавниться дикий улей
            WorldState.OnNextDayStartedCallback += SpawnBeeHivesIfThereIsNo;

            TradingSystem.Begin();

            UpdateLists();

            WeatherSoundManager.Begin();

            StormManager = new StormManager();
            SmokeManager = new SmokeManager();
            MessageManager = new MMessageManager();
            BuildingRangeRenderer = new BuildingRangeRenderer();

            uiRootNode = new UIRootNode(this);
            UIRootNodeScript = uiRootNode.GetComponent<UIRootNodeScript>();

            uiRootNode.Awake();
            uiRootNode.Begin();

            if(ProgressTree.AreAllTechnologiesUnlocked())
            {
                AchievementManager.UnlockAchievement(AchievementId.ENGINE_OF_PROGRESS);
            }

            ProgressTree.TechnologyUnlocked += (x) =>
            {
                if (ProgressTree.AreAllTechnologiesUnlocked())
                {
                    AchievementManager.UnlockAchievement(AchievementId.ENGINE_OF_PROGRESS);
                }
            };
        }

        private void SpawnBeeHivesIfThereIsNo(int day, Season season)
        {
            int beehivesAmount = 0;

            foreach (Entity entity in entityLayer.Entities)
            {
                BeeHiveBuildingCmp beehive = entity.Get<BeeHiveBuildingCmp>();
                if(beehive != null && beehive.IsBuilt)
                {
                    beehivesAmount++;
                }
            }

            if (beehivesAmount == 0)
            {
                // Шанс спавна улья равен 5% в день
                if(MyRandom.ProbabilityChance(5))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int randomX = MyRandom.Range(WorldSize);
                        int randomY = MyRandom.Range(WorldSize);
                        Entity entity = WorldManager.TryToBuild(Engine.Instance.Buildings["wild_bee_hive"], randomX, randomY, Direction.DOWN, true);
                        if (entity != null)
                        {
                            entity.Get<BeeHiveBuildingCmp>().SetProgress(0);

                            UIRootNodeScript.NotificationsUI.GetComponent<NotificationsUIScript>()
                                        .AddNotification($"{Localization.GetLocalizedText("a_wild_bee_hive_has_appeared_near_you")}", 
                                        NotificationLevel.INFO, entity);

                            break;
                        }
                    }
                }
            }
        }

        public override void Initialize()
        {
            Engine.ClearColor = new Color(0, 0, 0);
        }

        private List<Tile> irrigationCanalsToCheck = new List<Tile>();
        private float irrigationCanalFillTimer = 0;
        private const int IRRIGATION_CANAL_SOURCE_STRENGTH = 20;

        public void BuildIrrigationCanal(Tile tile)
        {
            tile.GroundTopType = GroundTopType.IrrigationCanalEmpty;

            foreach (var nTile in tile.GetNeighbourTiles())
            {
                if (nTile.GroundTopType == GroundTopType.Water || nTile.GroundTopType == GroundTopType.IrrigationCanalFull)
                {
                    irrigationCanalsToCheck.Add(tile);
                    break;
                }
            }
        }

        public void DestructIrrigationCanal(Tile tile)
        {
            tile.GroundTopType = GroundTopType.None;
            tile.IrrigationStrength = 0;

            if(irrigationCanalsToCheck.Contains(tile))
            {
                irrigationCanalsToCheck.Remove(tile);
            }

            foreach(var neighTile in tile.GetNeighbourTiles())
            {
                if(irrigationCanalsToCheck.Contains(neighTile) == false)
                {
                    if(neighTile.GroundTopType == GroundTopType.IrrigationCanalFull)
                    {
                        irrigationCanalsToCheck.Add(neighTile);
                    }
                }
            }
        }

        public override void Update()
        {
            if(WorldManager.TotalSettlersCount >= 15)
            {
                AchievementManager.UnlockAchievement(AchievementId.TRIBE);
            }

            if (WorldManager.TotalSettlersCount >= 30)
            {
                AchievementManager.UnlockAchievement(AchievementId.VILLAGE);
            }

            if (WorldManager.TotalSettlersCount >= 60)
            {
                AchievementManager.UnlockAchievement(AchievementId.CITY);
            }

            switch (backgroundSongState)
            {
                case BackgroundSongState.Play:
                    {
                        // Воспроизведение музыки окончено
                        if(MediaPlayer.State == MediaState.Stopped)
                        {
                            backgroundSongState = BackgroundSongState.Pause;

                            backgroundSongState = 0;
                        }
                    }
                    break;
                case BackgroundSongState.Pause:
                    {
                        backgroundSongPauseTimer += Engine.DeltaTime;

                        if(backgroundSongPauseTimer >= pauseBetweenBackgroundSongs)
                        {
                            backgroundSongState = BackgroundSongState.Play;

                            backgroundSongIndex++;

                            if(backgroundSongIndex >= backgroundSongs.Count)
                            {
                                backgroundSongIndex = 0;
                            }

                            MediaPlayer.Play(backgroundSongs[backgroundSongIndex]);
                        }
                    }
                    break;
            }

            Penumbra.Transform = RenderManager.MainCamera.Transformation;
            Penumbra.AmbientColor = WorldState.TimeOfDayColor;

            MouseOnUI = false;

            uiRootNode.Update(MInput.Mouse.X, MInput.Mouse.Y);

            if(MouseOnUI == false)
            {
                if (MouseTile?.WaterChunk != null)
                {
                    GlobalUI.ShowTooltips($"{Localization.GetLocalizedText("number_of_fish")}: " +
                        $"{MouseTile.WaterChunk.CurrentFishCount}/{MouseTile.WaterChunk.MaxFishCount}");
                }
            }

            if (!OnGameMenu)
            {
                irrigationCanalFillTimer += Engine.GameDeltaTime;

                if(irrigationCanalFillTimer >= 0.5f)
                {
                    irrigationCanalFillTimer = 0;

                    for (int i = irrigationCanalsToCheck.Count - 1; i >= 0; i--)
                    {
                        Tile irrigationCanalTile = irrigationCanalsToCheck[i];

                        foreach(var neighTile in irrigationCanalTile.GetNeighbourTiles())
                        {
                            if(neighTile.GroundTopType == GroundTopType.Water)
                            {
                                irrigationCanalTile.IrrigationStrength = IRRIGATION_CANAL_SOURCE_STRENGTH;
                                break;
                            }
                            else if(neighTile.GroundTopType == GroundTopType.IrrigationCanalFull)
                            {
                                if(neighTile.IrrigationStrength > irrigationCanalTile.IrrigationStrength + 1)
                                {
                                    irrigationCanalTile.IrrigationStrength = neighTile.IrrigationStrength - 1;
                                }
                            }
                        }

                        bool isOnlyGreat = true;
                        // Если канал один с большим числом, то делаем его пустым
                        foreach(var neighTile in irrigationCanalTile.GetNeighbourTiles())
                        {
                            if(neighTile.GroundTopType == GroundTopType.Water)
                            {
                                isOnlyGreat = false;
                                break;
                            }
                            else if(neighTile.IrrigationStrength >= irrigationCanalTile.IrrigationStrength)
                            {
                                isOnlyGreat = false;
                                break;
                            }
                        }

                        if(isOnlyGreat)
                        {
                            irrigationCanalTile.IrrigationStrength = 0;
                        }

                        if(irrigationCanalTile.IrrigationStrength > 0)
                        {
                            irrigationCanalTile.GroundTopType = GroundTopType.IrrigationCanalFull;

                            foreach (var neighTile in irrigationCanalTile.GetNeighbourTiles())
                            {
                                if (neighTile.GroundTopType == GroundTopType.IrrigationCanalEmpty)
                                {
                                    if (irrigationCanalsToCheck.Contains(neighTile) == false)
                                    {
                                        irrigationCanalsToCheck.Add(neighTile);
                                    }
                                }
                                else if (neighTile.GroundTopType == GroundTopType.IrrigationCanalFull)
                                {
                                    if (neighTile.IrrigationStrength < irrigationCanalTile.IrrigationStrength - 1)
                                    {
                                        if (irrigationCanalsToCheck.Contains(neighTile) == false)
                                        {
                                            irrigationCanalsToCheck.Add(neighTile);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            irrigationCanalTile.GroundTopType = GroundTopType.IrrigationCanalEmpty;

                            foreach(var neighTile in irrigationCanalTile.GetNeighbourTiles())
                            {
                                if(neighTile.GroundTopType == GroundTopType.IrrigationCanalFull)
                                {
                                    if (irrigationCanalsToCheck.Contains(neighTile) == false)
                                    {
                                        irrigationCanalsToCheck.Add(neighTile);
                                    }
                                }
                            }
                        }

                        irrigationCanalsToCheck.Remove(irrigationCanalTile);
                    }
                }
                
                UpdateMouseWorldPosition();
                UpdateMouseTilePosition();

                WorldState.Update();
                WeatherSoundManager.Update();
                ProblemIndicatorManager.Update();
                StormManager.Update(WorldState.CurrentWeather, WorldState.CurrentSeason);
                MessageManager.Update();
                BuildingRangeRenderer.Update();
                WorldManager.Update(WorldState.CurrentSeason);
                EnergyManager.Update();
                SmokeManager.Update();
                WaterChunkManager.Update();
                PrecipitationManager.Update();

                base.Update();

                World.Update();

                if(IsAutosaveTriggered)
                {
                    IsAutosaveTriggered = false;

                    string worldSavesDirectory = Path.Combine(Engine.GetGameDirectory(), "Saves", WorldName);
                    if (Directory.Exists(worldSavesDirectory) == false)
                    {
                        Directory.CreateDirectory(worldSavesDirectory);
                    }

                    for (int i = 0; i < 1000; i++)
                    {
                        if (File.Exists(Path.Combine(worldSavesDirectory, $"Auto {i}")) == false)
                        {
                            SaveGame("Auto " + i, true);
                            break;
                        }
                    }
                }
            }
        }

        public override void Render()
        {
            World.RenderUpdate();

            Penumbra.BeginDraw();

            // Строения
            RenderManager.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null, null, null, RenderManager.MainCamera.Transformation);

            // Основные тайлмапы
            if (WorldState.LastSeasonAlpha > 0)
            {
                switch (WorldState.LastSeason)
                {
                    case Season.Autumn:
                        World.RenderAutumnGroundTileMap(WorldState.LastSeasonAlpha);
                        break;
                    case Season.Summer:
                        World.RenderSummerGroundTileMap(WorldState.LastSeasonAlpha);
                        break;
                    case Season.Winter:
                        World.RenderWinterGroundTileMap(WorldState.LastSeasonAlpha);
                        break;
                    case Season.Spring:
                        World.RenderSpringGroundTileMap(WorldState.LastSeasonAlpha);
                        break;
                }
            }

            if (WorldState.CurrentSeasonAlpha > 0)
            {
                switch (WorldState.CurrentSeason)
                {
                    case Season.Autumn:
                        World.RenderAutumnGroundTileMap(WorldState.CurrentSeasonAlpha);
                        break;
                    case Season.Summer:
                        World.RenderSummerGroundTileMap(WorldState.CurrentSeasonAlpha);
                        break;
                    case Season.Winter:
                        World.RenderWinterGroundTileMap(WorldState.CurrentSeasonAlpha);
                        break;
                    case Season.Spring:
                        World.RenderSpringGroundTileMap(WorldState.CurrentSeasonAlpha);
                        break;
                }
            }

            World.RenderGroundTopTileMap();

            WaterChunkManager.Render();

            World.RenderSurfaceTileMap();
            World.RenderBlockTileMap();

            entityLayer.Render();

            // Предметы
            World.RenderItemTileMap();

            CreatureLayer.Render();

            SmokeManager.Render();

            PrecipitationManager.Render();

            RenderManager.SpriteBatch.End();

            Penumbra.Draw(Engine.GameTime);

            RenderManager.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null, null, null, RenderManager.MainCamera.Transformation);

            StormManager.Render();

            BuildingRangeRenderer.Render();

            if (ShowWaterChunks)
            {
                WaterChunkManager.DebugRender();
            }

            if (MouseTile?.WaterChunk != null)
            {
                MouseTile.WaterChunk.DebugRender();
            }

            if (ShowIrrigatedTiles)
            {
                for (int x = 0; x < World.Width; x++)
                {
                    for (int y = 0; y < World.Height; y++)
                    {
                        if (World.GetTileAt(x, y).IrrigationLevel > 0)
                        {
                            RenderManager.Rect(x * Engine.TILE_SIZE, y * Engine.TILE_SIZE, Engine.TILE_SIZE, Engine.TILE_SIZE, Color.Blue * 0.5f);
                        }
                    }
                }
            }

            if(ShowIlluminatedTiles)
            {
                for (int x = 0; x < World.Width; x++)
                {
                    for (int y = 0; y < World.Height; y++)
                    {
                        if (World.GetTileAt(x, y).IsIlluminated)
                        {
                            RenderManager.Rect(x * Engine.TILE_SIZE, y * Engine.TILE_SIZE, Engine.TILE_SIZE, Engine.TILE_SIZE, Color.Yellow * 0.5f);
                        }
                    }
                }
            }

            World.RenderMarkTileMap();

            // TODO: Before refactoring, I will place the rendering of the marked interactables here
            foreach(Entity entity in entityLayer.Entities)
            {
                Interactable interactable = entity.Get<Interactable>();
                if(interactable != null)
                {
                    RenderInteractableMarks(interactable);
                }
            }

            foreach (Entity entity in CreatureLayer.Entities)
            {
                Interactable interactable = entity.Get<Interactable>();
                if (interactable != null)
                {
                    RenderInteractableMarks(interactable);
                }
            }

            WorldManager.Render();

            EnergyManager.Render();

            MessageManager.Render();

            RenderManager.SpriteBatch.End();

            RenderManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null,
                Engine.RasterizerState);

            uiRootNode.Render();

            RenderManager.SpriteBatch.End();
        }

        private void RenderInteractableMarks(Interactable interactable)
        {
            Entity entity = interactable.Entity;
            foreach (InteractionType interactionType in interactable.AvailableInteractions)
            {
                InteractionData interactionData = Engine.InteractionsDatabase.GetInteractionData(interactionType);

                if (interactionData == null) continue;

                switch (interactionData.IconDisplayState)
                {
                    case InteractionIconDisplayState.OnMarked:
                        {
                            if (interactable.IsInteractionActivated(interactionType) &&
                                interactable.IsInteractionMarked(interactionType))
                            {
                                interactionData.Icon?.Draw(entity.Position, Color.White * 0.75f);
                            }
                        }
                        break;
                    case InteractionIconDisplayState.OnUnmarked:
                        {
                            if (interactable.IsInteractionMarked(interactionType) == false)
                            {
                                interactionData.Icon?.Draw(entity.Position, Color.White * 0.75f);
                                ResourceManager.DisableIcon.Draw(entity.Position, Color.White * 0.75f);
                            }
                        }
                        break;
                }
            }
        }

        public void AddEntity(Entity entity)
        {
            entityLayer.Add(entity);
        }

        private void UpdateMouseWorldPosition()
        {
            float mposx = (MInput.Mouse.X - Engine.HalfWidth) / RenderManager.MainCamera.Zoom;
            float mposy = (MInput.Mouse.Y - Engine.HalfHeight) / RenderManager.MainCamera.Zoom;

            MouseWorldPosition = new Vector2((int)mposx + RenderManager.MainCamera.Position.X, (int)mposy + RenderManager.MainCamera.Position.Y);
        }

        private void UpdateMouseTilePosition()
        {
            Vector2 globalPos = MouseWorldPosition;
            int x = (int)globalPos.X / Engine.TILE_SIZE;
            int y = (int)globalPos.Y / Engine.TILE_SIZE;

            if (x < 0)
                x = 0;
            else if (x >= World.Width)
                x = World.Width - 1;

            if (y < 0)
                y = 0;
            else if (y >= World.Height)
                y = World.Height - 1;

            MouseTile = World.GetTileAt(x, y);
            MouseTilePosition = new Vector2(x * Engine.TILE_SIZE, y * Engine.TILE_SIZE);
        }

        public Settler SpawnSettler(int x, int y, SettlerInfo settlerInfo, Dictionary<Item, bool> foodRationFilters = null)
        {
            Tile spawnTile = World.GetTileAt(x, y);
            Settler settler = new Settler(settlerInfo, WorldManager.SettlerBeverageRation, spawnTile);
            WorldManager.AddCreature(settler.Get<SettlerCmp>());
            CreatureLayer.Add(settler);

            if (foodRationFilters != null)
            {
                SettlerCmp settlerCmp = settler.Get<SettlerCmp>();

                foreach (var kvp in foodRationFilters)
                {
                    settlerCmp.FoodRation.SetFilter(kvp.Key, kvp.Value);
                }
            }

            return settler;
        }

        public AnimalCmp SpawnAnimal(int tileX, int tileY, AnimalTemplate animalTemplate, int daysUntilAging)
        {
            Tile spawnTile = World.GetTileAt(tileX, tileY);
            Entity animal = WorldManager.SpawnAnimal(animalTemplate, spawnTile, daysUntilAging);

            CreatureLayer.Add(animal);

            return animal.Get<AnimalCmp>();
        }

        public void SaveGame(string saveName, bool isAutosave)
        {
            // removing old saves
            if (isAutosave)
            {
                try
                {
                    Dictionary<DateTime, string> dateTimeSavePair = new Dictionary<DateTime, string>();
                    string[] saveFiles = Directory.GetFiles(Path.Combine(Engine.GetGameDirectory(), "Saves", WorldName));
                    foreach (var saveFile in saveFiles)
                    {
                        if (saveFile.EndsWith("_info"))
                        {
                            SaveInfo saveInfo = JsonConvert.DeserializeObject<SaveInfo>(File.ReadAllText(saveFile));

                            if (saveInfo.IsAutosave)
                            {
                                dateTimeSavePair.Add(DateTime.Parse(saveInfo.DateTime), saveFile);
                            }
                        }
                    }

                    int startFrom = 4;
                    if (dateTimeSavePair.Count > startFrom)
                    {
                        List<DateTime> orderedDatesList = dateTimeSavePair.Keys.OrderByDescending(d => d).ToList();

                        for (int i = startFrom; i < orderedDatesList.Count; i++)
                        {
                            string saveFileName = dateTimeSavePair[orderedDatesList[i]];
                            saveFileName = saveFileName.Substring(0, saveFileName.Length - "_info".Length);

                            if (File.Exists(saveFileName))
                            {
                                File.Delete(saveFileName);
                            }

                            if (File.Exists(saveFileName + "_info"))
                            {
                                File.Delete(saveFileName + "_info");
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }

            // creating new save
            SaveManager saveManager = new StorageSaveManager(Path.Combine("Saves", WorldName), saveName);
            saveManager.Data.WorldManagerSaveData = WorldManager.GetSaveData();
            saveManager.Data.WorldSaveData = World.GetSaveData();
            saveManager.Data.WorldStateSaveData = WorldState.GetSaveData();
            saveManager.Data.ProgressTreeSaveData = ProgressTree.GetSaveData();
            saveManager.Data.WorldSize = WorldSize;
            saveManager.Data.NomadsManagerSaveData = NomadsManager.GetSaveData();

            saveManager.Data.IrrigationCanalsToCheck = new List<Point>();
            foreach (var tile in irrigationCanalsToCheck)
            {
                saveManager.Data.IrrigationCanalsToCheck.Add(new Point(tile.X, tile.Y));
            }

            saveManager.Data.BuildingSaveDatas = new List<BuildingSaveData>();
            foreach (var entity in entityLayer.Entities)
            {
                if (entity.IsRemoved)
                    continue;

                BuildingCmp buildingCmp = entity.Get<BuildingCmp>();
                if (buildingCmp != null)
                {
                    saveManager.Data.BuildingSaveDatas.Add(buildingCmp.GetSaveData());
                }
            }

            foreach(var wall in GameplayScene.WorldManager.WallsList)
            {
                saveManager.Data.BuildingSaveDatas.Add(wall.GetSaveData());
            }

            saveManager.Data.CreatureSaveDatas = new List<CreatureSaveData>();
            foreach (var entity in CreatureLayer.Entities)
            {
                if (entity.IsRemoved)
                    continue;

                SettlerCmp settlerCmp = entity.Get<SettlerCmp>();
                if (settlerCmp != null)
                {
                    saveManager.Data.CreatureSaveDatas.Add(settlerCmp.GetSaveData());
                    continue;
                }

                AnimalCmp animalCmp = entity.Get<AnimalCmp>();
                if (animalCmp != null)
                {
                    saveManager.Data.CreatureSaveDatas.Add(animalCmp.GetSaveData());
                }
            }

            saveManager.Data.WaterChunkSaveDatas = new List<WaterChunkSaveData>();
            foreach (var waterChunk in WaterChunkManager.WaterChunks)
            {
                saveManager.Data.WaterChunkSaveDatas.Add(waterChunk.GetSaveData());
            }

            saveManager.Data.ResourcesLimits = ResourcesLimitManager.GetSaveData();

            saveManager.Data.UnlockedAchievements = AchievementManager.GetUnlockedAchievements();

            saveManager.Info.DateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            saveManager.Info.GameVersion = Engine.GameVersion;
            saveManager.Info.IsAutosave = isAutosave;

            saveManager.Save();
        }

    }
}
