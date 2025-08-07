namespace Technolithic
{
    public class AnimalCorpseSaveData : InteractableSaveData
    {
        public string AnimalKey { get; set; }
        public float SpoilageProgress { get; set; }
        public int TileX { get; set; }
        public int TileY { get; set; }
    }
}
