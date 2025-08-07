using MonoGame.Extended.Sprites;
using System.Collections.Generic;

namespace Technolithic
{
    public class AnimalCorpseCmp : Interactable
    {
        private AnimalTemplate _animalTemplate;
        private ButcheringData _butcheringData;
        private Tile _tile;

        public float SpoilageProgress { get; set; }

        private Sprite _sprite;

        private float _timer = 0.0f;

        public AnimalCorpseCmp(AnimalTemplate animalTemplate, Tile tile, InteractablesManager interactablesManager) 
            : base(interactablesManager)
        {
            _animalTemplate = animalTemplate;
            _tile = tile;
            _sprite = new Sprite(animalTemplate.DeadTexture);

            _butcheringData = animalTemplate.ButcheringData;

            // TODO: для разделки туши нужен инструмент
            AddAvailableInteraction(InteractionType.Butcher, LaborType.Cook);

            SetInteractionDuration(InteractionType.Butcher, _butcheringData.DurationInHours * WorldState.MINUTES_PER_HOUR);

            ActivateInteraction(InteractionType.Butcher);

            SpoilageProgress = 2.0f * WorldState.HOURS_PER_CYCLE * WorldState.MINUTES_PER_HOUR;
        }

        public override void Begin()
        {
            base.Begin();

            int textureWidth = _animalTemplate.DeadTexture.Width;
            int textureHeight = _animalTemplate.DeadTexture.Height;

            _sprite.Width = textureWidth;
            _sprite.Height = textureHeight;
            _sprite.SetOrigin(0, 0);
            _sprite.X = (Engine.TILE_SIZE / 2) - (textureWidth / 2);
            _sprite.Y = -(textureHeight - Engine.TILE_SIZE);

            Entity.Add(_sprite);
            Entity.Position = _tile.GetAsVector() * Engine.TILE_SIZE;
        }

        public override void Update()
        {
            base.Update();

            if (GameplayScene.Instance.WorldState.CurrentSeason != Season.Winter)
            {
                _timer -= Engine.GameDeltaTime;
                if (_timer <= 0)
                {
                    _timer = 1.0f;

                    SpoilageProgress -= 1.0f;

                    if (SpoilageProgress <= 0)
                    {
                        Destroy();

                        GameplayScene.Instance.RemoveAnimalCorpse(Entity as AnimalCorpse);
                    }
                }
            }
        }

        public override void CompleteInteraction(InteractionType interactionType)
        {
            base.CompleteInteraction(interactionType);

            switch (interactionType)
            {
                case InteractionType.Butcher:
                    {
                        foreach (var kvp in _butcheringData.RealLoot)
                        {
                            Item item = kvp.Key;
                            int count = kvp.Value;

                            _tile.Inventory.AddCargo(new ItemContainer(item, count, item.Durability));
                        }

                        Destroy();

                        GameplayScene.Instance.RemoveAnimalCorpse(Entity as AnimalCorpse);
                    }
                    break;
            }
        }

        public override Tile GetApproachableTile(CreatureCmp creature)
        {
            return GetApproachableTile(creature.Movement.CurrentTile.GetZoneId());
        }

        public override Tile GetApproachableTile(int zoneId)
        {
            if (_tile.GetZoneId() != zoneId) return null;

            if (_tile.IsWalkable == false) return null;

            return _tile;
        }

        public override IEnumerable<Tile> GetApproachableTiles()
        {
            if (_tile.IsWalkable)
            {
                yield return _tile;
            }
        }

        public override string GetUILabelText()
        {
            return _animalTemplate.GetNameWithAgeAndSex();
        }

        public virtual AnimalCorpseSaveData GetSaveData()
        {
            AnimalCorpseSaveData saveData = new AnimalCorpseSaveData();

            FillSaveData(saveData);

            saveData.AnimalKey = _animalTemplate.Json;
            saveData.SpoilageProgress = SpoilageProgress;
            saveData.TileX = _tile.X;
            saveData.TileY = _tile.Y;

            return saveData;
        }
    }
}
