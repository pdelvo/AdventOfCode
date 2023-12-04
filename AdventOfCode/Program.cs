using AdventOfCode.Days;

using System.Diagnostics;

namespace AdventOfCode
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var day = new Day4();

            day.RunPart1();

            // Run once for JIT compilation, not needed for AOT compile
            day.RunPart2();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            day.RunPart2();
            Console.WriteLine((watch.ElapsedTicks / 10) +  " us");
        }
    }
}