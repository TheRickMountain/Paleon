using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class HutUIScript : MScript
    {
        private HutBuildingCmp hutBuilding;

        private Dictionary<MNode, CreatureCmp> nodesCreatures;

        private List<MNode> slotsList;

        public HutUIScript() : base(true)
        {
            nodesCreatures = new Dictionary<MNode, CreatureCmp>();
            slotsList = new List<MNode>();
        }

        public override void Awake()
        {
            
        }

        public override void Begin()
        {
            CreateNode(0);
            CreateNode(1);
            CreateNode(2);
            CreateNode(3);
            CreateNode(4);
            CreateNode(5);
            CreateNode(6);
            CreateNode(7);

            int countY = 0;
            int countX = 0;
            foreach(var kvp in nodesCreatures)
            {
                MNode node = kvp.Key;
                node.X = 8 + countX * 100;
                node.Y = 38 + countY * (node.Height + 5);
                ParentNode.AddChildNode(node);

                countY++;
                if(countY == 4)
                {
                    countY = 0;
                    countX++;
                }
            }
        }

        private void CreateNode(int slotIndex)
        {
            MNode node = new MNode(ParentNode.Scene);

            Slot slot = new Slot(ParentNode.Scene);
            slot.SetMetadata("slot_index", slotIndex);

            MImageCmp slotIcon = slot.GetChildByName("Icon").GetComponent<MImageCmp>();

            slotIcon.Texture = RenderManager.Pixel;
            slot.GetComponent<ButtonScript>().AddOnClickedCallback(SlotPressed);
            slot.Name = "Slot";

            node.AddChildNode(slot);

            SmallButton freeUpButton = new SmallButton(ParentNode.Scene, ResourceManager.CancelIcon);
            freeUpButton.X = slot.LocalX + slot.Width + 5;
            freeUpButton.Tooltips = Localization.GetLocalizedText("free_up");
            freeUpButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnFreeUpButtonPressed);
            node.AddChildNode(freeUpButton);

            SmallButton viewButton = new SmallButton(ParentNode.Scene, ResourceManager.EyeIcon);
            viewButton.X = freeUpButton.LocalX + freeUpButton.Width + 5;
            viewButton.Tooltips = Localization.GetLocalizedText("view_settler");
            viewButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnViewSettlerButtonPressed);
            node.AddChildNode(viewButton);

            node.Height = slot.Height;

            nodesCreatures.Add(node, null);
            slotsList.Add(slot);
        }

        private void SlotPressed(bool value, ButtonScript script)
        {
            int slotIndex = script.ParentNode.GetMetadata<int>("slot_index");

            GameplayScene.UIRootNodeScript.OpenAssignHutUI(hutBuilding, slotIndex);

            foreach(var slot in slotsList)
            {
                slot.GetComponent<ButtonScript>().SetDefaultColor(Color.White, Color.White, Color.White);
            }

            slotsList[slotIndex].GetComponent<ButtonScript>().SetDefaultColor(Engine.ORANGE, Engine.ORANGE, Engine.ORANGE);
        }

        private void OnFreeUpButtonPressed(bool value, ButtonScript script)
        {
            CreatureCmp creature = nodesCreatures[script.ParentNode.ParentNode];
            if (creature != null)
            {
                creature.WakeUp();

                hutBuilding.UnassignCreature(creature);

                UpdateView(hutBuilding);
            }
        }

        private void OnViewSettlerButtonPressed(bool value, ButtonScript script)
        {
            CreatureCmp creature = nodesCreatures[script.ParentNode.ParentNode];
            if (creature != null)
            {
                creature.WakeUp();

                CreatureCmp selectedCreature = nodesCreatures[script.ParentNode.ParentNode];

                if (selectedCreature != null)
                {
                    GameplayScene.UIRootNodeScript.OpenCreatureUI(selectedCreature);
                }
            }
        }

        public void SetHut(HutBuildingCmp hutBuilding)
        {
            this.hutBuilding = hutBuilding;

            hutBuilding.OnInsiderCreaturesChangedCallback += UpdateView;

            UpdateView(hutBuilding);

            foreach (var slot in slotsList)
            {
                slot.GetComponent<ButtonScript>().SetDefaultColor(Color.White, Color.White, Color.White);
            }
        }

        public void Close()
        {
            if(hutBuilding != null)
                hutBuilding.OnInsiderCreaturesChangedCallback -= UpdateView;
        }

        public void UpdateView(HutBuildingCmp hutBuilding)
        {
            foreach (var kvp in nodesCreatures)
            {
                MNode node = kvp.Key;
                node.Active = false;
            }

            int slotsAmount = hutBuilding.Assignable.Slots;

            for (int i = 0; i < slotsAmount; i++)
            {
                var kvp = nodesCreatures.ElementAt(i);
                MNode node = kvp.Key;

                node.Active = true;

                Slot slot = (Slot)node.GetChildByName("Slot");

                nodesCreatures[node] = null;
            }

            for (int slotIndex = 0; slotIndex < slotsAmount; slotIndex++)
            {
                var kvp = nodesCreatures.ElementAt(slotIndex);
                MNode node = kvp.Key;

                Slot slot = (Slot)node.GetChildByName("Slot");

                CreatureCmp creatureCmp = hutBuilding.GetSlotCreature(slotIndex);

                nodesCreatures[node] = creatureCmp;

                slot.GetChildByName("Icon").Active = true;

                if (creatureCmp != null)
                {
                    slot.GetChildByName("Icon").GetComponent<MImageCmp>().Texture = creatureCmp.GetBodyIcon();
                    slot.GetChildByName("Icon").GetComponent<MImageCmp>().Color = Color.White;
                    slot.Tooltips = creatureCmp.Name;
                }
                else
                {
                    slot.GetChildByName("Icon").GetComponent<MImageCmp>().Texture = ResourceManager.OwnershipIcon;
                    slot.GetChildByName("Icon").GetComponent<MImageCmp>().Color = Color.White * 0.5f;
                    slot.Tooltips = Localization.GetLocalizedText("assign_a_settler");
                }
            }
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }
        }
    }
}
