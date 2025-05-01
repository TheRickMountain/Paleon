using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;


namespace Technolithic
{
    public class AssignHutUIScript : MScript
    {
        private Dictionary<CreatureCmp, MNode> creatureNodePairs = new Dictionary<CreatureCmp, MNode>();

        private HutBuildingCmp currentHut;
        private int slotIndex;

        public AssignHutUIScript() : base(true)
        {
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
        }

        public override void Update(int mouseX, int mouseY)
        {
        }

        public void Open(HutBuildingCmp currentHut, int slotIndex)
        {
            this.currentHut = currentHut;
            this.slotIndex = slotIndex;

            UpdateElements();
        }

        private void UpdateElements()
        {
            ListViewUIScript listViewUIScript = ParentNode.GetChildByName("ListView").GetComponent<ListViewUIScript>();
            listViewUIScript.Clear();

            foreach (var creature in GameplayScene.Instance.CreatureLayer.Entities
                .Where(x => x.Get<SettlerCmp>() != null)
                .Select(x => x.Get<SettlerCmp>())
                .Where(x => x.IsDead == false)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id))
            {
                MNode element;

                if (creatureNodePairs.ContainsKey(creature))
                {
                    element = creatureNodePairs[creature];
                }
                else
                {
                    element = CreateNode();

                    creatureNodePairs.Add(creature, element);
                }

                ((CreatureButton)element).SetCreature(creature);

                MImageCmp ownershipIndicatorImage = ((MImageUI)element.GetChildByName("ownership_indicator")).Image;

                ownershipIndicatorImage.Visible = creature.AssignedHut != null;
                ownershipIndicatorImage.Color = creature.AssignedHut == currentHut ? Engine.GREEN_COLOR : Color.White;

                if (creature.AssignedHut != null)
                {
                    if (creature.AssignedHut == currentHut)
                    {
                        element.Tooltips = Localization.GetLocalizedText("x_is_already_assigned_to_this_hut", creature.Name);
                    }
                    else
                    {
                        element.Tooltips = Localization.GetLocalizedText("x_is_already_assigned_to_another_hut", creature.Name);
                    }
                }
                else
                {
                    element.Tooltips = Localization.GetLocalizedText("assign_x_to_this_hut", creature.Name);
                }

                listViewUIScript.AddItem(element);
            }

            listViewUIScript.IsDirty = false;
            listViewUIScript.UpdateView(true);
        }

        private MNode CreateNode()
        {
            CreatureButton creatureButton = new CreatureButton(ParentNode.Scene);
            creatureButton.Name = "creature_button";
            creatureButton.Width = 300;
            creatureButton.ButtonScript.AddOnClickedCallback(OnCreatureButtonPressed);

            MImageUI ownershipIndicator = new MImageUI(ParentNode.Scene);
            ownershipIndicator.Name = "ownership_indicator";
            ownershipIndicator.Width = 32;
            ownershipIndicator.Height = 32;
            ownershipIndicator.X = creatureButton.Width - (ownershipIndicator.Width + 8);
            ownershipIndicator.Y = 8;
            ownershipIndicator.Image.Texture = ResourceManager.HutIcon;
            creatureButton.AddChildNode(ownershipIndicator);

            return creatureButton;
        }

        private void OnCreatureButtonPressed(bool value, ButtonScript buttonScript)
        {
            CreatureCmp creature = ((CreatureButton)buttonScript.ParentNode).Creature;

            currentHut.AssignCreature(creature, slotIndex);

            UpdateElements();

            GameplayScene.UIRootNodeScript.ParentNode.GetChildByName("Hut")
                .GetComponent<HutUIScript>().UpdateView(currentHut);
        }

    }
}
