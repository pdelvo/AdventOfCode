using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCode.Days
{
    public class Day2 : Day
    {
        public override string? Description => "Cube Conundrum";
        public override string RunPart1()
        {
            int sum = 0;
            Regex regex = new Regex("^Game (?<gameNumber>[0-9]+):(?:(?<colorList>(?: (?<color>[0-9]+ [A-Za-z]+),?)+);?)+$");
            Regex colorSplitRegex = new Regex("(?<count>[0-9]+) (?<color>[A-Za-z]+)");

            Span<Range> split = stackalloc Range[100];

            Dictionary<string, int> limit = new Dictionary<string, int>()
            {
                ["red"] = 12,
                ["green"] = 13,
                ["blue"] = 14,
            };

            foreach (var line in Lines)
            {
                var match = regex.Match(line);

                if (match.Success)
                {
                    int gameId = int.Parse(match.Groups["gameNumber"].ValueSpan);
                    bool success = true;

                    foreach (Capture capture in match.Groups["colorList"].Captures)
                    {
                        var colorList = capture.ValueSpan;

                        int splitCount = colorList.Split(split, ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);

                        var splitPart = split[..splitCount];
                        foreach (var part in splitPart)
                        {
                            var text = colorList[part].Trim();

                            int whiteSpace = text.IndexOf(' ');

                            int count = int.Parse(text[..whiteSpace]);
                            var color = text[(whiteSpace + 1)..];

                            if (limit[color.ToString()] < count)
                            {
                                success = false;
                                break;
                            }
                        }
                    }

                    if (success)
                    {
                        sum += gameId;
                    }
                }
            }

            return sum.ToString();
        }

        public override string RunPart2()
        {
            int sum = 0;
            Regex regex = new Regex("^Game (?<gameNumber>[0-9]+):(?:(?<colorList>(?: (?<color>[0-9]+ [A-Za-z]+),?)+);?)+$");
            Regex colorSplitRegex = new Regex("(?<count>[0-9]+) (?<color>[A-Za-z]+)");

            Span<Range> split = stackalloc Range[100];

            foreach (var line in Lines)
            {
                var match = regex.Match(line);

                Dictionary<string, int> counter = new Dictionary<string, int>()
                {
                    ["red"] = 0,
                    ["green"] = 0,
                    ["blue"] = 0,
                };

                if (match.Success)
                {
                    int gameId = int.Parse(match.Groups["gameNumber"].ValueSpan);

                    foreach (Capture capture in match.Groups["colorList"].Captures)
                    {
                        var colorList = capture.ValueSpan;

                        int splitCount = colorList.Split(split, ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);

                        var splitPart = split[..splitCount];
                        foreach (var part in splitPart)
                        {
                            var text = colorList[part].Trim();

                            int whiteSpace = text.IndexOf(' ');

                            int count = int.Parse(text[..whiteSpace]);
                            var color = text[(whiteSpace + 1)..];

                            if (counter[color.ToString()] < count)
                            {
                                counter[color.ToString()] = count;
                            }
                        }
                    }
                    int product = counter.Values.Aggregate((x, y) => x * y);
                    sum += product;
                }
            }

            return sum.ToString();
        }
    }
}