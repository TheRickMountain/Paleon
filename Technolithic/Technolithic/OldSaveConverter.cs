using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public static class OldSaveConverter
    {

        public static void Convert(SaveManager saveManager)
        {
            if(saveManager.Info.GameVersion < new GameVersion(1, 4, 0))
            {
                #region Floor
                WorldSaveData worldSaveData = saveManager.Data.WorldSaveData;
                List<BuildingSaveData> BuildingSaveDatas = saveManager.Data.BuildingSaveDatas;

                for (int x = 0; x < worldSaveData.Width; x++)
                {
                    for (int y = 0; y < worldSaveData.Height; y++)
                    {
                        worldSaveData.Tiles[x, y].SurfaceId = -1;
                    }
                }

                foreach (var buildingSaveData in BuildingSaveDatas)
                {
                    if (buildingSaveData.IsBuilt == false) continue;

                    Point tile = buildingSaveData.Tiles[0, 0];

                    if (buildingSaveData.BuildingTemplateName.Equals("dirt_path"))
                    {
                        worldSaveData.Tiles[tile.X, tile.Y].SurfaceId = Engine.Instance.Buildings["dirt_path_surface"].SurfaceData.Id;
                    }
                    else if (buildingSaveData.BuildingTemplateName == "stone_floor")
                    {
                        worldSaveData.Tiles[tile.X, tile.Y].SurfaceId = Engine.Instance.Buildings["stone_path_surface"].SurfaceData.Id;
                    }
                    else if (buildingSaveData.BuildingTemplateName == "pile_floor")
                    {
                        worldSaveData.Tiles[tile.X, tile.Y].SurfaceId = Engine.Instance.Buildings["wooden_bridge_surface"].SurfaceData.Id;
                    }
                    else if (buildingSaveData.BuildingTemplateName == "mudbrick_floor")
                    {
                        worldSaveData.Tiles[tile.X, tile.Y].SurfaceId = Engine.Instance.Buildings["mudbrick_path_surface"].SurfaceData.Id;
                    }
                    else if (buildingSaveData.BuildingTemplateName == "brick_floor")
                    {
                        worldSaveData.Tiles[tile.X, tile.Y].SurfaceId = Engine.Instance.Buildings["brick_path_surface"].SurfaceData.Id;
                    }
                }
                #endregion
            }

            if (saveManager.Info.GameVersion < new GameVersion(1, 5, 0))
            {
                foreach (var buildingSaveData in saveManager.Data.BuildingSaveDatas)
                {
                    if (buildingSaveData.AnimalsFilters != null)
                    {
                        Dictionary<string, bool> animalsFilters = buildingSaveData.AnimalsFilters;

                        var domesticatedAnimalsKeys = new[] { "cattle", "pig", "sheep", "dog", "horse" };
                        var animalGenders = new[] { "male", "female" };
                        var animalAgeStates = new[] { "baby", "adult", "old" };

                        foreach (string animalKey in domesticatedAnimalsKeys)
                        {
                            foreach (string animalGender in animalGenders)
                            {
                                if (animalsFilters.ContainsKey($"{animalKey}_{animalGender}") == false) continue;

                                bool isAllowed = animalsFilters[$"{animalKey}_{animalGender}"];

                                foreach (string animalAgeState in animalAgeStates)
                                {
                                    animalsFilters.Add($"{animalKey}_{animalGender}_{animalAgeState}", isAllowed);
                                }

                                animalsFilters.Remove($"{animalKey}_{animalGender}");
                            }
                        }
                    }
                }

                var oldAnimalsAges = new[]
                    {
                        ("cattle_male", 6, 66),
                        ("cattle_female", 6, 66),

                        ("aurochs_male", 6, 66),
                        ("aurochs_female", 6, 66),

                        ("pig_male", 4, 28),
                        ("pig_female", 4, 28),

                        ("boar_male", 4, 28),
                        ("boar_female", 4, 28),

                        ("horse_male", 6, 94),
                        ("horse_female", 6, 94),

                        ("tarpan_male", 6, 94),
                        ("tarpan_female", 6, 94),

                        ("mouflon_male", 5, 35),
                        ("mouflon_female", 5, 35),

                        ("sheep_male", 5, 35),
                        ("sheep_female", 5, 35),

                        ("dog_male", 6, 38),
                        ("dog_female", 6, 38),

                        ("wolf_male", 6, 38),
                        ("wolf_female", 6, 38),

                        ("mammoth_male", 8, 92),
                        ("mammoth_female", 8, 92)
                    };

                foreach (var creatureSaveData in saveManager.Data.CreatureSaveDatas)
                {
                    if(creatureSaveData.CreatureType == CreatureType.Animal)
                    {
                        foreach(var animalAge in oldAnimalsAges)
                        {
                            if(creatureSaveData.AnimalTemplateName == animalAge.Item1)
                            {
                                string newAnimalTemplateName;

                                if(creatureSaveData.Age >= animalAge.Item3)
                                {
                                    newAnimalTemplateName = $"{animalAge.Item1}_old";
                                }
                                else if(creatureSaveData.Age >= animalAge.Item2)
                                {
                                    newAnimalTemplateName = $"{animalAge.Item1}_adult";
                                }
                                else
                                {
                                    newAnimalTemplateName = $"{animalAge.Item1}_baby";
                                }

                                creatureSaveData.AnimalTemplateName = newAnimalTemplateName;
                                creatureSaveData.DaysUntilAging = AnimalTemplateDatabase.GetAnimalTemplateByName(newAnimalTemplateName).DaysUntilAging;
                            }
                        }
                    }
                }
            }

            if (saveManager.Info.GameVersion < new GameVersion(1, 6, 0))
            {
                foreach (var buildingSaveData in saveManager.Data.BuildingSaveDatas)
                {
                    if(Engine.Instance.Buildings.ContainsKey(buildingSaveData.BuildingTemplateName) == false)
                    {
                        continue;
                    }

                    BuildingTemplate buildingTemplate = Engine.Instance.Buildings[buildingSaveData.BuildingTemplateName];

                    if(buildingTemplate.BuildingType != BuildingType.Hut)
                    {
                        continue;
                    }

                    buildingSaveData.HutAssignedCreatures = new List<Guid>();

                    for (int i = 0; i < buildingTemplate.Assignable.Slots; i++)
                    {
                        buildingSaveData.HutAssignedCreatures.Add(Guid.Empty);
                    }
                }
            }
        
            if (saveManager.Info.GameVersion < new GameVersion(1, 6, 1))
            {
                foreach(var creatureSaveData in saveManager.Data.CreatureSaveDatas)
                {
                    creatureSaveData.Tools = new List<Tuple<int, int, float>>();

                    if(creatureSaveData.ToolId != -1)
                    {
                        Item item = ItemDatabase.GetItemById(creatureSaveData.ToolId);

                        if (item == null)
                            continue;
                        
                        int amount = creatureSaveData.ToolFactWeight;
                        float durability = creatureSaveData.ToolDurability;

                        creatureSaveData.Tools.Add(new Tuple<int, int, float>(item.Id, amount, durability));
                    }
                }
            }
        }

    }
}
