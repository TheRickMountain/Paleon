using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class BuildLabor : Labor
    {

        private BuildingCmp building;

        public BuildLabor(BuildingCmp building, LaborType laborType) : base(laborType)
        {
            this.building = building;
        }

        public override bool Check(CreatureCmp creature)
        {
            if (CreatureTasks.Count > 0)
                return false;

            Tile targetTile = building.GetApproachableTile(creature);
            if (targetTile == null)
                return false;

            ToolType requiredToolType = building.BuildingTemplate.BuildingToolType;

            if (requiredToolType != ToolType.None)
            {
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
                    else if (creature.CreatureType == CreatureType.Animal)
                    {
                        return false;
                    }
                }
            }

            BuildTask buildTask = new BuildTask(creature, building);
            AddTask(creature, buildTask);

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            throw new NotImplementedException();
        }
    }
}
