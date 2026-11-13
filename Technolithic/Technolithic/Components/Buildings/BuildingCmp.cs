using Microsoft.Xna.Framework;
using Penumbra;
using System;
using System.Collections.Generic;

namespace Technolithic
{


    public class BuildingCmp : Component, ISupplyTarget
    {
        public AnimatedSprite Sprite { get; private set; }

        public BuildingPlacement Placement { get; }
        public TileInfo[,] TilesInfosArray => Placement.TilesInfosArray;
        public List<TileInfo> TilesInfosList => Placement.TilesInfosList;
        public HashSet<Tile> RangeTiles => Placement.RangeTiles;

        public bool IsCompleted { get; private set; }
        public bool IsDestroyed { get; private set; }

        public Action<BuildingCmp> OnBuildingDestructedCallback { get; set; }
        public Action<ISupplyTarget> OnRemovedCallback { get; set; }

        public float DeconstructionProgress { get; set; }

        public Inventory Inventory { get; private set; }

        private bool destruct;
        public bool Destruct
        {
            get { return destruct; }
            set
            {
                if (destruct == value)
                    return;

                destruct = value;

                if (destruct)
                {
                    if (BuildingTemplate.RequireBuilding)
                    {
                        Placement.SetMarkType(MarkType.Destruct);

                        AddBuildingToBuildingsToDestruct();
                    }
                    else
                    {
                        DestructBuilding();
                    }
                }
                else
                {
                    Placement.SetMarkType(MarkType.None);

                    RemoveBuildingFromBuildingsToDestruct();
                }
            }
        }

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
        public Direction Direction => Placement.Direction;

        private int popUpPositionX;
        private int popUpPositionY;

        public Vector2 CenterPosition { get; private set; }

        private string fuelString;
        private string consumptionString;

        public bool ReservedToSupplyFuel { get; set; }
        public bool ReservedToDestruct { get; set; }

        public bool IsReserved { get; set; }

        protected virtual bool RefundsRecipeOnDestruct => true;

        public virtual bool IsTileWalkable(TileInfo tileInfo) => tileInfo.IsWalkable;

        public bool IsPowered { get; set; } = true;

        private MyTimer smokeTimer;

        private float gearRotation = 0;

        public BuildingCmp(BuildingTemplate buildingTemplate, Direction direction) : base(true, true)
        {
            BuildingTemplate = buildingTemplate;
            Placement = new BuildingPlacement(buildingTemplate, direction);

            fuelString = Localization.GetLocalizedText("fuel");
            consumptionString = Localization.GetLocalizedText("consumption");

            Inventory = new Inventory(this);
            Inventory.IsStorage = false;

            Sprite = BuildingSprite.Create(buildingTemplate, Direction);
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

        public override void Update()
        {
            Sprite?.Update();

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
            if (IsDestroyed)
                return;

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

                        centerTile.World.TryDoForTilesInRadius(
                            centerTile.X,
                            centerTile.Y,
                            (int)(BuildingTemplate.LightEmitter.Radius / 33.33f),
                            tile => tile.IlluminationStrength++);
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

                        centerTile.World.TryDoForTilesInRadius(
                            centerTile.X,
                            centerTile.Y,
                            (int)(BuildingTemplate.LightEmitter.Radius / 33.33f),
                            tile => tile.IlluminationStrength--);
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

        public override void Render()
        {
            Sprite?.Render();

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

        public void Place(Tile[,] tiles, float currentFuelCondition)
        {
            Entity.X = tiles[0, 0].X * Engine.TILE_SIZE;
            Entity.Y = tiles[0, 0].Y * Engine.TILE_SIZE;

            CurrentFuelCondition = currentFuelCondition;

            Placement.Place(tiles, Entity);

            if (Sprite != null)
            {
                BuildingSprite.AttachTo(Sprite, Entity, Placement);
            }

            Inventory.OnItemAddedCallback += OnItemAdded;
            Inventory.OnItemRemovedCallback += OnItemRemoved;
        }

        protected virtual void OnItemAdded(Inventory senderInventory, Item item, int weight)
        {
            if (Inventory.InventoryRequired.ContainsKey(item))
            {
                Inventory.AddRequiredWeight(item, -weight);
            }
        }

        protected virtual void OnItemRemoved(Inventory senderInventory, Item item, int weight)
        {

        }

        private void OnSmokeTimerTimeout()
        {
            Vector2 smokePosition = Entity.Position + BuildingTemplate.SmokeGeneratorData.SpawnPosition;
            GameplayScene.Instance.SmokeManager.AddSmoke(smokePosition);
        }

        public virtual void CompleteBuilding()
        {
            IsCompleted = true;

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
                                tile.UpdateMovementSpeedPercent();
                            }
                        }
                    }
                    break;
            }

