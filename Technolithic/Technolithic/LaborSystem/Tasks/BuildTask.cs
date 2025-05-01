using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class BuildTask : ToolRequiredTask
    {
        private BuildingCmp building;

        public BuildTask(CreatureCmp creature, BuildingCmp building) 
            : base(creature, building.BuildingTemplate.BuildingToolType)
        {
            this.building = building;
        }

        public override void Begin()
        {

        }

        public override void BeforeUpdate()
        {
            base.BeforeUpdate();

            Tile tile = building.GetReachableTile(Owner);
            Owner.Movement.SetPath(tile, false);

            Owner.Slider.SetValue(0, building.BuildingTemplate.ConstructionTime, building.ConstructionProgress, Color.Orange);
        }

        public override void UpdateTask()
        {
            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        float additionalBuff = Engine.GameDeltaTime * GetEfficiency();

                        building.ConstructionProgress += Engine.GameDeltaTime + additionalBuff;

                        Owner.Slider.SetValue(0, building.BuildingTemplate.ConstructionTime, building.ConstructionProgress, Color.Orange);

                        if(building.ConstructionProgress >= building.BuildingTemplate.ConstructionTime)
                        {
                            building.CompleteBuilding();

                            if(building.BuildingTemplate.PlantData != null)
                            {
                                (building as FarmPlot).Harvest = true;
                            }
                                
                            State = TaskState.Success;

                            Owner.Slider.Active = false;
                        }
                        else
                        {
                            Owner.Slider.Active = true;
                            State = TaskState.Running;
                        }
                    }
                    break;
                case MovementState.Failed:
                    {
                        Tile tile = building.GetReachableTile(Owner);
                        if (tile != null)
                        {
                            Owner.Movement.SetPath(tile, false);
                        }
                        else
                        {
                            State = TaskState.Failed;
                        }
                    }
                    break;
                case MovementState.Running:
                    {
                        State = TaskState.Running;
                    }
                    break;
            }
        }

        public override void Cancel()
        {
            base.Cancel();

            Owner.Movement.ResetPath();

            Owner.Slider.Active = false;
        }
    }
}
