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
        private ConstructionSiteCmp constructionSite;

        public BuildTask(CreatureCmp creature, ConstructionSiteCmp constructionSite)
            : base(creature, constructionSite.BuildingTemplate.BuildingToolType)
        {
            this.constructionSite = constructionSite;
        }

        public override void Begin()
        {

        }

        public override void BeforeUpdate()
        {
            base.BeforeUpdate();

            Tile tile = constructionSite.GetApproachableTile(Owner);
            Owner.Movement.SetPath(tile, false);

            Owner.Slider.SetValue(0, constructionSite.BuildingTemplate.ConstructionTime, constructionSite.ConstructionProgress, Color.Orange);
        }

        public override void UpdateTask()
        {
            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        float additionalBuff = Engine.GameDeltaTime * GetEfficiency();

                        constructionSite.ConstructionProgress += Engine.GameDeltaTime + additionalBuff;

                        Owner.Slider.SetValue(0, constructionSite.BuildingTemplate.ConstructionTime, constructionSite.ConstructionProgress, Color.Orange);

                        if(constructionSite.ConstructionProgress >= constructionSite.BuildingTemplate.ConstructionTime)
                        {
                            BuildingCmp building = constructionSite.Complete();

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
                        Tile tile = constructionSite.GetApproachableTile(Owner);
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
