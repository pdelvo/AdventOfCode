using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCodeLib
{
    internal class UpdatablePriorityQueue<TElement> where TElement : notnull
    {
        private (TElement, int)[] data;
        private Dictionary<TElement, int> elementToPosition = new();
        private int count;

        public UpdatablePriorityQueue()
        {
            data = new (TElement, int)[128];
        }

        private int GetParent(int index) => (index - 1) >> 1;
        private int GetFirstChild(int index) => (index << 1) + 1;

        private void SetElement(int position, (TElement element, int priority) toPlace)
        {
            data[position] = toPlace;
            elementToPosition[toPlace.element] = position;
        }

        private void Grow(int minCapacity)
        {
            int newSize = minCapacity * 2;
            Array.Resize(ref data, newSize);
        }

        public void Enqueue(TElement element, int priority)
        {
            if (++count == data.Length)
            {
                Grow(count + 1);
            }

            SetElement(count - 1, (element, priority));
            BubbleUp(count - 1);
        }

        public void EnqueueOrUpdate(TElement element, int priority)
        {
            if (elementToPosition.TryGetValue(element, out var index))
            {
                (TElement element, int priority) current = data[index];
                SetElement(index, (element, priority));
                if (priority > current.priority)
                {
                    BubbleUp(index);
                }
                else
                {
                    BubbleDown((element, priority), index);
                }
            }
            else
            {
                Enqueue(element, priority);
            }
        }

        public TElement Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            var (element, _) = data[0];

            elementToPosition.Remove(element);

            RemoveRoot();

            return element;
        }

        private void RemoveRoot()
        {
            int lastNodeIndex = --count;
            var lastNode = data[lastNodeIndex];

            if (lastNodeIndex > 0)
            {
                BubbleDown(lastNode, 0);
            }
        }

        private void BubbleDown((TElement element, int priority) node, int position)
        {
            int i;
            while ((i = GetFirstChild(position)) < count)
            {
                (TElement element, int priority) smallestChild = data[i];
                (TElement element, int priority) child2 = data[i + 1];

                if (child2.priority < smallestChild.priority)
                {
                    i++;
                    smallestChild = child2;
                }

                if (node.priority < smallestChild.priority)
                {
                    break;
                }

                SetElement(position, smallestChild);
                position = i;
            }
            SetElement(position, node);
        }

        private void BubbleUp(int position)
        {
            (TElement element, int priority) = data[position];
            while (position != 0)
            {
                var parentIndex = GetParent(position);
                (TElement element, int priority) parent = data[position];

                if (priority < parent.priority)
                {
                    SetElement(position, parent);
                    position = parentIndex;
                }
                else
                {
                    break;
                }
            }

            SetElement(position, (element, priority));
        }

        public int Count => count;
    }
}
