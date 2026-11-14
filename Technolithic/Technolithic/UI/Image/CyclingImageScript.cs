using System.Collections.Generic;

namespace Technolithic
{
    // Shows the textures one after another, e.g. every tool that can be used for a building.
    public class CyclingImageScript : MScript
    {
        private const float SecondsPerTexture = 1f;

        private readonly IReadOnlyList<MyTexture> textures;
        private float elapsedSeconds;
        private int index;

        public CyclingImageScript(IReadOnlyList<MyTexture> textures) : base(true)
        {
            this.textures = textures;
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            ShowTexture(0);
        }

        public override void Update(int mouseX, int mouseY)
        {
            elapsedSeconds += Engine.DeltaTime;

            if (elapsedSeconds < SecondsPerTexture)
                return;

            elapsedSeconds -= SecondsPerTexture;
            ShowTexture((index + 1) % textures.Count);
        }

        private void ShowTexture(int textureIndex)
        {
            index = textureIndex;
            ((MImageUI)ParentNode).Image.Texture = textures[index];
        }
    }
}