            Placement.UseTargetPatternAsTargets();

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
        }

        public virtual void DestructBuilding()
        {
            if (smokeTimer != null)
            {
                smokeTimer.OnTimeout -= OnSmokeTimerTimeout;
            }

            Placement.SetMarkType(MarkType.None);

            RemoveBuildingFromBuildingsToDestruct();

            IsDestroyed = true;

            Inventory.OnItemAddedCallback -= OnItemAdded;
            Inventory.OnItemRemovedCallback -= OnItemRemoved;

            if (BuildingTemplate.LightEmitter != null)
            {
                GameplayScene.Instance.Penumbra.Lights.Remove(light);

                if (wasTurnedOn)
                {
                    Tile centerTile = GetRealCenterTile();

                    centerTile.World.TryDoForTilesInRadius(
                        centerTile.X,
                        centerTile.Y,
                        (int)(BuildingTemplate.LightEmitter.Radius / 33.33f),
                        tile => tile.IlluminationStrength--);
                }
            }

            Placement.Release();

            // Выкидываем все вложенные предметы
            ThrowAllItems();

            if (RefundsRecipeOnDestruct)
            {
                ThrowItems(BuildingTemplate.BuildingRecipe);
            }

            if (BuildingTemplate.BuildingType != BuildingType.Wall)
            {
                Entity.RemoveSelf();
            }
            else
            {
                GameplayScene.WorldManager.WallsList.Remove(Entity);
            }

            Inventory.ClearRequiredWeight();

            if (BuildingTemplate.FuelConsumer != null)
            {
                GameplayScene.WorldManager.FuelConsumerBuildings.Remove(this);
            }

            if (BuildingTemplate.EnergyConsumer != null)
            {
                GameplayScene.Instance.EnergyManager.RemoveEnergyConsumer(this);
            }

            OnBuildingDestructedCallback?.Invoke(this);
            OnRemovedCallback?.Invoke(this);
        }

