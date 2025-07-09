namespace Technolithic
{
    public class InteractionData
    {
      
        public InteractionType InteractionType { get; private set; }
        public LaborType LaborType { get; private set; }
        public string DisplayName { get; private set; }
        public MyTexture Icon { get; private set; }

        public InteractionData(InteractionType interactionType, LaborType laborType, string displayName, MyTexture icon)
        {
            InteractionType = interactionType;
            LaborType = laborType;
            DisplayName = displayName;
            Icon = icon;
        }

    }
}
