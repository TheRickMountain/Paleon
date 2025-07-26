using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Technolithic
{
    public class TechnologyDatabase
    {
        public static Dictionary<int, Technology> Technologies { get; private set; } = new Dictionary<int, Technology>();

        private static Dictionary<InteractionType, Technology> interactionUnlockTechnology = new();

        private static Dictionary<AnimalTemplate, Technology> animalUnlockTechnology = new();

        public static void Initialize(string contentDirectory)
        {
            Tileset tileset = new Tileset(ResourceManager.GetTexture("technologies_icons"), 16, 16);

            string jsonString = File.ReadAllText(Path.Combine(contentDirectory, "Technologies", "technologies_data.json"));

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ItemJsonConverter());
            settings.Converters.Add(new AnimalTemplateJsonConverter());
            settings.Converters.Add(new BuildingTemplateJsonConverter());

            List<Technology> technologies = JsonConvert.DeserializeObject<List<Technology>>(jsonString, settings);

            foreach (Technology technology in technologies)
            {
                Technologies.Add(technology.Id, technology);
            }

            foreach (Technology technology in technologies)
            {
                technology.Name = Localization.GetLocalizedText(technology.Name);

                if(technology.HasIcon)
                {
                    technology.Icon = tileset[technology.Id];
                }

                if (technology.Parents.Length > 0)
                {
                    technology.ParentTechnologies = new List<Technology>();

                    foreach (int parentId in technology.Parents)
                    {
                        Technology parentTechnology = Technologies[parentId];

                        technology.ParentTechnologies.Add(parentTechnology);

                        technology.Order = Math.Max(technology.Order, parentTechnology.Order) + 1;
                    }
                }

                foreach (InteractionType interactionType in technology.UnlockInteractionTypes)
                {
                    interactionUnlockTechnology.Add(interactionType, technology);
                }

                if (technology.UnlockedAnimals != null)
                {
                    foreach (AnimalTemplate animalTemplate in technology.UnlockedAnimals)
                    {
                        animalUnlockTechnology.Add(animalTemplate, technology);
                    }
                }
            }
        }

        public static Technology GetTechnologyThatUnlocksInteraction(InteractionType interactionType)
        {
            if (interactionUnlockTechnology.TryGetValue(interactionType, out var technology))
            {
                return technology;
            }

            return null;
        }

        public static Technology GetTechnologyThatUnlocksAnimal(AnimalTemplate animalTemplate)
        {
            if (animalUnlockTechnology.TryGetValue(animalTemplate, out var technology))
            {
                return technology;
            }

            return null;
        }

    }
}
