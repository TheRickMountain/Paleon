using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace Technolithic
{
    public enum InteractionType
    {
        Chop,
        EmptyFishTrap,
        Construct,
        Destruct,
        DestructWall,
        DestructSurface,
        DestructIrrigationCanal,
        Remove,
        CollectWildHoney,
        CollectHoney,
        CatchFish,
        GatherStone,
        GatherWood,
        Mine,
        AutoCleanPen,
        ProduceEnergy,
        Craft,
        Slaughter,
        GatherAnimalProduct,
        Domesticate,
        Hunt,
        AutoHarvest,
        Uproot,
        Plant,
        Plow,
        Irrigate,
        Fertilize
    }

    public class InteractionsDatabase
    {
        private List<InteractionData> allInteractions = new();
        private Dictionary<InteractionType, InteractionData> interactionTypeDataDict = new();

        public IReadOnlyList<InteractionData> AllInteractions => allInteractions;

        public InteractionsDatabase()
        {
            AddInteractionData(new InteractionData(
                InteractionType.Chop,
                Localization.GetLocalizedText("chop"),
                ResourceManager.ChopIcon,
                Color.Orange,
                InteractionIconDisplayState.OnMarked));

            AddInteractionData(new InteractionData(
                InteractionType.EmptyFishTrap,
                Localization.GetLocalizedText("automatically_empty_the_trap"),
                ResourceManager.EmptyFishTrapIcon,
                Color.Orange,
                InteractionIconDisplayState.OnMarked));

            AddInteractionData(new InteractionData(
                InteractionType.Construct,
                Localization.GetLocalizedText("construct"),
                ResourceManager.ConstructIcon,
                Color.Orange,
                InteractionIconDisplayState.Never));

            AddInteractionData(new InteractionData(
                InteractionType.Destruct,
                Localization.GetLocalizedText("destruct"),
                ResourceManager.DestructIcon,
                Color.Red,
                InteractionIconDisplayState.OnMarked));

            AddInteractionData(new InteractionData(
                InteractionType.DestructWall,
                Localization.GetLocalizedText("destruct_wall"),
                ResourceManager.DestructWallIcon,
                Color.Red,
                InteractionIconDisplayState.Never));

            AddInteractionData(new InteractionData(
                InteractionType.DestructSurface,
                Localization.GetLocalizedText("destruct_surface"),
                ResourceManager.DestructSurfaceIcon,
                Color.Red,
                InteractionIconDisplayState.Never));

            AddInteractionData(new InteractionData(
                InteractionType.DestructIrrigationCanal,
                Localization.GetLocalizedText("destruct_irrigation_canal"),
                RenderManager.Pixel, // TODO: icon
                Color.Red,
                InteractionIconDisplayState.Never));

            AddInteractionData(new InteractionData(
                InteractionType.CollectWildHoney,
                Localization.GetLocalizedText("auto_collect_wild_honey"),
                ResourceManager.CollectHoneyIcon,
                Color.Orange,
                InteractionIconDisplayState.OnMarked));

            AddInteractionData(new InteractionData(
                InteractionType.CollectHoney,
                Localization.GetLocalizedText("auto_collect_honey"),
                ResourceManager.CollectHoneyIcon,
                Color.Orange,
                InteractionIconDisplayState.OnMarked));

            AddInteractionData(new InteractionData(
                InteractionType.CatchFish,
                Localization.GetLocalizedText("catch_fish"),
                ResourceManager.CatchFishIcon,
                Color.Orange, 
                InteractionIconDisplayState.OnUnmarked));

            AddInteractionData(new InteractionData(
                InteractionType.GatherWood,
                Localization.GetLocalizedText("gather_wood"),
                ResourceManager.GatherWoodIcon,
                Color.Orange,
                InteractionIconDisplayState.OnMarked));

            AddInteractionData(new InteractionData(
                InteractionType.GatherStone,
                Localization.GetLocalizedText("gather_stone"),
                ResourceManager.GatherStoneIcon,
                Color.Orange,
                InteractionIconDisplayState.OnMarked));

            AddInteractionData(new InteractionData(
                InteractionType.Mine,
                Localization.GetLocalizedText("mine"),
                ResourceManager.MineIcon,
                Color.Orange,
                InteractionIconDisplayState.OnMarked));

            AddInteractionData(new InteractionData(
                InteractionType.AutoCleanPen,
                Localization.GetLocalizedText("auto_clean_pen"),
                ResourceManager.CleanUpIcon,
                Color.Orange,
                InteractionIconDisplayState.OnUnmarked));

            AddInteractionData(new InteractionData(
                InteractionType.ProduceEnergy,
                Localization.GetLocalizedText("produce_energy"),
                ResourceManager.ProduceEnergyIcon,
                Color.Orange,
                InteractionIconDisplayState.OnUnmarked));

            AddInteractionData(new InteractionData(
                InteractionType.Craft,
                Localization.GetLocalizedText("craft_an_item"),
                ResourceManager.CraftIcon,
                Color.Orange,
                InteractionIconDisplayState.OnUnmarked));

            AddInteractionData(new InteractionData(
                InteractionType.Slaughter,
                Localization.GetLocalizedText("slaughter"),
                ResourceManager.SlaughterIcon,
                Color.Orange,
                InteractionIconDisplayState.OnMarked));

            AddInteractionData(new InteractionData(
                InteractionType.GatherAnimalProduct,
                "gather_animal_product", // TODO: localization
                RenderManager.Pixel, // TODO: icon
                Color.Orange,
                InteractionIconDisplayState.OnMarked));

            AddInteractionData(new InteractionData(
                InteractionType.Domesticate,
                Localization.GetLocalizedText("domesticate"),
                ResourceManager.DomesticateIcon,
                Color.Orange,
                InteractionIconDisplayState.OnMarked));

            AddInteractionData(new InteractionData(
                InteractionType.Hunt,
                Localization.GetLocalizedText("hunt"),
                ResourceManager.HuntIcon,
                Color.Orange,
                InteractionIconDisplayState.OnMarked));

            AddInteractionData(new InteractionData(
                InteractionType.AutoHarvest,
                Localization.GetLocalizedText("auto_harvest"),
                ResourceManager.AutoHarvestIcon,
                Color.Orange,
                InteractionIconDisplayState.OnMarked));

            AddInteractionData(new InteractionData(
                InteractionType.Uproot,
                Localization.GetLocalizedText("uproot"),
                ResourceManager.UprootIcon,
                Color.Red,
                InteractionIconDisplayState.OnMarked));

            AddInteractionData(new InteractionData(
                InteractionType.Plant,
                Localization.GetLocalizedText("plant_action"),
                ResourceManager.PlantIcon,
                Color.Orange,
                InteractionIconDisplayState.Never));

            AddInteractionData(new InteractionData(
                InteractionType.Plow,
                Localization.GetLocalizedText("plow"),
                RenderManager.Pixel, // Draw icon
                Color.Orange,
                InteractionIconDisplayState.Never));

            AddInteractionData(new InteractionData(
                InteractionType.Irrigate,
                Localization.GetLocalizedText("irrigate"),
                ResourceManager.IrrigationIcon,
                Color.Orange,
                InteractionIconDisplayState.Never));

            AddInteractionData(new InteractionData(
                InteractionType.Fertilize,
                Localization.GetLocalizedText("fertilize"),
                ResourceManager.FertilizeIcon,
                Color.Orange,
                InteractionIconDisplayState.Never));

            AddInteractionData(new InteractionData(
                InteractionType.Remove,
                Localization.GetLocalizedText("remove"),
                ResourceManager.CancelIcon,
                Color.Orange,
                InteractionIconDisplayState.Never));
        }

        private void AddInteractionData(InteractionData interactionData)
        {
            Debug.Assert(interactionData != null);

            allInteractions.Add(interactionData);

            interactionTypeDataDict.Add(interactionData.InteractionType, interactionData);
        }

        public InteractionData GetInteractionData(InteractionType interactionType)
        {
            return interactionTypeDataDict[interactionType];
        }
    }
}
