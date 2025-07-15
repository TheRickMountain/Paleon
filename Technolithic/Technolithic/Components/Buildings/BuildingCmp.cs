using Microsoft.Xna.Framework;
using Penumbra;
using System;
using System.Collections.Generic;

namespace Technolithic
{
    public class BuildingCmp : Interactable
    {
        public AnimatedSprite Sprite { get; private set; }

        protected List<Tile> TargetTiles = new List<Tile>();
        public TileInfo[,] TilesInfosArray { get; private set; }
        public List<TileInfo> TilesInfosList { get; private set; }
        public HashSet<Tile> RangeTiles { get; private set; }

        public bool IsBuilt { get; protected set; }

        public Action<BuildingCmp> OnBuildingCompletedCallback { get; set; }
        public Action<BuildingCmp> OnBuildingCanceledCallback { get; set; }
        public Action<BuildingCmp> OnBuildingDestructedCallback { get; set; }

        public float ConstructionProgress { get; set; }

        public Inventory Inventory { get; private set; }

        private BuildLabor buildLabor;

        public bool IsTurnedOn { get; set; }

        // *** Fuel ***

        public float CurrentFuelCondition { get; set; } = 0;
        public Dictionary<Item, bool> ConsumableFuelDictionary { get; private set; }

        // *** *** ***

        private Light light;
        private Timer flickeringTimer;
        private Queue<float> smoothQueue;
        private float lastSum = 0;

        public BuildingTemplate BuildingTemplate { get; private set; }
        public Direction Direction { get; private set; }

        private int popUpPositionX;
        private int popUpPositionY;

        public Vector2 CenterPosition { get; private set; }

        private Tile[,] tiles;

        private string fuelString;
        private string consumptionString;

        public bool ReservedToSupplyFuel { get; set; }

        public bool IsReserved { get; set; }

        public bool ThrowBuildingRecipeItemsAfterDestructing { get; set; } = true;

        public bool IsPowered { get; set; } = true;

        private MyTimer smokeTimer;

        private float gearRotation = 0;

        public BuildingCmp(BuildingTemplate buildingTemplate, Direction direction) : base(true, true)
        {
            BuildingTemplate = buildingTemplate;
            if (BuildingTemplate.Rotatable == false)
            {
                Direction = Direction.DOWN;
            }
            else
            {
                Direction = direction;
            }

            fuelString = Localization.GetLocalizedText("fuel");
            consumptionString = Localization.GetLocalizedText("consumption");

            Inventory = new Inventory(this);
            Inventory.IsStorage = false;

            switch (buildingTemplate.BuildingType)
            {
                case BuildingType.Wall:
                    break;
                default:
                    {
                        MyTexture texture = buildingTemplate.Textures[Direction];

                        int textureWidth = -1;
                        int textureHeight = -1;

                        switch (Direction)
                        {
                            case Direction.DOWN:
                            case Direction.UP:
                                textureWidth = buildingTemplate.TextureWidth;
                                textureHeight = buildingTemplate.TextureHeight;
                                break;
                            case Direction.LEFT:
                            case Direction.RIGHT:
                                textureWidth = buildingTemplate.TextureHeight;
                                textureHeight = buildingTemplate.TextureWidth;
                                break;
                        }

                        Sprite = new AnimatedSprite(textureWidth, textureHeight);
                        Sprite.Add("Idle", new Animation(texture, 1, 0, textureWidth, textureHeight, 0, 0, 1));

                        if (buildingTemplate.Animated)
                        {
                            int frameCount = buildingTemplate.Textures[Direction].Width / textureWidth;
                            Sprite.Add("Process", new Animation(texture, frameCount, 0, textureWidth, textureHeight, 0, textureHeight, 5));
                        }
                    }
                    break;
            }

            TilesInfosArray = new TileInfo[buildingTemplate.Width, buildingTemplate.Height];
            TilesInfosList = new List<TileInfo>();

            for (int x = 0; x < TilesInfosArray.GetLength(0); x++)
            {
                for (int y = 0; y < TilesInfosArray.GetLength(1); y++)
                {
                    TileInfo tileInfo = new TileInfo(
                        buildingTemplate.WalkablePattern[x, y],
                        buildingTemplate.TargetPattern[x, y],
                        buildingTemplate.GroundPattern[x, y]);

                    TilesInfosArray[x, y] = tileInfo;
                    TilesInfosList.Add(tileInfo);
                }
            }

            if (BuildingTemplate.Rotatable)
                TilesInfosArray = Utils.RotateMatrix(TilesInfosArray, Direction);

        }

