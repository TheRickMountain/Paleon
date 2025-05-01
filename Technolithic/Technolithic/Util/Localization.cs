using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Technolithic
{

    public static class Localization
    {

        public static string CurrentLanguageName { get; private set; }

        public static string CurrentLanguage { get; private set; }
        public static string StartLanguage { get; private set; }

        private static Dictionary<string, Dictionary<string, string>> languageKeyValuePair;

        public static void SetLanguage(string language)
        {
            if (CurrentLanguage == language)
                return;

            CurrentLanguage = language;

            CurrentLanguageName = GetLanguageName(language);

            GameSettings.Translation = language;
            GameSettings.Save();
        }

        public static IEnumerable<string> GetLanguages()
        {
            foreach(var kvp in languageKeyValuePair)
            {
                yield return kvp.Key;
            }
        }

        public static void Initialize()
        {
            languageKeyValuePair = new Dictionary<string, Dictionary<string, string>>();

            string localizationDirectory = Path.Combine(Engine.ContentDirectory, "Fonts");
            string[] localizationFiles = Directory.GetFiles(localizationDirectory, "localization_*.csv");

            foreach (string filePath in localizationFiles)
            {
                string fileName = Path.GetFileName(filePath);
                string languageCode = fileName.Substring(13, fileName.Length - 17).ToUpper();
                LoadLanguageFile(languageCode, filePath);
            }

            CurrentLanguage = GameSettings.Translation;
            if (!languageKeyValuePair.ContainsKey(CurrentLanguage))
            {
                CurrentLanguage = "EN";
                GameSettings.Translation = "EN";
                GameSettings.Save();
            }

            CurrentLanguageName = GetLanguageName(CurrentLanguage);
            StartLanguage = CurrentLanguage;
        }

        private static void LoadLanguageFile(string languageCode, string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.WriteLine($"Warning: Localization file {filePath} not found.");
                return;
            }

            Dictionary<string, string> languageDict = new Dictionary<string, string>();
            languageKeyValuePair[languageCode] = languageDict;

            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    if (fields.Length >= 2)
                    {
                        languageDict[fields[0]] = fields[1];
                    }
                }
            }

            Debug.WriteLine($"Loaded localization for language: {languageCode}");
        }

        public static string GetLanguageName(string language)
        {
            if(languageKeyValuePair.ContainsKey(language))
            {
                return languageKeyValuePair[language]["language_name"];
            }

            return null;
        }

        public static string GetLocalizedText(string key)
        {
            if (languageKeyValuePair[CurrentLanguage].ContainsKey(key) == false)
                return "???";

            return languageKeyValuePair[CurrentLanguage][key];
        }

        public static string GetLocalizedText(string key, object arg0)
        {
            if (languageKeyValuePair[CurrentLanguage].ContainsKey(key) == false)
                return "???";

            return string.Format(languageKeyValuePair[CurrentLanguage][key], arg0);
        }

        public static string GetLocalizedText(string key, object arg0, object arg1)
        {
            if (languageKeyValuePair[CurrentLanguage].ContainsKey(key) == false)
                return "???";

            return string.Format(languageKeyValuePair[CurrentLanguage][key], arg0, arg1);
        }

        public static string GetLocalizedText(string key, object arg0, object arg1, object arg2)
        {
            if (languageKeyValuePair[CurrentLanguage].ContainsKey(key) == false)
                return "???";

            return string.Format(languageKeyValuePair[CurrentLanguage][key], arg0, arg1, arg2);
        }

    }
}
