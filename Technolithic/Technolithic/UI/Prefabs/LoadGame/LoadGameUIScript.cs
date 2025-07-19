using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class LoadGameUIScript : MScript
    {
        private SmallButton closeButton;

        private Dictionary<string, Dictionary<string, MNode>> saveFolderSaveFilesNodes;
        private Dictionary<string, MNode> saveFoldersNodes;

        private MNode backButton;

        private string selectedSaveFolder;

        private float removingTime = 100;
        private float removingProgress = 0;

        public LoadGameUIScript() : base(true)
        {
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            saveFoldersNodes = new Dictionary<string, MNode>();
            saveFolderSaveFilesNodes = new Dictionary<string, Dictionary<string, MNode>>();

            // Кнопка закрытия окна
            closeButton = new SmallButton(ParentNode.Scene, ResourceManager.CancelIcon);
            closeButton.X = ParentNode.Width - closeButton.Width;
            closeButton.Y = 0;
            closeButton.GetComponent<ButtonScript>().AddOnClickedCallback(Close);

            ParentNode.AddChildNode(closeButton);

            backButton = CreateButton(Localization.GetLocalizedText("back"));
            backButton.X = ParentNode.Width / 2 - backButton.Width / 2;
            backButton.Y = ParentNode.Height - backButton.Height - 5;

            ParentNode.AddChildNode(backButton);

            InitializeWorldFolders();
        }

        private void InitializeWorldFolders()
        {
            string savesDirectory = Path.Combine(Engine.GetGameDirectory(), "Saves");
            if (Directory.Exists(savesDirectory))
            {
                List<string> saveFolders = Directory.GetDirectories(savesDirectory).ToList();

                saveFolders = saveFolders.OrderBy(x => Directory.GetLastWriteTime(x)).Reverse().ToList();

                foreach (var saveFolder in saveFolders)
                {
                    string saveFolderName = saveFolder.Split(Path.DirectorySeparatorChar).Last();
                    DateTime saveFolderDateTime = Directory.GetLastWriteTime(saveFolder);

                    if (saveFoldersNodes.ContainsKey(saveFolderName) == false)
                    {
                        MNode element = CreateElement(saveFolderName, true, Color.White, saveFolderDateTime);
                        saveFoldersNodes.Add(saveFolderName, element);
                        saveFolderSaveFilesNodes.Add(saveFolderName, new Dictionary<string, MNode>());
                    }
                }
            }
        }

        public void Open()
        {
            backButton.Active = false;

            InitializeWorldFolders();

            MNode listView = ParentNode.GetChildByName("ListView");
            ListViewUIScript listViewScript = listView.GetComponent<ListViewUIScript>();
            listViewScript.Clear();

            foreach(var kvp in saveFoldersNodes)
            {
                listViewScript.AddItem(kvp.Value);
            }
        }

        private MNode CreateButton(string text)
        {
            MButtonUI button = new MButtonUI(ParentNode.Scene);

            button.Image.Texture = TextureBank.UITexture.GetSubtexture(144, 192, 24, 24);
            button.Image.ImageType = ImageType.Sliced;
            button.Image.SetBorder(8, 8, 8, 8);
            button.Image.BackgroundOverlap = 2;
            button.ButtonScript.AddOnClickedCallback(GoBack);

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

        public override void Update(int mouseX, int mouseY)
        {
            if(MInput.Mouse.ReleasedLeftButton)
            {
                removingProgress = 0;
            }
        }

        private MNode CreateElement(string save, bool isFolder, Color color, DateTime dateTime)
        {
            MNode saveElement = new MNode(ParentNode.Scene);

            MButtonUI element = new MButtonUI(ParentNode.Scene);

            element.Image.Texture = TextureBank.UITexture.GetSubtexture(192, 192, 24, 24);
            element.Image.ImageType = ImageType.Sliced;
            element.Image.SetBorder(8, 8, 8, 8);
            element.Image.BackgroundOverlap = 2;
            element.SetMetadata("save", save);

            if (isFolder)
            {
                element.ButtonScript.AddOnClickedCallback(OpenSaves);
            }
            else
            {
                element.ButtonScript.AddOnClickedCallback(LoadSave);
            }

            element.ButtonScript.SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);
            element.Width = ParentNode.Width - 64;
            element.Height = 34;

            MImageUI itemIcon = new MImageUI(ParentNode.Scene);
            itemIcon.GetComponent<MImageCmp>().Texture = isFolder ? ResourceManager.FolderIcon : ResourceManager.FileIcon;
            itemIcon.Width = 32;
            itemIcon.Height = 32;
            itemIcon.X = 8;
            itemIcon.Y = 0;

            element.AddChildNode(itemIcon);

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = save;
            itemName.Name = "Name";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.Color = color;
            itemName.X = itemIcon.LocalX + itemIcon.Width + 5;
            itemName.Y = 2;

            element.AddChildNode(itemName);

            MyText itemDate = new MyText(ParentNode.Scene);
            itemDate.Text = dateTime.ToString();
            itemDate.Name = "Date";
            itemDate.Width = itemDate.TextWidth;
            itemDate.Height = itemDate.TextHeight;
            itemDate.Color = Color.White * 0.75f;
            itemDate.X = (element.LocalX + element.Width) - (itemDate.Width + 5);
            itemDate.Y = 2;

            element.AddChildNode(itemDate);

            SmallButton removeButton = new SmallButton(ParentNode.Scene, ResourceManager.CancelIcon);
            removeButton.GetComponent<ButtonScript>().SetDefaultColor(Engine.RED_COLOR, Engine.RED_COLOR, Engine.RED_COLOR);
            removeButton.SetMetadata("save", save);
            removeButton.Name = "RemoveButton";
            removeButton.X = element.LocalX + element.Width;
            removeButton.ButtonScript.ButtonChecked += HoldSaveToRemove;

            saveElement.AddChildNode(element);
            saveElement.AddChildNode(removeButton);

            return saveElement;
        }

        private void GoBack(bool value, ButtonScript buttonScript)
        {
            backButton.Active = false;

            selectedSaveFolder = "";

            Open();
        }

        public void OpenSaves(bool value, ButtonScript buttonScript)
        {
            selectedSaveFolder = buttonScript.ParentNode.GetMetadata<string>("save");

            OpenSaves(selectedSaveFolder);
        }

        private void OpenSaves(string worldName)
        {
            backButton.Active = true;

            MNode listView = ParentNode.GetChildByName("ListView");
            ListViewUIScript listViewScript = listView.GetComponent<ListViewUIScript>();
            listViewScript.Clear();

            string saveFilesPath = Path.Combine(Engine.GetGameDirectory(), "Saves", worldName);
            if (Directory.Exists(saveFilesPath))
            {
                List<string> saveFiles = Directory.GetFiles(saveFilesPath).ToList();

                saveFiles = saveFiles.OrderBy(x => File.GetLastWriteTime(x)).Reverse().ToList();

                foreach (var saveFile in saveFiles)
                {
                    if (saveFile.EndsWith("_info"))
                        continue;

                    MNode element;

                    string saveFileName = saveFile.Split(Path.DirectorySeparatorChar).Last();
                    DateTime saveFileDateTime = File.GetLastWriteTime(saveFile);

                    if (saveFolderSaveFilesNodes[worldName].ContainsKey(saveFileName))
                    {
                        element = saveFolderSaveFilesNodes[worldName][saveFileName];
                    }
                    else
                    {
                        string saveDate = GetSaveInfo(selectedSaveFolder, saveFileName);

                        Color saveColor = Color.White;

                        if(string.IsNullOrEmpty(saveDate))
                        {
                            saveDate = "BROKEN";
                            saveColor = Engine.RED_COLOR;
                        }

                        element = CreateElement(saveFileName, false, saveColor, saveFileDateTime);
                        saveFolderSaveFilesNodes[worldName].Add(saveFileName, element);
                    }

                    listViewScript.AddItem(element);
                }
            }
        }

        private string GetSaveInfo(string saveFolder, string saveName)
        {
            string infoFilePath = Path.Combine(Engine.GetGameDirectory(), "Saves", saveFolder, saveName + "_info");
            if (!File.Exists(infoFilePath))
                return "";

            string infoDataText = File.ReadAllText(infoFilePath);
            SaveInfo saveInfo = JsonConvert.DeserializeObject<SaveInfo>(infoDataText);

            if(saveInfo == null)
            {
                return "";
            }
            else
            {
                return saveInfo.DateTime;
            }
        }

        public void LoadSave(bool value, ButtonScript sender)
        {
            Engine.Scene = new GameplayScene(sender.ParentNode.GetMetadata<string>("save"), selectedSaveFolder);
            Close(true, null);
        }

        private void RemoveWorldSaves(string worldName)
        {
            string worldSavesPath = Path.Combine(Engine.GetGameDirectory(), "Saves", worldName);
            if(Directory.Exists(worldSavesPath))
            {
                EmptyFolder(new DirectoryInfo(worldSavesPath));

                Directory.Delete(worldSavesPath);

                saveFoldersNodes.Remove(worldName);

                Open();
            }
        }

        private void HoldSaveToRemove(ButtonScript buttonScript)
        {
            removingProgress += Engine.DeltaTime * 100;

            GlobalUI.ShowActionProgressSlider(MInput.Mouse.X + 10, MInput.Mouse.Y - 20, Color.Red, (int)removingTime, (int)removingProgress);

            if (removingProgress >= removingTime)
            {
                removingProgress = 0;

                buttonScript.ButtonChecked -= HoldSaveToRemove;

                if (string.IsNullOrEmpty(selectedSaveFolder))
                {
                    RemoveWorldSaves(buttonScript.ParentNode.GetMetadata<string>("save"));
                }
                else
                {
                    string worldSavesDirectory = Path.Combine(Engine.GetGameDirectory(), "Saves", selectedSaveFolder);
                    string saveFilePath = Path.Combine(worldSavesDirectory, buttonScript.ParentNode.GetMetadata<string>("save"));
                    string saveFileInfoPath = Path.Combine(worldSavesDirectory, buttonScript.ParentNode.GetMetadata<string>("save") + "_info");

                    if (File.Exists(saveFilePath))
                    {
                        File.Delete(saveFilePath);
                        if (File.Exists(saveFileInfoPath))
                        {
                            File.Delete(saveFileInfoPath);
                        }

                        OpenSaves(selectedSaveFolder);
                    }
                }
            }
        }

        private void EmptyFolder(DirectoryInfo directoryInfo)
        {
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subfolder in directoryInfo.GetDirectories())
            {
                EmptyFolder(subfolder);
            }
        }

        public void Close(bool value, ButtonScript buttonScript)
        {
            selectedSaveFolder = "";

            ParentNode.Active = false;

            foreach(var kvp1 in saveFolderSaveFilesNodes)
            {
                foreach(var kvp2 in saveFolderSaveFilesNodes[kvp1.Key])
                {
                    MNode removeButton = kvp2.Value.GetChildByName("RemoveButton");
                    removeButton.GetComponent<ButtonScript>().SetDefaultColor(Color.White, Color.White, Color.White);
                }
            }
        }
    }
}
