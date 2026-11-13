using Microsoft.Xna.Framework;
using System;

namespace Technolithic
{
    public class ConstructionSiteCmp : Component, ISupplyTarget
    {
        public BuildingPlacement Placement { get; }
        public BuildingTemplate BuildingTemplate => Placement.Template;
        public Direction Direction => Placement.Direction;

        public Inventory Inventory { get; }
        public AnimatedSprite Sprite { get; }

        public float ConstructionProgress { get; set; }

        public bool IsDestroyed { get; private set; }

        public Action<ConstructionSiteCmp> OnCanceledCallback { get; set; }
        public Action<ConstructionSiteCmp, BuildingCmp> OnCompletedCallback { get; set; }
        public Action<ISupplyTarget> OnRemovedCallback { get; set; }

        private BuildLabor buildLabor;

        public ConstructionSiteCmp(BuildingTemplate buildingTemplate, Direction direction) : base(true, true)
        {
            Placement = new BuildingPlacement(buildingTemplate, direction);

            Inventory = new Inventory(this);
            Inventory.IsStorage = false;

            Sprite = BuildingSprite.Create(buildingTemplate, Placement.Direction);
        }

        public void Place(Tile[,] tiles)
        {
            Entity.X = tiles[0, 0].X * Engine.TILE_SIZE;
            Entity.Y = tiles[0, 0].Y * Engine.TILE_SIZE;

            Placement.Place(tiles, Entity);

            if (Sprite != null)
            {
                BuildingSprite.AttachTo(Sprite, Entity, Placement);
                Sprite.Color = Color.White * 0.5f;
            }

            Placement.UseNeighbourTilesAsTargets();

            foreach (var kvp in BuildingTemplate.BuildingRecipe)
            {
                Inventory.AddRequiredWeight(kvp.Key, kvp.Value);
            }

            if (BuildingTemplate.BuildingRecipe.Count == 0)
            {
                AddBuildLabor();
            }

            Inventory.OnItemAddedCallback += OnItemAdded;
        }

        public override void Update()
        {
            Sprite?.Update();
        }

        public override void Render()
        {
            Sprite?.Render();
        }

        public BuildingCmp Complete()
        {
            IsDestroyed = true;

            Inventory.OnItemAddedCallback -= OnItemAdded;
            Inventory.ClearRequiredWeight();
            Inventory.ClearCargo();

            BuildingCmp building = BuildingTemplate.TileChange != null ? ApplyTileChange() : ReplaceWithBuilding();

            OnCompletedCallback?.Invoke(this, building);
            OnRemovedCallback?.Invoke(this);

            return building;
        }

        private BuildingCmp ApplyTileChange()
        {
            foreach (TileInfo tileInfo in Placement.TilesInfosList)
            {
                BuildingTemplate.TileChange.Apply(tileInfo.Tile);
            }

            Placement.Release();

            RemoveFromWorld();

            return null;
        }

        private BuildingCmp ReplaceWithBuilding()
        {
            Entity entity = Entity;
            entity.Remove(this);

            BuildingCmp building = BuildingTemplate.CreateBuildingCmp(Direction);
            entity.Add(building);

            building.Place(Placement.Tiles, 0);
            building.Begin();
            building.CompleteBuilding();

            return building;
        }

        public void Cancel()
        {
            IsDestroyed = true;

            if (buildLabor != null)
            {
                buildLabor.CancelAndClearAllTasksAndComplete();
                buildLabor = null;
            }

            Placement.Release();

            Inventory.ThrowCargo(Placement.GetCenterTile());

            RemoveFromWorld();

            Inventory.ClearRequiredWeight();

            OnCanceledCallback?.Invoke(this);
            OnRemovedCallback?.Invoke(this);

            Inventory.OnItemAddedCallback -= OnItemAdded;
        }

        public Tile GetApproachableTile(CreatureCmp creature) => Placement.GetReachableTile(creature);

        public Room GetRoom() => Placement.GetRoom();

        public string GetInformation()
        {
            string info = $"{Localization.GetLocalizedText("ingredients")}:\n";

            foreach (var kvp in BuildingTemplate.BuildingRecipe)
            {
                Item item = kvp.Key;

                int requiredWeight = kvp.Value;

                int factWeight = Inventory.GetInventoryFactWeight(item);

                info += $"- {item.Name}: {factWeight} / {requiredWeight}\n";
            }

            return info + "\n";
        }

        public BuildingSaveData GetSaveData()
        {
            BuildingSaveData buildingSaveData = new BuildingSaveData();
            buildingSaveData.BuildingTemplateName = BuildingTemplate.Json;
            buildingSaveData.BuildingProgress = ConstructionProgress;
            buildingSaveData.Tiles = Placement.GetTilesAsPoints();
            buildingSaveData.IsBuilt = false;
            buildingSaveData.Direction = Direction;
            buildingSaveData.InventoryItems = Inventory.GetItemsSaveData();

            return buildingSaveData;
        }

        private void OnItemAdded(Inventory senderInventory, Item item, int weight)
        {
            if (Inventory.InventoryRequired.ContainsKey(item))
            {
                Inventory.AddRequiredWeight(item, -weight);
            }

            if (IsSupplyCompleted())
            {
                AddBuildLabor();
            }
        }

        private bool IsSupplyCompleted()
        {
            foreach (var recipe in BuildingTemplate.BuildingRecipe)
            {
                int factWeight = Inventory.GetInventoryFactWeight(recipe.Key);

                if (factWeight != recipe.Value)
                    return false;
            }

            return true;
        }

        private void RemoveFromWorld()
        {
            Entity.RemoveSelf();
        }

        private void AddBuildLabor()
        {
            buildLabor = new BuildLabor(this, BuildingTemplate.BuildingLaborType);
            GameplayScene.WorldManager.LaborManager.Add(buildLabor);
        }
    }
}
