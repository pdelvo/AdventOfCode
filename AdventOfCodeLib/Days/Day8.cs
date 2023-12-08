using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCodeLib.Days
{
    public class Day8 : Day
    {
        public override string? Description => "Haunted Wasteland";
        public override string TestInput1 => @"LLR

AAA = (BBB, BBB)
BBB = (AAA, ZZZ)
ZZZ = (ZZZ, ZZZ)";
        public override string TestOutput1 => "6";
        public override string TestInput2 => @"LR

11A = (11B, XXX)
11B = (XXX, 11Z)
11Z = (11B, XXX)
22A = (22B, XXX)
22B = (22C, 22C)
22C = (22Z, 22Z)
22Z = (22B, 22B)
XXX = (XXX, XXX)";
        public override string TestOutput2 => "6";
        public override string RunPart1()
        {
            Dictionary<int, (int left, int right)> map;
            var instructions = ReadData(out map);

            int counter = 0;

            int start = NodeNameToInt("AAA");
            int end = NodeNameToInt("ZZZ");

            while (start != end)
            {
                var instruction = instructions[counter++ % instructions.Length];
                var (left, right) = map[start];

                if (instruction == 'L')
                {
                    start = left;
                }
                else
                {
                    start = right;
                }
            }

            return counter.ToString();
        }

        public override string RunPart2()
        {
            Dictionary<int, (int left, int right)> map;
            var instructions = ReadData(out map).ToArray();


            int[] start = map.Keys.Where(x => (x & 0xFF) == 'A').ToArray();
            long[] counter = new long[start.Length];

            Parallel.For(0, start.Length, i=>
            {
                counter[i] = GetLength(map, instructions, start[i]);
            });

            long totalCounter = counter[0];
            for (int i = 1; i < counter.Length; i++)
            {
                totalCounter = LeastCommonMultiple(totalCounter, counter[i]);
            }

            return totalCounter.ToString();
        }

        private long LeastCommonMultiple(long a, long b)
        {
            return a * (b / GreatestCommonDivisor(a, b));
        }

        private (long min, long max) MinMax(long a, long b)
        {
            if (a < b)
            {
                return (a, b);
            }

            return (b, a);
        }

        private long GreatestCommonDivisor(long a, long b)
        {
            while (true)
            {
                (a, b) = MinMax(a, b);

                if (a == 0)
                {
                    return b;
                }

                b = b % a;
            }
        }

        private static long GetLength(Dictionary<int, (int left, int right)> map, ReadOnlySpan<char> instructions, int start)
        {
            int counter = 0;
            while (true)
            {
                if ((start & 0xFF) == 'Z')
                {
                    return counter;
                }

                var instruction = instructions[counter++ % instructions.Length];
                var (left, right) = map[start];

                if (instruction == 'L')
                {
                    start = left;
                }
                else
                {
                    start = right;
                }
            }
        }

        private ReadOnlySpan<char> ReadData(out Dictionary<int, (int, int)> map)
        {
            map = new Dictionary<int, (int, int)>();

            for (int i = 2; i < Lines.Length; i++)
            {
                int start = NodeNameToInt(Lines[i].AsSpan()[0..3]);
                int left = NodeNameToInt(Lines[i].AsSpan()[7..10]);
                int right = NodeNameToInt(Lines[i].AsSpan()[12..15]);

                map.Add(start, (left, right));
            }

            return Lines[0];
        }

        private int NodeNameToInt(ReadOnlySpan<char> nodeName)
        {
            return (nodeName[0] << 16) | (nodeName[1] << 8) | nodeName[2];
        }
    }
}