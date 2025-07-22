using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class WorldSettingsUIScript : MScript
    {

        private SmallButton closeButton;

        private Dictionary<MNode, int> nodeWorldSizePair = new Dictionary<MNode, int>();

        private MNode createButton;

        private int selectedWorldSize = 0;

        private MTextInput worldSeedInput;

        private MTextInput worldNameInput;

        private MyText attentionText;

        private bool IsDuplicateWorldName;

        public WorldSettingsUIScript() : base(true)
        {
            
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            closeButton = new SmallButton(ParentNode.Scene, ResourceManager.CancelIcon);
            closeButton.X = ParentNode.Width - closeButton.Width;
            closeButton.Y = 0;
            closeButton.GetComponent<ButtonScript>().AddOnClickedCallback(Close);

            ParentNode.AddChildNode(closeButton);

            MNode listView = ParentNode.GetChildByName("ListView");
            ListViewUIScript listViewScript = listView.GetComponent<ListViewUIScript>();

            MyText worldSeedText = new MyText(ParentNode.Scene);
            worldSeedText.Text = Localization.GetLocalizedText("world_seed");
            worldSeedText.Color = Color.LightGreen;
            listViewScript.AddItem(worldSeedText);

            Random random = new Random();
            worldSeedInput = new MTextInput(ParentNode.Scene);
            worldSeedInput.Width = 300 - 16;
            worldSeedInput.Height = 48;
            worldSeedInput.GetComponent<MTextInputScript>().CurrentText = $"{random.Next(1, int.MaxValue)}";
            worldSeedInput.GetComponent<MTextInputScript>()
                .AddValidationRule(new MaxLengthRule(int.MaxValue.ToString().Length))
                .AddValidationRule(new NumericRule(int.MaxValue, false));
            worldSeedInput.GetComponent<MTextInputScript>().OnCurrentTextChanges += OnWorldSeedChangedCallback;
            listViewScript.AddItem(worldSeedInput);

            MyText worldNameText = new MyText(ParentNode.Scene);
            worldNameText.Text = Localization.GetLocalizedText("world_name");
            worldNameText.Color = Color.LightGreen;
            listViewScript.AddItem(worldNameText);

            worldNameInput = new MTextInput(ParentNode.Scene);
            worldNameInput.Width = 300 - 16;
            worldNameInput.Height = 48;
            worldNameInput.GetComponent<MTextInputScript>()
                .AddValidationRule(new MaxLengthRule(200))
                .AddValidationRule(new AlphaNumericRule(true, true, false));
            worldNameInput.GetComponent<MTextInputScript>().OnCurrentTextChanges += OnWorldNameChangedCallback;
            listViewScript.AddItem(worldNameInput);

            MyText worldSizeText = new MyText(ParentNode.Scene);
            worldSizeText.Text = Localization.GetLocalizedText("world_size");
            worldSizeText.Color = Color.LightGreen;
            listViewScript.AddItem(worldSizeText);

            MNode node128 = CreateElement(128);
            nodeWorldSizePair.Add(node128, 128);
            listViewScript.AddItem(node128);

            MNode node256 = CreateElement(256);
            nodeWorldSizePair.Add(node256, 256);
            listViewScript.AddItem(node256);

            createButton = CreateButton(Localization.GetLocalizedText("create"));
            createButton.X = ParentNode.Width / 2 - createButton.Width / 2;
            createButton.Y = ParentNode.Height - createButton.Height - 5;
            createButton.GetComponent<ButtonScript>().IsDisabled = true;
            ParentNode.AddChildNode(createButton);

            attentionText = new MyText(ParentNode.Scene);
            attentionText.Y = createButton.LocalY + createButton.Height + 5;
            attentionText.X = 8;
            attentionText.Color = Color.Yellow;
            attentionText.Active = false;
            attentionText.Text = Localization.GetLocalizedText("world_with_that_name_already_exists") + "!";
            ParentNode.AddChildNode(attentionText);
        }

        private void OnWorldNameChangedCallback(string text)
        {
            string savesDirectory = Engine.GetGameDirectory() + Path.DirectorySeparatorChar + "Saves";
            if (Directory.Exists(savesDirectory))
            {
                string[] saveFolders = Directory.GetDirectories(savesDirectory);

                IsDuplicateWorldName = saveFolders.Where(x => x.Split(Path.DirectorySeparatorChar).Last() == text)
                    .Select(x => x)
                    .Count() > 0;

                if (IsDuplicateWorldName)
                {
                    attentionText.Active = true;
                }
                else
                {
                    attentionText.Active = false;
                }
            }

            RecheckCreateButton();
        }

        private void OnWorldSeedChangedCallback(string text)
        {
            RecheckCreateButton();
        }

        private void RecheckCreateButton()
        {
            bool isDisabled = false;

            if (selectedWorldSize == 0)
            {
                isDisabled = true;
            }

            if (string.IsNullOrEmpty(worldNameInput.GetComponent<MTextInputScript>().CurrentText))
            {
                isDisabled = true;
            }

            if (string.IsNullOrWhiteSpace(worldNameInput.GetComponent<MTextInputScript>().CurrentText))
            {
                isDisabled = true;
            }

            if (string.IsNullOrEmpty(worldSeedInput.GetComponent<MTextInputScript>().CurrentText))
            {
                isDisabled = true;
            }

            if (string.IsNullOrWhiteSpace(worldSeedInput.GetComponent<MTextInputScript>().CurrentText))
            {
                isDisabled = true;
            }

            if (IsDuplicateWorldName)
            {
                isDisabled = true;
            }

            createButton.GetComponent<ButtonScript>().IsDisabled = isDisabled;
        }

        public override void Update(int mouseX, int mouseY)
        {
        }

        private MNode CreateElement(int worldSize)
        {
            MButtonUI element = new MButtonUI(ParentNode.Scene);

            element.Image.Texture = TextureBank.UITexture.GetSubtexture(192, 192, 24, 24);
            element.Image.ImageType = ImageType.Sliced;
            element.Image.SetBorder(8, 8, 8, 8);
            element.Image.BackgroundOverlap = 2;

            element.ButtonScript.AddOnClickedCallback(SelectWorldSize);
            element.ButtonScript.SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);
            element.Width = ParentNode.Width - (16 + 16);
            element.Height = 34;

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = worldSize + "x" + worldSize;
            itemName.Name = "Name";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = 8;
            itemName.Y = 2;

            element.AddChildNode(itemName);

            return element;
        }

        public void SelectWorldSize(bool value, ButtonScript sender)
        {
            foreach (var kvp in nodeWorldSizePair)
            {
                kvp.Key.GetComponent<ButtonScript>().SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);
            }

            selectedWorldSize = nodeWorldSizePair[sender.ParentNode];

            sender.ParentNode.GetComponent<ButtonScript>().SetDefaultColor(Color.Orange, Color.Orange, Color.Orange);

            RecheckCreateButton();
        }

        public void Close(bool value, ButtonScript buttonScript)
        {
            worldNameInput.GetComponent<MTextInputScript>().ResetText();

            ParentNode.Active = false;
        }

        private MNode CreateButton(string text)
        {
            MButtonUI button = new MButtonUI(ParentNode.Scene);

            button.Image.Texture = TextureBank.UITexture.GetSubtexture(144, 192, 24, 24);
            button.Image.ImageType = ImageType.Sliced;
            button.Image.SetBorder(8, 8, 8, 8);
            button.Image.BackgroundOverlap = 2;
            button.ButtonScript.AddOnClickedCallback(CreateWorld);

            MyText title = new MyText(ParentNode.Scene);
            title.X = 8;
            title.Y = 8;
            title.Text = text;
            title.Width = title.TextWidth;
            button.AddChildNode(title);

            button.Width = title.Width + 16;
            button.Height = 48;

            return button;
        }

        private void CreateWorld(bool value, ButtonScript buttonScript)
        {
            ParentNode.Active = false;

            string seedString = worldSeedInput.GetComponent<MTextInputScript>().CurrentText;

            WorldSettings worldSettings = new WorldSettings()
            {
                Size = selectedWorldSize,
                Name = worldNameInput.GetComponent<MTextInputScript>().CurrentText,
                Seed = int.Parse(seedString),
                StartSeason = Season.Summer
            };

            Engine.Scene = new GameplayScene(worldSettings);

            worldNameInput.GetComponent<MTextInputScript>().ResetText();
            worldSeedInput.GetComponent<MTextInputScript>().ResetText();
        }
    }
}