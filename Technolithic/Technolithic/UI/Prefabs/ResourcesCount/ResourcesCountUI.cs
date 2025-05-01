using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class ResourcesCountUI : ListViewUI
    {

        public ResourcesCountUI(Scene scene) : base(scene, 96, 32, 10, 1, true, false, false)
        {
            AddComponent(new ResourcesCountUIScript());
        }

    }
}
