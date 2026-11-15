namespace Technolithic
{
    public static class BuildingPipette
    {
        public static bool CanPick(BuildingTemplate template, ProgressTree progressTree)
        {
            return template.Cloneable && progressTree.IsBuildingUnlocked(template);
        }

        public static void Pick(BuildingTemplate template, Direction direction)
        {
            GameplayScene.WorldManager.SetMyAction(MyAction.Build, null);
            GameplayScene.WorldManager.SetBuilding(template, direction);
            GameplayScene.UIRootNodeScript.CloseEntityPanel();
        }
    }
}
