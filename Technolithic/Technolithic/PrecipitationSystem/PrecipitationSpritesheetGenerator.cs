using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Technolithic
{
    public class PrecipitationSpritesheetGenerator
    {

        // Увеличить скорость некоторых капель
        // Завершать анимацию а зависимости от поверхности земли
        // Генерировать анимацию в соответсвии с размерами карты
        // Сделать класс статичным и имеющий метод, который принимает массив тайлов, стараться не брать данных извне

        private PrecipitationSpritesheetGenerator()
        {

        }

        public static MyTexture GenerateSnowSprite(int width, int height)
        {
            List<SnowDrop> snowDrops = new List<SnowDrop>();
            List<Color[]> colorDatas = new List<Color[]>();

            MyTexture snowTexture = ResourceManager.GetTexture("snow_drop");

            colorDatas.Add(snowTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(snowTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(snowTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(snowTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(snowTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(snowTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(snowTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(snowTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(snowTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(snowTexture.GetSubtexture(0, 16, 16, 16).GetData());
            colorDatas.Add(snowTexture.GetSubtexture(16, 16, 16, 16).GetData());
            colorDatas.Add(snowTexture.GetSubtexture(32, 16, 16, 16).GetData());
            colorDatas.Add(snowTexture.GetSubtexture(48, 16, 16, 16).GetData());
            colorDatas.Add(snowTexture.GetSubtexture(64, 16, 16, 16).GetData());
            colorDatas.Add(snowTexture.GetSubtexture(80, 16, 16, 16).GetData());

            for (int i = 0; i < width / 4; i++)
                snowDrops.Add(new SnowDrop(MyRandom.Range(width), MyRandom.Range(height), MyRandom.Range(13)));

            Texture2D[] frames = new Texture2D[12];

            for (int i = 0; i < frames.Length; i++)
            {
                Color[,] data = new Color[width, height];

                foreach (SnowDrop drop in snowDrops)
                {
                    Color[] colorData = colorDatas[drop.LifeStage];

                    for (int x = 0; x < 16; x++)
                    {
                        for (int y = 0; y < 16; y++)
                        {
                            int dropYPos = (drop.Y + y);
                            int dropXPos = (drop.X + x);

                            if (dropYPos < 0)
                                dropYPos += height;
                            else if (dropYPos >= height)
                                dropYPos -= height;

                            if (dropXPos < 0)
                                dropXPos += width;
                            else if (dropXPos >= width)
                                dropXPos -= width;


                            data[dropXPos, dropYPos] = colorData[y * 16 + x];
                        }
                    }
                }

                // Move rain drops to the next life stage
                foreach (SnowDrop rainDrop in snowDrops)
                    rainDrop.NextLifeStage();

                frames[i] = CreateTexture(Engine.Instance.GraphicsDevice, data);
            }

            Texture2D spriteSheet = new Texture2D(Engine.Instance.GraphicsDevice, width, height * frames.Length);

            Color[] spriteSheetData = new Color[width * (height * frames.Length)];

            for (int i = 0; i < frames.Length; i++)
            {
                Color[] frameData = new Color[width * height];

                frames[i].GetData(frameData);

                for (int j = 0; j < frameData.Length; j++)
                    spriteSheetData[j + (width * height) * i] = frameData[j];

            }

            spriteSheet.SetData(spriteSheetData);

            Stream stream = File.Create("snow_file.png");
            spriteSheet.SaveAsPng(stream, spriteSheet.Width, spriteSheet.Height);
            stream.Dispose();

            return new MyTexture(spriteSheet);
        }

        public static MyTexture GenerateRainSprite(int width, int height)
        {
            List<RainDrop> rainDrops = new List<RainDrop>();
            List<Color[]> colorDatas = new List<Color[]>();

            MyTexture rainTexture = ResourceManager.GetTexture("rain_drop");

            colorDatas.Add(rainTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(rainTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(rainTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(rainTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(rainTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(rainTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(rainTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(rainTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(rainTexture.GetSubtexture(0, 0, 16, 16).GetData());
            colorDatas.Add(rainTexture.GetSubtexture(0, 16, 16, 16).GetData());
            colorDatas.Add(rainTexture.GetSubtexture(16, 16, 16, 16).GetData());
            colorDatas.Add(rainTexture.GetSubtexture(32, 16, 16, 16).GetData());
            colorDatas.Add(rainTexture.GetSubtexture(48, 16, 16, 16).GetData());

            for (int i = 0; i < width / 4; i++)
                rainDrops.Add(new RainDrop(MyRandom.Range(width), MyRandom.Range(height), MyRandom.Range(13))); 

            Texture2D[] frames = new Texture2D[12];

            for (int i = 0; i < frames.Length; i++)
            {
                Color[,] data = new Color[width, height];

                foreach (RainDrop drop in rainDrops)
                {
                    Color[] colorData = colorDatas[drop.LifeStage];

                    for (int x = 0; x < 16; x++)
                    {
                        for (int y = 0; y < 16; y++)
                        {
                            int dropYPos = (drop.Y + y);
                            int dropXPos = (drop.X + x);

                            if (dropYPos < 0)
                                dropYPos += height;
                            else if (dropYPos >= height)
                                dropYPos -= height;

                            if (dropXPos < 0)
                                dropXPos += width;
                            else if (dropXPos >= width)
                                dropXPos -= width;


                            data[dropXPos, dropYPos] = colorData[y * 16 + x];
                        }
                    }
                }

                // Move rain drops to the next life stage
                foreach (RainDrop rainDrop in rainDrops)
                    rainDrop.NextLifeStage();

                frames[i] = CreateTexture(Engine.Instance.GraphicsDevice, data);
            }

            Texture2D spriteSheet = new Texture2D(Engine.Instance.GraphicsDevice, width, height * frames.Length);

            Color[] spriteSheetData = new Color[width * (height * frames.Length)];

            for(int i = 0; i < frames.Length; i++)
            {
                Color[] frameData = new Color[width * height];

                frames[i].GetData(frameData);

                for (int j = 0; j < frameData.Length; j++)
                    spriteSheetData[j + (width * height) * i] = frameData[j];

            }
            
            spriteSheet.SetData(spriteSheetData);

            Stream stream = File.Create("rain_file.png");
            spriteSheet.SaveAsPng(stream, spriteSheet.Width, spriteSheet.Height);
            stream.Dispose();

            return new MyTexture(spriteSheet);
        }


        private static Texture2D CreateTexture(GraphicsDevice device, Color[,] data)
        {
            //initialize a texture
            Texture2D texture = new Texture2D(device, data.GetLength(0), data.GetLength(1));

            //set the color
            texture.SetData(ConvertTo1D(data));

            return texture;
        }

        private static Color[] ConvertTo1D(Color[,] data)
        {
            Color[] data1D = new Color[data.GetLength(0) * data.GetLength(1)];

            for(int x = 0; x < data.GetLength(0); ++x)
            {
                for(int y = 0; y < data.GetLength(1); ++y)
                {
                    data1D[y * data.GetLength(0) + x] = data[x, y];
                }
            }

            return data1D;
        }

    }
}
