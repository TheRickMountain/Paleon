using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class Layer
    {
        /// <summary>
        /// Layer's parent scene
        /// </summary>
        public Scene Scene { get; private set; }

        /// <summary>
        /// Layer'n name used for searching
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// If false, layer won't be rendered
        /// </summary>
        public bool Visible = true;

        /// <summary>
        /// If true, layer won't be updated
        /// </summary>
        public bool Active = true;

        public EntityList Entities { get; private set; }

        public Layer(Scene scene, string name)
        {
            Scene = scene;
            Name = name;

            Entities = new EntityList(this);
        }

        public virtual void UpdateLists()
        {
            Entities.UpdateLists();
        }

        public virtual void Begin()
        {
                       
        }

        public virtual void Update()
        {
            Entities.Update();
        }

        public virtual void Render()
        {
            Entities.Render();
        }

        public void Add(Entity entity)
        {
            Entities.Add(entity);
            entity.Layer = this;
        }

        public void Remove(Entity entity)
        {
            Entities.Remove(entity);
            entity.Layer = null;
        }

    }
}
