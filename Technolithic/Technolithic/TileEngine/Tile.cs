using Microsoft.Xna.Framework;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Technolithic
{

    public enum GroundType
    {
        Ground = 0,
        Grass = 1,
        FarmPlot = 2,
        FarmPlotFertilized = 3,
        FarmPlotWet = 4,
        FarmPlotWetFertilized = 5,
    }

    public enum GroundTopType
    {
        Water = 0,
        DeepWater = 1,
        Stone = 2,
        Clay = 3,
        Iron = 5,
        Tin = 6,
        Copper = 7,
        None = 8,
        IrrigationCanalEmpty = 13,
        IrrigationCanalFull = 14
    }

    public class Tile
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        private World world;

        private bool isWalkable = true;
        public bool IsWalkable
        {
            get { return isWalkable; }
            set
            {
                if (isWalkable == value)
                    return;

                isWalkable = value;

                Chunk.UpdateTile(this);

                GameplayScene.WorldManager.CbOnTileWalkableChanged(this);
            }
        }

        public Tile LeftTile { get; set; }
        public Tile RightTile { get; set; }
        public Tile TopTile { get; set; }
        public Tile BottomTile { get; set; }

        public Tile LeftTopTile { get; set; }
        public Tile LeftBottomTile { get; set; }
        public Tile RightTopTile { get; set; }
        public Tile RightBottomTile { get; set; }

        private List<Tile> neighbourTiles;
        private List<Tile> allNeighbourTiles;

        private TileMap summerGroundTileMap;
        private TileMap autumnGroundTileMap;
        private TileMap winterGroundTileMap;
        private TileMap springGroundTileMap;
        private TileMap groundTopTileMap;
        private TileMap surfaceTileMap;
        private TileMap blockTileMap;
        private TileMap itemTileMap;

        private Room room;
        public Room Room 
        { 
            get { return room; } 
            set { room = value; }
        }

        public MChunk Chunk { get; internal set; }
        public WaterChunk WaterChunk { get; set; }

        private GroundType groundType = GroundType.Ground;
        public GroundType GroundType
        {
            get { return groundType; }
            set
            {
                groundType = value;

                UpdateGround(this);

                foreach (Tile tile in GetAllNeighbourTiles())
                    UpdateGround(tile);

                UpdateMovementSpeedPercent();
            }
        }

        private GroundTopType groundTopType = GroundTopType.None;
        public GroundTopType GroundTopType
        {
            get { return groundTopType; }
            set
            {
                if (groundTopType == value)
                    return;

                GroundTopType lastGroundTopType = groundTopType;
                groundTopType = value;

                UpdateGroundTop(this);

                foreach (Tile tile in GetAllNeighbourTiles())
                    UpdateGroundTop(tile);

                switch(groundTopType)
                {
                    case GroundTopType.Water:
                        {
                            if (lastGroundTopType != GroundTopType.Water)
                            {
                                IrrigationLevel++;

                                foreach (var tile in world.GetTilesWithinRadius(this, 3))
                                {
                                    tile.IrrigationLevel++;
                                }
                            }
                        }
                        break;
                    case GroundTopType.IrrigationCanalFull:
                        {
                            if (lastGroundTopType != GroundTopType.IrrigationCanalFull)
                            {
                                IrrigationLevel++;

                                foreach (var tile in world.GetTilesWithinRadius(this, 3))
                                {
                                    tile.IrrigationLevel++;
                                }
                            }
                        }
                        break;
                    default:
                        {
                            if (lastGroundTopType == GroundTopType.Water || lastGroundTopType == GroundTopType.IrrigationCanalFull)
                            {
                                IrrigationLevel--;

                                foreach (var tile in world.GetTilesWithinRadius(this, 3))
                                {
                                    tile.IrrigationLevel--;
                                }
                            }
                        }
                        break;
                }

                UpdateMovementSpeedPercent();
            }
        }

        public int SurfaceId
        {
            get => surfaceTileMap.GetCellTerrainId(X, Y);
            set 
            { 
                surfaceTileMap.SetCellTerrainId(X, Y, value);

                UpdateMovementSpeedPercent();
            }
        }

        private Hull hull;

        private Entity entity;
        public Entity Entity 
        { 
            get { return entity; } 
            set
            {
                entity = value;

                OnEntityChangedCallback?.Invoke(entity);

                if (entity != null && entity.Get<WallCmp>() != null)
                {
                    if (entity.Get<BuildingCmp>().IsBuilt && !entity.Get<BuildingCmp>().BuildingTemplate.LetLight)
                    {
                        hull = new Hull(new Vector2(0.5f), new Vector2(-0.5f, 0.5f), new Vector2(-0.5f), new Vector2(0.5f, -0.5f))
                        {
                            Position = new Vector2(X * Engine.TILE_SIZE + Engine.TILE_SIZE / 2, 
                            Y * Engine.TILE_SIZE + Engine.TILE_SIZE / 2),
                            Scale = new Vector2(Engine.TILE_SIZE)
                        };

                        GameplayScene.Instance.Penumbra.Hulls.Add(hull);
                    }
                }
                else
                {
                    if (hull != null)
                        GameplayScene.Instance.Penumbra.Hulls.Remove(hull);

                    hull = null;
                }

                UpdateBlock(this);

                foreach (Tile tile in GetAllNeighbourTiles())
                    UpdateBlock(tile);

                if (entity == null || entity.Get<BuildingCmp>().IsBuilt)
                    UpdateMovementSpeedPercent();
            }
        }

        private PlantData plantData;
        public PlantData PlantData 
        { 
            get { return plantData; }
            set
            {
                PlantData previous = plantData;
                
                plantData = value;

                PlantDataChanged?.Invoke(previous, plantData);
            }
        }

        public Action<PlantData, PlantData> PlantDataChanged { get; set; }

        public int MovementSpeedPercent { get; private set; }
        public int StrengthValue { get; set; }
        public Action<Entity> OnEntityChangedCallback { get; set; }

        public Inventory Inventory { get; private set; }

        public CreatureCmp OccupiedBy { get; set; }

        public int HomeArea { get; set; } = 0;

        public int IrrigationLevel { get; set; }

        private Point point;
        private Vector2 vector;

        public Dictionary<EnergyType, List<EnergySourceCmp>> EnergySources { get; private set; } = new Dictionary<EnergyType, List<EnergySourceCmp>>();

        public float MoistureLevel { get; set; }
        public float FertilizerLevel { get; set; }


        public int IrrigationStrength { get; set; }

        public bool IsIlluminated { get; set; } = false;

        public Tile(int x, int y, TileMap summerGroundTileMap, TileMap autumnGroundTileMap, TileMap winterGroundTileMap, TileMap springGroundTileMap, TileMap groundTopTileMap, TileMap surfaceTileMap, TileMap blockTileMap, 
            TileMap itemTileMap, World world)
        {
            X = x;
            Y = y;
            this.world = world;

            point = new Point(X, Y);
            vector = new Vector2(X, Y);

            this.summerGroundTileMap = summerGroundTileMap;
            this.autumnGroundTileMap = autumnGroundTileMap;
            this.winterGroundTileMap = winterGroundTileMap;
            this.springGroundTileMap = springGroundTileMap;
            this.groundTopTileMap = groundTopTileMap;
            this.surfaceTileMap = surfaceTileMap;
            this.blockTileMap = blockTileMap;
            this.itemTileMap = itemTileMap;

            Inventory = new Inventory(this);
            Inventory.IsStorage = true;
            Inventory.OnItemAddedCallback += OnItemAddedCallback;
            Inventory.OnItemRemovedCallback += OnItemRemovedCallback;

            foreach (EnergyType energyType in Enum.GetValues(typeof(EnergyType)))
            {
                EnergySources.Add(energyType, new List<EnergySourceCmp>());
            }
        }

        private bool CheckGroundTopBaseAndNeighbourTiles(Tile baseTile, Tile neighbour)
        {
            if (neighbour == null)
                return false;

            if (baseTile.GroundTopType == neighbour.GroundTopType)
                return true;

            if (baseTile.groundTopType == GroundTopType.DeepWater)
                return false;

            if (baseTile.GroundTopType == GroundTopType.Water && neighbour.GroundTopType == GroundTopType.DeepWater)
                return true;

            if (baseTile.GroundTopType == GroundTopType.IrrigationCanalFull && neighbour.GroundTopType == GroundTopType.Water)
                return true;

            if (baseTile.GroundTopType == GroundTopType.IrrigationCanalEmpty && neighbour.GroundTopType == GroundTopType.Water)
                return true;

            if (baseTile.GroundTopType == GroundTopType.IrrigationCanalEmpty && neighbour.GroundTopType == GroundTopType.IrrigationCanalFull)
                return true;

            if (baseTile.GroundTopType == GroundTopType.IrrigationCanalFull && neighbour.GroundTopType == GroundTopType.IrrigationCanalEmpty)
                return true;

            return false;
        }

        private void UpdateGround(Tile tile)
        {
            int leftTop = tile.LeftTopTile != null && tile.LeftTopTile.GroundType == tile.GroundType ? 1 : 0;

            int top = tile.TopTile != null && tile.TopTile.GroundType == tile.GroundType ? 2 : 0;

            int rightTop = tile.RightTopTile != null && tile.RightTopTile.GroundType == tile.GroundType ? 4 : 0;

            int left = tile.LeftTile != null && tile.LeftTile.GroundType == tile.GroundType ? 8 : 0;

            int right = tile.RightTile != null && tile.RightTile.GroundType == tile.GroundType ? 16 : 0;

            int leftBottom = tile.LeftBottomTile != null && tile.LeftBottomTile.GroundType == tile.GroundType ? 32 : 0;

            int bottom = tile.BottomTile != null && tile.BottomTile.GroundType == tile.GroundType ? 64 : 0;

            int rightBottom = tile.RightBottomTile != null && tile.RightBottomTile.GroundType == tile.GroundType ? 128 : 0;

            int bitmask = leftTop + top + rightTop + left + right + leftBottom + bottom + rightBottom;

            if (tile.GroundType == GroundType.Grass)
            {
                summerGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 0, 10));
                autumnGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 0, 25));
                winterGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 11, 20));
                springGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 11, 25));
            }
            else if (tile.GroundType == GroundType.FarmPlot)
            {
                summerGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 0));
                autumnGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 0));
                winterGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 0));
                springGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 0));
            }
            else if (tile.GroundType == GroundType.FarmPlotFertilized)
            {
                summerGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 10));
                autumnGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 10));
                winterGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 10));
                springGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 10));
            }
            else if (tile.GroundType == GroundType.FarmPlotWet)
            {
                summerGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 5));
                autumnGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 5));
                winterGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 5));
                springGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 5));
            }
            else if (tile.GroundType == GroundType.FarmPlotWetFertilized)
            {
                summerGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 15));
                autumnGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 15));
                winterGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 15));
                springGroundTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 15));
            }
            else
            {
                summerGroundTileMap.SetCell(tile.X, tile.Y, 992);
                autumnGroundTileMap.SetCell(tile.X, tile.Y, 992);
                winterGroundTileMap.SetCell(tile.X, tile.Y, 960);
                springGroundTileMap.SetCell(tile.X, tile.Y, 992);
            }
        }

        private void UpdateGroundTop(Tile tile)
        {
            if (tile.GroundTopType != GroundTopType.None)
            {
                int leftTop = CheckGroundTopBaseAndNeighbourTiles(tile, tile.LeftTopTile) ? 1 : 0;

                int top = CheckGroundTopBaseAndNeighbourTiles(tile, tile.TopTile) ? 2 : 0;

                int rightTop = CheckGroundTopBaseAndNeighbourTiles(tile, tile.RightTopTile) ? 4 : 0;

                int left = CheckGroundTopBaseAndNeighbourTiles(tile, tile.LeftTile) ? 8 : 0;

                int right = CheckGroundTopBaseAndNeighbourTiles(tile, tile.RightTile) ? 16 : 0;

                int leftBottom = CheckGroundTopBaseAndNeighbourTiles(tile, tile.LeftBottomTile) ? 32 : 0;

                int bottom = CheckGroundTopBaseAndNeighbourTiles(tile, tile.BottomTile) ? 64 : 0;

                int rightBottom = CheckGroundTopBaseAndNeighbourTiles(tile, tile.RightBottomTile) ? 128 : 0;

                int bitmask = leftTop + top + rightTop + left + right + leftBottom + bottom + rightBottom;

                if (tile.GroundTopType == GroundTopType.Water)
                {
                    groundTopTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 0, 0));
                }
                else if(tile.GroundTopType == GroundTopType.DeepWater)
                {
                    groundTopTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 11, 15));
                }
                else if(tile.GroundTopType == GroundTopType.Stone)
                {
                    groundTopTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 0, 5));
                }
                else if(tile.GroundTopType == GroundTopType.Clay)
                {
                    groundTopTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 0, 15));
                }
                else if(tile.GroundTopType == GroundTopType.Iron)
                {
                    groundTopTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 11, 0));
                }
                else if (tile.GroundTopType == GroundTopType.Tin)
                {
                    groundTopTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 11, 5));
                }
                else if (tile.GroundTopType == GroundTopType.Copper)
                {
                    groundTopTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 11, 10));
                }
                else if(tile.GroundTopType == GroundTopType.IrrigationCanalEmpty)
                {
                    groundTopTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 20));

                }
                else if(tile.GroundTopType == GroundTopType.IrrigationCanalFull)
                {
                    groundTopTileMap.SetCell(tile.X, tile.Y, BitmaskGenerator.GetTileNumber(bitmask, 512, Engine.TILE_SIZE, 22, 25));
                }
            }
            else
            {
                groundTopTileMap.SetCell(tile.X, tile.Y, 0);
            }
        }
     
        private void UpdateBlock(Tile tile)
        {
            BuildingCmp building = tile?.Entity?.Get<BuildingCmp>();

            if (building != null && (building.BuildingTemplate.BuildingType == BuildingType.Wall))
            {
                int top;
                int left;
                int right;
                int bottom;

                if (building.BuildingTemplate.ConnectWithOtherBlocks == true)
                {
                    BuildingCmp topBuilding = tile.TopTile?.Entity?.Get<BuildingCmp>();
                    top = (topBuilding != null && (topBuilding.BuildingTemplate.BuildingType == BuildingType.Wall) 
                        && topBuilding.BuildingTemplate.ConnectWithOtherBlocks) ? 1 : 0;

                    BuildingCmp leftBuilding = tile.LeftTile?.Entity?.Get<BuildingCmp>();
                    left = (leftBuilding != null && (leftBuilding.BuildingTemplate.BuildingType == BuildingType.Wall) 
                        && leftBuilding.BuildingTemplate.ConnectWithOtherBlocks) ? 2 : 0;

                    BuildingCmp rightBuilding = tile.RightTile?.Entity?.Get<BuildingCmp>();
                    right = (rightBuilding != null && (rightBuilding.BuildingTemplate.BuildingType == BuildingType.Wall) 
                        && rightBuilding.BuildingTemplate.ConnectWithOtherBlocks) ? 4 : 0;

                    BuildingCmp bottomBuilding = tile.BottomTile?.Entity?.Get<BuildingCmp>();
                    bottom = (bottomBuilding != null && (bottomBuilding.BuildingTemplate.BuildingType == BuildingType.Wall) 
                        && bottomBuilding.BuildingTemplate.ConnectWithOtherBlocks) ? 8 : 0;
                }
                else
                {
                    BuildingCmp topBuilding = tile.TopTile?.Entity?.Get<BuildingCmp>();
                    top = (topBuilding != null && (topBuilding.BuildingTemplate.BuildingType == BuildingType.Wall) && 
                        topBuilding.BuildingTemplate.Name == building.BuildingTemplate.Name) ? 1 : 0;

                    BuildingCmp leftBuilding = tile.LeftTile?.Entity?.Get<BuildingCmp>();
                    left = (leftBuilding != null && (leftBuilding.BuildingTemplate.BuildingType == BuildingType.Wall) &&
                        leftBuilding.BuildingTemplate.Name == building.BuildingTemplate.Name) ? 2 : 0;

                    BuildingCmp rightBuilding = tile.RightTile?.Entity?.Get<BuildingCmp>();
                    right = (rightBuilding != null && (rightBuilding.BuildingTemplate.BuildingType == BuildingType.Wall) &&
                        rightBuilding.BuildingTemplate.Name == building.BuildingTemplate.Name) ? 4 : 0;

                    BuildingCmp bottomBuilding = tile.BottomTile?.Entity?.Get<BuildingCmp>();
                    bottom = (bottomBuilding != null && (bottomBuilding.BuildingTemplate.BuildingType == BuildingType.Wall) &&
                        bottomBuilding.BuildingTemplate.Name == building.BuildingTemplate.Name) ? 8 : 0;
                }

                int bitmask = top + left + right + bottom;

                bitmask += building.BuildingTemplate.TilesetOffset;

                if (building.IsBuilt == false)
                {
                    blockTileMap.SetCell(tile.X, tile.Y, bitmask, Color.White * 0.5f);
                }
                else
                {
                    blockTileMap.SetCell(tile.X, tile.Y, bitmask);
                }
            }
            else
            {
                blockTileMap.SetCell(tile.X, tile.Y, 0);
            }
        }  

        private void OnItemAddedCallback(Inventory senderInventory, Item item, int weight)
        {
            GameplayScene.WorldManager.OpenItem(item);

            if(GameplayScene.WorldManager.ItemsDecayer.Inventories.Contains(Inventory) == false) 
                GameplayScene.WorldManager.ItemsDecayer.Inventories.Add(Inventory);

            UpdateTileItemView();

            if (Room == null)
                return;

            int roomId = Room.ZoneId;

            if(weight > 0)
            {
                var tilesThatHaveItems = GameplayScene.WorldManager.TilesThatHaveItems;

                if (tilesThatHaveItems[roomId].ContainsKey(item) == false)
                {
                    tilesThatHaveItems[roomId].Add(item, new List<Inventory>());
                }

                if(tilesThatHaveItems[roomId][item].Contains(Inventory) == false)
                {
                    tilesThatHaveItems[roomId][item].Add(Inventory);
                }
            }
        }

        private void OnItemRemovedCallback(Inventory senderInventory, Item item, int weight)
        {
            UpdateTileItemView();

            if(Inventory.TotalItemsCount <= 0)
            {
                GameplayScene.WorldManager.ItemsDecayer.Inventories.Remove(Inventory);
            }

            if (Room == null)
                return;

            int roomId = Room.ZoneId;

            int availableWeight = senderInventory.GetAvailableItemCount(item);
            if(availableWeight <= 0)
            {
                if(GameplayScene.WorldManager.TilesThatHaveItems[roomId].ContainsKey(item))
                    GameplayScene.WorldManager.TilesThatHaveItems[roomId][item].Remove(senderInventory);
            }
        }

        public void UpdateTileItemView()
        {
            if(Inventory.ItemItemContainerPair.Count > 0)
            {
                foreach(var kvp in Inventory.ItemItemContainerPair)
                {
                    int factWeight = Inventory.GetInventoryFactWeight(kvp.Key);
                    
                    if (factWeight > 0)
                    {
                        itemTileMap.SetCell(X, Y, kvp.Key.Id);
                        return;
                    }
                }
            }

            itemTileMap.SetCell(X, Y, 0);
        }

        private void UpdateMovementSpeedPercent()
        {
            BuildingTemplate buildingTemplate = null;

            if(Entity != null)
            {
                buildingTemplate = Entity.Get<BuildingCmp>().BuildingTemplate;
            }
            else if(SurfaceId != -1)
            {
                buildingTemplate = Engine.Instance.SurfaceIdBuildingTemplate[SurfaceId];
            }

            if(buildingTemplate != null && buildingTemplate.MovementSpeed != -1)
            {
                MovementSpeedPercent = buildingTemplate.MovementSpeed;
            }
            else
            {
                switch (GroundTopType)
                {
                    case GroundTopType.Water:
                    case GroundTopType.DeepWater:
                    case GroundTopType.IrrigationCanalFull:
                        MovementSpeedPercent = 30;
                        break;
                    default:
                        MovementSpeedPercent = 87;
                        break;
                }
            }
        }

        public List<Tile> GetNeighbourTiles()
        {
            if(neighbourTiles == null)
            {
                neighbourTiles = new List<Tile>();

                if (TopTile != null)
                    neighbourTiles.Add(TopTile);

                if (BottomTile != null)
                    neighbourTiles.Add(BottomTile);

                if (LeftTile != null)
                    neighbourTiles.Add(LeftTile);

                if (RightTile != null)
                    neighbourTiles.Add(RightTile);
            }

            return neighbourTiles;
        }

        public List<Tile> GetAllNeighbourTiles(bool takeCenter = false)
        {
            if (allNeighbourTiles == null)
            {
                allNeighbourTiles = new List<Tile>();

                if(takeCenter)
                    allNeighbourTiles.Add(this);

                if (TopTile != null)
                    allNeighbourTiles.Add(TopTile);

                if (BottomTile != null)
                    allNeighbourTiles.Add(BottomTile);

                if (LeftTile != null)
                    allNeighbourTiles.Add(LeftTile);

                if (RightTile != null)
                    allNeighbourTiles.Add(RightTile);

                if (LeftTopTile != null)
                    allNeighbourTiles.Add(LeftTopTile);

                if (LeftBottomTile != null)
                    allNeighbourTiles.Add(LeftBottomTile);

                if (RightTopTile != null)
                    allNeighbourTiles.Add(RightTopTile);

                if (RightBottomTile != null)
                    allNeighbourTiles.Add(RightBottomTile);
            }

            return allNeighbourTiles;
        }

        public TileSaveData GetSaveData()
        {
            TileSaveData tileData = new TileSaveData();
            tileData.GroundType = (int)GroundType;
            tileData.GroundTopType = (int)GroundTopType;
            tileData.SurfaceId = SurfaceId;
            tileData.MoistureLevel = MoistureLevel;
            tileData.FertilizerLevel = FertilizerLevel;
            tileData.IrrigationStrength = IrrigationStrength;
            tileData.InventoryItems = new List<Tuple<int, int, float>>();

            foreach (var kvp in Inventory.ItemItemContainerPair)
            {
                List<ItemContainer> itemContainers = kvp.Value;

                foreach(var itemContainer in itemContainers)
                {
                    tileData.InventoryItems.Add(Tuple.Create(itemContainer.Item.Id, itemContainer.FactWeight, itemContainer.Durability));
                }
            }

            if(tileData.InventoryItems.Count == 0)
            {
                tileData.InventoryItems = null;
            }

            return tileData;
        }

        public string GetInformation()
        {
            int zondId = Room != null ? Room.ZoneId : -1;
            string info = $"X {X}, Y {Y} | Zone id {zondId} | Movement speed {MovementSpeedPercent}%";

            return info;
        }

        public Point GetAsPoint()
        {
            return point;
        }

        public Vector2 GetAsVector()
        {
            return vector;
        }

        public int GetZoneId()
        {
            return Room == null ? -1 : Room.ZoneId;
        }

    }
}
