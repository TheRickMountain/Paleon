using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class CreatureUI : MyPanelUI
    {

        public CreatureUI(Scene scene) : base(scene, "Creature", Color.White)
        {
            ListViewUI statsListView = new ListViewUI(scene, 250, 32, 8, 1, false, false);
            statsListView.Name = "StatsListView";
            statsListView.X = 8;
            statsListView.Y = 8 + 32;
            statsListView.Active = false;
            AddChildNode(statsListView);

            InventoryListViewUI inventoryListView = new InventoryListViewUI(scene, 250, 32, 8);
            inventoryListView.Name = "InventoryListView";
            inventoryListView.X = 8;
            inventoryListView.Y = 8 + 32;
            inventoryListView.Active = false;
            AddChildNode(inventoryListView);

            EquipmentListViewUI equipmentListViewUI = new EquipmentListViewUI(scene, 250, 32, 8);
            equipmentListViewUI.Name = "EquipmentListView";
            equipmentListViewUI.X = 8;
            equipmentListViewUI.Y = 8 + 32;
            equipmentListViewUI.Active = false;
            AddChildNode(equipmentListViewUI);

            ListViewUI buttonsListView = new ListViewUI(Scene, 48, 48, 1, 5, false, false);
            buttonsListView.Name = "ButtonsListView";
            buttonsListView.X = 8;
            buttonsListView.Y = Height - 48 - 8;
            AddChildNode(buttonsListView);

            Width = statsListView.Width + 16;

            Tab statsTab = new Tab(scene, TextureBank.UITexture.GetSubtexture(208, 144, 16, 16), false);
            statsTab.Name = "StatsTab";
            statsTab.X = 8;
            statsTab.Y = -statsTab.Height;
            statsTab.Tooltips = Localization.GetLocalizedText("information");
            AddChildNode(statsTab);

            Tab inventoryTab = new Tab(scene, TextureBank.UITexture.GetSubtexture(224, 144, 16, 16), false);
            inventoryTab.Name = "InventoryTab";
            inventoryTab.X = statsTab.LocalX + statsTab.Width;
            inventoryTab.Y = -inventoryTab.Height;
            inventoryTab.Tooltips = Localization.GetLocalizedText("inventory");
            AddChildNode(inventoryTab);

            Tab equipmentTab = new Tab(scene, ResourceManager.EquipmentIcon, false);
            equipmentTab.Name = "EquipmentTab";
            equipmentTab.X = inventoryTab.LocalX + inventoryTab.Width;
            equipmentTab.Y = -equipmentTab.Height;
            equipmentTab.Tooltips = Localization.GetLocalizedText("equipment");
            AddChildNode(equipmentTab);

            AddComponent(new CreatureUIScript());
        }

    }
}
