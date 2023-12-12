using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCodeLib.Days
{
    public class Day2 : Day
    {
        public override string? Description => "Cube Conundrum";
        public override string TestInput1 => @"Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green";
        public override string TestOutput1 => "8";
        public override string TestOutput2 => "2286";
        public override string RunPart1()
        {
            int sum = 0;

            Span<Range> split = stackalloc Range[100];

            Dictionary<char, int> limit = new()
            {
                ['r'] = 12,
                ['g'] = 13,
                ['b'] = 14,
            };

            foreach (var line in Lines)
            {
                int colon = line.IndexOf(':');
                var after = line.AsSpan()[(colon + 1)..];
                int gameId = int.Parse(line.AsSpan()[5..colon]);
                bool success = true;


                while (after.Length > 0)
                {
                    int index = after.IndexOf(";");
                    if (index == -1)
                    {
                        index = after.Length;
                    }

                    var colorList = after[..index];

                    int splitCount = colorList.Split(split, ',');

                    var splitPart = split[..splitCount];
                    foreach (var part in splitPart)
                    {
                        var text = colorList[part][1..];

                        int whiteSpace = text.IndexOf(' ');

                        int count = int.Parse(text[..whiteSpace]);
                        var colorChar = text[(whiteSpace + 1)];

                        if (limit[colorChar] < count)
                        {
                            success = false;
                            break;
                        }
                    }

                    if (index + 1 > after.Length)
                    {
                        break;
                    }
                    after = after[(index + 1)..];
                }

                if (success)
                {
                    sum += gameId;
                }
            }

            return sum.ToString();
        }

        public override string RunPart2()
        {
            int sum = 0;

            Span<Range> split = stackalloc Range[100];

            foreach (var line in Lines)
            {
                int colon = line.IndexOf(':');
                var after = line.AsSpan()[(colon + 1)..];
                int gameId = int.Parse(line.AsSpan()[5..colon]);

                Dictionary<char, int> counter = new()
                {
                    ['r'] = 0,
                    ['g'] = 0,
                    ['b'] = 0,
                };

                while (after.Length > 0)
                {
                    int index = after.IndexOf(";");
                    if (index == -1)
                    {
                        index = after.Length;
                    }

                    var colorList = after[..index];

                    int splitCount = colorList.Split(split, ',', StringSplitOptions.RemoveEmptyEntries);

                    var splitPart = split[..splitCount];
                    foreach (var part in splitPart)
                    {
                        var text = colorList[part].Trim();

                        int whiteSpace = text.IndexOf(' ');

                        int count = int.Parse(text[..whiteSpace]);
                        var colorChar = text[(whiteSpace + 1)];

                        if (counter[colorChar] < count)
                        {
                            counter[colorChar] = count;
                        }
                    }

                    if (index + 1 > after.Length)
                    {
                        break;
                    }
                    after = after[(index + 1)..];
                }
                int product = counter.Values.Aggregate((x, y) => x * y);
                sum += product;
            }

            return sum.ToString();
        }
    }
}