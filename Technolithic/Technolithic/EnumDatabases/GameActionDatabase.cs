namespace Technolithic
{
    public enum MyAction
    {
        Build, Uproot, Chop, AutoHarvest, Hunt, Slaughter,
        Cancel, DestructConstruction, DestructSurface, DestructWall,
        CopySettings, Mine, GatherStone, GatherWood,
        BuildIrrigationCanal, DestructIrrigationCanal,
        Plow, DestructFarmPlot, None
    }


    public static class GameActionDatabase
    {
        public static void Initialize()
        {
            EnumDatabase<MyAction>.Register(MyAction.GatherStone, ResourceManager.GatherStoneIcon, "gather_stone");
            EnumDatabase<MyAction>.Register(MyAction.GatherWood, ResourceManager.GatherWoodIcon, "gather_wood");
            EnumDatabase<MyAction>.Register(MyAction.Mine, ResourceManager.MineIcon, "mine");
            EnumDatabase<MyAction>.Register(MyAction.Chop, ResourceManager.ChopIcon, "chop");
            EnumDatabase<MyAction>.Register(MyAction.AutoHarvest, ResourceManager.AutoHarvestIcon, "auto_harvest");
            EnumDatabase<MyAction>.Register(MyAction.Uproot, ResourceManager.UprootIcon, "uproot");
            EnumDatabase<MyAction>.Register(MyAction.Hunt, ResourceManager.HuntIcon, "hunt");
            EnumDatabase<MyAction>.Register(MyAction.Slaughter, ResourceManager.SlaughterIcon, "slaughter");
            EnumDatabase<MyAction>.Register(MyAction.BuildIrrigationCanal, TextureBank.UITexture.GetSubtexture(112, 160, 16, 16), "build_irrigation_canal");
            EnumDatabase<MyAction>.Register(MyAction.DestructIrrigationCanal, TextureBank.UITexture.GetSubtexture(128, 160, 16, 16), "destruct_irrigation_canal");
            EnumDatabase<MyAction>.Register(MyAction.Cancel, TextureBank.UITexture.GetSubtexture(48, 0, 16, 16), "cancel");
            EnumDatabase<MyAction>.Register(MyAction.DestructConstruction, TextureBank.UITexture.GetSubtexture(224, 160, 16, 16), "destruct_construction");
            EnumDatabase<MyAction>.Register(MyAction.DestructSurface, ResourceManager.DestructSurfaceIcon, "destruct_surface");
            EnumDatabase<MyAction>.Register(MyAction.DestructWall, ResourceManager.DestructWallIcon, "destruct_wall");
            EnumDatabase<MyAction>.Register(MyAction.Plow, ResourceManager.PlowIcon, "plow");
            EnumDatabase<MyAction>.Register(MyAction.DestructFarmPlot, AssetManager.GetTexture("Sprites", "ui").GetSubtexture(192, 32, 16, 16), "destruct_farm_plot");
        }
    }
}