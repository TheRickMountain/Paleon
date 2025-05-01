using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AudioSourceCmp : Component
    {

        private Dictionary<string, SoundEffectInstance> soundEffects;

        private AudioEmitter audioEmitter;
        private AudioListener audioListener;

        private string lastPlayedSoundEffect;

        public AudioSourceCmp() : base(true, false)
        {
            soundEffects = new Dictionary<string, SoundEffectInstance>();

            audioEmitter = new AudioEmitter();
            audioListener = new AudioListener();
        }

        public void Play(string soundEffectName)
        {
            lastPlayedSoundEffect = soundEffectName;

            int x = (int)((Entity.X - RenderManager.MainCamera.Position.X) / Engine.TILE_SIZE);

            if (x <= 2 && x >= 0)
                x = 2;
            else if (x >= -2 && x <= 0)
                x = -2;

            int y = (int)((Entity.Y - RenderManager.MainCamera.Position.Y) / Engine.TILE_SIZE);

            if (y <= 2 && y >= 0)
                y = 2;
            else if (y >= -2 && y <= 0)
                y = -2;

            int zoom = (int)(4 - RenderManager.MainCamera.Zoom) * 6;
            audioEmitter.Position = new Vector3(x, zoom, y);

            SoundEffectInstance soundEffect = soundEffects[soundEffectName];

            soundEffect.Apply3D(audioListener, audioEmitter);
            soundEffect.Play();
        }

        public void Stop()
        {
            if (lastPlayedSoundEffect == null)
                return;

            SoundEffectInstance soundEffect = soundEffects[lastPlayedSoundEffect];
            soundEffect.Stop();
        }

        public void AddSoundEffect(string soundEffectName, SoundEffect soundEffect)
        {
            soundEffects.Add(soundEffectName, soundEffect.CreateInstance());
        }
    }
}
