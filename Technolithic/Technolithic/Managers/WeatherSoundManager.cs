using Microsoft.Xna.Framework.Audio;
using System;

namespace Technolithic
{

    public enum SoundSwitchState
    {
        WAITING_FOR_NEW_SOUND,
        SWITCHING_TO_NEW_SOUND
    }

    public class WeatherSoundManager : IDisposable
    {

        private WorldState worldState;

        private SoundEffectInstance daySoundEffect;
        private SoundEffectInstance nightSoundEffect;
        private SoundEffectInstance rainSoundEffect;
        private SoundEffectInstance winterSoundEffect;

        private SoundEffectInstance lastSoundEffect;
        private SoundEffectInstance newSoundEffect;

        private float switchingTimer;
        private float switchTime = 10.0f;

        private SoundSwitchState soundSwitchState;

        public WeatherSoundManager(WorldState worldState)
        {
            this.worldState = worldState;
        }

        public void Begin()
        {
            daySoundEffect = CreateSoundEffect("day_sound", true);
            nightSoundEffect = CreateSoundEffect("night_sound", true);
            rainSoundEffect = CreateSoundEffect("rain_sound", true);
            winterSoundEffect = CreateSoundEffect("winter_sound", true);

            lastSoundEffect = GetNextWeatherSound();
            lastSoundEffect.Play();
        }

        private SoundEffectInstance CreateSoundEffect(string name, bool loop)
        {
            var soundEffect = ResourceManager.GetSoundEffect(name).CreateInstance();
            soundEffect.IsLooped = loop;
            return soundEffect;
        }

        public void Update()
        {
            switch (soundSwitchState)
            {
                case SoundSwitchState.WAITING_FOR_NEW_SOUND:
                    {
                        newSoundEffect = GetNextWeatherSound();
                        if (lastSoundEffect != newSoundEffect)
                        {
                            newSoundEffect.Volume = 0.0f;
                            newSoundEffect.Play();

                            soundSwitchState = SoundSwitchState.SWITCHING_TO_NEW_SOUND;
                        }
                    }
                    break;
                case SoundSwitchState.SWITCHING_TO_NEW_SOUND:
                    {
                        if (switchingTimer < switchTime)
                        {
                            float alpha = switchingTimer / switchTime;

                            lastSoundEffect.Volume = MathUtils.Lerp(1.0f, 0.0f, alpha);
                            newSoundEffect.Volume = MathUtils.Lerp(0.0f, 1.0f, alpha);

                            switchingTimer += Engine.GameDeltaTime;
                        }
                        else
                        {
                            switchingTimer = 0.0f;

                            lastSoundEffect.Stop();
                            lastSoundEffect = newSoundEffect;
                            lastSoundEffect.Volume = 1.0f;

                            soundSwitchState = SoundSwitchState.WAITING_FOR_NEW_SOUND;
                        }
                    }
                    break;
            }
        }

        private SoundEffectInstance GetNextWeatherSound()
        {
            var timeOfDay = worldState.GetCurrentHourTimeOfDay();
            var season = worldState.CurrentSeason;
            var weather = worldState.CurrentWeather;

            if (timeOfDay == TimeOfDay.Day)
            {
                if (season == Season.Winter)
                {
                    if (weather == Weather.Precipitation)
                    {
                        return winterSoundEffect;
                    }
                    else if (weather == Weather.Sun)
                    {
                        return daySoundEffect;
                    }
                }
                else
                {
                    if (weather == Weather.Precipitation)
                    {
                        return rainSoundEffect;
                    }
                    else if (weather == Weather.Sun)
                    {
                        return daySoundEffect;
                    }
                }
            }
            else if (timeOfDay == TimeOfDay.Night)
            {
                if (season == Season.Winter)
                {
                    return winterSoundEffect;
                }
                else
                {
                    if (weather == Weather.Precipitation)
                    {
                        return rainSoundEffect;
                    }
                    else if (weather == Weather.Sun)
                    {
                        return nightSoundEffect;
                    }
                }
            }

            return null;
        }

        public void Dispose()
        {
            daySoundEffect?.Dispose();
            nightSoundEffect?.Dispose();
            rainSoundEffect?.Dispose();
            winterSoundEffect?.Dispose();
        }
    }
}
