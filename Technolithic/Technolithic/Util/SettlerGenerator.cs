using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public struct SettlerInfo
    {
        public string Name { get; private set; }
        public Color HairColor { get; private set; }
        public int HairTextureId { get; private set; }
        public int BodyTextureId { get; private set; }

        public SettlerInfo(string name, Color hairColor, int bodyTextureId, int hairTextureId)
        {
            Name = name;
            HairColor = hairColor;
            BodyTextureId = bodyTextureId;
            HairTextureId = hairTextureId;
        }
    }

    public static class SettlerGenerator
    {

        private static Color eumelanin = new Color(35, 18, 11);
        private static Color pheomelanin = new Color(218, 104, 15);

        private static string[] names = new string[]
        {
            "Ghilk",
            "Tolk",
            "Bhat",
            "Grit",
            "Dhod",
            "Tovda",
            "Vadvag",
            "Gegrarc",
            "Duggir",
            "Khaazdub",
            "Chono",
            "Ghizel",
            "Defni",
            "Cuyge",
            "Ghoclen",
            "Cilol",
            "Dero",
            "Stuse",
            "Jaree",
            "Bhartu",
            "Kami",
            "Weena"
        };

        private static Color GetHairColor(float eumelaninPercent, float pheomelaninPercent)
        {
            Color eumelaninColor = Color.Lerp(Color.White, eumelanin, eumelaninPercent);
            Color pheomelaninColor = Color.Lerp(Color.White, pheomelanin, pheomelaninPercent);

            return new Color(
                Blend(eumelaninColor.R, pheomelaninColor.R), 
                Blend(eumelaninColor.G, pheomelaninColor.G), 
                Blend(eumelaninColor.B, pheomelaninColor.B));
        }

        private static string GetRandomName()
        {
            return names[MyRandom.Range(names.Length)];
        }

        private static int Blend(int ecu, int pcu)
        {
            float ecuPercent = ecu / 255f;
            return (int)(ecuPercent * pcu);
        }

        public static SettlerInfo GenerateSettler()
        {
            string name = GetRandomName();
            Color hairColor = GetHairColor(MyRandom.NextFloat(), MyRandom.NextFloat());

            return new SettlerInfo(name, hairColor, MyRandom.Range(5), MyRandom.Range(6));
        }

    }
}
