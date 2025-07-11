using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class BuildingSaveData
    {
        public List<InteractionType> MarkedInteractions;
        public Dictionary<InteractionType, float> InteractionPercentProgressDict;

        public string BuildingTemplateName { get; set; }
        public Point[,] Tiles { get; set; }
        public bool IsBuilt { get; set; }
        public Direction Direction { get; set; }
        public float BuildingProgress { get; set; }

        public float CurrentFuelCondition { get; set; }
        public Dictionary<int, bool> ConsumableFuel { get; set; }

        public List<Tuple<int, int, float>> InventoryItems { get; set; }

        public bool IsWild { get; set; }

        public bool Harvest { get; set; }
        public bool Chop { get; set; }
        public bool Fertilize { get; set; }
        public bool Irrigate { get; set; }
        public float GrowthPercent { get; set; }
        public int AdditionalHarvestScrore { get; set; }
        public float HarvestingCurrentProgress { get; set; }


        public GateState GateState { get; set; }

        public Dictionary<int, bool> StorageFilters { get; set; }
        public AllowanceMode AllowanceMode { get; set; }
        public int StorageCapacity { get; set; }
        public int StoragePriority { get; set; }

        public float CurrentCraftingProgress { get; set; }
        public Dictionary<int, int> CraftingRecipesToCraft { get; set; }
        public bool IsCrafterPrepared { get; set; }

        public float FishTrapCurrentTime { get; set; }
        public int FishTrapCatchedItemId { get; set; }
        public int FishTrapCurrentNumberOfUses { get; set; }

        // Deposit region
        public bool IsMarkedToObtain { get; set; }
        public int DepositCurrentStage { get; set; }
        public float MiningCurrentTime { get; set; }

        public bool IsBeeHiveEmpty { get; set; }
        public float BeeHiveCurrentPercentProgress { get; set; }
        public bool GatherBeeHiveResources { get; set; }

        // Animal pen
        public Dictionary<string, bool> AnimalsFilters { get; set; }
        public float CurrentManureProgress { get; set; }
        public bool IsFlaggedToCleanManure { get; set; }

        // Mine

        public bool MineBuildingAutoMineSpawnedDeposits {get; set;}
    
        public float MineBuildingTimer { get; set; }

        // Trading post
        public Dictionary<int, int> TradingPostItems { get; set; }
        public Dictionary<string, int> TradingPostAnimals { get; set; }

        // Hut
        public List<Guid> HutAssignedCreatures { get; set; }

        // Tree
        public float TreeBuildingGrowthProgress { get; set; }
    }
}
