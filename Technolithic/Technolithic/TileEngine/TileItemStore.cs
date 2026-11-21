using System;
using System.Collections.Generic;

namespace Technolithic
{
    public class TileItemStore
    {
        private readonly Dictionary<Tile, Inventory> piles = new Dictionary<Tile, Inventory>();

        public event Action<Inventory, Item, int> ItemAdded;
        public event Action<Inventory, Item, int> ItemRemoved;
        public event Action<Inventory, Item, int> ReservedWeightChanged;

        public IReadOnlyCollection<Inventory> Piles => piles.Values;

        public Inventory GetPile(Tile tile)
        {
            return piles.GetValueOrDefault(tile);
        }

        public void Drop(Tile tile, ItemContainer itemContainer)
        {
            GetOrCreatePile(tile).AddCargo(itemContainer);
        }

        public void Drop(Tile tile, Item item, int weight)
        {
            Drop(tile, new ItemContainer(item, weight, item.Durability));
        }

        public void Drop(Tile tile, Item item, int weight, float durability)
        {
            Drop(tile, new ItemContainer(item, weight, durability));
        }

        private Inventory GetOrCreatePile(Tile tile)
        {
            if (piles.TryGetValue(tile, out Inventory pile))
                return pile;

            pile = new Inventory(tile)
            {
                CountsInTotalResourcesChart = false
            };

            pile.OnItemAddedCallback += OnPileItemAdded;
            pile.OnItemRemovedCallback += OnPileItemRemoved;
            pile.OnReservedWeightChanged += OnPileReservedWeightChanged;

            piles.Add(tile, pile);

            return pile;
        }

        private void OnPileItemAdded(Inventory pile, Item item, int weight)
        {
            ItemAdded?.Invoke(pile, item, weight);
        }

        private void OnPileItemRemoved(Inventory pile, Item item, int weight)
        {
            ItemRemoved?.Invoke(pile, item, weight);
        }

        private void OnPileReservedWeightChanged(Inventory pile, Item item, int weight)
        {
            ReservedWeightChanged?.Invoke(pile, item, weight);
        }
    }
}
