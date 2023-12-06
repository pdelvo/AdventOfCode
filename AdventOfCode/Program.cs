using AdventOfCode.Days;

using Alba.CsConsoleFormat;

using System.Diagnostics;
using System.Text;

using static System.ConsoleColor;

namespace AdventOfCode
{
    public partial class Program
    {
        private const double seconds = 1;

        static Program()
        {
            Console.OutputEncoding = Encoding.UTF8;
        }

        static void Main(string[] args)
        {
            IInstanceProvider instanceProvider = new AdventOfCodeDownloader(2023);
            List<(string dayText, string? result1, double? time1InUs, string? result2, double? time2InUs)> table = new();
            for (int i = 0; i < instanceProvider.Days.Length; i++)
            {
                var day = instanceProvider.Days[i];

                if (day == null)
                {
                    continue;
                }
                table.Add((day.Name + (day.Description != null ? ": " + day.Description : ""), null, null, null, null));
                RenderTable(table);
            }
            for (int i = 0; i < instanceProvider.Days.Length; i++)
            {
                var day = instanceProvider.Days[i];

                if (day == null)
                {
                    continue;
                }

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

        static void RenderTable(List<(string dayText, string? result1, double? time1InUs, string? result2, double? time2InUs)> table)
        {
            var grid = new Grid() { Color = DarkGray };
            var document = new Document(grid);
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });

            grid.Children.Add(new Cell("🎄 Advent of Code 2023 🎄") { Color = Magenta, ColumnSpan = 5, Align = Align.Center });
            grid.Children.Add(new Cell("Day") { Color = Yellow, RowSpan = 2, Align = Align.Center, VerticalAlign = VerticalAlign.Center });
            grid.Children.Add(new Cell("Part 1") { Color = Yellow, ColumnSpan = 2, Align = Align.Center });
            grid.Children.Add(new Cell("Part 2") { Color = Yellow, ColumnSpan = 2, Align = Align.Center });
            grid.Children.Add(new Cell("result") { Color = Blue, Align = Align.Center });
            grid.Children.Add(new Cell("time") { Color = Red, Align = Align.Center });
            grid.Children.Add(new Cell("result") { Color = Blue, Align = Align.Center });
            grid.Children.Add(new Cell("time") { Color = Red, Align = Align.Center });

            foreach (var (dayText, result1, time1InUs, result2, time2InUs) in table)
            {
                grid.Children.Add(new Cell(dayText) { Color = White, Align = Align.Left });
                grid.Children.Add(new Cell(result1) { Color = White, Align = Align.Right });
                grid.Children.Add(new Cell(time1InUs?.ToString("0.00") + " µs") { Color = White, Align = Align.Right });
                grid.Children.Add(new Cell(result2) { Color = White, Align = Align.Right });
                grid.Children.Add(new Cell(time2InUs?.ToString("0.00") + " µs") { Color = White, Align = Align.Right });
            }

            Console.Clear();
            ConsoleRenderer.RenderDocument(document);
        }

        static (double timeInUs, string result) SpeedTest(Func<string> method)
        {
            // Run once for JIT compilation, not needed for AOT compile
            //method();

            string result = "";

            Stopwatch sw = Stopwatch.StartNew();
            //();
            int iterations = 1; // Math.Max((int)(seconds / sw.Elapsed.TotalSeconds), 1);
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