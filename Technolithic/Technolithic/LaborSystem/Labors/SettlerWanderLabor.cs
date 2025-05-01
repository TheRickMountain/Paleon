using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class SettlerWanderLabor : Labor
    {

        public SettlerWanderLabor() : base(LaborType.Waner)
        {

        }

        public override bool Check(CreatureCmp creature)
        {
            if (CreatureTasks.Count > 0)
                return false;

            Tile targetTile = null;

            if(GameplayScene.WorldManager.HomeArea.Count > 0)
            {
                Tile randomTile = GameplayScene.WorldManager.HomeArea[MyRandom.Range(GameplayScene.WorldManager.HomeArea.Count)];
                if (randomTile.Room != null && randomTile.Room.Id == creature.Movement.CurrentTile.Room.Id)
                {
                    targetTile = randomTile;
                }
            }

            if (targetTile == null)
            {
                Room room = creature.Movement.CurrentTile.Room;

                List<Tile> roomTiles;

                bool moveInsideCurrentRoom = MyRandom.GetRandomBool();

                if (room.Neighbours.Count == 0 || moveInsideCurrentRoom)
                {
                    roomTiles = room.Tiles;
                }
                else
                {
                    roomTiles = room.Neighbours[MyRandom.Range(room.Neighbours.Count)].Tiles;
                }


                if (roomTiles.Count == 0)
                {
                    targetTile = creature.Movement.CurrentTile;
                }
                else
                {
                    targetTile = roomTiles[MyRandom.Range(roomTiles.Count)];

                    // Если мы наткнулись на тайл с водой, то просто передаем тайл на котором стоит поселенец
                    if (targetTile.GroundTopType == GroundTopType.Water || targetTile.GroundTopType == GroundTopType.DeepWater)
                    {
                        targetTile = creature.Movement.CurrentTile;
                    }
                }
            }

            WanderTask wander = new WanderTask(creature, targetTile);
            AddTask(creature, wander);

            return true;
        }

        public override void CreateTasks(CreatureCmp creature)
        {
            throw new NotImplementedException();
        }

    }
}
