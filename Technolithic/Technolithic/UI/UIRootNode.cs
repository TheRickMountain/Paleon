using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Technolithic.UI.Prefabs.BuildingRecipe;
using System.Collections.Generic;
using System;

namespace Technolithic
{


    public class UIRootNode : MNode
    {
        private ActionPanelUI actionPanel;
        private BuildingPanelUI buildingPanel;
        private LevelUI levelUI;
        private BigButton laborPriorityButton;
        private BigButton rationButton;
        private BigButton resourcesLimitButton;
        private BigButton resourcesButton;
        private BigButton achievementsButton;
        private ResourcesCountUI resourcesCountUI;
        private BuildingsListUI buildingListUI;

        public UIRootNode(Scene scene) : base(scene)
        {
            actionPanel = new ActionPanelUI(scene);
            actionPanel.X = Engine.Width - actionPanel.Width - 5;
            actionPanel.Name = "ActionPanel";
            AddChildNode(actionPanel);

            buildingPanel = new BuildingPanelUI(scene);
            buildingPanel.X = 5;
            buildingPanel.Y = Engine.Height - buildingPanel.Height - 5;
            buildingPanel.Name = "BuildingPanel";
            AddChildNode(buildingPanel);

            WorldStateUI worldStateUI = new WorldStateUI(scene);
            worldStateUI.X = 5;
            worldStateUI.Y = 5;
            worldStateUI.Name = "WorldState";
            AddChildNode(worldStateUI);

            TimeControllerUI timeControllerUI = new TimeControllerUI(scene);
            timeControllerUI.X = 5;
            timeControllerUI.Name = "TimeController";
            timeControllerUI.Y = worldStateUI.Y + worldStateUI.Height + 5;
            AddChildNode(timeControllerUI);

            IndicatorPanelUI indicatorPanelUI = new IndicatorPanelUI(scene);
            indicatorPanelUI.X = timeControllerUI.LocalX + timeControllerUI.Width + 5;
            indicatorPanelUI.Y = worldStateUI.Y + worldStateUI.Height + 5;
            indicatorPanelUI.Name = "IndicatorPanel";
            AddChildNode(indicatorPanelUI);

            levelUI = new LevelUI(scene);
            levelUI.X = Engine.Width - levelUI.Width - 5;
            levelUI.Y = 5;
            levelUI.Name = "Level";
            AddChildNode(levelUI);

            // *** Labor priority button *** //

            laborPriorityButton = new BigButton(scene, TextureBank.UITexture.GetSubtexture(240, 16, 16, 16), false);
            laborPriorityButton.X = Engine.Width - laborPriorityButton.Width - 5;
            laborPriorityButton.Y = levelUI.Y + levelUI.Height + 5;
            laborPriorityButton.Name = "LaborPriorityButton";
            AddChildNode(laborPriorityButton);

            // *** Ration button ***

            rationButton = new BigButton(scene, ResourceManager.FoodIcon, false);
            rationButton.X = laborPriorityButton.X - laborPriorityButton.Width - 5;
            rationButton.Y = levelUI.Y + levelUI.Height + 5;
            rationButton.Name = "RationButton";
            AddChildNode(rationButton);

            // *** Resources limit button ***

            resourcesLimitButton = new BigButton(scene, ResourceManager.ResourcesLimitIcon, false);
            resourcesLimitButton.X = rationButton.X - rationButton.Width - 5;
            resourcesLimitButton.Y = levelUI.Y + levelUI.Height + 5;
            resourcesLimitButton.Name = "ResourcesLimitButton";
            AddChildNode(resourcesLimitButton);

            // *** Resources button ***

            resourcesButton = new BigButton(scene, ResourceManager.ResourcesIcon, false);
            resourcesButton.X = resourcesLimitButton.X - resourcesLimitButton.Width - 5;
            resourcesButton.Y = levelUI.Y + levelUI.Height + 5;
            resourcesButton.Name = "ResourcesButton";
            AddChildNode(resourcesButton);

            // *** Achievements button ***

            achievementsButton = new BigButton(scene, TextureBank.UITexture.GetSubtexture(144, 240, 16, 16), false);
            achievementsButton.X = resourcesButton.X - resourcesButton.Width - 5;
            achievementsButton.Y = levelUI.Y + levelUI.Height + 5;
            achievementsButton.Name = "AchievementsButton";
            AddChildNode(achievementsButton);

            // *** Notifications ***
            NotificationsUI notificationsUI = new NotificationsUI(scene);
            notificationsUI.Name = "Notifications";
            notificationsUI.X = 0;
            notificationsUI.Y = timeControllerUI.Y + timeControllerUI.Height + 5;
            AddChildNode(notificationsUI);

            TechTreeUI techTree = new TechTreeUI(scene);
            techTree.Active = false;
            techTree.Name = "TechTree";
            AddChildNode(techTree);

            LaborPriorityUI laborPriority = new LaborPriorityUI(scene);
            laborPriority.Active = false;
            laborPriority.Name = "LaborPriority";
            AddChildNode(laborPriority);

            CommandLineUI commandLineUI = new CommandLineUI(scene);
            commandLineUI.Active = false;
            commandLineUI.Name = "CommandLine";
            AddChildNode(commandLineUI);

            RationUI rationUI = new RationUI(scene);
            rationUI.Active = false;
            rationUI.Name = "Ration";
            AddChildNode(rationUI);

            ResourcesLimitUI resourcesLimitUI = new ResourcesLimitUI(scene);
            resourcesLimitUI.Active = false;
            resourcesLimitUI.Name = "ResourcesLimit";
            AddChildNode(resourcesLimitUI);

            ResourcesUI resourcesUI = new ResourcesUI(scene);
            resourcesUI.Active = false;
            resourcesUI.Name = "Resources";
            AddChildNode(resourcesUI);

            AchievementsUI achievementsUI = new AchievementsUI(scene);
            achievementsUI.Active = false;
            achievementsUI.Name = "Achievements";
            AddChildNode(achievementsUI);

            NomadsUI nomadsUI = new NomadsUI(scene);
            nomadsUI.Active = false;
            nomadsUI.Name = "Nomads";
            AddChildNode(nomadsUI);

            TradingUI tradingUI = new TradingUI(scene);
            tradingUI.Active = false;
            tradingUI.Name = "Trading";
            AddChildNode(tradingUI);

            resourcesCountUI = new ResourcesCountUI(scene);
            resourcesCountUI.Active = true;
            resourcesCountUI.Name = "ResourcesCount";
            resourcesCountUI.X = Engine.Width - resourcesCountUI.Width;
            resourcesCountUI.Y = rationButton.Y + rationButton.Height + 5;
            AddChildNode(resourcesCountUI);

            buildingListUI = new BuildingsListUI(scene);
            buildingListUI.Active = false;
            buildingListUI.X = 5;
            buildingListUI.Y = Engine.Height - buildingListUI.Height - 10 - 48;
            buildingListUI.Name = "BuildingList";
            AddChildNode(buildingListUI);
            
            CreatureUI creatureUI = new CreatureUI(scene);
            creatureUI.Active = false;
            creatureUI.Name = "Creature";
            AddChildNode(creatureUI);

            StatusEffectsUI statusEffectsUI = new StatusEffectsUI(scene);
            statusEffectsUI.Active = false;
            statusEffectsUI.Name = "StatusEffects";
            AddChildNode(statusEffectsUI);

            AssignPetUI attachPetUI = new AssignPetUI(scene);
            attachPetUI.Active = false;
            attachPetUI.Name = "AttachPet";
            AddChildNode(attachPetUI);

            AssignHutUI assignHutUI = new AssignHutUI(scene);
            assignHutUI.Active = false;
            assignHutUI.Name = "AssignHut";
            AddChildNode(assignHutUI);

            BuildingUI buildingUI = new BuildingUI(scene);
            buildingUI.Active = false;
            buildingUI.Name = "Building";
            AddChildNode(buildingUI);

            HutUI hutUI = new HutUI(scene);
            hutUI.Active = false;
            hutUI.Name = "Hut";
            AddChildNode(hutUI);

            GateUI gateUI = new GateUI(scene);
            gateUI.Active = false;
            gateUI.Name = "Gate";
            AddChildNode(gateUI);

            CrafterUI crafterUI = new CrafterUI(scene);
            crafterUI.Active = false;
            crafterUI.Name = "Crafter";
            AddChildNode(crafterUI);

            StorageUI storageUI = new StorageUI(scene);
            storageUI.Name = "Storage";
            storageUI.Active = false;
            AddChildNode(storageUI);

            AnimalPenUI animalPenUI = new AnimalPenUI(scene);
            animalPenUI.Name = "AnimalPen";
            animalPenUI.Active = false;
            AddChildNode(animalPenUI);

            ItemsStackUI itemsStackUI = new ItemsStackUI(scene);
            itemsStackUI.Active = false;
            itemsStackUI.Name = "ItemsStack";
            AddChildNode(itemsStackUI);

            BuildRecipeUI buildingRecipeUI = new BuildRecipeUI(scene);
            buildingRecipeUI.Active = false;
            buildingRecipeUI.Name = "BuildingRecipe";
            AddChildNode(buildingRecipeUI);

            PriorityUI priorityUI= new PriorityUI(scene);
            priorityUI.Active = false;
            priorityUI.Name = "Priority";
            AddChildNode(priorityUI);

            AddChildNode(new GameMenuUI(scene) { Active = false, Name = "GameMenu" });

            AddComponent(new UIRootNodeScript());

            Engine.Instance.Window.ClientSizeChanged += OnClientSizeChanged;
            Scene.SceneEnded += OnSceneEnded;
        }