        protected void ThrowAllItems()
        {
            Inventory.ThrowCargo(TilesInfosArray[0, 0].Tile);
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

        public Tile GetReachableTile(CreatureCmp creature) => Placement.GetReachableTile(creature);

        public Tile GetReachableTile() => Placement.GetReachableTile();

        public Tile GetCenterTile() => Placement.GetCenterTile();

        public Tile GetRealCenterTile() => Placement.GetRealCenterTile();

        Tile IApproachable.GetApproachableTile(CreatureCmp creature) => Placement.GetReachableTile(creature);

        Room IApproachable.GetRoom() => Placement.GetRoom();

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

        public bool IsCanBeAchieved() => Placement.IsCanBeAchieved();

        public void AddBuildingToBuildingsToDestruct()
        {
            if (BuildingTemplate.BuildingType == BuildingType.Wall || BuildingTemplate.BuildingType == BuildingType.Gate)
            {
                Tile centerTile = GetCenterTile();

                Tile leftTile = centerTile.LeftTile;
                int leftTileRoomId = -1;
                if (leftTile != null && leftTile.Room != null)
                {
                    leftTileRoomId = leftTile.Room.Id;
                    if (GameplayScene.WorldManager.BuildingsToDestruct[leftTileRoomId].Contains(this) == false)
                    {
                        GameplayScene.WorldManager.BuildingsToDestruct[leftTileRoomId].Add(this);
                    }
                }

                Tile rightTile = centerTile.RightTile;
                int rightTileRoomId = -1;
                if (rightTile != null && rightTile.Room != null)
                {
                    rightTileRoomId = rightTile.Room.Id;
                    if (GameplayScene.WorldManager.BuildingsToDestruct[rightTileRoomId].Contains(this) == false)
                    {
                        GameplayScene.WorldManager.BuildingsToDestruct[rightTileRoomId].Add(this);
                    }
                }

                Tile topTile = centerTile.TopTile;
                int topTileRoomId = -1;
                if (topTile != null && topTile.Room != null)
                {
                    topTileRoomId = topTile.Room.Id;
                    if (GameplayScene.WorldManager.BuildingsToDestruct[topTileRoomId].Contains(this) == false)
                    {
                        GameplayScene.WorldManager.BuildingsToDestruct[topTileRoomId].Add(this);
                    }
                }

                Tile bottomTile = centerTile.BottomTile;
                int bottomTimeRoomId = -1;
                if (bottomTile != null && bottomTile.Room != null)
                {
                    bottomTimeRoomId = bottomTile.Room.Id;
                    if (GameplayScene.WorldManager.BuildingsToDestruct[bottomTimeRoomId].Contains(this) == false)
                    {
                        GameplayScene.WorldManager.BuildingsToDestruct[bottomTimeRoomId].Add(this);
                    }
                }

                if (leftTileRoomId == -1 && rightTileRoomId == -1 && topTileRoomId == -1 && bottomTimeRoomId == -1)
                {
                    GameplayScene.WorldManager.BuildingsToDestruct[-1].Add(this);
                }
            }
            else
            {
                int roomId = GetCenterTile().Room.Id;
                GameplayScene.WorldManager.BuildingsToDestruct[roomId].Add(this);
            }
        }

        public void RemoveBuildingFromBuildingsToDestruct()
        {
            if (BuildingTemplate.BuildingType == BuildingType.Wall || BuildingTemplate.BuildingType == BuildingType.Gate)
            {
                Tile centerTile = GetCenterTile();

                Tile leftTile = centerTile.LeftTile;
                int leftTileRoomId = -1;
                if (leftTile != null && leftTile.Room != null)
                {
                    leftTileRoomId = leftTile.Room.Id;
                    GameplayScene.WorldManager.BuildingsToDestruct[leftTileRoomId].Remove(this);
                }

                Tile rightTile = centerTile.RightTile;
                int rightTileRoomId = -1;
                if (rightTile != null && rightTile.Room != null)
                {
                    rightTileRoomId = rightTile.Room.Id;
                    GameplayScene.WorldManager.BuildingsToDestruct[rightTileRoomId].Remove(this);
                }

                Tile topTile = centerTile.TopTile;
                int topTileRoomId = -1;
                if (topTile != null && topTile.Room != null)
                {
                    topTileRoomId = topTile.Room.Id;
                    GameplayScene.WorldManager.BuildingsToDestruct[topTileRoomId].Remove(this);
                }

                Tile bottomTile = centerTile.BottomTile;
                int bottomTileRoomId = -1;
                if (bottomTile != null && bottomTile.Room != null)
                {
                    bottomTileRoomId = bottomTile.Room.Id;
                    GameplayScene.WorldManager.BuildingsToDestruct[bottomTileRoomId].Remove(this);
                }

                if (leftTileRoomId == -1 && rightTileRoomId == -1 && topTileRoomId == -1 && bottomTileRoomId == -1)
                {
                    GameplayScene.WorldManager.BuildingsToDestruct[-1].Remove(this);
                }
            }
            else
            {
                int roomId = GetCenterTile().Room.Id;
                GameplayScene.WorldManager.BuildingsToDestruct[roomId].Remove(this);
            }
        }

        public virtual BuildingSaveData GetSaveData()
        {
            BuildingSaveData buildingSaveData = new BuildingSaveData();
            buildingSaveData.BuildingTemplateName = BuildingTemplate.Json;
            buildingSaveData.DestructingProgress = DeconstructionProgress;
            buildingSaveData.Destruct = Destruct;

            buildingSaveData.CurrentFuelCondition = CurrentFuelCondition;

            if (ConsumableFuelDictionary != null)
            {
                buildingSaveData.ConsumableFuel = new Dictionary<int, bool>();

                foreach (var kvp in ConsumableFuelDictionary)
                {
                    buildingSaveData.ConsumableFuel.Add(kvp.Key.Id, kvp.Value);
                }
            }

            buildingSaveData.Tiles = Placement.GetTilesAsPoints();

            buildingSaveData.IsBuilt = true;
            buildingSaveData.Direction = Direction;

            buildingSaveData.InventoryItems = Inventory.GetItemsSaveData();

            return buildingSaveData;
        }
    }
}
