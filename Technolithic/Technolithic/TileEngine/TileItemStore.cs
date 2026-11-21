using System;
using System.Collections.Generic;

namespace Technolithic
{
    public class TileItemStore
    {
        private readonly Dictionary<Tile, Inventory> piles = new Dictionary<Tile, Inventory>();
        private readonly HashSet<Inventory> emptyPiles = new HashSet<Inventory>();

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

        public void RemoveEmptyPiles()
        {
            if (emptyPiles.Count == 0)
                return;

            List<Inventory> candidates = new List<Inventory>(emptyPiles);
            emptyPiles.Clear();

            foreach (Inventory pile in candidates)
            {
                if (IsEmptyAndUnreserved(pile) == false)
                    continue;

                piles.Remove(pile.Tile);

                pile.OnItemAddedCallback -= OnPileItemAdded;
                pile.OnItemRemovedCallback -= OnPileItemRemoved;
                pile.OnReservedWeightChanged -= OnPileReservedWeightChanged;
            }
        }

        private static bool IsEmptyAndUnreserved(Inventory pile)
        {
            return pile.TotalItemsCount <= 0 && pile.TotalReservedCount <= 0;
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

            RememberIfEmpty(pile);
        }

        private void OnPileReservedWeightChanged(Inventory pile, Item item, int weight)
        {
            ReservedWeightChanged?.Invoke(pile, item, weight);

            RememberIfEmpty(pile);
        }

        private void RememberIfEmpty(Inventory pile)
        {
            if (IsEmptyAndUnreserved(pile))
                emptyPiles.Add(pile);
        }
    }
}
