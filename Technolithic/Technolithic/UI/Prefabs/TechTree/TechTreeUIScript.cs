using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Technolithic
{
    public class TechTreeUIScript : MScript
    {

        private SmallButton closeButton;

        private Dictionary<MNode, Technology> nodeTechPair = new Dictionary<MNode, Technology>();
        private Dictionary<Technology, MNode> techNodePair = new Dictionary<Technology, MNode>();
        private Dictionary<Age, MNode> ageNodePair = new Dictionary<Age, MNode>();
        private Dictionary<MNode, Age> nodeAgePair = new Dictionary<MNode, Age>();

        private Dictionary<MNode, Technology> listViewNodeTechPair = new Dictionary<MNode, Technology>();
        private Dictionary<Technology, MNode> listViewTechNodePair = new Dictionary<Technology, MNode>();

        private MNode parentScrollableNode;
        private MNode childScrollableNode;

        private Vector2 mouseLastPosition;

        private MyText currextXpText;
        private MImageUI currentXpIcon;

        private ListViewUIScript listView;

        private SortedList<int, List<Technology>> sortedTechnologies = new SortedList<int, List<Technology>>();

        private List<LineUI> lines = new List<LineUI>();

        private float unlockingProgress = 0;
        private float unlockingTime = 100;

        public TechTreeUIScript() : base(true)
        {
            
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

            currextXpText = new MyText(ParentNode.Scene);
            currextXpText.Text = "Progress points:";
            currextXpText.Outlined = true;
            currextXpText.Width = currextXpText.TextWidth;
            currextXpText.Height = currextXpText.TextHeight;
            currextXpText.X = ParentNode.Width / 2 - currextXpText.Width / 2;
            currextXpText.Y = 40;
            currextXpText.Color = Color.White;
            ParentNode.AddChildNode(currextXpText);

            currentXpIcon = new MImageUI(ParentNode.Scene);
            currentXpIcon.GetComponent<MImageCmp>().Texture = ItemDatabase.GetItemByName("knowledge_points").Icon;
            currentXpIcon.Width = 32;
            currentXpIcon.Height = 32;
            currentXpIcon.Y = 40;
            ParentNode.AddChildNode(currentXpIcon);

            listView = ParentNode.GetChildByName("ListView").GetComponent<ListViewUIScript>();
            listView.ParentNode.Y = currextXpText.LocalY + currextXpText.Height + 5;

            parentScrollableNode = new MImageUI(ParentNode.Scene);
            parentScrollableNode.GetComponent<MImageCmp>().Color = new Color(32, 32, 32);
            parentScrollableNode.GetComponent<MImageCmp>().Texture = RenderManager.Pixel;
            parentScrollableNode.X = listView.ParentNode.LocalX + listView.ParentNode.Width;
            parentScrollableNode.Y = currextXpText.LocalY + currextXpText.Height + 5;
            parentScrollableNode.Width = ParentNode.Width - 16 - listView.ParentNode.Width;
            parentScrollableNode.Height = ParentNode.Height - currextXpText.LocalY - currextXpText.Height - 5 - 8;
            parentScrollableNode.ClipContent = true;
            ParentNode.AddChildNode(parentScrollableNode);

            childScrollableNode = new MNode(ParentNode.Scene);
            parentScrollableNode.AddChildNode(childScrollableNode);

            ProgressTree levelSystem = GameplayScene.Instance.ProgressTree;

            // инициализация sorted technologies
            foreach (var kvp in levelSystem.TechnologiesStates)
            {
                Technology technology = kvp.Key;

                if (sortedTechnologies.ContainsKey(technology.Order) == false)
                {
                    sortedTechnologies.Add(technology.Order, new List<Technology>());
                }
            }

            // Создание всех нод технологий
            foreach (var kvp in levelSystem.TechnologiesStates)
            {
                Technology technology = kvp.Key;

                MNode technologyNode = CreateTechnologyNode(technology);
                technologyNode.Active = false;

                nodeTechPair.Add(technologyNode, technology);
                techNodePair.Add(technology, technologyNode);

                MNode technologyListViewNode = CreateTechnologyListViewNode(technology);

                listViewNodeTechPair.Add(technologyListViewNode, technology);
                listViewTechNodePair.Add(technology, technologyListViewNode);

                if (ageNodePair.ContainsKey(technology.Age) == false)
                {
                    MNode ageNode = CreateAgeListViewNode(technology.Age);
                    ageNodePair.Add(technology.Age, ageNode);
                    nodeAgePair.Add(ageNode, technology.Age);
                }
            }

            // Добавление всех нод технологий в childScrollableNode
            foreach (var kvp in levelSystem.TechnologiesStates)
            {
                Technology technology = kvp.Key;

                MNode node = techNodePair[technology];
                childScrollableNode.AddChildNode(node);
            }
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (MInput.Mouse.PressedLeftButton)
            {
                mouseLastPosition = new Vector2(MInput.Mouse.X, MInput.Mouse.Y);
            }

            if (MInput.Mouse.CheckLeftButton)
            {
                Vector2 mouseNewPosition = new Vector2(MInput.Mouse.X, MInput.Mouse.Y);

                //childScrollableNode.X = (int)(childScrollableNode.LocalX + (mouseNewPosition.X - mouseLastPosition.X));
                childScrollableNode.Y = (int)(childScrollableNode.LocalY + (mouseNewPosition.Y - mouseLastPosition.Y));

                mouseLastPosition = mouseNewPosition;
            }

            if(MInput.Mouse.ReleasedLeftButton)
            {
                unlockingProgress = 0;
            }

            if (ParentNode.Intersects(mouseX, mouseY))
            {
                GameplayScene.MouseOnUI = true;
            }
        }

        public void Open()
        {
            UpdateProgressPointsText();
            UpdateNodes();

            SortListView();
        }

        private void SortListView()
        {
            listView.Clear();

            foreach(var ageNodeKVP in ageNodePair)
            {
                Age age = ageNodeKVP.Key;

                listView.AddItem(ageNodeKVP.Value);

                foreach (var kvp in GameplayScene.Instance.ProgressTree.TechnologiesStates)
                {
                    Technology technology = kvp.Key;
                    TechnologyState technologyState = kvp.Value;

                    if (technologyState == TechnologyState.ReadyToUnlock && technology.Age == age)
                    {
                        listView.AddItem(listViewTechNodePair[technology]);
                    }
                }

                foreach (var kvp in GameplayScene.Instance.ProgressTree.TechnologiesStates)
                {
                    Technology technology = kvp.Key;
                    TechnologyState technologyState = kvp.Value;

                    if (technologyState == TechnologyState.Locked && technology.Age == age)
                    {
                        listView.AddItem(listViewTechNodePair[technology]);
                    }
                }

                foreach (var kvp in GameplayScene.Instance.ProgressTree.TechnologiesStates)
                {
                    Technology technology = kvp.Key;
                    TechnologyState technologyState = kvp.Value;

                    if (technologyState == TechnologyState.Unlocked && technology.Age == age)
                    {
                        listView.AddItem(listViewTechNodePair[technology]);
                    }
                }
            }

            
        }

        private void UpdateProgressPointsText()
        {
            currextXpText.Text = $"{GameplayScene.Instance.ProgressTree.CurrentExp}";
            currentXpIcon.X = currextXpText.LocalX + currextXpText.TextWidth + 5;
        }

        private void UpdateNodes()
        {
            foreach (var kvp in GameplayScene.Instance.ProgressTree.TechnologiesStates)
            {
                Technology technology = kvp.Key;
                TechnologyState technologyState = kvp.Value;

                MNode techNode = techNodePair[technology];
                MNode listViewTechNode = listViewTechNodePair[technology];

                switch (technologyState)
                {
                    case TechnologyState.Locked:
                        techNode.GetComponent<ButtonScript>().SetDefaultColor(Engine.RED_COLOR, Engine.RED_COLOR, Engine.RED_COLOR);
                        listViewTechNode.GetComponent<ButtonScript>().SetDefaultColor(Engine.RED_COLOR, Engine.RED_COLOR, Engine.RED_COLOR);
                        break;
                    case TechnologyState.ReadyToUnlock:
                        techNode.GetComponent<ButtonScript>().SetDefaultColor(Color.White, Color.White, Color.White);
                        listViewTechNode.GetComponent<ButtonScript>().SetDefaultColor(Color.White, Color.White, Color.White);
                        break;
                    case TechnologyState.Unlocked:
                        techNode.GetComponent<ButtonScript>().SetDefaultColor(Engine.GREEN_COLOR, Engine.GREEN_COLOR, Engine.GREEN_COLOR);
                        listViewTechNode.GetComponent<ButtonScript>().SetDefaultColor(Engine.GREEN_COLOR, Engine.GREEN_COLOR, Engine.GREEN_COLOR);
                        break;
                }

            }
        }

        private void ClearSortedTechnologies()
        {
            foreach(var kvp in sortedTechnologies)
            {
                kvp.Value.Clear();
            }
        }

        private void ArrangeSortedTechnologiesNodes()
        {
            int count = 0;
            int startY = 0;
            foreach(var kvp in sortedTechnologies.Reverse())
            {
                List<Technology> technologies = kvp.Value;

                if (technologies.Count == 0)
                    continue;

                int totalWidth = 0;

                foreach(Technology technology in technologies)
                {
                    MNode technologyNode = techNodePair[technology];
                    totalWidth += technologyNode.Width;
                }

                totalWidth += (technologies.Count - 1) * 5;

                int startX = parentScrollableNode.Width / 2 - totalWidth / 2;

                MNode lastTechnologyNode = null;

                int highestNodeHeight = 0;

                foreach (Technology technology in technologies)
                {
                    MNode technologyNode = techNodePair[technology];
                    if (lastTechnologyNode == null)
                    {
                        technologyNode.X = startX;
                    }
                    else
                    {
                        technologyNode.X = lastTechnologyNode.LocalX + lastTechnologyNode.Width + 5;
                    }

                    lastTechnologyNode = technologyNode;

                    technologyNode.Y = startY;

                    if(technologyNode.Height > highestNodeHeight)
                    {
                        highestNodeHeight = technologyNode.Height;
                    }
                }

                startY += highestNodeHeight + 64;

                count++;
            }
        }

        private void ResetScrollPosition()
        {
            childScrollableNode.Y = 0;
        }

        private void CreateLinesBetweenTechnologies()
        {
            foreach (var line in lines)
            {
                childScrollableNode.RemoveChild(line);
            }

            lines.Clear();

            foreach (var kvp in sortedTechnologies)
            {
                foreach(Technology childTechnology in kvp.Value)
                {
                    if (childTechnology.ParentTechnologies == null)
                        continue;

                    MNode childNode = techNodePair[childTechnology];

                    foreach(var parentTechnology in childTechnology.ParentTechnologies)
                    {
                        MNode parentNode = techNodePair[parentTechnology];

                        int childX = childNode.Width / 2 + childNode.LocalX;
                        int childY = childNode.LocalY + childNode.Height;

                        int parentX = parentNode.Width / 2 + parentNode.LocalX;
                        int parentY = parentNode.LocalY;

                        LineUI lineUI = new LineUI(ParentNode.Scene, new Vector2(childX, childY), new Vector2(parentX, parentY), Color.White, 2, true);
                        lines.Add(lineUI);
                        childScrollableNode.AddChildNode(lineUI);
                    }
                }
            }
        }

        private void CreateTechnologyChain(bool value, ButtonScript buttonScript)
        {
            ResetScrollPosition();

            ClearSortedTechnologies();

            foreach(var kvp in nodeTechPair)
            {
                MNode node = kvp.Key;
                node.Active = false;
            }

            Technology technology = listViewNodeTechPair[buttonScript.ParentNode];

            if(sortedTechnologies[technology.Order].Contains(technology) == false)
            {
                sortedTechnologies[technology.Order].Add(technology);
            }

            SortParentTechnologies(technology);

            ActivateSortedTechnologies();

            ArrangeSortedTechnologiesNodes();

            CreateLinesBetweenTechnologies();
        }

        private void ActivateSortedTechnologies()
        {
            foreach(var kvp in sortedTechnologies)
            {
                foreach(Technology technology in kvp.Value)
                {
                    techNodePair[technology].Active = true;
                }
            }
        }

        private void SortParentTechnologies(Technology childTechnology)
        {
            if (childTechnology.ParentTechnologies == null)
                return;

            int maxOrder = 0;

            foreach(var parentTechnology in childTechnology.ParentTechnologies)
            {
                maxOrder = Math.Max(maxOrder, parentTechnology.Order);
            }

            foreach (var parentTechnology in childTechnology.ParentTechnologies)
            {
                if (sortedTechnologies[maxOrder].Contains(parentTechnology) == false)
                {
                    sortedTechnologies[maxOrder].Add(parentTechnology);
                }

                SortParentTechnologies(parentTechnology);
            }
        }

        private MNode CreateAgeListViewNode(Age age)
        {
            MNode mNode = new MNode(ParentNode.Scene);

            MyText title = new MyText(ParentNode.Scene);
            title.X = 8;
            title.Y = 8;
            title.Text = AgeInfo.GetAgeName(age);
            title.Width = title.TextWidth;
            mNode.AddChildNode(title);

            mNode.Width = listView.ParentNode.Width - 16;
            mNode.Height = 48;

            return mNode;
        }

        private MNode CreateTechnologyListViewNode(Technology technology)
        {
            MButtonUI button = new MButtonUI(ParentNode.Scene);

            button.Image.Texture = TextureBank.UITexture.GetSubtexture(144, 192, 24, 24);
            button.Image.ImageType = ImageType.Sliced;
            button.Image.SetBorder(8, 8, 8, 8);
            button.Image.BackgroundOverlap = 2;
            button.ButtonScript.AddOnClickedCallback(CreateTechnologyChain);

            MyText technologyNameText = new MyText(ParentNode.Scene);
            technologyNameText.X = 8;
            technologyNameText.Y = 8;
            technologyNameText.Text = technology.Name;
            technologyNameText.Width = technologyNameText.TextWidth;
            button.AddChildNode(technologyNameText);

            button.Width = listView.ParentNode.Width - 16;
            button.Height = 48;

            MImageUI knowledgePointsIcon = new MImageUI(ParentNode.Scene);
            knowledgePointsIcon.Image.Texture = ItemDatabase.GetItemByName("knowledge_points").Icon;
            knowledgePointsIcon.Width = 32;
            knowledgePointsIcon.Height = 32;
            knowledgePointsIcon.X = button.Width - knowledgePointsIcon.Width - 8;
            knowledgePointsIcon.Tooltips = ItemDatabase.GetItemByName("knowledge_points").Name;
            knowledgePointsIcon.Y = 4;

            button.AddChildNode(knowledgePointsIcon);

            MyText requiredKnowledgePointsText = new MyText(ParentNode.Scene);
            requiredKnowledgePointsText.Text = $"{technology.RequiredXP}";
            requiredKnowledgePointsText.Outlined = true;
            requiredKnowledgePointsText.X = knowledgePointsIcon.LocalX - requiredKnowledgePointsText.TextWidth - 5;
            requiredKnowledgePointsText.Y = 4;

            button.AddChildNode(requiredKnowledgePointsText);

            return button;
        }

        private MNode CreateTechnologyNode(Technology technology)
        {
            MButtonUI button = new MButtonUI(ParentNode.Scene);

            button.Image.Texture = TextureBank.UITexture.GetSubtexture(144, 192, 24, 24);
            button.Image.ImageType = ImageType.Sliced;
            button.Image.SetBorder(8, 8, 8, 8);
            button.Image.BackgroundOverlap = 2;
            button.ButtonScript.ButtonChecked += HoldTechnologyToUnlock;

            MyText technologyNameText = new MyText(ParentNode.Scene);
            technologyNameText.X = 8;
            technologyNameText.Y = 8;
            technologyNameText.Text = technology.Name;
            technologyNameText.Height = 32;
            button.AddChildNode(technologyNameText);

            MyText requiredKnowledgePointsText = new MyText(ParentNode.Scene);
            requiredKnowledgePointsText.Text = $"   {technology.RequiredXP}";
            requiredKnowledgePointsText.Outlined = true;
            requiredKnowledgePointsText.X = technologyNameText.LocalX + technologyNameText.TextWidth + 5;
            requiredKnowledgePointsText.Y = 8;
            button.AddChildNode(requiredKnowledgePointsText);

            MImageUI knowledgePointsIcon = new MImageUI(ParentNode.Scene);
            knowledgePointsIcon.Image.Texture = ItemDatabase.GetItemByName("knowledge_points").Icon;
            knowledgePointsIcon.Width = 32;
            knowledgePointsIcon.Height = 32;
            knowledgePointsIcon.X = requiredKnowledgePointsText.LocalX + requiredKnowledgePointsText.TextWidth + 5;
            knowledgePointsIcon.Tooltips = ItemDatabase.GetItemByName("knowledge_points").Name;
            knowledgePointsIcon.Y = 4;
            button.AddChildNode(knowledgePointsIcon);

            int totalWidth = 0;
            int totalHeight = 0;

            if (technology.Icon != null)
            {
                MImageUI icon = new MImageUI(ParentNode.Scene);
                icon.Width = technology.Icon.Width * 2;
                icon.Height = technology.Icon.Height * 2;
                icon.X = 8;
                icon.Y = 8 + technologyNameText.Height;
                icon.GetComponent<MImageCmp>().Texture = technology.Icon;
                button.AddChildNode(icon);

                totalWidth += icon.Width;
                totalHeight = icon.Height;
            }
            else
            {
                if (technology.UnlockedBuildings != null)
                {
                    foreach (var buildingTemplate in technology.UnlockedBuildings)
                    {
                        MImageUI icon = new MImageUI(ParentNode.Scene);
                        icon.Width = buildingTemplate.Icons[Direction.DOWN].Width * 2;
                        icon.Height = buildingTemplate.Icons[Direction.DOWN].Height * 2;
                        icon.X = 8 + totalWidth;
                        icon.Y = 8 + technologyNameText.Height;
                        icon.Tooltips = buildingTemplate.GetInformation();
                        icon.GetComponent<MImageCmp>().Texture = buildingTemplate.Icons[Direction.DOWN];
                        button.AddChildNode(icon);

                        totalWidth += icon.Width + 5;

                        totalHeight = Math.Max(totalHeight, icon.Height);
                    }
                }

                if (technology.UnlockedItems != null)
                {
                    foreach (var item in technology.UnlockedItems)
                    {
                        MImageUI icon = new MImageUI(ParentNode.Scene);
                        icon.Width = item.Icon.Width * 2;
                        icon.Height = item.Icon.Height * 2;
                        icon.X = 8 + totalWidth;
                        icon.Y = 8 + technologyNameText.Height;
                        icon.Tooltips = item.GetInformation();
                        icon.GetComponent<MImageCmp>().Texture = item.Icon;
                        button.AddChildNode(icon);

                        totalWidth += icon.Width + 5;

                        totalHeight = Math.Max(totalHeight, icon.Height);
                    }
                }

                if(technology.UnlockedAnimals != null)
                {
                    foreach(var animal in technology.UnlockedAnimals)
                    {
                        MImageUI icon = new MImageUI(ParentNode.Scene);
                        icon.Width = animal.Texture.Width * 2;
                        icon.Height = animal.Texture.Height * 2;
                        icon.X = 8 + totalWidth;
                        icon.Y = 8 + technologyNameText.Height;
                        icon.Tooltips = animal.Name;
                        icon.GetComponent<MImageCmp>().Texture = animal.Texture;
                        button.AddChildNode(icon);

                        totalWidth += icon.Width + 5;

                        totalHeight = Math.Max(totalHeight, icon.Height);
                    }
                }
            }

            button.Width = Math.Max(technologyNameText.TextWidth + 5 + requiredKnowledgePointsText.TextWidth + knowledgePointsIcon.Width, totalWidth) + 24;
            button.Height = totalHeight + technologyNameText.Height + 16;
            

            return button;
        }

        private void HoldTechnologyToUnlock(ButtonScript buttonScript)
        {
            Technology technology = nodeTechPair[buttonScript.ParentNode];
            ProgressTree progressTree = GameplayScene.Instance.ProgressTree;

            if (progressTree.CurrentExp < technology.RequiredXP)
                return;

            if (progressTree.TechnologiesStates[technology] != TechnologyState.ReadyToUnlock)
                return;

            unlockingProgress += Engine.DeltaTime * 100;

            GlobalUI.ShowActionProgressSlider(MInput.Mouse.X + 10, MInput.Mouse.Y - 20, Engine.GREEN_COLOR, (int)unlockingTime, (int)unlockingProgress);

            if (unlockingProgress >= unlockingTime)
            {
                unlockingProgress = 0;

                buttonScript.ButtonChecked -= HoldTechnologyToUnlock;

                UnlockTechnology(true, buttonScript);
            }
        }

        private void UnlockTechnology(bool value, ButtonScript buttonScript)
        {
            Technology technology = nodeTechPair[buttonScript.ParentNode];

            ProgressTree progressTree = GameplayScene.Instance.ProgressTree;

            if (progressTree.CurrentExp >= technology.RequiredXP && progressTree.TechnologiesStates[technology] == TechnologyState.ReadyToUnlock)
            {
                if (progressTree.TechnologiesStates[technology] == TechnologyState.Unlocked)
                    return;

                progressTree.UnlockTechnology(technology);

                UpdateProgressPointsText();
                UpdateNodes();
            }
        }

        public void Close(bool value, ButtonScript buttonScript)
        {
            GameplayScene.UIRootNodeScript.CloseMainPanel();
        }
    }
}
