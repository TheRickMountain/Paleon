using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class TradingUIScript : MScript
    {

        private ListViewUIScript traderListViewScript;
        private ListViewUIScript stockListViewScript;

        private ListViewUIScript toBuyListViewScript;
        private ListViewUIScript toSellListViewScript;

        private Dictionary<ITradable, MNode> traderItemsNodes;
        private Dictionary<MNode, ITradable> nodesTraderItems;

        private Dictionary<ITradable, MNode> stockItemsNodes;
        private Dictionary<MNode, ITradable> nodesStockItems;

        private Dictionary<ITradable, MNode> toBuyItemsNodes;
        private Dictionary<MNode, ITradable> nodesToBuyItems;

        private Dictionary<ITradable, MNode> toSellItemsNodes;
        private Dictionary<MNode, ITradable> nodesToSellItems;

        private MyText totalToBuy;
        private MyText totalToSell;

        private BigButton acceptButton;

        private SmallButton closeButton;

        private TradingSystem tradingSystem;

        public TradingUIScript() : base(true)
        {
            tradingSystem = GameplayScene.Instance.TradingSystem;
        }

        public override void Awake()
        {

        }

        public override void Begin()
        {
            // Кнопка закрытия окна
            closeButton = new SmallButton(ParentNode.Scene, ResourceManager.CancelIcon);
            closeButton.X = ParentNode.Width - closeButton.Width;
            closeButton.Y = 0;
            closeButton.GetComponent<ButtonScript>().AddOnClickedCallback(Close);
            ParentNode.AddChildNode(closeButton);

            traderItemsNodes = new Dictionary<ITradable, MNode>();
            nodesTraderItems = new Dictionary<MNode, ITradable>();

            stockItemsNodes = new Dictionary<ITradable, MNode>();
            nodesStockItems = new Dictionary<MNode, ITradable>();

            toBuyItemsNodes = new Dictionary<ITradable, MNode>();
            nodesToBuyItems = new Dictionary<MNode, ITradable>();

            toSellItemsNodes = new Dictionary<ITradable, MNode>();
            nodesToSellItems = new Dictionary<MNode, ITradable>();

            // Trader list view
            ListViewUI traderListView = new ListViewUI(ParentNode.Scene, 300, 32, 12, 1);
            traderListView.X = 8;
            traderListView.Y = 32 + 8;
            ParentNode.AddChildNode(traderListView);

            traderListViewScript = traderListView.GetComponent<ListViewUIScript>();

            // To buy list view
            ListViewUI toBuyListView = new ListViewUI(ParentNode.Scene, 300, 32, 5, 1);
            toBuyListView.X = traderListView.LocalX + traderListView.Width + 5;
            toBuyListView.Y = 32 + 8 + 42;
            ParentNode.AddChildNode(toBuyListView);

            toBuyListViewScript = toBuyListView.GetComponent<ListViewUIScript>();

            // Stock list view
            ListViewUI stockListView = new ListViewUI(ParentNode.Scene, 300, 32, 12, 1);
            stockListView.X = toBuyListView.LocalX + toBuyListView.Width + 5;
            stockListView.Y = 32 + 8;
            ParentNode.AddChildNode(stockListView);

            stockListViewScript = stockListView.GetComponent<ListViewUIScript>();

            // To sell list view
            ListViewUI toSellListView = new ListViewUI(ParentNode.Scene, 300, 32, 5, 1);
            toSellListView.X = traderListView.LocalX + traderListView.Width + 5;
            toSellListView.Y = toBuyListView.LocalY + toBuyListView.Height + 5 + 32;
            ParentNode.AddChildNode(toSellListView);

            toSellListViewScript = toSellListView.GetComponent<ListViewUIScript>();

            totalToBuy = new MyText(ParentNode.Scene);
            totalToBuy.Text = "";
            totalToBuy.X = toBuyListView.LocalX;
            totalToBuy.Y = 32 + 8;
            ParentNode.AddChildNode(totalToBuy);

            totalToSell = new MyText(ParentNode.Scene);
            totalToSell.Text = "";
            totalToSell.X = toSellListView.LocalX;
            totalToSell.Y = toBuyListView.LocalY + toBuyListView.Height + 5;
            ParentNode.AddChildNode(totalToSell);

            acceptButton = new BigButton(ParentNode.Scene, ResourceManager.AcceptIcon, false);
            acceptButton.X = ParentNode.Width / 2 - acceptButton.Width / 2;
            acceptButton.Y = ParentNode.Height - acceptButton.Height - 8;
            acceptButton.GetChildByName("Icon").GetComponent<MImageCmp>().Color = Engine.GREEN_COLOR;
            acceptButton.ButtonScript.AddOnClickedCallback(Accept);
            acceptButton.ButtonScript.SoundEffect = ResourceManager.GetSoundEffect("buy_sell").CreateInstance();
            ParentNode.AddChildNode(acceptButton);
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }
        }

        private MNode CreateItemNode(ITradable item, Action<bool, ButtonScript> function)
        {
            MButtonUI element = new MButtonUI(ParentNode.Scene);

            element.Image.Texture = TextureBank.UITexture.GetSubtexture(192, 192, 24, 24);
            element.Image.ImageType = ImageType.Sliced;
            element.Image.SetBorder(8, 8, 8, 8);
            element.Image.BackgroundOverlap = 2;
            element.Tooltips = item.GetMarketInformation();

            element.ButtonScript.AddOnClickedCallback(function);
            element.ButtonScript.SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);
            element.Width = 290;
            element.Height = 34;

            MImageUI itemIcon = new MImageUI(ParentNode.Scene);
            itemIcon.Image.Texture = item.GetMarketIcon();
            itemIcon.Name = "Icon";
            itemIcon.Width = 32;
            itemIcon.Height = 32;
            itemIcon.X = 8;
            itemIcon.Y = 2;

            RichTextUI itemCount = new RichTextUI(ParentNode.Scene);
            itemCount.Text = item.GetMarketName();
            itemCount.Name = "Name";
            itemCount.Width = 100;
            itemCount.Height = 32;
            itemCount.X = itemIcon.LocalX + itemIcon.Width + 5;
            itemCount.Y = 2;

            element.AddChildNode(itemIcon);
            element.AddChildNode(itemCount);

            return element;
        }

        private void SelectTraderItem(bool value, ButtonScript buttonScript)
        {
            int count = 1;

            if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftShift))
            {
                count = 5;
            }
            else if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftControl))
            {
                count = 10;
            }

            ITradable item = nodesTraderItems[buttonScript.ParentNode];

            tradingSystem.RearrangeItems(tradingSystem.ToBuyItems, tradingSystem.TraderItems, item, count);

            UpdateTraderAndToBuyItems();
        }

        private void SelectStockItem(bool value, ButtonScript buttonScript)
        {
            int count = 1;

            if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftShift))
            {
                count = 5;
            }
            else if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftControl))
            {
                count = 10;
            }

            ITradable item = nodesStockItems[buttonScript.ParentNode];

            tradingSystem.RearrangeItems(tradingSystem.ToSellItems, tradingSystem.StockItems, item, count);

            UpdateStockAndToSellItems();
        }

        private void SelectToBuyItem(bool value, ButtonScript buttonScript)
        {
            int count = 1;

            if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftShift))
            {
                count = 5;
            }
            else if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftControl))
            {
                count = 10;
            }

            ITradable item = nodesToBuyItems[buttonScript.ParentNode];

            tradingSystem.RearrangeItems(tradingSystem.TraderItems, tradingSystem.ToBuyItems, item, count);

            UpdateTraderAndToBuyItems();
        }

        private void SelectToSellItem(bool value, ButtonScript buttonScript)
        {
            int count = 1;

            if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftShift))
            {
                count = 5;
            }
            else if (MInput.Keyboard.Check(Microsoft.Xna.Framework.Input.Keys.LeftControl))
            {
                count = 10;
            }

            ITradable item = nodesToSellItems[buttonScript.ParentNode];

            tradingSystem.RearrangeItems(tradingSystem.StockItems, tradingSystem.ToSellItems, item, count);

            UpdateStockAndToSellItems();
        }

        private void Accept(bool value, ButtonScript script)
        {
            tradingSystem.Trade();

            UpdateTraderAndToBuyItems();
            UpdateStockAndToSellItems();
        }

        private void UpdateToBuyItemsListView()
        {
            toBuyListViewScript.Clear();

            foreach (var kvp in tradingSystem.ToBuyItems)
            {
                ITradable item = kvp.Key;
                int count = kvp.Value;

                if (toBuyItemsNodes.ContainsKey(item) == false)
                {
                    MNode node = CreateItemNode(item, SelectToBuyItem);

                    toBuyItemsNodes.Add(item, node);
                    nodesToBuyItems.Add(node, item);
                }

                ((RichTextUI)toBuyItemsNodes[item].GetChildByName("Name")).Text = $"{item.GetMarketName()} {count}";

                if (count > 0)
                    toBuyListViewScript.AddItem(toBuyItemsNodes[item]);
            }

            toBuyListViewScript.IsDirty = false;
            toBuyListViewScript.UpdateView(true);
        }

        private void UpdateTraderItemsListView()
        {
            traderListViewScript.Clear();

            foreach (var kvp in tradingSystem.TraderItems)
            {
                ITradable item = kvp.Key;
                int count = kvp.Value;

                if (traderItemsNodes.ContainsKey(item) == false)
                {
                    MNode node = CreateItemNode(item, SelectTraderItem);

                    traderItemsNodes.Add(item, node);
                    nodesTraderItems.Add(node, item);
                }

                ((RichTextUI)traderItemsNodes[item].GetChildByName("Name")).Text = $"{item.GetMarketName()} {count}";

                traderListViewScript.AddItem(traderItemsNodes[item]);
            }

            traderListViewScript.IsDirty = false;
            traderListViewScript.UpdateView(true);
        }

        private void UpdateToSellItemsListView()
        {
            toSellListViewScript.Clear();

            foreach (var kvp in tradingSystem.ToSellItems)
            {
                ITradable item = kvp.Key;
                int count = kvp.Value;

                if (toSellItemsNodes.ContainsKey(item) == false)
                {
                    MNode node = CreateItemNode(item, SelectToSellItem);

                    toSellItemsNodes.Add(item, node);
                    nodesToSellItems.Add(node, item);
                }

                ((RichTextUI)toSellItemsNodes[item].GetChildByName("Name")).Text = $"{item.GetMarketName()} {count}";

                if (count > 0)
                    toSellListViewScript.AddItem(toSellItemsNodes[item]);
            }

            toSellListViewScript.IsDirty = false;
            toSellListViewScript.UpdateView(true);
        }

        private void UpdateStockItemsListView()
        {
            stockListViewScript.Clear();

            foreach (var kvp in tradingSystem.StockItems)
            {
                ITradable item = kvp.Key;
                int count = kvp.Value;

                if (stockItemsNodes.ContainsKey(item) == false)
                {
                    MNode node = CreateItemNode(item, SelectStockItem);

                    stockItemsNodes.Add(item, node);
                    nodesStockItems.Add(node, item);
                }

                ((RichTextUI)stockItemsNodes[item].GetChildByName("Name")).Text = $"{item.GetMarketName()} {count}";

                stockListViewScript.AddItem(stockItemsNodes[item]);
            }

            stockListViewScript.IsDirty = false;
            stockListViewScript.UpdateView(true);
        }

        public void Open(TradingPostBuildingCmp tradingPost)
        {
            tradingSystem.SetActiveTradingPost(tradingPost);
            
            UpdateTraderAndToBuyItems();
            UpdateStockAndToSellItems();
        }

        private void UpdateTraderAndToBuyItems()
        {
            totalToBuy.Text = $"{Localization.GetLocalizedText("buying")}: {tradingSystem.GetTotalToBuyPrice()}";
            UpdateToBuyItemsListView();
            UpdateTraderItemsListView();

            UpdateAcceptButton();
        }

        private void UpdateStockAndToSellItems()
        {
            totalToSell.Text = $"{Localization.GetLocalizedText("selling")}: {tradingSystem.GetTotalToSellPrice()}";
            UpdateToSellItemsListView();
            UpdateStockItemsListView();

            UpdateAcceptButton();
        }

        private void UpdateAcceptButton()
        {
            if (tradingSystem.CanCompleteTrade())
            {
                acceptButton.GetComponent<ButtonScript>().IsDisabled = false;
            }
            else
            {
                acceptButton.GetComponent<ButtonScript>().IsDisabled = true;
            }
        }

        public void Close(bool value, ButtonScript buttonScript)
        {
            GameplayScene.UIRootNodeScript.CloseMainPanel();
        }

    }
}