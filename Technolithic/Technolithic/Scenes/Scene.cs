using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public abstract class Scene
    {
        private List<Layer> layers;

        public Action<Scene> SceneEnded { get; set; }

        public Scene()
        {
            layers = new List<Layer>();
        }

        public virtual void Initialize()
        {

        }

        public virtual void UpdateLists()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].UpdateLists();
            }
        }

        public virtual void Begin()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].Begin();
            }
        }

        public virtual void Update()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].Update();
            }
        }

        public virtual void Render()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].Render();
            }
        }

        public virtual void End()
        {
            SceneEnded?.Invoke(this);
        }
        
        public Layer CreateLayer(string name)
        {
            Layer layer = new Layer(this, name);
            layers.Add(layer);
            return layer;
        }

    }
}
