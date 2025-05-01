using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class NotificationsUI : ListViewUI
    {

        public NotificationsUI(Scene scene) : base(scene, 240, 32, 10, 1, true, false, true)
        {
            AddComponent(new NotificationsUIScript());
        }
    }
}
