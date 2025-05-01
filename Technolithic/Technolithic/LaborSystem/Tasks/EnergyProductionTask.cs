using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class EnergyProductionTask : Task
    {

        private CreaturePoweredEnergySource energySource;

        public EnergyProductionTask(CreatureCmp owner, CreaturePoweredEnergySource energySource) : base(owner)
        {
            this.energySource = energySource;
        }

        public override void Begin()
        {
            energySource.IsReserved = true;
        }

        public override void BeforeUpdate()
        {
            Tile tile = energySource.GetReachableTile(Owner);
            Owner.Movement.SetPath(tile, false);
        }

        public override void UpdateTask()
        {
            if (energySource.IsBuilt == false)
            {
                State = TaskState.Failed;
                return;
            }

            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        energySource.CurrentCreature = Owner;
                    }
                    break;
                case MovementState.Failed:
                    {
                        Tile tile = energySource.GetReachableTile(Owner);
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

        public override void Complete()
        {
            base.Complete();

            Owner.Movement.ResetPath();

            energySource.IsReserved = false;

            energySource.CurrentCreature = null;
        }

        public override void Cancel()
        {
            base.Cancel();

            Owner.Movement.ResetPath();

            energySource.IsReserved = false;

            energySource.CurrentCreature = null;
        }
    }
}
