using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class PrecipitationManager
    {

        private AnimatedSprite[] sprites;

        private float alpha = 0;

        public PrecipitationManager(Season startSeason)
        {
            MyTexture rainTexture = PrecipitationSpritesheetGenerator.GenerateRainSprite(256, 256);
            MyTexture snowTexture = PrecipitationSpritesheetGenerator.GenerateSnowSprite(256, 256);

            int width = GameplayScene.WorldSize / Engine.TILE_SIZE;
            int height = GameplayScene.WorldSize / Engine.TILE_SIZE;

            sprites = new AnimatedSprite[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    AnimatedSprite sprite = new AnimatedSprite(256, 256);
                    sprite.Add("Rain", new Animation(rainTexture, 12, 0, 256, 256, 0, 0, 20, true));
                    sprite.Add("Snow", new Animation(snowTexture, 12, 0, 256, 256, 0, 0, 20, true));
                    sprite.Play("Snow");
                    sprite.X = 256 * x;
                    sprite.Y = 256 * y;
                    sprites[y * width + x] = sprite;
                }
            }

            OnDayChanged(0, startSeason);
        }

        public void OnDayChanged(int day, Season season)
        {
            if(season == Season.Winter)
            {
                foreach(var sprite in sprites)
                {
                    sprite.Play("Snow");
                }
            }
            else
            {
                foreach (var sprite in sprites)
                {
                    sprite.Play("Rain");
                }
            }
        }

        public void Update()
        {
            if (GameplayScene.Instance.WorldState.CurrentWeather == Weather.Precipitation)
            {
                alpha += 0.1f * Engine.GameDeltaTime;

                if (alpha >= 1)
                    alpha = 1;
                else
                    for (int i = 0; i < sprites.Length; i++)
                        sprites[i].Color = Color.White * alpha;
            }
            else
            {
                alpha -= 0.1f * Engine.GameDeltaTime;

                if (alpha <= 0)
                    alpha = 0;
                else
                    for (int i = 0; i < sprites.Length; i++)
                        sprites[i].Color = Color.White * alpha;
            }

            if (alpha > 0)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    sprites[i].Update();
                }
            }
        }

        public void Render()
        {
            if (alpha > 0)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    sprites[i].Render();
                }
            }
        }

    }
}
