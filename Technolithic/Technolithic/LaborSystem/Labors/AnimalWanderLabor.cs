using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AnimalWanderLabor : Labor
    {
        public AnimalWanderLabor() : base(LaborType.Waner)
        {

        }

        public override bool Check(CreatureCmp creature)
        {
            if (CreatureTasks.Count > 0)
                return false;

            Tile targetTile = null;

            AnimalCmp animal = creature as AnimalCmp;

            if (animal.IsDomesticated)
            {
                if (animal.Parent != null)
                {
                    CreatureCmp parent = animal.Parent;

                    if (animal.GetRoomId() == parent.GetRoomId())
                    {
                        Room room = parent.Movement.CurrentTile.Room;

                        targetTile = room.GetRandomTile();
                    }
                }

                if (targetTile == null)
                {
                    // Trying to find new animal pen
                    if (animal.TargetAnimalPen == null || animal.TargetAnimalPen.GetApproachableTile(animal) == null)
                    {
                        foreach (var animalPen in GameplayScene.WorldManager.AnimalPenBuildings)
                        {
                            if (animalPen.BuildingTemplate.AnimalPenData.CheckAnimalFitsToFilters(animal.AnimalTemplate) == false)
                                continue;

                            if (animalPen.FreeSlots <= 0)
                                continue;

                            if (animalPen.GetAnimalTemplateFilter(animal.AnimalTemplate) && animalPen.GetApproachableTile(animal) != null)
                            {
                                if (animal.TargetAnimalPen != null)
                                {
                                    animal.TargetAnimalPen.RemoveAnimal(animal);
                                }

                                animalPen.AddAnimal(animal);
                                break;
                            }
                        }
                    }

                    if (animal.TargetAnimalPen != null)
                    {
                        Tile randomTile = animal.TargetAnimalPen.TilesInfosList[MyRandom.Range(animal.TargetAnimalPen.TilesInfosList.Count - 1)].Tile;
                        if (randomTile.Room != null && randomTile.Room.ZoneId == creature.Movement.CurrentTile.Room.ZoneId)
                        {
                            targetTile = randomTile;
                        }
                    }
                }
            }

            if (targetTile == null)
            {
                if (GameplayScene.WorldManager.HomeArea.Count > 0 && creature.IsDomesticated)
                {
                    Tile randomTile = GameplayScene.WorldManager.HomeArea[MyRandom.Range(GameplayScene.WorldManager.HomeArea.Count)];
                    if (randomTile.Room != null && randomTile.Room.ZoneId == creature.Movement.CurrentTile.Room.ZoneId)
                    {
                        targetTile = randomTile;
                    }
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
