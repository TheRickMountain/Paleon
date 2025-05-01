using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Technolithic
{
    public class AssignPetUIScript : MScript
    {

        private Dictionary<CreatureCmp, MNode> creatureNodePairs = new Dictionary<CreatureCmp, MNode>();

        private CreatureCmp parentCreature;

        public AssignPetUIScript() : base(true)
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

        public void Open(CreatureCmp parentCreature)
        {
            this.parentCreature = parentCreature;

            UpdateElements();
        }

        private void UpdateElements()
        {
            ListViewUIScript listViewUIScript = ParentNode.GetChildByName("ListView").GetComponent<ListViewUIScript>();
            listViewUIScript.Clear();

            foreach (var creature in GameplayScene.Instance.CreatureLayer.Entities
                .Where(x => x.Get<AnimalCmp>() != null)
                .Select(x => x.Get<AnimalCmp>())
                .Where(x => x.IsDead == false && x.IsDomesticated && x.AgeState == AgeState.Adult && x.AnimalTemplate.IsPet)
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

                if (creature.Parent != null)
                {
                    ownershipIndicatorImage.Visible = true;

                    if(creature.Parent == parentCreature)
                    {
                        ownershipIndicatorImage.Color = Engine.GREEN_COLOR;
                    }
                    else
                    {
                        ownershipIndicatorImage.Color = Color.White;
                    }
                }
                else
                {
                    ownershipIndicatorImage.Visible = false;
                }

                if (creature.Parent != null)
                {
                    if (creature.Parent != parentCreature)
                    {
                        element.Tooltips = Localization.GetLocalizedText("assigned_to", creature.Parent.Name);
                    }
                    else
                    {
                        element.Tooltips = Localization.GetLocalizedText("unassign_from", creature.Parent.Name);
                    }
                }
                else
                {
                    element.Tooltips = Localization.GetLocalizedText("assign_to", parentCreature.Name);
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
            ownershipIndicator.Image.Texture = ResourceManager.OwnershipIcon;
            creatureButton.AddChildNode(ownershipIndicator);

            return creatureButton;
        }

        private void OnCreatureButtonPressed(bool value, ButtonScript buttonScript)
        {
            CreatureCmp childCreature = ((CreatureButton)buttonScript.ParentNode).Creature;

            if (childCreature.Parent == null)
            {
                childCreature.Parent = parentCreature;

                if (parentCreature.Child != null)
                {
                    parentCreature.Child.Parent = null;
                }

                parentCreature.Child = childCreature;
            }
            else if (childCreature.Parent == parentCreature)
            {
                childCreature.Parent = null;
                parentCreature.Child = null;
            }
            else
            {
                if(childCreature.Parent != null)
                {
                    childCreature.Parent.Child = null;
                }

                childCreature.Parent = parentCreature;

                if(parentCreature.Child != null)
                {
                    parentCreature.Child.Parent = null;
                }

                parentCreature.Child = childCreature;
            }

            UpdateElements();
        }
    }
}
