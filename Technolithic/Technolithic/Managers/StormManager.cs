using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class StormManager
    {

        private float lightningAlpha = 0.0f;

        private float timer;

        private SoundEffectInstance[] thunderEffects;

        private Sprite lightningSprite;

        public StormManager()
        {
            timer = MyRandom.Range(25, 150);

            thunderEffects = new SoundEffectInstance[4];
            for (int i = 0; i < 4; i++)
            {
                thunderEffects[i] = CreateSoundEffect($"thunder_{i + 1}", false);
            }

            lightningSprite = new Sprite(ResourceManager.GetTexture("lightning"), 16, 256);
            lightningSprite.Active = false;
        }

        private SoundEffectInstance CreateSoundEffect(string name, bool loop)
        {
            var soundEffect = ResourceManager.GetSoundEffect(name).CreateInstance();
            soundEffect.IsLooped = loop;
            return soundEffect;
        }

        public void Update(Weather currentWeather, Season currentSeason)
        {
            if(currentWeather == Weather.Precipitation && currentSeason != Season.Winter)
            {
                if(timer <= 0)
                {
                    Tile randomTile = GetRandomTile();
                    HitLightning(randomTile);
                    timer = MyRandom.Range(25, 150);
                }
                else
                {
                    timer -= Engine.GameDeltaTime;
                }
            }

            if(lightningAlpha > 0)
            {
                lightningAlpha -= Engine.GameDeltaTime;
                GameplayScene.Instance.Penumbra.AmbientColor = Color.Lerp(GameplayScene.Instance.WorldState.TimeOfDayColor, Color.White, lightningAlpha);

                lightningSprite.Color = Color.White * lightningAlpha;

                if(lightningAlpha <= 0)
                {
                    lightningSprite.Active = false;
                }
            }
        }

        private Tile GetRandomTile()
        {
            int x = MyRandom.Range(GameplayScene.Instance.World.Width);
            int y = MyRandom.Range(GameplayScene.Instance.World.Height);
            return GameplayScene.Instance.World.GetTileAt(x, y);
        }

        public void Render()
        {
            if (lightningSprite.Active)
            {
                lightningSprite.Render();
            }
        }

        public void HitLightning(Tile tile)
        {
            lightningAlpha = 1.5f;

            thunderEffects[MyRandom.Range(thunderEffects.Length)].Play();

            lightningSprite.Active = true;
            lightningSprite.Position = tile.GetAsVector() * Engine.TILE_SIZE;
            lightningSprite.Y -= lightningSprite.Height - Engine.TILE_SIZE;
            lightningSprite.Color = Color.White;

            GameplayScene.Instance.SmokeManager.AddSmoke(tile.GetAsVector() * Engine.TILE_SIZE + new Vector2(Engine.TILE_SIZE / 2));
        }

    }
}
