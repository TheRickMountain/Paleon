using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Settler : Entity
    {

        public Settler(SettlerInfo settlerInfo, List<Item> beverageRation, Tile spawnTile, InteractablesManager interactablesManager)
        {
            Add(new CreatureThoughts());

            CreatureStats creatureStats = new CreatureStats(0, 2, 1.0f);
            creatureStats.DeactivateAll();
            creatureStats.Energy.Active = true;
            creatureStats.Happiness.Active = true;
            creatureStats.Health.Active = true;
            creatureStats.Hunger.Active = true;
            creatureStats.Temperature.Active = true;


            SettlerCmp settler = new SettlerCmp(settlerInfo, creatureStats, 4.6f, spawnTile, interactablesManager);

            Add(settler);

            foreach (var item in Engine.Instance.SettlerRation)
            {
                settler.FoodRation.Add(item);
            }

            foreach(var item in beverageRation)
            {
                settler.BeverageRation.Add(item);
            }

            Add(new SelectableCmp(0, -8, 16, 24, SelectableType.Settler));
        }

    }
}
