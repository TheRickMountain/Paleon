using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class UnitCommandUI : ListViewUI
    {

        public UnitCommandUI(Scene scene) : base(scene, 48, 48, 1, 4, false, false)
        {
            AddComponent(new UnitCommandUIScript());
        }

    }
}
