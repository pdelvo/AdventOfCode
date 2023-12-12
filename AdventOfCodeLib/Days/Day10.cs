using System.Text;

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

        static readonly char[] neighborTypes = ['|', '-', 'L', 'J', '7', 'F'];

        public override string TestOutput2 => "4";
        public override string RunPart1()
        {
            var (counter, _, _, _)= WalkPipes();

            return (counter / 2).ToString();
        }
        public override string RunPart2()
        {
            var (_, map, start, sKind) = WalkPipes();

            char GetCharAtPositionWithReplacement(int x, int y)
            {
                if (start == (x, y))
                {
                    return sKind;
                }
                return Lines[y][x];
            }

            int insideCounter = 0;


            for (var y = 0; y < Lines.Length; y++)
            {
                bool inside = false;

                for (int x = 0; x < Lines[0].Length; x++)
                {
                    if (map[x, y])
                    {
                        if (GetCharAtPositionWithReplacement(x, y) == '|')
                        {
                            inside = !inside;
                        }
                        else if (GetCharAtPositionWithReplacement(x, y) == 'F')
                        {
                            x++;
                            while (GetCharAtPositionWithReplacement(x, y) == '-')
                            {
                                x++;
                            }

                            if (GetCharAtPositionWithReplacement(x, y) == 'J')
                            {
                                inside = !inside;
                            }
                        }
                        else if (GetCharAtPositionWithReplacement(x, y) == 'L')
                        {
                            x++;
                            while (GetCharAtPositionWithReplacement(x, y) == '-')
                            {
                                x++;
                            }

                            if (GetCharAtPositionWithReplacement(x, y) == '7')
                            {
                                inside = !inside;
                            }
                        }
                    }
                    else
                    {
                        if (inside)
                        {
                            insideCounter++;
                        }
                    }
                }
            }

            return insideCounter.ToString();

        }

        private string FormatInsideOutsideMap(bool?[,] insideOutsideMap, bool[,] map)
        {
            int width = Lines[0].Length;
            int height = Lines.Length;
            StringBuilder builder = new();

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

        private (int, bool[,] map, (int x, int y) start, char SKind) WalkPipes()
        {
            var map = new bool[Lines[0].Length, Lines.Length];
            var (startX, startY) = FindStart();
            map[startX, startY] = true;

            var startNeighbors = FindPositionsAfterStart((startX, startY));
            var neighbor = startNeighbors[0];
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
            char sKind = ' ';
            // Replace start by what it should be
            foreach (var neighborType in neighborTypes)
            {
                Lines[startY] = ReplaceAt(Lines[startY], startX, 1, neighborType.ToString());

                var (neighbor1, neighbor2) = FindNeighbors((startX, startY))!.Value;

                if (startNeighbors.Contains(neighbor1) && startNeighbors.Contains(neighbor2))
                {
                    sKind = neighborType;
                    Lines[startY] = ReplaceAt(Lines[startY], startX, 1, "S");
                    break;
                }
            }

            return (counter, map, (startX, startY),sKind);
        }

        private (int x, int y)[] FindPositionsAfterStart((int x, int y) start)
        {
            (int x, int y)[] neighbors = new (int x, int y)[2];
            var neighbor = (start.x - 1, start.y);
            int counter = 0;

            if (FindNextPosition(neighbor, start) != null)
            {
                neighbors[counter++] = neighbor;
            }

            neighbor = (start.x + 1, start.y);

            if (FindNextPosition(neighbor, start) != null)
            {
                neighbors[counter++] = neighbor;
            }
            neighbor = (start.x, start.y - 1);

            if (FindNextPosition(neighbor, start) != null)
            {
                neighbors[counter++] = neighbor;
            }
            neighbor = (start.x, start.y + 1);

            if (FindNextPosition(neighbor, start) != null)
            {
                neighbors[counter++] = neighbor;
            }
            return neighbors;
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
            return GetPosition(x, y) switch
            {
                '|' => (((int x, int y), (int x, int y))?)((x, y - 1), (x, y + 1)),
                '-' => (((int x, int y), (int x, int y))?)((x - 1, y), (x + 1, y)),
                'L' => (((int x, int y), (int x, int y))?)((x, y - 1), (x + 1, y)),
                'J' => (((int x, int y), (int x, int y))?)((x - 1, y), (x, y - 1)),
                '7' => (((int x, int y), (int x, int y))?)((x - 1, y), (x, y + 1)),
                'F' => (((int x, int y), (int x, int y))?)((x + 1, y), (x, y + 1)),
                _ => null,
            };
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

        public static string ReplaceAt(string str, int index, int length, string replace)
        {
            return string.Create(str.Length - length + replace.Length, (str, index, length, replace),
                (span, state) =>
                {
                    state.str.AsSpan()[..state.index].CopyTo(span);
                    state.replace.AsSpan().CopyTo(span[state.index..]);
                    state.str.AsSpan()[(state.index + state.length)..].CopyTo(span[(state.index + state.replace.Length)..]);
                });
        }
    }
}