        protected void PlayIdleAnimation()
        {
            Sprite?.Play("Idle");
        }

        protected void PlayProcessAnimation()
        {
            if (BuildingTemplate.Animated)
            {
                Sprite?.Play("Process");
            }
        }

        private bool wasTurnedOn = false;

        // Этот метод нельзя наследовать, только UpdateCompleted!!!
        public override void Update()
        {
            Sprite?.Update();

            if (IsBuilt)
            {
                UpdateCompleted();
            }
        }

        public virtual void UpdateCompleted()
        {
            if (BuildingTemplate.EnergyConsumer != null)
            {
                if (IsTurnedOn)
                {
                    gearRotation += Engine.GameDeltaTime;
                }
            }
        }

        public override void LateUpdate()
        {
            if (IsBuilt)
            {
                if (BuildingTemplate.FuelConsumer != null)
                {
                    // Если строение не имеет топлива, то выключаем ее
                    if (CurrentFuelCondition <= 0)
                        IsTurnedOn = false;

                    // Если строение включено и имеет достаточно топлива
                    if (IsTurnedOn && CurrentFuelCondition > 0)
                    {
                        CurrentFuelCondition -= (BuildingTemplate.FuelConsumer.EnergyConsumption * Engine.GameDeltaTime);

                    }
                }

                if (IsTurnedOn)
                {
                    if (BuildingTemplate.SmokeGeneratorData != null)
                    {
                        smokeTimer.Update(Engine.GameDeltaTime);
                    }

                    // Включаем освещение
                    if (BuildingTemplate.LightEmitter != null)
                    {
                        if (wasTurnedOn == false)
                        {
                            light.Intensity = 1.0f;

                            wasTurnedOn = true;

                            Tile centerTile = GetRealCenterTile();

                            foreach (var tilesToIlluminate in Utils.GetTilesInCircle(centerTile, (int)(BuildingTemplate.LightEmitter.Radius / 33.33f)))
                            {
                                tilesToIlluminate.IsIlluminated = true;
                            }
                        }

                        if (flickeringTimer.GetTime() >= 0.1f)
                        {
                            flickeringTimer.Reset();

                            while (smoothQueue.Count >= 5)
                            {
                                lastSum -= smoothQueue.Dequeue();
                            }

                            float newVal = MyRandom.Range(0.5f, 1.2f);
                            smoothQueue.Enqueue(newVal);
                            lastSum += newVal;

                            light.Intensity = lastSum / (float)smoothQueue.Count;
                        }
                    }
                }
                else
                {
                    // Выключить освещение
                    if (BuildingTemplate.LightEmitter != null)
                    {
                        if (wasTurnedOn)
                        {
                            light.Intensity = 0;

                            wasTurnedOn = false;

                            Tile centerTile = GetRealCenterTile();

                            foreach (var tilesToIlluminate in Utils.GetTilesInCircle(centerTile, (int)(BuildingTemplate.LightEmitter.Radius / 33.33f)))
                            {
                                tilesToIlluminate.IsIlluminated = false;
                            }
                        }
                    }
                }

                if (IsTurnedOn)
                {
                    PlayProcessAnimation();
                }
                else
                {
                    PlayIdleAnimation();
                }
            }
        }

        public override void Render()
        {
            Sprite?.Render();

            if (IsBuilt)
            {
                if (BuildingTemplate.FuelConsumer != null)
                {
                    if (CurrentFuelCondition <= 0)
                    {
                        ResourceManager.NoFuelIcon.Draw(new Rectangle(popUpPositionX, popUpPositionY, 16, 16), Color.White * 0.75f);
                    }
                }

                if (BuildingTemplate.EnergyConsumer != null && BuildingTemplate.EnergyConsumer.EnergyType == EnergyType.Kinetic)
                {
                    Tile rightBottomTile = TilesInfosArray[BuildingTemplate.Width - 1, BuildingTemplate.Height - 1].Tile;
                    ResourceManager.GearIcon.Draw(new Vector2(
                        rightBottomTile.X * Engine.TILE_SIZE + 8,
                        rightBottomTile.Y * Engine.TILE_SIZE + 8), new Vector2(8, 8), Color.White, 1, gearRotation);
                }

                if (IsPowered == false)
                {
                    if (BuildingTemplate.EnergyConsumer != null)
                    {
                        switch (BuildingTemplate.EnergyConsumer.EnergyType)
                        {
                            case EnergyType.Gas:
                                {
                                    ResourceManager.NoGasIcon.Draw(new Rectangle(popUpPositionX, popUpPositionY, 16, 16), Color.White * 0.75f);
                                }
                                break;
                            case EnergyType.Kinetic:
                                {
                                    ResourceManager.NoEnergyIcon.Draw(new Rectangle(popUpPositionX, popUpPositionY, 16, 16), Color.White * 0.75f);
                                }
                                break;
                        }
                    }
                }
            }
        }

