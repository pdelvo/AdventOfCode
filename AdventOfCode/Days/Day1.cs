using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCode.Days
{
    public class Day1 : Day
    {
        string[] map = new string [0];

        public override string? Description => "Trebuchet?!";
        public override void Initialize(InstanceDownloader downloader)
        {
            base.Initialize(downloader);

            map = ["zero",
                "one",
                "two",
                "three",
                "four",
                "five",
                "six",
                "seven",
                "eight",
                "nine"];
        }

        public override int RunPart1()
        {
            int sum = 0;

            foreach (var line in Lines)
            {
                int lineValue = 0;

                for (int i = 0; i < line.Length; i++)
                {
                    if (char.IsDigit(line[i]))
                    {
                        lineValue += 10 * (line[i] - '0');
                        break;
                    }
                }

                for (int i = line.Length - 1; i >= 0; i--)
                {
                    if (char.IsDigit(line[i]))
                    {
                        lineValue += line[i] - '0';
                        break;
                    }
                }

                sum += lineValue;
            }

            return sum;
        }

        public override int RunPart2()
        {
            int sum = 0;
            foreach (var line in Lines)
            {
                var left = GetLeftValue(line);
                var right = GetRightValue(line);

                sum += 10 * left + right;
            }

            return sum;
        }

        int GetLeftValue(ReadOnlySpan<char> input)
        {
            while(input.Length > 0)
            {
                if (char.IsDigit(input[0]))
                {
                    return input[0] - '0';
                }
                else
                {
                    for (int j = 0; j < map.Length; j++)
                    {
                        if (input.StartsWith(map[j]))
                        {
                            return j;
                        }
                    }
                }

                input = input[1..];
            }

            return -1;
        }

        int GetRightValue(ReadOnlySpan<char> input)
        {
            while (input.Length > 0)
            {
                if (char.IsDigit(input[input.Length - 1]))
                {
                    return input[input.Length - 1] - '0';
                }
                else
                {
                    for (int j = 0; j < map.Length; j++)
                    {
                        if (input.EndsWith(map[j]))
                        {
                            return j;
                        }
                    }
                }

                input = input[.. (input.Length - 1)];
            }

            return -1;
        }
    }
}