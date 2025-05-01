using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technolithic
{
    public class ListViewUIScript : MScript
    {

        private List<MNode> containers = new List<MNode>();

        private List<MNode> items = new List<MNode>();

        private int startElementIndex;
        private int endElementIndex;

        private int elementHeight;
        private int elementWidth;

        private int rowsCount;
        private int columnsCount;

        private int offsetBetweenElements = 5;

        private MNode scroller;
        private MImageCmp scrollerImage;
        private MNode scrollerPath;

        private int realRowsCount;

        public bool GrabMouse { get; set; } = true;

        public bool IsScrollable { get; private set; }

        public bool IsDirty { get; set; } = false;

        public ListViewUIScript(int elementHeight, int elementWidth, int rowsCount, int columnsCount, bool scrollable = true) : base(true)
        {
            this.elementHeight = elementHeight;
            this.elementWidth = elementWidth;
            this.rowsCount = rowsCount;
            this.columnsCount = columnsCount;
            IsScrollable = scrollable;
        }

        public override void Awake()
        {
        }

        public override void Begin()
        {
            for (int y = 0; y < rowsCount; y++)
            {
                for (int x = 0; x < columnsCount; x++)
                {
                    MNode container = new MNode(ParentNode.Scene);

                    container.Y = y * (elementHeight + offsetBetweenElements);
                    container.X = x * (elementWidth + offsetBetweenElements);

                    ParentNode.AddChildNode(container);

                    containers.Add(container);
                }
            }

            if (IsScrollable)
            {
                scroller = ParentNode.GetChildByName("Scroller");
                scrollerImage = scroller.GetComponent<MImageCmp>();
                scrollerPath = ParentNode.GetChildByName("ScrollerPath");
            }

            UpdateView();
        }

        private bool scrollerWasGrabbed = false;

        public override void Update(int mouseX, int mouseY)
        {
            if(IsDirty)
            {
                UpdateView();
                IsDirty = false;
            }

            if(IsScrollable)
            {
                if (scrollerPath.Intersects(mouseX, mouseY))
                {
                    if (MInput.Mouse.CheckLeftButton)
                    {
                        scrollerWasGrabbed = true;
                    }

                    scrollerImage.Color = Engine.ORANGE;
                }
                else
                {
                    scrollerImage.Color = Color.DarkGray;
                }

                if (scrollerWasGrabbed)
                {
                    scrollerImage.Color = Engine.ORANGE;
                }

                if (MInput.Mouse.ReleasedLeftButton)
                {
                    scrollerWasGrabbed = false;
                }

                if (scrollerWasGrabbed)
                {
                    int allowedOffset = realRowsCount - rowsCount;

                    if (allowedOffset > 0)
                    {
                        float stepHeight = (float)scrollerPath.Height / (float)allowedOffset;
                        int newStartElementIndex = (int)((mouseY - scrollerPath.Y) / stepHeight);
                        if (newStartElementIndex >= 0 && newStartElementIndex <= allowedOffset)
                        {

                            startElementIndex = newStartElementIndex;
                            endElementIndex = startElementIndex + rowsCount;

                            UpdateElements(startElementIndex, endElementIndex);

                            UpdateScroller();
                        }
                    }
                }
            }

            if (ParentNode.Intersects(mouseX, mouseY))
            {
                if(GrabMouse)
                    GameplayScene.MouseOnUI = true;

                if (IsScrollable)
                {
                    int wheel = MInput.Mouse.WheelDelta;
                    TryToScroll(wheel);
                }
            }
        }

        public void TryToScroll(int wheel)
        {
            if(wheel != 0)
            {
                int allowedOffset = realRowsCount - rowsCount;
                if (allowedOffset <= 0)
                {
                    return;
                }
                else
                {
                    int dt = -MathHelper.Clamp(wheel, -1, 1);
                    startElementIndex = MathHelper.Clamp(startElementIndex + dt, 0, allowedOffset);
                    endElementIndex = startElementIndex + rowsCount;

                    UpdateElements(startElementIndex, endElementIndex);

                    UpdateScroller();
                }
            }
        }

        public void UpdateView(bool continueFromLastPosition = false)
        {
            if (continueFromLastPosition)
            {
                IsDirty = false;

                if (endElementIndex > realRowsCount)
                {
                    int diff = endElementIndex - realRowsCount;
                    startElementIndex -= diff;
                    endElementIndex -= diff;
                }

                if (startElementIndex <= 0)
                {
                    startElementIndex = 0;

                    endElementIndex = realRowsCount > containers.Count ? containers.Count : realRowsCount;
                }

                UpdateElements(startElementIndex, endElementIndex);
            }
            else
            {
                startElementIndex = 0;

                int itemsCount = realRowsCount + (items.Count % columnsCount > 0 ? 1 : 0);

                endElementIndex = itemsCount > rowsCount ? rowsCount : itemsCount;

                UpdateElements(0, endElementIndex);
            }

            if(IsScrollable && scroller != null)
                UpdateScroller();
        }

        private void UpdateElements(int startElement, int endElement)
        {
            for (int i = 0; i < containers.Count; i++)
                containers[i].RemoveAllChildren();

            int count = 0;
            for (int i = startElement * columnsCount; i < endElement * columnsCount; i++)
            {
                if (i >= items.Count)
                    break;

                containers[count].AddChildNode(items[i]);

                count++;
            }
        }

        public bool ContainsItem(MNode item)
        {
            return items.Contains(item);
        }

        public int GetIndexOfItem(MNode item)
        {
            return items.IndexOf(item);
        }

        public IEnumerable<MNode> GetItemsBetween(MNode firstItem, MNode lastItem)
        {
            int indexOfFirst = items.IndexOf(firstItem);
            int indexOfLast = items.IndexOf(lastItem);

            if(indexOfFirst == indexOfLast)
            {
                yield return items[indexOfFirst];
                yield break;
            }

            if(indexOfFirst > indexOfLast)
            {
                int temp = indexOfLast;
                indexOfLast = indexOfFirst;
                indexOfFirst = temp;
            }

            for (int i = indexOfFirst; i <= indexOfLast; i++)
            {
                yield return items[i];
            }
        }

        public void AddItem(MNode item)
        {
            IsDirty = true;

            items.Add(item);

            float temp = (float)items.Count / (float)columnsCount;
            realRowsCount = (int)Math.Ceiling(temp);

            if (scroller != null)
            {
                UpdateScroller();
            }
        }

        public void AddItemAfter(MNode item, MNode afterItem)
        {
            IsDirty = true;

            int afterItemIndex = items.IndexOf(afterItem);
            items.Insert(afterItemIndex + 1, item);

            float temp = (float)items.Count / (float)columnsCount;
            realRowsCount = (int)Math.Ceiling(temp);

            if (scroller != null)
            {
                UpdateScroller();
            }
        }

        public void RemoveItem(MNode item)
        {
            IsDirty = true;

            items.Remove(item);

            float temp = (float)items.Count / (float)columnsCount;
            realRowsCount = (int)Math.Ceiling(temp);

            if (scroller != null)
            {
                UpdateScroller();
            }
        }

        public void Clear()
        {
            IsDirty = true;

            items.Clear();

            if(scroller != null)
            {
                UpdateScroller();
            }
        }

        private void UpdateScroller()
        {
            if (realRowsCount > rowsCount)
            {
                float divider = (float)(realRowsCount) / (float)rowsCount;
                scroller.Height = (int)(scrollerPath.Height / divider);

                scroller.Y = (int)((float)scrollerPath.LocalY + ((float)scrollerPath.Height / (float)(realRowsCount)) * (float)startElementIndex);
            }
            else
            {
                scroller.Height = scrollerPath.Height;
                scroller.Y = scrollerPath.LocalY;
            }
        }

        public IEnumerator<MNode> GetEnumerator()
        {
            return items.GetEnumerator();
        }
        
    }
}