        public void SetToBuild(Tile[,] tiles, bool requireBuilding, float currentFuelCondition)
        {
            this.tiles = tiles;

            Entity.X = tiles[0, 0].X * Engine.TILE_SIZE;
            Entity.Y = tiles[0, 0].Y * Engine.TILE_SIZE;

            CurrentFuelCondition = currentFuelCondition;

            // Инициализируем Tile info и назначаем entity для каждого тайла
            for (int column = 0; column < TilesInfosArray.GetLength(0); ++column)
            {
                for (int row = 0; row < TilesInfosArray.GetLength(1); ++row)
                {
                    Tile tile = tiles[column, row];
                    TileInfo tileInfo = TilesInfosArray[column, row];

                    tileInfo.Tile = tile;

                    switch (BuildingTemplate.BuildingType)
                    {
                        case BuildingType.Wall:
                            tileInfo.Tile.Entity = Entity;
                            break;
                        default:
                            tileInfo.Tile.Entity = Entity;
                            break;
                    }
                }
            }

            switch (BuildingTemplate.BuildingType)
            {
                case BuildingType.Wall:
                    break;
                default:
                    {
                        Sprite.Entity = Entity;

                        Sprite.Color = Color.White * 0.5f;

                        Sprite.SetOrigin(Sprite.Width / 2, Sprite.Height / 2);

                        int imageYOffset = TilesInfosArray.GetLength(1) * Engine.TILE_SIZE - Sprite.Height;
                        int imageXOffset = (TilesInfosArray.GetLength(0) * Engine.TILE_SIZE - Sprite.Width) / 2;

                        Sprite.X = imageXOffset + Sprite.Width / 2;
                        Sprite.Y = imageYOffset + Sprite.Height / 2;

                        Sprite.Play("Idle");
                    }
                    break;
            }

            // Добавляем все соседние тайлы занятые строением для доставки ресурсов
            foreach (var tileInfo in TilesInfosArray)
            {
                foreach (var neigh in tileInfo.Tile.GetNeighbourTiles())
                    if (!ContainsTile(TilesInfosList, neigh))
                        TargetTiles.Add(neigh);
            }

            // Если строение не требует строительства, то завершаем постройку сразу
            if (requireBuilding == false)
            {
                Inventory.InventoryRequired.Clear();

                CompleteBuilding();
            }
            else
            {
                bool hasRequiredItems = false;

                foreach (var buildingRecipe in BuildingTemplate.BuildingRecipe)
                {
                    Item item = buildingRecipe.Key;
                    int weight = buildingRecipe.Value;

                    Inventory.AddRequiredWeight(item, weight);
                    hasRequiredItems = true;
                }

                // Если строение не требует предметов, то строим сразу
                if (hasRequiredItems == false)
                {
                    buildLabor = new BuildLabor(this, BuildingTemplate.BuildingLaborType);
                    GameplayScene.WorldManager.LaborManager.Add(buildLabor);
                }
            }

            Inventory.OnItemAddedCallback += OnItemAdded;
            Inventory.OnItemRemovedCallback += OnItemRemoved;
        }

        private bool ContainsTile(List<TileInfo> tilesInfos, Tile tile)
        {
            foreach (var item in tilesInfos)
            {
                if (item.Tile == tile)
                    return true;
            }

            return false;
        }

