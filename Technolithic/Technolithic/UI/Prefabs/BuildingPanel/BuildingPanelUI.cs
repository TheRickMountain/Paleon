using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class BuildingPanelUI : MNode
    {

        public BuildingPanelUI(Scene scene) : base(scene)
        {
            Height = 48;

            AddComponent(new BuildingPanelUIScript());
        }

    }
}
