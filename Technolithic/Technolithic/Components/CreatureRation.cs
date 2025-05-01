using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class CreatureRation
    {
        private Dictionary<Item, bool> filters;

        public int Count { get { return filters.Count; } }

        public CreatureRation()
        {
            filters = new Dictionary<Item, bool>();
        }

        public void Add(Item item)
        {
            filters.Add(item, true);
        }

        public void SetFilter(Item item, bool value)
        {
            filters[item] = value;
        }

        public IEnumerable<Item> GetAllowedRation()
        {
            foreach(var kvp in filters)
            {
                Item item = kvp.Key;
                if (filters[item])
                    yield return item;
            }
        }

        public IEnumerable<KeyValuePair<Item, bool>> GetFilters()
        {
            foreach (var kvp in filters)
                yield return kvp;
        }

        public bool IsFoodAllowed(Item item)
        {
            if(filters.ContainsKey(item))
                return filters[item];

            return false;
        }
    }
}
