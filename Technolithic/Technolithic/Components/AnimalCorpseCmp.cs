namespace Technolithic
{
    public class AnimalCorpseCmp : Component
    {
        private Sprite _bodySprite;

        public AnimalCorpseCmp(AnimalTemplate animalTemplate) : base(true, true)
        {
            MyTexture corpseTexture = animalTemplate.CorpseTexture;

            _bodySprite = new Sprite(corpseTexture, corpseTexture.Width, corpseTexture.Height);
            _bodySprite.SetOrigin(0, 0);
            _bodySprite.X = (Engine.TILE_SIZE / 2) - (corpseTexture.Width / 2);
            _bodySprite.Y = -corpseTexture.Height / 2 + Engine.TILE_SIZE / 2;
        }

        public override void Begin()
        {
            base.Begin();

            _bodySprite.Entity = Entity;
        }

        public override void Render()
        {
            if(_bodySprite.Visible)
            {
                _bodySprite.Render();
            }

            base.Render();
        }
    }
}