        private void OnSceneEnded(Scene scene)
        {
            Engine.Instance.Window.ClientSizeChanged -= OnClientSizeChanged;
            Scene.SceneEnded -= OnSceneEnded;
        }

        private void OnClientSizeChanged(object sender, EventArgs e)
        {
            actionPanel.GetComponent<ActionPanelScript>().ArrangeButtons();

            buildingPanel.X = 5;
            buildingPanel.Y = Engine.Height - buildingPanel.Height - 5;

            levelUI.X = Engine.Width - levelUI.Width - 5;
            levelUI.Y = 5;

            laborPriorityButton.X = Engine.Width - laborPriorityButton.Width - 5;
            laborPriorityButton.Y = levelUI.Y + levelUI.Height + 5;

            rationButton.X = laborPriorityButton.X - laborPriorityButton.Width - 5;
            rationButton.Y = levelUI.Y + levelUI.Height + 5;

            resourcesLimitButton.X = rationButton.X - rationButton.Width - 5;
            resourcesLimitButton.Y = levelUI.Y + levelUI.Height + 5;

            resourcesButton.X = resourcesLimitButton.X - resourcesLimitButton.Width - 5;
            resourcesButton.Y = levelUI.Y + levelUI.Height + 5;

            achievementsButton.X = resourcesButton.X - resourcesButton.Width - 5;
            achievementsButton.Y = levelUI.Y + levelUI.Height + 5;

            resourcesCountUI.X = Engine.Width - resourcesCountUI.Width;
            resourcesCountUI.Y = rationButton.Y + rationButton.Height + 5;

            buildingListUI.X = 5;
            buildingListUI.Y = Engine.Height - buildingListUI.Height - 10 - 48;
        }

    }
}
