using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{ 

    public class CreatureUIScript : MScript
    {
        private CreatureCmp selectedCreature;

        private MNode huntButton;
        private MNode domesticateButton;
        private MNode slaughterButton;
        private MNode gatherProductButton;
        private MNode assignPetButton;

        private Dictionary<Tab, MNode> tabNodes = new Dictionary<Tab, MNode>();

        private Dictionary<AttributeType, RichTextUI> attributesTexts = new Dictionary<AttributeType, RichTextUI>();

        private MyText informationText;

        private ListViewUIScript statsListView;

        private SmallButton renameButton;

        public CreatureUIScript() : base(true)
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

            huntButton = new BigButton(ParentNode.Scene, ResourceManager.HuntIcon, true);
            huntButton.Tooltips = Localization.GetLocalizedText("hunt");
            huntButton.GetComponent<ButtonScript>().AddOnClickedCallback(Hunt);

            domesticateButton = new BigButton(ParentNode.Scene, ResourceManager.DomesticateIcon, true);
            domesticateButton.GetComponent<ButtonScript>().AddOnClickedCallback(Domesticate);

            slaughterButton = new BigButton(ParentNode.Scene, ResourceManager.SlaughterIcon, true);
            slaughterButton.Tooltips = Localization.GetLocalizedText("slaughter");
            slaughterButton.GetComponent<ButtonScript>().AddOnClickedCallback(Slaughter);

            gatherProductButton = new BigButton(ParentNode.Scene, RenderManager.Pixel, true);
            gatherProductButton.GetComponent<ButtonScript>().AddOnClickedCallback(GatherProduct);

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

            if (creature is AnimalCmp)
            {
                AnimalCmp animal = (AnimalCmp)creature;

                if (animal.CanDomesticate)
                {
                    domesticateButton.GetComponent<ButtonScript>().IsSelected = animal.Domesticate;

                    int domesticationChance = animal.AnimalTemplate.DomesticationData.Chance;

                    domesticateButton.Tooltips = $"{Localization.GetLocalizedText("domesticate")}\n" +
                        $"{Localization.GetLocalizedText("chance")}: {domesticationChance}%";

                    buttonsListView.AddItem(domesticateButton);
                }

                if (animal.CanHunt)
                {
                    huntButton.GetComponent<ButtonScript>().IsSelected = animal.Hunt;
                    buttonsListView.AddItem(huntButton);
                }

                if (animal.CanSlaughter)
                {
                    slaughterButton.GetComponent<ButtonScript>().IsSelected = animal.Slaughter;
                    buttonsListView.AddItem(slaughterButton);
                }

                if (animal.CanGatherProduct)
                {
                    Item product = animal.AnimalTemplate.AnimalProduct.Product;

                    gatherProductButton.GetComponent<ButtonScript>().IsSelected = animal.GatherProduct;
                    gatherProductButton.GetChildByName("Icon").GetComponent<MImageCmp>().Texture = product.Icon;
                    gatherProductButton.Tooltips = $"{Localization.GetLocalizedText("gather")}: {product.Name}";
                    buttonsListView.AddItem(gatherProductButton);
                }
            }
            else if(creature is SettlerCmp)
            {
                buttonsListView.AddItem(assignPetButton);
            }
        }

        private void Hunt(bool value, ButtonScript script)
        {
            AnimalCmp animal = selectedCreature as AnimalCmp;
            animal.Hunt = value;

            if (value)
            {
                domesticateButton.GetComponent<ButtonScript>().IsSelected = false;
            }
        }

        private void Slaughter(bool value, ButtonScript script)
        {
            AnimalCmp animal = selectedCreature as AnimalCmp;
            animal.Slaughter = value;
        }

        private void Domesticate(bool value, ButtonScript script)
        {
            AnimalCmp animal = selectedCreature as AnimalCmp;
            animal.Domesticate = value;

            if (value)
            {
                huntButton.GetComponent<ButtonScript>().IsSelected = false;
            }
        }

        private void GatherProduct(bool value, ButtonScript script)
        {
            AnimalCmp animal = selectedCreature as AnimalCmp;
            animal.GatherProduct = value;
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
