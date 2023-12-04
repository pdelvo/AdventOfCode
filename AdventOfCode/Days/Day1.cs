using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace AdventOfCode.Days
{
    public class Day1 : IDay
    {
        public void RunPart1()
        {
            int sum = 0;

            foreach (var line in File.ReadAllLines("input/input1.txt"))
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

            Console.WriteLine("Day 1 Part 1: " + sum);
        }

        public void RunPart2()
        {
            Regex regex = new Regex("(?=(zero|one|two|three|four|five|six|seven|eight|nine|[0-9]))");
            int sum = 0;
            string[] map = ["zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];

            int Parse(string input) => input.Length == 1 ? input[0] - '0' : Array.IndexOf(map, input);
            foreach (var line in File.ReadAllLines("input/input1.txt"))
            {
                var matches = regex.Matches(line);
                var left = matches[0].Groups[1].Value;
                var right = matches[matches.Count - 1].Groups[1].Value;

                sum += 10 * Parse(left) + Parse(right);
            }

            Console.WriteLine($"Day 1 Part 2: {sum}");
        }
    }
}