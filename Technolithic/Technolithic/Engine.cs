using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using Newtonsoft.Json;
using Steamworks;

namespace Technolithic
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Engine : Game
    {
        public string Title { get; private set; }

        public static Engine Instance { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }

        public static int Width { get; private set; }
        public static int Height { get; private set; }
        public static int HalfWidth { get; private set; }
        public static int HalfHeight { get; private set; }

        public static float GameDeltaTime { get; private set; }
        public static float DeltaTime { get; private set; }
        public static GameTime GameTime { get; private set; }

        public const int TILE_SIZE = 16;

        public static Color ClearColor;

        private Scene scene;
        private Scene nextScene;

        public static int GameSpeed { get; set; } = 1;

        public MainMenuScene MainMenuScene { get; private set; }
        public GameplayScene GameplayScene { get; private set; }

        public Action<Scene> OnSceneLoadedCallback;
        public Action<int, int> OnScreenSizeChanged { get; set; }

        public static Color GREEN_COLOR { get; } = Color.YellowGreen;
        public static Color ORANGE { get; } = Color.Orange;
        public static Color RED_COLOR { get; } = Color.IndianRed;
        public static Color YELLOW_GREEN { get; } = new Color(120, 255, 0);

        public static RasterizerState RasterizerState { get; private set; }

#if !CONSOLE
        private static string AssemblyDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#endif

        public static string ContentDirectory
        {
            get { return Path.Combine(AssemblyDirectory, Instance.Content.RootDirectory); }
        }

        public const string GAME_NAME = "Paleon";
        public static GameVersion GameVersion { get; } = new GameVersion(1, 7, 0);

#if DEBUG
        public static string VERSION_STRING { get; private set; } = $"{GameVersion.ToString()} [DEBUG]";
#elif RELEASE
        public static string VERSION_STRING { get; private set; } = GameVersion.ToString();
#endif

        public Dictionary<string, BuildingTemplate> Buildings { get; private set; } = new Dictionary<string, BuildingTemplate>();
        public Dictionary<BuildingCategory, List<BuildingTemplate>> BuildingsByCategories = new Dictionary<BuildingCategory, List<BuildingTemplate>>();
        public Dictionary<int, BuildingTemplate> SurfaceIdBuildingTemplate = new();
        public Dictionary<int, BuildingTemplate> WallIdBuildingTemplate = new();
        public Dictionary<int, string> SeasonsLocalizations = new Dictionary<int, string>();
        public List<Item> SettlerRation { get; private set; } = new List<Item>();
        public WorldGenerationData WorldGenerationData { get; private set; }

        public List<Item> DefaultItemsUnlocks { get; private set; }
        public List<BuildingTemplate> DefaultBuildingsUnlocks { get; private set; }

        public FrameCounter FrameCounter { get; private set; }

        public static InteractionsDatabase InteractionsDatabase { get; private set; }

        private bool resizing = false;

        private bool isSteamRunning = false;

        public Engine()
        {
            Instance = this;

            Title = GAME_NAME + " " + VERSION_STRING;
            ClearColor = Color.Black;

            Graphics = new GraphicsDeviceManager(this);
            Graphics.IsFullScreen = false;
            Graphics.ApplyChanges();

            Content.RootDirectory = @"Content";

            IsMouseVisible = true;

            Window.ClientSizeChanged += OnClientSizeChanged;

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            FrameCounter = new FrameCounter();

            try
            {
                SteamClient.Init(1554220);
                isSteamRunning = true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ChangeScreenSize(0, 0, true);

            RasterizerState = new RasterizerState() { ScissorTestEnable = true };

            base.Initialize();

            Window.Title = "Paleon";

            GameSettings.Load();
            SoundEffect.MasterVolume = GameSettings.SoundVolume / 100f;
            MediaPlayer.Volume = GameSettings.MusicVolume / 100f;

            MInput.Initialize();

            Localization.Initialize();

            AchievementDatabase.Initialize(new Tileset(ResourceManager.GetTexture("achievements_icons"), 16, 16));

            ItemDatabase.Initialize();

            ImportSettlerRation();

            AnimalTemplateDatabase.Initialize();

            ImportBuildings();

            ImportDefaultUnlocks();

            ImportWorldGenerationData();

            ImportSeasons();

            TechnologyDatabase.Initialize(ContentDirectory);

            InteractionsDatabase = new InteractionsDatabase();

            MainMenuScene = new MainMenuScene();

            Scene = MainMenuScene;

            GlobalUI.Initialize();
        }

        protected override void OnExiting(object sender, ExitingEventArgs args)
        {
            GameSettings.Save();

            base.OnExiting(sender, args);

            if(isSteamRunning)
            {
                SteamClient.Shutdown();
            }
        }

        private void ImportSeasons()
        {
            JObject jobject = JObject.Parse(File.ReadAllText(Path.Combine(Engine.ContentDirectory, "seasons.json")));

            foreach (var kvp in jobject)
            {
                int key = int.Parse(kvp.Key);
                string name = Localization.GetLocalizedText(kvp.Value["key"].Value<string>());
                SeasonsLocalizations.Add(key, name);
            }
        }

        private void ImportSettlerRation()
        {
            string jsonString = File.ReadAllText(Path.Combine(ContentDirectory, "settler_ration.json"));

            foreach (var item in JArray.Parse(jsonString))
            {
                SettlerRation.Add(ItemDatabase.GetItemByName(item.Value<string>()));
            }

            SettlerRation = SettlerRation.OrderBy(x => x.Consumable.Statistics[AttributeType.Happiness]).ToList();
        }

        private void ImportWorldGenerationData()
        {
            string jsonString = File.ReadAllText(Path.Combine(ContentDirectory, "world_generation.json"));

            WorldGenerationData = JsonConvert.DeserializeObject<WorldGenerationData>(jsonString);
        }

        private void ImportBuildings()
        {
            foreach (string file in Directory.GetFiles(Path.Combine(Engine.ContentDirectory, "Buildings")))
            {
                if (Path.GetExtension(file) == ".json")
                {
                    BuildingTemplate buildingTemplate = new BuildingTemplate(Path.GetFileName(file));

                    Buildings.Add(Path.GetFileNameWithoutExtension(file), buildingTemplate);

                    if (!BuildingsByCategories.ContainsKey(buildingTemplate.BuildingCategory))
                        BuildingsByCategories.Add(buildingTemplate.BuildingCategory, new List<BuildingTemplate>());

                    BuildingsByCategories[buildingTemplate.BuildingCategory].Add(buildingTemplate);
                
                    if(buildingTemplate.BuildingType == BuildingType.Surface)
                    {
                        SurfaceIdBuildingTemplate.Add(buildingTemplate.SurfaceData.Id, buildingTemplate);
                    }
                    else if(buildingTemplate.BuildingType == BuildingType.Wall)
                    {
                        WallIdBuildingTemplate.Add(buildingTemplate.WallData.Id, buildingTemplate);
                    }
                }
            }
        }

        private void ImportDefaultUnlocks()
        {
            string jsonString = File.ReadAllText(Path.Combine(ContentDirectory, "default_unlocks.json"));

            JObject jobject = JObject.Parse(jsonString);

            DefaultItemsUnlocks = new List<Item>();

            foreach (var itemKey in jobject["Items"])
            {
                Item item = ItemDatabase.GetItemByName(itemKey.Value<string>());

                DefaultItemsUnlocks.Add(item);
            }

            DefaultBuildingsUnlocks = new List<BuildingTemplate>();

            foreach (var buildingTemplateKey in jobject["Buildings"])
            {
                BuildingTemplate buildingTemplate = Buildings[buildingTemplateKey.Value<string>()];

                DefaultBuildingsUnlocks.Add(buildingTemplate);
            }
        }

        public void ChangeScreenSize(int width = 0, int height = 0, bool fullscreen = true)
        {
            Width = width == 0 ? GraphicsDevice.DisplayMode.Width : width;
            Height = height == 0 ? GraphicsDevice.DisplayMode.Height : height;
            HalfWidth = Width / 2;
            HalfHeight = Height / 2;

            Graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            Graphics.PreferMultiSampling = false;
            Graphics.SynchronizeWithVerticalRetrace = true;

            Graphics.HardwareModeSwitch = false;
            Graphics.IsFullScreen = fullscreen;

            Graphics.ApplyChanges();

            GraphicsDevice.Viewport = new Viewport(0, 0, Width, Width);
            GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, Width, Width);
            Graphics.PreferredBackBufferWidth = Width;
            Graphics.PreferredBackBufferHeight = Height;

            Graphics.ApplyChanges();
        }

        public static string GetGameDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GAME_NAME);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            RenderManager.Initialize(GraphicsDevice);

            ResourceManager.Initialize(Content);

            TextureBank.Initialize();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GameDeltaTime = DeltaTime * GameSpeed;

            //Update input
            if (IsActive)
            {
                MInput.Update();
            }

            GlobalUI.Update();

            if (scene != null)
            {
                scene.UpdateLists();
                scene.Update();
            }

            if (scene != nextScene)
            {
                var lastScene = scene;
                if(scene != null)
                {
                    scene.End();
                }
                scene = nextScene;
                OnSceneTransition(lastScene, nextScene);
                if (scene != null)
                {
                    scene.Initialize();
                    scene.Begin();
                    scene.UpdateLists();
                    OnSceneLoadedCallback?.Invoke(scene);
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            FrameCounter.Update(deltaTime);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearColor);

            if (scene != null)
                scene.Render();

            RenderManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null,
                Engine.RasterizerState);

            GlobalUI.Render();

            RenderManager.SpriteBatch.End();

            base.Draw(gameTime);
        }

        public static Scene Scene
        {
            get { return Instance.scene; }
            set { Instance.nextScene = value; }
        }

        protected virtual void OnSceneTransition(Scene from, Scene to)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        protected virtual void OnClientSizeChanged(object sender, EventArgs e)
        {
            int width = Window.ClientBounds.Width;
            int height = Window.ClientBounds.Height;
            if (width > 0 && height > 0 && !resizing)
            {
                resizing = true;

                Width = width;
                Height = height;
                HalfWidth = Width / 2;
                HalfHeight = Height / 2;

                Graphics.PreferredBackBufferWidth = width;
                Graphics.PreferredBackBufferHeight = height;
                Graphics.ApplyChanges();

                OnScreenSizeChanged?.Invoke(Width, Height);

                resizing = false;
            }
        }

        public void SetWindowed(int width, int height)
        {
            if (width > 0 && height > 0)
            {
                resizing = true;

                Width = width;
                Height = height;
                HalfWidth = Width / 2;
                HalfHeight = Height / 2;

                Graphics.PreferredBackBufferWidth = width;
                Graphics.PreferredBackBufferHeight = height;
                Graphics.IsFullScreen = false;
                Graphics.ApplyChanges();
                Window.AllowUserResizing = true;

                OnScreenSizeChanged?.Invoke(Width, Height);

                resizing = false;
            }
        }

        public void SetFullscreen()
        {
            resizing = true;

            Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            HalfWidth = Width / 2;
            HalfHeight = Height / 2;

            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
            Window.AllowUserResizing = false;

            OnScreenSizeChanged?.Invoke(Width, Height);

            resizing = false;
        }
    }
}
