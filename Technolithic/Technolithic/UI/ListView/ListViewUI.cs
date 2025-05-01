using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ListViewUI : MNode
    {

        public ListViewUI(Scene scene, int elementWidth, int elementHeight, int rowsCount, int columnsCount = 1, bool scrollable = true, 
            bool showBackground = true, bool scrollerLeft = false) : base(scene)
        {
            Width = (elementWidth * columnsCount) + (5 * (columnsCount - 1)) + 12;
            Height = (elementHeight * rowsCount) + (5 * (rowsCount - 1));

            if (showBackground)
            {
                MImageUI background = new MImageUI(scene);
                background.Width = Width;
                background.Height = Height;
                background.GetComponent<MImageCmp>().Texture = RenderManager.Pixel;
                background.GetComponent<MImageCmp>().Color = Color.Black * 0.25f;
                AddChildNode(background);
            }

            if (scrollable)
            {
                MImageUI scrollerPath = new MImageUI(scene);
                scrollerPath.Width = 8;
                scrollerPath.Height = Height;

                if(scrollerLeft)
                    scrollerPath.X = 0;
                else
                    scrollerPath.X = Width - 8;

                scrollerPath.Y = 0;
                scrollerPath.GetComponent<MImageCmp>().Texture = RenderManager.Pixel;
                scrollerPath.GetComponent<MImageCmp>().Color = Color.Black;
                scrollerPath.Name = "ScrollerPath";
                AddChildNode(scrollerPath);

                MImageUI scroller = new MImageUI(scene);
                scroller.Width = 8;
                scroller.Height = Height;
                scroller.X = scrollerPath.LocalX;
                scroller.Y = scrollerPath.LocalY;
                scroller.GetComponent<MImageCmp>().Texture = RenderManager.Pixel;
                scroller.GetComponent<MImageCmp>().Color = Color.DarkGray;
                scroller.Name = "Scroller";
                AddChildNode(scroller);
            }

            AddComponent(new ListViewUIScript(elementHeight, elementWidth, rowsCount, columnsCount, scrollable));
        }

    }
}
