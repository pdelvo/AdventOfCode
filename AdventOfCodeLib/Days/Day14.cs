namespace AdventOfCodeLib.Days
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
            char[,] data = GetData();
            Tilt(data, (x, y) => (x, y), data.GetLength(1), data.GetLength(0));
            return ComputeTotal(data).ToString();
        }

        public override string RunPart2()
        {
            char[,] data = GetData();
            Dictionary<int, int> firstSeen = new Dictionary<int, int>();
            for (int i = 0; i < limit; i++)
            {
                var lastSeen = GetLastSeen(data, firstSeen, i);

                if (lastSeen > 0 && i + lastSeen < limit)
                {
                    var offset = i - 1 - lastSeen;
                    i += ((limit - i - 1) / offset) * offset;
                }
                Cycle(data);
            }

            long total = ComputeTotal(data);
            return total.ToString();
        }

        private void Print(char[,] data)
        {
            for (int y = 0; y < data.GetLength(0); y++)
            {
                for (int x = 0; x < data.GetLength(1); x++)
                {
                    Console.Write(data[y, x]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private int GetLastSeen(char[,] data, Dictionary<int, int> firstSeen, int position)
        {
            int hashCode = GetHash(data);

            if (firstSeen.TryGetValue(hashCode, out var lastSeen))
            {
                return lastSeen;
            }

            firstSeen.TryAdd(GetHash(data), position - 1);
            return 0;
        }

        private static int GetHash(char[,] data)
        {
            int hashCode = 0;
            for (int x = 0; x < data.GetLength(1); x++)
            {
                for (int y = 0; y < data.GetLength(0); y++)
                {
                    hashCode = HashCode.Combine(hashCode, (int)data[y, x]);
                }
            }

            return hashCode;
        }

        private void Cycle(char[,] data)
        {
            // Tilt north
            Tilt(data, (x, y) => (x, y), data.GetLength(1), data.GetLength(0));

            // Tilt west
            Tilt(data, (x, y) => (y, x), data.GetLength(0), data.GetLength(1));

            // Tilt south
            Tilt(data, (x, y) => (x, data.GetLength(1) - 1 - y), data.GetLength(1), data.GetLength(0));

            // Tilt east
            Tilt(data, (x, y) => (data.GetLength(0) - 1 - y, x), data.GetLength(0), data.GetLength(1));
        }

        private char[,] GetData()
        {
            char[,] result = new char[Lines.Length, Lines[0].Length];

            for (int line = 0; line < Lines.Length; line++)
            {
                for (int column = 0; column < Lines[line].Length; column++)
                {
                    result[line, column] = Lines[line][column];
                }
            }

            return result;
        }

        private long ComputeTotal(char[,] data)
        {
            long total = 0;
            for (int y = 0; y < data.GetLength(0); y++)
            {
                for (int x = 0; x < data.GetLength(1); x++)
                {
                    if (data[y, x] == 'O')
                    {
                        total += data.GetLength(0) - y;
                    }
                }
            }
            return total;
        }

        private void Tilt(char[,] data, Func<int, int, (int x, int y)> getPosition, int width, int height)
        {
            Span<int> moveTo = stackalloc int[width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var (posX, posY) = getPosition(x, y);
                    switch (data[posY, posX])
                    {
                        case '.':
                            continue;
                        case '#':
                            moveTo[x] = y + 1;
                            break;
                        case 'O':
                            var (moveToX, moveToY) = getPosition(x, moveTo[x]);
                            data[posY, posX] = '.';
                            data[moveToY, moveToX] = 'O';
                            moveTo[x]++;
                            break;
                        default:
                            throw new Exception($"Invalid symbol '{data[posY, posX]}'.");
                    }
                }
            }
        }
    }
}