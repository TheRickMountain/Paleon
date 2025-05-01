using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AnimalTemplateDatabase
    {

        private static Dictionary<string, AnimalTemplate> animals = new Dictionary<string, AnimalTemplate>();

        private static List<AnimalTemplate> wildAnimals = new List<AnimalTemplate>();

        public static void Initialize()
        {
            foreach (string file in Directory.GetFiles(Path.Combine(Engine.ContentDirectory, "Animals")))
            {
                if (Path.GetExtension(file) == ".json")
                {
                    AnimalTemplate animalTemplate = new AnimalTemplate(Path.GetFileName(file));

                    animals.Add(Path.GetFileNameWithoutExtension(file), animalTemplate);

                    if(animalTemplate.IsWild)
                    {
                        wildAnimals.Add(animalTemplate);
                    }
                }
            }

            foreach (AnimalTemplate animal in animals.Values)
            {
                if(animal.PregnancyData != null)
                {
                    animal.PregnancyData.OffspringTemplates = new List<AnimalTemplate>();

                    foreach (string offspringTemplateName in animal.PregnancyData.Offspring)
                    {
                        animal.PregnancyData.OffspringTemplates.Add(animals[offspringTemplateName]);
                    }
                }

                if(animal.DomesticationData != null)
                {
                    animal.DomesticationData.TamedFormAnimalTemplate = animals[animal.DomesticationData.TamedForm];
                }
            }
        }

        public static IEnumerable<AnimalTemplate> GetAnimalTemplates()
        {
            foreach(var kvp in animals)
            {
                yield return kvp.Value;
            }
        }

        public static AnimalTemplate GetAnimalTemplateByName(string name)
        {
            AnimalTemplate animalTemplate;
            animals.TryGetValue(name, out animalTemplate);
            return animalTemplate;
        }

        public static AnimalTemplate GetRandomWildAnimalTemplate()
        {
            int randomNumber = MyRandom.Range(wildAnimals.Count);
            return wildAnimals[randomNumber];
        }

    }
}