        protected virtual void OnItemAdded(Inventory senderInventory, Item item, int weight)
        {
            if (Inventory.InventoryRequired.ContainsKey(item))
            {
                Inventory.AddRequiredWeight(item, -weight);
            }

            // Если строение не построено, но доставлены все необходимые ресурсы, то создаем работу по строительству
            if (!IsBuilt && IsSupplyCompleted())
            {
                buildLabor = new BuildLabor(this, BuildingTemplate.BuildingLaborType);
                GameplayScene.WorldManager.LaborManager.Add(buildLabor);
            }
        }

        protected virtual void OnItemRemoved(Inventory senderInventory, Item item, int weight)
        {

        }

        private bool IsSupplyCompleted()
        {
            foreach (var recipe in BuildingTemplate.BuildingRecipe)
            {
                int factWeight = Inventory.GetInventoryFactWeight(recipe.Key);

                if (factWeight != recipe.Value)
                    return false;
            }

            return true;
        }

        private void OnSmokeTimerTimeout()
        {
            Vector2 smokePosition = Entity.Position + BuildingTemplate.SmokeGeneratorData.SpawnPosition;
            GameplayScene.Instance.SmokeManager.AddSmoke(smokePosition);
        }

        public virtual void CompleteBuilding()
        {
            IsBuilt = true;

            if(BuildingTemplate.IsDestructible)
            {
                // TODO: some buildings may require a tool for deconstruction
                AddAvailableInteraction(InteractionType.Destruct, LaborType.Build, false);

                SetInteractionDuration(InteractionType.Destruct, BuildingTemplate.ConstructionTime);
                ActivateInteraction(InteractionType.Destruct);
            }

            if (BuildingTemplate.SmokeGeneratorData != null)
            {
                smokeTimer = new MyTimer();
                smokeTimer.Start();
                smokeTimer.OnTimeout += OnSmokeTimerTimeout;
                smokeTimer.SetInterval(1.5f);
            }

            switch (BuildingTemplate.BuildingType)
            {
                case BuildingType.Wall:
                    {
                        TileInfo tileInfo = TilesInfosArray[0, 0];
                        tileInfo.Tile.IsWalkable = tileInfo.IsWalkable;

                        if (tileInfo.Tile.IsWalkable == false)
                            tileInfo.Tile.StrengthValue = 100;

                        tileInfo.Tile.Entity = Entity;
                    }
                    break;
                default:
                    {
                        Sprite.Color = Color.White;

                        // Устанавливаем проходимость тайлов
                        for (int c = 0; c < TilesInfosArray.GetLength(0); c++)
                        {
                            for (int r = 0; r < TilesInfosArray.GetLength(1); r++)
                            {
                                TileInfo tileInfo = TilesInfosArray[c, r];
                                Tile tile = tileInfo.Tile;

                                tile.IsWalkable = tileInfo.IsWalkable;
                            }
                        }
                    }
                    break;
            }

            TargetTiles.Clear();

            List<Tile> reservedTiles = new List<Tile>();

            // Проверяем, имеет ли строение целевые тайлы
            foreach (var reservedTileInfo in TilesInfosArray)
            {
                Tile tile = reservedTileInfo.Tile;
                bool target = reservedTileInfo.IsTarget;

                reservedTiles.Add(tile);

                if (target)
                {
                    TargetTiles.Add(tile);
                }
            }

            if (TargetTiles.Count == 0)
            {
                foreach (var reservedTileInfo in TilesInfosArray)
                {
                    foreach (var neighTile in reservedTileInfo.Tile.GetNeighbourTiles())
                    {
                        if (!reservedTiles.Contains(neighTile) && !TargetTiles.Contains(neighTile))
                        {
                            TargetTiles.Add(neighTile);
                        }
                    }
                }
            }

            // Удаляем все предметы из Inventory
            Inventory.ClearCargo();

            if (BuildingTemplate.FuelConsumer != null)
            {
                ConsumableFuelDictionary = new Dictionary<Item, bool>();

                foreach (var fuel in BuildingTemplate.FuelConsumer.ConsumableFuel)
                {
                    ConsumableFuelDictionary.Add(fuel, true);
                }

                GameplayScene.WorldManager.FuelConsumerBuildings.Add(this);
            }

            if (BuildingTemplate.LightEmitter != null)
            {
                Tile centralTile = TilesInfosArray[0, 0].Tile;
                int xCenter = centralTile.X;
                int yCenter = centralTile.Y;

                int xOffset = BuildingTemplate.Width / 2;
                int yOffset = BuildingTemplate.Height / 2;

                xCenter += xOffset;
                yCenter += yOffset;

                light = new PointLight
                {
                    Scale = new Vector2(BuildingTemplate.LightEmitter.Radius),
                    ShadowType = ShadowType.Solid,
                    Intensity = 0,
                    Color = Color.Orange
                };

                light.Position = new Vector2(xCenter * Engine.TILE_SIZE + Engine.TILE_SIZE / 2,
                    yCenter * Engine.TILE_SIZE + Engine.TILE_SIZE / 2);

                GameplayScene.Instance.Penumbra.Lights.Add(light);

                flickeringTimer = new Timer();

                smoothQueue = new Queue<float>(5);
            }

            popUpPositionX = (int)(Entity.X + ((BuildingTemplate.Width * Engine.TILE_SIZE) / 2 - Engine.TILE_SIZE / 2));
            popUpPositionY = (int)(Entity.Y - Engine.TILE_SIZE / 2);

            if (BuildingTemplate.EnergyConsumer != null)
            {
                GameplayScene.Instance.EnergyManager.AddEnergyConsumer(this);
            }

            CenterPosition = new Vector2(Entity.X + (BuildingTemplate.Width * Engine.TILE_SIZE) / 2,
            Entity.Y + (BuildingTemplate.Height * Engine.TILE_SIZE) / 2);

            if (BuildingTemplate.Range > 0)
            {
                RangeTiles = TryToGetRangeTiles();
            }

            OnBuildingCompletedCallback?.Invoke(this);
        }

