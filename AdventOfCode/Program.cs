using AdventOfCodeLib;
using AdventOfCodeLib.Days;

using Alba.CsConsoleFormat;

using System.Diagnostics;
using System.Text;

using static System.ConsoleColor;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode
{
    public partial class Program
    {
        private const double seconds = 1;
        private const bool Benchmark = false;

        static Program()
        {
            Console.OutputEncoding = Encoding.UTF8;
        }

        static void Main(string[] args)
        {
            InstanceProvider instanceProvider = new AdventOfCodeDownloader(2023);
            List<(string dayText, string? test1, string? result1, double? time1InUs, string? test2, string? result2, double? time2InUs)> table = [];

            if (!Benchmark)
            {
                Array.Reverse(instanceProvider.Days);
            }

            for (int i = 0; i < instanceProvider.Days.Length; i++)
            {
                var day = instanceProvider.Days[i];

                if (day == null)
                {
                    continue;
                }
                table.Add((day.Name + (day.Description != null ? ": " + day.Description : ""), null, null, null, null, null, null));
                RenderTable(table);
            }
            for (int i = 0; i < instanceProvider.Days.Length; i++)
            {
                var day = instanceProvider.Days[i];

                if (day == null)
                {
                    continue;
                }
                day.Lines = day.TestInput1.Split("\r\n");
                string testResult1 = day.RunPart1();
                day.Lines = day.TestInput2.Split("\r\n");
                string testResult2 = day.RunPart2();
                if (testResult1 == day.TestOutput1)
                {
                    table[i] = table[i] with
                    {
                        test1 = "Success"
                    };
                }
                else
                {
                    table[i] = table[i] with
                    {
                        test1 = $"Expected: {day.TestOutput1}, Actual: {testResult1}"
                    };
                }
                if (testResult2 == day.TestOutput2)
                {
                    table[i] = table[i] with
                    {
                        test2 = "Success"
                    };
                }
                else
                {
                    table[i] = table[i] with
                    {
                        test2 = $"Expected: {day.TestOutput2}, Actual: {testResult2}"
                    };
                }
                RenderTable(table);

                day.Initialize(instanceProvider);



                (double time1InUs, string result1) = SpeedTest(day.RunPart1);

                table[i] = table[i] with
                {
                    result1 = result1,
                    time1InUs = time1InUs
                };

                RenderTable(table);
                (double time2InUs, string result2) = SpeedTest(day.RunPart2);

                table[i] = table[i] with
                {
                    result2 = result2,
                    time2InUs = time2InUs
                };

                RenderTable(table);
            }
        }

        static void RenderTable(List<(string dayText, string? test1, string? result1, double? time1InUs, string? test2, string? result2, double? time2InUs)> table)
        {
            var grid = new Grid() { Color = DarkGray };
            var document = new Document(grid);
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });

            grid.Children.Add(new Cell("🎄 Advent of Code 2023 🎄") { Color = Magenta, ColumnSpan = 7, Align = Align.Center });
            grid.Children.Add(new Cell("Day") { Color = Yellow, RowSpan = 2, Align = Align.Center, VerticalAlign = VerticalAlign.Center });
            grid.Children.Add(new Cell("Part 1") { Color = Yellow, ColumnSpan = 3, Align = Align.Center });
            grid.Children.Add(new Cell("Part 2") { Color = Yellow, ColumnSpan = 3, Align = Align.Center });
            grid.Children.Add(new Cell("test") { Color = Green, Align = Align.Center });
            grid.Children.Add(new Cell("result") { Color = Blue, Align = Align.Center });
            grid.Children.Add(new Cell("time") { Color = Red, Align = Align.Center });
            grid.Children.Add(new Cell("test") { Color = Green, Align = Align.Center });
            grid.Children.Add(new Cell("result") { Color = Blue, Align = Align.Center });
            grid.Children.Add(new Cell("time") { Color = Red, Align = Align.Center });

            foreach (var (dayText, test1, result1, time1InUs, test2, result2, time2InUs) in table)
            {
                grid.Children.Add(new Cell(dayText) { Color = White, Align = Align.Left });
                grid.Children.Add(new Cell(test1) { Color = White, Align = Align.Right });
                grid.Children.Add(new Cell(result1) { Color = White, Align = Align.Right });
                grid.Children.Add(new Cell(time1InUs?.ToString("0.00") + " µs") { Color = White, Align = Align.Right });
                grid.Children.Add(new Cell(test2) { Color = White, Align = Align.Right });
                grid.Children.Add(new Cell(result2) { Color = White, Align = Align.Right });
                grid.Children.Add(new Cell(time2InUs?.ToString("0.00") + " µs") { Color = White, Align = Align.Right });
            }

            Console.Clear();
            ConsoleRenderer.RenderDocument(document);
        }

        static (double timeInUs, string result) SpeedTest(Func<string> method)
        {
            // Run once for JIT compilation, not needed for AOT compile
            method();

            string result = "";

            Stopwatch sw = Stopwatch.StartNew();
            method();
            int iterations = 1;
            if (Benchmark)
            {
                iterations = Math.Max((int)(seconds / sw.Elapsed.TotalSeconds), 1);
            }
            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                result = method();
            }

            var totalTime = ((double)sw.ElapsedTicks / Stopwatch.Frequency) * Math.Pow(10, 6) / iterations;

            return (totalTime, result);
        }
    }
}