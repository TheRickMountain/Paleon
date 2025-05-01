using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Technolithic
{
    public class Entity
    {
        public bool Active = true;
        public bool Visible = true;
        public bool IsRemoved { get; private set; } = false;
        public Vector2 Position;

        public ComponentList Components { get; private set; }

        public float X
        {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        public Layer Layer
        {
            get; internal set;
        }

        public Entity()
        {
            Components = new ComponentList(this);
        }

        public void Awake()
        {
            Components.Awake();
        }

        public void Begin()
        {
            Components.Begin();
        }

        public void Update()
        {
            Components.Update();
        }

        public void Render()
        {
            Components.Render();
        }

        public void Add(Component component)
        {
            Components.Add(component);
        }

        public void Add(params Component[] components)
        {
            Components.Add(components);
        }

        public void Remove(Component component)
        {
            Components.Remove(component);
        }

        public void Remove(params Component[] components)
        {
            Components.Remove(components);
        }

        public void RemoveSelf()
        {
            IsRemoved = true;

            Layer.Remove(this);

            Layer = null;
        }

        public T Get<T>() where T : Component
        {
            return Components.Get<T>();
        }

        public bool Has<T>() where T : Component
        {
            if (Components.Get<T>() != null)
                return true;

            return false;
        }

        internal int CompareTo(Entity entityB)
        {
            if (entityB.Y < Y)
                return 1;
            else if(entityB.Y > Y)
                return -1;
            else
            {
                if(entityB.GetHashCode() > GetHashCode())
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}
