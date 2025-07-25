﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class LocalizationUI : MyPanelUI
    {

        public LocalizationUI(Scene scene, string title) : base(scene, title, Color.White)
        {
            ListViewUI listView = new ListViewUI(scene, 300, 34, 8);
            listView.Name = "ListView";
            listView.X = 8;
            listView.Y = 8 + 32;

            AddChildNode(listView);

            Width = listView.Width + 16;
            Height = 410;

            AddComponent(new LocalizationUIScript());
        }

    }
}
