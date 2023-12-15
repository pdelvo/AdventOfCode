using AdventOfCodeLib;

using Alba.CsConsoleFormat;

using System.Diagnostics;
using System.Text;

using static System.ConsoleColor;

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
            List<(string dayText, string? test1, string? result1, double? time1InS, string? test2, string? result2, double? time2InS)> table = [];

            if (!Benchmark)
            {
#pragma warning disable CS0162 // Unreachable code detected
                Array.Reverse(instanceProvider.Days);
#pragma warning restore CS0162 // Unreachable code detected
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
                day.Lines = day.TestInput1.Split("\n", StringSplitOptions.TrimEntries);
                string testResult1 = day.RunPart1();
                day.Lines = day.TestInput2.Split("\n", StringSplitOptions.TrimEntries);
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



                (double time1InS, string result1) = SpeedTest(day.RunPart1);

                table[i] = table[i] with
                {
                    result1 = result1,
                    time1InS = time1InS
                };

                RenderTable(table);
                (double time2InS, string result2) = SpeedTest(day.RunPart2);

                table[i] = table[i] with
                {
                    result2 = result2,
                    time2InS = time2InS
                };

                RenderTable(table);
            }
        }

        static void RenderTable(List<(string dayText, string? test1, string? result1, double? time1InS, string? test2, string? result2, double? time2InS)> table)
        {
            var grid = new Grid() { Color = DarkGray };
            var document = new Document(grid);
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });

            grid.Children.Add(new Cell("🎄 Advent of Code 2023 🎄") { Color = Magenta, ColumnSpan = 7, Align = Align.Center });
            grid.Children.Add(new Cell("Day") { Color = Yellow, RowSpan = 2, Align = Align.Center, VerticalAlign = VerticalAlign.Center });
            grid.Children.Add(new Cell("Tests") { Color = Green, Align = Align.Center, RowSpan = 2 });
            grid.Children.Add(new Cell("Part 1") { Color = Yellow, ColumnSpan = 2, Align = Align.Center });
            grid.Children.Add(new Cell("Part 2") { Color = Yellow, ColumnSpan = 2, Align = Align.Center });
            grid.Children.Add(new Cell("result") { Color = Blue, Align = Align.Center });
            grid.Children.Add(new Cell("time") { Color = Red, Align = Align.Center });
            grid.Children.Add(new Cell("result") { Color = Blue, Align = Align.Center });
            grid.Children.Add(new Cell("time") { Color = Red, Align = Align.Center });

            foreach (var (dayText, test1, result1, time1InUs, test2, result2, time2InUs) in table)
            {
                grid.Children.Add(new Cell(dayText) { Color = White, Align = Align.Left });
                var testText = "";
                if (test1 != "Success")
                {
                    testText = test1;
                }
                else if (test2 != "Success")
                {
                    testText = test2;
                }
                else
                {
                    testText = "Success";
                }
                grid.Children.Add(new Cell(testText) { Color = White, Align = Align.Right });
                grid.Children.Add(new Cell(result1) { Color = White, Align = Align.Right });
                grid.Children.Add(new Cell(FormatTime(time1InUs)) { Color = White, Align = Align.Right });
                grid.Children.Add(new Cell(result2) { Color = White, Align = Align.Right });
                grid.Children.Add(new Cell(FormatTime(time2InUs)) { Color = White, Align = Align.Right });
            }

            //Console.Clear();
            Console.CursorVisible = false;
            var result = ConsoleRenderer.RenderDocumentToText(document, new AnsiRenderTarget());
            Console.Clear();
            Console.Write(result);
            Console.CursorVisible = true;
        }

        static (double timeInS, string result) SpeedTest(Func<string> method)
        {
            // Run once for JIT compilation, not needed for AOT compile
            method();

            string result = "";

            Stopwatch sw = Stopwatch.StartNew();
            method();
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            int iterations = 1;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            if (Benchmark)
            {
                iterations = Math.Max((int)(seconds / sw.Elapsed.TotalSeconds), 1);
            }
            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                result = method();
            }

            var totalTime = ((double)sw.ElapsedTicks / Stopwatch.Frequency) / iterations;

            return (totalTime, result);
        }

        static string FormatTime(double? timeInS)
        {
            if (timeInS == null)
            {
                return "-";
            }

            return FormatMetric(timeInS.Value) + "s";
        }
        static string FormatMetric(double input)
        {
            string prefix = "";
            var value = Math.Abs(input);
            if (value == 0 || (value >= 1 && value < 1000))
            {
            }
            else if (value >= 1000)
            {
                string[] prefixes =
                [
                    "",
                    "k",
                    "M",
                    "G",
                    "T",
                    "P",
                    "E",
                    "Z",
                    "Y",
                    "R",
                    "Q"
                ];
                int index = 0;
                while (value >= 1000)
                {
                    value /= 1000;
                    index++;
                }
                prefix = prefixes[index];
            }
            else
            {
                string[] prefixes =
                [
                    "",
                    "m",
                    "μ",
                    "n",
                    "p",
                    "f",
                    "a",
                    "z",
                    "y",
                    "r",
                    "q"
                ];
                int index = 0;
                while (value < 1)
                {
                    value *= 1000;
                    index++;
                }
                prefix = prefixes[index];
            }

            if (input < 0)
            {
                value *= -1;
            }

            return value.ToString("0.00") + " " + prefix;
        }
    }
}