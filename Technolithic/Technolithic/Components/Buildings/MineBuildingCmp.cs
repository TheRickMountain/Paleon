using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MineBuildingCmp : BuildingCmp
    {
        public bool AutoMineSpawnedDeposits { get; set; } = false;

        public float Timer { get; set; } = 0;

        private BuildingTemplate depositToSpawn;

        private Slider miningProgressSlider;

        public MineBuildingCmp(BuildingTemplate buildingTemplate, Direction direction) 
            : base(buildingTemplate, direction)
        {

        }

        public override void UpdateCompleted()
        {
            base.UpdateCompleted();

            IsTurnedOn = IsPowered;

            if (IsPowered == false)
                return;

            Timer += Engine.GameDeltaTime;
            if (Timer >= BuildingTemplate.MineData.SpawnTime)
            {
                Timer = 0;

                foreach (var tile in RangeTiles)
                {
                    if (MyRandom.ProbabilityChance(BuildingTemplate.MineData.SpawnChance) == false)
                    {
                        continue;
                    }

                    Entity deposit = GameplayScene.WorldManager.TryToBuild(depositToSpawn, tile.X, tile.Y, Direction.DOWN, true);
                    if(deposit == null)
                    {
                        continue;
                    }

                    deposit.Get<DepositCmp>().MarkInteraction(InteractionType.Mine);
                }
            }
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            miningProgressSlider = new Slider(BuildingTemplate.Width * Engine.TILE_SIZE, 4, Color.Black, Color.Orange);

            depositToSpawn = FindDepositToSpawn();
        }

        public override void DestructBuilding()
        {
            base.DestructBuilding();
        }

        public override void Render()
        {
            base.Render();

            if (IsBuilt && IsPowered)
            {
                miningProgressSlider.SetValue(0, BuildingTemplate.MineData.SpawnTime, Timer, Color.Orange);
                miningProgressSlider.Position = new Vector2(Entity.Position.X, Entity.Position.Y - 15);
                miningProgressSlider.Render();
            }
        }

        private BuildingTemplate FindDepositToSpawn()
        {
            Tile checkTile = TilesInfosArray[0, 0].Tile;
            GroundTopType groundTopType = checkTile.GroundTopType;
            string buildingTemplateKey = $"{groundTopType.ToString().ToLowerInvariant()}_deposit";
            return Engine.Instance.Buildings[buildingTemplateKey];
        }

        public override string GetInformation()
        {
            string baseInfo = base.GetInformation();

            if (IsBuilt)
            {
                baseInfo += $"\n{depositToSpawn.Name}\n- {Localization.GetLocalizedText("progress")}: {(int)Timer}/{BuildingTemplate.MineData.SpawnTime}";
            }

            return baseInfo;
        }

        public override BuildingSaveData GetSaveData()
        {
            var saveData = base.GetSaveData();

            saveData.MineBuildingAutoMineSpawnedDeposits = AutoMineSpawnedDeposits;
            saveData.MineBuildingTimer = Timer;

            return saveData;
        }

    }
}
