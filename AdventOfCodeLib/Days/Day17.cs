using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace AdventOfCodeLib.Days
{
    public class Day17 : Day
    {
        public override string? Description => "Clumsy Crucible";
        public override string TestInput1 => @"2413432311323
3215453535623
3255245654254
3446585845452
4546657867536
1438598798454
4457876987766
3637877979653
4654967986887
4564679986453
1224686865563
2546548887735
4322674655533";
        public override string TestOutput1 => "102";

        public override string TestOutput2 => "94";
        public override string RunPart1()
        {
            const int maxDirection = 3;
            const int minDirection = 1;
            return Solve(maxDirection, minDirection).ToString();
        }

        public override string RunPart2()
        {
            const int maxDirection = 10;
            const int minDirection = 4;
            return Solve(maxDirection, minDirection).ToString();
        }

        private int Solve(int maxDirection, int minDirection)
        {
            var startConfiguration = (0, 0, Orientation.None);
            Dictionary<(int x, int y, Orientation orientation), int> map = new()
            {
                { startConfiguration, 0 }
            };
            Dictionary<(int x, int y, Orientation orientation), (int x, int y, Orientation orientation)> preceding = new();
            UpdatablePriorityQueue<(int x, int y, Orientation orientation)> toVisit = new();
            Dictionary<(int x, int y, Orientation orientation), int> toVisitMap = new()
            {
                { startConfiguration, 0 }
            };
            toVisit.Enqueue(startConfiguration, 0);
            int shortestPathFound = int.MaxValue;
            while (toVisit.Count > 0)
            {
                var (x, y, orientation) = toVisit.Dequeue();
                var currentCost = CurrentCost(x, y, orientation);

                if (x == Lines[0].Length - 1 && y == Lines.Length - 1)
                {
                    if (currentCost < shortestPathFound)
                    {
                        shortestPathFound = currentCost;
                    }
                    continue;
                }

                if (currentCost > shortestPathFound)
                {
                    continue;
                }

                if (orientation != Orientation.Horizontal)
                {
                    int plusCounter = 0;
                    int minusCounter = 0;
                    for (int i = 1; i <= maxDirection; i++)
                    {
                        (int x, int y) negative = (x - i, y);
                        (int x, int y) positive = (x + i, y);
                        minusCounter += GetValue(x - i, y);
                        plusCounter += GetValue(x + i, y);
                        if (i < minDirection)
                        {
                            continue;
                        }
                        UpdateIfValid(x, y, orientation, Orientation.Horizontal, currentCost, minusCounter, negative);
                        UpdateIfValid(x, y, orientation, Orientation.Horizontal, currentCost, plusCounter, positive);
                    }
                }
                if (orientation != Orientation.Vertical)
                {
                    int plusCounter = 0;
                    int minusCounter = 0;
                    for (int i = 1; i <= maxDirection; i++)
                    {
                        (int x, int y) negative = (x, y - i);
                        (int x, int y) positive = (x, y + i);
                        minusCounter += GetValue(x, y - i);
                        plusCounter += GetValue(x, y + i);
                        if (i < minDirection)
                        {
                            continue;
                        }
                        UpdateIfValid(x, y, orientation, Orientation.Vertical, currentCost, minusCounter, negative);
                        UpdateIfValid(x, y, orientation, Orientation.Vertical, currentCost, plusCounter, positive);
                    }
                }
            }

            return shortestPathFound;

            int CurrentCost(int x, int y, Orientation orientation)
            {
                if (map.TryGetValue((x, y, orientation), out var value))
                {
                    return value;
                }
                return int.MaxValue;
            }

            void UpdateIfValid(int x, int y, Orientation orientation, Orientation newOrientation, int currentCost, int counter, (int x, int y) coordinates)
            {
                if (IsValid(coordinates))
                {
                    if (CurrentCost(coordinates.x, coordinates.y, newOrientation) > currentCost + counter)
                    {
                        map[(coordinates.x, coordinates.y, newOrientation)] = currentCost + counter;
                        preceding[(coordinates.x, coordinates.y, newOrientation)] = (x, y, orientation);
                        // Todo: Add estimation here
                        toVisit.EnqueueOrUpdate((coordinates.x, coordinates.y, newOrientation), currentCost + counter);
                    }
                }
            }
        }

        private int GetValue(int x, int y)
        {
            if (!IsValid(x, y))
            {
                return 0;
            }

            return (Lines[y][x] - '0');
        }

        private bool IsValid(int x, int y) => x >= 0 && y >= 0 && x < Lines[0].Length && y < Lines.Length;
        private bool IsValid((int x, int y) coordinates)
            => coordinates.x >= 0 && coordinates.y >= 0 && coordinates.x < Lines[0].Length && coordinates.y < Lines.Length;

        private enum Orientation
        {
            Horizontal,
            Vertical,
            None
        }

        private class UpdatablePriorityQueue<TElement> where TElement : notnull
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
                if(Count == 0)
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
}