namespace Technolithic
{
    public class MaxLengthRule : IInputValidationRule
    {
        private readonly int maxLength;

        public MaxLengthRule(int maxLength)
        {
            this.maxLength = maxLength;
        }

        public bool IsValid(string currentText, char newChar, int position)
        {
            return currentText.Length < maxLength;
        }
    }
}
