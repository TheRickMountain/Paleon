using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class BuildLabor : Labor
    {

        private ConstructionSiteCmp constructionSite;

        public BuildLabor(ConstructionSiteCmp constructionSite, LaborType laborType) : base(laborType)
        {
            this.constructionSite = constructionSite;
        }

        public override bool Check(CreatureCmp creature)
        {
            if (CreatureTasks.Count > 0)
                return false;

            Tile targetTile = constructionSite.GetApproachableTile(creature);
            if (targetTile == null)
                return false;

            ToolType requiredToolType = constructionSite.BuildingTemplate.BuildingToolType;

            // Animals only help where they work with a tool, e.g. plowing with a plow.
            if (creature.CreatureType == CreatureType.Animal && requiredToolType == ToolType.None)
                return false;

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

            BuildTask buildTask = new BuildTask(creature, constructionSite);
            AddTask(creature, buildTask);

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            throw new NotImplementedException();
        }
    }
}
