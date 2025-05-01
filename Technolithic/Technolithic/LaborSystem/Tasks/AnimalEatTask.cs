using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AnimalEatTask : Task
    {

        private AnimalPenBuildingCmp animalPen;
        private Item hayItem;

        public AnimalEatTask(CreatureCmp creature, AnimalPenBuildingCmp stable) : base(creature)
        {
            this.animalPen = stable;
        }

        public override void Begin()
        {
            hayItem = ItemDatabase.GetItemByName("hay");

            animalPen.Inventory.ReserveItemWeight(hayItem, 1);
        }

        public override void BeforeUpdate()
        {
            Tile targetTile = animalPen.GetReachableTile(Owner);
            Owner.Movement.SetPath(targetTile, true);
        }

        public override void UpdateTask()
        {
            if (animalPen.IsBuilt == false)
            {
                State = TaskState.Failed;
                return;
            }

            switch (Owner.Movement.MovementState)
            {
                case MovementState.Success:
                    {
                        animalPen.Inventory.PopItem(hayItem, 1);
                        animalPen.Inventory.AddRequiredWeight(hayItem, 1);
                        animalPen.Inventory.ReserveItemWeight(hayItem, -1);
                        Owner.CreatureStats.Hunger.CurrentValue = Owner.CreatureStats.Hunger.MaxValue;

                        State = TaskState.Success;
                    }
                    break;
                case MovementState.Failed:
                    {
                        State = TaskState.Failed;
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

            animalPen.Inventory.ReserveItemWeight(hayItem, -1);
        }
    }
}