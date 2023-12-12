namespace AdventOfCodeLib.Days
{
    public class Day3 : Day
    {
        public override string? Description => "Gear Ratios";
        public override string TestInput1 => @"467..114..
...*......
..35..633.
......#...
617*......
.....+.58.
..592.....
......755.
...$.*....
.664.598..";
        public override string TestOutput1 => "4361";
        public override string TestOutput2 => "467835";
        public override string RunPart1()
        {
            int sum = 0;
            string[] lines = Lines;

            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length;)
                {
                    if (char.IsDigit(lines[i][j]))
                    {
                        var (partNumber, length) = ParseIntAtPosition(lines[i], j);

                        if (IsSurroundedBySymbol(lines, i, j, length, null, out _))
                        {
                            sum += partNumber;
                        }

                        j += length;
                    }
                    else
                    {
                        j++;
                    }
                }
            }
            return sum.ToString();
        }

        readonly List<(int x, int y)> positions = [];
        private bool IsSurroundedBySymbol(string[] lines, int lineNumber, int lineStart, int length, char? symbol, out List<(int x, int y)> positions)
        {
            positions = this.positions;
            this.positions.Clear();

            for (int i = lineNumber - 1; i <= lineNumber + 1; i++)
            {
                for (int j = lineStart - 1; j <= lineStart + length; j++)
                {
                    if (IsSymbolSafe(lines, i, j, symbol))
                    {
                        positions.Add((i, j));
                    }
                }
            }

            return positions.Count > 0;
        }

        private static bool IsSymbolSafe(string[] lines, int lineNumber, int position, char? symbol)
        {
            if (lineNumber < 0 || lineNumber >= lines.Length || position < 0 || position >= lines[lineNumber].Length)
            {
                return false;
            }

            char x = lines[lineNumber][position];

            return !char.IsDigit(x) && (symbol == null ? x != '.' : x == symbol);
        }

        private static (int number, int length) ParseIntAtPosition(string str, int position)
        {
            for (int end = position; end < str.Length; end++)
            {
                if (!char.IsDigit(str[end]))
                {
                    return (int.Parse(str[position..end]), end - position);
                }
            }

            return (int.Parse(str[position..]), str.Length - position);
        }

        public override string RunPart2()
        {
            int sum = 0;
            string[] lines = Lines;

            Dictionary<(int x, int y), int> gears = [];
            Dictionary<(int x, int y), int> result = [];

            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length;)
                {
                    if (char.IsDigit(lines[i][j]))
                    {
                        var (partNumber, length) = ParseIntAtPosition(lines[i], j);

                        if (IsSurroundedBySymbol(lines, i, j, length, '*', out var positions))
                        {
                            foreach (var position in positions)
                            {
                                if(gears.TryGetValue(position, out var gear))
                                {
                                    if (gear == -1)
                                    {
                                        result[position] = 0;
                                    }
                                    else
                                    {
                                        result[position] = gear * partNumber;
                                        gears[position] = -1;
                                    }
                                }
                                else
                                {
                                    gears[position] = partNumber;
                                }
                            }
                        }

                        j += length;
                    }
                    else
                    {
                        j++;
                    }
                }
            }

            sum = result.Values.Sum();

            return sum.ToString();
        }
    }
}