namespace AdventOfCodeLib.Days
{
    public class Day9 : Day
    {
        public override string? Description => "Mirage Maintenance";
        public override string TestInput1 => @"0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45";
        public override string TestOutput1 => "114";
        public override string TestOutput2 => "2";
        public override string RunPart1()
        {
            int SolveAfter(int sum, int[] buffer, int numberLength, int currentSize)
            {
                int nextPosition = 0;
                for (int row = currentSize + 1; row <= numberLength; row++)
                {
                    int position = GetRowPosition(numberLength, row) + row - 1;
                    nextPosition = buffer[position] + nextPosition;
                }

                sum += nextPosition;
                return sum;
            }
            return Solve(SolveAfter).ToString();
        }

        private int Solve(Func<int, int[], int, int, int> solveFunction)
        {
            int sum = 0;

            int[] buffer = new int[1000];

            for (int lineIndex = 0; lineIndex < Lines.Length; lineIndex++)
            {
                var numbers = ParsingHelpers.ParseNumberList(buffer, Lines[lineIndex]);

                int expectedSize = GetTowerHeight(numbers.Length);
                if (expectedSize >= buffer.Length)
                {
                    Array.Resize(ref buffer, expectedSize);
                }
                Span<int> part = buffer;
                for (int currentSize = numbers.Length; currentSize >= 0;)
                {
                    for (int j = 0; j < currentSize - 1; j++)
                    {
                        part[currentSize + j] = part[j + 1] - part[j];
                    }
                    part = part[currentSize..];
                    currentSize--;
                    bool all0s = true;
                    for (int j = 0; j < currentSize; j++)
                    {
                        if (part[j] != 0)
                        {
                            all0s = false;
                            break;
                        }
                    }

                    if (all0s)
                    {
                        // Get next entry
                        sum = solveFunction(sum, buffer, numbers.Length, currentSize);
                        break;
                    }
                }
            }

            return sum;
        }

        public override string RunPart2()
        {
            int SolveBefore(int sum, int[] buffer, int numberLength, int currentSize)
            {
                // Get next entry
                int nextPosition = 0;
                for (int row = currentSize + 1; row <= numberLength; row++)
                {
                    int position = GetRowPosition(numberLength, row);
                    nextPosition = buffer[position] - nextPosition;
                }

                sum += nextPosition;
                return sum;
            }
            return Solve(SolveBefore).ToString();
        }

        private int GetRowPosition(int startingSize, int row)
        {
            var fullTowerSize = GetTowerHeight(startingSize);
            var rowTowerSize = GetTowerHeight(row);

            return Math.Max(fullTowerSize - rowTowerSize, 0);
        }

        private static int GetTowerHeight(int height) => ((height + 1) * height) / 2;
    }
}