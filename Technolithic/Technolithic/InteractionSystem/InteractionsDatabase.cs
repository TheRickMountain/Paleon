using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace Technolithic
{
    public enum InteractionType
    {
        Chop,
        EmptyFishTrap,
        Destruct,
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
        Uproot
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
                InteractionType.Destruct,
                Localization.GetLocalizedText("destruct"),
                ResourceManager.DestructIcon,
                Color.Red,
                InteractionIconDisplayState.OnMarked));

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
                InteractionIconDisplayState.OnMarked));

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
