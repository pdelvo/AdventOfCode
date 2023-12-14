﻿namespace AdventOfCodeLib.Days
{
    public class Day14 : Day
    {
        private const int limit = 1000000000;

        public override string? Description => "Parabolic Reflector Dish";
        public override string TestInput1 => @"O....#....
O.OO#....#
.....##...
OO.#O....O
.O.....O#.
O.#..O.#.#
..O..#O..O
.......O..
#....###..
#OO..#....";
        public override string TestOutput1 => "136";

        public override string TestOutput2 => "64";
        public override string RunPart1()
        {
            char[][] data = GetData();
            TiltNorth(data);
            return ComputeTotal(data).ToString();
        }

        public override string RunPart2()
        {
            char[][] data = GetData();
            Dictionary<int, int> firstSeen = new Dictionary<int, int>();
            List<long> results = new List<long>();
            for (int i = 0; i < limit; i++)
            {
                var lastSeen = GetLastSeen(data, firstSeen, i);

                if (lastSeen > 0 && i + lastSeen < limit)
                {
                    var offset = i - 1 - lastSeen;
                    var index = (limit - i - 1) % offset;

                    return results[lastSeen + index + 1].ToString();
                }
                Cycle(data);
                results.Add(ComputeTotal(data));
            }

            long total = ComputeTotal(data);
            return total.ToString();
        }

        private void Print(char[][] data)
        {
            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[0].Length; x++)
                {
                    Console.Write(data[y][x]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private int GetLastSeen(char[][] data, Dictionary<int, int> firstSeen, int position)
        {
            int hashCode = GetHash(data);

            if (firstSeen.TryGetValue(hashCode, out var lastSeen))
            {
                return lastSeen;
            }

            firstSeen.TryAdd(GetHash(data), position - 1);
            return 0;
        }

        private static int GetHash(char[][] data)
        {
            int hashCode = 0;
            for (int x = 0; x < data[0].Length; x++)
            {
                for (int y = 0; y < data[0].Length; y++)
                {
                    hashCode = HashCode.Combine(hashCode, (int)data[y][x]);
                }
            }

            return hashCode;
        }

        private void Cycle(char[][] data)
        {
            // Tilt north
            TiltNorth(data);

            // Tilt west
            TiltWest(data);

            // Tilt south
            TiltSouth(data);

            // Tilt east
            TiltEast(data);
        }

        private char[][] GetData()
        {
            char[][] result = new char[Lines.Length][];

            for (int line = 0; line < Lines.Length; line++)
            {
                result[line] = new char[Lines[line].Length];
                for (int column = 0; column < Lines[line].Length; column++)
                {
                    result[line][column] = Lines[line][column];
                }
            }

            return result;
        }

        private long ComputeTotal(char[][] data)
        {
            long total = 0;
            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[y].Length; x++)
                {
                    if (data[y][x] == 'O')
                    {
                        total += data.Length - y;
                    }
                }
            }
            return total;
        }

        private void TiltNorth(char[][] data)
        {
            var width = data.Length;
            var height = data[0].Length;
            Span<int> moveTo = stackalloc int[width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    switch (data[y][x])
                    {
                        case '.':
                            continue;
                        case '#':
                            moveTo[x] = y + 1;
                            break;
                        case 'O':
                            data[y][x] = '.';
                            data[moveTo[x]][x] = 'O';
                            moveTo[x]++;
                            break;
                        default:
                            throw new Exception($"Invalid symbol '{data[y][x]}'.");
                    }
                }
            }
        }

        private void TiltSouth(char[][] data)
        {
            var width = data.Length;
            var height = data[0].Length;
            Span<int> moveTo = stackalloc int[width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    switch (data[height - 1 - y][x])
                    {
                        case '.':
                            continue;
                        case '#':
                            moveTo[x] = y + 1;
                            break;
                        case 'O':
                            data[height - 1 - y][x] = '.';
                            data[height - 1 - moveTo[x]][x] = 'O';
                            moveTo[x]++;
                            break;
                        default:
                            throw new Exception($"Invalid symbol '{data[height - 1 - y][x]}'.");
                    }
                }
            }
        }

        private void TiltWest(char[][] data)
        {
            var width = data.Length;
            var height = data[0].Length;
            Span<int> moveTo = stackalloc int[height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    switch (data[y][x])
                    {
                        case '.':
                            continue;
                        case '#':
                            moveTo[y] = x + 1;
                            break;
                        case 'O':
                            data[y][x] = '.';
                            data[y][moveTo[y]] = 'O';
                            moveTo[y]++;
                            break;
                        default:
                            throw new Exception($"Invalid symbol '{data[y][x]}'.");
                    }
                }
            }
        }

        private void TiltEast(char[][] data)
        {
            var width = data.Length;
            var height = data[0].Length;
            Span<int> moveTo = stackalloc int[height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    switch (data[y][width - 1 - x])
                    {
                        case '.':
                            continue;
                        case '#':
                            moveTo[y] = x + 1;
                            break;
                        case 'O':
                            data[y][width - 1 - x] = '.';
                            data[y][width - 1 - moveTo[y]] = 'O';
                            moveTo[y]++;
                            break;
                        default:
                            throw new Exception($"Invalid symbol '{data[y][width - 1 - x]}'.");
                    }
                }
            }
        }
    }
}