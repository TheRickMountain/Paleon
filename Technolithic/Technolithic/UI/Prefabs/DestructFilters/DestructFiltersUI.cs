using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Technolithic
{
    public class DestructFiltersUI : MyPanelUI
    {
        public const int Padding = 8;
        public const int TitleHeight = 32;
        public const int RowHeight = 32;
        public const int RowSpacing = 5;

        private const int _nameX = MToggleUI.WIDTH + 5 + RowHeight + 5;

        public DestructFiltersUI(Scene scene) : base(scene, Localization.GetLocalizedText("destruct_filters"), Color.White)
        {
            Width = Math.Max(Width, _nameX + DestructFilters.All.Max(filter => MeasureText(scene, filter.Name)) + Padding * 3);

            Dictionary<DestructFilter, MNode> rows = new Dictionary<DestructFilter, MNode>();

            foreach (DestructFilter filter in DestructFilters.All)
            {
                MNode row = CreateRow(scene, filter);
                rows.Add(filter, row);
                AddChildNode(row);
            }

            AddComponent(new DestructFiltersUIScript(rows));
        }

        private static int MeasureText(Scene scene, string text)
        {
            MyText measure = new MyText(scene);
            measure.Text = text;
            return measure.TextWidth;
        }

        private MNode CreateRow(Scene scene, DestructFilter filter)
        {
            MNode row = new MNode(scene);
            row.X = Padding;
            row.Width = Width - Padding * 2;
            row.Height = RowHeight;

            MToggleUI toggle = new MToggleUI(scene, DestructFilters.IsEnabled(filter));
            toggle.Name = "Toggle";

            MImageUI icon = new MImageUI(scene);
            icon.Image.Texture = filter.Icon;
            icon.Width = RowHeight;
            icon.Height = RowHeight;
            icon.X = MToggleUI.WIDTH + 5;

            MyText name = new MyText(scene);
            name.Text = filter.Name;
            name.X = _nameX;
            name.Width = row.Width - _nameX;
            name.Height = RowHeight;

            row.AddChildNode(toggle);
            row.AddChildNode(icon);
            row.AddChildNode(name);

            return row;
        }
    }
}
