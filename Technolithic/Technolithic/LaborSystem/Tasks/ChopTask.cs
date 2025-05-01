
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Technolithic
{
    public class ChopTask : ToolRequiredTask
    {

        private FarmPlot farmPlot;

        public ChopTask(CreatureCmp creature, FarmPlot farmPlot) : base(creature, farmPlot.PlantData.ToolType)
        {
            this.farmPlot = farmPlot;
        }

        public override void Begin()
        {
        }

        public override void BeforeUpdate()
        {
            base.BeforeUpdate();

            Owner.Movement.SetPath(farmPlot.GetReachableTile(), false);

            Owner.Slider.SetValue(0, farmPlot.DestructingMaxProgress, farmPlot.DestructingCurrentProgress, Color.Orange);
        }

        public override void UpdateTask()
        {
            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        float additionalBuff = Engine.GameDeltaTime * GetEfficiency();

                        farmPlot.DestructingCurrentProgress += Engine.GameDeltaTime + additionalBuff;

                        Owner.Slider.SetValue(0, farmPlot.DestructingMaxProgress, farmPlot.DestructingCurrentProgress, Color.Orange);

                        if (farmPlot.DestructingCurrentProgress >= farmPlot.DestructingMaxProgress)
                        {
                            farmPlot.TryToChop();

                            Owner.Slider.Active = false;
                            State = TaskState.Success;
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
                        Tile tile = farmPlot.GetReachableTile(Owner);
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
