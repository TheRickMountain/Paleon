using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class AnimatedSprite : Sprite
    {

        public bool Animate;

        private Dictionary<string, Animation> animations;
        public Animation CurrentAnimation { get; private set; }
        public string CurrentAnimationKey { get; private set; }

        public AnimatedSprite(int width, int height) : base(null, width, height, true)
        {
            animations = new Dictionary<string, Animation>(StringComparer.OrdinalIgnoreCase);
            CurrentAnimationKey = "";
            Animate = true;
        }

        public void Add(string id, Animation animation)
        {
            animations.Add(id, animation);
        }

        public void Replace(string id, Animation animation)
        {
            animations[id] = animation;
            if(CurrentAnimationKey == id)
            {
                CurrentAnimation = animation;
            }
        }

        public Animation GetAnimation(string id)
        {
            return animations[id];
        }

        public void Play(string id)
        {
            if (CurrentAnimationKey == id)
                return;

            CurrentAnimationKey = id;
            CurrentAnimation = animations[CurrentAnimationKey];
        }

        public void Reset()
        {
            CurrentAnimation?.Reset();
        }

        public override void Update()
        {
            if(Animate && CurrentAnimation != null)
            {
                CurrentAnimation.Update();
                Texture = CurrentAnimation.Frames[CurrentAnimation.CurrentFrame];
            }
        }

    }
}
