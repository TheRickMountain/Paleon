using System.Collections.Generic;

namespace Technolithic
{
    public class TileItemTracker
    {
        private readonly WorldManager worldManager;
        private readonly TotalResourcesChart totalResourcesChart;

        public TileItemTracker(TileItemStore store, WorldManager worldManager, TotalResourcesChart totalResourcesChart)
        {
            this.worldManager = worldManager;
            this.totalResourcesChart = totalResourcesChart;

            store.ItemAdded += OnItemAdded;
            store.ItemRemoved += OnItemRemoved;
            store.ReservedWeightChanged += OnReservedWeightChanged;
        }

        private void OnItemAdded(Inventory pile, Item item, int weight)
        {
            totalResourcesChart.AddItem(item, weight);

            worldManager.OpenItem(item);

            if (worldManager.ItemsDecayer.Inventories.Contains(pile) == false)
                worldManager.ItemsDecayer.Inventories.Add(pile);

            if (pile.Tile.Room == null)
                return;

            if (weight > 0)
                AddToRoomLookup(pile, item, pile.Tile.Room.Id);
        }

        private void OnItemRemoved(Inventory pile, Item item, int weight)
        {
            totalResourcesChart.AddItem(item, -weight);

            if (pile.TotalItemsCount <= 0)
                worldManager.ItemsDecayer.Inventories.Remove(pile);

            if (pile.Tile.Room == null)
                return;

            if (pile.GetAvailableItemCount(item) <= 0)
                RemoveFromRoomLookup(pile, item, pile.Tile.Room.Id);
        }

        private void OnReservedWeightChanged(Inventory pile, Item item, int weight)
        {
            int roomId = pile.Tile.GetRoomId();

            if (pile.GetAvailableItemCount(item) <= 0)
                RemoveFromRoomLookup(pile, item, roomId);
            else
                AddToRoomLookup(pile, item, roomId);
        }

        private void AddToRoomLookup(Inventory pile, Item item, int roomId)
        {
            Dictionary<Item, List<Inventory>> roomItems = worldManager.TilesThatHaveItems[roomId];

            if (roomItems.TryGetValue(item, out List<Inventory> piles) == false)
            {
                piles = new List<Inventory>();
                roomItems.Add(item, piles);
            }

            if (piles.Contains(pile) == false)
                piles.Add(pile);
        }

        private void RemoveFromRoomLookup(Inventory pile, Item item, int roomId)
        {
            if (worldManager.TilesThatHaveItems[roomId].TryGetValue(item, out List<Inventory> piles))
                piles.Remove(pile);
        }
    }
}
