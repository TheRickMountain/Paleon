using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MAttribute
    {
        public AttributeType AttributeType { get; set; }

        private float currentValue;

        public string Name;
        public string DeadMessage;

        public float CurrentValue
        {
            get => currentValue; 
            set 
            {
                currentValue = value;

                if(currentValue > MaxValue && CanBeMoreThanMaxValue == false)
                {
                    currentValue = MaxValue;
                }

                if(currentValue < MinValue)
                {
                    currentValue = MinValue;
                }
            }
        }

        private float minValue;
        
        public float MinValue 
        {
            get => minValue;
            set
            {
                minValue = value;

                if(CurrentValue < minValue)
                {
                    CurrentValue = minValue;
                }
            }
        }

        private float maxValue;

        public float MaxValue 
        {
            get => maxValue;
            set
            {
                maxValue = value;

                if(CurrentValue > maxValue && CanBeMoreThanMaxValue == false)
                {
                    CurrentValue = maxValue;
                }
            }
        }
        
        public float DissatisfiedThreshold { get; set; }
        public float SatisfiedThreshold { get; set; }
        public bool Active { get; set; }
        public bool DeadlyIfLower { get; set; } // true - существо умирает при показателе меньше минимального
        public bool DeadlyIfMore { get; set; } // true - существо умирает при показателе больше максимального
        public bool CanBeMoreThanMaxValue { get; set; } = false;

        public float ChangePerSecond { get; private set; }

        private float changePerDay;
        public float ChangePerDay
        {
            get { return changePerDay; }
            set
            {
                changePerDay = value;

                ChangePerSecond = value / (WorldState.MINUTES_PER_HOUR * WorldState.HOURS_PER_CYCLE);
            }
        }

        public float ExtraChangePerSecond { get; private set; }

        private float extraChangePerDay;
        public float ExtraChangePerDay
        {
            get { return extraChangePerDay; }
            set
            {
                extraChangePerDay = value;

                ExtraChangePerSecond = value / (WorldState.MINUTES_PER_HOUR * WorldState.HOURS_PER_CYCLE);
            }
        }

        public bool IsSatisfied()
        {
            return CurrentValue >= SatisfiedThreshold;
        }

        public bool IsDissatisfied()
        {
            return CurrentValue <= DissatisfiedThreshold;
        }
        
        public string GetInformation()
        {
            string changingSymbolString = "";

            float totalChangePerDay = ChangePerDay + ExtraChangePerDay;

            if (totalChangePerDay > 0)
            {
                changingSymbolString = "/c[#99CD32]>/cd";
            }
            else if(totalChangePerDay < 0)
            {
                changingSymbolString = "/c[#FF2A00]</cd";
            }



            if (CurrentValue <= DissatisfiedThreshold)
            {
                return $"- {Name}: /c[#FF781F]{(int)CurrentValue}/{(int)MaxValue}/cd   {changingSymbolString}";
            }
            else
            {
                return $"- {Name}: {(int)CurrentValue}/{(int)MaxValue}   {changingSymbolString}";
            }
        }

        public string GetAdditionalInformation()
        {
            return $"{(ChangePerDay + ExtraChangePerDay).ToString("+#;-#;0")} {Localization.GetLocalizedText("per_day")}";
        }
    }
}
