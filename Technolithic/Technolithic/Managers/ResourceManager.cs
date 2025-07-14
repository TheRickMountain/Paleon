using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Technolithic
{
    public static class ResourceManager
    {

        private static Dictionary<string, MyTexture> sprites;
        private static Dictionary<string, SoundEffect> soundEffects;
        private static Dictionary<string, Song> songs;

        public static MyTexture RenameIcon { get; private set; }
        public static MyTexture GearIcon { get; private set; }
        public static MyTexture EyeIcon { get; private set; }
        public static MyTexture AcceptIcon { get; private set; }
        public static MyTexture CancelIcon { get; private set; }
        public static MyTexture HuntIcon { get; private set; }
        public static MyTexture SlaughterIcon { get; private set; }
        public static MyTexture DomesticateIcon { get; private set; }
        public static MyTexture ArrowIcon { get; private set; }
        public static MyTexture AgricultureIcon { get; private set; }
        public static MyTexture IrrigationIcon { get; private set; }
        public static MyTexture FertilizeIcon { get; private set; }
        public static MyTexture TurnOnIcon { get; private set; }
        public static MyTexture DestructIcon { get; private set; }
        public static MyTexture ChopIcon { get; private set; }
        public static MyTexture CutIcon { get; private set; }
        public static MyTexture CutCompletelyIcon { get; private set; }
        public static MyTexture FoodIcon { get; private set; }
        public static MyTexture Slot24 { get; private set; }
        public static MyTexture ExclamationMarkIcon { get; private set; }
        public static MyTexture NoFuelIcon { get; private set; }
        public static MyTexture NoEnergyIcon { get; private set; }
        public static MyTexture NoGasIcon { get; private set; }
        public static MyTexture FuelIcon { get; private set; }
        public static MyTexture ResourcesLimitIcon { get; private set; }
        public static MyTexture ResourcesIcon { get; private set; }
        public static MyTexture CopyIcon { get; private set; }
        public static MyTexture PasteIcon { get; private set; }
        public static MyTexture TradingIcon { get; private set; }
        public static MyTexture DestructSurfaceIcon { get; private set; }
        public static MyTexture FishTexture { get; private set; }
        public static MyTexture RemoveEquipmentIcon { get; private set; }
        public static MyTexture QuestionMarkIcon { get; private set; }

        public static MyTexture NotificationDangerIcon { get; private set; }
        public static MyTexture NotificationInfoIcon { get; private set; }
        public static MyTexture NotificationWarningIcon { get; private set; }
        public static MyTexture NotificationAchievementIcon { get; private set; }

        public static MyTexture SunAndMoonSprite { get; private set; }
        public static MyTexture SunAndMoonPointerSprite { get; private set; }
        public static MyTexture GatherStoneIcon { get; private set; }
        public static MyTexture GatherWoodIcon { get; private set; }
        public static MyTexture MineIcon { get; private set; }
        public static MyTexture MiningIcon { get; private set; }
        public static MyTexture LampIcon { get; private set; }
        public static MyTexture MetallurgyIcon { get; private set; }
        public static MyTexture MechanismIcon { get; private set; }
        public static MyTexture BeeIcon { get; private set; }
        public static MyTexture EquipmentIcon { get; private set; }
        public static MyTexture HungerThought { get; private set; }
        public static MyTexture SleepThought { get; private set; }
        public static MyTexture HealthThought { get; private set; }
        public static MyTexture HappyThought { get; private set; }
        public static MyTexture NevermindThought { get; private set; }
        public static MyTexture LoveThought { get; private set; }
        public static MyTexture PoisonedThought { get; private set; }
        public static MyTexture UnhappyThought { get; private set; }
        public static MyTexture ColdThought { get; private set; }
        public static MyTexture BedBottom { get; private set; }
        public static MyTexture BedTop { get; private set;}

        public static MyTexture PickAnimation { get; private set; }
        public static Tileset CreaturesTileset { get; private set; }

        public static Tileset HairsTileset { get; private set; }

        public static Tileset AmmoTileset { get; private set; }

        public static MyTexture RotateSprite { get; private set; }

        public static MyTexture FileIcon { get; private set; }
        public static MyTexture FolderIcon { get; private set; }
        public static MyTexture WindowedModeIcon { get; private set; }
        public static MyTexture OpenedFolderIcon { get; private set; }
        public static MyTexture FullscreenModeIcon { get; private set; }

        public static MyTexture LeftSliderBarTexture { get; private set; }
        public static MyTexture CenterSliderBarTexture { get; private set; }
        public static MyTexture RightSliderBarTexture { get; private set; }
        public static MyTexture DraggerTexture { get; private set; }
        public static MyTexture PriorityIcon { get; private set; }
        public static MyTexture CreditsIcon { get; private set; }
        public static MyTexture MusicOnIcon { get; private set; }
        public static MyTexture MusicOffIcon { get; private set; }


        public static MyTexture HungerIndicator { get; private set; }
        public static MyTexture ColdIndicator { get; private set; }
        public static MyTexture UnhappyIndicator { get; private set; }

        public static MyTexture UnitSelectorSprite { get; private set; }

        public static MyTexture MoveToIcon { get; private set; }
        public static MyTexture DisbandIcon { get; private set; }
        public static MyTexture AttackIcon { get; private set; }

        public static MyTexture MedicineIcon { get; private set; }
        public static MyTexture PetIcon { get; private set; }
        public static MyTexture OwnershipIcon { get; private set; }
        public static MyTexture EnergyProductionIcon { get; private set; }
        public static MyTexture HutIcon { get; private set; }
        public static MyTexture EmptyFishTrapIcon { get; private set; }
        public static MyTexture CollectHoneyIcon { get; private set; }
        public static MyTexture DiscordIcon { get; private set; }
        public static MyTexture CatchFishIcon { get; private set; }
        public static MyTexture CleanUpIcon { get; private set; }
        public static MyTexture DisableIcon { get; private set; }
        public static MyTexture ProduceEnergyIcon { get; private set; }

        public static SoundEffectInstance BuildingSoundEffect { get; private set; }
        public static SoundEffectInstance ClickSoundEffect { get; private set; }
        public static SoundEffectInstance NotificationInfoSoundEffect { get; private set; }
        public static SoundEffectInstance NotificationDangerSoundEffect { get; private set; }
        public static SoundEffectInstance NotificationWarningSoundEffect { get; private set; }
        public static SoundEffectInstance NotificationAchievementSoundEffect { get; private set; }

        public static MyTexture CloudTexture { get; private set; }

        private static MyTexture uiSpriteSheet;

        private static bool wasInitialized = false;

        public static void Initialize(ContentManager content)
        {
            sprites = LoadSprites(content, "Sprites", "Buildings", "Animals", "Technologies");
            soundEffects = LoadContent<SoundEffect>(content, "Sound");
            songs = LoadContent<Song>(content, "Songs");

            wasInitialized = true;

            uiSpriteSheet = GetTexture("ui");

            CloudTexture = uiSpriteSheet.GetSubtexture(144, 96, 18, 18);

            LeftSliderBarTexture = uiSpriteSheet.GetSubtexture(0, 208, 2, 8);
            CenterSliderBarTexture = uiSpriteSheet.GetSubtexture(2, 208, 1, 8);
            RightSliderBarTexture = uiSpriteSheet.GetSubtexture(3, 208, 2, 8);
            DraggerTexture = uiSpriteSheet.GetSubtexture(7, 208, 3, 12);
            PriorityIcon = uiSpriteSheet.GetSubtexture(48, 144, 16, 16);
            CreditsIcon = uiSpriteSheet.GetSubtexture(144, 144, 16, 16);
            MusicOnIcon = uiSpriteSheet.GetSubtexture(48, 176, 16, 16);
            MusicOffIcon = uiSpriteSheet.GetSubtexture(64, 176, 16, 16);

            MoveToIcon = uiSpriteSheet.GetSubtexture(48, 240, 16, 16);
            DisbandIcon = uiSpriteSheet.GetSubtexture(208, 128, 16, 16);
            AttackIcon = uiSpriteSheet.GetSubtexture(64, 240, 16, 16);

            RenameIcon = uiSpriteSheet.GetSubtexture(160, 128, 16, 16);
            NotificationDangerIcon = uiSpriteSheet.GetSubtexture(16, 240, 16, 16);
            NotificationInfoIcon = uiSpriteSheet.GetSubtexture(32, 240, 16, 16);
            NotificationWarningIcon = uiSpriteSheet.GetSubtexture(96, 240, 16, 16);
            NotificationAchievementIcon = uiSpriteSheet.GetSubtexture(80, 240, 16, 16);
            FileIcon = uiSpriteSheet.GetSubtexture(16, 144, 16, 16);
            FolderIcon = uiSpriteSheet.GetSubtexture(32, 144, 16, 16);
            OpenedFolderIcon = uiSpriteSheet.GetSubtexture(16, 160, 16, 16);
            WindowedModeIcon = uiSpriteSheet.GetSubtexture(112, 144, 16, 16);
            FullscreenModeIcon = uiSpriteSheet.GetSubtexture(128, 144, 16, 16);
            CopyIcon = uiSpriteSheet.GetSubtexture(80, 128, 16, 16);
            PasteIcon = uiSpriteSheet.GetSubtexture(80, 144, 16, 16);
            TradingIcon = uiSpriteSheet.GetSubtexture(96, 128, 16, 16);
            AcceptIcon = uiSpriteSheet.GetSubtexture(48, 48, 16, 16);
            GearIcon = uiSpriteSheet.GetSubtexture(160, 144, 16, 16);
            EyeIcon = uiSpriteSheet.GetSubtexture(112, 176, 16, 16);
            CancelIcon = uiSpriteSheet.GetSubtexture(64, 112, 16, 16);
            HuntIcon = uiSpriteSheet.GetSubtexture(112, 0, 16, 16);
            SlaughterIcon = uiSpriteSheet.GetSubtexture(80, 112, 16, 16);
            DomesticateIcon = uiSpriteSheet.GetSubtexture(128, 112, 16, 16);
            AgricultureIcon = uiSpriteSheet.GetSubtexture(144, 0, 16, 16);
            IrrigationIcon = uiSpriteSheet.GetSubtexture(64, 160, 16, 16);
            FertilizeIcon = uiSpriteSheet.GetSubtexture(208, 48, 16, 16);
            TurnOnIcon = uiSpriteSheet.GetSubtexture(240, 48, 16, 16);
            DestructIcon = uiSpriteSheet.GetSubtexture(0, 16, 16, 16);
            ChopIcon = uiSpriteSheet.GetSubtexture(16, 0, 16, 16);
            CutIcon = uiSpriteSheet.GetSubtexture(64, 0, 16, 16);
            CutCompletelyIcon = uiSpriteSheet.GetSubtexture(48, 16, 16, 16);
            SunAndMoonSprite = uiSpriteSheet.GetSubtexture(192, 96, 32, 32);
            SunAndMoonPointerSprite = uiSpriteSheet.GetSubtexture(224, 96, 32, 32);
            FoodIcon = uiSpriteSheet.GetSubtexture(96, 16, 16, 16);
            Slot24 = uiSpriteSheet.GetSubtexture(176, 224, 24, 24);
            ExclamationMarkIcon = uiSpriteSheet.GetSubtexture(32, 128, 16, 16);
            NoFuelIcon = uiSpriteSheet.GetSubtexture(0, 176, 16, 16);
            FuelIcon = uiSpriteSheet.GetSubtexture(16, 176, 16, 16);
            NoEnergyIcon = uiSpriteSheet.GetSubtexture(0, 192, 16, 16);
            NoGasIcon = uiSpriteSheet.GetSubtexture(16, 192, 16, 16);
            ResourcesLimitIcon = uiSpriteSheet.GetSubtexture(128, 96, 16, 16);
            ResourcesIcon = uiSpriteSheet.GetSubtexture(192, 80, 16, 16);
            DestructSurfaceIcon = uiSpriteSheet.GetSubtexture(0, 32, 16, 16);
            MechanismIcon = uiSpriteSheet.GetSubtexture(0, 144, 16, 16);
            BeeIcon = uiSpriteSheet.GetSubtexture(144, 160, 16, 16);
            FishTexture = uiSpriteSheet.GetSubtexture(16, 208, 16, 16);
            EquipmentIcon = uiSpriteSheet.GetSubtexture(128, 240, 16, 16);
            RemoveEquipmentIcon = uiSpriteSheet.GetSubtexture(112, 96, 16, 16);
            QuestionMarkIcon = uiSpriteSheet.GetSubtexture(176, 32, 16, 16);

            HungerIndicator = uiSpriteSheet.GetSubtexture(48, 192, 24, 24);
            ColdIndicator = uiSpriteSheet.GetSubtexture(72, 192, 24, 24);
            UnhappyIndicator = uiSpriteSheet.GetSubtexture(208, 224, 24, 24);

            RotateSprite = uiSpriteSheet.GetSubtexture(80, 160, 32, 32);

            GatherStoneIcon = uiSpriteSheet.GetSubtexture(48, 128, 16, 16);
            GatherWoodIcon = uiSpriteSheet.GetSubtexture(64, 128, 16, 16);
            MineIcon = uiSpriteSheet.GetSubtexture(128, 0, 16, 16);
            LampIcon = uiSpriteSheet.GetSubtexture(80, 64, 16, 16);
            MetallurgyIcon = uiSpriteSheet.GetSubtexture(224, 16, 16, 16);

            MiningIcon = uiSpriteSheet.GetSubtexture(32, 224, 16, 16);

            PickAnimation = GetTexture("pick");

            CreaturesTileset = new Tileset(GetTexture("creatures"), 24, 24);

            BedBottom = CreaturesTileset[16];
            BedTop = CreaturesTileset[17];

            UnitSelectorSprite = CreaturesTileset[18];

            HairsTileset = new Tileset(GetTexture("hairs"), 24, 24);
            AmmoTileset = new Tileset(GetTexture("tileset"), 16, 16);

            ArrowIcon = uiSpriteSheet.GetSubtexture(96, 96, 16, 16);

            MedicineIcon = uiSpriteSheet.GetSubtexture(128, 16, 16, 16);
            PetIcon = uiSpriteSheet.GetSubtexture(0, 128, 16, 16);
            OwnershipIcon = uiSpriteSheet.GetSubtexture(0, 112, 16, 16);
            EnergyProductionIcon = uiSpriteSheet.GetSubtexture(112, 240, 16, 16);
            HutIcon = uiSpriteSheet.GetSubtexture(16, 128, 16, 16);
            EmptyFishTrapIcon = uiSpriteSheet.GetSubtexture(112, 112, 16, 16);
            CollectHoneyIcon = uiSpriteSheet.GetSubtexture(96, 112, 16, 16);
            DiscordIcon = uiSpriteSheet.GetSubtexture(240, 240, 16, 16);
            CatchFishIcon = uiSpriteSheet.GetSubtexture(112, 128, 16, 16);
            CleanUpIcon = uiSpriteSheet.GetSubtexture(32, 0, 16, 16);
            DisableIcon = uiSpriteSheet.GetSubtexture(32, 176, 16, 16);
            ProduceEnergyIcon = uiSpriteSheet.GetSubtexture(32, 192, 16, 16);

            MyTexture emotest = GetTexture("emotest");
            HungerThought = emotest.GetSubtexture(16, 0, 16, 16);
            SleepThought = emotest.GetSubtexture(0, 0, 16, 16);
            HealthThought = emotest.GetSubtexture(32, 0, 16, 16);
            HappyThought = emotest.GetSubtexture(0, 16, 16, 16);
            NevermindThought = emotest.GetSubtexture(16, 16, 16, 16);
            LoveThought = emotest.GetSubtexture(32, 16, 16, 16);
            PoisonedThought = emotest.GetSubtexture(0, 32, 16, 16);
            UnhappyThought = emotest.GetSubtexture(16, 32, 16, 16);
            ColdThought = emotest.GetSubtexture(32, 32, 16, 16);

            BuildingSoundEffect = GetSoundEffect("building").CreateInstance();
            ClickSoundEffect = GetSoundEffect("click").CreateInstance();
            NotificationInfoSoundEffect = GetSoundEffect("notification_info").CreateInstance();
            NotificationDangerSoundEffect = GetSoundEffect("notification_danger").CreateInstance();
            NotificationWarningSoundEffect = GetSoundEffect("notification_warning").CreateInstance();
            NotificationAchievementSoundEffect = GetSoundEffect("notification_achievement").CreateInstance();
        }

        public static MyTexture GetTexture(string name)
        {
            if (!wasInitialized)
                throw new Exception("Resource Manager was not initialized!");

            try
            {
                return sprites[name];
            }
            catch(Exception e)
            {
                throw new Exception("Sprite with '" + name + "' name not found!: " + e.ToString());
            }
        }

        public static Song GetSong(string name)
        {
            if (!wasInitialized)
                throw new Exception("Resource Manager was not initialized!");

            try
            {
                return songs[name];
            }
            catch (Exception e)
            {
                throw new Exception("Song with '" + name + "' name not found!: " + e.ToString());
            }
        }

        public static SoundEffect GetSoundEffect(string name)
        {
            if (!wasInitialized)
                throw new Exception("Resource Manager was not initialized!");

            try
            {
                return soundEffects[name];
            }
            catch (Exception e)
            {
                throw new Exception("Sound effect with '" + name + "' name not found!: " + e.ToString());
            }
        }

        private static Dictionary<string, T> LoadContent<T>(ContentManager contentManager, string contentFolder)
        {
            DirectoryInfo dir = new DirectoryInfo(contentManager.RootDirectory + "/" + contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            Dictionary<string, T> result = new Dictionary<string, T>();
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                result[key] = contentManager.Load<T>(contentFolder + "/" + key);
            }
            return result;
        }

        private static Dictionary<string, MyTexture> LoadSprites(ContentManager contentManager, params string[] contentFolders)
        {
            Dictionary<string, MyTexture> result = new Dictionary<string, MyTexture>();

            for (int i = 0; i < contentFolders.Length; i++)
            {
                string contentFolder = contentFolders[i];

                DirectoryInfo dir = new DirectoryInfo(contentManager.RootDirectory + "/" + contentFolder);
                if (!dir.Exists)
                    throw new DirectoryNotFoundException();

                FileInfo[] files = dir.GetFiles("*.*");
                foreach (FileInfo file in files)
                {
                    if (file.Extension == ".xnb")
                    {
                        string key = Path.GetFileNameWithoutExtension(file.Name);
                        result[key] = new MyTexture(contentManager.Load<Texture2D>(contentFolder + "/" + key));
                    }
                }
            }

            
            return result;
        }

    }
}
