namespace AdventOfCodeLib.Days
{
    public class Day12 : Day
    {
        public override string? Description => "Hot Springs";
        public override string TestInput1 => @"???.### 1,1,3
.??..??...?##. 1,1,3
?#?#?#?#?#?#?#? 1,3,1,6
????.#...#... 4,1,1
????.######..#####. 1,6,5
?###???????? 3,2,1";
        public override string TestOutput1 => "21";

        public override string TestOutput2 => "Test";
        public override string RunPart1()
        {
            long sum = 0;

            int[] counters = new int[20];
            for (int i = 0;i < Lines.Length; i++)
            {
                ReadOnlySpan<char> record;
                Span<int> lineCounters = ParseLine(Lines[i], counters, out record);
                
                sum += CountOptions(record, lineCounters);
            }

            return sum.ToString();
        }

        public override string RunPart2()
        {
            return "";
        }

        private long CountOptions(ReadOnlySpan<char> record, ReadOnlySpan<int> lineCounters)
        {
            Span<Range> parts = stackalloc Range[20];
            int partCount = record.Split(parts, '.', StringSplitOptions.RemoveEmptyEntries);
            Span<int> recordCounters = stackalloc int[20];

            for (int i = 0; i < partCount; i++)
            {
                recordCounters[i] = parts[i].End.Value - parts[i].Start.Value;
            }

            recordCounters = recordCounters[0..partCount];

            // Use a dynamic program.
            // dynamicProgramData[i, j] gives the number of options to distribute the first j lineCounters
            // in the first i sections
            long[,] dynamicProgramData = new long[recordCounters.Length + 1, lineCounters.Length + 1];

            for (int i = 0; i < recordCounters.Length + 1; i++)
            {
                for (int j = 0; j < lineCounters.Length + 1; j++)
                {
                    if (i == 0 || j == 0)
                    {
                        dynamicProgramData[i, j] = 1;
                    }

                    // k is how many should be distributed in the current part
                    for (int k = 0; k <= j; k++)
                    {
                        dynamicProgramData[i, j] = Math.Max(dynamicProgramData[i, j],
                            CountOptions(recordCounters[j], lineCounters[(j - k)..j]) + dynamicProgramData[i, j - k]);
                    }
                }
            }

            return dynamicProgramData[recordCounters.Length, lineCounters.Length];
        }

        private long CountOptions(int partLength,  ReadOnlySpan<int> lineCounters)
        {
            var counterLength = lineCounters.Sum();

            // We need to distribute remainder onto lineCounters.Length - 1 many non-empty buckets
            int remainder = partLength - counterLength;
            if (remainder < 0)
            {
                return 0;
            }

            // We can use stirling numbers of the 2nd kind for this

            return AOCMath.StirlingNumber2ndKind(remainder, lineCounters.Length - 1);
        }

        private Span<int> ParseLine(string input, int[] counters, out ReadOnlySpan<char> record)
        {
            int splitPosition = input.IndexOf(' ');

            record = input.AsSpan()[0..splitPosition];

            return ParsingHelpers.ParseNumberList(counters, input.AsSpan()[(splitPosition + 1)..], ',');
        }
    }
}