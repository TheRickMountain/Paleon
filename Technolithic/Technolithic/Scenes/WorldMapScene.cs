using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace Technolithic
{
    public class WorldMapScene : Scene
    {
        private const int WORLD_MAP_SIZE = 512;
        private const float GROUND_HEIGHT = 0.33f;
        private const int TILES_PER_PIXEL = 8;

        private WorldSettings _worldSettings;

        private float[,] _globalHeightMap;
        private MyTexture _worldMap;

        private GameplayCamera _camera;

        private Vector2 _mouseWorldPosition;

        private MNode _uiNode;
        private WorldMapRegionSizeSelectorUI _regionSizeSelectorUI;

        private TShortTextButtonUI _cancelButton;
        private TShortTextButtonUI _startButton;

        private int _currentRegionSize = 128;

        private Rectangle _currentRegion;
        private Rectangle _selectedRegion = Rectangle.Empty;

        private Color _regionSelectorColor = Color.White;

        public WorldMapScene(WorldSettings worldSettings)
        {
            _worldSettings = worldSettings;

            _uiNode = new MNode(this);

            _regionSizeSelectorUI = new WorldMapRegionSizeSelectorUI(this);
            _regionSizeSelectorUI.RegionSizeChanged += RegionSizeSelectorUI_RegionSizeChanged;
            _uiNode.AddChildNode(_regionSizeSelectorUI);

            _cancelButton = new TShortTextButtonUI(this);
            _cancelButton.SetButtonColor(Color.IndianRed);
            _cancelButton.Text = Localization.GetLocalizedText("cancel");
            _cancelButton.ButtonUp += _cancelButton_ButtonPressed;

            _startButton = new TShortTextButtonUI(this);
            _startButton.SetButtonColor(Color.YellowGreen);
            _startButton.Text = Localization.GetLocalizedText("start");
            _startButton.ButtonUp += _startButton_ButtonPressed;

            worldSettings.GroundHeight = GROUND_HEIGHT;

            _globalHeightMap = GenerateHeightMap(WORLD_MAP_SIZE, WORLD_MAP_SIZE, worldSettings.Seed);

            _worldMap = ConvertHeightMapToTexture(_globalHeightMap, GROUND_HEIGHT);

            RenderManager.MainCamera.Position = new Vector2(WORLD_MAP_SIZE / 2, WORLD_MAP_SIZE / 2);
            _camera = new GameplayCamera(true);
            _camera.Get<CameraMovementScript>().Bounds = new Rectangle(0, 0, WORLD_MAP_SIZE, WORLD_MAP_SIZE);
            _camera.Awake();
            _camera.Begin();

            UpdateNodesPositions();
        }

        private void _cancelButton_ButtonPressed(TButtonUI obj)
        {
            _uiNode.AddChildNode(_regionSizeSelectorUI);

            _uiNode.RemoveChild(_cancelButton);
            _uiNode.RemoveChild(_startButton);

            _selectedRegion = Rectangle.Empty;
        }

        private void _startButton_ButtonPressed(TButtonUI obj)
        {
            float[,] regionHeightMap = new float[_currentRegionSize, _currentRegionSize];

            for (int x = 0; x < _currentRegionSize; x++)
            {
                for (int y = 0; y < _currentRegionSize; y++)
                {
                    float subX = _selectedRegion.X + (_selectedRegion.Width / (float)_currentRegionSize) * x;
                    float subY = _selectedRegion.Y + (_selectedRegion.Height / (float)_currentRegionSize) * y;

                    float heightValue = _globalHeightMap[(int)subX, (int)subY];

                    regionHeightMap[x, y] = heightValue;
                }
            }

            regionHeightMap = SmoothHeightMap(regionHeightMap, 3.5f);

            _worldSettings.HeightMap = regionHeightMap;

            Engine.Scene = new GameplayScene(_worldSettings);
        }

        public override void Begin()
        {
            _uiNode.Awake();

            _uiNode.Begin();

            Engine.Instance.Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            UpdateNodesPositions();
        }

        private void UpdateNodesPositions()
        {
            _regionSizeSelectorUI.X = 5;
            _regionSizeSelectorUI.Y = 5;

            _cancelButton.X = Engine.Width / 2 - (_cancelButton.Width + 5);
            _cancelButton.Y = Engine.Height - _cancelButton.Height - 5;

            _startButton.X = Engine.Width / 2 + 5;
            _startButton.Y = Engine.Height - _startButton.Height - 5;
        }

        public override void Update()
        {
            UpdateMouseWorldPosition();

            _camera.Update();

            if(_selectedRegion.IsEmpty)
            {
                Vector2 regionSelectorSize = new Vector2(_currentRegionSize / TILES_PER_PIXEL);
                Vector2 regionSelectorPosition = new Vector2((int)_mouseWorldPosition.X, (int)_mouseWorldPosition.Y);

                regionSelectorPosition = Vector2.Clamp(regionSelectorPosition, Vector2.Zero, new Vector2(WORLD_MAP_SIZE) - regionSelectorSize);

                _currentRegion = new Rectangle(regionSelectorPosition.ToPoint(), regionSelectorSize.ToPoint());

                float groundPercent = GetRegionGroundPercent(_currentRegion, _globalHeightMap);

                if (groundPercent <= 0)
                {
                    _regionSelectorColor = Color.Red;
                    GlobalUI.ShowTooltips(Localization.GetLocalizedText("it_is_impossible_to_land_on_water").Paint(Color.Red));
                }
                else
                {
                    _regionSelectorColor = Color.White;
                }

                if (groundPercent > 0 && _regionSizeSelectorUI.Intersects(MInput.Mouse.X, MInput.Mouse.Y) == false)
                {
                    if (_selectedRegion == Rectangle.Empty && MInput.Mouse.ReleasedLeftButton
                        && new Rectangle(0, 0, WORLD_MAP_SIZE, WORLD_MAP_SIZE).Contains(_mouseWorldPosition))
                    {
                        _selectedRegion = _currentRegion;

                        _uiNode.RemoveChild(_regionSizeSelectorUI);

                        _uiNode.AddChildNode(_cancelButton);
                        _uiNode.AddChildNode(_startButton);
                    }
                }
            }

            _uiNode.Update(MInput.Mouse.X, MInput.Mouse.Y);

            base.Update();
        }

        public override void Render()
        {
            base.Render();

            RenderManager.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null, null, null, RenderManager.MainCamera.Transformation);

            _worldMap.Draw(new Vector2(0, 0));

            if(_selectedRegion != Rectangle.Empty)
            {
                RenderManager.Rect(_selectedRegion, Color.Yellow * 0.5f);
            }
            else
            {
                RenderManager.Rect(_currentRegion, _regionSelectorColor * 0.5f);
            }
                
            RenderManager.SpriteBatch.End();

            RenderManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null,
                Engine.RasterizerState);

            _uiNode.Render();

            RenderManager.SpriteBatch.End();
        }

        private float GetRegionGroundPercent(Rectangle regionRect, float[,] heightMap)
        {
            int totalMapTiles = regionRect.Width * regionRect.Height;

            int totalGroundTiles = 0;

            for (int x = _currentRegion.X; x < _currentRegion.Right; x++)
            {
                for (int y = _currentRegion.Y; y < _currentRegion.Bottom; y++)
                {
                    float heightValue = heightMap[x, y];

                    if(heightValue >= GROUND_HEIGHT)
                    {
                        totalGroundTiles++;
                    }
                }
            }

            return totalGroundTiles / (float)totalMapTiles;
        }

        private void RegionSizeSelectorUI_RegionSizeChanged(int regionSize)
        {
            _currentRegionSize = regionSize;
        }

        private float[,] GenerateHeightMap(int width, int height, int seed)
        {
            FastNoiseLite noise = new FastNoiseLite(seed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFrequency(0.01f);

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

                    float continentHeight = noise.GetNoise(x, y);
                    continentHeight = (continentHeight + 1.0f) * 0.5f;

                    float finalHeight = continentHeight;

                    finalHeight *= centerGradient;

                    heightMap[x, y] = MathHelper.Clamp(finalHeight, 0f, 1f);
                }
            }

            return heightMap;
        }

        private MyTexture ConvertHeightMapToTexture(float[,] heightMap, float groundHeight)
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

                    if (heightValue > groundHeight)
                    {
                        if (heightValue > groundHeight + 0.02)
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

        private void UpdateMouseWorldPosition()
        {
            float mposx = (MInput.Mouse.X - Engine.HalfWidth) / RenderManager.MainCamera.Zoom;
            float mposy = (MInput.Mouse.Y - Engine.HalfHeight) / RenderManager.MainCamera.Zoom;

            _mouseWorldPosition = new Vector2((int)mposx + RenderManager.MainCamera.Position.X, (int)mposy + RenderManager.MainCamera.Position.Y);
        }
    }
}
