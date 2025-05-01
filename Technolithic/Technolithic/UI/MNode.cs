using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class MNode
    {
        public string Name { get; set; }
        public Scene Scene { get; private set; }
        public bool ClipContent { get; set; } = false;

        private Action<MNode> cbOnIntersects;

        private int x;
        public int X
        {
            get { return x + (ParentNode != null ? ParentNode.X : 0); }
            set
            {
                x = value;
            }
        }

        private int y;
        public int Y
        {
            get { return y + (ParentNode != null ? ParentNode.Y : 0); }
            set
            {
                y = value;
            }
        }

        public Vector2 GetVector2()
        {
            return new Vector2(X, Y);
        }

        public Vector2 GetLocalVector2()
        {
            return new Vector2(LocalX, LocalY);
        }

        private int width;
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
            }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
            }
        }

        public int LocalX
        {
            get { return x; }
        }

        public int LocalY
        {
            get { return y; }
        }

        public bool Active { get; set; } = true;

        public MNode ParentNode { get; private set; }

        public string Tooltips { get; set; } = "";

        private List<MNode> childNodes;
        private List<MNode> childNodesToAdd;

        private List<MComponent> components;
        private List<MComponent> scripts;
        private List<MComponent> componentsToAdd;

        private bool wasAwakened = false;

        private Dictionary<string, object> metadata;

        public MNode(Scene scene)
        {
            Scene = scene;

            childNodes = new List<MNode>();
            childNodesToAdd = new List<MNode>();

            components = new List<MComponent>();
            scripts = new List<MComponent>();
            componentsToAdd = new List<MComponent>();

            metadata = new Dictionary<string, object>();
        }

        public void AddChildNode(MNode node)
        {
            if (childNodes.Contains(node) || childNodesToAdd.Contains(node))
                throw new Exception("Node was added already!");

            node.ParentNode = this;
            childNodesToAdd.Add(node);
        }

        public void RemoveChild(MNode node)
        {
            node.ParentNode = null;
            childNodes.Remove(node);
            childNodesToAdd.Remove(node);
        }

        public void RemoveAllChildren()
        {
            for (int i = childNodes.Count - 1; i >=0; i--)
                RemoveChild(childNodes[i]);

            for (int i = childNodesToAdd.Count - 1; i >= 0; i--)
                RemoveChild(childNodesToAdd[i]);
        }

        public void AddComponent(MComponent component)
        {
            if (components.Contains(component) || componentsToAdd.Contains(component))
                throw new Exception("Component was added already!");

            component.ParentNode = this;
            componentsToAdd.Add(component);
        }

        public void RemoveComponent(MComponent component)
        {
            component.ParentNode = null;

            if (component is MScript)
                scripts.Remove(component);
            else
                components.Remove(component);
        }

        public void Awake()
        {
            for (int i = 0; i < componentsToAdd.Count; i++)
            {
                componentsToAdd[i].Awake();
                componentsToAdd[i].WasAwakened = true;

                MComponent component = componentsToAdd[i];

                if (component is MScript)
                    scripts.Add(component);
                else
                    components.Add(component);
            }

            for (int i = 0; i < childNodesToAdd.Count; i++)
            {
                childNodesToAdd[i].Awake();
                childNodesToAdd[i].wasAwakened = true;
                childNodes.Add(childNodesToAdd[i]);
            }
        }

        public void Begin()
        {
            // Стартуем компоненты прошедшие awake
            for (int i = 0; i < componentsToAdd.Count; i++)
            {
                if (componentsToAdd[i].WasAwakened)
                    componentsToAdd[i].Begin();
            }

            // Удаляем проинициализированные компоненты
            for (int i = componentsToAdd.Count - 1; i >= 0; i--)
            {
                if (componentsToAdd[i].WasAwakened)
                    componentsToAdd.RemoveAt(i);
            }

            // Стартуем ноды прошедшие awake
            for (int i = 0; i < childNodesToAdd.Count; i++)
            {
                if (childNodesToAdd[i].wasAwakened)
                    childNodesToAdd[i].Begin();
            }

            // Удаляем проинициализированные ноды
            for (int i = childNodesToAdd.Count - 1; i >= 0; i--)
            {
                if (childNodesToAdd[i].wasAwakened)
                    childNodesToAdd.RemoveAt(i);
            }
        }

        public virtual void Update(int mouseX, int mouseY)
        {
            if (componentsToAdd.Count > 0 || childNodesToAdd.Count > 0)
            {
                Awake();
                Begin();
            }

            if (Active)
            {
                for (int i = 0; i < scripts.Count; i++)
                {
                    MComponent script = scripts[i];
                    if (script.Active)
                        script.Update(mouseX, mouseY);
                }

                for (int i = 0; i < components.Count; i++)
                {
                    MComponent component = components[i];
                    if (component.Active)
                        component.Update(mouseX, mouseY);
                }

                for (int i = 0; i < childNodes.Count; i++)
                {
                    childNodes[i].Update(mouseX, mouseY);
                }

                if(cbOnIntersects != null || string.IsNullOrEmpty(Tooltips) == false)
                {
                    if (Intersects(mouseX, mouseY))
                    {
                        cbOnIntersects?.Invoke(this);
                    
                        if(string.IsNullOrEmpty(Tooltips) == false)
                        {
                            GlobalUI.ShowTooltips(Tooltips);
                        }
                    }
                }
            }
        }

        public virtual void Render()
        {
            if (Active)
            {
                if (ClipContent)
                {
                    Rectangle currentRect = RenderManager.SpriteBatch.GraphicsDevice.ScissorRectangle;

                    RenderManager.SpriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(X, Y, Width, Height);

                    for (int i = 0; i < components.Count; i++)
                    {
                        MComponent component = components[i];
                        if (component.Visible)
                            component.Render();
                    }

                    for (int i = 0; i < childNodes.Count; i++)
                    {
                        childNodes[i].Render();
                    }

                    RenderManager.SpriteBatch.GraphicsDevice.ScissorRectangle = currentRect;
                }
                else
                {
                    for (int i = 0; i < components.Count; i++)
                    {
                        MComponent component = components[i];
                        if (component.Visible)
                            component.Render();
                    }

                    for (int i = 0; i < childNodes.Count; i++)
                    {
                        childNodes[i].Render();
                    }
                }
            }
        }

        public int GetChildrenCount()
        {
            return childNodesToAdd.Count + childNodes.Count;
        }

        public MNode GetChildByName(string name)
        {
            for(int i = 0; i < childNodesToAdd.Count; i++)
            {
                if (childNodesToAdd[i].Name == name)
                    return childNodesToAdd[i];
            }

            for (int i = 0; i < childNodes.Count; i++)
            {
                if (childNodes[i].Name == name)
                    return childNodes[i];
            }

            return null;
        }

        public IEnumerable<MNode> GetChidlrenEnumerable()
        {
            foreach (var node in childNodesToAdd)
            {
                yield return node;
            }

            foreach (var node in childNodes)
            {
                if (childNodesToAdd.Contains(node) == false)
                    yield return node;
            }
        }

        public T GetComponent<T>() where T : MComponent
        {
            for(int i = 0; i < componentsToAdd.Count; i++)
            {
                MComponent componentToAdd = componentsToAdd[i];
                if (componentToAdd is T)
                    return componentToAdd as T;
            }

            for(int i = 0; i < scripts.Count; i++)
            {
                MComponent script = scripts[i];
                if (script is T)
                    return script as T;
            }

            for (int i = 0; i < components.Count; i++)
            {
                MComponent component = components[i];
                if (component is T)
                    return component as T;
            }

            return null;
        }

        public bool HasComponent<T>() where T : MComponent
        {
            if (GetComponent<T>() != null)
                return true;

            return false;
        }

        public bool Intersects(int mouseX, int mouseY)
        {
            return new Rectangle(X, Y, Width, Height).Contains(mouseX, mouseY);
        }

        public void AddOnIntersectsCallback(Action<MNode> callback)
        {
            cbOnIntersects += callback;
        }
    
        public void SetMetadata<T>(string key, T value)
        {
            if (metadata.ContainsKey(key))
            {
                metadata[key] = value;
            }
            else
            {
                metadata.Add(key, value);
            }
        }

        public T GetMetadata<T>(string key)
        {
            return (T)metadata[key];
        }

        public void RemoveMetadata(string key)
        {
            if (metadata.ContainsKey(key))
            {
                metadata.Remove(key);
            }
        }
    }
}
