namespace Technolithic
{
    public interface IInputValidationRule
    {
        bool IsValid(string currentText, char newChar, int position);
    }
}
