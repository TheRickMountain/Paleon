using Microsoft.Xna.Framework;

namespace Technolithic
{
    public enum InteractionIconDisplayState
    {
        OnMarked,
        OnUnmarked
    }

    public class InteractionData
    {
        
      
        public InteractionType InteractionType { get; private set; }
        public LaborType LaborType { get; private set; }
        public string DisplayName { get; private set; }
        public MyTexture Icon { get; private set; }
        public Color ProgressBarColor { get; private set; }
        public InteractionIconDisplayState IconDisplayState { get; private set; }

        public InteractionData(InteractionType interactionType, LaborType laborType, string displayName, MyTexture icon, 
            Color progressBarColor, InteractionIconDisplayState iconDisplayState)
        {
            InteractionType = interactionType;
            LaborType = laborType;
            DisplayName = displayName;
            Icon = icon;
            ProgressBarColor = progressBarColor;
            IconDisplayState = iconDisplayState;
        }

    }
}
