using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Media;

namespace Technolithic
{
    public class MainMenuScene : Scene
    {

        public static MainMenuScene Instance { get; private set; }

        private MNode uiNode;

        private MImageUI logoImage;

        private LocalizationUI localizationUI;
        private LoadGameUI loadGameUI;
        private WorldSettingsUI worldSettingsUI;
        private CreditsUI creditsUI;

        private MButtonUI newGameButton;
        private MButtonUI languageButton;
        private MButtonUI loadGameButton;
        private MButtonUI exitToDesktopButton;

        private MButtonUI windowModeButton;
        private MButtonUI openGameDataButton;
        private MButtonUI creditsButton;
        private MButtonUI musicOnOffButton;

        private MyText gameVersionText;

        private List<MButtonUI> buttons = new List<MButtonUI>();

        private List<MImage> stars = new List<MImage>();

        private Vector2 relative;

        private Song mainThemeSong;

        public MainMenuScene()
        {
            Instance = this;

            uiNode = new MNode(this);

            MyTexture starTexture = ResourceManager.GetTexture("star");

            CreateStars(50, 5, starTexture.GetSubtexture(16, 0, 16, 16));
            CreateStars(30, 11, starTexture.GetSubtexture(0, 16, 16, 16));
            CreateStars(10, 22, starTexture.GetSubtexture(0, 0, 16, 16));

            MyTexture logoTexture = ResourceManager.GetTexture("logo");

            logoImage = new MImageUI(this);
            logoImage.GetComponent<MImageCmp>().Texture = logoTexture;
            logoImage.GetComponent<MImageCmp>().CenterOrigin();
            logoImage.Width = logoTexture.Texture.Width * 3;
            logoImage.Height = logoTexture.Texture.Height * 3;

            logoImage.X = Engine.Width / 2;
            logoImage.Y = Engine.Height / 2;

            uiNode.AddChildNode(logoImage);

            buttons.Add(newGameButton = CreateButton(this, Localization.GetLocalizedText("new_game"), StartNewGame));
            buttons.Add(languageButton = CreateButton(this, Localization.GetLocalizedText("language"), OpenLanguageUI));
            buttons.Add(loadGameButton = CreateButton(this, Localization.GetLocalizedText("load_game"), OpenLoadGameUI));
            buttons.Add(exitToDesktopButton = CreateButton(this, Localization.GetLocalizedText("exit_to_desktop"), ExitToDesktop));

            windowModeButton = new BigButton(this, ResourceManager.WindowedModeIcon, false, false);
            windowModeButton.X = Engine.Width - windowModeButton.Width - 5;
            windowModeButton.Y = 5;
            windowModeButton.Tooltips = Localization.GetLocalizedText("windowed");
            windowModeButton.GetComponent<ButtonScript>().AddOnClickedCallback(SwitchToWindowMode);
            uiNode.AddChildNode(windowModeButton);

            openGameDataButton = new BigButton(this, ResourceManager.OpenedFolderIcon, false, false);
            openGameDataButton.X = windowModeButton.X - windowModeButton.Width - 5;
            openGameDataButton.Y = 5;
            openGameDataButton.GetComponent<ButtonScript>().AddOnClickedCallback(OpenGameDataFolder);
            openGameDataButton.Tooltips = Localization.GetLocalizedText("open_game_data_folder");
            uiNode.AddChildNode(openGameDataButton);

            creditsButton = new BigButton(this, ResourceManager.CreditsIcon, false, false);
            creditsButton.X = openGameDataButton.X - openGameDataButton.Width - 5;
            creditsButton.Y = 5;
            creditsButton.Tooltips = Localization.GetLocalizedText("credits");
            creditsButton.GetComponent<ButtonScript>().AddOnClickedCallback(OpenCreditsUI);
            uiNode.AddChildNode(creditsButton);

            musicOnOffButton = new BigButton(this, GameSettings.PlayMainTheme ? ResourceManager.MusicOnIcon : ResourceManager.MusicOffIcon,
                false, false);
            musicOnOffButton.X = creditsButton.X - creditsButton.Width - 5;
            musicOnOffButton.Y = 5;
            musicOnOffButton.GetComponent<ButtonScript>().AddOnClickedCallback(OnMusicOnOffButtonPressed);
            musicOnOffButton.SetMetadata("musicOn", GameSettings.PlayMainTheme);
            musicOnOffButton.Tooltips = GameSettings.PlayMainTheme ? Localization.GetLocalizedText("turn_off_music") :
                Localization.GetLocalizedText("turn_on_music");
            uiNode.AddChildNode(musicOnOffButton);

            int offsetBetweenButtons = 16;

            int totalButtonsWidth = newGameButton.Width * buttons.Count + offsetBetweenButtons * buttons.Count - 1;
            int buttonsStartX = Engine.Width / 2 - totalButtonsWidth / 2;

            for (int i = 0; i < buttons.Count; i++)
            {
                MButtonUI button = buttons[i];
                button.X = buttonsStartX + i * (newGameButton.Width + offsetBetweenButtons);
                button.Y = Engine.Height - button.Height - offsetBetweenButtons;
            }

            localizationUI = new LocalizationUI(this, Localization.GetLocalizedText("language"));
            localizationUI.X = Engine.Width / 2 - localizationUI.Width / 2;
            localizationUI.Y = Engine.Height / 2 - localizationUI.Height / 2;
            localizationUI.Active = false;
            uiNode.AddChildNode(localizationUI);

            creditsUI = new CreditsUI(this, "Paleon");
            creditsUI.X = Engine.Width / 2 - creditsUI.Width / 2;
            creditsUI.Y = Engine.Height / 2 - creditsUI.Height / 2;
            creditsUI.Active = false;
            uiNode.AddChildNode(creditsUI);

            loadGameUI = new LoadGameUI(this, Localization.GetLocalizedText("load_game"));
            loadGameUI.X = Engine.Width / 2 - loadGameUI.Width / 2;
            loadGameUI.Y = Engine.Height / 2 - loadGameUI.Height / 2;
            loadGameUI.Active = false;
            uiNode.AddChildNode(loadGameUI);

            worldSettingsUI = new WorldSettingsUI(this, Localization.GetLocalizedText("world_settings"));
            worldSettingsUI.X = Engine.Width / 2 - worldSettingsUI.Width / 2;
            worldSettingsUI.Y = Engine.Height / 2 - worldSettingsUI.Height / 2;
            worldSettingsUI.Active = false;
            uiNode.AddChildNode(worldSettingsUI);

            gameVersionText = new MyText(this);
            gameVersionText.Text = Engine.GAME_NAME + " " + Engine.VERSION_STRING;
            gameVersionText.X = 4;
            gameVersionText.Y = 4;
            uiNode.AddChildNode(gameVersionText);

            mainThemeSong = ResourceManager.GetSong("main_theme");

            MediaPlayer.IsRepeating = false;
            MediaPlayer.Play(mainThemeSong);

            if (GameSettings.PlayMainTheme == false)
            {
                MediaPlayer.IsMuted = true;
            }
        }

