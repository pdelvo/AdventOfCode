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

        public override string TestOutput2 => "525152";
        public override string RunPart1()
        {
            long sum = 0;

            int[] counters = new int[20];
            for (int i = 0;i < Lines.Length; i++)
            {
                memoization.Clear();
                ReadOnlySpan<char> record;
                Span<int> lineCounters = ParseLine(Lines[i], counters, out record);

                long count = CountOptions(record, lineCounters);
                sum += count;
            }

            return sum.ToString();
        }

        public override string RunPart2()
        {
            long sum = 0;

            int[] counters = new int[20];
            for (int i = 0; i < Lines.Length; i++)
            {
                memoization.Clear();
                ReadOnlySpan<char> record;
                Span<int> lineCounters = ParseLine(Lines[i], counters, out record);

                Span<char> recordMultiplied = new char[record.Length * 5 + 4];

                for (int j = 0; j < 5; j++)
                {
                    record.CopyTo(recordMultiplied[((record.Length + 1) * j)..]);

                    if (j != 4)
                    {
                        recordMultiplied[(record.Length + 1) * (j + 1) - 1] = '?';
                    }
                }

                Span<int> lineCountersMultiplied = new int[lineCounters.Length * 5];

                for (int j = 0; j < 5; j++)
                {
                    lineCounters.CopyTo(lineCountersMultiplied[(lineCounters.Length * j)..]);
                }

                long count = CountOptions(recordMultiplied, lineCountersMultiplied);
                sum += count;
            }

            return sum.ToString();
        }
        Dictionary<(int recordLength, int lineCounterLength), long> memoization = new Dictionary<(int recordLength, int lineCounterLength), long>();

        private long CountOptions(ReadOnlySpan<char> record, ReadOnlySpan<int> lineCounters)
        {
            if (memoization.TryGetValue((record.Length, lineCounters.Length), out var memoizationResult))
            {
                return memoizationResult;
            }
            if (lineCounters.Length == 0)
            {
                return record.Contains('#') ? 0 : 1;
            }

            Span<Range> parts = stackalloc Range[20];
            int partCount = record.Split(parts, '.', StringSplitOptions.RemoveEmptyEntries);
            Span<int> recordCounters = stackalloc int[20];

            for (int i = 0; i < partCount; i++)
            {
                recordCounters[i] = parts[i].End.Value - parts[i].Start.Value;
            }
            recordCounters = recordCounters[0..partCount];
            parts = parts[0..partCount];
            long counter = 0;

            for (int partToPlaceIn = 0; partToPlaceIn < partCount; partToPlaceIn++)
            {
                var currentRecord = record[parts[partToPlaceIn]];
                for (int offset = 0; offset <= currentRecord.Length - lineCounters[0]; offset++)
                {
                    if (parts[partToPlaceIn].Start.Value + offset + lineCounters[0] == record.Length)
                    {
                        counter += lineCounters.Length == 1 ? 1 : 0;
                    }
                    else
                    {
                        if (offset + lineCounters[0] < currentRecord.Length && currentRecord[offset + lineCounters[0]] == '#')
                        {
                            if (currentRecord[offset] == '#')
                            {
                                memoization.Add((record.Length, lineCounters.Length), counter);
                                return counter;
                            }
                            continue;
                        }
                        counter += CountOptions(record[(parts[partToPlaceIn].Start.Value + offset + lineCounters[0] + 1)..], lineCounters[1..]);

                        if (currentRecord[offset] == '#')
                        {
                            memoization.Add((record.Length, lineCounters.Length), counter);
                            return counter;
                        }
                    }
                }

                if (currentRecord.Contains('#'))
                {
                    break;
                }
            }

            memoization.Add((record.Length, lineCounters.Length), counter);
            return counter;
        }

        private Span<int> ParseLine(string input, int[] counters, out ReadOnlySpan<char> record)
        {
            int splitPosition = input.IndexOf(' ');

            record = input.AsSpan()[0..splitPosition];

            return ParsingHelpers.ParseNumberList(counters, input.AsSpan()[(splitPosition + 1)..], ',');
        }
    }
}