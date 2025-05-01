using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class PriorityUIScript : MScript
    {

        private StorageBuildingCmp selectedStorage;

        private Dictionary<int, MNode> prioritiesNodesPair = new Dictionary<int, MNode>();

        private int lastSelectedPriority = 0;

        public PriorityUIScript() : base(true)
        {
            
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            for (int i = 4; i >= -4; i--)
            {
                MNode node = CreateNode(i);
                node.SetMetadata("Priority", i);
                prioritiesNodesPair.Add(i, node);
                ParentNode.GetChildByName("ListView").GetComponent<ListViewUIScript>().AddItem(node);
            }
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }
        }

        public void Open(StorageBuildingCmp storage)
        {
            selectedStorage = storage;

            prioritiesNodesPair[lastSelectedPriority].GetComponent<ButtonScript>().SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);
            lastSelectedPriority = selectedStorage.Priority;
            prioritiesNodesPair[lastSelectedPriority].GetComponent<ButtonScript>().SetDefaultColor(Color.Orange, Color.Orange, Color.Orange);
        }

        private MNode CreateNode(int priority)
        {
            MButtonUI element = new MButtonUI(ParentNode.Scene);
            element.Image.Texture = TextureBank.UITexture.GetSubtexture(192, 192, 24, 24);
            element.Image.ImageType = ImageType.Sliced;
            element.Image.BackgroundOverlap = 2;
            element.Image.SetBorder(8, 8, 8, 8);
            element.ButtonScript.AddOnClickedCallback(OnPrioritySelectedCallback);
            element.ButtonScript.SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = priority + "";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = 8;

            element.AddChildNode(itemName);

            element.Width = ParentNode.Width - (16 + 16); // borders + scroller width
            element.Height = 34;

            return element;
        }

        private void OnPrioritySelectedCallback(bool value, ButtonScript buttonScript)
        {
            int priority = buttonScript.ParentNode.GetMetadata<int>("Priority");

            prioritiesNodesPair[lastSelectedPriority].GetComponent<ButtonScript>().SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);
            lastSelectedPriority = priority;
            prioritiesNodesPair[lastSelectedPriority].GetComponent<ButtonScript>().SetDefaultColor(Color.Orange, Color.Orange, Color.Orange);

            selectedStorage.SetPriority(priority);
        }
    }
}
