using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Technolithic
{
    public class Trap
    {

        private List<Item> catchItems = new List<Item>();

        public IReadOnlyList<Item> CatchItems
        {
            get { return catchItems.AsReadOnly(); }
        }

        public Trap(JObject jobject)
        {
            JToken trapToken = jobject["trap"];

            foreach(var item in trapToken["catch_items"])
            {
                catchItems.Add(ItemDatabase.GetItemByName(item.ToString()));
            }
        }

    }
}
