using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{

    public class LaborManager
    {

        public Dictionary<LaborType, List<Labor>> LaborsByType { get; private set; }

        private List<Labor> labors;
        private List<Labor> laborsToRemove;

        public int Count { get { return labors.Count; } }

        public LaborManager()
        {
            LaborsByType = new Dictionary<LaborType, List<Labor>>();

            foreach (var laborType in Enum.GetValues(typeof(LaborType)))
            {
                LaborsByType.Add((LaborType)laborType, new List<Labor>());
            }

            labors = new List<Labor>();
            laborsToRemove = new List<Labor>();
        }

        public void Update()
        {
            CheckCompletedLabors();
        }

        private void CheckCompletedLabors()
        {
            // Looking for completed labors
            for (int i = 0; i < labors.Count; i++)
            {
                Labor labor = labors[i];
                if (labor.IsCompleted)
                    laborsToRemove.Add(labor);
            }

            // Remove all completed labors
            if (laborsToRemove.Count > 0)
            {
                for (int i = 0; i < laborsToRemove.Count; i++)
                {
                    Labor labor = laborsToRemove[i];
                    labors.Remove(labor);
                    LaborsByType[labor.LaborType].Remove(labor);
                }

                laborsToRemove.Clear();
            }
        }

        public void Add(Labor labor)
        {
            if (labors.Contains(labor))
                throw new Exception("LaborManager has this labor!");

            LaborsByType[labor.LaborType].Add(labor);

            labors.Add(labor);
        }


    }
}
