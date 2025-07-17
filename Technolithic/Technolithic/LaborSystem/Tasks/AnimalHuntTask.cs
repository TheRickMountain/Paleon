using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AnimalHuntTask : Task
    {

        private CreatureCmp targetCreature;

        private Tile targetTile;
        private Tool currentTool;

        private const float CHASE_TIME = 30f;
        private float chaseTimeProgress = 0f;

        public AnimalHuntTask(CreatureCmp creature, CreatureCmp targetCreature) : base(creature)
        {
            this.targetCreature = targetCreature;
        }

        public override void Begin()
        {

        }

        public override void BeforeUpdate()
        {
            targetTile = targetCreature.Movement.CurrentTile;
            Owner.Movement.SetPath(targetTile, true);

            if(Owner.CreatureEquipment.ToolItemContainer != null)
            {
                currentTool = Owner.CreatureEquipment.ToolItemContainer.Item.Tool;
            }
        }

        public override void UpdateTask()
        {
            if (targetCreature.IsHidden || targetCreature.IsDead)
            {
                State = TaskState.Success;
                return;
            }

            if (Owner.Movement.CurrentTile.Room.ZoneId != targetCreature.Movement.CurrentTile.Room.ZoneId)
            {
                State = TaskState.Success;
                return;
            }

            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        Owner.LookAt(targetCreature);

                        int distance = Utils.GetDistance(Owner.Movement.CurrentTile, targetCreature.Movement.CurrentTile);

                        if (distance <= 12 && distance > 1 && Owner.CreatureEquipment.HasTool(ToolType.HuntingRange))
                        {
                            Owner.CreatureEquipment.ToolItemContainer = Owner.CreatureEquipment.TryGetTool(ToolType.HuntingRange);

                            Owner.DoRangeAttack(targetCreature);

                            chaseTimeProgress = 0;
                        }
                        else if (distance <= 1)
                        {
                            Owner.CreatureEquipment.ToolItemContainer = Owner.CreatureEquipment.TryGetTool(ToolType.HuntingMelee);

                            Owner.DoMeleeAttack(targetCreature);

                            chaseTimeProgress = 0;
                        }
                        else
                        {
                            Owner.Movement.SetPath(targetCreature.Movement.CurrentTile, true);
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
                        // Только дикое животное перестает следовать за жертвой
                        if (Owner.CreatureType == CreatureType.Animal && 
                            Owner.IsDomesticated == false)
                        {
                            chaseTimeProgress += Engine.GameDeltaTime;
                            if (chaseTimeProgress >= CHASE_TIME)
                            {
                                chaseTimeProgress = 0;
                                State = TaskState.Success;
                                return;
                            }
                        }

                        int distance = Utils.GetDistance(Owner.Movement.CurrentTile, targetCreature.Movement.CurrentTile);

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

            Owner.CreatureEquipment.ToolItemContainer = null;
        }

        public override void Cancel()
        {
            base.Cancel();

            Owner.Movement.ResetPath();

            Owner.CreatureEquipment.ToolItemContainer = null;
        }
    }
}
