using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Technolithic
{
    public class DestructFiltersUI : MyPanelUI
    {
        private const int Padding = 8;
        private const int TitleHeight = 32;
        private const int RowHeight = 32;
        private const int RowSpacing = 5;

        public DestructFiltersUI(Scene scene) : base(scene, Localization.GetLocalizedText("destruct_filters"), Color.White)
        {
            Dictionary<DestructFilter, MToggleUI> toggles = new Dictionary<DestructFilter, MToggleUI>();

            int y = Padding + TitleHeight;

            for (int i = 0; i < DestructFilters.All.Count; i++)
            {
                DestructFilter filter = DestructFilters.All[i];

                MToggleUI toggle = new MToggleUI(scene, DestructFilters.IsEnabled(filter));
                toggles.Add(filter, toggle);

                MNode row = CreateRow(scene, filter, toggle, hotkeyNumber: i + 1);
                row.Y = y;
                AddChildNode(row);

                y += RowHeight + RowSpacing;
            }

            Height = y - RowSpacing + Padding;

            AddComponent(new DestructFiltersUIScript(toggles));
        }

        private MNode CreateRow(Scene scene, DestructFilter filter, MToggleUI toggle, int hotkeyNumber)
        {
            MNode row = new MNode(scene);
            row.X = Padding;
            row.Width = Width - Padding * 2;
            row.Height = RowHeight;
            row.Tooltips = GetTooltip(filter, hotkeyNumber);

            MImageUI icon = new MImageUI(scene);
            icon.Image.Texture = filter.Icon;
            icon.Width = RowHeight;
            icon.Height = RowHeight;
            icon.X = toggle.Width + 5;

            MyText name = new MyText(scene);
            name.Text = filter.Name;
            name.X = icon.LocalX + icon.Width + 5;
            name.Width = row.Width - name.LocalX;
            name.Height = RowHeight;

            row.AddChildNode(toggle);
            row.AddChildNode(icon);
            row.AddChildNode(name);

            return row;
        }

        private static string GetTooltip(DestructFilter filter, int hotkeyNumber)
        {
            return $"{filter.Name}\n/c[#919090]" +
                Localization.GetLocalizedText("destruct_filter_toggle_hotkey", hotkeyNumber) + "\n" +
                Localization.GetLocalizedText("destruct_filter_only_hotkey", hotkeyNumber) + "\n" +
                Localization.GetLocalizedText("destruct_filter_all_hotkey") + "/cd";
        }
    }
}
