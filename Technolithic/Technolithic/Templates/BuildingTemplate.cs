using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public enum BuildingType
    {
        Hut,
        Gate,
        Crafter,
        Wall,
        Stockpile,
        FarmPlot,
        FishingPlace,
        FishTrap,
        WindPoweredEnergySource,
        CreaturePoweredEnergySource,
        Deposit,
        BuildIrrigationCanal,
        DestructIrrigationCanal,
        BeeHive,
        TimeMachine,
        AnimalPen,
        Mine,
        Lighter,
        Noria,
        Surface,
        DestructSurface,
        TradingPost,
        None
    }

    public enum BuildingCategory
    {
        Wall,
        Floor,
        Settlement,
        Production,
        Storage,
        Agriculture,
        Food,
        Metallurgy,
        Mechanisms,
        Mining,
        Knowledge,
        Medicine,
        None
    }

    public class BuildingTemplate
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public Dictionary<Direction, MyTexture> Icons { get; private set; }
        public Dictionary<Direction, MyTexture> Textures { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public BuildingType BuildingType { get; private set; }
        public BuildingCategory BuildingCategory { get; private set; }
        public LaborType BuildingLaborType { get; private set; }
        public ToolType BuildingToolType { get; private set; }

        public bool ConnectWithOtherBlocks { get; private set; }

        public int TilesetOffset { get; private set; }

        public bool Animated { get; private set; }
        public bool Rotatable { get; private set; }
        public bool Cloneable { get; private set; }

        public bool LetLight { get; private set; }

        public int TextureWidth { get; private set; }
        public int TextureHeight { get; private set; }

        public Dictionary<Item, int> BuildingRecipe { get; private set; }

        public bool RequireBuilding { get; private set; }

        public int ConstructionTime { get; private set; }

        public bool[,] WalkablePattern;
        public bool[,] TargetPattern;
        public char[,] GroundPattern;

        public bool ShowItemOnTop { get; private set; }

        public JObject JObject { get; private set; }

        public string Json { get; private set; }


        public LightEmitter LightEmitter { get; private set; }
        public FuelConsumer FuelConsumer { get; private set; }
        public EnergyConsumer EnergyConsumer { get; private set; }
        public EnergySourceData EnergySourceData { get; private set; }
        public Storage Storage { get; private set; }
        public Trap Trap { get; private set; }
        public Assignable Assignable { get; private set; }
        public Deposit Deposit { get; private set; }
        public MineData MineData { get; private set; }
        public Crafter Crafter { get; private set; }
        public BeeHiveData BeeHiveData { get; private set; }
        public AnimalPenData AnimalPenData { get; private set; }
        public PlantData PlantData { get; private set; }
        public SmokeGeneratorData SmokeGeneratorData { get; private set; }
        public SurfaceData SurfaceData { get; private set; }

        public bool IsDestructible { get; private set; }
        
        public int MovementSpeed { get; private set; }

        public int Range { get; private set; }

        public string GetInformation()
        {
            string baseInfo = $"{Name}\n/c[#919090]{Description}/cd";

            if(Storage != null)
            {
                baseInfo += "/c[#3690FF]" + Storage.GetInformation() + "/cd";
            }

            if(Assignable != null)
            {
                baseInfo += "/c[#3690FF]" + Assignable.GetInformation() + "/cd";
            }

            if(EnergyConsumer != null)
            {
                baseInfo += "/c[#3690FF]" + EnergyConsumer.GetInformation() + "/cd";
            }

            if (EnergySourceData != null)
            {
                baseInfo += "/c[#3690FF]" + EnergySourceData.GetInformation() + "/cd";
            }

            if(MineData != null)
            {
                baseInfo += "/c[#3690FF]" + MineData.GetInformation() + "/cd";
            }

            if (BuildingType == BuildingType.Surface)
            {
                baseInfo += $"\n/c[#3690FF]{Localization.GetLocalizedText("travel_speed")}: " + MovementSpeed + "%/cd";
            }

            return baseInfo;
        }

        public BuildingTemplate(string json)
        {
            Json = json.Split('.')[0];

            JObject = JObject.Parse(File.ReadAllText(Path.Combine(Engine.ContentDirectory, "Buildings", json)));

            Name = Localization.GetLocalizedText(JObject["name_key"].Value<string>());
            Description = Localization.GetLocalizedText(JObject["description_key"].Value<string>());

            BuildingType = (BuildingType)Enum.Parse(typeof(BuildingType), JObject["type"].Value<string>());
            BuildingCategory = (BuildingCategory)Enum.Parse(typeof(BuildingCategory), JObject["category"].Value<string>());
            BuildingLaborType = GetJsonParameter(JObject, "buildingLaborType", LaborType.Build);
            BuildingToolType = GetJsonParameter(JObject, "buildingToolType", ToolType.None);

            ConstructionTime = JObject["constructionTime"].Value<int>();

            Icons = new Dictionary<Direction, MyTexture>();

            MovementSpeed = JObject["movementSpeed"].Value<int>();

            Range = JObject["range"].IsNullOrEmpty() ? 0 : JObject["range"].Value<int>();

            Cloneable = JObject["cloneable"].Value<bool>();

            switch (BuildingType)
            {
                case BuildingType.Wall:
                    {
                        TilesetOffset = JObject["tilesetOffset"].Value<int>();
                        LetLight = JObject["letLight"].Value<bool>();

                        ConnectWithOtherBlocks = JObject["connectWithOtherBlocks"].Value<bool>();

                        Icons.Add(Direction.DOWN, ResourceManager.GetTexture("block_tileset").GetSubtexture(0, TilesetOffset, 16, 16));

                        bool walkable = JObject["walkable"].Value<bool>();

                        WalkablePattern = new bool[,] { { walkable } };
                        TargetPattern = new bool[,] { { false } };
                        RequireBuilding = true;

                        Animated = false;

                        Width = 1;
                        Height = 1;
                    }
                    break;
                default:
                    {
                        Rotatable = JObject["rotatable"].Value<bool>();

                        Textures = new Dictionary<Direction, MyTexture>();

                        TextureWidth = JObject["texture"]["width"].Value<int>();
                        TextureHeight = JObject["texture"]["height"].Value<int>();

                        if (Rotatable)
                        {
                            Textures.Add(Direction.DOWN, ResourceManager.GetTexture(JObject["texture"]["name"].Value<string>() + "_down"));
                            Textures.Add(Direction.UP, ResourceManager.GetTexture(JObject["texture"]["name"].Value<string>() + "_up"));
                            Textures.Add(Direction.LEFT, ResourceManager.GetTexture(JObject["texture"]["name"].Value<string>() + "_left"));
                            Textures.Add(Direction.RIGHT, ResourceManager.GetTexture(JObject["texture"]["name"].Value<string>() + "_right"));
                        
                            Icons.Add(Direction.DOWN, Textures[Direction.DOWN].GetSubtexture(0, 0, TextureWidth, TextureHeight));
                            Icons.Add(Direction.UP, Textures[Direction.UP].GetSubtexture(0, 0, TextureWidth, TextureHeight));
                            Icons.Add(Direction.LEFT, Textures[Direction.LEFT].GetSubtexture(0, 0, TextureHeight, TextureWidth));
                            Icons.Add(Direction.RIGHT, Textures[Direction.RIGHT].GetSubtexture(0, 0, TextureHeight, TextureWidth));
                        }
                        else
                        {
                            Textures.Add(Direction.DOWN, ResourceManager.GetTexture(JObject["texture"]["name"].Value<string>()));

                            Icons.Add(Direction.DOWN, Textures[Direction.DOWN].GetSubtexture(0, 0, TextureWidth, TextureHeight));
                        }
                        
                        Width = JObject["width"].Value<int>();
                        Height = JObject["height"].Value<int>();

                        WalkablePattern = new bool[Width, Height];
                        TargetPattern = new bool[Width, Height];

                        for (int x = 0; x < Width; x++)
                        {
                            for (int y = 0; y < Height; y++)
                            {
                                bool walkable = JObject["walkable_pattern"][y][x].Value<bool>();
                                bool target = JObject["target_pattern"][y][x].Value<bool>();
                                WalkablePattern[x, y] = walkable;
                                TargetPattern[x, y] = target;
                            }
                        }

                        Animated = JObject["animated"].Value<bool>();
                        RequireBuilding = JObject["requireBuilding"].Value<bool>();
                    }
                    break;
            }

            GroundPattern = new char[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    GroundPattern[x, y] = JObject["ground_pattern"][y][x].Value<char>();
                }
            }

            IsDestructible = JObject["destructible"].Value<bool>();

            if(!JObject["crafter"].IsNullOrEmpty())
            {
                Crafter = new Crafter(this);
            }

            if (!JObject["show_item_on_top"].IsNullOrEmpty())
            {
                ShowItemOnTop = JObject["show_item_on_top"].Value<bool>();
            }

            BuildingRecipe = new Dictionary<Item, int>();

            if (JObject["ingredients"].IsNullOrEmpty() == false)
            {
                foreach (var ingredient in JObject["ingredients"])
                {
                    Item item = ItemDatabase.GetItemByName(ingredient["item"].Value<string>());
                    int weight = ingredient["weight"].Value<int>();

                    BuildingRecipe.Add(item, weight);
                }
            }

            if(!JObject["fuelConsumer"].IsNullOrEmpty())
            {
                List<Item> requiredFuel = new List<Item>();

                foreach (var fuel in JObject["fuelConsumer"]["fuel"])
                {
                    Item item = ItemDatabase.GetItemByName(fuel.Value<string>());

                    requiredFuel.Add(item);
                }

                float energyConsumption = JObject["fuelConsumer"]["energy_consumption"].Value<float>();

                FuelConsumer = new FuelConsumer(requiredFuel, energyConsumption);
            }

            if (!JObject["EnergyConsumer"].IsNullOrEmpty())
            {
                EnergyConsumer = JsonSerializer.CreateDefault().Deserialize<EnergyConsumer>(JObject["EnergyConsumer"].CreateReader());
            }

            if(!JObject["EnergySourceData"].IsNullOrEmpty())
            {
                EnergySourceData = JsonSerializer.CreateDefault().Deserialize<EnergySourceData>(JObject["EnergySourceData"].CreateReader());
            }

            if(!JObject["beeHive"].IsNullOrEmpty())
            {
                BeeHiveData = new BeeHiveData(JObject);
            }

            if(!JObject["smokeGenerator"].IsNullOrEmpty())
            {
                SmokeGeneratorData = new SmokeGeneratorData(JObject["smokeGenerator"]["x"].Value<float>(),
                    JObject["smokeGenerator"]["y"].Value<float>());
            }

            if(!JObject["plant"].IsNullOrEmpty())
            {
                PlantData = new PlantData(JObject);
            }

            if(!JObject["lightEmitter"].IsNullOrEmpty())
            {
                int radius = JObject["lightEmitter"]["radius"].Value<int>();
                LightEmitter = new LightEmitter(radius);
            }

            if(!JObject["storage"].IsNullOrEmpty())
            {
                Storage = new Storage(this);
            }

            if(!JObject["trap"].IsNullOrEmpty())
            {
                Trap = new Trap(JObject);
            }

            if(!JObject["assignable"].IsNullOrEmpty())
            {
                Assignable = new Assignable(JObject);
            }

            if(!JObject["animalPen"].IsNullOrEmpty())
            {
                AnimalPenData = new AnimalPenData(JObject);
            }

            if(!JObject["deposit"].IsNullOrEmpty())
            {
                Deposit = new Deposit(JObject);
            }

            if(!JObject["MineData"].IsNullOrEmpty())
            {
                MineData = JsonSerializer.CreateDefault().Deserialize<MineData>(JObject["MineData"].CreateReader());
            }

            if (!JObject["SurfaceData"].IsNullOrEmpty())
            {
                SurfaceData = JsonSerializer.CreateDefault().Deserialize<SurfaceData>(JObject["SurfaceData"].CreateReader());
            }

            if(PlantData != null)
            {
                Icons[Direction.DOWN] = PlantData.Icon;
            }
        }

        private T GetJsonParameter<T>(JObject jobject, string parameter, T defaultValue) where T : struct, IConvertible
        {
            if(jobject[parameter].IsNullOrEmpty())
            {
                return defaultValue;
            }
            else
            {
                return (T)Enum.Parse(typeof(T), jobject[parameter].Value<string>());
            }
        }

        public Entity CreateEntity(Tile[,] tiles, Direction direction, bool completeImmediately, float currentFuelCondition)
        {
            Entity entity = new Entity();

            BuildingCmp buildingCmp = null;

            switch (BuildingType)
            {
                case BuildingType.Hut:
                    buildingCmp = new HutBuildingCmp(this, direction);
                    break;
                case BuildingType.Crafter:
                    buildingCmp = new CrafterBuildingCmp(this, direction);
                    break;
                case BuildingType.Stockpile:
                    buildingCmp = new StorageBuildingCmp(this, direction);
                    break;
                case BuildingType.Wall:
                    buildingCmp = new WallCmp(this, direction);
                    break;
                case BuildingType.FarmPlot:
                    buildingCmp = new FarmPlot(this, direction);
                    break;
                case BuildingType.Gate:
                    buildingCmp = new GateCmp(this, direction);
                    break;
                case BuildingType.FishingPlace:
                    buildingCmp = new FishingPlaceCmp(this, direction);
                    break;
                case BuildingType.FishTrap:
                    buildingCmp = new FishTrap(this, direction);
                    break;
                case BuildingType.Deposit:
                    buildingCmp = new DepositCmp(this, direction);
                    break;
                case BuildingType.BuildIrrigationCanal:
                    buildingCmp = new BuildIrrigationCanalCmp(this, direction);
                    break;
                case BuildingType.DestructIrrigationCanal:
                    buildingCmp = new DestructIrrigationCanalCmp(this, direction);
                    break;
                case BuildingType.WindPoweredEnergySource:
                    buildingCmp = new WindPoweredEnergySourceCmp(this, direction);
                    break;
                case BuildingType.CreaturePoweredEnergySource:
                    buildingCmp = new CreaturePoweredEnergySource(this, direction);
                    break;
                case BuildingType.BeeHive:
                    buildingCmp = new BeeHiveBuildingCmp(this, direction);
                    break;
                case BuildingType.TimeMachine:
                    buildingCmp = new TimeMachine(this, direction);
                    break;
                case BuildingType.AnimalPen:
                    buildingCmp = new AnimalPenBuildingCmp(this, direction);
                    break;
                case BuildingType.Mine:
                    buildingCmp = new MineBuildingCmp(this, direction);
                    break;
                case BuildingType.Lighter:
                    buildingCmp = new LighterCmp(this, direction);
                    break;
                case BuildingType.Noria:
                    buildingCmp = new NoriaBuildingCmp(this, direction);
                    break;
                case BuildingType.Surface:
                    buildingCmp = new SurfaceBuilding(this, direction);
                    break;
                case BuildingType.DestructSurface:
                    buildingCmp = new DestructSurfaceBuilding(this, direction);
                    break;
                case BuildingType.TradingPost:
                    buildingCmp = new TradingPostBuildingCmp(this, direction);
                    break;
                case BuildingType.None:
                    buildingCmp = new BuildingCmp(this, direction);
                    break;
            }
            
            entity.Add(buildingCmp);

            switch(direction)
            {
                case Direction.DOWN:
                case Direction.UP:
                    entity.Add(new SelectableCmp(0, 0, Width * Engine.TILE_SIZE, Height * Engine.TILE_SIZE, SelectableType.Building));
                    break;
                case Direction.LEFT:
                case Direction.RIGHT:
                    entity.Add(new SelectableCmp(0, 0, Height * Engine.TILE_SIZE, Width * Engine.TILE_SIZE, SelectableType.Building));
                    break;
            }

            if (completeImmediately)
            {
                buildingCmp.SetToBuild(tiles, false, currentFuelCondition);
            }
            else
            {
                buildingCmp.SetToBuild(tiles, RequireBuilding, currentFuelCondition);
            }

            return entity;
        }

    }
}
