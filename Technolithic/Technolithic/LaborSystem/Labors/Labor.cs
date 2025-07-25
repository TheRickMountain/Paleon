using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public enum LaborType
    {
        None = 0,
        Build = 2,
        Agriculture = 28,
        Craft = 3,
        Supply = 4,
        Haul = 5,
        Fish = 7,
        Hunt = 9,
        Ranching = 11,
        Chop = 1,
        Gathering = 6,
        Mining = 8,
        Waner = 12,
        Sleep = 13,
        Eat = 14,
        Drink = 15,
        Equip = 16,
        Hide = 17,
        Rest = 18,
        Pray = 19,
        Heal = 20,
        Cook = 21,
        Beekeeping = 22,
        Learning = 23,
        Fertilization = 24,
        Medicine = 25,
        Cure = 26,
        Farm = 29,
        EnergyProduction = 30,
        Metallurgy = 31
    }

    public abstract class Labor
    {
        public LaborType LaborType { get; private set; }

        public bool IsCompleted { get; set; }

        public bool Repeat { get; set; }
        public bool IsMultiCreatureLabor { get; set; }

        public Dictionary<CreatureCmp, List<Task>> CreatureTasks;

        public Labor(LaborType laborType)
        {
            LaborType = laborType;

            CreatureTasks = new Dictionary<CreatureCmp, List<Task>>();
        }

        public abstract bool Check(CreatureCmp creature);

        public abstract void CreateTasks(CreatureCmp creature);

        public void CancelAndClearTasks(CreatureCmp creature)
        {
            List<Task> tasks = CreatureTasks[creature];

            for (int i = 0; i < tasks.Count; i++)
                tasks[i].Cancel();

            tasks.Clear();

            CreatureTasks.Remove(creature);
        }

        public virtual void CancelAndClearAllTasksAndComplete()
        {
            foreach (var kvp in CreatureTasks.ToList())
                CancelAndClearTasks(kvp.Key);

            IsCompleted = true;
        }

        public virtual void Complete()
        {
            foreach (var kvp in CreatureTasks.ToList())
                CancelAndClearTasks(kvp.Key);

            if (Repeat == false && IsMultiCreatureLabor == false)
            {
                IsCompleted = true;
            }
        }

        public void AddTask(CreatureCmp creature, Task task)
        {
            if (!CreatureTasks.ContainsKey(creature))
            {
                CreatureTasks.Add(creature, new List<Task>() { task });
                return;
            }

            CreatureTasks[creature].Add(task);
        }

        public List<Task> GetTasks(CreatureCmp creature)
        {
            return CreatureTasks[creature];
        }

        public Task GetNextTask(CreatureCmp creature)
        {
            if (CreatureTasks.ContainsKey(creature) == false)
            {
                return null;
            }

            List<Task> tasks = CreatureTasks[creature];
            tasks.RemoveAt(0);

            if (tasks.Count > 0)
                return tasks[0];
            
            if(Repeat == false && IsMultiCreatureLabor == false)
            {
                IsCompleted = true;
                Complete();
            }
            else
            {
                CreatureTasks.Remove(creature);
            }

            return null;
        }

        public void InitTasks(CreatureCmp creature)
        {
            List<Task> tasks = CreatureTasks[creature];

            foreach (var task in tasks)
                task.Begin();
        }

        public static IEnumerable<LaborType> GetWorkLaborEnumerator()
        {
            foreach (var labor in Enum.GetValues(typeof(LaborType)))
            {
                LaborType laborType = (LaborType)labor;
                if (laborType != LaborType.Eat &&
                    laborType != LaborType.Waner && 
                    laborType != LaborType.Sleep &&
                    laborType != LaborType.Rest && 
                    laborType != LaborType.Hide && 
                    laborType != LaborType.Equip &&
                    laborType != LaborType.Heal &&
                    laborType != LaborType.Pray &&
                    laborType != LaborType.Drink &&
                    laborType != LaborType.Fertilization &&
                    laborType != LaborType.None &&
                    laborType != LaborType.Cure &&
                    laborType != LaborType.Farm)
                {
                    yield return (LaborType)labor;
                }
            }
        }

        public static string GetLaborString(LaborType laborType)
        {
            switch (laborType)
            {
                case LaborType.Chop:
                    return Localization.GetLocalizedText("felling");
                case LaborType.Gathering:
                    return Localization.GetLocalizedText("gathering_labor");
                case LaborType.Mining:
                    return Localization.GetLocalizedText("mining_labor");
                case LaborType.Haul:
                    return Localization.GetLocalizedText("hauling");
                case LaborType.Build:
                    return Localization.GetLocalizedText("building");
                case LaborType.Craft:
                    return Localization.GetLocalizedText("crafting");
                case LaborType.Fish:
                    return Localization.GetLocalizedText("fishing");
                case LaborType.Supply:
                    return Localization.GetLocalizedText("supplying");
                case LaborType.Ranching:
                    return Localization.GetLocalizedText("ranching");
                case LaborType.Hunt:
                    return Localization.GetLocalizedText("hunting");
                case LaborType.Cook:
                    return Localization.GetLocalizedText("cooking");
                case LaborType.Beekeeping:
                    return Localization.GetLocalizedText("beekeeping");
                case LaborType.Learning:
                    return Localization.GetLocalizedText("learning");
                case LaborType.Medicine:
                    return Localization.GetLocalizedText("medicine");
                case LaborType.Agriculture:
                    return Localization.GetLocalizedText("agriculture");
                case LaborType.EnergyProduction:
                    return Localization.GetLocalizedText("energy_production");
                case LaborType.Metallurgy:
                    return Localization.GetLocalizedText("metallurgy");
                default:
                    return laborType.ToString();

            }
        }

        public static MyTexture GetLaborIcon(LaborType laborType)
        {
            switch (laborType)
            {
                case LaborType.Chop:
                    return ResourceManager.GetTexture("ui").GetSubtexture(0, 224, 16, 16);
                case LaborType.Gathering:
                    return ResourceManager.GetTexture("ui").GetSubtexture(16, 224, 16, 16);
                case LaborType.Mining:
                    return ResourceManager.GetTexture("ui").GetSubtexture(32, 224, 16, 16);
                case LaborType.Haul:
                    return ResourceManager.GetTexture("ui").GetSubtexture(48, 224, 16, 16);
                case LaborType.Build:
                    return ResourceManager.GetTexture("ui").GetSubtexture(64, 224, 16, 16);
                case LaborType.Craft:
                    return ResourceManager.GetTexture("ui").GetSubtexture(80, 224, 16, 16);
                case LaborType.Fish:
                    return ResourceManager.GetTexture("ui").GetSubtexture(112, 224, 16, 16);
                case LaborType.Supply:
                    return ResourceManager.GetTexture("ui").GetSubtexture(128, 224, 16, 16);
                case LaborType.Ranching:
                    return ResourceManager.GetTexture("ui").GetSubtexture(144, 224, 16, 16);
                case LaborType.Hunt:
                    return ResourceManager.GetTexture("ui").GetSubtexture(160, 224, 16, 16);
                case LaborType.Cook:
                    return ResourceManager.FoodIcon;
                case LaborType.Beekeeping:
                    return ResourceManager.BeeIcon;
                case LaborType.Learning:
                    return ResourceManager.LampIcon;
                case LaborType.Medicine:
                    return ResourceManager.MedicineIcon;
                case LaborType.Agriculture:
                    return ResourceManager.GetTexture("ui").GetSubtexture(96, 224, 16, 16);
                case LaborType.EnergyProduction:
                    return ResourceManager.EnergyProductionIcon;
                case LaborType.Metallurgy:
                    return ResourceManager.MetallurgyIcon;
                default:
                    return RenderManager.Pixel;

            }
        }

        public static LaborType GetLaborTypeByName(string laborName)
        {
            return (LaborType)Enum.Parse(typeof(LaborType), laborName);
        }

    }
}
