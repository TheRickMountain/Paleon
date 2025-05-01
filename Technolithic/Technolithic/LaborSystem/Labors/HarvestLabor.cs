using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class HarvestLabor : Labor
    {
        private FarmPlot farmPlot;

        private Tile tile;

        public HarvestLabor(FarmPlot farmPlot, MarkType markType, LaborType laborType) : base(laborType)
        {
            this.farmPlot = farmPlot;

            tile = farmPlot.GetCenterTile();
            tile.MarkType = markType;
        }

        public override bool Check(CreatureCmp creature)
        {
            if (CreatureTasks.Count > 0)
                return false;

            if (farmPlot.GetReachableTile(creature) == null)
                return false;

            if (farmPlot.PlantData.ToolType != ToolType.None)
            {
                ToolType requiredToolType = farmPlot.PlantData.ToolType;
                bool toolRequired = farmPlot.PlantData.ToolRequired;

                if (creature.CreatureEquipment.HasTool(requiredToolType) == false)
                {
                    var tuplePair = GameplayScene.WorldManager.FindTool(creature, requiredToolType);

                    if (tuplePair?.Item1 != null)
                    {
                        Inventory inventory = tuplePair.Item1;
                        Item item = tuplePair.Item2;

                        EquipItemTask equipTask = new EquipItemTask(creature, inventory, item);
                        AddTask(creature, equipTask);
                    }
                    else
                    {
                        if (toolRequired)
                            return false;

                        if (creature.CreatureType == CreatureType.Animal)
                            return false;
                    }
                }
            }

            HarvestTask harvestTask = new HarvestTask(creature, farmPlot);
            AddTask(creature, harvestTask);

            return true;
        }

        public override void CancelAndClearAllTasksAndComplete()
        {
            base.CancelAndClearAllTasksAndComplete();

            tile.MarkType = MarkType.None;
        }

        public override void Complete()
        {
            base.Complete();

            tile.MarkType = MarkType.None;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            throw new NotImplementedException();
        }

    }
}
