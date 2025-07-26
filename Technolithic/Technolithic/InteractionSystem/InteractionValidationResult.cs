namespace Technolithic
{
    public class InteractionValidationResult
    {
        public bool IsAllowed { get; }
        public string BlockingReason { get; }

        private InteractionValidationResult(bool isAllowed, string blockingReason = null)
        {
            IsAllowed = isAllowed;
            BlockingReason = blockingReason;
        }

        public static InteractionValidationResult Allow() => new(true);
        public static InteractionValidationResult Block(string reason) => new(false, reason);
    }

    public delegate InteractionValidationResult InteractionValidator(Interactable interactable);
}
