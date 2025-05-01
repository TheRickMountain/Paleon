using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public enum EnergyType
    {
        Kinetic,
        Gas
    }

    public class EnergyManager
    {

        private Dictionary<BuildingCmp, List<BuildingCmp>> consumerSourcesMap;
        private Dictionary<EnergySourceCmp, List<BuildingCmp>> sourceConsumersMap;

        private List<BuildingCmp> energyConsumersList;
        private List<EnergySourceCmp> energySourcesList;

        public EnergyManager()
        {
            consumerSourcesMap = new Dictionary<BuildingCmp, List<BuildingCmp>>();
            sourceConsumersMap = new Dictionary<EnergySourceCmp, List<BuildingCmp>>();

            energyConsumersList = new List<BuildingCmp>();
            energySourcesList = new List<EnergySourceCmp>();
        }

        public void AddEnergySource(EnergySourceCmp energySource)
        {
            energySourcesList.Add(energySource);
            sourceConsumersMap.Add(energySource, new List<BuildingCmp>());

            foreach(Tile circleTile in energySource.RangeTiles)
            {
                circleTile.EnergySources[energySource.BuildingTemplate.EnergySourceData.EnergyType].Add(energySource);

                if(circleTile.Entity != null)
                {
                    BuildingCmp buildingCmp = circleTile.Entity.Get<BuildingCmp>();

                    if (buildingCmp == energySource)
                        continue;

                    if (buildingCmp == null)
                        continue;

                    if (buildingCmp.IsBuilt == false)
                        continue;

                    if (buildingCmp.BuildingTemplate.EnergyConsumer == null)
                        continue;

                    if (buildingCmp.BuildingTemplate.EnergyConsumer.EnergyType != energySource.BuildingTemplate.EnergySourceData.EnergyType)
                        continue;

                    AddKeyValuePair(energySource, buildingCmp, sourceConsumersMap);
                }
            }

            // Добавляем источник энергии в коллекцию источников энергии потребителя
            foreach (var energyConsumer in sourceConsumersMap[energySource])
            {
                consumerSourcesMap[energyConsumer].Add(energySource);
            }
        }

        public void AddEnergyConsumer(BuildingCmp energyConsumer)
        {
            energyConsumersList.Add(energyConsumer);
            consumerSourcesMap.Add(energyConsumer, new List<BuildingCmp>());

            foreach (var tileInfo in energyConsumer.TilesInfosList)
            {
                foreach (var energySource in tileInfo.Tile.EnergySources[energyConsumer.BuildingTemplate.EnergyConsumer.EnergyType])
                {
                    AddKeyValuePair(energyConsumer, energySource, consumerSourcesMap);
                }
            }

            // Добавляем потребитель энергии в коллекцию потребителей энергии источника
            foreach (var energySource in consumerSourcesMap[energyConsumer])
            {
                sourceConsumersMap[energySource as EnergySourceCmp].Add(energyConsumer);
            }
        }

        private void AddKeyValuePair<T>(T key, BuildingCmp value, Dictionary<T, List<BuildingCmp>> map) where T : BuildingCmp
        {
            if(map.ContainsKey(key) == false)
            {
                map.Add(key, new List<BuildingCmp>());
            }

            if(map[key].Contains(value) == false)
            {
                map[key].Add(value);
            }
        }

        public void RemoveEnergySource(EnergySourceCmp energySourceCmp)
        {
            foreach (Tile circleTile in energySourceCmp.RangeTiles)
            {
                circleTile.EnergySources[energySourceCmp.BuildingTemplate.EnergySourceData.EnergyType].Remove(energySourceCmp);
            }

            // Удаление источника от потребителей энергии
            foreach (var energyConsumer in sourceConsumersMap[energySourceCmp])
            {
                consumerSourcesMap[energyConsumer].Remove(energySourceCmp);
            }

            sourceConsumersMap.Remove(energySourceCmp);
            energySourcesList.Remove(energySourceCmp);
        }

        public void RemoveEnergyConsumer(BuildingCmp energyConsumer)
        {
            // Удаление потребителя от источников энергии
            foreach(var energySource in consumerSourcesMap[energyConsumer])
            {
                sourceConsumersMap[energySource as EnergySourceCmp].Remove(energyConsumer);
            }

            consumerSourcesMap.Remove(energyConsumer);
            energyConsumersList.Remove(energyConsumer);
        }

        public void Update()
        {
            // Сбрасываем мощность источников до максимального
            foreach(var energySource in energySourcesList)
            {
                energySource.AvailablePower = energySource.GetActualEnergyOutput();
            }

            // Каждый потребитель отнимает мощность источника энергии
            foreach (var energyConsumer in energyConsumersList)
            {
                int totalPower = 0;

                bool success = false;

                // Определяем, хватит ли энергии со всех источников для потребителя
                foreach (var energySource in consumerSourcesMap[energyConsumer])
                {
                    // Если иссточник энергии потребляет топливо, то в случае его отсутсвия, источник не вырабатывает энергию
                    if (energySource.BuildingTemplate.FuelConsumer != null)
                        if (energySource.CurrentFuelCondition <= 0)
                            continue;

                    totalPower += (energySource as EnergySourceCmp).AvailablePower;

                    if (totalPower >= energyConsumer.BuildingTemplate.EnergyConsumer.RequiredPower)
                    {
                        success = true;
                        break;
                    }
                }

                // Если энергии хватит, то отнимаем мощность с источников
                if (success)
                {
                    energyConsumer.IsPowered = true;

                    int requiredPower = energyConsumer.BuildingTemplate.EnergyConsumer.RequiredPower;

                    foreach (var energySource in consumerSourcesMap[energyConsumer])
                    {
                        if ((energySource as EnergySourceCmp).AvailablePower >= requiredPower)
                        {
                            (energySource as EnergySourceCmp).AvailablePower -= requiredPower;
                            break;
                        }
                        else
                        {
                            requiredPower -= (energySource as EnergySourceCmp).AvailablePower;
                            (energySource as EnergySourceCmp).AvailablePower = 0;
                        }
                    }
                }
                else
                {
                    energyConsumer.IsPowered = false;
                }
            }
        }

        public void Render()
        {
            if (GameplayScene.WorldManager.CurrentAction == MyAction.Build)
            {
                World world = GameplayScene.Instance.World;

                BuildingTemplate currentBuildingTemplate = GameplayScene.WorldManager.CurrentBuildingTemplate;

                if (currentBuildingTemplate == null)
                    return;

                if (currentBuildingTemplate.EnergyConsumer == null)
                    return;

                for (int x = 0; x < GameplayScene.Instance.World.Width; x++)
                {
                    for (int y = 0; y < world.Height; y++)
                    {
                        if (world.GetTileAt(x, y).EnergySources[currentBuildingTemplate.EnergyConsumer.EnergyType].Count > 0)
                        {
                            RenderManager.Rect(x * Engine.TILE_SIZE, y * Engine.TILE_SIZE,
                                Engine.TILE_SIZE, Engine.TILE_SIZE, Color.Blue * 0.25f);
                        }
                    }
                }
            }
        }
    }
}