        public override void CompleteInteraction(InteractionType interactionType)
        {
            base.CompleteInteraction(interactionType);

            switch (interactionType)
            {
                case InteractionType.Destruct:
                    {
                        DestructBuilding();
                    }
                    break;
            }
        }

        private HashSet<Tile> TryToGetRangeTiles()
        {
            HashSet<Tile> tiles = new HashSet<Tile>();

            foreach (var tileInfo in TilesInfosList)
            {
                Tile centerTile = tileInfo.Tile;

                foreach (var rangeTile in Utils.GetTilesInCircle(centerTile, BuildingTemplate.Range))
                {
                    tiles.Add(rangeTile);
                }
            }

            foreach (var tileInfo in TilesInfosList)
            {
                Tile centerTile = tileInfo.Tile;

                tiles.Remove(centerTile);
            }

            return tiles;
        }

        public void CancelBuilding()
        {
            IsBuilt = false;

            // Если строительство уже начато
            if (buildLabor != null)
            {
                buildLabor.CancelAndClearAllTasksAndComplete();
                buildLabor = null;
            }

            RemoveBuildingFromTiles();

            ThrowAllItems();

            if (BuildingTemplate.BuildingType != BuildingType.Wall)
            {
                Entity.RemoveSelf();
            }
            else
            {
                GameplayScene.WorldManager.WallsList.Remove(this);
            }

            ClearInventoryRequiredWeight();

            OnBuildingCanceledCallback?.Invoke(this);

            Inventory.OnItemAddedCallback -= OnItemAdded;
            Inventory.OnItemRemovedCallback -= OnItemRemoved;
        }

        public virtual void DestructBuilding()
        {
            Destroy();

            if (smokeTimer != null)
            {
                smokeTimer.OnTimeout -= OnSmokeTimerTimeout;
            }

            foreach (var tileInfo in TilesInfosList)
            {
                tileInfo.Tile.MarkType = MarkType.None;
            }

            IsBuilt = false;

            if (BuildingTemplate.LightEmitter != null)
            {
                GameplayScene.Instance.Penumbra.Lights.Remove(light);
            }

            RemoveBuildingFromTiles();

            // Выкидываем все вложенные предметы
            ThrowAllItems();

            if (ThrowBuildingRecipeItemsAfterDestructing)
            {
                ThrowItems(BuildingTemplate.BuildingRecipe);
            }

            if (BuildingTemplate.BuildingType != BuildingType.Wall)
            {
                Entity.RemoveSelf();
            }
            else
            {
                GameplayScene.WorldManager.WallsList.Remove(this);
            }

            ClearInventoryRequiredWeight();

            if (BuildingTemplate.FuelConsumer != null)
            {
                GameplayScene.WorldManager.FuelConsumerBuildings.Remove(this);
            }

            if (BuildingTemplate.EnergyConsumer != null)
            {
                GameplayScene.Instance.EnergyManager.RemoveEnergyConsumer(this);
            }

            OnBuildingDestructedCallback?.Invoke(this);

            Inventory.OnItemAddedCallback -= OnItemAdded;
            Inventory.OnItemRemovedCallback -= OnItemRemoved;
        }

