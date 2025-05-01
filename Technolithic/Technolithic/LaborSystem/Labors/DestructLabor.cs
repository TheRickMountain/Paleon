using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class DestructLabor : Labor
    {

        public DestructLabor() : base(LaborType.Build)
        {
            
        }

        public override bool Check(CreatureCmp creature)
        {
            int creatureRoomId = creature.Movement.CurrentTile.Room.Id;

            if (GameplayScene.WorldManager.BuildingsToDestruct[creatureRoomId].Count == 0)
                return false;

            BuildingCmp targetBuilding = null;

            foreach(var building in GameplayScene.WorldManager.BuildingsToDestruct[creatureRoomId])
            {
                if (building.ReservedToDestruct == false)
                {
                    targetBuilding = building;
                    break;
                }
            }

            if (targetBuilding == null)
                return false;

            DestructTask destructTask = new DestructTask(creature, targetBuilding);
            AddTask(creature, destructTask);

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            throw new NotImplementedException();
        }
    }
}
