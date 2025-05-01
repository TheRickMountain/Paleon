using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class GameMenuScript : MScript
    {

        private Dictionary<Tab, MNode> tabNodes = new Dictionary<Tab, MNode>();

        private SmallButton closeButton;

        private ListViewUIScript saveListView;

        private MNode newSaveNode;
        private MTextInput saveNameInput;

        private MNode saveButton;
        private MNode cancelButton;

        private MSliderBar soundSlider;
        private MSliderBar musicSlider;

        private MToggleUI edgeScrollingCameraCheckbox;
        private MyText edgeScrollingCameraText;

        public GameMenuScript() : base(false)
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

            ParentNode.GetChildByName("ExitTab").GetComponent<ButtonScript>().AddOnClickedCallback(SetTab);
            ParentNode.GetChildByName("SaveTab").GetComponent<ButtonScript>().AddOnClickedCallback(SetTab);
            ParentNode.GetChildByName("AudioTab").GetComponent<ButtonScript>().AddOnClickedCallback(SetTab);
            ParentNode.GetChildByName("SettingsTab").GetComponent<ButtonScript>().AddOnClickedCallback(SetTab);

            ListViewUIScript exitListView = ParentNode.GetChildByName("ExitListView").GetComponent<ListViewUIScript>();
            ListViewUIScript settingsListView = ParentNode.GetChildByName("SettingsListView").GetComponent<ListViewUIScript>();
            saveListView = ParentNode.GetChildByName("SaveListView").GetComponent<ListViewUIScript>();
            ListViewUIScript audioListView = ParentNode.GetChildByName("AudioListView").GetComponent<ListViewUIScript>();

            tabNodes.Add((Tab)ParentNode.GetChildByName("ExitTab"), ParentNode.GetChildByName("ExitListView"));
            tabNodes.Add((Tab)ParentNode.GetChildByName("SaveTab"), ParentNode.GetChildByName("SaveListView"));
            tabNodes.Add((Tab)ParentNode.GetChildByName("AudioTab"), ParentNode.GetChildByName("AudioListView"));
            tabNodes.Add((Tab)ParentNode.GetChildByName("SettingsTab"), ParentNode.GetChildByName("SettingsListView"));

            SetTab(true, ParentNode.GetChildByName("ExitTab").GetComponent<ButtonScript>());

            exitListView.AddItem(CreateButton(Localization.GetLocalizedText("exit_to_main_menu"), ExitToMainMenu, 300 - 16));
            exitListView.AddItem(CreateButton(Localization.GetLocalizedText("exit_to_desktop"), ExitToDesktop, 300 - 16));

            saveButton = CreateButton(Localization.GetLocalizedText("save"), AddSave, 300 - 16);
            cancelButton = CreateButton(Localization.GetLocalizedText("cancel"), CancelSave, 300 - 16);

            newSaveNode = CreateElement("< new save >", OpenNewSaveUI, false);
            saveNameInput = new MTextInput(ParentNode.Scene);
            saveNameInput.Width = 300 - 16;
            saveNameInput.Height = 48;

            // Sound

            MyText soundText = new MyText(ParentNode.Scene);
            soundText.Text = Localization.GetLocalizedText("sound_volume");
            soundText.X = audioListView.ParentNode.Width / 2 - soundText.TextWidth / 2;
            
            soundSlider = new MSliderBar(ParentNode.Scene, 200, 32);
            soundSlider.X = audioListView.ParentNode.Width / 2 - soundSlider.Width / 2;
            soundSlider.MaxValue = 100;
            soundSlider.SetCurrentValueSilent(GameSettings.SoundVolume);
            soundSlider.CurrentValueChanged += OnSoundSliderValueChanged;
            
            audioListView.AddItem(soundText);
            audioListView.AddItem(soundSlider);

            // Music

            MyText musicText = new MyText(ParentNode.Scene);
            musicText.Text = Localization.GetLocalizedText("music_volume");
            musicText.X = audioListView.ParentNode.Width / 2 - musicText.TextWidth / 2;

            musicSlider = new MSliderBar(ParentNode.Scene, 200, 32);
            musicSlider.X = audioListView.ParentNode.Width / 2 - musicSlider.Width / 2;
            musicSlider.MaxValue = 100;
            musicSlider.SetCurrentValueSilent(GameSettings.MusicVolume);
            musicSlider.CurrentValueChanged += OnMusicSliderValueChanged;

            audioListView.AddItem(musicText);
            audioListView.AddItem(musicSlider);

            edgeScrollingCameraText = new MyText(ParentNode.Scene);
            edgeScrollingCameraText.Text = Localization.GetLocalizedText("edge_scrolling_camera");
            edgeScrollingCameraText.X = 8;

            edgeScrollingCameraCheckbox = new MToggleUI(ParentNode.Scene, GameSettings.EdgeScrollingCamera, false);
            edgeScrollingCameraCheckbox.GetComponent<ToggleScript>().AddOnValueChangedCallback(OnEdgeSlideringCameraCheckboxChanged);
            edgeScrollingCameraCheckbox.X = edgeScrollingCameraText.LocalX + edgeScrollingCameraText.TextWidth + 5;

            MNode edgeScrollingCameraNode = new MNode(ParentNode.Scene);
            edgeScrollingCameraNode.AddChildNode(edgeScrollingCameraText);
            edgeScrollingCameraNode.AddChildNode(edgeScrollingCameraCheckbox);


            settingsListView.AddItem(edgeScrollingCameraNode);
        }

        private void OnSoundSliderValueChanged(int value, MSliderBar sliderBar)
        {
            GameSettings.SoundVolume = value;
            SoundEffect.MasterVolume = (float)(value / 100f);
        }

        private void OnMusicSliderValueChanged(int value, MSliderBar sliderBar)
        {
            GameSettings.MusicVolume = value;
            MediaPlayer.Volume = (float)(value / 100f);
        }

        private void OnEdgeSlideringCameraCheckboxChanged(bool value, MToggleUI toggle)
        {
            GameSettings.EdgeScrollingCamera = value;
        }

        private void SetTab(bool value, ButtonScript buttonScript)
        {
            foreach (var kvp in tabNodes)
            {
                Tab tab = kvp.Key;
                MNode node = kvp.Value;

                if (tab != buttonScript.ParentNode)
                {
                    tab.Y = -tab.Height;
                    node.Active = false;
                }
                else
                {
                    tab.Y = -tab.Height + 6;
                    node.Active = true;
                }
            }
        }

        public void Open()
        {
            saveListView.Clear();
            saveListView.AddItem(newSaveNode);
        }

        public override void Update(int mouseX, int mouseY)
        {
            
        }

        private void AddSave(bool value, ButtonScript buttonScript)
        {
            string saveName = saveNameInput.GetComponent<MTextInputScript>().CurrentText;

            if (string.IsNullOrEmpty(saveName) || string.IsNullOrWhiteSpace(saveName))
                return;

            saveName = saveName.TrimEnd();
            saveName = saveName.TrimStart();
            GameplayScene.Instance.SaveGame(saveName, false);
            saveNameInput.GetComponent<MTextInputScript>().ResetText();
            saveListView.Clear();
            saveListView.AddItem(newSaveNode);
        }

        private void CancelSave(bool value, ButtonScript buttonScript)
        {
            saveNameInput.GetComponent<MTextInputScript>().ResetText();
            saveListView.Clear();
            saveListView.AddItem(newSaveNode);
        }

        public void Close(bool value, ButtonScript buttonScript)
        {
            GameplayScene.UIRootNodeScript.CloseMainPanel();
        }

        private void ExitToMainMenu(bool value, ButtonScript buttonScript)
        {
            Engine.Scene = Engine.Instance.MainMenuScene;
        }

        private void ExitToDesktop(bool value, ButtonScript buttonScript)
        {
            Engine.Instance.Exit();
        }

        private void OpenNewSaveUI(bool value, ButtonScript buttonScript)
        {
            saveListView.Clear();
            saveListView.AddItem(saveNameInput);
            saveListView.AddItem(saveButton);
            saveListView.AddItem(cancelButton);
        }

        private MNode CreateButton(string name, Action<bool, ButtonScript> action, int width)
        {
            MButtonUI button = new MButtonUI(ParentNode.Scene);

            button.Image.Texture = TextureBank.UITexture.GetSubtexture(144, 192, 24, 24);
            button.Image.ImageType = ImageType.Sliced;
            button.Image.SetBorder(8, 8, 8, 8);
            button.Image.BackgroundOverlap = 2;
            button.ButtonScript.AddOnClickedCallback(action);

            MyText title = new MyText(ParentNode.Scene);
            title.X = 8;
            title.Y = 8;
            title.Text = name;
            title.Width = title.TextWidth;
            button.AddChildNode(title);

            button.Width = width;
            button.Height = 48;

            return button;
        }

        private MNode CreateElement(string save, Action<bool, ButtonScript> action, bool hasRemoveButton)
        {
            MNode saveElement = new MNode(ParentNode.Scene);

            MButtonUI element = new MButtonUI(ParentNode.Scene);

            element.Image.Texture = TextureBank.UITexture.GetSubtexture(192, 192, 24, 24);
            element.Image.ImageType = ImageType.Sliced;
            element.Image.SetBorder(8, 8, 8, 8);
            element.Image.BackgroundOverlap = 2;
            element.SetMetadata("save", save);
            element.GetComponent<ButtonScript>().AddOnClickedCallback(action);

            element.ButtonScript.SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);
            element.Width = ParentNode.Width - 64;
            element.Height = 34;

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = save;
            itemName.Name = "Name";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = 8;
            itemName.Y = 2;

            element.AddChildNode(itemName);

            if (hasRemoveButton)
            {
                SmallButton removeButton = new SmallButton(ParentNode.Scene, ResourceManager.CancelIcon);
                removeButton.SetMetadata("save", save);
                removeButton.Name = "RemoveButton";
                removeButton.X = element.LocalX + element.Width;
                removeButton.GetComponent<ButtonScript>().AddOnClickedCallback(RemoveSave);
                saveElement.AddChildNode(removeButton);
            }

            saveElement.AddChildNode(element);

            return saveElement;
        }

        public void RemoveSave(bool value, ButtonScript sender)
        {
            if (sender.NormalColor == Engine.RED_COLOR)
            {
                string savesDirectory = Engine.GetGameDirectory() + Path.DirectorySeparatorChar + "Saves";
                if (File.Exists(Path.Combine(savesDirectory, sender.ParentNode.GetMetadata<string>("save"))))
                {
                    File.Delete(Path.Combine(savesDirectory, sender.ParentNode.GetMetadata<string>("save")));

                    Open();
                }
            }
            else
            {
                sender.SetDefaultColor(Engine.RED_COLOR, Engine.RED_COLOR, Engine.RED_COLOR);
            }
        }

    }
}
