using System;
using System.Collections.Generic;


namespace Technolithic
{
    public struct EnumData
    {
        public string DisplayText;
        public MyTexture Icon;
    }


    public static class EnumDatabase<TEnum> where TEnum : Enum
    {
        private static readonly Dictionary<TEnum, EnumData> dict = new();


        public static void Register(TEnum key, MyTexture icon, string displayText)
        {
            var data = new EnumData
            {
                DisplayText = Localization.GetLocalizedText(displayText),
                Icon = icon
            };


            dict[key] = data;
        }


        public static EnumData GetData(TEnum key)
        {
            if (dict.TryGetValue(key, out var data))
                return data;


            throw new KeyNotFoundException($"No data registered for {key}");
        }


        public static void Clear()
        {
            dict.Clear();
        }
    }
}
