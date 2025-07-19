using System.Collections.Generic;

namespace Technolithic
{
    public class InputValidator
    {
        private readonly List<IInputValidationRule> rules;

        public InputValidator()
        {
            rules = new();
        }

        public InputValidator AddRule(IInputValidationRule rule)
        {
            rules.Add(rule);
            return this;
        }

        public bool ValidateInput(string currentText, char newChar, int position)
        {
            foreach (var rule in rules)
            {
                if (rule.IsValid(currentText, newChar, position) == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
