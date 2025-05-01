using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MSpriteCmp : MImageCmp
    {

        public bool Animate;

        private Dictionary<string, Animation> animations;
        public Animation CurrentAnimation { get; private set; }
        public string CurrentAnimationKey { get; private set; }

        public MSpriteCmp() 
        {
            animations = new Dictionary<string, Animation>(StringComparer.OrdinalIgnoreCase);
            CurrentAnimationKey = "";
            Animate = true;
            Active = true;
        }

        public void Add(string id, Animation animation)
        {
            animations.Add(id, animation);
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

        public override void Update(int x, int y)
        {
            if (Animate && CurrentAnimation != null)
            {
                CurrentAnimation.Update();
                Texture = CurrentAnimation.Frames[CurrentAnimation.CurrentFrame];
            }
        }

    }
}
