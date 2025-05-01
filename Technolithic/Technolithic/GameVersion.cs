using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    [JsonConverter(typeof(GameVersionJsonConverter))]
    public class GameVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }

        public GameVersion(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public static bool operator <(GameVersion a, GameVersion b)
        {
            if (a.Major != b.Major) 
            { 
                return a.Major < b.Major; 
            }
            
            if (a.Minor != b.Minor) 
            { 
                return a.Minor < b.Minor; 
            }
            
            return a.Patch < b.Patch;
        }

        public static bool operator >(GameVersion a, GameVersion b)
        {
            return b < a;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }
    }
}
