using AdventOfCode.Days;

using Alba.CsConsoleFormat;

using System.Diagnostics;

using static System.ConsoleColor;

namespace AdventOfCode
{
    public partial class Program
    {
        static int[] padding;
        static string[] header = ["Day", "Part 1 result", "Part 1 time", "Part 2 result", "Part 2 time"];

        static Program()
        {
            padding = header.Select(x => x.Length).ToArray();
        }

        static void Main(string[] args)
        {
            InstanceDownloader instanceDownloader = new InstanceDownloader(2023);
            List<List<object>> table = new List<List<object>>();

            for (int i = 0; i < Days.Length; i++)
            {
                var day = Days[i];

                if(day == null)
                {
                    continue;
                }
                table.Add([i + 1]);
                RenderTable(table);

                day.Initialize(instanceDownloader);

                (double timeInUs1, int result1) = SpeedTest(day.RunPart1);
                table[i].AddRange([result1, timeInUs1.ToString("0.00") + " µs"]);
                RenderTable(table);
                (double timeInUs2, int result2) = SpeedTest(day.RunPart2);
                table[i].AddRange([result2, timeInUs2.ToString("0.00") + " µs"]);
                //table.AddRow(i + 1, result1, timeInUs1 + " µs", result2, timeInUs2 + " µs");
                RenderTable(table);
            }
        }

        static void RenderTable(List<List<object>> table)
        {
            var grid = new Grid() { Color = DarkGray };
            var document = new Document(new Span("Advent of Code") { Color = Yellow }, "\n", grid);
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });
            grid.Columns.Add(new Column { Width = GridLength.Auto });

            grid.Children.Add(new Cell("Day") { Color = Yellow, RowSpan = 2, Align = Align.Center });
            grid.Children.Add(new Cell("Part 1") { Color = Yellow, ColumnSpan = 2, Align = Align.Center });
            grid.Children.Add(new Cell("Part 2") { Color = Yellow, ColumnSpan = 2, Align = Align.Center });
            grid.Children.Add(new Cell("result") { Color = Blue, Align = Align.Center });
            grid.Children.Add(new Cell("time") { Color = Red, Align = Align.Center });
            grid.Children.Add(new Cell("result") { Color = Blue, Align = Align.Center });
            grid.Children.Add(new Cell("time") { Color = Red, Align = Align.Center });

            foreach (var row in table)
            {
                foreach (var rowCell in row)
                {
                    grid.Children.Add(new Cell(rowCell) { Color = White, Align = Align.Right });
                }
            }

            Console.Clear();
            ConsoleRenderer.RenderDocument(document);
        }

        static void PrintTableRow(params object[] parts)
        {
            Console.WriteLine(string.Join("│", parts.Select((x, i) => x.ToString()!.PadLeft(padding[i]))));
        }

        static (double timeInUs, int result) SpeedTest(Func<int> method, int iterations = 1000)
        {
            // Run once for JIT compilation, not needed for AOT compile
            method();

            int result = 0;

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                result = method();
            }

            var totalTime = ((double)sw.ElapsedTicks / Stopwatch.Frequency) * Math.Pow(10, 6) / iterations;

            return (totalTime, result);
        }
    }
}