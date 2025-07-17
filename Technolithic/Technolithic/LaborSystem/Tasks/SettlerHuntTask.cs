using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class SettlerHuntTask : Task
    {

        public AnimalCmp TargetAnimal { get; private set; }

        private Tile targetTile;

        public SettlerHuntTask(CreatureCmp creature, AnimalCmp targetAnimal) : base(creature)
        {
            TargetAnimal = targetAnimal;
        }

        public override void Begin()
        {
            TargetAnimal.Reserve();
        }

        public override void BeforeUpdate()
        {
            targetTile = TargetAnimal.Movement.CurrentTile;
            Owner.Movement.SetPath(targetTile, true);
        }

        public override void UpdateTask()
        {
            if(TargetAnimal.IsDead)
            {
                State = TaskState.Success;
                return;
            }

            if(TargetAnimal.IsHidden || IsTaskValid() == false)
            {
                State = TaskState.Failed;
                return;
            }

            if (Owner.Movement.CurrentTile.GetZoneId() != TargetAnimal.Movement.CurrentTile.GetZoneId())
            {
                State = TaskState.Failed;
                return;
            }

            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        int distance = Utils.GetDistance(Owner.Movement.CurrentTile, TargetAnimal.Movement.CurrentTile);

                        if (distance <= 12 && distance > 1 && Owner.CreatureEquipment.HasTool(ToolType.HuntingRange))
                        {
                            Owner.CreatureEquipment.ToolItemContainer = Owner.CreatureEquipment.TryGetTool(ToolType.HuntingRange);

                            Owner.DoRangeAttack(TargetAnimal);
                        }
                        else if (distance <= 1)
                        {
                            Owner.CreatureEquipment.ToolItemContainer = Owner.CreatureEquipment.TryGetTool(ToolType.HuntingMelee);

                            Owner.DoMeleeAttack(TargetAnimal);
                        }
                        else
                        {
                            Owner.Movement.SetPath(TargetAnimal.Movement.CurrentTile, true);
                        }

                        State = TaskState.Running;
                    }
                    break;
                case MovementState.Failed:
                    {
                        State = TaskState.Failed;
                    }
                    break;
                case MovementState.Running:
                    {
                        int distance = Utils.GetDistance(Owner.Movement.CurrentTile, TargetAnimal.Movement.CurrentTile);

                        if (distance <= 12 && distance > 1 && Owner.CreatureEquipment.HasTool(ToolType.HuntingRange))
                        {
                            Owner.Movement.ResetPath();
                        }
                        else if (distance <= 1)
                        {
                            Owner.Movement.ResetPath();
                        }

                        State = TaskState.Running;
                    }
                    break;
            }
        }

        public override void Complete()
        {
            base.Complete();

            TargetAnimal.Unreserve();

            Owner.CreatureEquipment.ToolItemContainer = null;
        }

        public override void Cancel()
        {
            base.Cancel();

            TargetAnimal.Unreserve();

            Owner.Movement.ResetPath();

            Owner.CreatureEquipment.ToolItemContainer = null;
        }

        private bool IsTaskValid()
        {
            return TargetAnimal.IsInteractionMarked(InteractionType.Hunt) &&
                TargetAnimal.IsInteractionActivated(InteractionType.Hunt);
        }
    }
}
