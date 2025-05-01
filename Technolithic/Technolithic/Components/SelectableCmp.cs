using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{

    public enum SelectableType
    {
        Building,
        Settler,
        Animal, 
        ItemContainers,
        Trader
    }

    public class SelectableCmp : Component
    {

        private Vector2 position;

        private Rectangle boundingBox;
        public Rectangle BoundingBox
        {
            get
            {
                Vector2 newPos = Entity != null ? Entity.Position + position : position;
                boundingBox.X = (int)newPos.X;
                boundingBox.Y = (int)newPos.Y;
                return boundingBox;
            }
        }

        public SelectableType SelectableType { get; private set; }

        public SelectableCmp(int offsetX, int offsetY, int width, int height, SelectableType selectableType) : base(false, false)
        {
            position = new Vector2(offsetX, offsetY);
            boundingBox = new Rectangle(0, 0, width, height);
            SelectableType = selectableType;
        }

        public bool Intersects(int x, int y)
        {
            return BoundingBox.Contains(new Point(x, y));
        }

        public bool Intersects(Rectangle rectangle)
        {
            return BoundingBox.Intersects(rectangle);
        }
    }
}
