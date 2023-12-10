using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCodeLib.Days
{
    public class Day10 : Day
    {
        public override string? Description => "Pipe Maze";
        public override string TestInput1 => @"7-F7-
.FJ|7
SJLL7
|F--J
LJ.LJ";
        public override string TestOutput1 => "8";
        public override string TestInput2 => @"..........
.S------7.
.|F----7|.
.||....||.
.||....||.
.|L-7F-J|.
.|..||..|.
.L--JL--J.
..........";
        public override string TestOutput2 => "4";
        public override string RunPart1()
        {
            var (counter, _)= WalkPipes();

            return (counter / 2).ToString();
        }
        public override string RunPart2()
        {
            var (counter, map) = WalkPipes();

            var insideOutsideMap = new bool?[Lines[0].Length, Lines.Length];

            Queue<(int x, int y, bool inside)> toVisit = new Queue<(int x, int y, bool inside)>();
            HashSet<(int x, int y)> seen = new HashSet<(int x, int y)>();

            for (int x = 0; x < Lines[0].Length; x++)
            {
                PositionSeen(toVisit, seen, map, (x, 0), false);
                PositionSeen(toVisit, seen, map, (x, Lines.Length - 1), false);
            }
            for (int y = 1; y < Lines.Length - 1; y++)
            {
                PositionSeen(toVisit, seen, map, (0, y), false);
                PositionSeen(toVisit, seen, map, (Lines[0].Length - 1, y), false);
            }

            while(toVisit.Count > 0)
            {
                var (x, y, inside) = toVisit.Dequeue();
                if (!map[x, y])
                {
                    insideOutsideMap[x, y] = inside;
                    PositionSeen(toVisit, seen, map, (x - 1, y), inside);
                    PositionSeen(toVisit, seen, map, (x, y - 1), inside);
                    PositionSeen(toVisit, seen, map, (x + 1, y), inside);
                    PositionSeen(toVisit, seen, map, (x, y + 1), inside);
                }
                else
                {
                    char? pipePart = GetPosition(x, y);
                    switch (pipePart)
                    {
                        case '|':
                            MakeDifferent(toVisit, seen, insideOutsideMap, map, (x - 1, y), (x + 1, y));
                            break;
                        case '-':
                            MakeDifferent(toVisit, seen, insideOutsideMap, map, (x, y - 1), (x, y + 1));
                            break;
                        case 'L':
                            MakeDifferent(toVisit, seen, insideOutsideMap, map, (x + 1, y - 1), (x - 1, y));
                            MakeDifferent(toVisit, seen, insideOutsideMap, map, (x + 1, y - 1), (x, y + 1));
                            MakeDifferent(toVisit, seen, insideOutsideMap, map, (x + 1, y - 1), (x - 1, y + 1));
                            break;
                        case 'J':
                            MakeDifferent(toVisit, seen, insideOutsideMap, map, (x - 1, y - 1), (x + 1, y));
                            MakeDifferent(toVisit, seen, insideOutsideMap, map, (x - 1, y - 1), (x, y + 1));
                            MakeDifferent(toVisit, seen, insideOutsideMap, map, (x - 1, y - 1), (x + 1, y + 1));
                            break;
                        case '7':
                            MakeDifferent(toVisit, seen, insideOutsideMap, map, (x - 1, y + 1), (x + 1, y));
                            MakeDifferent(toVisit, seen, insideOutsideMap, map, (x - 1, y + 1), (x, y + 1));
                            MakeDifferent(toVisit, seen, insideOutsideMap, map, (x - 1, y + 1), (x + 1, y + 1));
                            break;
                        case 'F':
                            MakeDifferent(toVisit, seen, insideOutsideMap, map, (x + 1, y + 1), (x - 1, y));
                            MakeDifferent(toVisit, seen, insideOutsideMap, map, (x + 1, y + 1), (x, y - 1));
                            MakeDifferent(toVisit, seen, insideOutsideMap, map, (x + 1, y + 1), (x - 1, y - 1));
                            break;
                        case 'S':
                            break;
                        default: throw new Exception("Pipe part expected");
                    }
                }
            }

            string testString = FormatInsideOutsideMap(insideOutsideMap, map);
            Console.WriteLine(testString);

            int insideCounter = 0;

            for (int x = 0; x < Lines[0].Length; x++)
            {
                for (int y = 0; y < Lines.Length; y++)
                {
                    if (insideOutsideMap[x, y] is true && map[x, y] is false)
                    {
                        insideCounter++;
                    }
                }
            }

            return insideCounter.ToString();
        }

        private string FormatInsideOutsideMap(bool?[,] insideOutsideMap, bool[,] map)
        {
            int width = Lines[0].Length;
            int height = Lines.Length;
            StringBuilder builder = new StringBuilder();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (map[x, y])
                    {
                        builder.Append(Lines[y][x]);
                    }
                    else
                    {
                        builder.Append(insideOutsideMap[x, y] == true ? 'I' : 'O');
                    }
                }
                builder.AppendLine();
            }

            return builder.ToString();
        }

        private void MakeDifferent(Queue<(int x, int y, bool inside)> toVisit,
            HashSet<(int x, int y)> seen,
            bool?[,] insideOutsideMap,
            bool[,] map,
            (int x, int y) position1,
            (int x, int y) position2)
        {
            if (!IsPositionValid(position1) || !IsPositionValid(position2))
            {
                return;
            }

            if (insideOutsideMap[position1.x, position1.y] is bool pos1Value)
            {
                insideOutsideMap[position2.x, position2.y] = !pos1Value ^ map[position1.x, position1.y];
                PositionSeen(toVisit, seen, map, position2, !pos1Value);
            }
            else if (insideOutsideMap[position2.x, position2.y] is bool pos2Value)
            {
                insideOutsideMap[position1.x, position1.y] = !pos2Value ^ map[position2.x, position2.y];
                PositionSeen(toVisit, seen, map, position1, !pos2Value);
            }
        }

        private void PositionSeen(Queue<(int x, int y, bool inside)> toVisit, HashSet<(int x, int y)> seen, bool[,] map, (int x, int y) position, bool inside)
        {
            if (IsPositionValid(position) && !seen.Contains(position))
            {
                toVisit.Enqueue((position.x, position.y, inside ^ map[position.x, position.y]));
                seen.Add(position);
            }
        }

        private bool IsPositionValid((int x, int y) position)
        {
            return position.x >= 0 && position.y >= 0 && position.x < Lines[0].Length && position.y < Lines.Length;
        }

        private (int, bool[,] map) WalkPipes()
        {
            var map = new bool[Lines[0].Length, Lines.Length];
            var (startX, startY) = FindStart();
            map[startX, startY] = true;

            var neighbor = FindPositionAfterStart((startX, startY));
            map[neighbor.x, neighbor.y] = true;
            var previousNeighbor = (startX, startY);
            int counter = 1;
            while (neighbor != (startX, startY))
            {
                var oldNeighbor = neighbor;
                neighbor = FindNextPosition(neighbor, previousNeighbor)!.Value;
                map[neighbor.x, neighbor.y] = true;

                previousNeighbor = oldNeighbor;
                counter++;
            }

            return (counter, map);
        }

        private (int x, int y) FindPositionAfterStart((int x, int y) start)
        {
            var neighbor = (start.x - 1, start.y);

            if (FindNextPosition(neighbor, start) != null)
            {
                return neighbor;
            }

            neighbor = (start.x + 1, start.y);

            if (FindNextPosition(neighbor, start) != null)
            {
                return neighbor;
            }
            neighbor = (start.x, start.y - 1);

            if (FindNextPosition(neighbor, start) != null)
            {
                return neighbor;
            }

            // Should never be reached from here
            neighbor = (start.x - 1, start.y + 1);

            if (FindNextPosition(neighbor, start) != null)
            {
                return neighbor;
            }

            throw new Exception("Could not find neighbor of start");
        }

        private (int x, int y)? FindNextPosition((int x, int y) currentPosition, (int x, int y) previousPosition)
        {
            var neighbors = FindNeighbors(currentPosition);

            if (neighbors == null)
            {
                return null;
            }

            var (neighbor1, neighbor2) = neighbors.Value;

            if (previousPosition == neighbor1)
            {
                return neighbor2;
            }
            else if (previousPosition == neighbor2)
            {
                return neighbor1;
            }

            return null;
        }

        private ((int x, int y), (int x, int y))? FindNeighbors((int x, int y) position)
        {
            var (x, y) = position;
            switch (GetPosition(x, y))
            {
                case '|':
                    return ((x, y - 1), (x, y + 1));
                case '-':
                    return ((x - 1, y), (x + 1, y));
                case 'L':
                    return ((x, y - 1), (x + 1, y));
                case 'J':
                    return ((x - 1, y), (x, y - 1));
                case '7':
                    return ((x - 1, y), (x, y + 1));
                case 'F':
                    return ((x + 1, y), (x, y + 1));
                default:
                    return null;
                    // throw new ArgumentException($"Char '{GetPosition(position.x, position.y)}' is invalid");
            }
        }

        private (int startX, int startY) FindStart()
        {
            for (var y = 0; y < Lines.Length; y++)
            {
                for (int x = 0; x < Lines[0].Length; x++)
                {
                    if (GetPosition(x, y) == 'S')
                    {
                        return (x, y);
                    }
                }
            }

            throw new InvalidOperationException("Could not find start");
        }

        private char? GetPosition(int x, int y)
        {
            if (IsPositionValid((x, y)))
            {
                return Lines[y][x];
            }

            return null;
        }
    }
}