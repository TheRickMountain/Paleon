using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Technolithic
{
    public class NomadsUIScript : MScript
    {

        private BigButton acceptButton;
        private BigButton cancelButton;

        private Queue<MNode> itemsNodesPool = new Queue<MNode>();
        private List<MNode> itemsNodesToEnqueue = new List<MNode>();

        private Queue<MNode> settlersNodesPool = new Queue<MNode>();
        private List<MNode> settlersNodesToEnqueue = new List<MNode>();

        public NomadsUIScript() : base(true)
        {

        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            acceptButton = new BigButton(ParentNode.Scene, ResourceManager.AcceptIcon, false);
            acceptButton.GetChildByName("Icon").GetComponent<MImageCmp>().Color = Engine.GREEN_COLOR;
            acceptButton.Tooltips = Localization.GetLocalizedText("accept");
            acceptButton.GetComponent<ButtonScript>().AddOnClickedCallback(Accept);
            ParentNode.AddChildNode(acceptButton);

            cancelButton = new BigButton(ParentNode.Scene, ResourceManager.CancelIcon, false);
            cancelButton.GetChildByName("Icon").GetComponent<MImageCmp>().Color = Engine.RED_COLOR;
            cancelButton.Tooltips = Localization.GetLocalizedText("decline");
            cancelButton.GetComponent<ButtonScript>().AddOnClickedCallback(Cancel);
            ParentNode.AddChildNode(cancelButton);

            int startX = ParentNode.Width / 2 - (acceptButton.Width + 5 + cancelButton.Width) / 2;
            int startY = ParentNode.Height - acceptButton.Height - 5;

            acceptButton.X = startX;
            acceptButton.Y = startY;

            cancelButton.X = acceptButton.LocalX + acceptButton.Width + 5;
            cancelButton.Y = startY;    
        }

        public override void Update(int mouseX, int mouseY)
        {
            if(ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }
        }

        private void EnqueueNodes()
        {
            foreach (var node in settlersNodesToEnqueue)
            {
                settlersNodesPool.Enqueue(node);
            }

            settlersNodesToEnqueue.Clear();

            foreach (var node in itemsNodesToEnqueue)
            {
                itemsNodesPool.Enqueue(node);
            }

            itemsNodesToEnqueue.Clear();
        }

        public void Open()
        {
            NomadsManager nomadsGenerator = GameplayScene.Instance.NomadsManager;

            ListViewUIScript listView = ParentNode.GetChildByName("ListView").GetComponent<ListViewUIScript>();

            listView.Clear();

            EnqueueNodes();

            // *** Settlers ***
            foreach (var settlerInfo in nomadsGenerator.Settlers)
            {
                MNode settlerNode;

                if(settlersNodesPool.Count > 0)
                {
                    settlerNode = UpdateSettlerNode(settlersNodesPool.Dequeue(), settlerInfo);
                }
                else
                {
                    settlerNode = CreateSettlerNode(settlerInfo);
                }

                settlersNodesToEnqueue.Add(settlerNode);
                listView.AddItem(settlerNode);
            }

            // *** *** ***

            // *** Items ***
            foreach (var kvp in nomadsGenerator.Items)
            {
                Item item = kvp.Key;
                int count = kvp.Value;

                MNode itemNode;

                if (itemsNodesPool.Count > 0)
                {
                    itemNode = UpdateItemNode(itemsNodesPool.Dequeue(), item, count);
                }
                else
                {
                    itemNode = CreateItemNode(item, count);
                }

                itemsNodesToEnqueue.Add(itemNode);
                listView.AddItem(itemNode);
            }
            // *** *** ***
        }

        private MNode UpdateSettlerNode(MNode node, SettlerInfo settlerInfo)
        {
            node.GetChildByName("Body").GetComponent<MImageCmp>().Texture = ResourceManager.CreaturesTileset[settlerInfo.BodyTextureId];
            node.GetChildByName("Hair").GetComponent<MImageCmp>().Texture = ResourceManager.HairsTileset[settlerInfo.HairTextureId];
            ((MyText)node.GetChildByName("Name")).Text = settlerInfo.Name;

            return node;
        }

        private MNode CreateSettlerNode(SettlerInfo settlerInfo)
        {
            MNode element = new MNode(ParentNode.Scene);

            MImageUI bodyIcon = new MImageUI(ParentNode.Scene);
            bodyIcon.Image.Texture = ResourceManager.CreaturesTileset[settlerInfo.BodyTextureId];
            bodyIcon.Name = "Body";
            bodyIcon.Width = 48;
            bodyIcon.Height = 48;

            MImageUI hairIcon = new MImageUI(ParentNode.Scene);
            hairIcon.Image.Texture = ResourceManager.HairsTileset[settlerInfo.HairTextureId];
            hairIcon.Name = "Hair";
            hairIcon.Width = 48;
            hairIcon.Height = 48;
            hairIcon.Image.Color = settlerInfo.HairColor;

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = settlerInfo.Name;
            itemName.Name = "Name";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = bodyIcon.Width + 5;

            element.Width = 32 + 5 + 100;
            element.Height = 32;

            element.AddChildNode(bodyIcon);
            element.AddChildNode(hairIcon);
            element.AddChildNode(itemName);

            return element;
        }

        private MNode UpdateItemNode(MNode node, Item item, int count)
        {
            node.GetChildByName("Icon").GetComponent<MImageCmp>().Texture = item.Icon;
            ((MyText)node.GetChildByName("Name")).Text = $"{item.Name} x{count}";

            return node;
        }

        private MNode CreateItemNode(Item item, int count)
        {
            MNode element = new MNode(ParentNode.Scene);

            MImageUI itemIcon = new MImageUI(ParentNode.Scene);
            itemIcon.Image.Texture = item.Icon;
            itemIcon.Name = "Icon";
            itemIcon.Width = 32;
            itemIcon.Height = 32;

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = $"{item.Name} x{count}";
            itemName.Name = "Name";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = itemIcon.Width + 5;

            element.Width = 32 + 5 + 100;
            element.Height = 32;

            element.AddChildNode(itemIcon);
            element.AddChildNode(itemName);

            return element;
        }

        private void Cancel(bool value, ButtonScript script)
        {
            GameplayScene.Instance.NomadsManager.Reset();
            
            Close();

            GameplayScene.Instance.AchievementManager.UnlockAchievement(AchievementId.DENIED);
        }

        private void Accept(bool value, ButtonScript script)
        {
            GameplayScene.Instance.NomadsManager.Spawn();

            Close();
        }

        public void Close()
        {
            GameplayScene.UIRootNodeScript.CloseMainPanel();
        }
    }
}
