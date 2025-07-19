namespace Technolithic
{
    public class AlphaNumericRule : IInputValidationRule
    {
        private bool _allowSpaces;
        private bool _allowUnderscore;
        private bool _allowLeadingSpaces;

        public AlphaNumericRule(bool allowSpaces, bool allowUnderscore, bool allowLeadingSpaces)
        {
            _allowSpaces = allowSpaces;
            _allowUnderscore = allowUnderscore;
            _allowLeadingSpaces = allowLeadingSpaces;
        }

        public bool IsValid(string currentText, char newChar, int position)
        {
            if (char.IsLetterOrDigit(newChar))
            {
                return true;
            }

            if (_allowSpaces && newChar == ' ')
            {
                if (_allowLeadingSpaces == false && position == 0)
                {
                    return false;
                }

                return true;
            }

            if (_allowUnderscore && newChar == '_')
            {
                return true;
            }

            return false;
        }
    }
}
