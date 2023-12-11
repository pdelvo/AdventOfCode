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
            var instructions = ReadData(out Dictionary<int, (int left, int right)> map);

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
            var instructions = ReadData(out Dictionary<int, (int left, int right)> map).ToArray();


            int[] start = map.Keys.Where(x => (x & 0xFF) == 'A').ToArray();
            List<(int name, long distance)>[] counter = new List<(int name, long distance)>[start.Length];

            Parallel.For(0, start.Length, i=>
            {
                counter[i] = AnalyzeRoute(map, instructions, start[i]);
            });

            var list = counter[0];

            var totalCounter = list[0].distance;
            for (int i = 1; i < counter.Length; i++)
            {
                totalCounter = AOCMath.LeastCommonMultiple(totalCounter, counter[i][0].distance);
            }

            return totalCounter.ToString();
        }

        private static List<(int name, long distance)> AnalyzeRoute(Dictionary<int, (int left, int right)> map, ReadOnlySpan<char> instructions, int start)
        {
            List<(int name, long distance)> endPoints = [];
            int counter = 0;
            while (true)
            {
                if ((start & 0xFF) == 'Z')
                {
                    if (endPoints.Count > 0 && endPoints[0].name == start)
                    {
                        return endPoints;
                    }
                    else
                    {
                        endPoints.Add((start, counter));
                    }
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
            map = [];

            for (int i = 2; i < Lines.Length; i++)
            {
                int start = NodeNameToInt(Lines[i].AsSpan()[0..3]);
                int left = NodeNameToInt(Lines[i].AsSpan()[7..10]);
                int right = NodeNameToInt(Lines[i].AsSpan()[12..15]);

                map.Add(start, (left, right));
            }

            return Lines[0];
        }

        private static int NodeNameToInt(ReadOnlySpan<char> nodeName)
        {
            return (nodeName[0] << 16) | (nodeName[1] << 8) | nodeName[2];
        }
    }
}