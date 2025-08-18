using System;

namespace Technolithic
{
    public class WorldSettings
    {
        public int Seed { get; init; }
        public string Name { get; init; }
        public Season StartSeason { get; init; }
        public float GroundHeight { get; set; }
        public float[,] HeightMap { get; set; }
    }
}
