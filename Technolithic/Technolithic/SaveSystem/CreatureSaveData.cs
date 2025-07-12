using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class CreatureSaveData
    {

        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public Guid ChildId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
        public Color HairColor { get; set; }
        public Dictionary<int, bool> Ration { get; set; }
        public Dictionary<string, int> LaborTypePriorityPair { get; set; }

        public List<Tuple<int, int, float>> Tools { get; set; }

        public int ToolId { get; set; } = -1;
        public int ToolFactWeight { get; set; }
        public float ToolDurability { get; set; }

        public int OutfitId { get; set; } = -1;
        public int OutfitFactWeight { get; set; }
        public float OutfitDurability { get; set; }

        public int TopOutfitId { get; set; } = -1;
        public int TopOutfitFactWeight { get; set; }
        public float TopOutfitDurability { get; set; }

        public int BodyTextureId { get; set; }
        public int HairTextureId { get; set; }

        public List<Tuple<int, int, float>> InventoryItems { get; set; }

        public float Hunger { get; set; }
        public float Health { get; set; }
        public float Energy { get; set; }
        [Obsolete("Cannot be used, only to support old saves")]
        public float Age { get; set; }
        public float Temperature { get; set; }
        public float Happiness { get; set; }

        public bool Hunt { get; set; }
        public bool Domesticate { get; set; }
        public bool Slaughter { get; set; }
        public bool GatherProduct { get; set; }

        public bool WasAttacked { get; set; }
        public float ProductReadyPercent { get; set; }
        public CreatureType CreatureType { get; set; }
        [Obsolete("Cannot be used, only to support old saves")]
        public AgeState AgeState { get; set; }

        public int DaysUntilAging { get; set; }

        public string AnimalTemplateName { get; set; }

        public bool IsPregnant { get; set; }
        public int PregnancyProgressInDays { get; set; }

        public int HoursPassedFromLastFertilization { get; set; }
        public int NextFertilizationHoursSum { get; set; }
        public bool IsReadyToFertilization { get; set; }

        public Dictionary<StatusEffectId, float> StatusEffects { get; set; }
    }
}
