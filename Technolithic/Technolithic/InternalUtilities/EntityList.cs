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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class EntityList : IEnumerable<Entity>, IEnumerable
    {
        public Scene Scene { get { return Layer.Scene; } }
        public Layer Layer { get; private set; }

        public bool SortByYAxisAlways { get; set; } = false;
        public bool SortByYAxisWhenAdded { get; set; } = false;

        private List<Entity> entities;
        private List<Entity> toAdd;
        private List<Entity> toRemove;

        private EntityComparer toCompare = new EntityComparer();

        public EntityList(Layer layer)
        {
            Layer = layer;

            entities = new List<Entity>();
            toAdd = new List<Entity>();
            toRemove = new List<Entity>();
        }

        public void Add(Entity entity)
        {
            toAdd.Add(entity);
        }

        public void Remove(Entity entity)
        {
            toRemove.Add(entity);
        }

        public int Count
        {
            get { return entities.Count; }
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Entity[] ToArray()
        {
            return entities.ToArray<Entity>();
        }

        public void UpdateLists()
        {
            if (toAdd.Count > 0)
            {
                for (int i = 0; i < toAdd.Count; i++)
                {
                    Entity entity = toAdd[i];
                    entity.Awake();
                }

                for (int i = 0; i < toAdd.Count; i++)
                {
                    Entity entity = toAdd[i];
                    entities.Add(entity);
                    entity.Begin();
                }

                if(SortByYAxisWhenAdded)
                {
                    entities.Sort(toCompare);
                }

                toAdd.Clear();
            }

            if (toRemove.Count > 0)
            {
                for (int i = 0; i < toRemove.Count; i++)
                {
                    Entity entity = toRemove[i];
                    entities.Remove(entity);
                }

                toRemove.Clear();
            }

            if (SortByYAxisAlways)
            {
                entities.Sort(toCompare);
            }
        }

        public void Update()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                if (entity.Active && !entity.IsRemoved)
                    entity.Update();
            }
        }

        public void Render()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                if (entity.Visible && !entity.IsRemoved)
                    entity.Render();
            }
        }
    }

    public class EntityComparer : IComparer<Entity>
    {
        public int Compare(Entity entityA, Entity entityB)
        {
            return entityA.CompareTo(entityB);
        }
    }
}