        private void ClearInventoryRequiredWeight()
        {
            List<Item> keys = new List<Item>(Inventory.InventoryRequired.Keys);

            foreach (var keyItem in keys)
            {
                int requiredWeight = Inventory.InventoryRequired[keyItem];

                if (requiredWeight > 0)
                {
                    Inventory.AddRequiredWeight(keyItem, -requiredWeight);
                }
            }
        }

        protected void ThrowAllItems()
        {
            Inventory.ThrowCargo(TilesInfosArray[0, 0].Tile);
        }

        private void RemoveBuildingFromTiles()
        {
            // Удаляем строение из тайлов
            for (int c = 0; c < TilesInfosArray.GetLength(0); c++)
            {
                for (int r = 0; r < TilesInfosArray.GetLength(1); r++)
                {

                    Tile tile = TilesInfosArray[c, r].Tile;

                    switch (BuildingTemplate.BuildingType)
                    {
                        case BuildingType.Wall:
                            {
                                tile.Entity = null;
                                tile.IsWalkable = true;
                                tile.StrengthValue = 0;
                            }
                            break;
                        default:
                            {
                                tile.Entity = null;
                                tile.IsWalkable = true;
                            }
                            break;
                    }
                }
            }
        }

        protected void ThrowItems(Dictionary<Item, int> items)
        {
            foreach (var kvp in items)
            {
                Item item = kvp.Key;
                int weight = kvp.Value;

                if (weight > 0)
                {
                    TilesInfosArray[0, 0].Tile.Inventory.AddCargo(new ItemContainer(item, weight, item.Durability));
                }
            }
        }

        public override Tile GetApproachableTile(CreatureCmp creature)
        {
            for (int i = 0; i < TargetTiles.Count; ++i)
            {
                Tile tile = TargetTiles[i];

                if (tile.IsWalkable && creature.Movement.CurrentTile.Room.Id == tile.Room.Id)
                    return tile;
            }

            return null;
        }

        public override Tile GetApproachableTile(int zoneId)
        {
            for (int i = 0; i < TargetTiles.Count; ++i)
            {
                Tile tile = TargetTiles[i];

                if (tile.IsWalkable == false) continue;

                if(tile.Room.Id != zoneId) continue;

                return tile;
            }

            return null;
        }

        public override Tile GetApproachableTile()
        {
            for (int i = 0; i < TargetTiles.Count; ++i)
            {
                Tile tile = TargetTiles[i];

                if (tile.IsWalkable)
                    return tile;
            }

            return null;
        }

        public override IEnumerable<Tile> GetApproachableTiles()
        {
            for (int i = 0; i < TargetTiles.Count; ++i)
            {
                Tile tile = TargetTiles[i];

                if (tile.IsWalkable == false) continue;

                yield return tile;
            }
        }

        // TODO: переделать
        public Tile GetCenterTile()
        {
            return TilesInfosArray[0, 0].Tile;
        }

        public Tile GetRealCenterTile()
        {
            switch(Direction)
            {
                case Direction.LEFT:
                case Direction.RIGHT:
                    return TilesInfosArray[(int)(BuildingTemplate.Height / 2), (int)(BuildingTemplate.Width / 2)].Tile;
                case Direction.UP: 
                case Direction.DOWN:
                    return TilesInfosArray[(int)(BuildingTemplate.Width / 2), (int)(BuildingTemplate.Height / 2)].Tile;
            }

            return null;
        }

        public bool IsFuelAllowed(Item fuel)
        {
            return ConsumableFuelDictionary[fuel];
        }

        public void SetFuelFilter(Item fuel, bool allow)
        {
            ConsumableFuelDictionary[fuel] = allow;
        }

