using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class WanderTask : Task
    {
        private Tile targetTile;

        public WanderTask(CreatureCmp creature, Tile tile) : base(creature)
        {
            targetTile = tile;
        }

        public override void Begin()
        {
        }

        public override void BeforeUpdate()
        {
            Owner.Movement.SetPath(targetTile, false);
        }

        public override void UpdateTask()
        {
            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    State = TaskState.Success;
                    break;
                case MovementState.Failed:
                    State = TaskState.Failed;
                    break;
                case MovementState.Running:
                    State = TaskState.Running;
                    break;
            }
        }

        public override void Cancel()
        {
            base.Cancel();

            Owner.Movement.ResetPath();
        }

    }
}