        private void CreateStars(int count, float layer, MyTexture texture)
        {
            for (int i = 0; i < count; i++)
            {
                MImage image = new MImage(this);
                image.X = MyRandom.Range(20, Engine.Width - 25);
                image.Y = MyRandom.Range(25, Engine.Height - 25);
                image.Active = true;
                image.Width = 48;
                image.Height = 48;
                image.Texture = texture;
                uiNode.AddChildNode(image);

                stars.Add(image);
                image.SetMetadata("originalPosition", new Vector2(image.X, image.Y));
                image.SetMetadata("layer", layer);
                image.SetMetadata("alphaValue", MyRandom.NextFloat());
                image.SetMetadata("alphaReverse", false);
            }
        }

        private void UpdatePositions()
        {
            logoImage.X = Engine.Width / 2;
            logoImage.Y = Engine.Height / 2;

            windowModeButton.X = Engine.Width - windowModeButton.Width - 5;
            windowModeButton.Y = 5;

            openGameDataButton.X = windowModeButton.X - windowModeButton.Width - 5;
            openGameDataButton.Y = 5;

            creditsButton.X = openGameDataButton.X - openGameDataButton.Width - 5;
            creditsButton.Y = 5;

            musicOnOffButton.X = creditsButton.X - creditsButton.Width - 5;
            musicOnOffButton.Y = 5;

            int offsetBetweenButtons = 16;

            int totalButtonsWidth = newGameButton.Width * buttons.Count + offsetBetweenButtons * buttons.Count - 1;
            int buttonsStartX = Engine.Width / 2 - totalButtonsWidth / 2;

            for (int i = 0; i < buttons.Count; i++)
            {
                MButtonUI button = buttons[i];
                button.X = buttonsStartX + i * (newGameButton.Width + offsetBetweenButtons);
                button.Y = Engine.Height - button.Height - offsetBetweenButtons;
            }

            localizationUI.X = Engine.Width / 2 - localizationUI.Width / 2;
            localizationUI.Y = Engine.Height / 2 - localizationUI.Height / 2;

            loadGameUI.X = Engine.Width / 2 - loadGameUI.Width / 2;
            loadGameUI.Y = Engine.Height / 2 - loadGameUI.Height / 2;

            worldSettingsUI.X = Engine.Width / 2 - worldSettingsUI.Width / 2;
            worldSettingsUI.Y = Engine.Height / 2 - worldSettingsUI.Height / 2;

            creditsUI.X = Engine.Width / 2 - creditsUI.Width / 2;
            creditsUI.Y = Engine.Height / 2 - creditsUI.Height / 2;

            gameVersionText.X = 4;
            gameVersionText.Y = 4;
        }

