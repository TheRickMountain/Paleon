using System;

namespace Technolithic
{
    public class NumericRule : IInputValidationRule
    {
        private int _maxNumber;
        private bool _allowLeadingZeros;

        public NumericRule(int maxNumber, bool allowLeadingZeros)
        {
            _maxNumber = maxNumber;
            _allowLeadingZeros = allowLeadingZeros;
        }

        public bool IsValid(string currentText, char newChar, int position)
        {
            if (char.IsDigit(newChar) == false) return false;

            if (_allowLeadingZeros == false && position == 0 && newChar == '0') return false;

            try
            {
                int value = int.Parse(currentText + newChar);

                if (value > _maxNumber)
                {
                    return false;
                }
            }
            catch (Exception) { return false; }

            return true;
        }
    }
}
