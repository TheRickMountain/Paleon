using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class CraftTask : Task
    {

        private CrafterBuildingCmp crafter;


        public CraftTask(CreatureCmp creature, CrafterBuildingCmp crafter) : base(creature)
        {
            this.crafter = crafter;
        }

        public override void Begin()
        {
            crafter.IsReserved = true;
        }

        public override void BeforeUpdate()
        {
            Tile tile = crafter.GetApproachableTile(Owner);
            if (tile != null)
            {
                Owner.Movement.SetPath(tile, false);
                Owner.Slider.SetValue(0, crafter.TargetCraftingRecipe.Time, crafter.CurrentCraftingProgress, Color.Orange);
            }
            else
            {
                State = TaskState.Failed;
            }
        }

        public override void UpdateTask()
        {
            if(crafter.CanCraft == false || crafter.IsBuilt == false)
            {
                State = TaskState.Failed;
                return;
            }

            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        Owner.Slider.Active = true;

                        switch (crafter.ProcessCraft(Owner))
                        {
                            case CraftingState.Success:
                                {
                                    State = TaskState.Success;

                                    Owner.Slider.Active = false;

                                    crafter.IsReserved = false;
                                }
                                break;
                            case CraftingState.Fail:
                                {
                                    State = TaskState.Failed;

                                    Owner.Slider.Active = false;
                                }
                                break;

                        }

                        if (crafter.TargetCraftingRecipe != null)
                        {
                            Owner.Slider.SetValue(0, crafter.TargetCraftingRecipe.Time, crafter.CurrentCraftingProgress, Color.Orange);
                        }
                    }
                    break;
                case MovementState.Failed:
                    {
                        Tile tile = crafter.GetApproachableTile(Owner);
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

            crafter.IsReserved = false;
        }

    }
}