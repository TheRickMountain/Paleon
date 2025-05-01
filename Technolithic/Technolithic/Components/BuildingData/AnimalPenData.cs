using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AnimalPenData
    {

        public int Slots { get; private set; }
        private HashSet<AnimalTemplate> filters;

        public AnimalPenData(JObject jobject)
        {
            Slots = jobject["animalPen"]["slots"].Value<int>();

            filters = new HashSet<AnimalTemplate>();
            foreach(var animal in jobject["animalPen"]["filters"])
            {
                string animalJson = animal.Value<string>();
                AnimalTemplate animalTemplate = AnimalTemplateDatabase.GetAnimalTemplateByName(animalJson);
                filters.Add(animalTemplate);
            }
        }

        public bool CheckAnimalFitsToFilters(AnimalTemplate animalTemplate)
        {
            return filters.Contains(animalTemplate);
        }

        public IEnumerable<AnimalTemplate> GetAllowedAnimalTemplates()
        {
            foreach(var animalTemplate in filters)
            {
                yield return animalTemplate;
            }
        }

    }
}
