using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCodeLib.Days
{
    public class Day6 : Day
    {
        public override string? Description => "Wait For It";
        public override string TestInput1 => @"Time:      7  15   30
Distance:  9  40  200";
        public override string TestOutput1 => "288";
        public override string TestOutput2 => "71503";
        public override string RunPart1()
        {
            long[] timesBuffer = new long[100];
            long[] distanceBuffer = new long[100];
            var times = ParsingHelpers.ParseNumberList(timesBuffer, Lines[0].AsSpan()["Time: ".Length..]);
            var distances = ParsingHelpers.ParseNumberList(distanceBuffer, Lines[1].AsSpan()["Distance: ".Length..]);

            return ComputeSolution(times, distances).ToString();
        }

        private static int ComputeSolution(Span<long> times, Span<long> thresholds)
        {
            int result = 1;

            for (int i = 0; i < times.Length; i++)
            {
                double halfTime = times[i] / 2.0;
                double sqrt = Math.Sqrt(Math.Pow(halfTime, 2) - thresholds[i]);
                var lowSolution = halfTime - sqrt;
                var highSolution = halfTime + sqrt;

                var lowRounded = (int)Math.Floor(lowSolution) + 1;
                var highRounded = (int)Math.Ceiling(highSolution) - 1;

                var numberOfSolutions = highRounded - lowRounded + 1;

                result *= numberOfSolutions;
            }

            return result;
        }

        public override string RunPart2()
        {
            long[] timesBuffer = new long[100];
            long[] distanceBuffer = new long[100];
            var line0 = Lines[0].AsSpan()["Time: ".Length..].ToString().Replace(" ", "");
            var line1 = Lines[1].AsSpan()["Distance: ".Length..].ToString().Replace(" ", "");
            var times = ParsingHelpers.ParseNumberList(timesBuffer, line0);
            var distances = ParsingHelpers.ParseNumberList(distanceBuffer, line1);

            return ComputeSolution(times, distances).ToString();
        }
    }
}