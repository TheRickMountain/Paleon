using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Technolithic
{
    public class WorldMapUI : MyPanelUI
    {
        public event Action CloseButtonPressed;

        private SmallButton _closeButton;

        private WorldSettings _worldSettings;

        private FastNoiseLite _continentNoise;

        private MyText _regionSizeText;
        private MNode _regionSize128Node;
        private MNode _regionSize256Node;

        private TImageUI _worldMap;
        private TImageUI _regionSelector;
        private TImageUI _selectedRegion;

        private float[,] _globalHeightMap;
        private float[,] _regionHeightMap;

        private float _groundHeight = 0.33f;

        private int _regionSize = 128;

        public WorldMapUI(Scene scene) : base(scene, Localization.GetLocalizedText("world_map"), Color.White)
        {
            _continentNoise = new FastNoiseLite();

            _regionSizeText = new MyText(scene);
            _regionSizeText.Text = "Region size:"; // TODO: localize
            _regionSizeText.X = 8;
            _regionSizeText.Y = 40;
            _regionSizeText.Width = _regionSizeText.TextWidth;
            _regionSizeText.Height = _regionSizeText.TextHeight;
            AddChildNode(_regionSizeText);

            _regionSize128Node = CreateRegionSizeNode(128);
            _regionSize128Node.X = _regionSizeText.LocalX + _regionSizeText.Width + 5;
            _regionSize128Node.Y = 40;
            _regionSize128Node.GetChildByName("Toggle").GetComponent<ToggleScript>().SilentCheck(true);
            AddChildNode(_regionSize128Node);

            _regionSize256Node = CreateRegionSizeNode(256);
            _regionSize256Node.X = _regionSize128Node.LocalX + _regionSize128Node.Width + 5;
            _regionSize256Node.Y = 40;
            AddChildNode(_regionSize256Node);

            _worldMap = new TImageUI(scene);
            _worldMap.Width = 512;
            _worldMap.Height = 512;
            _worldMap.X = 8;
            _worldMap.Y = 32 + 40;
            AddChildNode(_worldMap);

            _regionSelector = new TImageUI(scene);
            _regionSelector.Width = 16;
            _regionSelector.Height = 16;
            _regionSelector.SelfColor = Color.Yellow * 0.5f;
            _worldMap.AddChildNode(_regionSelector);

            _selectedRegion = new TImageUI(scene);
            _selectedRegion.Width = 16;
            _selectedRegion.Height = 16;
            _selectedRegion.SelfColor = Color.Orange * 0.5f;
            _selectedRegion.Active = false;
            _worldMap.AddChildNode(_selectedRegion);

            Width = 512 + 16;
            Height = 512 + 16 + 64;

            _closeButton = new SmallButton(scene, ResourceManager.CancelIcon);
            _closeButton.X = Width - _closeButton.Width;
            _closeButton.Y = 0;
            _closeButton.GetComponent<ButtonScript>().AddOnClickedCallback((x, y) =>
            {
                CloseButtonPressed?.Invoke();
            });
            AddChildNode(_closeButton);
        }

        public override void Update(int mouseX, int mouseY)
        {
            if (Active == false) return;

            base.Update(mouseX, mouseY);

            if (_worldMap.Intersects(mouseX, mouseY))
            {
                _regionSelector.X = (mouseX - _worldMap.X) - _regionSelector.Width / 2;
                _regionSelector.Y = (mouseY - _worldMap.Y) - _regionSelector.Height / 2;

                if (MInput.Mouse.PressedLeftButton)
                {
                    _selectedRegion.X = _regionSelector.LocalX;
                    _selectedRegion.Y = _regionSelector.LocalY;
                    _selectedRegion.Width = _regionSelector.Width;
                    _selectedRegion.Height = _regionSelector.Height;
                    _selectedRegion.Active = true;

                    _regionHeightMap = new float[_regionSize, _regionSize];

                    for (int x = 0; x < _regionSize; x++)
                    {
                        for (int y = 0; y < _regionSize; y++)
                        {
                            float subX = _selectedRegion.LocalX + (_selectedRegion.Width / (float)_regionSize) * x;
                            float subY = _selectedRegion.LocalY + (_selectedRegion.Height / (float)_regionSize) * y;

                            float heightValue = _globalHeightMap[(int)subX, (int)subY];

                            _regionHeightMap[x, y] = heightValue;
                        }
                    }

                    _regionHeightMap = SmoothHeightMap(_regionHeightMap, 3.5f);
                }
            }

            // TODO: temp
            if(MInput.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                _worldSettings.HeightMap = _regionHeightMap;
                _worldSettings.GroundHeight = _groundHeight;

                Engine.Scene = new GameplayScene(_worldSettings);
            }
        }

        public void SetWorldSettings(WorldSettings settings)
        {
            _worldSettings = settings;

            int worldMapWidth = 512;
            int worldMapHeight = 512;

            _globalHeightMap = GenerateHeightMap(worldMapWidth, worldMapHeight, settings.Seed);

            MyTexture texture = ConvertHeightMapToTexture(_globalHeightMap);

            if (_worldMap.Texture != null && _worldMap.Texture != RenderManager.Pixel)
            {
                _worldMap.Texture.Unload();
            }

            _worldMap.Texture = texture;
        }

        private MNode CreateRegionSizeNode(int regionSize)
        {
            MNode element = new MNode(Scene);

            MToggleUI toggle = new MToggleUI(Scene, false, true);
            toggle.Name = "Toggle";
            toggle.GetComponent<ToggleScript>().AddOnValueChangedCallback(OnRegionSizeChanged);
            element.AddChildNode(toggle);

            MyText itemName = new MyText(Scene);
            itemName.Text = regionSize + "x" + regionSize;
            itemName.Width = itemName.TextWidth;
            itemName.Height = itemName.TextHeight;
            itemName.X = toggle.Width + 5;
            element.AddChildNode(itemName);

            element.Width = toggle.Width + 5 + itemName.Width;
            element.Height = 34;

            return element;
        }

        private void OnRegionSizeChanged(bool obj1, MToggleUI obj2)
        {
            if (obj2.ParentNode == _regionSize128Node)
            {
                if (_regionSize == 128)
                {
                    _regionSize128Node.GetChildByName("Toggle").GetComponent<ToggleScript>().SilentCheck(true);
                    return;
                }

                _regionSize256Node.GetChildByName("Toggle").GetComponent<ToggleScript>().SilentCheck(false);

                _regionSelector.Width = 16;
                _regionSelector.Height = 16;

                _regionSize = 128;
            }
            else if(obj2.ParentNode == _regionSize256Node)
            {
                if (_regionSize == 256)
                {
                    _regionSize256Node.GetChildByName("Toggle").GetComponent<ToggleScript>().SilentCheck(true);
                    return;
                }

                _regionSize128Node.GetChildByName("Toggle").GetComponent<ToggleScript>().SilentCheck(false);

                _regionSelector.Width = 32;
                _regionSelector.Height = 32;

                _regionSize = 256;
            }
        }

        private MyTexture ConvertHeightMapToTexture(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            MyTexture texture = new MyTexture(new Texture2D(Engine.Instance.GraphicsDevice, width, height));

            Color[] colorData = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int idx = y * width + x;

                    float heightValue = heightMap[x, y];

                    if (heightValue > _groundHeight)
                    {
                        if (heightValue > _groundHeight + 0.02)
                        {
                            colorData[idx] = Color.ForestGreen;
                        }
                        else
                        {
                            colorData[idx] = Color.SandyBrown;
                        }
                    }
                    else
                    {
                        colorData[idx] = Color.Navy;
                    }
                }
            }

            texture.Texture.SetData(colorData);

            return texture;
        }

        private float[,] GenerateHeightMap(int width, int height, int seed)
        {
            _continentNoise.SetSeed(seed);
            _continentNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            _continentNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            _continentNoise.SetFrequency(0.01f);

            float[,] heightMap = new float[width, height];
            float centerX = width / 2f;
            float centerY = height / 2f;
            float maxDistance = Math.Min(width, height) * 0.9f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float distanceFromCenter = (float)Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));

                    float centerGradient = Math.Max(0, 1.0f - (distanceFromCenter / maxDistance));
                    centerGradient = (float)Math.Pow(centerGradient, 1.5);

                    float continentHeight = _continentNoise.GetNoise(x, y);
                    continentHeight = (continentHeight + 1.0f) * 0.5f;

                    float finalHeight = continentHeight;

                    finalHeight *= centerGradient;

                    heightMap[x, y] = MathHelper.Clamp(finalHeight, 0f, 1f);
                }
            }

            return heightMap;
        }

        private float[,] SmoothHeightMap(float[,] heightMap, float smoothingStrength)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            if (smoothingStrength <= 0f) return heightMap;

            float[,] smoothed = new float[width, height];

            int smoothRadius = Math.Max(1, (int)Math.Ceiling(smoothingStrength));

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float sum = 0f;
                    float weightSum = 0f;

                    for (int dx = -smoothRadius; dx <= smoothRadius; dx++)
                    {
                        for (int dy = -smoothRadius; dy <= smoothRadius; dy++)
                        {
                            int nx = x + dx;
                            int ny = y + dy;

                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                                float weight = (float)Math.Exp(-(distance * distance) / (2 * smoothingStrength * smoothingStrength));

                                sum += heightMap[nx, ny] * weight;
                                weightSum += weight;
                            }
                        }
                    }

                    if (weightSum > 0)
                    {
                        float smoothedValue = sum / weightSum;

                        float blendFactor = Math.Min(1.0f, smoothingStrength);
                        smoothed[x, y] = MathHelper.Lerp(heightMap[x, y], smoothedValue, blendFactor);
                    }
                    else
                    {
                        smoothed[x, y] = heightMap[x, y];
                    }
                }
            }

            return smoothed;
        }
    }
}
