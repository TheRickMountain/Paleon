using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MTextInput : MNode
    {
        public MTextInput(Scene scene) : base(scene)
        {
            MImageCmp mImageCmp = new MImageCmp();

            mImageCmp.Texture = TextureBank.UITexture.GetSubtexture(112, 192, 24, 24);
            mImageCmp.ImageType = ImageType.Sliced;
            mImageCmp.SetBorder(8, 8, 8, 8);
            mImageCmp.BackgroundOverlap = 2;

            AddComponent(mImageCmp);
            AddComponent(new MTextInputScript());
        }
    }
}
