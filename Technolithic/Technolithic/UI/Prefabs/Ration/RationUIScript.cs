using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class RationUIScript : MScript
    {

        private Dictionary<SettlerCmp, MNode> settlerNodePairs = new Dictionary<SettlerCmp, MNode>();


        private Dictionary<MToggleUI, SettlerCmp> toggles = new Dictionary<MToggleUI, SettlerCmp>();

        private int offsetBetweenToggles = 16;

        private SmallButton closeButton;


        public RationUIScript() : base(true)
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

            MyText newSettlersTest = new MyText(ParentNode.Scene);
            newSettlersTest.Text = Localization.GetLocalizedText("new_settlers");
            newSettlersTest.Width = 100;
            newSettlersTest.Height = 32;
            newSettlersTest.X = 8;
            newSettlersTest.Y = 48;
            ParentNode.AddChildNode(newSettlersTest);

            int startX = 180;
            foreach (Item item in Engine.Instance.SettlerRation)
            {
                SmallButton foodButton = new SmallButton(ParentNode.Scene, null);
                foodButton.Image.Texture = item.Icon;
                foodButton.SetMetadata("item", item);
                foodButton.ButtonScript.AllowRightClick = true;
                foodButton.Tooltips = item.GetInformation();
                foodButton.Tooltips += $"/c[#808080]\n[{Localization.GetLocalizedText("left_click")}] " +
                    $"{Localization.GetLocalizedText("enable_all")}\n[{Localization.GetLocalizedText("right_click")}] " +
                    $"{Localization.GetLocalizedText("disable_all")}/cd";
                foodButton.X = 8 + startX;
                foodButton.Y = 8;
                foodButton.ButtonScript.AddOnClickedCallback(OnFoodButtonPressed);
                ParentNode.AddChildNode(foodButton);

                MToggleUI toggle = new MToggleUI(ParentNode.Scene, GameplayScene.WorldManager.NewSettlerFoodRationFilters[item]);
                toggle.SetMetadata("item", item);
                toggle.X = 8 + startX;
                toggle.Y = foodButton.LocalY + foodButton.Height + 5;
                toggle.GetComponent<ToggleScript>().AddOnValueChangedCallback(SetRationForNewSettlers);
                ParentNode.AddChildNode(toggle);

                startX += MToggleUI.WIDTH + offsetBetweenToggles;
            }
        }

        private void OnFoodButtonPressed(bool value, ButtonScript buttonScript)
        {
            Item foodItem = buttonScript.ParentNode.GetMetadata<Item>("item");

            // Update filters
            foreach(var kvp in settlerNodePairs)
            {
                SettlerCmp settler = kvp.Key;

                if(buttonScript.LeftButtonDetected)
                {
                    settler.FoodRation.SetFilter(foodItem, true);
                }
                else if(buttonScript.RightButtonDetected)
                {
                    settler.FoodRation.SetFilter(foodItem, false);
                }
            }

            // Update toggles
            foreach (var kvp in toggles)
            {
                MToggleUI toggle = kvp.Key;
                SettlerCmp settler = kvp.Value;

                if(toggle.GetMetadata<Item>("item") == foodItem)
                {
                    bool isFoodAllowed = settler.FoodRation.IsFoodAllowed(foodItem);
                    toggle.GetComponent<ToggleScript>().SilentCheck(isFoodAllowed);
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

        public void Open()
        {
            ListViewUIScript listViewUIScript = ParentNode.GetChildByName("ListView").GetComponent<ListViewUIScript>();
            listViewUIScript.Clear();

            foreach (var settler in GameplayScene.Instance.CreatureLayer.Entities
                .Where(x => x.Get<SettlerCmp>() != null)
                .Select(x => x.Get<SettlerCmp>())
                .Where(x => x.IsDead == false)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id))
            {
                MNode element;

                if (settlerNodePairs.ContainsKey(settler))
                {
                    element = settlerNodePairs[settler];   
                }
                else
                {
                    element = CreateNode(settler);

                    settlerNodePairs.Add(settler, element);
                }

                ((CreatureButton)element.GetChildByName("creature_button")).SetCreature(settler);

                listViewUIScript.AddItem(element);
            }
        }

        private void SetRation(bool value, MToggleUI sender)
        {
            SettlerCmp settler = toggles[sender];
            Item item = sender.GetMetadata<Item>("item");
            bool toggleValue = sender.GetComponent<ToggleScript>().IsOn;
            settler.FoodRation.SetFilter(item, toggleValue);
        }

        private void SetRationForNewSettlers(bool value, MToggleUI sender)
        {
            Item item = sender.GetMetadata<Item>("item");
            bool toggleValue = sender.GetComponent<ToggleScript>().IsOn;
            GameplayScene.WorldManager.NewSettlerFoodRationFilters[item] = toggleValue;
        }

        private MNode CreateNode(SettlerCmp settler)
        {
            MNode element = new MNode(ParentNode.Scene);

            CreatureButton creatureButton = new CreatureButton(ParentNode.Scene);
            creatureButton.Name = "creature_button";
            creatureButton.ButtonScript.AddOnClickedCallback(OnCreatureButtonPressed);

            int count = 180;
            foreach (var kvp in settler.FoodRation.GetFilters())
            {
                Item item = kvp.Key;
                bool flag = kvp.Value;

                MToggleUI toggle = new MToggleUI(ParentNode.Scene, flag);
                toggle.SetMetadata("item", item);
                toggle.X = count;
                toggle.GetComponent<ToggleScript>().AddOnValueChangedCallback(SetRation);
                element.AddChildNode(toggle);
                count += toggle.Width + offsetBetweenToggles;
                toggles.Add(toggle, settler);
            }

            element.AddChildNode(creatureButton);

            return element;
        }

        private void OnCreatureButtonPressed(bool value, ButtonScript buttonScript)
        {
            GameplayScene.UIRootNodeScript.CloseMainPanel();

            CreatureButton creatureButton = (CreatureButton)buttonScript.ParentNode;

            CreatureCmp creature = creatureButton.Creature;

            CameraMovementScript cameraMovementScript = GameplayScene.Instance.GameplayCamera.Get<CameraMovementScript>();
            cameraMovementScript.SetEntityToFollow(creature.Entity);

            GameplayScene.UIRootNodeScript.OpenCreatureUI(creature);
        }


        public void Close(bool value, ButtonScript buttonScript)
        {
            GameplayScene.UIRootNodeScript.CloseMainPanel();
        }

    }
}
