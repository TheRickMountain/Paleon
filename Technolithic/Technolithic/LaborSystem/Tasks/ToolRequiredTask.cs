using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public abstract class ToolRequiredTask : Task
    {

        private ToolType toolType;
        private Tool tool;

        public ToolRequiredTask(CreatureCmp owner, ToolType toolType) : base(owner)
        {
            this.toolType = toolType;
        }

        public override void BeforeUpdate()
        {
            ItemContainer itemContainer = Owner.CreatureEquipment.TryGetTool(toolType);

            if (itemContainer != null)
            {
                tool = itemContainer.Item.Tool;

                Owner.CreatureEquipment.ToolItemContainer = itemContainer;
            }
        }

        public override void Complete()
        {
            if (Owner.CreatureEquipment.TryGetTool(toolType) != null)
            {
                Owner.CreatureEquipment.DegradeTool(toolType, 1);
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
