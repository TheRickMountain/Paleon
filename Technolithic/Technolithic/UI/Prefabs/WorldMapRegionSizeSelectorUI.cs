using Microsoft.Xna.Framework;
using System;

namespace Technolithic
{
    public class WorldMapRegionSizeSelectorUI : MyPanelUI
    {
        public event Action<int> RegionSizeChanged;

        private MNode node128;
        private MNode node256;

        public WorldMapRegionSizeSelectorUI(Scene scene) 
            : base(scene, Localization.GetLocalizedText("region_size"), Color.White)
        {
            Width = 200;
            Height = 150;

            node128 = CreateRegionSizeElement(128);
            node128.X = 8;
            node128.Y = 40;
            AddChildNode(node128);

            node256 = CreateRegionSizeElement(256);
            node256.X = 8;
            node256.Y = node128.LocalY + node128.Height + 5;
            AddChildNode(node256);

            node128.GetChildByName("Toggle").GetComponent<ToggleScript>().SilentCheck(true);
        }

        private MNode CreateRegionSizeElement(int regionSize)
        {
            MNode element = new MNode(Scene);

            MToggleUI toggle = new MToggleUI(Scene, false, true);
            toggle.Name = "Toggle";
            toggle.X = 8;
            toggle.GetComponent<ToggleScript>().AddOnValueChangedCallback(OnTogglePressed);
            toggle.SetMetadata("region_size", regionSize);

            element.AddChildNode(toggle);

            MyText itemName = new MyText(Scene);
            itemName.Text = regionSize + "x" + regionSize;
            itemName.Name = "Name";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = toggle.LocalX + toggle.Width + 5;
            itemName.Y = 2;
            element.AddChildNode(itemName);

            element.Width = toggle.Width + 5 + itemName.Width;
            element.Height = 34;

            return element;
        }

        private void OnTogglePressed(bool obj1, MToggleUI obj2)
        {
            node128.GetChildByName("Toggle").GetComponent<ToggleScript>().SilentCheck(false);
            node256.GetChildByName("Toggle").GetComponent<ToggleScript>().SilentCheck(false);

            obj2.GetComponent<ToggleScript>().SilentCheck(true);

            int regionSize = obj2.GetMetadata<int>("region_size");
            RegionSizeChanged?.Invoke(regionSize);
        }
    }
}