        public override void Initialize()
        {
            Engine.ClearColor = new Color(4, 0, 31);   
        }

        private MButtonUI CreateButton(Scene scene, string name, Action<bool, ButtonScript> actionOnButtonPressed)
        {
            MButtonUI button = new MButtonUI(scene);
            button.Image.Texture = TextureBank.UITexture.GetSubtexture(144, 192, 24, 24);
            button.Image.ImageType = ImageType.Sliced;
            button.Image.SetBorder(8, 8, 8, 8);
            button.Image.BackgroundOverlap = 2;
            button.Name = name;
            button.ButtonScript.AddOnClickedCallback(actionOnButtonPressed);

            button.Width = 250;
            button.Height = 50;

            MyText text = new MyText(scene);
            text.Text = name;
            text.Name = "Text";
            text.X = 8;
            text.Y = 8;
            button.AddChildNode(text);

            uiNode.AddChildNode(button);

            return button;
        }

        private void OpenGameDataFolder(bool value, ButtonScript buttonScript)
        {
            string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string gameDataPath = Path.Combine(appdataPath, "Paleon");

            if (Directory.Exists(gameDataPath))
            {
                if (OperatingSystem.IsLinux())
                {
                    Process.Start("xdg-open", gameDataPath);
                }
                else if(OperatingSystem.IsWindows())
                {
                    Process.Start("explorer.exe", gameDataPath);
                }
            }
        }

        private void SwitchToWindowMode(bool value, ButtonScript buttonScript)
        {
            if (Engine.Graphics.IsFullScreen)
            {
                int width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                int height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

                int widthFourth = width / 8;
                width = widthFourth * 7;

                int heightFourth = height / 8;
                height = heightFourth * 7;

                Engine.Instance.SetWindowed(width, height);

                windowModeButton.GetChildByName("Icon").GetComponent<MImageCmp>().Texture = ResourceManager.FullscreenModeIcon;
                windowModeButton.Tooltips = Localization.GetLocalizedText("fullscreen");
            }
            else
            {
                Engine.Instance.SetFullscreen();

                windowModeButton.GetChildByName("Icon").GetComponent<MImageCmp>().Texture = ResourceManager.WindowedModeIcon;
                windowModeButton.Tooltips = Localization.GetLocalizedText("windowed");
            }

            //UpdatePositions();
        }

        private void StartNewGame(bool value, ButtonScript button)
        {
            worldSettingsUI.Active = true;

            localizationUI.Active = false;
            loadGameUI.Active = false;
            creditsUI.Active = false;
        }

