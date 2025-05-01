using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class LocalizationUIScript : MScript
    {
        private Dictionary<string, MNode> languagesNodes = new Dictionary<string, MNode>();
        private Dictionary<MNode, string> nodesLanguages = new Dictionary<MNode, string>();

        private SmallButton closeButton;

        private MyText attentionText;

        public LocalizationUIScript() : base(true)
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

            MNode listView = ParentNode.GetChildByName("ListView");
            ListViewUIScript listViewScript = listView.GetComponent<ListViewUIScript>();

            List<string> sortedLanguages = Localization.GetLanguages().ToList();
            sortedLanguages.Sort();

            foreach (var language in sortedLanguages)
            {
                MNode element = CreateElement(language);

                languagesNodes.Add(language, element);
                nodesLanguages.Add(element, language);

                listViewScript.AddItem(element);
            }

            attentionText = new MyText(ParentNode.Scene);
            attentionText.Y = listView.LocalY + listView.Height + 5;
            attentionText.X = 8;
            attentionText.Color = Color.Yellow;
            ParentNode.AddChildNode(attentionText);

            UpdateNodes();
        }

        public override void Update(int mouseX, int mouseY)
        {

        }

        private MNode CreateElement(string language)
        {
            MButtonUI element = new MButtonUI(ParentNode.Scene);

            element.Image.Texture = TextureBank.UITexture.GetSubtexture(192, 192, 24, 24);
            element.Image.ImageType = ImageType.Sliced;
            element.Image.SetBorder(8, 8, 8, 8);
            element.Image.BackgroundOverlap = 2;

            element.ButtonScript.AddOnClickedCallback(SelectLanguage);
            element.ButtonScript.SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);
            element.Width = ParentNode.Width - (16 + 16);
            element.Height = 34;

            string languageName = Localization.GetLanguageName(language);

            MyText itemName = new MyText(ParentNode.Scene);
            itemName.Text = $"{languageName} [{language}]";
            itemName.Name = "Name";
            itemName.Width = 100;
            itemName.Height = 32;
            itemName.X = 8;
            itemName.Y = 2;

            element.AddChildNode(itemName);

            return element;
        }

        public void SelectLanguage(bool value, ButtonScript sender)
        {
            MNode element = sender.ParentNode;

            string selectedLanguage = nodesLanguages[element];

            Localization.SetLanguage(selectedLanguage);

            if (Localization.StartLanguage == selectedLanguage)
            {
                attentionText.Active = false;
            }
            else
            {
                attentionText.Active = true;
                ((MyText)attentionText).Text = Localization.GetLocalizedText("language_change_warning");
            }

            UpdateNodes();
        }

        private void UpdateNodes()
        {
            string language = Localization.CurrentLanguage;

            foreach (var kvp in languagesNodes)
            {
                if (language == kvp.Key)
                {
                    kvp.Value.GetComponent<ButtonScript>().SetDefaultColor(Color.Orange, Color.Orange, Color.Orange);
                }
                else
                {
                    kvp.Value.GetComponent<ButtonScript>().SetDefaultColor(Color.White * 0.0f, Color.White, Color.White);
                }
            }
        }

        public void Close(bool value, ButtonScript buttonScript)
        {
            ParentNode.Active = false;
        }
    }
}
