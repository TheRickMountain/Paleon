using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public abstract class ToolRequiredTask : Task
    {

        private InteractionType interactionType;
        private Tool tool;

        public ToolRequiredTask(CreatureCmp owner, InteractionType interactionType) : base(owner)
        {
            this.interactionType = interactionType;
        }

        public override void BeforeUpdate()
        {
            ItemContainer itemContainer = Owner.CreatureEquipment.TryGetTool(interactionType);

            if (itemContainer != null)
            {
                tool = itemContainer.Item.Tool;

                Owner.CreatureEquipment.ToolItemContainer = itemContainer;
            }
        }

        public override void Complete()
        {
            if (Owner.CreatureEquipment.TryGetTool(interactionType) != null)
            {
                Owner.CreatureEquipment.DecreaseToolDurability(interactionType, 1);
            }

            Owner.CreatureEquipment.ToolItemContainer = null;
        }

        public override void Cancel()
        {
            base.Cancel();

            Owner.CreatureEquipment.ToolItemContainer = null;
        }

        public float GetEfficiency() => tool != null ? tool.Efficiency : 0;

    }
}
