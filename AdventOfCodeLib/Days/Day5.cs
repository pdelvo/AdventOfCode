using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCodeLib.Days
{
    public class Day5 : Day
    {
        public override string? Description => "If You Give A Seed A Fertilizer";
        public override string RunPart1()
        {
            var seeds = ReadList(Lines[0]);

            ReadOnlySpan<string> text = Lines[2..];
            int skip = 0;

            List<List<Mapper>> maps = new List<List<Mapper>>();

            while (text.Length > 0)
            {
                (var map, skip) = ReadMap(text);
                text = text[skip..];

                maps.Add(map);
            }

            for (int j = 0; j < maps.Count; j++)
            {
                for (int i = 0; i < seeds.Length; i++)
                {
                    seeds[i] = MapValue(maps[j], seeds[i]);
                }
            }

            long min = long.MaxValue;

            for (int i = 0; i < seeds.Length; i++)
            {
                if (min > seeds[i])
                {
                    min = seeds[i];
                }
            }

            return min.ToString();
        }

        public override string RunPart2()
        {
            ReadOnlySpan<LongRange> ranges = ReadRanges(Lines[0]);

            ReadOnlySpan<string> text = Lines[2..];
            int skip = 0;

            List<List<Mapper>> maps = new List<List<Mapper>>();

            while (text.Length > 0)
            {
                (var map, skip) = ReadMap(text);
                text = text[skip..];

                maps.Add(map);
            }

            for (int j = 0; j < maps.Count; j++)
            {
                ranges = GetNextRanges(ranges, maps[j]);
            }

            long min = long.MaxValue;

            for (int i = 0; i < ranges.Length; i++)
            {
                if (min > ranges[i].Start)
                {
                    min = ranges[i].Start;
                }
            }

            return min.ToString();
        }

        ReadOnlySpan<LongRange> GetNextRanges(ReadOnlySpan<LongRange> input, List<Mapper> mapper)
        {
            List<LongRange> result = new List<LongRange>();

            for (int i = 0; i < input.Length; i++)
            {
                var currentRange = input[i];
                int currentMapperIndex = 0;
                while (true)
                {
                    if (currentMapperIndex == mapper.Count)
                    {
                        result.Add(currentRange);
                        break;
                    }

                    var currentMapper = mapper[currentMapperIndex];

                    if (currentRange.Start < currentMapper.Range.Start)
                    {
                        if (currentRange.EndExclusive <= currentMapper.Range.Start)
                        {
                            result.Add(currentRange);

                            // Range done
                            break;
                        }
                        else
                        {
                            result.Add(LongRange.FromStartEnd(currentRange.Start, currentMapper.Range.Start));
                            currentRange = currentRange.FromNewStart(currentMapper.Range.Start);
                        }
                    }
                    else
                    {
                        if (currentRange.Start >= currentMapper.Range.EndExclusive)
                        {
                            currentMapperIndex++;
                            continue;
                        }
                        else
                        {
                            if (currentRange.EndExclusive <= currentMapper.Range.EndExclusive)
                            {
                                // Take the full range
                                result.Add(new LongRange(currentMapper.Map(currentRange.Start), currentRange.Length));
                                break;
                            }
                            else
                            {
                                // Take the partial range
                                result.Add(LongRange.FromStartEnd(currentMapper.Map(currentRange.Start), currentMapper.Range.EndExclusive + currentMapper.Offset));
                                currentRange = currentRange.FromNewStart(currentMapper.Range.EndExclusive);
                                currentMapperIndex++;
                                continue;
                            }
                        }
                    }
                }
            }

            return result.ToArray();
        }

        Span<long> ReadList(string text)
        {
            long[] seedList = new long[text.Length];

            return Tools.ParseNumberList(seedList, text.AsSpan()[6..]);
        }

        Span<LongRange> ReadRanges(string text)
        {
            long[] rangeList = new long[text.Length];

            var numbers = Tools.ParseNumberList(rangeList, text.AsSpan()[6..]);

            LongRange[] ranges = new LongRange[numbers.Length / 2];

            for (int i = 0; i < numbers.Length / 2; i++)
            {
                ranges[i] = new LongRange(numbers[2 * i], numbers[2 * i + 1]);
            }

            return ranges;
        }

        (List<Mapper> result, int skippedLines) ReadMap(ReadOnlySpan<string> text)
        {
            int skippedLines = 0;
            while (!text[0].Contains(':'))
            {
                text = text[1..];
                skippedLines++;
            }
            text = text[1..];
            skippedLines++;

            long[] parsedLine = new long[3];

            List<Mapper> result = new List<Mapper>(text.Length);

            for (int i = 0; text.Length > 0 && text[0].Length > 0; i++)
            {

                Tools.ParseNumberList(parsedLine, text[0]);

                result.Add(Mapper.FromData(parsedLine[1], parsedLine[0], parsedLine[2]));

                text = text[1..];
                skippedLines++;
            }

            result.Sort();

            return (result, skippedLines);
        }

        long MapValue(List<Mapper> map, long value)
        {
            int index = map.BinarySearch(Mapper.FromData(value, 0, 0));

            if (index < 0)
            {
                index = ~index - 1;
            }

            if (index == -1)
            {
                return value;
            }

            return map[index].Map(value);
        }

        record struct Mapper (LongRange Range, long Offset) : IComparable<Mapper>, IEquatable<Mapper>
        {
            public int CompareTo(Mapper other)
            {
                return Range.Start.CompareTo(other.Range.Start);
            }

            public static Mapper FromData(long fromStart, long toStart, long length)
            {
                return new Mapper(new LongRange(fromStart, length), toStart - fromStart);
            }

            public long Map(long input)
            {
                if (Range.Contains(input))
                {
                    return input + Offset;
                }

                return input;
            }
        }

        record struct LongRange(long Start, long Length)
        {
            public long EndExclusive => Start + Length;

            public static LongRange FromStartEnd(long start, long endExclusive) => new LongRange(start, endExclusive - start);

            public bool Contains(long value)
            {
                return value >= Start && value < Start + Length;
            }

            public LongRange FromNewStart(long start)
            {
                return new LongRange(start, Length - (start - Start));
            }
        }
    }
}