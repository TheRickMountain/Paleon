using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class BuildingUI : MyPanelUI
    {

        public BuildingUI(Scene scene) : base(scene, "Test", Color.White)
        {
            ListViewUI statsListView = new ListViewUI(scene, 32, 32, 8, 1, false, false);
            statsListView.Width = 280;
            statsListView.Height = 32 * 8;
            statsListView.Name = "StatsListView";
            statsListView.X = 8;
            statsListView.Y = 8 + 32;
            statsListView.Active = false;
            AddChildNode(statsListView);

            InventoryListViewUI inventoryListView = new InventoryListViewUI(scene, 240, 32, 7);
            inventoryListView.Name = "InventoryListView";
            inventoryListView.X = 8;
            inventoryListView.Y = 8 + 32;
            inventoryListView.Active = false;
            AddChildNode(inventoryListView);

            ListViewUI fuelListView = new ListViewUI(scene, 240, 32, 7);
            fuelListView.Name = "FuelListView";
            fuelListView.X = 8;
            fuelListView.Y = 8 + 32;
            fuelListView.Active = false;
            AddChildNode(fuelListView);

            Tab statsTab = new Tab(scene, TextureBank.UITexture.GetSubtexture(208, 144, 16, 16), false);
            statsTab.Name = "StatsTab";
            statsTab.X = 8;
            statsTab.Y = -statsTab.Height;
            statsTab.Tooltips = Localization.GetLocalizedText("information");
            AddChildNode(statsTab);

            Width = 48 + statsListView.Width;
            Height = 370;

            Tab inventoryTab = new Tab(scene, TextureBank.UITexture.GetSubtexture(224, 144, 16, 16), false);
            inventoryTab.Name = "InventoryTab";
            inventoryTab.X = statsTab.LocalX + statsTab.Width;
            inventoryTab.Y = -inventoryTab.Height;
            inventoryTab.Tooltips = Localization.GetLocalizedText("inventory");
            AddChildNode(inventoryTab);

            Tab fuelTab = new Tab(scene, ResourceManager.FuelIcon, false);
            fuelTab.Name = "FuelTab";
            fuelTab.X = inventoryTab.LocalX + inventoryTab.Width;
            fuelTab.Y = -fuelTab.Height;
            fuelTab.Tooltips = Localization.GetLocalizedText("fuel");
            AddChildNode(fuelTab);

            ListViewUI buttonsListView = new ListViewUI(Scene, 48, 48, 1, 6, false, false);
            buttonsListView.Name = "ButtonsListView";
            buttonsListView.X = 8;
            buttonsListView.Y = Height - buttonsListView.Height - 8;
            AddChildNode(buttonsListView);

            AddComponent(new BuildingUIScript());
        }

    }
}
