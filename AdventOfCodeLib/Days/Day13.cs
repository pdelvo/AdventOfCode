using System.Numerics;
using System.Runtime.InteropServices;

namespace AdventOfCodeLib.Days
{
    public class Day13 : Day
    {
        public override string? Description => "Point of Incidence";
        public override string TestInput1 => @"#.##..##.
..#.##.#.
##......#
##......#
..#.##.#.
..##..##.
#.#.##.#.

#...##..#
#....#..#
..##..###
#####.##.
#####.##.
..##..###
#....#..#";
        public override string TestOutput1 => "405";

        public override string TestOutput2 => "400";
        public override string RunPart1()
        {
            int lineCounter = 0;
            int sum = 0;

            while (lineCounter < Lines.Length)
            {
                sum += ProcessPart(ref lineCounter);
            }
            return sum.ToString();
        }

        public override string RunPart2()
        {
            int lineCounter = 0;
            int sum = 0;

            while (lineCounter < Lines.Length)
            {
                sum += ProcessPart(ref lineCounter, 1);
            }
            return sum.ToString();
        }

        private int ProcessPart(ref int lineCounter, int numberOfSmudges = 0)
        {
            int width = Lines[lineCounter].Length;
            List<long> rows = new List<long>();
            long[] columns = new long[width];
            int height = 0;
            while (lineCounter < Lines.Length && Lines[lineCounter] != "")
            {
                rows.Add(0);
                for (int i = 0; i < Lines[lineCounter].Length; i++)
                {
                    rows[height] |= (Lines[lineCounter][i] == '#' ? 1u : 0u) << i;
                    columns[i] |= (Lines[lineCounter][i] == '#' ? 1u : 0u) << height;
                }

                height++;
                lineCounter++;
            }
            lineCounter++;

            int rowValue = 100 * GetMirrorValue(CollectionsMarshal.AsSpan(rows), numberOfSmudges);

            if (rowValue > 0)
            {
                return rowValue;
            }

            int columnValue = GetMirrorValue(columns, numberOfSmudges);

            if (columnValue > 0)
            {
                return columnValue;
            }

            return 0;
        }

        private int GetMirrorValue(ReadOnlySpan<long> data, int numberOfSmudges = 0)
        {
            int mirrorIndex = 0;

            // Try to find mirror
            for (; mirrorIndex < data.Length - 1; mirrorIndex++)
            {
                int foundDifferences = 0;
                for (int j = 0; j < data.Length; j++)
                {
                    int index1 = mirrorIndex - j;
                    int index2 = mirrorIndex + j + 1;
                    if (index1 < 0 || index2 >= data.Length)
                    {
                        if (foundDifferences == numberOfSmudges)
                        {
                            return mirrorIndex + 1;
                        }
                        break;
                    }

                    long difference = data[index1] ^ data[index2];
                    foundDifferences += BitOperations.PopCount((ulong)difference);

                    if (foundDifferences > numberOfSmudges)
                    {
                        break;
                    }
                }
            }

            return 0;
        }
    }
}