using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Technolithic
{
    public class StorageSaveManager : SaveManager
    {
        public StorageSaveManager(string folderName, string fileName)
            : base(folderName, fileName)
        { }

        public override void Load()
        {
            if (!Directory.Exists(Engine.GetGameDirectory()))
                return;

            if (!Directory.Exists(Path.Combine(Engine.GetGameDirectory(), folderName)))
                return;

            string saveFilePath = Path.Combine(Engine.GetGameDirectory(), folderName, fileName);
            if (!File.Exists(saveFilePath))
                return;

            string saveDataText = File.ReadAllText(saveFilePath);
            Data = JsonConvert.DeserializeObject<SaveData>(saveDataText);

            string infoFilePath = Path.Combine(Engine.GetGameDirectory(), folderName, fileName + "_info");
            if (!File.Exists(infoFilePath))
                return;

            string infoDataText = File.ReadAllText(infoFilePath);
            Info = JsonConvert.DeserializeObject<SaveInfo>(infoDataText);
        }

        public override void Save()
        {
            if (!Directory.Exists(Engine.GetGameDirectory()))
                Directory.CreateDirectory(Engine.GetGameDirectory());

            if (!Directory.Exists(Path.Combine(Engine.GetGameDirectory(), folderName)))
                Directory.CreateDirectory(Path.Combine(Engine.GetGameDirectory(), folderName));

            string saveFilePath = Path.Combine(Engine.GetGameDirectory(), folderName, fileName);

            string serializedSaveData = JsonConvert.SerializeObject(Data, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            File.WriteAllText(saveFilePath, serializedSaveData);

            string saveInfoPath = Path.Combine(Engine.GetGameDirectory(), folderName, fileName + "_info");

            string serializedInfoData = JsonConvert.SerializeObject(Info, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            File.WriteAllText(saveInfoPath, serializedInfoData);
        }
    }
}