using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public static class GameSettings
    {

        public static string Translation { get; set; } = "EN";
        public static int SoundVolume { get; set; } = 50;
        public static int MusicVolume { get; set; } = 50;
        public static bool PlayMainTheme { get; set; } = true;
        public static bool EdgeScrollingCamera { get; set; } = true;

        public static void Load()
        {
            if (!Directory.Exists(Engine.GetGameDirectory()))
                return;

            string saveFilePath = Path.Combine(Engine.GetGameDirectory(), "settings.json");
            if (!File.Exists(saveFilePath))
                return;

            string saveDataText = File.ReadAllText(saveFilePath);
            JObject settingsJson = JObject.Parse(saveDataText);

            if (settingsJson["translation"].IsNullOrEmpty() == false)
            {
                Translation = settingsJson["translation"].Value<string>();
            }

            if (settingsJson["sound_volume"].IsNullOrEmpty() == false)
            {
                SoundVolume = settingsJson["sound_volume"].Value<int>();
            }

            if (settingsJson["music_volume"].IsNullOrEmpty() == false)
            {
                MusicVolume = settingsJson["music_volume"].Value<int>();
            }

            if (settingsJson["play_main_theme"].IsNullOrEmpty() == false)
            {
                PlayMainTheme = settingsJson["play_main_theme"].Value<bool>();
            }

            if(settingsJson["edge_scrolling_camera"].IsNullOrEmpty() == false)
            {
                EdgeScrollingCamera = settingsJson["edge_scrolling_camera"].Value<bool>();
            }
        }

        public static void Save()
        {
            if (!Directory.Exists(Engine.GetGameDirectory()))
                Directory.CreateDirectory(Engine.GetGameDirectory());

            string saveFilePath = Path.Combine(Engine.GetGameDirectory(), "settings.json");

            JObject settingsJson = new JObject
            {
                new JProperty("translation", Translation),
                new JProperty("sound_volume", SoundVolume),
                new JProperty("music_volume", MusicVolume),
                new JProperty("play_main_theme", PlayMainTheme),
                new JProperty("edge_scrolling_camera", EdgeScrollingCamera)
            };

            string serializedSaveData = JsonConvert.SerializeObject(settingsJson, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            File.WriteAllText(saveFilePath, serializedSaveData);
        }

    }
}
