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
#if DEBUG
        private const bool Benchmark = false;
#else
        private const bool Benchmark = true;
#endif

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
                table.Add((day.Name + (day.Description != null ? ":\r\n" + day.Description : ""), null, null, null, null, null, null));
            }
            RenderTable(table);
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
            int numberOfColumns = 2;
            var grid = new Grid() { Color = DarkGray };
            var document = new Document(grid);
            for (int i = 0; i < numberOfColumns; i++)
            {
                AddColumns(grid);
            }

            grid.Children.Add(new Cell("🎄 Advent of Code 2023 🎄") { Color = Magenta, ColumnSpan = 7 * numberOfColumns, Align = Align.Center });
            for (int i = 0; i < numberOfColumns; i++)
            {
                AddHeader1(grid);
            }
            for (int i = 0; i < numberOfColumns; i++)
            {
                AddHeader2(grid);
            }

            int perColumn = (int)Math.Ceiling((double)table.Count / numberOfColumns);
            for (int i = 0; i < perColumn; i++)
            {
                for (int j = 0; j < numberOfColumns; j++)
                {
                    int index = j * perColumn + i;
                    if (index < table.Count)
                    {
                        var row = table[index];
                        AddDay(grid, row);
                    }
                    else
                    {
                        AddDay(grid, default);
                    }
                }
            }

            Console.Clear();
            ConsoleRenderer.RenderDocument(document);
        }

        private static void AddColumns(Grid grid)
        {
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
        }

        private static void AddDay(Grid grid, (string dayText, string? test1, string? result1, double? time1InUs, string? test2, string? result2, double? time2InUs) row)
        {
            var (dayText, test1, result1, time1InUs, test2, result2, time2InUs) = row;
            grid.Children.Add(new Cell(dayText) { Color = White, Align = Align.Left });
            grid.Children.Add(new Cell(test1) { Color = test1 == "Success" ? Green : Red, Align = Align.Right });
            grid.Children.Add(new Cell(result1) { Color = White, Align = Align.Right });
            grid.Children.Add(new Cell(time1InUs?.ToString("0.00") + " µs") { Color = White, Align = Align.Right });
            grid.Children.Add(new Cell(test2) { Color = test2 == "Success" ? Green : Red, Align = Align.Right });
            grid.Children.Add(new Cell(result2) { Color = White, Align = Align.Right });
            grid.Children.Add(new Cell(time2InUs?.ToString("0.00") + " µs") { Color = White, Align = Align.Right });
        }

        private static void AddHeader1(Grid grid)
        {
            grid.Children.Add(new Cell("Day") { Color = Yellow, RowSpan = 2, Align = Align.Center, VerticalAlign = VerticalAlign.Center });
            grid.Children.Add(new Cell("Part 1") { Color = Yellow, ColumnSpan = 3, Align = Align.Center });
            grid.Children.Add(new Cell("Part 2") { Color = Yellow, ColumnSpan = 3, Align = Align.Center });
        }

        private static void AddHeader2(Grid grid)
        {
            grid.Children.Add(new Cell("test") { Color = Green, Align = Align.Center });
            grid.Children.Add(new Cell("result") { Color = Blue, Align = Align.Center });
            grid.Children.Add(new Cell("time") { Color = Red, Align = Align.Center });
            grid.Children.Add(new Cell("test") { Color = Green, Align = Align.Center });
            grid.Children.Add(new Cell("result") { Color = Blue, Align = Align.Center });
            grid.Children.Add(new Cell("time") { Color = Red, Align = Align.Center });
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

        static string ToStringSI(double value)
        {
            int exponent = 0;
            int offset = 0;
            if (value >= 1000)
            {
                offset = 3;
            }
            else if (value < 1)
            {
                offset = -3;
            }
            while (value is not (>= 1 and < 1000))
            {
                value = value * Math.Pow(10, -offset);
            }

            return value.ToString("0.00") + " ";
        }
    }
}