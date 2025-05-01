using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MLineCmp : MComponent
    {

        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
        public Color Color { get; set; }
        public float Thickness { get; set; }
        public float OutlineThickness { get; set; }
        public Color OutlineColor { get; set; } = Color.Black;

        public bool Outlined { get; private set; }

        public MLineCmp(Vector2 start, Vector2 end, Color color, float thickness, bool outlined) : base(false, true)
        {
            Start = start;
            End = end;
            Color = color;
            Thickness = thickness;
            Outlined = outlined;
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
        }

        public override void Render()
        {
            if (Visible)
            {
                if (Outlined)
                    RenderManager.Line(Start.X + ParentNode.X, Start.Y + ParentNode.Y, End.X + ParentNode.X, End.Y + ParentNode.Y, OutlineColor, OutlineThickness);

                RenderManager.Line(Start.X + ParentNode.X, Start.Y + ParentNode.Y, End.X +ParentNode.X, End.Y + ParentNode.Y, Color, Thickness);
            }
        }

        public override void Update(int mouseX, int mouseY)
        {
        }
    }
}