        public virtual string GetInformation()
        {
            if (IsBuilt)
            {
                string info = "";

                if (BuildingTemplate.FuelConsumer != null)
                {
                    info += $"- {fuelString}: {string.Format("{0:0.00}", CurrentFuelCondition)} MJ\n" +
                        $"- {consumptionString}: {BuildingTemplate.FuelConsumer.EnergyConsumption} MJ/s\n\n";
                }

                if (BuildingTemplate.EnergyConsumer != null)
                {
                    info += $"\n{Localization.GetLocalizedText("energy_type")}: " +
                        $"{Localization.GetLocalizedText(BuildingTemplate.EnergyConsumer.EnergyType.ToString().ToLower())}";

                    info += $"\n{Localization.GetLocalizedText("energy_consumption")}: {BuildingTemplate.EnergyConsumer.RequiredPower}";

                    if (IsPowered)
                    {
                        info += $"\n{Localization.GetLocalizedText("energy_source")}: " +
                            $"/c[#00FF00]{Localization.GetLocalizedText("connected")}/cd\n";
                    }
                    else
                    {
                        info += $"\n{Localization.GetLocalizedText("energy_source")}: " +
                            $"/c[#FF0000]{Localization.GetLocalizedText("disconnected")}/cd\n";
                    }
                }

                return info;
            }
            else
            {
                string info = $"{Localization.GetLocalizedText("ingredients")}:\n";

                var buildingRecipe = BuildingTemplate.BuildingRecipe;

                foreach (var kvp in buildingRecipe)
                {
                    Item item = kvp.Key;

                    int requiredWeight = kvp.Value;

                    int factWeight = Inventory.GetInventoryFactWeight(item);

                    info += $"- {item.Name}: {factWeight} / {requiredWeight}\n";
                }

                return info + "\n";
            }
        }

        public void SupplyFuel(Item item)
        {
            if (item == null)
                return;

            if (item.Consumable == null)
                return;

            if (item.Consumable.Statistics.ContainsKey(AttributeType.Fuel) == false)
                return;

            CurrentFuelCondition = item.Consumable.Statistics[AttributeType.Fuel];
        }

        public virtual BuildingSaveData GetSaveData()
        {
            BuildingSaveData buildingSaveData = new BuildingSaveData();

            // TODO: Interactable needs its own method for getting saves
            buildingSaveData.MarkedInteractions = new List<InteractionType>();
            buildingSaveData.InteractionPercentProgressDict = new Dictionary<InteractionType, float>();
            foreach(InteractionType interactionType in AvailableInteractions)
            {
                if(IsInteractionMarked(interactionType))
                {
                    buildingSaveData.MarkedInteractions.Add(interactionType);
                }

                float interactionProgress = GetInteractionProgressPercent(interactionType);
                if (interactionProgress > 0)
                {
                    buildingSaveData.InteractionPercentProgressDict.Add(interactionType, interactionProgress);
                }
            }

            buildingSaveData.BuildingTemplateName = BuildingTemplate.Json;
            buildingSaveData.BuildingProgress = ConstructionProgress;

            buildingSaveData.CurrentFuelCondition = CurrentFuelCondition;

            if (ConsumableFuelDictionary != null)
            {
                buildingSaveData.ConsumableFuel = new Dictionary<int, bool>();

                foreach (var kvp in ConsumableFuelDictionary)
                {
                    buildingSaveData.ConsumableFuel.Add(kvp.Key.Id, kvp.Value);
                }
            }

            buildingSaveData.Tiles = new Point[tiles.GetLength(0), tiles.GetLength(1)];

            // Инициализируем Tile info и назначаем entity для каждого тайла
            for (int column = 0; column < tiles.GetLength(0); column++)
            {
                for (int row = 0; row < tiles.GetLength(1); row++)
                {
                    buildingSaveData.Tiles[column, row] = tiles[column, row].GetAsPoint();
                }
            }

            buildingSaveData.IsBuilt = IsBuilt;
            buildingSaveData.Direction = Direction;

            buildingSaveData.InventoryItems = new List<Tuple<int, int, float>>();
            foreach (var kvp in Inventory.ItemItemContainerPair)
            {
                List<ItemContainer> itemContainers = kvp.Value;

                foreach (var itemContainer in itemContainers)
                {
                    buildingSaveData.InventoryItems.Add(Tuple.Create(itemContainer.Item.Id, itemContainer.FactWeight, itemContainer.Durability));
                }
            }

            if (buildingSaveData.InventoryItems.Count == 0)
            {
                buildingSaveData.InventoryItems = null;
            }

            return buildingSaveData;
        }
    }
}
