namespace Technolithic
{
    public class InteractionConfig
    {
        public InteractionValidator Validator { get; set; }

        public InteractionValidationResult ValidateInteraction(Interactable interactable)
        {
            if (Validator != null)
            {
                return Validator(interactable);
            }

            return InteractionValidationResult.Allow();
        }
    }
}
