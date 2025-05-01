using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class LaborPriorityUIScript : MScript
    {

        private Dictionary<CreatureCmp, MNode> creatureNodePairs = new Dictionary<CreatureCmp, MNode>();


        private Dictionary<ButtonScript, CreatureCmp> buttons = new Dictionary<ButtonScript, CreatureCmp>();
        private Dictionary<CreatureCmp, Dictionary<LaborType, ButtonScript>> creaturesLaborPriorityButtons = new Dictionary<CreatureCmp, Dictionary<LaborType, ButtonScript>>();

        private MToggleUI settlersToggle;
        private MyText settlersText;
        private MToggleUI animalsToggle;
        private MyText animalsText;
        private CreatureType creatureType = CreatureType.Settler;

        private int offsetBetweenToggles = 16;

        private SmallButton closeButton;

        private Dictionary<int, Color> priorityColorPair;

        private Dictionary<LaborType, int> copiedLaborsPriorities = new Dictionary<LaborType, int>();

        private Dictionary<int, MyTexture> prioritiyIconPair = new Dictionary<int, MyTexture>();

        public LaborPriorityUIScript() : base(true)
        {
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            priorityColorPair = new Dictionary<int, Color>
            {
                { -1, new Color(249, 67, 63) },
                { 0, new Color(244, 134, 37) },
                { 1, new Color(268, 187, 46) },
                { 2, new Color(254, 241, 2) },
                { 3, new Color(114, 190, 68) },
                { 4, new Color(33, 197, 128) }
            };

            prioritiyIconPair = new Dictionary<int, MyTexture>
            {
                { -1, ResourceManager.GetTexture("ui").GetSubtexture(128, 32, 16, 16) },
                { 0, ResourceManager.GetTexture("ui").GetSubtexture(160, 48, 16, 16) },
                { 1, ResourceManager.GetTexture("ui").GetSubtexture(144, 48, 16, 16) },
                { 2, ResourceManager.GetTexture("ui").GetSubtexture(128, 48, 16, 16) },
                { 3, ResourceManager.GetTexture("ui").GetSubtexture(96, 48, 16, 16) },
                { 4, ResourceManager.GetTexture("ui").GetSubtexture(112, 48, 16, 16) }
            };

            // Кнопка закрытия окна
            closeButton = new SmallButton(ParentNode.Scene, ResourceManager.CancelIcon);
            closeButton.X = ParentNode.Width - closeButton.Width;
            closeButton.Y = 0;
            closeButton.GetComponent<ButtonScript>().AddOnClickedCallback(Close);
            ParentNode.AddChildNode(closeButton);

            settlersToggle = new MToggleUI(ParentNode.Scene, true, true);
            settlersToggle.X = 8;
            settlersToggle.Y = 8;
            settlersToggle.Name = "Settlers";
            settlersToggle.GetComponent<ToggleScript>().AddOnValueChangedCallback(SetCreatureType);
            ParentNode.AddChildNode(settlersToggle);

            settlersText = new MyText(ParentNode.Scene);
            settlersText.Text = Localization.GetLocalizedText("settlers");
            settlersText.X = settlersToggle.LocalX + settlersToggle.Width + 5;
            settlersText.Y = settlersToggle.LocalY;
            ParentNode.AddChildNode(settlersText);

            animalsToggle = new MToggleUI(ParentNode.Scene, false, true);
            animalsToggle.X = 8;
            animalsToggle.Y = settlersToggle.LocalY + settlersToggle.Height + 5;
            animalsToggle.Name = "Animals";
            animalsToggle.GetComponent<ToggleScript>().AddOnValueChangedCallback(SetCreatureType);
            ParentNode.AddChildNode(animalsToggle);

            animalsText = new MyText(ParentNode.Scene);
            animalsText.Text = Localization.GetLocalizedText("animals");
            animalsText.X = animalsToggle.LocalX + animalsToggle.Width + 5;
            animalsText.Y = animalsToggle.LocalY;
            ParentNode.AddChildNode(animalsText);

            int startX = 180;
            foreach (LaborType labor in Labor.GetWorkLaborEnumerator())
            {
                MImageUI laborIcon = new MImageUI(ParentNode.Scene);
                laborIcon.GetComponent<MImageCmp>().Texture = Labor.GetLaborIcon(labor);
                laborIcon.Tooltips = Labor.GetLaborString(labor);
                laborIcon.Name = labor.ToString();
                laborIcon.Width = 32;
                laborIcon.Height = 32;
                laborIcon.X = 8 + startX;
                laborIcon.Y = 8 + 32;
                ParentNode.AddChildNode(laborIcon);
                startX += MToggleUI.WIDTH + offsetBetweenToggles;
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
            copiedLaborsPriorities.Clear();

            ListViewUIScript listViewUIScript = ParentNode.GetChildByName("ListView").GetComponent<ListViewUIScript>();
            listViewUIScript.Clear();

            foreach (var creature in GameplayScene.Instance.CreatureLayer.Entities
                .Where(x => x.Get<CreatureCmp>() != null)
                .Select(x => x.Get<CreatureCmp>())
                .Where(x => x.IsDead == false && x.IsDomesticated && x.AgeState == AgeState.Adult && x.CreatureType == creatureType)
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
                    element = CreateNode(creature);

                    creatureNodePairs.Add(creature, element);
                }

                ((CreatureButton)element.GetChildByName("creature_button")).SetCreature(creature);

                listViewUIScript.AddItem(element);
            }
        }

        private void SetCreatureType(bool value, MToggleUI sender)
        {
            switch(sender.Name)
            {
                case "Settlers":
                    settlersToggle.GetComponent<ToggleScript>().SilentCheck(true);
                    animalsToggle.GetComponent<ToggleScript>().SilentCheck(false);
                    creatureType = CreatureType.Settler;
                    Open();
                    break;
                case "Animals":
                    animalsToggle.GetComponent<ToggleScript>().SilentCheck(true);
                    settlersToggle.GetComponent<ToggleScript>().SilentCheck(false);
                    creatureType = CreatureType.Animal;
                    Open();
                    break;
            }
        }

        private void SetLabor(bool value, ButtonScript buttonScript)
        {
            CreatureCmp creature = buttons[buttonScript];
            LaborType laborType = Labor.GetLaborTypeByName(buttonScript.ParentNode.Name);
            int priorityValue = buttonScript.ParentNode.GetMetadata<int>("priority");

            if (buttonScript.LeftButtonDetected)
            {
                priorityValue++;

                if (priorityValue > 4)
                {
                    priorityValue = -1;
                }
            }

            if(buttonScript.RightButtonDetected)
            {
                priorityValue--;

                if (priorityValue < -1)
                {
                    priorityValue = 4;
                }
            }

            buttonScript.ParentNode.SetMetadata("priority", priorityValue);

            Color priorityColor = priorityColorPair[priorityValue];

            buttonScript.SetDefaultColor(priorityColor, priorityColor, priorityColor);

            ((MImageUI)buttonScript.ParentNode.GetChildByName("Icon")).Image.Texture = prioritiyIconPair[priorityValue];

            creature.SetLaborPriority(laborType, priorityValue);
        }

        private MNode CreateNode(CreatureCmp creature)
        {
            creaturesLaborPriorityButtons.Add(creature, new Dictionary<LaborType, ButtonScript>());

            MNode element = new MNode(ParentNode.Scene);

            CreatureButton creatureButton = new CreatureButton(ParentNode.Scene);
            creatureButton.Name = "creature_button";
            creatureButton.ButtonScript.AddOnClickedCallback(OnCreatureButtonPressed);

            int count = 180;
            foreach (LaborType labor in Labor.GetWorkLaborEnumerator())
            {
                if (creature.IsLaborAllowed(labor))
                {
                    SmallButton button = new SmallButton(ParentNode.Scene, null);
                    button.ButtonScript.AllowRightClick = true;
                    button.Name = labor.ToString();
                    button.X = count;
                    button.SetMetadata("priority", creature.GetLaborPriority(labor));
                    button.GetComponent<ButtonScript>().AddOnClickedCallback(SetLabor);
                    Color priorityColor = priorityColorPair[creature.GetLaborPriority(labor)];
                    button.ButtonScript.SetDefaultColor(priorityColor, priorityColor, priorityColor);

                    ((MImageUI)button.GetChildByName("Icon")).Image.Texture = prioritiyIconPair[creature.GetLaborPriority(labor)];

                    element.AddChildNode(button);
                    buttons.Add(button.ButtonScript, creature);

                    creaturesLaborPriorityButtons[creature].Add(labor, button.ButtonScript);
                }

                count += MToggleUI.WIDTH + offsetBetweenToggles;
            }

            SmallButton copyButton = new SmallButton(ParentNode.Scene, null);
            copyButton.Tooltips = Localization.GetLocalizedText("copy");
            copyButton.Image.Texture = ResourceManager.CopyIcon;
            copyButton.X = count;
            copyButton.ButtonScript.AddOnClickedCallback(OnCopySettlerLaborPrioritiesButtonPressed);
            copyButton.SetMetadata("creature", creature);
            element.AddChildNode(copyButton);

            count += MToggleUI.WIDTH + offsetBetweenToggles;

            SmallButton pasteButton = new SmallButton(ParentNode.Scene, null);
            pasteButton.Tooltips = Localization.GetLocalizedText("paste");
            pasteButton.Image.Texture = ResourceManager.PasteIcon;
            pasteButton.X = count;
            pasteButton.ButtonScript.AddOnClickedCallback(OnPasteSettlerLaborPrioritiesButtonPressed);
            pasteButton.SetMetadata("creature", creature);
            element.AddChildNode(pasteButton);


            element.AddChildNode(creatureButton);

            return element;
        }

        private void OnCopySettlerLaborPrioritiesButtonPressed(bool value, ButtonScript buttonScript)
        {
            copiedLaborsPriorities.Clear();

            CreatureCmp creature = buttonScript.ParentNode.GetMetadata<CreatureCmp>("creature");
            foreach (LaborType labor in Labor.GetWorkLaborEnumerator())
            {
                if (creature.IsLaborAllowed(labor))
                {
                    copiedLaborsPriorities.Add(labor, creature.GetLaborPriority(labor));
                }
            }
        }

        private void OnPasteSettlerLaborPrioritiesButtonPressed(bool value, ButtonScript buttonScript)
        {
            CreatureCmp creature = buttonScript.ParentNode.GetMetadata<CreatureCmp>("creature");
            foreach(var kvp in copiedLaborsPriorities)
            {
                LaborType labor = kvp.Key;
                int priorityValue = kvp.Value;

                if(creature.IsLaborAllowed(labor))
                {
                    ButtonScript priorityButtonScript = creaturesLaborPriorityButtons[creature][labor];

                    priorityButtonScript.ParentNode.SetMetadata("priority", priorityValue);

                    Color priorityColor = priorityColorPair[priorityValue];

                    priorityButtonScript.SetDefaultColor(priorityColor, priorityColor, priorityColor);

                    ((MImageUI)priorityButtonScript.ParentNode.GetChildByName("Icon")).Image.Texture = prioritiyIconPair[priorityValue];

                    creature.SetLaborPriority(labor, priorityValue);
                }
            }
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
