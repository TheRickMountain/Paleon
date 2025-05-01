using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Technolithic
{

    public enum TimeOfDay
    {
        Night,
        Day,
    }

    public enum Weather
    {
        Sun,
        Precipitation
    }

    public enum Season
    {
        Summer = 0,
        Autumn = 1,
        Winter = 2,
        Spring = 3
    }

    public enum WindSpeed
    {
        Calm = 0,
        Light = 1,
        Moderate = 2,
        Strong = 3
    }

    public class WorldState
    {

        public struct HourInfo
        {
            public TimeOfDay TimeOfDay { get; private set; }

            public Color Color { get; private set; }
            public Color RainColor { get; private set; }

            public HourInfo(TimeOfDay timeOfDay, Color color, Color rainColor)
            {
                TimeOfDay = timeOfDay;
                Color = color;
                RainColor = rainColor;
            }
        }

        public Color dayColor { get; private set; } = new Color(255, 255, 255, 255);
        public Color sunsetColor { get; private set; } = new Color(240, 180, 170, 255);
        public Color sunriseColor { get; private set; } = new Color(250, 180, 170, 255);
        public Color nightColor { get; private set; } = new Color(63, 137, 255, 255);
        public Color rainColor { get; private set; } = new Color(145, 202, 222, 255);

        public Weather CurrentWeather { get; private set; }
        public Season CurrentSeason { get; private set; }
        public Season LastSeason { get; private set; }

        public const int MINUTES_PER_HOUR = 25;
        public const int HOURS_PER_CYCLE = 24;
        public const int CYCLES_PER_SEASON = 10;

        public int CurrentDay { get; private set; }
        public int CurrentHour { get; private set; }

        private float currentMinute;

        private HourInfo[] hourInfos = new HourInfo[24];

        public Color TimeOfDayColor { get; private set; }

        public Action<int, Season> OnNextDayStartedCallback { get; set; }
        public Action<int> NextHourStarted { get; set; }

        public float LastSeasonAlpha { get; private set; }
        public float CurrentSeasonAlpha { get; private set; }

        public WindSpeed WindSpeed { get; private set; }

        public WorldState(WorldStateSaveData worldStateSaveData)
        {
            GenerateSchedule();

            if (worldStateSaveData == null)
            {
                CurrentHour = 8; 
                CurrentDay = 1; 

                currentMinute = CurrentHour * MINUTES_PER_HOUR;

                CurrentWeather = Weather.Sun;

                CurrentSeason = Season.Summer;
                LastSeason = Season.Summer;

                LastSeasonAlpha = 0.0f;
                CurrentSeasonAlpha = 1.0f;

                WindSpeed = WindSpeed.Moderate;
            }
            else
            {
                CurrentHour = worldStateSaveData.CurrentHour;
                CurrentDay = worldStateSaveData.CurrentDay;
                currentMinute = worldStateSaveData.CurrentMinute;
                CurrentWeather = worldStateSaveData.CurrentWeather;
                CurrentSeason = worldStateSaveData.CurrentSeason;
                LastSeason = worldStateSaveData.LastSeason;
                CurrentSeasonAlpha = worldStateSaveData.CurrentSeasonAlpha;
                LastSeasonAlpha = worldStateSaveData.LastSeasonAlpha;
                WindSpeed = worldStateSaveData.WindSpeed;
            }
        }

        public float GetDayProgressPercent()
        {
            float totalMinutesCount = HOURS_PER_CYCLE * MINUTES_PER_HOUR;

            return (100 * currentMinute) / totalMinutesCount;
        }

        private void GenerateSchedule()
        {
            hourInfos[0] = new HourInfo(TimeOfDay.Day, nightColor, nightColor);
            hourInfos[1] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[2] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[3] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[4] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[5] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[6] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[7] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[8] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[9] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[10] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[11] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[12] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[13] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[14] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[15] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[16] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[17] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[18] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[19] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[20] = new HourInfo(TimeOfDay.Day, dayColor, rainColor);
            hourInfos[21] = new HourInfo(TimeOfDay.Night, dayColor, rainColor);
            hourInfos[22] = new HourInfo(TimeOfDay.Night, nightColor, nightColor);
            hourInfos[23] = new HourInfo(TimeOfDay.Night, nightColor, nightColor);
        }

        public void Update()
        {
            currentMinute += Engine.GameDeltaTime;

            int newHour = (int)(currentMinute / MINUTES_PER_HOUR);

            if (CurrentHour != newHour)
            {
                CurrentHour = newHour;
                NextHourStarted?.Invoke(CurrentHour);
            }

            if (CurrentHour >= HOURS_PER_CYCLE)
            {
                CurrentHour = 0;
                currentMinute = 0;
                NextDay();
            }

            if(CurrentSeason != LastSeason)
            {
                CurrentSeasonAlpha = MathHelper.Clamp(CurrentSeasonAlpha + Engine.GameDeltaTime * 0.1f, 0.0f, 1.0f);
                LastSeasonAlpha = MathHelper.Clamp(LastSeasonAlpha - Engine.GameDeltaTime * 0.1f, 0.0f, 1.0f);

                if(LastSeasonAlpha <= 0)
                {
                    LastSeason = CurrentSeason;
                }
            }

            TimeOfDayColor = DayColorChanger.GetTimeOfDayColor(currentMinute, MINUTES_PER_HOUR, CurrentWeather, hourInfos);
        }

        private void NextDay()
        {
            CurrentDay++;
            CurrentWeather = MyRandom.Range(100) <= 19 ? Weather.Precipitation : Weather.Sun;

            int seasonsPassedCount = CurrentDay / CYCLES_PER_SEASON;
            int seasonsCount = Enum.GetNames(typeof(Season)).Length;
            int currentSeasonNumber = seasonsPassedCount % seasonsCount;
            LastSeason = CurrentSeason;
            CurrentSeason = (Season)currentSeasonNumber;

            if(LastSeason != CurrentSeason)
            {
                LastSeasonAlpha = 1.0f;
                CurrentSeasonAlpha = 0.0f;
            }

            WindSpeed = MyRandom.GetRandomEnumValue<WindSpeed>();

            UpdateGrassGrowth();

            OnNextDayStartedCallback?.Invoke(CurrentDay, CurrentSeason);

            GameplayScene.Instance.IsAutosaveTriggered = true;

            if (CurrentDay >= 100 && GameplayScene.WorldManager.TotalSettlersCount > 0)
            {
                GameplayScene.Instance.AchievementManager.UnlockAchievement(AchievementId.I_SURVIVED_100_DAYS);
            }
        }

        private void UpdateGrassGrowth()
        {
            List<Tile> tilesForGrowingGrass = new List<Tile>();

            World world = GameplayScene.Instance.World;

            for (int x = 0; x < world.Width; x++)
            {
                for (int y = 0; y < world.Height; y++)
                {
                    Tile tile = world.GetTileAt(x, y);

                    // Трава не может прорасти, если тайл не является обычной землей
                    if (tile.GroundType != GroundType.Ground)
                        continue;

                    // Трава не может прорасти, если поверх находится вода, руды и оросительные каналы
                    if (tile.GroundTopType != GroundTopType.None)
                        continue;

                    // Трава не может прорасти под поверхностями
                    if (tile.SurfaceId != -1)
                        continue;

                    foreach(var neighTile in tile.GetNeighbourTiles())
                    {
                        if(neighTile.GroundType == GroundType.Grass)
                        {
                            tilesForGrowingGrass.Add(tile);
                            break;
                        }
                    }
                }
            }

            foreach(var tile in tilesForGrowingGrass)
            {
                if (MyRandom.ProbabilityChance(50))
                {
                    tile.GroundType = GroundType.Grass;
                }
            }
        }

        public TimeOfDay GetCurrentHourTimeOfDay()
        {
            return hourInfos[CurrentHour].TimeOfDay;
        }

        public float GetWindSpeedModificator()
        {
            switch(WindSpeed)
            {
                case WindSpeed.Calm:
                    return 0.0f;
                case WindSpeed.Light:
                    return 0.3333334f;
                case WindSpeed.Moderate:
                    return 0.6666667f;
                case WindSpeed.Strong:
                    return 1.0f;
            }

            throw new Exception($"Missing return value for enum: {WindSpeed}");
        }

        public WorldStateSaveData GetSaveData()
        {
            WorldStateSaveData worldStateSaveData = new WorldStateSaveData();
            worldStateSaveData.CurrentDay = CurrentDay;
            worldStateSaveData.CurrentHour = CurrentHour;
            worldStateSaveData.CurrentMinute = currentMinute;
            worldStateSaveData.CurrentWeather = CurrentWeather;
            worldStateSaveData.CurrentSeason = CurrentSeason;
            worldStateSaveData.LastSeason = LastSeason;
            worldStateSaveData.CurrentSeasonAlpha = CurrentSeasonAlpha;
            worldStateSaveData.LastSeasonAlpha = LastSeasonAlpha;
            worldStateSaveData.WindSpeed = WindSpeed;

            return worldStateSaveData;
        }

    }
}
