using AdventOfCodeLib.Days;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static AdventOfCodeLib.Days.Day5;

namespace AdventOfCodeLib
{
    public class HardInstanceProvider : IInstanceProvider
    {
        Random random = new Random(0);
        public Day[] Days => [new Day5()];
        public string[] GetInstance(int day)
        {
            switch (day)
            {
                case 1:
                    return GenerateDay1();
                case 5:
                    return GenerateDay5();
            }

            return null;
        }

        private string[] GenerateDay1()
        {
            int lineCount = 1000000;
            var lines = new string[lineCount];

            int lineLengthMin = 500;
            int lineLengthMax = 1000;
            int minNumberOfNumberNames = 5, maxNumberOfNumberNames = 300;

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";

            for (int i = 0; i < lineCount; i++)
            {
                char[] result = new char[random.Next(lineLengthMin, lineLengthMax)];
                for (int j = 0; j < result.Length; j++)
                {
                    result[j] = chars[random.Next(chars.Length)];
                }

                int numberOfNumberNames = random.Next(minNumberOfNumberNames, maxNumberOfNumberNames);

                for (int j = 0; j < numberOfNumberNames; j++)
                {
                    string numberName = Day1.NumberNameList[random.Next(0)];
                    numberName.AsSpan().CopyTo(result[random.Next(0, result.Length - numberName.Length)..]);
                }

                lines[i] = new string(result);
            }

            return lines;

        }

        private string[] GenerateDay5()
        {
            LongRange workingRange = new LongRange(0, 200000000000000);

            int numberOfEvilComponents = 100;
            int evilComponentSplitCount = 1000;
            int evilComponentSplitSize = 1000;
            int evilComponentSize = evilComponentSplitCount * evilComponentSplitSize * 2;

            List<Mapper>[] result = new List<Mapper>[7];
            List<LongRange> startingRanges = new List<LongRange>();

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new List<Mapper> ();
            }

            Random random = new Random(0);

            int numberOfComponentsPlaced = 0;

            while (numberOfComponentsPlaced < numberOfEvilComponents)
            {
                var start = random.NextInt64(workingRange.Start, workingRange.EndExclusive - evilComponentSize);
                var range = new LongRange(start, evilComponentSize);

                if (startingRanges.Any(x => x.Intersects(range)))
                {
                    continue;
                }

                List<Mapper> part1 = PlaceEvilComponent(range, evilComponentSize, evilComponentSplitCount, evilComponentSplitSize);
                List<Mapper> part2 = PlaceEvilComponent(range.Offset(evilComponentSize + evilComponentSplitSize / 2), -evilComponentSize, evilComponentSplitCount, evilComponentSplitSize);

                result[0].AddRange(part1);
                result[1].AddRange(part2);
                result[2].AddRange(part1.Select(x => new Mapper(x.Range, x.Offset + random.NextInt64(-evilComponentSplitSize / 2, evilComponentSplitSize / 2))));
                result[3].AddRange(part2.Select(x => new Mapper(x.Range, x.Offset + random.NextInt64(-evilComponentSplitSize / 2, evilComponentSplitSize / 2))));
                result[4].AddRange(part1.Select(x => new Mapper(x.Range, x.Offset + random.NextInt64(-evilComponentSplitSize / 2, evilComponentSplitSize / 2))));
                result[5].AddRange(part2.Select(x => new Mapper(x.Range, x.Offset + random.NextInt64(-evilComponentSplitSize / 2, evilComponentSplitSize / 2))));
                result[6].AddRange(part1.Select(x => new Mapper(x.Range, x.Offset + random.NextInt64(-evilComponentSplitSize / 2, evilComponentSplitSize / 2))));

                startingRanges.Add(range);
                numberOfComponentsPlaced++;
            }


            var output = FormatDay5(startingRanges, result);

            File.WriteAllLines("output.txt", output);

            return output;
        }

        private static List<Mapper> PlaceEvilComponent(LongRange range, int evilComponentSize, int evilComponentSplitCount, int evilComponentSplitSize)
        {
            List<Mapper> result = new List<Mapper> ();
            for (int i = 0; i < evilComponentSplitCount; i++)
            {
                long splitStart = range.Start + i * range.Length / evilComponentSplitCount;

                result.Add(new Mapper(new LongRange(splitStart, evilComponentSplitSize), evilComponentSize));
            }

            return result;
        }

        private string[] FormatDay5(List<LongRange> startingRanges, List<Mapper>[] result)
        {
            List<string> resultBuilder = new List<string> ();

            resultBuilder.Add("seeds: " + string.Join(' ', startingRanges.Select(x => x.Start + " " + x.Length)));

            string[] mapNames = ["seed-to-soil",
                "soil-to-fertilizer",
                "fertilizer-to-water",
                "water-to-light",
                "light-to-temperature",
                "temperature-to-humidity",
                "humidity-to-location"];

            for (int i = 0; i < result.Length; i++)
            {
                resultBuilder.Add("");
                resultBuilder.Add(mapNames[i] + " map:");

                for (int j = 0; j < result[i].Count; j++)
                {
                    var mapper = result[i][j];

                    resultBuilder.Add((mapper.Range.Start + mapper.Offset) + " " + mapper.Range.Start + " " + mapper.Range.Length);
                }
            }

            return resultBuilder.ToArray();
        }
    }
}
