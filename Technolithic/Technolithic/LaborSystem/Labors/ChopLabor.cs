using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ChopLabor : Labor
    {
        private FarmPlot farmPlot;

        private Tile tile;

        public ChopLabor(FarmPlot farmPlot, MarkType markType, LaborType laborType) : base(laborType)
        {
            this.farmPlot = farmPlot;

            tile = farmPlot.GetCenterTile();

            tile.MarkType = markType;
        }

        public override bool Check(CreatureCmp creature)
        {
            if (CreatureTasks.Count > 0)
                return false;

            if (farmPlot.GetApproachableTile(creature) == null)
                return false;

            if (farmPlot.PlantData.ToolType != ToolType.None)
            {
                bool toolRequired = farmPlot.PlantData.ToolRequired;
                ToolType requiredToolType = farmPlot.PlantData.ToolType;

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

            ChopTask chopTask = new ChopTask(creature, farmPlot);
            AddTask(creature, chopTask);

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