        private void OpenCreditsUI(bool value, ButtonScript buttonScript)
        {
            creditsUI.Active = true;

            loadGameUI.Active = false;
            worldSettingsUI.Active = false;
            localizationUI.Active = false;
        }

        private void OnMusicOnOffButtonPressed(bool value, ButtonScript buttonScript)
        {
            bool musicOn = buttonScript.ParentNode.GetMetadata<bool>("musicOn");
            musicOn = !musicOn;
            buttonScript.ParentNode.SetMetadata("musicOn", musicOn);

            if (musicOn)
            {
                buttonScript.ParentNode.GetChildByName("Icon").GetComponent<MImageCmp>().Texture = ResourceManager.MusicOnIcon;
                buttonScript.ParentNode.Tooltips = Localization.GetLocalizedText("turn_off_music");
                MediaPlayer.IsMuted = false;
                GameSettings.PlayMainTheme = true;
            }
            else
            {
                buttonScript.ParentNode.GetChildByName("Icon").GetComponent<MImageCmp>().Texture = ResourceManager.MusicOffIcon;
                buttonScript.ParentNode.Tooltips = Localization.GetLocalizedText("turn_on_music");
                MediaPlayer.IsMuted = true;
                GameSettings.PlayMainTheme = false;
            }
        }

        private void OpenLanguageUI(bool value, ButtonScript buttonScript)
        {
            localizationUI.Active = true;

            loadGameUI.Active = false;
            worldSettingsUI.Active = false;
            creditsUI.Active = false;
        }

        private void OpenLoadGameUI(bool value, ButtonScript button)
        {
            loadGameUI.Active = true;
            loadGameUI.GetComponent<LoadGameUIScript>().Open();

            localizationUI.Active = false;
            worldSettingsUI.Active = false;
            creditsUI.Active = false;
        }

        private void ExitToDesktop(bool value, ButtonScript button)
        {
            Engine.Instance.Exit();
        }

        public override void Begin()
        {
            uiNode.Awake();

            uiNode.Begin();

            Engine.Instance.Window.ClientSizeChanged += OnClientSizeChanged;
        }

        private void OnClientSizeChanged(object sender, EventArgs e)
        {
            UpdatePositions();
        }


        public override void Update()
        {
            uiNode.Update(MInput.Mouse.X, MInput.Mouse.Y);

            relative.X = -1.0f * (float)(MInput.Mouse.X - (Engine.Width / 2)) / (float)(Engine.Width / 2);
            relative.Y = -1.0f * (float)(MInput.Mouse.Y - (Engine.Height / 2)) / (float)(Engine.Height / 2);

            foreach(var star in stars)
            {
                Vector2 originalPosition = star.GetMetadata<Vector2>("originalPosition");
                float layer = star.GetMetadata<float>("layer");
                Vector2 offsetPosition = originalPosition + relative * layer;
                star.X = (int)offsetPosition.X;
                star.Y = (int)offsetPosition.Y;
                float alphaValue = star.GetMetadata<float>("alphaValue");
                star.Color = Color.White * alphaValue;
                if (!star.GetMetadata<bool>("alphaReverse"))
                {
                    star.SetMetadata("alphaValue", alphaValue + Engine.DeltaTime * 0.5f);
                    if (alphaValue >= 1.0f)
                    {
                        star.SetMetadata("alphaReverse", true);
                    }
                }
                else
                {
                    star.SetMetadata("alphaValue", alphaValue - Engine.DeltaTime * 0.5f);
                    if (alphaValue <= 0.0f)
                    {
                        star.SetMetadata("alphaReverse", false);
                        star.X = MyRandom.Range(20, Engine.Width - 25);
                        star.Y = MyRandom.Range(25, Engine.Height - 25);
                        star.SetMetadata("originalPosition", new Vector2(star.X, star.Y));
                    }
                }
            }

            logoImage.X = (int)(Engine.Width / 2 + relative.X * 40);
            logoImage.Y = (int)(Engine.Height / 2 + relative.Y * 40);

            base.Update();
        }

        public override void Render()
        {
            RenderManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null,
                Engine.RasterizerState);

            uiNode.Render();

            RenderManager.SpriteBatch.End();
        }

    }
}
