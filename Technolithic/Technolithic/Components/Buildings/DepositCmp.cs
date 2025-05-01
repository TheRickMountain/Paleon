using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class DepositCmp : BuildingCmp
    {
        public int CurrentStage { get; set; }

        private bool isMarkedToObtain = false;
        public bool IsMarkedToObtain 
        {
            get => isMarkedToObtain; 
            set
            {
                isMarkedToObtain = value;

                if(isMarkedToObtain)
                {
                    if (BuildingTemplate.Deposit.RequiredToolType == ToolType.Pick)
                    {
                        foreach (var tileInfo in TilesInfosList)
                        {
                            tileInfo.Tile.MarkType = MarkType.Mine;
                        }
                    }
                    else
                    {
                        foreach (var tileInfo in TilesInfosList)
                        {
                            tileInfo.Tile.MarkType = MarkType.Gather;
                        }
                    }
                }
                else
                {
                    foreach (var tileInfo in TilesInfosList)
                    {
                        tileInfo.Tile.MarkType = MarkType.None;
                    }
                }
            }
        }

        public float MiningCurrentTime { get; set; }

        public DepositCmp(BuildingTemplate buildingTemplate, Direction direction) : base(buildingTemplate, direction)
        {
            IsReserved = false;
            MiningCurrentTime = 0;
            CurrentStage = 0;
        }

        public override void Begin()
        {
            base.Begin();

            Sprite.CurrentAnimation.Frames[0] = BuildingTemplate.Deposit.StagesTextures[CurrentStage];
        }

        public bool ProcessInteraction(float speed)
        {
            MiningCurrentTime += speed;

            if(MiningCurrentTime >= BuildingTemplate.Deposit.MiningTime)
            {
                MiningCurrentTime = 0;

                int dropPerCollect = BuildingTemplate.Deposit.DropPerCollect;

                Tile lootTile = GetCenterTile();

                lootTile.Inventory.AddCargo(BuildingTemplate.Deposit.DepositResource, dropPerCollect);

                CurrentStage++;

                if (CurrentStage == BuildingTemplate.Deposit.Stages)
                {
                    DestructBuilding();
                }
                else
                {
                    Sprite.CurrentAnimation.Frames[0] = BuildingTemplate.Deposit.StagesTextures[CurrentStage];
                }

                return true;
            }

            return false;
        }

        public bool CanBeObtained()
        {
            return CurrentStage < BuildingTemplate.Deposit.Stages;
        }

        public override void CompleteBuilding()
        {
            base.CompleteBuilding();

            if (BuildingTemplate.Deposit.RequiredToolType == ToolType.Pick)
            {
                GameplayScene.WorldManager.MineableDeposits.Add(this);
            }
            else
            {
                GameplayScene.WorldManager.GatherableDeposits.Add(this);
            }
        }

        public override void DestructBuilding()
        {
            base.DestructBuilding();

            if (BuildingTemplate.Deposit.RequiredToolType == ToolType.Pick)
            {
                GameplayScene.WorldManager.MineableDeposits.Remove(this);
            }
            else
            {
                GameplayScene.WorldManager.GatherableDeposits.Remove(this);
            }
        }

        public override string GetInformation()
        {
            string info = base.GetInformation();

            info += $"{Localization.GetLocalizedText("resource")}: {BuildingTemplate.Deposit.DepositResource.Name}";

            return info;
        }

        public override BuildingSaveData GetSaveData()
        {
            var saveData = base.GetSaveData();

            if (IsBuilt)
            {
                saveData.DepositCurrentStage = CurrentStage;
                saveData.IsMarkedToObtain = IsMarkedToObtain;
                saveData.MiningCurrentTime = MiningCurrentTime;
            }

            return saveData;
        }

    }
}
