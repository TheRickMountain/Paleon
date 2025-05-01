//  m o n O c l e
//
//   e n g i n e
//
//
//Copyright (c) 2012 - 2014 Matt Thorson
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ComponentList
    {
        public Entity Entity { get; internal set; }

        private List<Component> components;

        internal ComponentList(Entity entity)
        {
            Entity = entity;

            components = new List<Component>();
        }

        public void Add(Component component)
        {
            components.Add(component);
            component.Entity = Entity;
        }

        public void Add(params Component[] components)
        {
            for (int i = 0; i < components.Length; i++)
                Add(components[i]);
        }

        public void Remove(Component component)
        {
            components.Remove(component);
            component.Entity = null;
        }

        public void Remove(params Component[] components)
        {
            for (int i = 0; i < components.Length; i++)
                Remove(components[i]);
        }

        public int Count
        {
            get { return components.Count; }
        }

        internal void Awake()
        {
            for (int i = 0; i < components.Count; i++)
                components[i].Awake();
        }

        internal void Begin()
        {
            for (int i = 0; i < components.Count; i++)
                components[i].Begin();
        }

        internal void Update()
        {
            for(int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                if (component.Active)
                {
                    component.Update();
                    component.LateUpdate();
                }
            }
        }

        internal void Render()
        {
            for (int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                if (component.Visible)
                    component.Render();
            }
        }

        public Component this[int index]
        {
            get
            {
                if (index < 0 || index >= components.Count)
                    throw new IndexOutOfRangeException();
                else
                    return components[index];
            }
        }

        public T Get<T>() where T : Component
        {
            for(int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                if (component is T)
                    return component as T;
            }
            return null;
        }

        public IEnumerable<T> GetAll<T>() where T : Component
        {
            for(int i = 0; i < components.Count; i++)
            {
                Component component = components[i];
                if (component is T)
                    yield return component as T;
            }
        }

        public Component[] ToArray()
        {
            return components.ToArray<Component>();
        }

    }
}
