using System.Linq;
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
            (int x, int y, Orientation orientation) shortestPathEndpoint = default;
            while (toVisit.Count > 0)
            {
                var (x, y, orientation) = toVisit.Dequeue();
                var currentCost = CurrentCost(x, y, orientation);

                if (x == Lines[0].Length - 1 && y == Lines.Length - 1)
                {
                    if (currentCost < shortestPathFound)
                    {
                        shortestPathFound = currentCost;
                        shortestPathEndpoint = (x, y, orientation);
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
                        minusCounter += GetValue(x - i, y);
                        plusCounter += GetValue(x + i, y);
                        if (i < minDirection)
                        {
                            continue;
                        }
                        if (IsValid(x - i, y))
                        {
                            if (CurrentCost(x - i, y, Orientation.Horizontal) > currentCost + minusCounter)
                            {
                                map[(x - i, y, Orientation.Horizontal)] = currentCost + minusCounter;
                                preceding[(x - i, y, Orientation.Horizontal)] = (x, y, orientation);
                                // Todo: Add estimation here
                                toVisit.EnqueueOrUpdate((x - i, y, Orientation.Horizontal), currentCost + minusCounter);
                            }
                        }
                        if (IsValid(x + i, y))
                        {
                            if (CurrentCost(x + i, y, Orientation.Horizontal) > currentCost + plusCounter)
                            {
                                map[(x + i, y, Orientation.Horizontal)] = currentCost + plusCounter;
                                preceding[(x + i, y, Orientation.Horizontal)] = (x, y, orientation);
                                // Todo: Add estimation here
                                toVisit.EnqueueOrUpdate((x + i, y, Orientation.Horizontal), currentCost + plusCounter);
                            }
                        }
                    }
                }
                if (orientation != Orientation.Vertical)
                {
                    int plusCounter = 0;
                    int minusCounter = 0;
                    for (int i = 1; i <= maxDirection; i++)
                    {
                        minusCounter += GetValue(x, y - i);
                        plusCounter += GetValue(x, y + i);
                        if (i < minDirection)
                        {
                            continue;
                        }
                        if (IsValid(x, y - i))
                        {
                            if (CurrentCost(x, y - i, Orientation.Vertical) > currentCost + minusCounter)
                            {
                                map[(x, y - i, Orientation.Vertical)] = currentCost + minusCounter;
                                preceding[(x, y - i, Orientation.Vertical)] = (x, y, orientation);
                                // Todo: Add estimation here
                                toVisit.EnqueueOrUpdate((x, y - i, Orientation.Vertical), currentCost + minusCounter);
                            }
                        }
                        if (IsValid(x, y + i))
                        {
                            if (CurrentCost(x, y + i, Orientation.Vertical) > currentCost + plusCounter)
                            {
                                map[(x, y + i, Orientation.Vertical)] = currentCost + plusCounter;
                                preceding[(x, y + i, Orientation.Vertical)] = (x, y, orientation);
                                // Todo: Add estimation here
                                toVisit.EnqueueOrUpdate((x, y + i, Orientation.Vertical), currentCost + plusCounter);
                            }
                        }
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

        private enum Orientation
        {
            Horizontal,
            Vertical,
            None
        }

        private class UpdatablePriorityQueue<TElement>
        {
            private Dictionary<TElement, int> data = new ();

            public void Enqueue(TElement element, int priority)
            {
                data.Add(element, priority);
            }
            public void EnqueueOrUpdate(TElement element, int priority)
            {
                data[element] = priority;
            }

            public TElement Dequeue()
            {
                if(Count == 0)
                {
                    throw new InvalidOperationException("Queue is empty");
                }

                TElement? smallest = default;
                int smallestPriority = int.MaxValue;

                foreach (var value in data)
                {
                    if (value.Value < smallestPriority)
                    {
                        smallestPriority = value.Value;
                        smallest = value.Key;
                    }
                }
                data.Remove(smallest!);
                return smallest!;
            }

            public int Count => data.Count;
        }
    }
}