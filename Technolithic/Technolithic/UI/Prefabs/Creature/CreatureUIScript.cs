using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{ 

    public class CreatureUIScript : InteractableUIScript
    {
        private CreatureCmp selectedCreature;

        private MNode assignPetButton;

        private Dictionary<Tab, MNode> tabNodes = new Dictionary<Tab, MNode>();

        private Dictionary<AttributeType, RichTextUI> attributesTexts = new Dictionary<AttributeType, RichTextUI>();

        private MyText informationText;

        private ListViewUIScript statsListView;

        private SmallButton renameButton;

        public CreatureUIScript()
        {

        }

        public override void Awake()
        {
            
        }

        public override void Begin()
        {
            renameButton = new SmallButton(ParentNode.Scene, ResourceManager.RenameIcon);
            renameButton.X = ParentNode.Width - renameButton.Width - 5;
            renameButton.Y = 8;
            renameButton.Tooltips = Localization.GetLocalizedText("rename");
            renameButton.ButtonScript.AddOnClickedCallback(OnRenameButtonPressed);
            renameButton.Active = false;
            ParentNode.AddChildNode(renameButton);

            statsListView = ParentNode.GetChildByName("StatsListView").GetComponent<ListViewUIScript>();

            assignPetButton = new BigButton(ParentNode.Scene, ResourceManager.PetIcon, false);
            assignPetButton.Tooltips = Localization.GetLocalizedText("assign_a_pet");
            assignPetButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnAssignPetButtonPressed);

            ParentNode.GetChildByName("StatsTab").GetComponent<ButtonScript>().AddOnClickedCallback(SetTab);
            ParentNode.GetChildByName("InventoryTab").GetComponent<ButtonScript>().AddOnClickedCallback(SetTab);
            ParentNode.GetChildByName("EquipmentTab").GetComponent<ButtonScript>().AddOnClickedCallback(SetTab);

            tabNodes.Add((Tab)ParentNode.GetChildByName("StatsTab"), ParentNode.GetChildByName("StatsListView"));
            tabNodes.Add((Tab)ParentNode.GetChildByName("InventoryTab"), ParentNode.GetChildByName("InventoryListView"));
            tabNodes.Add((Tab)ParentNode.GetChildByName("EquipmentTab"), ParentNode.GetChildByName("EquipmentListView"));

            SetTab(true, ParentNode.GetChildByName("StatsTab").GetComponent<ButtonScript>());
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }

            foreach (var attribute in selectedCreature.CreatureStats.Attribures)
            {
                if (attribute.Active == false)
                    continue;

                RichTextUI textUI = attributesTexts[attribute.AttributeType];
                textUI.Tooltips = attribute.GetAdditionalInformation();
                textUI.Text = attribute.GetInformation();
            }

            if (selectedCreature.IsDead)
            {
                GameplayScene.UIRootNodeScript.CloseEntityPanel();
            }
        }

        public void SetCreature(CreatureCmp creature)
        {
            selectedCreature = creature;

            renameButton.Active = selectedCreature.CanBeRenamed;

            RichTextUI uiLabel = ((RichTextUI)ParentNode.GetChildByName("Label"));

            uiLabel.Text = $"{creature.Name}";

            if (creature.CreatureType == CreatureType.Animal)
            {
                uiLabel.Text = (creature as AnimalCmp).AnimalTemplate.GetNameWithAgeAndSex();
            }

            UpdateStatsListView(creature);

            (ParentNode.GetChildByName("InventoryListView") as InventoryListViewUI).SetInventory(creature.Inventory);

            (ParentNode.GetChildByName("EquipmentListView") as EquipmentListViewUI)
                .SetEquipment(creature, creature.CreatureEquipment);

            ListViewUIScript buttonsListView = ParentNode.GetChildByName("ButtonsListView").GetComponent<ListViewUIScript>();
            buttonsListView.Clear();

            if (creature is SettlerCmp)
            {
                buttonsListView.AddItem(assignPetButton);
            }

            SetInteractable(creature);
        }

        private void OnAssignPetButtonPressed(bool value, ButtonScript script)
        {
            GameplayScene.UIRootNodeScript.OpenAttachPetUI(selectedCreature);
        }

        private void SetTab(bool value, ButtonScript buttonScript)
        {
            foreach(var kvp in tabNodes)
            {
                Tab tab = kvp.Key;
                MNode node = kvp.Value;

                if (tab != buttonScript.ParentNode)
                {
                    tab.Y = -tab.Height;
                    node.Active = false;
                }
                else
                {
                    tab.Y = -tab.Height + 6;
                    node.Active = true;
                }
            }
        }

        private void OnRenameButtonPressed(bool value, ButtonScript buttonScript)
        {
            GameplayScene.UIRootNodeScript.OpenRenameCreatureUI(selectedCreature);
        }

        private void UpdateStatsListView(CreatureCmp creature)
        {
            statsListView.Clear();

            foreach (var attribute in creature.CreatureStats.Attribures)
            {
                if (attribute.Active)
                {
                    if (attributesTexts.ContainsKey(attribute.AttributeType))
                    {
                        RichTextUI textUI = attributesTexts[attribute.AttributeType];
                        textUI.Tooltips = attribute.GetAdditionalInformation();
                        textUI.Text = attribute.GetInformation();
                        statsListView.AddItem(textUI);
                    }
                    else
                    {
                        RichTextUI textUI = new RichTextUI(ParentNode.Scene);
                        textUI.Tooltips = attribute.GetAdditionalInformation();
                        textUI.Text = attribute.GetInformation();
                        textUI.Width = textUI.TextWidth;
                        textUI.Height = textUI.TextHeight;

                        statsListView.AddItem(textUI);

                        attributesTexts.Add(attribute.AttributeType, textUI);
                    }
                }
            }

            if (informationText == null)
            {
                informationText = new MyText(ParentNode.Scene);
            }

            informationText.Text = creature.GetInformation();
            informationText.Width = informationText.TextWidth;
            informationText.Height = informationText.TextHeight;

            statsListView.AddItem(informationText);
        }

    }
}
